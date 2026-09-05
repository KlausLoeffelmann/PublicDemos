namespace WinForms.Audio.Synthesis;

/// <summary>
///  The basic wave shapes. All naive (no band limiting), which aliases above a few kHz; that is
///  the sound of every 8-bit machine you remember, so for a retro demo it's a feature.
/// </summary>
public enum Waveform
{
    /// <summary>
    ///  Pure tone. The one from the first slide.
    /// </summary>
    Sine,

    /// <summary>
    ///  Hollow, clarinet-ish. Only odd harmonics.
    /// </summary>
    Square,

    /// <summary>
    ///  Soft, flute-ish. Odd harmonics, falling off fast.
    /// </summary>
    Triangle,

    /// <summary>
    ///  Bright, brassy. All harmonics.
    /// </summary>
    Sawtooth,

    /// <summary>
    ///  A square with an adjustable duty cycle (<see cref="Oscillator.PulseWidth"/>). Thin and nasal at 10 %.
    /// </summary>
    Pulse
}

/// <summary>
///  A phase accumulator that reads a waveform. <c>phase += frequency / sampleRate</c>, wrap at 1, done.
/// </summary>
public sealed class Oscillator(int sampleRate)
{
    private double _phase;
    private double _frequency = 440;
    private double _renderedFrequency = double.NaN;
    private double _phaseIncrement;

    /// <summary>
    ///  Frequency in Hz. Can be changed while running (vibrato, pitch envelopes).
    /// </summary>
    public double Frequency
    {
        get => Volatile.Read(ref _frequency);
        set => Volatile.Write(ref _frequency, value);
    }

    /// <summary>
    ///  The wave shape.
    /// </summary>
    public Waveform Waveform { get; set; } = Waveform.Sine;

    /// <summary>
    ///  Duty cycle for <see cref="Waveform.Pulse"/>, 0..1. 0.5 is a square.
    /// </summary>
    public float PulseWidth { get; set; } = 0.25f;

    /// <summary>
    ///  Produces the next sample in -1..1.
    /// </summary>
    public float Next()
    {
        // Pitch can change on a sequencing thread. Read it once, then update the cached
        // increment on the rendering thread only; an unchanged pitch needs no division.
        double frequency = Frequency;
        if (frequency != _renderedFrequency)
        {
            _phaseIncrement = frequency / sampleRate;
            _renderedFrequency = frequency;
        }

        double phase = _phase;
        _phase += _phaseIncrement;

        if (_phase >= 1)
        {
            _phase -= Math.Floor(_phase);
        }

        return Waveform switch
        {
            Waveform.Sine => (float)Math.Sin(phase * Math.Tau),
            Waveform.Square => phase < 0.5 ? 1f : -1f,
            Waveform.Triangle => (float)(4 * Math.Abs(phase - 0.5) - 1),
            Waveform.Sawtooth => (float)(2 * phase - 1),
            Waveform.Pulse => phase < PulseWidth ? 1f : -1f,
            _ => 0f
        };
    }

    /// <summary>
    ///  Resets the phase to zero.
    /// </summary>
    public void Reset()
        => _phase = 0;
}

/// <summary>
///  White noise. Half of every drum sound.
/// </summary>
public sealed class NoiseSource
{
    private uint _state;

    /// <summary>
    ///  Starts an independent noise stream with a random seed.
    /// </summary>
    public NoiseSource()
        : this((uint)Random.Shared.Next(1, int.MaxValue))
    {
    }

    /// <summary>
    ///  Starts a repeatable noise stream for sample comparisons.
    /// </summary>
    internal NoiseSource(uint seed)
    {
        ArgumentOutOfRangeException.ThrowIfZero(seed);
        _state = seed;
    }

    /// <summary>
    ///  Produces the next sample in -1..1 (xorshift; good enough and allocation-free).
    /// </summary>
    public float Next()
    {
        _state ^= _state << 13;
        _state ^= _state >> 17;
        _state ^= _state << 5;

        return (_state / (float)uint.MaxValue) * 2f - 1f;
    }
}

/// <summary>
///  A one-pole filter. Not a real synth filter, but it tames noise into something drum-shaped.
/// </summary>
/// <remarks>
///  Configure cutoffs on the thread that owns the filter, before processing samples.
/// </remarks>
public sealed class OnePoleFilter
{
    private readonly int _sampleRate;
    private readonly float _samplePeriod;
    private float _lowPassHz;
    private float _highPassHz;
    private float _lowPassCoefficient;
    private float _highPassCoefficient;
    private float _low;
    private float _high;
    private float _lastInput;

    /// <summary>
    ///  Prepares the filter and its default cutoff coefficients for a sample rate.
    /// </summary>
    public OnePoleFilter(int sampleRate)
    {
        _sampleRate = sampleRate;
        _samplePeriod = 1f / sampleRate;
        LowPassHz = 20_000;
    }

    /// <summary>
    ///  Low-pass cutoff in Hz.
    /// </summary>
    public float LowPassHz
    {
        get => _lowPassHz;
        set
        {
            _lowPassHz = value;
            _lowPassCoefficient = 1f - Coefficient(value);
        }
    }

    /// <summary>
    ///  High-pass cutoff in Hz.
    /// </summary>
    public float HighPassHz
    {
        get => _highPassHz;
        set
        {
            _highPassHz = value;
            _highPassCoefficient = value > 0 ? Coefficient(value) : 0;
        }
    }

    /// <summary>
    ///  Runs one sample through high-pass, then low-pass.
    /// </summary>
    public float Next(float input)
    {
        float hp = input;

        if (HighPassHz > 0)
        {
            _high = _highPassCoefficient * (_high + input - _lastInput);
            _lastInput = input;
            hp = _high;
        }

        if (LowPassHz < _sampleRate / 2f)
        {
            _low += _lowPassCoefficient * (hp - _low);

            return _low;
        }

        return hp;
    }

    private float Coefficient(float hz)
    {
        float rc = 1f / (MathF.Tau * hz);

        // A cutoff describes the same filter for thousands of consecutive samples. Compute
        // its RC coefficient when the parameter changes, not inside that sample loop.
        return rc / (rc + _samplePeriod);
    }
}
