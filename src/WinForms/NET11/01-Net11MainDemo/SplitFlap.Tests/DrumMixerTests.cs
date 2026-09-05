using SplitFlap.Audio.Music;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Covers deterministic dry output, per-channel/master ramps, and the shared metallic-layer rules.
/// </summary>
public sealed class DrumMixerTests
{
    /// <summary>
    ///  Unity defaults preserve the old sample-by-sample sum, seeds, gates, retriggers, and calibration.
    /// </summary>
    [Fact]
    public void DefaultRenderer_IsBitExactWithTheOriginalUnmixedChannels()
    {
        const int sampleRate = 32_000;
        const uint seed = 42;
        PercussionScore score = new(1, Cr78Kit.Instruments.Select((instrument, step) =>
            new PercussionHit(0, step, instrument, 0.4f + step * 0.04f, GateSteps: 7)));
        PercussionMailbox mailbox = new(score, new Tempo(960));
        PercussionRenderer renderer = new(sampleRate, mailbox, seed);
        Cr78Generator[] original = Enumerable.Range(0, 14).Select(i =>
            new Cr78Generator(sampleRate, (Cr78Instrument)i, ChannelSeed(seed, (Cr78Instrument)i))).ToArray();
        Assert.Equal(1f, mailbox.Settings.MasterVolume);
        Assert.True(mailbox.Settings.MetallicEnabled);
        Assert.Equal(0f, mailbox.Settings.MetallicLevel);
        Assert.All(Cr78Kit.Instruments, instrument => Assert.Equal(1f, mailbox.GetInstrumentVolume(instrument)));
        mailbox.Start();
        for (int frame = 0; frame < 32_000; frame++)
        {
            if (frame % 127 == 0)
            {
                renderer.BeginBlock(frame);
            }

            if (frame % 500 == 0)
            {
                foreach (PercussionHit hit in score.GetStepHits(0, frame / 500 % 16))
                {
                    original[(int)hit.Instrument].Trigger(hit.Velocity, 500 * hit.GateSteps);
                }
            }

            float expected = 0;
            foreach (Cr78Generator generator in original)
            {
                expected += generator.Next();
            }

            Assert.Equal(BitConverter.SingleToInt32Bits(expected), BitConverter.SingleToInt32Bits(renderer.Next(frame)));
            Assert.InRange(Math.Abs(expected), 0f, 0.9051f);
        }
    }

