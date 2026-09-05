using System.Threading.Channels;
using WinForms.Audio.Core;
using WinForms.Audio.Music;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;
using WinForms.Audio.Synthesis;

namespace SplitFlap.Tests;

/// <summary>
///  Validates immutable scores, sample-clock scheduling, bounded transport, and played-output snapshots.
/// </summary>
public sealed class PercussionScoreTests
{
    /// <summary>
    ///  Copies caller collections, sorts events, and makes every edit a separate immutable score.
    /// </summary>
    [Fact]
    public void Score_CopiesSortsAndEditsWithoutMutableAliases()
    {
        PercussionHit[] source =
        [
            new(1, 12, Cr78Instrument.Guiro, 0.6f, GateSteps: 4),
            new(0, 0, Cr78Instrument.BassDrum)
        ];
        PercussionScore original = new(2, source);
        source[0] = new PercussionHit(0, 1, Cr78Instrument.SnareDrum);
        Assert.Equal(Cr78Instrument.BassDrum, original.Hits[0].Instrument);
        Assert.True(original.HasHit(1, Cr78Instrument.Guiro, 12));
        Assert.False(original.HasHit(0, Cr78Instrument.SnareDrum, 1));
        Assert.Throws<NotSupportedException>(() =>
            ((IList<PercussionHit>)original.Hits)[0] = new PercussionHit(0, 1, Cr78Instrument.SnareDrum));

        PercussionScore added = original.WithStep(0, Cr78Instrument.SnareDrum, 4, true, 0.75f);
        Assert.True(added.HasHit(0, Cr78Instrument.SnareDrum, 4));
        Assert.False(original.HasHit(0, Cr78Instrument.SnareDrum, 4));
        PercussionScore accented = added.WithStep(1, Cr78Instrument.Guiro, 12, true, 0.9f);
        PercussionHit scrape = Assert.Single(accented.Hits, hit => hit.Instrument == Cr78Instrument.Guiro);
        Assert.Equal(4, scrape.GateSteps);
        Assert.Equal(0.9f, scrape.Velocity);
        Assert.Equal(0.6f, original.Hits[1].Velocity);
        PercussionScore removed = accented.WithStep(0, Cr78Instrument.BassDrum, 0, false);
        Assert.False(removed.HasHit(0, Cr78Instrument.BassDrum, 0));
        Assert.True(original.HasHit(0, Cr78Instrument.BassDrum, 0));
        Assert.Same(removed, removed.WithStep(0, Cr78Instrument.BassDrum, 0, false));
    }

