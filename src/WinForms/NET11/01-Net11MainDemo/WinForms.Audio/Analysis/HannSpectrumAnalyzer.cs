namespace WinForms.Audio.Analysis;

/// <summary>
///  Computes a readable managed radix-2 FFT using cached permutation, window, and twiddle tables.
/// </summary>
internal sealed class HannSpectrumAnalyzer
{
    private readonly int _size;
    private readonly float _minimumDecibels;
    private readonly int[] _reversed;
    private readonly double[] _window;
    private readonly double[] _cosine;
    private readonly double[] _sine;
    private readonly double[] _real;
    private readonly double[] _imaginary;
    private readonly double _windowSum;

    /// <summary>
    ///  Prepares reusable analysis storage and all trigonometric coefficients.
    /// </summary>
    internal HannSpectrumAnalyzer(AudioSpectrumOptions options)
    {
        options.Validate();
        _size = options.FftSize;
        _minimumDecibels = options.MinimumDecibels;
        _reversed = new int[_size];
        _window = new double[_size];
        _cosine = new double[_size / 2];
        _sine = new double[_size / 2];
        _real = new double[_size];
        _imaginary = new double[_size];

        for (int i = 0; i < _size; i++)
        {
            int value = i;
            int reversed = 0;
            for (int bit = _size >> 1; bit != 0; bit >>= 1)
            {
                reversed = (reversed << 1) | (value & 1);
                value >>= 1;
            }

            _reversed[i] = reversed;
            // The periodic Hann is natural for an FFT. Its sum is the coherent gain:
            // dividing magnitudes by that sum restores a bin-centred sine's amplitude.
            _window[i] = 0.5 - 0.5 * Math.Cos(2 * Math.PI * i / _size);
            _windowSum += _window[i];
        }

        for (int i = 0; i < _cosine.Length; i++)
        {
            double angle = -2 * Math.PI * i / _size;
            _cosine[i] = Math.Cos(angle);
            _sine[i] = Math.Sin(angle);
        }
    }

    /// <summary>
    ///  Analyzes final interleaved PCM and matching pre-clamp samples without allocating per window.
    /// </summary>
    internal SpectrumLevels Analyze(
        ReadOnlySpan<short> pcm, ReadOnlySpan<float> preClamp, AudioFormat format, Span<float> decibels)
    {
        if (format.Channels <= 0 || format.SampleRate <= 0 || pcm.Length != _size * format.Channels ||
            preClamp.Length != _size || decibels.Length != _size / 2 + 1)
        {
            throw new ArgumentException("Spectrum input and output must match the configured FFT size and audio format.");
        }

        double sumSquares = 0;
        double peak = 0;
        long clipped = 0;
        for (int frame = 0; frame < _size; frame++)
        {
            if (!float.IsFinite(preClamp[frame]))
            {
                throw new InvalidOperationException("Spectrum monitoring encountered a non-finite pre-clamp audio sample.");
            }

            double sample = 0;
            for (int channel = 0; channel < format.Channels; channel++)
            {
                sample += pcm[frame * format.Channels + channel];
            }

            // Average channels before analysis, and use signed PCM's 32768 full-scale reference.
            // The engine currently duplicates mono; averaging also defines the stereo contract.
            sample /= 32768.0 * format.Channels;
            sumSquares += sample * sample;
            peak = Math.Max(peak, Math.Abs(preClamp[frame] / 32768.0));
            if (preClamp[frame] < short.MinValue || preClamp[frame] > short.MaxValue)
            {
                clipped += format.Channels;
            }

            int target = _reversed[frame];
            _real[target] = sample * _window[frame];
            _imaginary[target] = 0;
        }

        // Each pass combines pairs of smaller transforms. Cached sine/cosine values rotate
        // the upper half; the sum and difference become the next, twice-as-large transform.
        for (int length = 2; length <= _size; length <<= 1)
        {
            int half = length / 2;
            int step = _size / length;
            for (int start = 0; start < _size; start += length)
            {
                for (int i = 0; i < half; i++)
                {
                    int lower = start + i;
                    int upper = lower + half;
                    int twiddle = i * step;
                    double real = _real[upper] * _cosine[twiddle] - _imaginary[upper] * _sine[twiddle];
                    double imaginary = _real[upper] * _sine[twiddle] + _imaginary[upper] * _cosine[twiddle];
                    _real[upper] = _real[lower] - real;
                    _imaginary[upper] = _imaginary[lower] - imaginary;
                    _real[lower] += real;
                    _imaginary[lower] += imaginary;
                }
            }
        }

        int peakBin = 0;
        double peakMagnitude = 0;
        for (int bin = 0; bin < decibels.Length; bin++)
        {
            double magnitude = Math.Sqrt(_real[bin] * _real[bin] + _imaginary[bin] * _imaginary[bin]) / _windowSum;
            if (bin != 0 && bin != _size / 2)
            {
                // Real signals share energy between positive and negative frequencies.
                // DC and Nyquist are their own partners and must not be doubled.
                magnitude *= 2;
            }

            decibels[bin] = ToDecibels(magnitude);
            // Hann's immediate neighbor can tie an endpoint after one-sided scaling.
            // Prefer DC/Nyquist to that neighbor rather than letting round-off move a DC peak.
            if (magnitude > peakMagnitude * (1 + 1e-12) ||
                (bin == _size / 2 && magnitude > 0 && magnitude >= peakMagnitude * (1 - 1e-12)))
            {
                peakMagnitude = magnitude;
                peakBin = bin;
            }
        }

        return new(
            (float)((double)peakBin * format.SampleRate / _size),
            ToDecibels(peak),
            ToDecibels(Math.Sqrt(sumSquares / _size)),
            clipped);
    }

    private float ToDecibels(double amplitude)
        => amplitude > 0 ? Math.Max(_minimumDecibels, (float)(20 * Math.Log10(amplitude))) : _minimumDecibels;
}

/// <summary>
///  Contains window statistics computed alongside an FFT rather than on the audio producer.
/// </summary>
/// <param name="PeakFrequency">Strongest bin's frequency in Hz.</param>
/// <param name="PeakLevel">Pre-clamp peak in dBFS.</param>
/// <param name="RmsLevel">Post-conversion RMS in dBFS.</param>
/// <param name="ClippedSamples">Number of clamped interleaved samples in this window.</param>
internal readonly record struct SpectrumLevels(float PeakFrequency, float PeakLevel, float RmsLevel, long ClippedSamples);
