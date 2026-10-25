using System.Buffers.Binary;
using WinForms.Audio.Music;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;
using WinForms.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Verifies offline WAV timing, playback parity, range selection, tails and failure-safe publication.
/// </summary>
public sealed class PercussionWaveExportTests
{
    /// <summary>
    ///  Repetitions use one fractional sample clock and match the live calibrated mix sample for sample.
    /// </summary>
    [Fact]
    public async Task RepeatedLoops_MatchLiveRendererWithoutRoundingDriftOrNormalization()
    {
        using LoopTestFiles files = new();
        PercussionScore score = new(2,
        [
            new(0, 0, Cr78Instrument.BassDrum),
            new(0, 4, Cr78Instrument.SnareDrum, .8f),
            new(0, 15, Cr78Instrument.Cymbal, .6f),
            new(1, 0, Cr78Instrument.HiHat),
            new(1, 7, Cr78Instrument.Guiro2, .7f, 18),
            new(1, 15, Cr78Instrument.BassDrumAccent)
        ]);
        float[] volumes = Enumerable.Range(0, Cr78Kit.Instruments.Count).Select(i => .3f + i * .03f).ToArray();
        PercussionWaveExportOptions options = new(score, new Tempo(137))
        {
            SampleRate = 32_000,
            Repetitions = 3,
            MasterVolume = .7f,
            InstrumentVolumes = volumes,
            MetallicEnabled = true,
            MetallicLevel = .6f
        };
        string path = files.File("three-loops.wav");
        await PercussionWaveExporter.ExportAsync(path, options, TestContext.Current.CancellationToken);
        byte[] wav = await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        int frames = checked((int)PercussionClock.RoundFrame(32_000 * 240m / 137 * 2 * 3));
        AssertHeader(wav, options.SampleRate, frames);
        Assert.NotEqual(frames, PercussionClock.RoundFrame(32_000 * 240m / 137 * 2) * 3);
        PercussionMailbox mailbox = new(score, options.Tempo);
        mailbox.ApplyConfiguration(score, options.Tempo, options.MasterVolume, volumes,
            loop: true, options.MetallicEnabled, options.MetallicLevel);
        PercussionRenderer live = new(options.SampleRate, mailbox, 42);
        mailbox.Start();
        for (int frame = 0; frame < frames; frame++)
        {
            if (frame % 1_024 == 0)
            {
                live.BeginBlock(frame);
            }

            short expected = (short)Math.Clamp(
                live.Next(frame) * short.MaxValue, short.MinValue, short.MaxValue);
            Assert.Equal(expected, ReadLeft(wav, frame));
            Assert.Equal(expected, ReadRight(wav, frame));
        }

        Assert.Equal(score.Hits.Count * options.Repetitions, live.RenderedHitCount);
    }

    /// <summary>
    ///  A selected bar starts at frame zero and ignores hits in preceding/following bars.
    /// </summary>
    [Fact]
    public async Task SelectedBar_IsRebasedAndRetainsTheEntireEmptyRemainder()
    {
        using LoopTestFiles files = new();
        PercussionScore source = new(3,
        [
            new(0, 0, Cr78Instrument.Cymbal),
            new(1, 3, Cr78Instrument.RimShot),
            new(2, 0, Cr78Instrument.Cowbell)
        ]);
        string selected = files.File("bar.wav");
        string isolated = files.File("isolated.wav");
        await PercussionWaveExporter.ExportAsync(selected,
            new(source, new Tempo(240)) { FirstBar = 1, BarCount = 1 },
            TestContext.Current.CancellationToken);
        await PercussionWaveExporter.ExportAsync(isolated,
            new(new PercussionScore(1, [new(0, 3, Cr78Instrument.RimShot)]), new Tempo(240)),
            TestContext.Current.CancellationToken);
        byte[] wav = await File.ReadAllBytesAsync(selected, TestContext.Current.CancellationToken);
        Assert.Equal(await File.ReadAllBytesAsync(isolated, TestContext.Current.CancellationToken), wav);
        AssertHeader(wav, 48_000, 48_000);
        Assert.All(wav.AsSpan(44, 9_000 * 4).ToArray(), value => Assert.Equal(0, value));
        Assert.Contains(wav.AsSpan(44 + 9_000 * 4, 2_400 * 4).ToArray(), value => value != 0);
        Assert.All(wav.AsSpan(wav.Length - 4_000).ToArray(), value => Assert.Equal(0, value));
    }

