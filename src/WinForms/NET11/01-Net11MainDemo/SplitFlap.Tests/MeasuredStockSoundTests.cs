using System.Numerics;
using WinForms.Audio.Core;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Retains reference measurements rather than temporary recordings as regression fixtures.
/// </summary>
public sealed class MeasuredStockSoundTests
{
    /// <summary>
    ///  Stable enum IDs beyond the old palette cannot collide with the following step's index.
    /// </summary>
    [Fact]
    public void ExpandedPalette_IndexesAdjacentStepsWithoutCollisions()
    {
        PercussionHit[] hits =
        [
            new(0, 0, Cr78Instrument.BassDrumAccent),
            new(0, 0, Cr78Instrument.Guiro2),
            new(0, 1, Cr78Instrument.SnareDrum),
            new(0, 1, Cr78Instrument.BassDrum)
        ];
        PercussionScore score = new(1, hits);
        Assert.Equal(4, score.Hits.Count);
        Assert.Equal(2, score.GetStepHits(0, 0).Length);
        Assert.Equal(2, score.GetStepHits(0, 1).Length);
        Assert.True(score.HasHit(0, Cr78Instrument.BassDrumAccent, 0));
        Assert.True(score.HasHit(0, Cr78Instrument.SnareDrum, 1));
    }

    /// <summary>
    ///  Bounds full-signal spectral centroid and T90 against the supplied isolated recordings.
    /// </summary>
    [Theory]
    [InlineData(Cr78Instrument.BassDrum, 92, .053)]
    [InlineData(Cr78Instrument.BassDrumAccent, 108, .045)]
    [InlineData(Cr78Instrument.SnareDrum, 2401, .041)]
    [InlineData(Cr78Instrument.SnareDrumAccent, 1798, .029)]
    [InlineData(Cr78Instrument.RimShot, 1898, .0013)]
    [InlineData(Cr78Instrument.HiHat, 8215, .013)]
    [InlineData(Cr78Instrument.HiHatAccent, 8119, .012)]
    [InlineData(Cr78Instrument.MetallicHiHat, 5644, .034)]
    [InlineData(Cr78Instrument.Cymbal, 8681, .242)]
    [InlineData(Cr78Instrument.HighBongo, 604, .018)]
    [InlineData(Cr78Instrument.LowBongo, 980, .009)]
    [InlineData(Cr78Instrument.LowConga, 204, .070)]
    [InlineData(Cr78Instrument.Cowbell, 714, .042)]
    [InlineData(Cr78Instrument.Tambourine, 4767, .101)]
    [InlineData(Cr78Instrument.Tambourine2, 4725, .158)]
    [InlineData(Cr78Instrument.Guiro, 6198, .242)]
    [InlineData(Cr78Instrument.Guiro2, 6123, .733)]
    public void StockModels_MatchReferenceEnergyAndSpectralMeasurements(
        Cr78Instrument instrument, double referenceCentroid, double referenceEnergy90)
    {
        const int rate = 48_000;
        IVoice voice = Cr78Kit.CreateVoice(rate, instrument, 1, 0, 42);
        List<float> samples = [];
        while (!voice.IsFinished)
        {
            samples.Add(voice.Next());
            Assert.True(samples.Count < rate * 2);
        }

        double energy = samples.Sum(sample => (double)sample * sample);
        double accumulated = 0;
        int energy90 = 0;
        while (accumulated < energy * .9)
        {
            accumulated += (double)samples[energy90] * samples[energy90];
            energy90++;
        }

        double t90 = energy90 / (double)rate;
        Assert.InRange(t90, referenceEnergy90 * .7, referenceEnergy90 * 1.3);
        double centroid = SpectralCentroid(samples, rate);
        Assert.InRange(centroid, referenceCentroid * .75, referenceCentroid * 1.25);
    }

