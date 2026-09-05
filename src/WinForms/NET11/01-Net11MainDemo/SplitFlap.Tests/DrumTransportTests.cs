using System.Reflection;
using WinForms.Audio.Music;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Exercises paused musical time, ordered document resets, and delayed history without an endpoint.
/// </summary>
public sealed class DrumTransportTests
{
    /// <summary>
    ///  Repeated unequal pauses shift only wall-clock time, never rounding a musical remainder.
    /// </summary>
    [Theory]
    [InlineData(32_000, 73)]
    [InlineData(44_100, 137)]
    [InlineData(44_100, 136)]
    [InlineData(48_000, 119)]
    [InlineData(96_000, 997)]
    [InlineData(192_000, 239)]
    public void PauseResume_PreservesEveryFractionalOnset(int sampleRate, int bpm)
    {
        PercussionScore score = new(1, Enumerable.Range(0, 16)
            .Select(step => new PercussionHit(0, step, Cr78Instrument.Claves, 0.3f)));
        List<long> onsets = [];
        PercussionMailbox mailbox = new(score, new Tempo(bpm));
        PercussionRenderer renderer = new(sampleRate, mailbox, 42, (frame, _) => onsets.Add(frame));
        const long origin = 12_345;
        long cursor = origin;
        long totalPaused = 0;
        decimal stepFrames = sampleRate * 15m / bpm;
        mailbox.Start();
        for (int step = 0; step < 64; step++)
        {
            decimal ideal = origin + stepFrames * step + totalPaused;
            long onset = PercussionClock.RoundFrame(ideal);
            Render(renderer, cursor, onset + 1);
            Assert.Equal(step + 1, onsets.Count);
            Assert.InRange(Math.Abs(onsets[^1] - ideal), 0m, 1m);
            long pauseFrame = onset + Math.Max(2, (long)(stepFrames * (step % 5 + 1) / 7));
            Render(renderer, onset + 1, pauseFrame);
            mailbox.Pause();
            int duration = 137 + step * 83 % 701;
            Render(renderer, pauseFrame, pauseFrame + duration);
            Assert.Equal(step + 1, onsets.Count);
            DrumPlaybackSnapshot held = renderer.GetSnapshot(pauseFrame + duration, true);
            Assert.Equal(DrumTransportState.Paused, held.State);
            Assert.True(held.IsPaused);
            Assert.False(held.IsPlaying);
            Assert.Equal(step % 16, held.Step);
            mailbox.Start();
            totalPaused += duration;
            cursor = pauseFrame + duration;
        }
    }

    /// <summary>
    ///  Pausing immediately before, at, or after an onset neither skips nor duplicates that hit.
    /// </summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void PauseAtOnset_LeavesExactlyTheUnconsumedEvents(int offset)
    {
        PercussionScore score = new(1,
            [new(0, 0, Cr78Instrument.Claves), new(0, 1, Cr78Instrument.Claves), new(0, 2, Cr78Instrument.Claves)]);
        List<long> onsets = [];
        PercussionMailbox mailbox = new(score, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42, (frame, _) => onsets.Add(frame));
        mailbox.Start();
        long pauseFrame = 6_000 + offset;
        Render(renderer, 0, pauseFrame);
        mailbox.Pause();
        Render(renderer, pauseFrame, pauseFrame + 10_000);
        mailbox.Start();
        Render(renderer, pauseFrame + 10_000, 22_001);
        Assert.Equal(new long[] { 0, offset > 0 ? 6_000 : 16_000, 22_000 }, onsets);
    }