    /// <summary>
    ///  Imported hits cross repeat boundaries without reset; final tails are explicit, not automatic padding.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ImportedSounds_CrossLoopBoundariesAndOptionallyFinishTheirNaturalTail(bool includeTail)
    {
        using LoopTestFiles files = new();
        Sample sample = Sample.FromPcm(Enumerable.Repeat((short)12_000, 48_000).ToArray(), 48_000);
        PercussionWaveExportOptions options = new(
            new PercussionScore(1, [new(0, 15, Cr78Instrument.BassDrum)]), new Tempo(240))
        {
            Repetitions = 2,
            IncludeTail = includeTail,
            Samples = new Dictionary<Cr78Instrument, Sample> { [Cr78Instrument.BassDrum] = sample }
        };
        string path = files.File("import.wav");
        await PercussionWaveExporter.ExportAsync(path, options, TestContext.Current.CancellationToken);
        byte[] wav = await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        int frames = includeTail ? 93_000 + 48_000 + 1 : 96_000;
        AssertHeader(wav, 48_000, frames);
        Assert.All(wav.AsSpan(44, 45_000 * 4).ToArray(), value => Assert.Equal(0, value));
        Assert.True(ReadLeft(wav, 47_999) > 0);
        Assert.Equal(ReadLeft(wav, 47_999), ReadLeft(wav, 48_000));
        Assert.True(ReadLeft(wav, 95_999) > 0);
        if (includeTail)
        {
            Assert.True(ReadLeft(wav, frames - 2) > 0);
            Assert.Equal(0, ReadLeft(wav, frames - 1));
        }
    }

    /// <summary>
    ///  Exact zero master remains digital silence, and adding a tail does not extend an empty score.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SilentScores_AreNotNormalizedOrPadded(bool empty)
    {
        using LoopTestFiles files = new();
        PercussionScore score = new(1, empty ? [] : [new(0, 0, Cr78Instrument.BassDrum)]);
        string path = files.File("silence.wav");
        await PercussionWaveExporter.ExportAsync(path,
            new(score, new Tempo(240)) { MasterVolume = 0, IncludeTail = true },
            TestContext.Current.CancellationToken);
        byte[] wav = await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        AssertHeader(wav, 48_000, 48_000);
        Assert.All(wav.AsSpan(44).ToArray(), value => Assert.Equal(0, value));
    }

    /// <summary>
    ///  Appending a tail neither retriggers the first beat nor forces an early guiro release.
    /// </summary>
    [Fact]
    public async Task FinalTail_MatchesNaturalOneShotPlaybackIncludingLongGates()
    {
        using LoopTestFiles files = new();
        PercussionScore score = new(1,
        [
            new(0, 0, Cr78Instrument.Cowbell),
            new(0, 15, Cr78Instrument.Guiro2, .7f, GateSteps: 24)
        ]);
        Tempo tempo = new(240);
        string path = files.File("gated-tail.wav");
        await PercussionWaveExporter.ExportAsync(path,
            new(score, tempo) { IncludeTail = true }, TestContext.Current.CancellationToken);
        byte[] wav = await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        int frames = (wav.Length - 44) / 4;
        Assert.True(frames > 48_000 * 2);
        PercussionMailbox mailbox = new(score, tempo);
        mailbox.SetLoop(false);
        PercussionRenderer live = new(48_000, mailbox, 42);
        mailbox.Start();
        for (int frame = 0; frame < frames; frame++)
        {
            if (frame % 1_024 == 0 || frame == 48_000)
            {
                live.BeginBlock(frame);
            }

            short expected = (short)Math.Clamp(
                live.Next(frame) * short.MaxValue, short.MinValue, short.MaxValue);
            Assert.Equal(expected, ReadLeft(wav, frame));
        }

        Assert.Equal(2, live.RenderedHitCount);
        Assert.False(live.HasActiveSounds);
        Assert.False(mailbox.IsPlaying);
    }