    /// <summary>
    ///  Every primary fader affects both score hits and auditions, followed by one player-local master.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InstrumentAndMasterGains_ScaleEverySoundWithoutChangingStrikeVelocity(bool audition)
    {
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            PercussionScore score = new(1, audition ? [] : [new PercussionHit(0, 0, instrument, 0.8f, GateSteps: 2)]);
            PercussionMailbox dryMailbox = new(score, new Tempo(120));
            PercussionMailbox mixedMailbox = new(score, new Tempo(120));
            mixedMailbox.SetInstrumentVolume(instrument, 0.25f);
            mixedMailbox.SetMasterVolume(0.5f);
            PercussionRenderer dry = new(32_000, dryMailbox, 42);
            PercussionRenderer mixed = new(32_000, mixedMailbox, 42);
            if (audition)
            {
                dryMailbox.Audition(instrument, 0.8f);
                mixedMailbox.Audition(instrument, 0.8f);
            }
            else
            {
                dryMailbox.Start();
                mixedMailbox.Start();
            }

            dry.BeginBlock(0);
            mixed.BeginBlock(0);
            double energy = 0;
            for (int frame = 0; frame < 2_000; frame++)
            {
                float expected = dry.Next(frame) * 0.125f;
                float actual = mixed.Next(frame);
                Assert.Equal(expected, actual);
                energy += Math.Abs(actual);
            }

            Assert.True(energy > 0);
            Assert.Equal(audition ? 0 : 1, mixed.RenderedHitCount);
        }
    }

    /// <summary>
    ///  Live master and percussion faders ramp existing tails; muted channels continue from the same phase.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LiveFader_RampsTheTailAndMutedGeneratorsKeepAdvancing(bool master)
    {
        const int sampleRate = 48_000;
        const int rampSamples = sampleRate / 200;
        PercussionScore score = new(1, [new(0, 0, Cr78Instrument.Cymbal)]);
        PercussionMailbox dryMailbox = new(score, new Tempo(120));
        PercussionMailbox mixedMailbox = new(score, new Tempo(120));
        PercussionRenderer dry = new(sampleRate, dryMailbox, 42);
        PercussionRenderer mixed = new(sampleRate, mixedMailbox, 42);
        dryMailbox.Start();
        mixedMailbox.Start();
        dry.BeginBlock(0);
        mixed.BeginBlock(0);
        for (int frame = 0; frame < 2_000; frame++)
        {
            Assert.Equal(dry.Next(frame), mixed.Next(frame));
        }

        SetFader(0);
        mixed.BeginBlock(2_000);
        float previousGain = 1f;
        bool heardRamp = false;
        for (int frame = 2_000; frame < 7_000; frame++)
        {
            float expected = dry.Next(frame);
            float actual = mixed.Next(frame);
            Assert.True(float.IsFinite(actual));
            if (frame >= 2_000 + rampSamples)
            {
                Assert.Equal(0, actual);
            }
            else if (Math.Abs(expected) > 1e-6f)
            {
                float gain = actual / expected;
                Assert.InRange(gain, 0f, 1f);
                Assert.InRange(previousGain - gain, -2e-6f, 0.1f);
                previousGain = gain;
                heardRamp |= actual != 0;
            }
        }

        Assert.True(heardRamp);
        SetFader(1);
        mixed.BeginBlock(7_000);
        double resumedEnergy = 0;
        for (int frame = 7_000; frame < 9_000; frame++)
        {
            float expected = dry.Next(frame);
            float actual = mixed.Next(frame);
            if (frame >= 7_000 + rampSamples)
            {
                Assert.Equal(expected, actual);
                resumedEnergy += Math.Abs(actual);
            }
        }

        Assert.True(resumedEnergy > 0);
        Assert.Equal(1, mixed.RenderedHitCount);

        void SetFader(float volume)
        {
            if (master)
            {
                mixedMailbox.SetMasterVolume(volume);
            }
            else
            {
                mixedMailbox.SetInstrumentVolume(Cr78Instrument.Cymbal, volume);
            }
        }
    }

    /// <summary>
    ///  A gain publication halfway through mixing cannot alter an already admitted block.
    /// </summary>
    [Fact]
    public void GainChanges_BeginOnlyAtTheNextBlock()
    {
        PercussionScore score = new(1, [new(0, 0, Cr78Instrument.Cymbal)]);
        PercussionMailbox dryMailbox = new(score, new Tempo(120));
        PercussionMailbox mixedMailbox = new(score, new Tempo(120));
        PercussionRenderer dry = new(48_000, dryMailbox, 42);
        PercussionRenderer mixed = new(48_000, mixedMailbox, 42);
        dryMailbox.Start();
        mixedMailbox.Start();
        dry.BeginBlock(0);
        mixed.BeginBlock(0);
        for (int frame = 0; frame < 1_024; frame++)
        {
            if (frame == 100)
            {
                mixedMailbox.SetInstrumentVolume(Cr78Instrument.Cymbal, 0);
                mixedMailbox.SetMasterVolume(0);
            }

            Assert.Equal(dry.Next(frame), mixed.Next(frame));
        }

        mixed.BeginBlock(1_024);
        double expectedEnergy = 0;
        for (int frame = 1_024; frame < 2_000; frame++)
        {
            float expected = dry.Next(frame);
            float actual = mixed.Next(frame);
            if (frame >= 1_264)
            {
                Assert.Equal(0, actual);
                expectedEnergy += Math.Abs(expected);
            }
        }

        Assert.True(expectedEnergy > 0);
    }

    /// <summary>
    ///  A mute established in silence cannot leak an attack when an audition shares its first block.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MuteWhileStopped_IsEffectiveFromTheFirstAuditionSample(bool master)
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, []), new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        if (master)
        {
            mailbox.SetMasterVolume(0);
        }
        else
        {
            mailbox.SetInstrumentVolume(Cr78Instrument.Cymbal, 0);
        }

        mailbox.Audition(Cr78Instrument.Cymbal, 1);
        Assert.Equal(0, Render(renderer, 0, 5_000));
        mailbox.SetMasterVolume(1);
        mailbox.SetInstrumentVolume(Cr78Instrument.Cymbal, 1);
        Assert.True(Render(renderer, 5_000, 6_000) > 0);
        Assert.Equal(0, renderer.RenderedHitCount);
    }

    /// <summary>
    ///  Simultaneous HH/CY strikes excite one shared channel at max velocity, with amount applied once.
    /// </summary>
    [Fact]
    public void MetallicLayer_IsIndependentOfDryFadersAndDeduplicatedWithoutSquaringItsAmount()
    {
        const uint seed = 42;
        PercussionMailbox mailbox = new(new PercussionScore(1,
            [new(0, 0, Cr78Instrument.HiHat, 0.6f), new(0, 0, Cr78Instrument.Cymbal)]), new Tempo(120));
        mailbox.SetInstrumentVolume(Cr78Instrument.HiHat, 0);
        mailbox.SetInstrumentVolume(Cr78Instrument.Cymbal, 0);
        mailbox.SetMetallicLevel(0.5f);
        mailbox.SetMasterVolume(0.5f);
        PercussionRenderer renderer = new(48_000, mailbox, seed);
        Cr78Generator metallic = new(48_000, Cr78Instrument.MetallicBeat, ChannelSeed(seed, Cr78Instrument.MetallicBeat));
        metallic.Trigger(1, 12_000);
        mailbox.Start();
        renderer.BeginBlock(0);
        double energy = 0;
        for (int frame = 0; frame < 2_000; frame++)
        {
            float actual = renderer.Next(frame);
            Assert.Equal(metallic.Next() * 0.25f, actual);
            energy += Math.Abs(actual);
        }

        Assert.True(energy > 0);
        Assert.Equal(2, renderer.RenderedHitCount);
    }

    /// <summary>
    ///  Disabling automatic metal fades its current tail, remembers amount, and continues silent DSP time.
    /// </summary>
    [Fact]
    public void MetallicDisable_FadesExistingTailAndRemembersItsAmount()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, [new(0, 0, Cr78Instrument.HiHat)]), new Tempo(120));
        mailbox.SetInstrumentVolume(Cr78Instrument.HiHat, 0);
        mailbox.SetMetallicLevel(0.65f);
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        Cr78Generator reference = new(48_000, Cr78Instrument.MetallicBeat, ChannelSeed(42, Cr78Instrument.MetallicBeat));
        reference.Trigger(1, 12_000);
        mailbox.Start();
        renderer.BeginBlock(0);
        for (int frame = 0; frame < 300; frame++)
        {
            Assert.Equal(reference.Next() * 0.65f, renderer.Next(frame));
        }

        mailbox.SetMetallicEnabled(false);
        Assert.Equal(0.65f, mailbox.Settings.MetallicLevel);
        renderer.BeginBlock(300);
        double rampEnergy = 0;
        for (int frame = 300; frame < 1_000; frame++)
        {
            reference.Next();
            float actual = renderer.Next(frame);
            if (frame < 540)
            {
                rampEnergy += Math.Abs(actual);
            }
            else
            {
                Assert.Equal(0f, actual);
            }
        }

        Assert.True(rampEnergy > 0);
        mailbox.SetMetallicEnabled(true);
        renderer.BeginBlock(1_000);
        double resumedEnergy = 0;
        for (int frame = 1_000; frame < 2_000; frame++)
        {
            float expected = reference.Next() * 0.65f;
            float actual = renderer.Next(frame);
            if (frame >= 1_240)
            {
                Assert.Equal(expected, actual);
                resumedEnergy += Math.Abs(actual);
            }
        }

        Assert.True(resumedEnergy > 0);
        Assert.Equal(0.65f, mailbox.Settings.MetallicLevel);
    }

    /// <summary>
    ///  Explicit metal audition bypasses enable/amount, not master, until a later enabled layer strike.
    /// </summary>
    [Fact]
    public void MetallicAudition_BypassesDisabledLayerButSharesItsPreparedChannel()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1, []), new Tempo(120));
        mailbox.SetMetallicEnabled(false);
        mailbox.SetMasterVolume(0.5f);
        mailbox.SetInstrumentVolume(Cr78Instrument.HiHat, 0);
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        Cr78Generator reference = new(48_000, Cr78Instrument.MetallicBeat, ChannelSeed(42, Cr78Instrument.MetallicBeat));
        reference.Trigger(1, 12_000);
        mailbox.Audition(Cr78Instrument.MetallicBeat, 1);
        renderer.BeginBlock(0);
        double auditionEnergy = 0;
        for (int frame = 0; frame < 1_000; frame++)
        {
            if (frame == 300)
            {
                mailbox.SetMetallicLevel(0.25f);
                mailbox.Audition(Cr78Instrument.HiHat, 1);
                renderer.BeginBlock(frame);
            }

            float expected = reference.Next() * 0.5f;
            float actual = renderer.Next(frame);
            if (frame >= 240)
            {
                Assert.Equal(expected, actual);
                auditionEnergy += Math.Abs(actual);
            }
        }

        Assert.True(auditionEnergy > 0);
        Assert.Equal(DrumTransportState.Stopped, mailbox.State);
        mailbox.SetMetallicEnabled(true);
        mailbox.Audition(Cr78Instrument.HiHat, 1);
        reference.Trigger(1, 12_000);
        renderer.BeginBlock(1_000);
        for (int frame = 1_000; frame < 2_000; frame++)
        {
            float expected = reference.Next() * 0.25f * 0.5f;
            float actual = renderer.Next(frame);
            if (frame >= 1_240)
            {
                Assert.Equal(expected, actual);
            }
        }

        mailbox.SetMasterVolume(0);
        Render(renderer, 2_000, 2_240);
        mailbox.Audition(Cr78Instrument.MetallicBeat, 1);
        Assert.Equal(0, Render(renderer, 2_240, 3_000));
    }

    /// <summary>
    ///  Equal musical snapshots do not invent pending revisions, and caller gain arrays cannot mutate them.
    /// </summary>
    [Fact]
    public void Configuration_CopiesGainsAndDoesNotInventMixerOnlyMusicalRevisions()
    {
        PercussionScore original = new(1, [new(0, 0, Cr78Instrument.Claves, 0.8f, GateSteps: 3)]);
        PercussionScore equivalent = new(1, original.Hits);
        PercussionMailbox mailbox = new(original, new Tempo(120));
        PercussionRenderer renderer = new(48_000, mailbox, 42);
        mailbox.Start();
        Render(renderer, 0, 1_000);
        long revision = mailbox.Settings.Revision;
        float[] volumes = Enumerable.Range(0, 13).Select(index => index / 12f).ToArray();
        mailbox.ApplyConfiguration(equivalent, new Tempo(120), 0.4f, volumes, false, false, 0.75f);
        PercussionSettings snapshot = mailbox.Settings;
        Assert.Equal(revision, snapshot.Revision);
        Array.Fill(volumes, float.NaN);
        for (int i = 0; i < Cr78Kit.Instruments.Count; i++)
        {
            Assert.Equal(i / 12f, mailbox.GetInstrumentVolume(Cr78Kit.Instruments[i]));
        }

        Assert.False(renderer.GetSnapshot(1_000, true).HasPendingChanges);
        Render(renderer, 1_000, 2_000);
        Assert.False(renderer.GetSnapshot(2_000, true).HasPendingChanges);
        mailbox.SetInstrumentVolume(Cr78Instrument.SnareDrum, 0.9f);
        mailbox.SetMetallicLevel(0.1f);
        Assert.Equal(1 / 12f, snapshot.InstrumentVolumes[(int)Cr78Instrument.SnareDrum]);
        Assert.Equal(0.75f, snapshot.MetallicLevel);
        Assert.Equal(revision, mailbox.Settings.Revision);
        mailbox.ApplyConfiguration(original.WithStep(0, Cr78Instrument.HiHat, 2, true),
            new Tempo(130), 0.4f, UnityVolumes(), false, false, 0.1f);
        Assert.Equal(revision + 1, mailbox.Settings.Revision);
    }

    /// <summary>
    ///  Invalid whole or single-fader edits fail before publishing any partial settings.
    /// </summary>
    [Fact]
    public void MixerValidation_RejectsNonfiniteLevelsWrongCountsAndMetallicAsAPrimaryFader()
    {
        PercussionScore score = new(1, []);
        PercussionMailbox mailbox = new(score, new Tempo(120));
        PercussionSettings original = mailbox.Settings;
        Assert.Throws<ArgumentException>(() =>
            mailbox.ApplyConfiguration(score, new Tempo(120), 1f, new float[12], true, true, 0f));
        Assert.Throws<ArgumentException>(() =>
            mailbox.ApplyConfiguration(score, new Tempo(120), 1f, new float[14], true, true, 0f));
        Assert.Throws<ArgumentNullException>(() =>
            mailbox.ApplyConfiguration(score, new Tempo(120), 1f, null!, true, true, 0f));
        Assert.Throws<ArgumentNullException>(() =>
            mailbox.ApplyConfiguration(null!, new Tempo(120), 1f, UnityVolumes(), true, true, 0f));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            mailbox.ApplyConfiguration(score, default, 1f, UnityVolumes(), true, true, 0f));
        foreach (float invalid in new[] { -0.01f, 1.01f, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.SetMasterVolume(invalid));
            Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.SetInstrumentVolume(Cr78Instrument.BassDrum, invalid));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                mailbox.ApplyConfiguration(score, new Tempo(90), invalid, UnityVolumes(), false, false, 0f, true));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                mailbox.ApplyConfiguration(score, new Tempo(90), 1f, UnityVolumes(), false, false, invalid, true));
            float[] invalidVolumes = UnityVolumes();
            invalidVolumes[12] = invalid;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                mailbox.ApplyConfiguration(score, new Tempo(90), 1f, invalidVolumes, false, false, 0f, true));
        }

        foreach (Cr78Instrument invalid in new[] { Cr78Instrument.MetallicBeat, (Cr78Instrument)(-1), (Cr78Instrument)99 })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.GetInstrumentVolume(invalid));
            Assert.Throws<ArgumentOutOfRangeException>(() => mailbox.SetInstrumentVolume(invalid, 0.5f));
        }

        Assert.Same(original, mailbox.Settings);
        Assert.Equal(DrumTransportState.Stopped, mailbox.State);
        Assert.Equal(0, mailbox.TryDrain(new PercussionCommand[PercussionMailbox.MaximumCommands], out _));
    }

    /// <summary>
    ///  Rapid finite target changes stay inside the calibrated sum bound and end at exact mute.
    /// </summary>
    [Fact]
    public void RapidRamps_StayFiniteBoundedAndEventuallyExactlySilent()
    {
        Cr78VoiceBank bank = new(48_000, 42);
        float[] gains = UnityVolumes();
        for (int block = 0; block < 512; block++)
        {
            Array.Fill(gains, block % 3 / 2f);
            bank.SetMix(block % 5 / 4f, gains, block % 2 == 0, block % 7 / 6f);
            bank.Trigger((Cr78Instrument)(block % 14), 1f, 12_000);
            for (int sample = 0; sample < 37; sample++)
            {
                float value = bank.Next();
                Assert.True(float.IsFinite(value));
                Assert.InRange(Math.Abs(value), 0f, 0.9051f);
            }
        }

        bank.SetMix(0, gains, false, 0);
        for (int sample = 0; sample < 240; sample++)
        {
            Assert.True(float.IsFinite(bank.Next()));
        }

        for (int sample = 0; sample < 1_000; sample++)
        {
            Assert.Equal(0, bank.Next());
        }
    }

    /// <summary>
    ///  Consuming gain snapshots, transport commands, auditions, and recurring hits allocates no audio objects.
    /// </summary>
    [Fact]
    public void MixAndTransportHandoffs_DoNotAllocateOnTheRenderingThread()
    {
        PercussionMailbox mailbox = new(new PercussionScore(1,
            Cr78Kit.Instruments.Select(instrument => new PercussionHit(0, 0, instrument, 0.5f))), new Tempo(1_000));
        PercussionRenderer renderer = new(32_000, mailbox, 42);
        mailbox.Start();
        Render(renderer, 0, 20_000);
        long cursor = 20_000;
        for (int edit = 0; edit < 66; edit++)
        {
            mailbox.SetMasterVolume(edit % 4 / 3f);
            mailbox.SetInstrumentVolume(Cr78Kit.Instruments[edit % 13], edit % 5 / 4f);
            mailbox.SetMetallicEnabled(edit % 2 == 0);
            mailbox.SetMetallicLevel(edit % 3 / 2f);
            mailbox.Pause();
            mailbox.Audition(Cr78Instrument.MetallicBeat, 0.5f);
            mailbox.Start();
            long before = GC.GetAllocatedBytesForCurrentThread();
            Render(renderer, cursor, cursor + 512);
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            if (edit >= 2)
            {
                Assert.Equal(0, allocated);
            }

            cursor += 512;
        }
    }

    private static float[] UnityVolumes() => Enumerable.Repeat(1f, Cr78Kit.Instruments.Count).ToArray();

    private static uint ChannelSeed(uint seed, Cr78Instrument instrument)
        => (seed ^ (0x9E3779B9u * (uint)((int)instrument + 1))) | 1u;

    private static double Render(PercussionRenderer renderer, long start, long end)
    {
        double energy = 0;
        for (long frame = start; frame < end; frame++)
        {
            if ((frame - start) % 128 == 0)
            {
                renderer.BeginBlock(frame);
            }

            energy += Math.Abs(renderer.Next(frame));
        }

        return energy;
    }
}