    /// <summary>
    ///  Kick bodies have one stable pitch, monotonically decaying peaks, and no extended ringing tail.
    /// </summary>
    [Theory]
    [InlineData(Cr78Instrument.BassDrum, 32_000, .160, .015)]
    [InlineData(Cr78Instrument.BassDrum, 48_000, .160, .015)]
    [InlineData(Cr78Instrument.BassDrum, 96_000, .160, .015)]
    [InlineData(Cr78Instrument.BassDrum, 192_000, .160, .015)]
    [InlineData(Cr78Instrument.BassDrumAccent, 32_000, .140, .007)]
    [InlineData(Cr78Instrument.BassDrumAccent, 48_000, .140, .007)]
    [InlineData(Cr78Instrument.BassDrumAccent, 96_000, .140, .007)]
    [InlineData(Cr78Instrument.BassDrumAccent, 192_000, .140, .007)]
    public void Kicks_HaveShortDryBodiesWithoutVibrato(
        Cr78Instrument instrument, int rate, double duration, double maximumLateEnergy)
    {
        IVoice voice = Cr78Kit.CreateVoice(rate, instrument, 1, 0, 42);
        float[] samples = new float[(int)Math.Ceiling(duration * rate) + 2];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = voice.Next();
        }

        Assert.True(voice.IsFinished);
        Assert.Equal(0, samples[^1]);
        Assert.InRange(Math.Abs(samples[^3]), 0, 1e-5f);
        double energy = samples.Sum(value => (double)value * value);
        double lateEnergy = samples.Skip(rate / 10).Sum(value => (double)value * value);
        Assert.InRange(lateEnergy / energy, 0, maximumLateEnergy);
        List<double> crossings = [];
        List<float> peaks = [];
        for (int i = rate / 50; i < (duration - .013) * rate; i++)
        {
            if (samples[i] <= 0 && samples[i + 1] > 0)
            {
                double crossing = i - samples[i] / (double)(samples[i + 1] - samples[i]);
                if (crossings.Count > 0)
                {
                    Assert.InRange(rate / (crossing - crossings[^1]), 59.9, 60.1);
                }

                crossings.Add(crossing);
            }

            if (samples[i] > samples[i - 1] && samples[i] >= samples[i + 1])
            {
                if (peaks.Count > 0)
                {
                    Assert.True(samples[i] < peaks[^1] * .75f);
                }

                peaks.Add(samples[i]);
            }
        }

        Assert.True(crossings.Count >= 5);
        Assert.True(peaks.Count >= 5);
    }

    private static double SpectralCentroid(List<float> samples, int rate)
    {
        int size = (int)BitOperations.RoundUpToPowerOf2((uint)samples.Count);
        Complex[] spectrum = new Complex[size];
        for (int i = 0; i < samples.Count; i++)
        {
            spectrum[i] = new Complex(samples[i], 0);
        }

        for (int i = 1, j = 0; i < size; i++)
        {
            int bit = size >> 1;
            for (; (j & bit) != 0; bit >>= 1)
            {
                j ^= bit;
            }

            j ^= bit;
            if (i < j)
            {
                (spectrum[i], spectrum[j]) = (spectrum[j], spectrum[i]);
            }
        }

        for (int length = 2; length <= size; length *= 2)
        {
            Complex step = Complex.FromPolarCoordinates(1, -Math.Tau / length);
            for (int start = 0; start < size; start += length)
            {
                Complex rotation = Complex.One;
                for (int i = 0; i < length / 2; i++)
                {
                    Complex even = spectrum[start + i];
                    Complex odd = spectrum[start + i + length / 2] * rotation;
                    spectrum[start + i] = even + odd;
                    spectrum[start + i + length / 2] = even - odd;
                    rotation *= step;
                }
            }
        }

        double weighted = 0;
        double total = 0;
        for (int bin = 0; bin <= size / 2; bin++)
        {
            double power = spectrum[bin].Real * spectrum[bin].Real + spectrum[bin].Imaginary * spectrum[bin].Imaginary;
            weighted += power * bin * rate / size;
            total += power;
        }

        return weighted / total;
    }
}