    /// <summary>
    ///  Pause releases rather than freezing a waveform; silent time and audition do not advance score hits.
    /// </summary>
    [Fact]
    public void PausedRenderer_ReleasesTailsButKeepsAuditionsAndHeldHistory()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1,
            [new(0, 0, Cr78Instrument.Cymbal), new(0, 1, Cr78Instrument.Claves)]), new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        mailbox.Start();
        Assert.True(Render(renderer, 0, 1_000) > 0);
        mailbox.Pause();
        Assert.Equal(DrumTransportState.Paused, mailbox.State);
        Assert.Equal(DrumTransportState.Playing, renderer.GetSnapshot(1_000, true).State);
        Assert.True(Render(renderer, 1_000, 1_240) > 0);
        Assert.Equal(0, Render(renderer, 1_240, 100_000));
        Assert.Equal(1, renderer.RenderedHitCount);
        Assert.Equal(DrumTransportState.Paused, renderer.GetSnapshot(100_000, true).State);
        Assert.Equal(0, renderer.GetSnapshot(100_000, true).Step);

        mailbox.Audition(Cr78Instrument.Cowbell, 0.7f);
        Assert.True(Render(renderer, 100_000, 101_000) > 0);
        Assert.Equal(1, renderer.RenderedHitCount);
        Assert.Equal(DrumTransportState.Paused, mailbox.State);
        mailbox.Stop();
        Render(renderer, 101_000, 102_000);
        Assert.Equal(0, Render(renderer, 102_000, 103_000));
        Assert.False(renderer.IsFinished);
    }

    /// <summary>
    ///  Score/tempo edits during a held bar wait until the first bar boundary after resumption.
    /// </summary>
    [Fact]
    public void PausedConfiguration_WaitsForTheRebasedBarBoundary()
    {
        PercussionScore original = new(1,
        [
            new(0, 0, Cr78Instrument.BassDrum),
            new(0, 8, Cr78Instrument.Claves),
            new(0, 15, Cr78Instrument.SnareDrum)
        ]);
        PercussionScore edited = new(2,
            [new(1, 0, Cr78Instrument.HiHat), new(1, 1, Cr78Instrument.HiHat)]);
        List<(long Frame, Cr78Instrument Instrument)> onsets = [];
        PercussionMailbox mailbox = new(original, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42,
            (frame, hit) => onsets.Add((frame, hit.Instrument)));
        mailbox.Start();
        Render(renderer, 0, 13_000);
        mailbox.Pause();
        Render(renderer, 13_000, 15_000);
        mailbox.ApplyConfiguration(edited, new Tempo(240), 0.5f, UnityVolumes(), true, false, 0.6f);
        Render(renderer, 15_000, 113_000);
        DrumPlaybackSnapshot held = renderer.GetSnapshot(113_000, true);
        Assert.True(held.IsPaused);
        Assert.True(held.HasPendingChanges);
        Assert.Equal(2, held.Step);
        mailbox.Start();
        Render(renderer, 113_000, 199_002);

        Assert.Equal(new long[] { 0, 148_000, 190_000, 196_000, 199_000 }, onsets.Select(hit => hit.Frame));
        Assert.Equal(Cr78Instrument.Claves, onsets[1].Instrument);
        Assert.Equal(Cr78Instrument.SnareDrum, onsets[2].Instrument);
        Assert.Equal(Cr78Instrument.HiHat, onsets[3].Instrument);
        Assert.True(renderer.GetSnapshot(196_000, true).HasPendingChanges);
        DrumPlaybackSnapshot nextBar = renderer.GetSnapshot(196_001, true);
        Assert.Equal(1, nextBar.Bar);
        Assert.Equal(0, nextBar.Step);
        Assert.False(nextBar.HasPendingChanges);
        Assert.True(renderer.GetSnapshot(100_000, true).IsPaused);
    }

    /// <summary>
    ///  Explicit Stop rewinds both playing and paused scores without rewriting previously queued output.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void StopReset_RewindsOnlyAtItsCompletedOutputBoundary(bool pauseFirst)
    {
        PercussionMailbox mailbox = new(new PercussionScore(2,
            [new(0, 0, Cr78Instrument.Claves), new(1, 0, Cr78Instrument.Claves)]), new Tempo(1_000));
        List<long> onsets = [];
        PercussionRenderer renderer = new(32_000, mailbox, 42, (frame, _) => onsets.Add(frame));
        mailbox.Start();
        Render(renderer, 0, 8_200);
        if (pauseFirst)
        {
            mailbox.Pause();
        }

        Render(renderer, 8_200, 9_000);
        mailbox.Stop();
        Assert.Equal(DrumTransportState.Stopped, mailbox.State);
        Render(renderer, 9_000, 10_000);
        DrumPlaybackSnapshot old = renderer.GetSnapshot(9_000, true);
        Assert.Equal(1, old.Bar);
        Assert.Equal(pauseFirst ? DrumTransportState.Paused : DrumTransportState.Playing, old.State);
        DrumPlaybackSnapshot stopped = renderer.GetSnapshot(9_001, true);
        Assert.Equal(DrumTransportState.Stopped, stopped.State);
        Assert.Equal(0, stopped.Bar);
        Assert.Equal(0, stopped.Step);
        mailbox.Start();
        Render(renderer, 10_000, 10_001);
        Assert.Equal(new long[] { 0, 7_680, 10_000 }, onsets);
    }

    /// <summary>
    ///  Commands at a single boundary execute in order, including a pause before the first onset.
    /// </summary>
    [Fact]
    public void RapidStartPauseStop_PreservesOrderingAndUnconsumedFirstHit()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, [new(0, 0, Cr78Instrument.Claves)]), new Tempo(120));
        List<long> onsets = [];
        PercussionRenderer renderer = new(48_000, mailbox, 42, (frame, _) => onsets.Add(frame));
        mailbox.Pause();
        Assert.Equal(DrumTransportState.Stopped, mailbox.State);
        mailbox.Start();
        mailbox.Pause();
        mailbox.Pause();
        Render(renderer, 100, 1_000);
        Assert.Empty(onsets);
        Assert.True(renderer.GetSnapshot(1_000, true).IsPaused);
        mailbox.Start();
        mailbox.Start();
        Render(renderer, 1_000, 2_000);
        Assert.Equal(new long[] { 1_000 }, onsets);

        mailbox.Pause();
        mailbox.Start();
        mailbox.Stop();
        mailbox.Start();
        mailbox.Pause();
        mailbox.Stop();
        mailbox.Start();
        Render(renderer, 2_000, 2_001);
        Assert.Equal(new long[] { 1_000, 2_000 }, onsets);
        Assert.Equal(DrumTransportState.Playing, mailbox.State);
        Assert.Equal(DrumTransportState.Playing, renderer.GetSnapshot(2_001, true).State);
    }

    /// <summary>
    ///  A pause queued during the ending block cannot strand requested state or erase a newer restart.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NaturalCompletion_ReconcilesLatePauseAndPreservesNewerRestart(bool restart)
    {
        PercussionMailbox mailbox = new(new PercussionScore(1,
            [new(0, 0, Cr78Instrument.Claves), new(0, 15, Cr78Instrument.Cymbal)]), new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.SetLoop(false);
        mailbox.Start();
        Render(renderer, 0, 7_488);
        renderer.BeginBlock(7_488);
        for (long frame = 7_488; frame < 7_800; frame++)
        {
            if (frame == 7_500)
            {
                mailbox.Pause();
                if (restart)
                {
                    mailbox.Stop();
                    mailbox.Start();
                }
            }

            renderer.Next(frame);
        }

        Assert.Equal(restart ? DrumTransportState.Playing : DrumTransportState.Paused, mailbox.State);
        Assert.Equal(DrumTransportState.Stopped, renderer.GetSnapshot(7_800, true).State);
        Assert.Equal(15, renderer.GetSnapshot(7_800, true).Step);
        Render(renderer, 7_800, 7_801);
        Assert.Equal(restart ? DrumTransportState.Playing : DrumTransportState.Stopped, mailbox.State);
        Assert.Equal(restart ? 3 : 2, renderer.RenderedHitCount);
        if (!restart)
        {
            mailbox.Start();
            Render(renderer, 7_801, 7_802);
            Assert.Equal(7_801, renderer.LastHitFrame);
        }
    }

    /// <summary>
    ///  A loaded configuration is captured with its reset/start, not replaced by a later live edit.
    /// </summary>
    [Fact]
    public void AtomicLoadAndStart_KeepTheirConfigurationAheadOfLaterCoalescedEdits()
    {
        PercussionScore original = new(1, [new(0, 0, Cr78Instrument.BassDrum), new(0, 8, Cr78Instrument.Claves)]);
        PercussionScore loaded = new(1, [new(0, 0, Cr78Instrument.Cowbell), new(0, 1, Cr78Instrument.Cowbell)]);
        PercussionScore later = new(1, [new(0, 0, Cr78Instrument.SnareDrum), new(0, 1, Cr78Instrument.SnareDrum)]);
        List<(long Frame, Cr78Instrument Instrument)> onsets = [];
        PercussionMailbox mailbox = new(original, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42,
            (frame, hit) => onsets.Add((frame, hit.Instrument)));
        mailbox.Start();
        Render(renderer, 0, 12_345);
        float[] volumes = Enumerable.Repeat(0.4f, 13).ToArray();
        mailbox.ApplyConfiguration(loaded, new Tempo(90), 0.25f, volumes, false, false, 0.7f, resetTransport: true);
        Array.Fill(volumes, 0f);
        Assert.Equal(0.4f, mailbox.GetInstrumentVolume(Cr78Instrument.Cowbell));
        Assert.Equal(DrumTransportState.Stopped, mailbox.State);
        mailbox.Start();
        mailbox.ApplyConfiguration(later, new Tempo(240), 0.6f, UnityVolumes(), true, true, 0.5f);
        Render(renderer, 12_345, 20_346);

        Assert.Equal(new long[] { 0, 12_345, 20_345 }, onsets.Select(hit => hit.Frame));
        Assert.Equal(new[] { Cr78Instrument.BassDrum, Cr78Instrument.Cowbell, Cr78Instrument.Cowbell },
            onsets.Select(hit => hit.Instrument));
        Assert.True(renderer.GetSnapshot(20_346, true).HasPendingChanges);
        Assert.Equal(DrumTransportState.Playing, renderer.GetSnapshot(12_345, true).State);
        Assert.Equal(0, renderer.GetSnapshot(12_346, true).Step);
        Render(renderer, 20_346, 140_346);
        Assert.Equal(Cr78Instrument.SnareDrum, onsets[^1].Instrument);
        Assert.Equal(140_345, onsets[^1].Frame);
        Assert.False(renderer.GetSnapshot(140_346, true).HasPendingChanges);
    }

    /// <summary>
    ///  Loading while paused atomically accepts the new score/tempo and resets, without waiting for Play.
    /// </summary>
    [Fact]
    public void AtomicLoad_ReplacesPausedPendingSettingsAndResetsHistory()
    {
        PercussionScore original = new(2, [new(1, 0, Cr78Instrument.Cymbal)]);
        PercussionMailbox mailbox = new(original, new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.Start();
        Render(renderer, 0, 8_200);
        mailbox.Pause();
        Render(renderer, 8_200, 9_000);
        mailbox.SetTempo(new Tempo(500));
        PercussionScore loaded = new(1, [new(0, 0, Cr78Instrument.Claves)]);
        mailbox.ApplyConfiguration(loaded, new Tempo(120), 0.2f, UnityVolumes(), false, false, 0.8f, true);
        Render(renderer, 9_000, 10_000);
        Assert.True(renderer.GetSnapshot(9_000, true).IsPaused);
        DrumPlaybackSnapshot replaced = renderer.GetSnapshot(9_001, true);
        Assert.Equal(DrumTransportState.Stopped, replaced.State);
        Assert.Equal(0, replaced.Bar);
        Assert.Equal(0, replaced.Step);
        Assert.False(replaced.HasPendingChanges);
        Assert.Same(loaded, mailbox.Settings.Score);
        Assert.Equal(new Tempo(120), mailbox.Settings.Tempo);
        Assert.Equal(0.2f, mailbox.Settings.MasterVolume);
        Assert.False(mailbox.Settings.Loop);
        Assert.False(mailbox.Settings.MetallicEnabled);
        Assert.Equal(0.8f, mailbox.Settings.MetallicLevel);
        Assert.Equal(0, Render(renderer, 10_000, 11_000));
        mailbox.Start();
        Assert.True(Render(renderer, 11_000, 12_000) > 0);
        Assert.Equal(11_000, renderer.LastHitFrame);
    }

    /// <summary>
    ///  Queue overflow rejects an entire reset without changing settings, and disposal bypasses fullness.
    /// </summary>
    [Fact]
    public void FullMailbox_RejectsWholeConfigurationAndStillDisposesGracefully()
    {
        PercussionScore score = new(1, []);
        PercussionMailbox mailbox = new(score, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        mailbox.Audition(Cr78Instrument.Cowbell, 1f);
        Render(renderer, 0, 1_000);
        for (int i = 0; i < 42; i++)
        {
            mailbox.Start();
            mailbox.Pause();
            mailbox.Stop();
        }

        mailbox.Start();
        mailbox.Pause();
        PercussionSettings before = mailbox.Settings;
        Assert.Throws<InvalidOperationException>(mailbox.Start);
        Assert.Throws<InvalidOperationException>(mailbox.Stop);
        Assert.Throws<InvalidOperationException>(() => mailbox.ApplyConfiguration(
            new PercussionScore(2, []), new Tempo(90), 0.3f, UnityVolumes(), false, false, 0.8f, true));
        Assert.Same(before, mailbox.Settings);
        Assert.Equal(DrumTransportState.Paused, mailbox.State);
        mailbox.Release();
        mailbox.Release();
        Assert.Equal(DrumTransportState.Stopped, mailbox.State);
        Assert.Throws<ObjectDisposedException>(mailbox.Pause);
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetMasterVolume(0.5f));
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetInstrumentVolume(Cr78Instrument.Guiro, 0.5f));
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetMetallicEnabled(false));
        Assert.Throws<ObjectDisposedException>(() =>
            mailbox.ApplyConfiguration(score, new Tempo(120), 1f, UnityVolumes(), true, true, 0f));
        Render(renderer, 1_000, 1_240);
        Assert.True(renderer.IsFinished);
        Assert.Equal(0, Render(renderer, 1_240, 2_000));
        Assert.Equal(0, mailbox.TryDrain(new PercussionCommand[PercussionMailbox.MaximumCommands], out _));
    }

    /// <summary>
    ///  The audio side defers contended admission rather than waiting for a document producer.
    /// </summary>
    [Fact]
    public async Task MailboxDrain_NeverWaitsForTheCallerLock()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, []), new Tempo(120));
        object sync = typeof(PercussionMailbox).GetField("_sync", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mailbox)!;
        using ManualResetEventSlim acquired = new(false);
        using ManualResetEventSlim release = new(false);
        Task producer = Task.Run(() =>
        {
            lock (sync)
            {
                acquired.Set();
                release.Wait(TimeSpan.FromSeconds(5));
            }
        }, TestContext.Current.CancellationToken);
        try
        {
            Assert.True(acquired.Wait(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken));
            PercussionCommand[] commands = new PercussionCommand[PercussionMailbox.MaximumCommands];
            Assert.Equal(-1, mailbox.TryDrain(commands, out PercussionSettings? settings));
            Assert.Null(settings);
        }
        finally
        {
            release.Set();
            await producer.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        }
    }

    /// <summary>
    ///  Retains the bool-based snapshot constructor for existing callers.
    /// </summary>
    [Fact]
    public void SnapshotConstructor_RemainsSourceCompatible()
    {
        DrumPlaybackSnapshot old = new(1, 2, isPlaying: true, hasPendingChanges: false, isPlaybackSynchronized: true);
        Assert.Equal(DrumTransportState.Playing, old.State);
        Assert.False(old.IsPaused);
        DrumPlaybackSnapshot paused = new(1, 2, DrumTransportState.Paused, false, true);
        Assert.True(paused.IsPaused);
        Assert.False(paused.IsPlaying);
        Assert.Equal(DrumTransportState.Stopped, default(DrumPlaybackSnapshot).State);
    }

    private static float[] UnityVolumes() => Enumerable.Repeat(1f, Cr78Kit.Instruments.Count).ToArray();

    private static double Render(PercussionRenderer renderer, long start, long end, int blockFrames = 127)
    {
        double energy = 0;
        for (long frame = start; frame < end; frame++)
        {
            if ((frame - start) % blockFrames == 0)
            {
                renderer.BeginBlock(frame);
            }

            energy += Math.Abs(renderer.Next(frame));
        }

        return energy;
    }
}
