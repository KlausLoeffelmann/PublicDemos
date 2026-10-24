using System.Buffers.Binary;
using System.Text.Json.Nodes;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Music;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;
using WinForms.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Exercises portable recorded-sound mappings without depending on reference WAVs or an endpoint.
/// </summary>
public sealed class PercussionSoundPresetTests
{
    /// <summary>
    ///  All and only the fixed tracks can be mapped; caller dictionaries cannot mutate a preset.
    /// </summary>
    [Fact]
    public void Preset_PreservesFixedTracksAndOwnsItsMapping()
    {
        using LoopTestFiles files = new();
        Dictionary<Cr78Instrument, string> mapping = new() { [Cr78Instrument.BassDrum] = files.File("kick.wav") };
        PercussionSoundPreset preset = new("My kit", mapping);
        mapping.Clear();
        Assert.Single(preset.WaveFiles);
        Assert.Empty(PercussionSoundPreset.Stock.WaveFiles);
        Assert.Empty(preset.WithoutWaveFile(Cr78Instrument.BassDrum).WaveFiles);
        Assert.Single(preset.WithName("Renamed").WaveFiles);
        Assert.Throws<NotSupportedException>(() =>
            ((IDictionary<Cr78Instrument, string>)preset.WaveFiles).Clear());
        Assert.Throws<ArgumentOutOfRangeException>(() => preset.WithWaveFile(Cr78Instrument.MetallicBeat, files.File("a.wav")));
        Assert.Throws<ArgumentOutOfRangeException>(() => preset.WithWaveFile(Cr78Instrument.Claves, files.File("a.wav")));
        Assert.Throws<ArgumentException>(() => preset.WithWaveFile(Cr78Instrument.Cymbal, "relative.wav"));
        Assert.Throws<ArgumentException>(() => new PercussionSoundPreset(" "));
    }

    /// <summary>
    ///  Saved original WAV bytes and relative paths survive moving the kit and deleting its imports.
    /// </summary>
    [Fact]
    public async Task Store_CreatesPortableSelfContainedPresets()
    {
        using LoopTestFiles files = new();
        string source = files.File("import.wav");
        byte[] original = TestWave();
        await File.WriteAllBytesAsync(source, original, TestContext.Current.CancellationToken);
        string bundle = files.File("bundle");
        string manifest = Path.Combine(bundle, "Custom.drumkit.json");
        PercussionSoundPreset preset = new("Custom", new Dictionary<Cr78Instrument, string>
        {
            [Cr78Instrument.SnareDrumAccent] = source,
            [Cr78Instrument.Guiro2] = source
        });
        await PercussionSoundPresetStore.SaveAsync(preset, manifest, TestContext.Current.CancellationToken);
        string json = await File.ReadAllTextAsync(manifest, TestContext.Current.CancellationToken);
        Assert.DoesNotContain(source, json);
        File.Delete(source);
        string moved = files.File("moved");
        Directory.Move(bundle, moved);
        PercussionSoundPreset loaded = await PercussionSoundPresetStore.LoadAsync(
            Path.Combine(moved, "Custom.drumkit.json"), TestContext.Current.CancellationToken);
        Assert.Equal("Custom", loaded.Name);
        Assert.Equal(2, loaded.WaveFiles.Count);
        foreach (string path in loaded.WaveFiles.Values)
        {
            Assert.StartsWith(moved, path);
            Assert.Equal(original, await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken));
        }

