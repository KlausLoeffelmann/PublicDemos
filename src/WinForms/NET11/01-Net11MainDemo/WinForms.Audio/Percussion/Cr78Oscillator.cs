namespace WinForms.Audio.Percussion;

/// <summary>
///  A procedurally prepared finite Fourier series, never a recorded waveform.
/// </summary>
internal sealed class Cr78WaveTable
{
    private const int TableLength = 2_048;
    private readonly float[] _samples = new float[TableLength + 1];

    /// <summary>
    ///  Prepares a sine or odd-harmonic pulse with every partial below 45 percent of the rate.
    /// </summary>
    internal Cr78WaveTable(int sampleRate, double frequency, bool pulse)
    {
        int lastHarmonic = pulse ? Math.Min(31, (int)(sampleRate * 0.45 / frequency)) : 1;
        double normalization = 0;
        for (int harmonic = 1; harmonic <= lastHarmonic; harmonic += 2)
        {
            // A finite, gently tapered odd-harmonic sum is a band-limited square-like tone.
            // Keeping a Nyquist margin also leaves room for its decaying amplitude envelope.
            double weight = pulse
                ? Math.Cos((harmonic - 1) * Math.PI / (2 * (lastHarmonic + 1))) / harmonic
                : 1;
            normalization += weight;
            for (int i = 0; i < TableLength; i++)
            {
                _samples[i] += (float)(weight * Math.Sin(Math.Tau * harmonic * i / TableLength));
            }
        }

        for (int i = 0; i < TableLength; i++)
        {
            _samples[i] /= (float)normalization;
        }

        _samples[TableLength] = _samples[0];
    }

    /// <summary>
    ///  Reads a normalized phase with linear interpolation and a bounded output.
    /// </summary>
    internal float At(double phase)
    {
        double position = phase * TableLength;
        int index = (int)position;
        float fraction = (float)(position - index);
        return _samples[index] + (_samples[index + 1] - _samples[index]) * fraction;
    }
}

/// <summary>
///  A damped modal carrier's phase state; its amplitude decay is supplied by the sound envelope.
/// </summary>
internal struct Cr78Oscillator
{
    private readonly Cr78WaveTable? _table;
    private readonly double _increment;
    private double _phase;

    /// <summary>
    ///  Associates independent phase state with a shared immutable waveform table.
    /// </summary>
    internal Cr78Oscillator(Cr78WaveTable? table, double frequency, int sampleRate)
    {
        _table = table;
        _increment = frequency / sampleRate;
    }

    /// <summary>
    ///  Starts a damped mode at a zero crossing without rebuilding its waveform.
    /// </summary>
    internal void Reset()
        => _phase = 0;

    /// <summary>
    ///  Advances one phase step, avoiding trigonometry and allocation during rendering.
    /// </summary>
    internal float Next()
    {
        if (_table is null)
        {
            return 0;
        }

        float sample = _table.At(_phase);
        _phase += _increment;
        if (_phase >= 1)
        {
            _phase -= 1;
        }

        return sample;
    }
}

/// <summary>
///  A stable high-pass followed by two low-pass stages, with explicitly cleared quiet state.
/// </summary>
internal struct Cr78NoiseFilter
{
    private readonly double _highCoefficient;
    private readonly double _lowCoefficient;
    private double _highMemory;
    private double _lowMemory;
    private double _secondLowMemory;

    /// <summary>
    ///  Prepares the exponential one-pole coefficients outside the sample loop.
    /// </summary>
    internal Cr78NoiseFilter(int sampleRate, double highPassHz, double lowPassHz)
    {
        _highCoefficient = 1 - Math.Exp(-Math.Tau * highPassHz / sampleRate);
        _lowCoefficient = 1 - Math.Exp(-Math.Tau * lowPassHz / sampleRate);
    }

    /// <summary>
    ///  Colors a sample; the half-scale subtraction keeps bounded input bounded.
    /// </summary>
    internal float Next(float input)
    {
        _highMemory += _highCoefficient * (input - _highMemory);
        double highPassed = (input - _highMemory) * 0.5;
        _lowMemory += _lowCoefficient * (highPassed - _lowMemory);
        _secondLowMemory += _lowCoefficient * (_lowMemory - _secondLowMemory);

        // Recursive state below -360 dB is not useful audio. Clear it well before subnormal
        // floating-point values become possible, including a filter fed a long run of zeros.
        _highMemory = Flush(_highMemory);
        _lowMemory = Flush(_lowMemory);
        _secondLowMemory = Flush(_secondLowMemory);
        return (float)_secondLowMemory;
    }

    /// <summary>
    ///  Removes feedback residue once the parent sound has finished.
    /// </summary>
    internal void Clear()
        => _highMemory = _lowMemory = _secondLowMemory = 0;

    private static double Flush(double value)
        => Math.Abs(value) < 1e-18 ? 0 : value;
}
