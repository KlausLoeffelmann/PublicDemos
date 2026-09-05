using WinForms.Audio.Core;
using WinForms.Audio.Percussion;

namespace SplitFlap.Tests;

/// <summary>
///  Endpoint-free coverage of the documented percussion targets and reusable synthesis state.
/// </summary>
public sealed class Cr78SynthesisTests
{
    /// <summary>
    ///  Keeps the thirteen percussion sounds distinct from the metallic-layer audition.
    /// </summary>
    [Fact]
    public void Catalog_ContainsFullPaletteAndSeparateMetallicLayer()
    {
        Assert.Equal(13, Cr78Kit.Instruments.Count);
        Assert.Equal(14, Enum.GetValues<Cr78Instrument>().Length);
        Assert.DoesNotContain(Cr78Instrument.MetallicBeat, Cr78Kit.Instruments);
        Assert.Equal(13, Cr78Kit.Instruments.Distinct().Count());
        Assert.Equal(14, Enum.GetValues<Cr78Instrument>().Select(Cr78Kit.GetDisplayName).Distinct().Count());
        Assert.Contains("layer", Cr78Kit.GetDisplayName(Cr78Instrument.MetallicBeat));
        Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.GetDisplayName((Cr78Instrument)99));
        Assert.Throws<NotSupportedException>(() =>
            ((IList<Cr78Instrument>)Cr78Kit.Instruments)[0] = Cr78Instrument.Guiro);
    }

    /// <summary>
    ///  Verifies that every catalog entry produces finite, bounded, distinct audio and exact final silence.
    /// </summary>
    [Theory]
    [InlineData(32_000)]
    [InlineData(44_100)]
    [InlineData(48_000)]
    [InlineData(96_000)]
    public void EverySound_IsDistinctBoundedAndTerminates(int sampleRate)
    {
        HashSet<ulong> signatures = [];
        foreach (Cr78Instrument instrument in Enum.GetValues<Cr78Instrument>())
        {
            IVoice voice = Cr78Kit.CreateVoice(sampleRate, instrument, 1, 0, 123);
            ulong signature = 14695981039346656037UL;
            float peak = 0;
            int count = 0;
            while (!voice.IsFinished)
            {
                float sample = voice.Next();
                Assert.True(float.IsFinite(sample), instrument.ToString());
                Assert.InRange(Math.Abs(sample), 0, Cr78Preset.For(instrument).Level + 1e-6f);
                peak = Math.Max(peak, Math.Abs(sample));
                signature = unchecked((signature ^ (uint)BitConverter.SingleToInt32Bits(sample)) * 1099511628211UL);
                Assert.True(++count < sampleRate * 3, $"{instrument} did not terminate.");
            }

            Assert.True(peak > 0.001f, $"{instrument} was effectively silent.");
            Assert.True(signatures.Add(signature), $"{instrument} duplicated another model.");
            for (int i = 0; i < 512; i++)
            {
                Assert.Equal(0f, voice.Next());
            }
        }
    }

    /// <summary>
    ///  Rejects malformed rates, sound identifiers, levels, and deterministic seeds explicitly.
    /// </summary>
    [Fact]
    public void InvalidModelArguments_AreRejected()
    {
        foreach (int sampleRate in new[] { -1, 0, 8_000, 22_050, 24_000, 192_001 })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.CreateVoice(sampleRate, Cr78Instrument.HiHat));
        }

        foreach (float level in new[] { -0.01f, 1.01f, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.CreateVoice(48_000, Cr78Instrument.HiHat, level));
            Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.CreateVoice(48_000, Cr78Instrument.HiHat, 1, level));
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.CreateVoice(48_000, (Cr78Instrument)(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.CreateVoice(48_000, (Cr78Instrument)14));
        Assert.Throws<ArgumentOutOfRangeException>(() => Cr78Kit.CreateVoice(48_000, Cr78Instrument.Cymbal, 1, 0, 0));
        IVoice silent = Cr78Kit.CreateVoice(48_000, Cr78Instrument.BassDrum, velocity: 0);
        Assert.True(silent.IsFinished);
        Assert.Equal(0, silent.Next());
    }

    /// <summary>
    ///  Measures late modal zero crossings and the scanned V-to-V/10 decay convention.
    /// </summary>
    [Theory]
    [InlineData(Cr78Instrument.BassDrum, 62.5, 100)]
    [InlineData(Cr78Instrument.HighBongo, 600, 40)]
    [InlineData(Cr78Instrument.LowBongo, 400, 40)]
    [InlineData(Cr78Instrument.LowConga, 208, 150)]
    [InlineData(Cr78Instrument.Claves, 2630, 18)]
    [InlineData(Cr78Instrument.RimShot, 1480, 5)]
    public void DampedModes_FollowNominalFrequencyAndD20(Cr78Instrument instrument, double frequency, double d20Milliseconds)
    {
        const int sampleRate = 48_000;
        float[] samples = Render(Cr78Kit.CreateVoice(sampleRate, instrument, 1, 0, 123), sampleRate);
        int d20 = (int)Math.Round(sampleRate * d20Milliseconds / 1_000);
        List<double> crossings = [];
        for (int i = Math.Max(1, (int)(d20 * 0.8)); i < d20 * 2.8; i++)
        {
            if (samples[i - 1] <= 0 && samples[i] > 0)
            {
                crossings.Add(i - 1 + -samples[i - 1] / (double)(samples[i] - samples[i - 1]));
            }
        }

        Assert.True(crossings.Count >= 5);
        double measured = (crossings.Count - 1) * sampleRate / (crossings[^1] - crossings[0]);
        Assert.InRange(measured, frequency * 0.985, frequency * 1.015);
        int window = Math.Max(16, (int)Math.Round(sampleRate * 2 / frequency));
        double initial = Rms(samples.AsSpan(d20 / 2, window));
        double decayed = Rms(samples.AsSpan(d20 / 2 + d20, window));
        Assert.InRange(decayed / initial, 0.07, 0.14);
        Assert.InRange(samples.Length, d20 * 5 - 1, d20 * 5 + 1);
    }

    /// <summary>
    ///  Finds the snare's tuned body under its unpitched noise burst.
    /// </summary>
    [Fact]
    public void Snare_HasTheDocumentedBodyResonance()
    {
        float[] samples = Render(Cr78Kit.CreateVoice(48_000, Cr78Instrument.SnareDrum, 1, 0, 55), 48_000);
        double body = Magnitude(samples, 48_000, 340);
        Assert.True(body > Magnitude(samples, 48_000, 265) * 2);
        Assert.True(body > Magnitude(samples, 48_000, 425) * 2);
    }

    /// <summary>
    ///  Checks both non-harmonic cowbell oscillator targets rather than only a generic bell peak.
    /// </summary>
    [Fact]
    public void Cowbell_ContainsBothFactoryComponents()
    {
        float[] samples = Render(Cr78Kit.CreateVoice(48_000, Cr78Instrument.Cowbell, 1, 0, 55), 48_000);
        double between = Magnitude(samples, 48_000, 670);
        Assert.True(Magnitude(samples, 48_000, 555) > between * 3);
        Assert.True(Magnitude(samples, 48_000, 800) > between * 3);
    }

    /// <summary>
    ///  Checks the three factory metallic frequencies at both common output rates.
    /// </summary>
    [Theory]
    [InlineData(44_100)]
    [InlineData(48_000)]
    public void MetallicLayer_ContainsThreeInharmonicOscillators(int sampleRate)
    {
        float[] samples = Render(Cr78Kit.CreateVoice(sampleRate, Cr78Instrument.MetallicBeat, 1, 0, 55), sampleRate);
        foreach (double frequency in new[] { 6_170d, 5_620d, 4_080d })
        {
            double peak = Magnitude(samples, sampleRate, frequency);
            double neighbor = Magnitude(samples, sampleRate, frequency + 160);
            Assert.True(peak > neighbor * 3);
        }
    }

    /// <summary>
    ///  Verifies that procedurally prepared bright carriers exclude folded high partials.
    /// </summary>
    [Theory]
    [InlineData(32_000)]
    [InlineData(44_100)]
    [InlineData(48_000)]
    public void BrightCarrier_IsPreparedWithoutOutOfBandHarmonics(int sampleRate)
    {
        const double frequency = 6_170;
        Cr78WaveTable table = new(sampleRate, frequency, pulse: true);
        Cr78Oscillator oscillator = new(table, frequency, sampleRate);
        float[] samples = new float[sampleRate];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = oscillator.Next();
        }

        double fundamental = Magnitude(samples, sampleRate, frequency);
        double fifthAlias = Math.Abs(5 * frequency - sampleRate);
        Assert.True(Magnitude(samples, sampleRate, fifthAlias) < fundamental * 0.002);
    }

    /// <summary>
    ///  Keeps seeded runs repeatable while subsequent hits consume fresh continuing noise.
    /// </summary>
    [Fact]
    public void Noise_IsDeterministicButNotIdenticalOnEveryRetrigger()
    {
        Cr78Generator first = new(48_000, Cr78Instrument.HiHat, 42);
        Cr78Generator second = new(48_000, Cr78Instrument.HiHat, 42);
        float[] firstHit = new float[1_024];
        float[] secondHit = new float[1_024];
        for (int hit = 0; hit < 2; hit++)
        {
            first.Trigger(1, 6_000);
            second.Trigger(1, 6_000);
            for (int i = 0; i < 20_000; i++)
            {
                float sample = first.Next();
                Assert.Equal(sample, second.Next());
                if (i < firstHit.Length)
                {
                    (hit == 0 ? firstHit : secondHit)[i] = sample;
                }
            }
        }

        Assert.False(firstHit.SequenceEqual(secondHit));
    }

    /// <summary>
    ///  Ensures the metallic control changes only hi-hat/cymbal and leaves their dry source deterministic.
    /// </summary>
    [Fact]
    public void MetallicControl_LayersOnlyOntoHiHatAndCymbal()
    {
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            IVoice dry = Cr78Kit.CreateVoice(48_000, instrument, 1, 0, 777);
            IVoice layered = Cr78Kit.CreateVoice(48_000, instrument, 1, 0.8f, 777);
            bool changed = false;
            for (int i = 0; i < 16_000; i++)
            {
                float left = dry.Next();
                float right = layered.Next();
                changed |= left != right;
                if (i >= 12_000)
                {
                    Assert.Equal(left, right);
                }
            }

            Assert.Equal(instrument is Cr78Instrument.HiHat or Cr78Instrument.Cymbal, changed);
        }
    }

    /// <summary>
    ///  Covers natural guiro gate release, explicit cancellation, and click-free modal retriggering.
    /// </summary>
    [Fact]
    public void RetriggerGateAndRelease_KeepTailsBoundedAndReachSilence()
    {
        Cr78Generator conga = new(48_000, Cr78Instrument.LowConga, 42);
        conga.Trigger(1, 1);
        float preceding = 0;
        for (int i = 0; i < 517; i++)
        {
            preceding = conga.Next();
        }

        conga.Trigger(0.7f, 1);
        Assert.InRange(Math.Abs(conga.Next() - preceding), 0, 0.005f);
        for (int i = 0; i < 500; i++)
        {
            conga.Trigger(1, 1);
            Assert.InRange(Math.Abs(conga.Next()), 0, Cr78Preset.For(Cr78Instrument.LowConga).Level);
        }

        conga.Release();
        for (int i = 0; i < 240; i++)
        {
            Assert.True(float.IsFinite(conga.Next()));
        }

        Assert.False(conga.IsActive);
        for (int i = 0; i < 48_000; i++)
        {
            Assert.Equal(0, conga.Next());
        }

        Cr78Generator shortScrape = new(48_000, Cr78Instrument.Guiro, 55);
        Cr78Generator longScrape = new(48_000, Cr78Instrument.Guiro, 55);
        shortScrape.Trigger(1, 2_000);
        longScrape.Trigger(1, 20_000);
        for (int i = 0; i < 12_000; i++)
        {
            shortScrape.Next();
            longScrape.Next();
        }

        Assert.False(shortScrape.IsActive);
        Assert.True(longScrape.IsActive);
        longScrape.Release();
        for (int i = 0; i < 240; i++)
        {
            longScrape.Next();
        }

        Assert.False(longScrape.IsActive);
    }

    /// <summary>
    ///  Keeps the complete bank headroom-safe, including retriggers and its shared metallic channel.
    /// </summary>
    [Fact]
    public void CompleteBank_LeavesHeadroomWithoutPerHitDspAllocation()
    {
        Cr78VoiceBank bank = new(48_000, 42);
        for (int i = 0; i < 512; i++)
        {
            bank.Trigger((Cr78Instrument)(i % 14), 1, 8_000);
            bank.Next();
        }

        double checksum = 0;
        float peak = 0;
        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int hit = 0; hit < 2_048; hit++)
        {
            bank.Trigger((Cr78Instrument)(hit % 14), 1, 8_000);
            for (int i = 0; i < 32; i++)
            {
                float sample = bank.Next();
                peak = Math.Max(peak, Math.Abs(sample));
                checksum += sample;
            }
        }

        bank.ReleaseAll();
        for (int i = 0; i < 24_000; i++)
        {
            checksum += bank.Next();
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
        Assert.True(double.IsFinite(checksum));
        Assert.InRange(peak, 0.001f, 0.9051f);
        Assert.False(bank.IsActive);
    }

    /// <summary>
    ///  Flushes filter feedback before subnormal values can persist during a stopped session.
    /// </summary>
    [Fact]
    public void Filter_LongIdleBecomesExactZero()
    {
        Cr78NoiseFilter filter = new(48_000, 30, 5_000);
        Assert.NotEqual(0, filter.Next(1));
        for (int i = 0; i < 48_000; i++)
        {
            filter.Next(0);
        }

        for (int i = 0; i < 48_000; i++)
        {
            Assert.Equal(0, filter.Next(0));
        }
    }

    private static float[] Render(IVoice voice, int sampleRate)
    {
        List<float> samples = new(sampleRate);
        while (!voice.IsFinished)
        {
            samples.Add(voice.Next());
            Assert.True(samples.Count <= sampleRate * 3);
        }

        return samples.ToArray();
    }

    private static double Rms(ReadOnlySpan<float> samples)
    {
        double sum = 0;
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        return Math.Sqrt(sum / samples.Length);
    }

    private static double Magnitude(ReadOnlySpan<float> samples, int sampleRate, double frequency)
    {
        double real = 0;
        double imaginary = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            double angle = Math.Tau * frequency * i / sampleRate;
            real += samples[i] * Math.Cos(angle);
            imaginary -= samples[i] * Math.Sin(angle);
        }

        return Math.Sqrt(real * real + imaginary * imaginary) / samples.Length;
    }
}