        IReadOnlyDictionary<Cr78Instrument, Sample> samples =
            await loaded.PrepareAsync(44_100, TestContext.Current.CancellationToken);
        Assert.Equal(2, samples.Count);
        Assert.All(samples.Values, sample => Assert.Equal(44_100, sample.SampleRate));
        Assert.All(samples.Values, sample => Assert.Equal(441, sample.Data.Length));
    }

    /// <summary>
    ///  Missing or invalid imports never replace a valid manifest or leave an unpublished asset folder.
    /// </summary>
    [Fact]
    public async Task FailedSave_PreservesExistingPresetAndCleansOnlyOwnedAssets()
    {
        using LoopTestFiles files = new();
        string source = files.File("valid.wav");
        await File.WriteAllBytesAsync(source, TestWave(), TestContext.Current.CancellationToken);
        string manifest = files.File("Saved.drumkit.json");
        PercussionSoundPreset original = PercussionSoundPreset.Stock.WithWaveFile(Cr78Instrument.Cowbell, source);
        await PercussionSoundPresetStore.SaveAsync(original, manifest, TestContext.Current.CancellationToken);
        byte[] previous = await File.ReadAllBytesAsync(manifest, TestContext.Current.CancellationToken);
        string[] folders = Directory.GetDirectories(files.DirectoryPath);
        string invalid = files.File("invalid.wav");
        await File.WriteAllTextAsync(invalid, "not a WAV", TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<FormatException>(() => PercussionSoundPresetStore.SaveAsync(
            original.WithWaveFile(Cr78Instrument.BassDrum, invalid), manifest, TestContext.Current.CancellationToken));
        Assert.Equal(previous, await File.ReadAllBytesAsync(manifest, TestContext.Current.CancellationToken));
        Assert.Equal(folders, Directory.GetDirectories(files.DirectoryPath));

        using (FileStream held = new(manifest, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            await Assert.ThrowsAnyAsync<IOException>(() => PercussionSoundPresetStore.SaveAsync(
                original, manifest, TestContext.Current.CancellationToken));
        }

        Assert.Equal(folders, Directory.GetDirectories(files.DirectoryPath));
        Assert.Equal(previous, await File.ReadAllBytesAsync(manifest, TestContext.Current.CancellationToken));
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            PercussionSoundPresetStore.SaveAsync(original, manifest, cancellation.Token));
    }

    /// <summary>
    ///  Rejects missing assets, path traversal, duplicate tracks and malformed PCM explicitly.
    /// </summary>
    [Theory]
    [InlineData("escape")]
    [InlineData("absolute")]
    [InlineData("duplicate")]
    [InlineData("unknown")]
    [InlineData("missing")]
    public async Task Load_RejectsInvalidOrIncompleteSoundMappings(string issue)
    {
        using LoopTestFiles files = new();
        string source = files.File("source.wav");
        await File.WriteAllBytesAsync(source, TestWave(), TestContext.Current.CancellationToken);
        string manifest = files.File("Saved.drumkit.json");
        await PercussionSoundPresetStore.SaveAsync(
            PercussionSoundPreset.Stock.WithWaveFile(Cr78Instrument.BassDrum, source), manifest,
            TestContext.Current.CancellationToken);
        JsonObject root = JsonNode.Parse(await File.ReadAllTextAsync(manifest, TestContext.Current.CancellationToken))!.AsObject();
        JsonArray sounds = root["sounds"]!.AsArray();
        switch (issue)
        {
            case "escape": sounds[0]!["file"] = "..\\outside.wav"; break;
            case "absolute": sounds[0]!["file"] = source; break;
            case "duplicate": sounds.Add(sounds[0]!.DeepClone()); break;
            case "unknown": sounds[0]!["instrument"] = "MetallicBeat"; break;
            case "missing": sounds[0]!["file"] = "missing.wav"; break;
        }

        await File.WriteAllTextAsync(manifest, root.ToJsonString(), TestContext.Current.CancellationToken);
        if (issue == "missing")
        {
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                PercussionSoundPresetStore.LoadAsync(manifest, TestContext.Current.CancellationToken));
        }
        else
        {
            await Assert.ThrowsAsync<InvalidDataException>(() =>
                PercussionSoundPresetStore.LoadAsync(manifest, TestContext.Current.CancellationToken));
        }
    }

    /// <summary>
    ///  Sample overrides own PCM, apply only at a block boundary, and leave the score revision unchanged.
    /// </summary>
    [Fact]
    public void Samples_AreImmutableAndSwitchOnTheRenderingThread()
    {
        Sample sample = Sample.FromPcm(Enumerable.Repeat((short)12000, 480).ToArray(), 48_000);
        PercussionSampleSet set = new(new Dictionary<Cr78Instrument, Sample>
        {
            [Cr78Instrument.BassDrum] = sample
        }, 48_000);
        Array.Fill(sample.Data, float.NaN);
        PercussionMailbox mailbox = new(new PercussionScore(1, []), new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        long revision = mailbox.Settings.Revision;
        mailbox.SetSamples(set);
        mailbox.Audition(Cr78Instrument.BassDrum, 1f);
        renderer.BeginBlock(0);
        for (int frame = 0; frame < 480; frame++)
        {
            Assert.InRange(renderer.Next(frame),
                0.03f * Cr78VoiceBank.PercussionOutputGain, 0.04f * Cr78VoiceBank.PercussionOutputGain);
        }

        Assert.Equal(0, renderer.Next(480));
        Assert.Equal(0, renderer.Next(481));
        Assert.Equal(revision, mailbox.Settings.Revision);
        mailbox.SetSamples(PercussionSampleSet.Stock);
        mailbox.Audition(Cr78Instrument.BassDrum, 1f);
        Assert.Equal(0, renderer.Next(482));
        renderer.BeginBlock(483);
        bool heardNegative = false;
        for (int frame = 483; frame < 4000; frame++)
        {
            heardNegative |= renderer.Next(frame) < 0;
        }

        Assert.True(heardNegative);
        Assert.Equal(revision, mailbox.Settings.Revision);
    }

    /// <summary>
    ///  Rejects unsupported channels and malformed chunks before division, slicing, or large allocation.
    /// </summary>
    [Theory]
    [InlineData("truncated")]
    [InlineData("format-short")]
    [InlineData("bad-bits")]
    [InlineData("bad-rate")]
    [InlineData("zero-channels")]
    [InlineData("bad-alignment")]
    [InlineData("nan-float")]
    public async Task WavDecoder_RejectsMalformedImports(string issue)
    {
        byte[] wav = TestWave();
        switch (issue)
        {
            case "truncated": wav = wav[..^1]; break;
            case "format-short": BinaryPrimitives.WriteInt32LittleEndian(wav.AsSpan(16), 2); break;
            case "bad-bits": BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(34), 0); break;
            case "bad-rate": BinaryPrimitives.WriteInt32LittleEndian(wav.AsSpan(24), 0); break;
            case "zero-channels": BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(22), 0); break;
            case "bad-alignment": BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(32), 0); break;
            case "nan-float":
                BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(20), 3);
                BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(34), 32);
                BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(32), 4);
                BinaryPrimitives.WriteSingleLittleEndian(wav.AsSpan(44), float.NaN);
                break;
        }

        using MemoryStream stream = new(wav);
        Exception? error = await Record.ExceptionAsync(() =>
            Sample.FromWaveStreamAsync(stream, cancellationToken: TestContext.Current.CancellationToken));
        Assert.True(error is FormatException or NotSupportedException or ArgumentException, error?.ToString());
    }

    /// <summary>
    ///  Decodes the advertised non-16-bit imports with correct signed and floating-point amplitudes.
    /// </summary>
    [Theory]
    [InlineData(8)]
    [InlineData(24)]
    [InlineData(32)]
    public async Task WavDecoder_AcceptsPcmAndFloatFormats(int bits)
    {
        int dataBytes = bits == 8 ? 2 : bits == 24 ? 6 : 8;
        byte[] wav = WaveFile.ToWavBytes(new short[dataBytes / 2], 48_000);
        BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(34), (short)bits);
        BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(32), (short)(bits / 8));
        BinaryPrimitives.WriteInt32LittleEndian(wav.AsSpan(28), 48_000 * bits / 8);
        if (bits == 8)
        {
            wav[44] = 192;
            wav[45] = 64;
        }
        else if (bits == 24)
        {
            wav[46] = 0x40;
            wav[49] = 0xC0;
        }
        else
        {
            BinaryPrimitives.WriteInt16LittleEndian(wav.AsSpan(20), 3);
            BinaryPrimitives.WriteSingleLittleEndian(wav.AsSpan(44), .5f);
            BinaryPrimitives.WriteSingleLittleEndian(wav.AsSpan(48), -.5f);
        }

        using MemoryStream stream = new(wav);
        Sample sample = await Sample.FromWaveStreamAsync(stream, 48_000, TestContext.Current.CancellationToken);
        Assert.Equal([.5f, -.5f], sample.Data);
    }

    /// <summary>
    ///  Imports ordinary WAVE_FORMAT_EXTENSIBLE PCM rather than treating its GUID as compressed audio.
    /// </summary>
    [Fact]
    public async Task WavDecoder_AcceptsExtensiblePcmAndStereoDownmix()
    {
        byte[] pcm = WaveFile.ToWavBytes([16384, 0, -16384, 0], 48_000, channels: 2);
        byte[] extensible = new byte[pcm.Length + 24];
        pcm.AsSpan(0, 36).CopyTo(extensible);
        pcm.AsSpan(36).CopyTo(extensible.AsSpan(60));
        BinaryPrimitives.WriteInt32LittleEndian(extensible.AsSpan(4), extensible.Length - 8);
        BinaryPrimitives.WriteInt32LittleEndian(extensible.AsSpan(16), 40);
        BinaryPrimitives.WriteUInt16LittleEndian(extensible.AsSpan(20), 0xFFFE);
        BinaryPrimitives.WriteInt16LittleEndian(extensible.AsSpan(36), 22);
        BinaryPrimitives.WriteInt16LittleEndian(extensible.AsSpan(38), 16);
        new Guid("00000001-0000-0010-8000-00aa00389b71").TryWriteBytes(extensible.AsSpan(44));
        using MemoryStream stream = new(extensible);
        Sample sample = await Sample.FromWaveStreamAsync(stream, 48_000, TestContext.Current.CancellationToken);
        Assert.Equal([.25f, -.25f], sample.Data);
    }

    private static byte[] TestWave()
        => WaveFile.ToWavBytes(WaveFile.Sine(440, 48_000, 0.01, 0.25), 48_000);
}