    /// <summary>
    ///  Rejects malformed, duplicate, and out-of-range events instead of hiding ambiguous grid cells.
    /// </summary>
    [Fact]
    public void Score_RejectsInvalidEventsAndUnsupportedLayerTracks()
    {
        Assert.Throws<ArgumentNullException>(() => new PercussionScore(1, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionScore(0, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionScore(-1, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionScore(PercussionScore.MaximumBars + 1, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionScore(1, [default(PercussionHit)]));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PercussionScore(1, [new PercussionHit(1, 0, Cr78Instrument.BassDrum)]));
        Assert.Throws<ArgumentException>(() => new PercussionScore(1,
        [
            new(0, 0, Cr78Instrument.BassDrum, 0.2f),
            new(0, 0, Cr78Instrument.BassDrum, 0.9f)
        ]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(-1, 0, Cr78Instrument.BassDrum));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, -1, Cr78Instrument.BassDrum));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, 16, Cr78Instrument.BassDrum));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, 0, Cr78Instrument.MetallicBeat));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, 0, (Cr78Instrument)99));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, 0, Cr78Instrument.BassDrum, float.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, 0, Cr78Instrument.BassDrum, 1.1f));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionHit(0, 0, Cr78Instrument.Guiro, GateSteps: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PercussionHit(0, 0, Cr78Instrument.Guiro, GateSteps: PercussionScore.MaximumGateSteps + 1));
        PercussionScore score = new(1, []);
        Assert.Throws<ArgumentOutOfRangeException>(() => score.HasHit(1, Cr78Instrument.BassDrum, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => score.WithStep(0, Cr78Instrument.HiHat, 16, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => score.WithStep(0, Cr78Instrument.MetallicBeat, 0, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => score.WithStep(0, Cr78Instrument.HiHat, 0, true, float.PositiveInfinity));
    }

    /// <summary>
    ///  Retains sub-sample phase over a million steps instead of rounding and drifting every loop.
    /// </summary>
    [Theory]
    [InlineData(32_000, 73)]
    [InlineData(44_100, 137)]
    [InlineData(44_100, 136)]
    [InlineData(48_000, 119)]
    [InlineData(48_000, 997)]
    [InlineData(192_000, 1)]
    public void FractionalClock_DoesNotAccumulateLoopDrift(int sampleRate, int bpm)
    {
        const long origin = 12_345_678_901;
        PercussionClock clock = new(sampleRate, new Tempo(bpm));
        clock.Reset(origin);
        for (int step = 0; step <= 1_000_000; step++)
        {
            if (step % 997 == 0 || step == 1_000_000)
            {
                decimal ideal = origin + sampleRate * 15m * step / bpm;
                Assert.InRange(Math.Abs(clock.NextFrame - ideal), 0, 1);
                decimal gateEnd = origin + sampleRate * 15m * (step + 7) / bpm;
                long expectedGate = PercussionClock.RoundFrame(gateEnd) - PercussionClock.RoundFrame(ideal);
                Assert.InRange(Math.Abs(clock.GateFrames(7) - expectedGate), 0, 1);
            }

            clock.Advance();
        }
    }

    /// <summary>
    ///  Keeps the fractional origin when tempo alternates repeatedly at bar boundaries.
    /// </summary>
    [Fact]
    public void FractionalClock_TempoChangesPreserveItsRemainder()
    {
        PercussionClock clock = new(44_100, new Tempo(137));
        decimal expected = 0;
        for (int bar = 0; bar < 10_000; bar++)
        {
            int bpm = bar % 2 == 0 ? 137 : 119;
            clock.SetTempo(new Tempo(bpm));
            for (int step = 0; step < 16; step++)
            {
                clock.Advance();
            }

            expected += 44_100 * 15m * 16 / bpm;
            Assert.InRange(Math.Abs(clock.NextFrame - expected), 0, 1);
        }
    }

    /// <summary>
    ///  Checks real renderer onsets and event counts across non-integral step and block boundaries.
    /// </summary>
    [Theory]
    [InlineData(32_000, 73, 127)]
    [InlineData(44_100, 137, 257)]
    [InlineData(44_100, 136, 509)]
    [InlineData(48_000, 119, 480)]
    [InlineData(48_000, 199, 1_024)]
    public void Renderer_EmitsEachEventAtItsSampleAccurateLoopPosition(int sampleRate, int bpm, int blockFrames)
    {
        PercussionScore score = new(2,
        [
            new(0, 0, Cr78Instrument.BassDrum, 0.7f),
            new(0, 7, Cr78Instrument.HiHat, 0.5f),
            new(1, 3, Cr78Instrument.Cowbell, 0.6f),
            new(1, 15, Cr78Instrument.Guiro, 0.4f, GateSteps: 2)
        ]);
        List<(long Frame, PercussionHit Hit)> onsets = [];
        PercussionMailbox mailbox = new(score, new Tempo(bpm));
        PercussionRenderer renderer = new(sampleRate, mailbox, 42, (frame, hit) => onsets.Add((frame, hit)));
        const long origin = 1_234;
        mailbox.Start();
        long end = PercussionClock.RoundFrame(origin + sampleRate * 15m * 64 / bpm);
        RenderRange(renderer, origin, end, blockFrames);
        Assert.Equal(8, onsets.Count);
        Assert.Equal(8, renderer.RenderedHitCount);
        for (int loop = 0; loop < 2; loop++)
        {
            for (int eventIndex = 0; eventIndex < score.Hits.Count; eventIndex++)
            {
                PercussionHit hit = score.Hits[eventIndex];
                decimal ideal = origin + sampleRate * 15m * (loop * 32 + hit.Bar * 16 + hit.Step) / bpm;
                (long frame, PercussionHit actualHit) = onsets[loop * score.Hits.Count + eventIndex];
                Assert.Equal(hit, actualHit);
                Assert.InRange(Math.Abs(frame - ideal), 0, 1);
            }
        }
    }

    /// <summary>
    ///  Applies the latest score/tempo together at a bar boundary and retains old played history.
    /// </summary>
    [Fact]
    public void ScoreAndTempoEdits_AreAtomicAtTheNextBarAndAudibleRevision()
    {
        PercussionScore initial = new(1,
        [
            new(0, 0, Cr78Instrument.BassDrum),
            new(0, 8, Cr78Instrument.Claves)
        ]);
        PercussionScore edited = new(2,
        [
            new(1, 0, Cr78Instrument.HiHat),
            new(1, 1, Cr78Instrument.HiHat)
        ]);
        List<(long Frame, PercussionHit Hit)> onsets = [];
        PercussionMailbox mailbox = new(initial, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42, (frame, hit) => onsets.Add((frame, hit)));
        mailbox.Start();
        RenderRange(renderer, 0, 12_345);
        mailbox.SetScore(edited);
        mailbox.SetTempo(new Tempo(180));
        mailbox.SetTempo(new Tempo(240));
        Assert.Same(edited, mailbox.Settings.Score);
        Assert.Equal(240, mailbox.Settings.Tempo.BeatsPerMinute);
        RenderRange(renderer, 12_345, 103_000);

        Assert.Equal(new long[] { 0, 48_000, 96_000, 99_000 }, onsets.Select(item => item.Frame));
        Assert.Equal(Cr78Instrument.Claves, onsets[1].Hit.Instrument);
        Assert.Equal(Cr78Instrument.HiHat, onsets[2].Hit.Instrument);
        DrumPlaybackSnapshot old = renderer.GetSnapshot(60_001, deviceSynchronized: true);
        Assert.Equal(0, old.Bar);
        Assert.Equal(10, old.Step);
        Assert.True(old.HasPendingChanges);
        Assert.True(old.IsPlaybackSynchronized);
        Assert.True(renderer.GetSnapshot(96_000, true).HasPendingChanges);
        DrumPlaybackSnapshot boundary = renderer.GetSnapshot(96_001, true);
        Assert.Equal(1, boundary.Bar);
        Assert.Equal(0, boundary.Step);
        Assert.False(boundary.HasPendingChanges);
        Assert.Equal(1, renderer.GetSnapshot(99_001, true).Step);
    }

    /// <summary>
    ///  Keeps non-integral old-bar phase when the next bar uses a different tempo.
    /// </summary>
    [Fact]
    public void TempoChange_DoesNotRoundAwayTheBarOrigin()
    {
        PercussionScore score = new(1, [new(0, 0, Cr78Instrument.Claves), new(0, 1, Cr78Instrument.Claves)]);
        List<long> onsets = [];
        PercussionMailbox mailbox = new(score, new Tempo(137));
        PercussionRenderer renderer = new(44_100, mailbox, 42, (frame, _) => onsets.Add(frame));
        mailbox.Start();
        RenderRange(renderer, 0, 10_000);
        mailbox.SetTempo(new Tempo(119));
        decimal boundary = 44_100 * 15m * 16 / 137;
        decimal nextStep = boundary + 44_100 * 15m / 119;
        RenderRange(renderer, 10_000, PercussionClock.RoundFrame(nextStep) + 2);
        Assert.Equal(4, onsets.Count);
        Assert.InRange(Math.Abs(onsets[2] - boundary), 0, 1);
        Assert.InRange(Math.Abs(onsets[3] - nextStep), 0, 1);
    }

    /// <summary>
    ///  Preserves an old bar number and tempo until its queued audio has actually been played.
    /// </summary>
    [Fact]
    public void PlayedHistory_SurvivesScoreLengthAndTempoChanges()
    {
        PercussionScore original = new(2, [new(0, 0, Cr78Instrument.BassDrum), new(1, 0, Cr78Instrument.SnareDrum)]);
        PercussionScore shorter = new(1, [new(0, 0, Cr78Instrument.HiHat)]);
        PercussionMailbox mailbox = new(original, new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.Start();
        RenderRange(renderer, 0, 8_000);
        mailbox.SetScore(shorter);
        mailbox.SetTempo(new Tempo(500));
        RenderRange(renderer, 8_000, 18_000);

        DrumPlaybackSnapshot queuedOldBar = renderer.GetSnapshot(7_681, true);
        Assert.Equal(1, queuedOldBar.Bar);
        Assert.Equal(0, queuedOldBar.Step);
        Assert.True(queuedOldBar.HasPendingChanges);
        DrumPlaybackSnapshot newBar = renderer.GetSnapshot(15_361, true);
        Assert.Equal(0, newBar.Bar);
        Assert.Equal(0, newBar.Step);
        Assert.False(newBar.HasPendingChanges);
        Assert.Equal(1, renderer.GetSnapshot(16_321, true).Step);
        Assert.False(renderer.GetSnapshot(16_321, false).IsPlaybackSynchronized);
    }

    /// <summary>
    ///  Accepts stopped edits on a block boundary rather than leaving them pending until Play.
    /// </summary>
    [Fact]
    public void StoppedEdits_AreAcceptedAndUsedByTheNextStart()
    {
        PercussionScore initial = new(1, []);
        PercussionScore edited = new(1, [new(0, 0, Cr78Instrument.Claves), new(0, 1, Cr78Instrument.Claves)]);
        List<long> onsets = [];
        PercussionMailbox mailbox = new(initial, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42, (frame, _) => onsets.Add(frame));
        RenderRange(renderer, 0, 128);
        mailbox.SetScore(edited);
        mailbox.SetTempo(new Tempo(90));
        Assert.True(renderer.GetSnapshot(128, true).HasPendingChanges);
        RenderRange(renderer, 128, 256);
        Assert.False(renderer.GetSnapshot(129, true).HasPendingChanges);
        Assert.False(renderer.GetSnapshot(129, true).IsPlaying);
        mailbox.Start();
        RenderRange(renderer, 256, 8_258);
        Assert.Equal(new long[] { 256, 8_256 }, onsets);
    }

    /// <summary>
    ///  Preserves Stop/Start ordering, cancelling old scheduling rather than accidentally resuming it.
    /// </summary>
    [Fact]
    public void StartStopStart_IsOrderedAndRestartsAtTheNewSampleOrigin()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1,
            [new(0, 0, Cr78Instrument.BassDrum), new(0, 1, Cr78Instrument.SnareDrum)]), new Tempo(120));
        List<long> onsets = [];
        PercussionRenderer renderer = new(48_000, mailbox, 42, (frame, _) => onsets.Add(frame));
        mailbox.Start();
        mailbox.Stop();
        mailbox.Start();
        mailbox.Start();
        RenderRange(renderer, 1_000, 2_048);
        Assert.Equal(new long[] { 1_000 }, onsets);
        mailbox.Stop();
        mailbox.Start();
        RenderRange(renderer, 2_048, 3_000);
        Assert.Equal(new long[] { 1_000, 2_048 }, onsets);
        mailbox.Stop();
        RenderRange(renderer, 3_000, 10_000);
        Assert.Equal(2, onsets.Count);
        Assert.False(mailbox.IsPlaying);
        Assert.False(renderer.GetSnapshot(10_000, true).IsPlaying);
        Assert.Equal(0, RenderRange(renderer, 10_000, 11_000));
        mailbox.Audition(Cr78Instrument.Cowbell, 1);
        Assert.True(RenderRange(renderer, 11_000, 12_000) > 0);
        Assert.False(mailbox.IsPlaying);
        Assert.Equal(2, onsets.Count);
    }

    /// <summary>
    ///  Applies incoming controls only when a new block begins, never after part of a block was mixed.
    /// </summary>
    [Fact]
    public void Commands_DoNotChangeAHalfRenderedBlock()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1,
        [
            new(0, 0, Cr78Instrument.Claves),
            new(0, 1, Cr78Instrument.Claves),
            new(0, 2, Cr78Instrument.Claves),
            new(0, 3, Cr78Instrument.Claves)
        ]), new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.Start();
        renderer.BeginBlock(0);
        for (int frame = 0; frame < 1_024; frame++)
        {
            if (frame == 100)
            {
                mailbox.Stop();
            }

            renderer.Next(frame);
        }

        Assert.Equal(3, renderer.RenderedHitCount);
        RenderRange(renderer, 1_024, 2_048);
        Assert.Equal(3, renderer.RenderedHitCount);
    }

    /// <summary>
    ///  Keeps already-started guiro gates and tails unchanged by next-bar score/tempo replacement.
    /// </summary>
    [Fact]
    public void QueuedEdits_DoNotCutOrRetimingAnExistingGuiroGate()
    {
        PercussionMailbox mailbox = new(
            new PercussionScore(2, [new(0, 15, Cr78Instrument.Guiro, GateSteps: 4)]), new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.Start();
        RenderRange(renderer, 0, 7_400);
        mailbox.SetScore(new PercussionScore(2, []));
        mailbox.SetTempo(new Tempo(500));
        Assert.True(RenderRange(renderer, 7_400, 10_000) > 0);
        RenderRange(renderer, 10_000, 14_800);
        Assert.Equal(0, RenderRange(renderer, 14_800, 16_000));
        Assert.Equal(1, renderer.RenderedHitCount);
    }

    /// <summary>
    ///  Ends a non-looped score exactly once while allowing its last natural sound tail to finish.
    /// </summary>
    [Fact]
    public void NonLoopingScore_CompletesWithoutCuttingItsLastCymbal()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, [new(0, 15, Cr78Instrument.Cymbal)]), new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.SetLoop(false);
        mailbox.Start();
        RenderRange(renderer, 0, 7_680);
        Assert.True(mailbox.IsPlaying);
        Assert.True(RenderRange(renderer, 7_680, 12_000) > 0);
        Assert.False(mailbox.IsPlaying);
        Assert.Equal(1, renderer.RenderedHitCount);
        Assert.False(renderer.GetSnapshot(7_681, true).IsPlaying);
        Assert.False(renderer.IsFinished);
    }

    /// <summary>
    ///  Coalesces noisy input but explicitly rejects transport overflow without dropping command order.
    /// </summary>
    [Fact]
    public void Mailbox_IsBoundedCoalescedAndOrdered()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, []), new Tempo(120));
        for (int i = 0; i < 10_000; i++)
        {
            mailbox.Audition((Cr78Instrument)(i % 14), 0.5f);
            mailbox.SetTempo(new Tempo(100 + i % 100));
        }

        PercussionCommand[] commands = new PercussionCommand[PercussionMailbox.MaximumCommands];
        Assert.Equal(14, mailbox.TryDrain(commands, out PercussionSettings? settings));
        Assert.Equal(199, settings!.Tempo.BeatsPerMinute);
        for (int i = 1; i < 14; i++)
        {
            Assert.True(commands[i].Sequence > commands[i - 1].Sequence);
        }

        for (int i = 0; i < PercussionMailbox.MaximumCommands / 2; i++)
        {
            mailbox.Start();
            mailbox.Stop();
        }

        Assert.False(mailbox.IsPlaying);
        Assert.Throws<InvalidOperationException>(mailbox.Start);
        Assert.False(mailbox.IsPlaying);
        Assert.Equal(PercussionMailbox.MaximumCommands, mailbox.TryDrain(commands, out _));
        for (int i = 0; i < commands.Length; i++)
        {
            Assert.Equal(i % 2 == 0 ? PercussionCommandKind.Start : PercussionCommandKind.Stop, commands[i].Kind);
            if (i > 0)
            {
                Assert.True(commands[i].Sequence > commands[i - 1].Sequence);
            }
        }

        mailbox.Start();
        Assert.True(mailbox.IsPlaying);
    }

    /// <summary>
    ///  Does not let an older natural completion clear a newly queued restart.
    /// </summary>
    [Fact]
    public void NaturalCompletion_CannotOverwriteANewerTransportRequest()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, []), new Tempo(120));
        PercussionCommand[] commands = new PercussionCommand[PercussionMailbox.MaximumCommands];
        mailbox.Start();
        mailbox.TryDrain(commands, out _);
        long oldStart = commands[0].Sequence;
        mailbox.Stop();
        mailbox.Start();
        mailbox.CompleteStart(oldStart);
        Assert.True(mailbox.IsPlaying);
    }

    /// <summary>
    ///  Marks insufficient retained history as unsynchronized instead of using an unrelated rendered bar.
    /// </summary>
    [Fact]
    public void History_ReportsOverwriteAndCoalescesSameFrameTransitions()
    {
        PercussionHistory history = new(capacity: 2);
        history.Write(new PercussionHistoryPoint(1_000, 0, 0, true, 1_000, 100, 1));
        history.Write(new PercussionHistoryPoint(1_000, 0, 0, false, 1_000, 100, 1));
        history.Write(new PercussionHistoryPoint(2_000, 1, 0, true, 2_000, 100, 2));
        Assert.True(history.TryRead(1_100, out PercussionHistoryPoint stopped, out bool unavailable));
        Assert.False(stopped.Playing);
        Assert.False(unavailable);
        history.Write(new PercussionHistoryPoint(3_000, 2, 0, true, 3_000, 100, 3));
        Assert.False(history.TryRead(1_100, out _, out unavailable));
        Assert.True(unavailable);
        Assert.False(history.TryRead(999, out _, out unavailable));
        Assert.False(unavailable);
        Assert.True(history.TryRead(2_001, out PercussionHistoryPoint retained, out unavailable));
        Assert.Equal(1, retained.Bar);
        Assert.Equal(2, retained.Revision);
        Assert.False(unavailable);
    }

    /// <summary>
    ///  Keeps recurring score hits and bar-history publication free from managed audio-thread allocation.
    /// </summary>
    [Fact]
    public void RepeatedBars_DoNotAllocateDspOrHistoryObjects()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1,
            [new(0, 0, Cr78Instrument.HighBongo, 0.5f), new(0, 2, Cr78Instrument.HiHat, 0.6f)]),
            new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.Start();
        RenderRange(renderer, 0, 20_000, 256);
        long before = GC.GetAllocatedBytesForCurrentThread();
        double energy = RenderRange(renderer, 20_000, 340_000, 256);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
        Assert.True(energy > 0);
        Assert.True(renderer.RenderedHitCount >= 80);
        mailbox.Release();
        RenderRange(renderer, 340_000, 341_000, 256);
        Assert.True(renderer.IsFinished);
    }

    /// <summary>
    ///  Rejects invalid transport settings and closes every command API after release.
    /// </summary>
    [Fact]
    public void Transport_ValidatesSettingsAndDisposal()
    {
        PercussionScore score = new(1, []);
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionMailbox(score, default));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionMailbox(score, new Tempo(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercussionMailbox(score, new Tempo(1_001)));
        PercussionMailbox mailbox = new(score, new Tempo(120));
        Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.SetTempo(new Tempo(0)));
        Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.SetMetallicLevel(float.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.Audition((Cr78Instrument)99, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.Audition(Cr78Instrument.Cowbell, -1));
        mailbox.Release();
        mailbox.Release();
        Assert.Throws<ObjectDisposedException>(mailbox.Start);
        Assert.Throws<ObjectDisposedException>(mailbox.Stop);
        Assert.Throws<ObjectDisposedException>(() => mailbox.Audition(Cr78Instrument.Cowbell, 1));
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetTempo(new Tempo(130)));
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetScore(score));
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetLoop(false));
        Assert.Throws<ObjectDisposedException>(() => mailbox.SetMetallicLevel(0.5f));
    }

    /// <summary>
    ///  Exercises the actual engine admission origin, device-clock delay, stop/audition, and voice disposal.
    /// </summary>
    [Fact]
    public async Task PublicPlayer_FollowsPlayedBuffersAndDoesNotOwnTheEngine()
    {
        using ClockedSink sink = new(4_800);
        using AudioEngine engine = AudioEngine.Create(sink);
        engine.MasterVolume = 1;
        engine.Reverb = ReverbSettings.Off;
        await sink.ReadAsync();
        using DrumMachinePlayer player = new(engine, new PercussionScore(1,
            [new(0, 0, Cr78Instrument.BassDrum), new(0, 1, Cr78Instrument.HiHat)]), new Tempo(120));
        Assert.True(player.Loop);
        Assert.Equal(DrumTransportState.Stopped, player.State);
        Assert.False(player.IsPaused);
        Assert.Equal(1f, player.MasterVolume);
        Assert.True(player.MetallicEnabled);
        Assert.All(Cr78Kit.Instruments, instrument => Assert.Equal(1f, player.GetInstrumentVolume(instrument)));
        Assert.Equal(0, player.MetallicLevel);
        player.Start();
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);
        Assert.True(player.IsPlaying);
        Assert.False(player.GetPlaybackSnapshot().IsPlaying);
        sink.Advance();
        await sink.ReadAsync();
        sink.SetCompletedFrames(9_600);
        Assert.Equal(14_400, engine.RenderedFrames);
        DrumPlaybackSnapshot heard = player.GetPlaybackSnapshot();
        Assert.True(heard.IsPlaybackSynchronized);
        Assert.True(heard.IsPlaying);
        Assert.Equal(0, heard.Step);

        player.Stop();
        Assert.False(player.IsPlaying);
        sink.Advance();
        await sink.ReadAsync();
        Assert.True(player.GetPlaybackSnapshot().IsPlaying);
        sink.SetCompletedFrames(14_400);
        Assert.Equal(1, player.GetPlaybackSnapshot().Step);
        Assert.True(player.GetPlaybackSnapshot().IsPlaying);
        sink.Advance();
        await sink.ReadAsync();
        sink.SetCompletedFrames(19_200);
        Assert.False(player.GetPlaybackSnapshot().IsPlaying);

        player.Audition(Cr78Instrument.MetallicBeat);
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);
        player.Dispose();
        Assert.Throws<ObjectDisposedException>(player.Start);
        Assert.Throws<ObjectDisposedException>(() => player.Audition(Cr78Instrument.Cowbell));
        sink.Advance();
        await sink.ReadAsync();
        Assert.Equal(0, engine.ActiveVoices);
        Assert.False(engine.Completion.IsCompleted);

        Task otherVoice = engine.Play(new ProbeVoice());
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);
        Assert.True(otherVoice.IsCompletedSuccessfully);
        Assert.False(engine.Completion.IsCompleted);
    }

    /// <summary>
    ///  Public Pause leaves fake-device progress and auditions alive; player master cannot mute another voice.
    /// </summary>
    [Fact]
    public async Task PublicPlayer_PauseAndLocalMasterDoNotPauseOrMuteTheEngine()
    {
        using ClockedSink sink = new(4_800);
        using AudioEngine engine = AudioEngine.Create(sink);
        engine.MasterVolume = 1;
        engine.Reverb = ReverbSettings.Off;
        await sink.ReadAsync();
        using DrumMachinePlayer player = new(engine, new PercussionScore(1,
            [new(0, 0, Cr78Instrument.Claves), new(0, 1, Cr78Instrument.Claves), new(0, 2, Cr78Instrument.Claves)]),
            new Tempo(120));
        player.Start();
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();
        sink.SetCompletedFrames(9_600);
        player.Pause();
        Assert.True(player.IsPaused);
        Assert.False(player.IsPlaying);
        Assert.True(player.GetPlaybackSnapshot().IsPlaying);
        sink.Advance();
        await sink.ReadAsync();
        sink.SetCompletedFrames(14_400);
        Assert.True(player.GetPlaybackSnapshot().IsPlaying);
        sink.Advance();
        Assert.All(await sink.ReadAsync(), sample => Assert.Equal(0, sample));
        Assert.Equal(24_000, engine.RenderedFrames);
        sink.SetCompletedFrames(19_200);
        Assert.True(player.GetPlaybackSnapshot().IsPaused);
        Assert.Equal(1, player.GetPlaybackSnapshot().Step);
        player.Audition(Cr78Instrument.Cowbell);
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);
        Assert.True(player.IsPaused);
        player.Start();
        Assert.True(player.IsPlaying);
        Assert.True(player.GetPlaybackSnapshot().IsPaused);
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);
        // ReadAsync exposes a rendered block while Write is still blocked. Submit that
        // resumed block before asking the fake device to report it as played.
        sink.Advance();
        await sink.ReadAsync();
        sink.SetCompletedFrames(33_600);
        Assert.True(player.GetPlaybackSnapshot().IsPlaying);
        Assert.Equal(2, player.GetPlaybackSnapshot().Step);

        player.Stop();
        player.MasterVolume = 0;
        player.MetallicEnabled = false;
        player.MetallicLevel = 0.5f;
        player.SetInstrumentVolume(Cr78Instrument.Cowbell, 0.25f);
        Assert.Equal(0.25f, player.GetInstrumentVolume(Cr78Instrument.Cowbell));
        Assert.Equal(0.5f, player.MetallicLevel);
        sink.Advance();
        await sink.ReadAsync();
        player.Audition(Cr78Instrument.MetallicBeat);
        Task other = engine.Play(new ProbeVoice());
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);
        Assert.True(other.IsCompletedSuccessfully);
        Assert.Equal(1f, engine.MasterVolume);
        sink.Advance();
        Assert.All(await sink.ReadAsync(), sample => Assert.Equal(0, sample));
        Assert.Equal(DrumTransportState.Stopped, player.State);
        Assert.False(engine.Completion.IsCompleted);
    }

    /// <summary>
    ///  Labels a sink lacking completed-buffer progress as submitted-stream approximation.
    /// </summary>
    [Fact]
    public async Task PublicPlayer_LabelsUnclockedSinkAsApproximate()
    {
        using ManualSink sink = new(4_800);
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        using DrumMachinePlayer player = new(engine, new PercussionScore(1, [new(0, 0, Cr78Instrument.BassDrum)]), new Tempo(120));
        player.Start();
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();
        DrumPlaybackSnapshot snapshot = player.GetPlaybackSnapshot();
        Assert.True(snapshot.IsPlaying);
        Assert.False(snapshot.IsPlaybackSynchronized);
        Assert.Equal(0, snapshot.Step);
    }

    private static double RenderRange(PercussionRenderer renderer, long start, long end, int blockFrames = 128)
    {
        double energy = 0;
        renderer.BeginBlock(start);
        for (long frame = start; frame < end; frame++)
        {
            if (frame != start && (frame - start) % blockFrames == 0)
            {
                renderer.BeginBlock(frame);
            }

            energy += Math.Abs(renderer.Next(frame));
        }

        return energy;
    }

    private sealed class ProbeVoice : IVoice
    {
        private int _left = 4;

        /// <summary>
        ///  Gets whether the short engine-liveness probe has completed.
        /// </summary>
        public bool IsFinished => _left <= 0;

        /// <summary>
        ///  Produces a small known sample before ending.
        /// </summary>
        public float Next() => _left-- > 0 ? 0.1f : 0;

        /// <summary>
        ///  Ends the probe without owning any resources.
        /// </summary>
        public void Release() => _left = 0;
    }

    private class ManualSink(int framesPerBuffer) : IAudioSink
    {
        private readonly Channel<short[]> _blocks = Channel.CreateUnbounded<short[]>();
        private readonly SemaphoreSlim _advance = new(0);
        private int _disposed;

        /// <summary>
        ///  Gets a normal full-palette mono format without opening an endpoint.
        /// </summary>
        public AudioFormat Format => new(48_000, 1);

        /// <summary>
        ///  Gets the controllable fake-device block size.
        /// </summary>
        public int FramesPerBuffer => framesPerBuffer;

        /// <summary>
        ///  Waits for the next rendered but not yet successfully submitted block.
        /// </summary>
        internal async Task<short[]> ReadAsync()
            => await _blocks.Reader.ReadAsync(TestContext.Current.CancellationToken).AsTask()
                .WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        /// <summary>
        ///  Allows the current sink write to succeed and the engine to render its next block.
        /// </summary>
        internal void Advance()
            => _advance.Release();

        /// <summary>
        ///  Captures PCM and holds successful submission until explicitly advanced by the test.
        /// </summary>
        public void Write(ReadOnlySpan<short> pcm)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            Assert.Equal(FramesPerBuffer, pcm.Length);
            Assert.True(_blocks.Writer.TryWrite(pcm.ToArray()));
            _advance.Wait();
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        }

        /// <summary>
        ///  Wakes a blocked pump on engine disposal without counting discarded PCM as played.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _advance.Release();
            }
        }
    }

    private sealed class ClockedSink(int framesPerBuffer) : ManualSink(framesPerBuffer), IAudioPlaybackProgress
    {
        private long _completedFrames;

        /// <summary>
        ///  Gets the independently controlled completed-device-buffer count.
        /// </summary>
        public long CompletedFrames => Volatile.Read(ref _completedFrames);

        /// <summary>
        ///  Gets a realistic upper bound for queued fake-device frames.
        /// </summary>
        public int BufferCapacityFrames => FramesPerBuffer * 4;

        /// <summary>
        ///  Advances only audible completion, independently of rendering or submission.
        /// </summary>
        internal void SetCompletedFrames(long frames)
            => Volatile.Write(ref _completedFrames, frames);
    }
}