    /// <summary>
    ///  Caller mutations after admission cannot change the sound or levels being rendered in the worker.
    /// </summary>
    [Fact]
    public async Task Export_CopiesMutableSamplesAndFadersBeforeReturning()
    {
        using LoopTestFiles files = new();
        Sample sample = Sample.FromPcm(Enumerable.Repeat((short)12_000, 480).ToArray(), 48_000);
        float[] volumes = Enumerable.Repeat(1f, Cr78Kit.Instruments.Count).ToArray();
        Dictionary<Cr78Instrument, Sample> samples = new() { [Cr78Instrument.BassDrum] = sample };
        PercussionWaveExportOptions options = new(
            new PercussionScore(1, [new(0, 0, Cr78Instrument.BassDrum)]), new Tempo(240))
        {
            InstrumentVolumes = volumes,
            Samples = samples
        };
        Cr78Generator reference = new(48_000, Cr78Instrument.BassDrum,
            (42u ^ 0x9E3779B9u) | 1u);
        reference.SetSample((float[])sample.Data.Clone());
        reference.Trigger(1, 3_000);
        float expected = 0;
        for (int frame = 0; frame <= 100; frame++)
        {
            expected = reference.Next() * Cr78VoiceBank.PercussionOutputGain;
        }

        string path = files.File("snapshot.wav");
        Task export = PercussionWaveExporter.ExportAsync(path, options, TestContext.Current.CancellationToken);
        Array.Fill(sample.Data, float.NaN);
        Array.Fill(volumes, 0);
        samples.Clear();
        await export;
        byte[] wav = await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        Assert.True(expected > 0);
        Assert.Equal((short)(expected * short.MaxValue), ReadLeft(wav, 100));
        Assert.Equal(0, ReadLeft(wav, 481));
    }

    /// <summary>
    ///  Validation and resource bounds fail before touching an existing file.
    /// </summary>
    [Fact]
    public void InvalidRequests_DoNotTouchExistingOutput()
    {
        using LoopTestFiles files = new();
        string path = files.File("existing.wav");
        File.WriteAllText(path, "original");
        PercussionScore score = new(2, []);
        PercussionWaveExportOptions[] invalid =
        [
            new(score, new Tempo(120)) { FirstBar = -1 },
            new(score, new Tempo(120)) { FirstBar = 2 },
            new(score, new Tempo(120)) { FirstBar = 1, BarCount = 2 },
            new(score, new Tempo(120)) { BarCount = 0 },
            new(score, new Tempo(120)) { Repetitions = 0 },
            new(score, new Tempo(120)) { Repetitions = 10_001 },
            new(score, new Tempo(1)) { Repetitions = 8 },
            new(score, new Tempo(120)) { SampleRate = 8_000 },
            new(score, new Tempo(120)) { MasterVolume = float.NaN },
            new(score, new Tempo(120)) { InstrumentVolumes = [] }
        ];
        foreach (PercussionWaveExportOptions options in invalid)
        {
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                _ = PercussionWaveExporter.ExportAsync(path, options, TestContext.Current.CancellationToken);
            });
            Assert.Equal("original", File.ReadAllText(path));
        }

        Assert.Single(Directory.GetFiles(files.DirectoryPath));
    }

    /// <summary>
    ///  Cancellation and failed publication preserve the old destination and clean owned temporary files.
    /// </summary>
    [Fact]
    public async Task CancelledOrFailedExport_PreservesDestinationAndCleansTemporaryFile()
    {
        using LoopTestFiles files = new();
        string path = files.File("existing.wav");
        await File.WriteAllTextAsync(path, "original", TestContext.Current.CancellationToken);
        PercussionWaveExportOptions options = new(new PercussionScore(1, []), new Tempo(240));
        using CancellationTokenSource cancellation = new();
        Task export = PercussionWaveExporter.ExportAsync(path, options, cancellation.Token);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => export);
        Assert.Equal("original", await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken));
        Assert.Single(Directory.GetFiles(files.DirectoryPath));

        using (FileStream held = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            Exception? error = await Record.ExceptionAsync(() =>
                PercussionWaveExporter.ExportAsync(path, options, TestContext.Current.CancellationToken));
            Assert.True(error is IOException or UnauthorizedAccessException, error?.ToString());
        }

        Assert.Equal("original", await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken));
        Assert.Single(Directory.GetFiles(files.DirectoryPath));
        await PercussionWaveExporter.ExportAsync(path, options, TestContext.Current.CancellationToken);
        AssertHeader(await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken), 48_000, 48_000);
    }

    private static short ReadLeft(byte[] wav, int frame)
        => BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(44 + frame * 4));

    private static short ReadRight(byte[] wav, int frame)
        => BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(46 + frame * 4));

    private static void AssertHeader(byte[] wav, int rate, int frames)
    {
        Assert.Equal("RIFF"u8.ToArray(), wav[..4]);
        Assert.Equal("WAVE"u8.ToArray(), wav[8..12]);
        Assert.Equal(wav.Length - 8, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(4)));
        Assert.Equal(1, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(20)));
        Assert.Equal(2, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(22)));
        Assert.Equal(rate, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(24)));
        Assert.Equal(rate * 4, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(28)));
        Assert.Equal(4, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(32)));
        Assert.Equal(16, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(34)));
        Assert.Equal(frames * 4, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(40)));
        Assert.Equal(44 + frames * 4, wav.Length);
    }
}
