using System.Numerics;
using System.Text.Json;

namespace WinForms.Audio.Percussion;

/// <summary>
///  Prepares original one-shots from fitted resonances, noise coloration and amplitude envelopes.
///  The embedded parameters contain neither PCM nor recorded spectral phases.
/// </summary>
internal static class MeasuredPercussionModel
{
    private static readonly Dictionary<string, Model> s_models = ReadModels();

    /// <summary>
    ///  Synthesizes a bounded strike off the render thread; legacy models use their existing DSP.
    /// </summary>
    internal static float[]? Prepare(Cr78Instrument instrument, int sampleRate, uint seed)
    {
        if (!s_models.TryGetValue(instrument.ToString(), out Model? model))
        {
            if (Cr78Kit.Instruments.Contains(instrument))
            {
                throw new InvalidDataException($"The fitted stock model for '{instrument}' is missing.");
            }

            return null;
        }

        int noiseCount = (int)Math.Ceiling(model.Duration * sampleRate);
        double duration = model.KickBody?.Duration ?? model.Duration;
        int count = (int)Math.Ceiling(duration * sampleRate);
        int size = (int)BitOperations.RoundUpToPowerOf2((uint)noiseCount);
        Complex[] noise = new Complex[size];
        Random random = new(unchecked((int)seed));
        for (int bin = 1; bin < size / 2; bin++)
        {
            double frequency = bin * (double)sampleRate / size;
            double gain = Interpolate(model.Spectrum, frequency);
            Complex value = new((random.NextDouble() * 2 - 1) * gain,
                (random.NextDouble() * 2 - 1) * gain);
            noise[bin] = value;
            noise[size - bin] = Complex.Conjugate(value);
        }

        InverseTransform(noise);
        double energy = 0;
        for (int i = 0; i < noiseCount; i++)
        {
            energy += noise[i].Real * noise[i].Real;
        }

        double normalization = energy > 0 ? Math.Sqrt(noiseCount / energy) : 0;
        float[] result = new float[count];
        float peak = 1;
        for (int i = 0; i < count; i++)
        {
            double time = i / (double)sampleRate;
            double sample = noise[i].Real * normalization * Interpolate(model.Envelope, time);
            if (model.ScrapeRate > 0)
            {
                double cycles = model.ScrapeRate * Math.Min(time, model.ScrapeSwitch)
                    + model.ScrapeRate2 * Math.Max(0, time - model.ScrapeSwitch);
                sample *= Math.Pow(Math.Max(0, Math.Cos(Math.Tau * (cycles - model.ScrapePhase))), model.ScrapePower);
            }

            foreach (Mode mode in model.Modes)
            {
                if (mode.Frequency < sampleRate * 0.45)
                {
                    double phase = mode.Phase + Math.Tau *
                        (mode.Frequency * time + mode.Chirp * time * time / 2);
                    sample += mode.Amplitude * Math.Exp(-time / mode.Decay) * Math.Cos(phase);
                }
            }

            if (model.KickBody is Mode kick)
            {
                // Preserve the fitted attack, but remove detuned modes and random bass after 12 ms.
                double blend = Math.Clamp((time - 0.004) / 0.008, 0, 1);
                blend *= blend * (3 - 2 * blend);
                double body = kick.Amplitude * Math.Exp(-time / kick.Decay)
                    * Math.Cos(kick.Phase + Math.Tau * kick.Frequency * time);
                sample = sample * (1 - blend) + body * blend;
            }

            double fade = model.KickBody is null ? 0.002 : 0.012;
            sample *= Math.Min(1, time / 0.00002) * Math.Min(1, (duration - time) / fade);
            result[i] = (float)sample;
            peak = Math.Max(peak, Math.Abs(result[i]));
        }

        for (int i = 0; i < count; i++)
        {
            result[i] /= peak;
        }

        return result;
    }

    private static Dictionary<string, Model> ReadModels()
    {
        using Stream stream = typeof(MeasuredPercussionModel).Assembly.GetManifestResourceStream(
            "WinForms.Audio.Percussion.MeasuredModels.json")
            ?? throw new InvalidOperationException("The stock percussion model parameters are missing.");
        return JsonSerializer.Deserialize<Dictionary<string, Model>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException("The stock percussion model parameters are empty.");
    }

    private static double Interpolate(double[][] points, double position)
    {
        if (position <= points[0][0])
        {
            return points[0][1];
        }

        int low = 0;
        int high = points.Length - 1;
        if (position > points[high][0])
        {
            return 0;
        }

        while (high - low > 1)
        {
            int middle = (low + high) / 2;
            if (position < points[middle][0])
            {
                high = middle;
            }
            else
            {
                low = middle;
            }
        }

        double fraction = (position - points[low][0]) / (points[high][0] - points[low][0]);
        return points[low][1] + fraction * (points[high][1] - points[low][1]);
    }

    private static void InverseTransform(Complex[] values)
    {
        int size = values.Length;
        for (int i = 1, reversed = 0; i < size; i++)
        {
            int bit = size >> 1;
            for (; (reversed & bit) != 0; bit >>= 1)
            {
                reversed ^= bit;
            }

            reversed ^= bit;
            if (i < reversed)
            {
                (values[i], values[reversed]) = (values[reversed], values[i]);
            }
        }

        for (int length = 2; length <= size; length <<= 1)
        {
            Complex step = Complex.FromPolarCoordinates(1, Math.Tau / length);
            for (int start = 0; start < size; start += length)
            {
                Complex rotation = Complex.One;
                for (int i = 0; i < length / 2; i++)
                {
                    Complex lower = values[start + i];
                    Complex upper = values[start + i + length / 2] * rotation;
                    values[start + i] = lower + upper;
                    values[start + i + length / 2] = lower - upper;
                    rotation *= step;
                }
            }
        }

        // Overall scale is immaterial: preparation normalizes the resulting noise RMS.
    }

    private sealed class Model
    {
        public double Duration { get; set; }
        public Mode[] Modes { get; set; } = [];
        public Mode? KickBody { get; set; }
        public double[][] Spectrum { get; set; } = [];
        public double[][] Envelope { get; set; } = [];
        public double ScrapeRate { get; set; }
        public double ScrapeRate2 { get; set; }
        public double ScrapeSwitch { get; set; }
        public double ScrapePhase { get; set; }
        public double ScrapePower { get; set; }
    }

    private sealed class Mode
    {
        public double Duration { get; set; }
        public double Frequency { get; set; }
        public double Decay { get; set; }
        public double Chirp { get; set; }
        public double Amplitude { get; set; }
        public double Phase { get; set; }
    }
}
