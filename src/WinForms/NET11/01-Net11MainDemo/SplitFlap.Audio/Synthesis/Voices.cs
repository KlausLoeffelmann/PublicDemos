namespace SplitFlap.Audio.Synthesis;

/// <summary>
///  An oscillator through an envelope. With a gate length it releases itself; without, it sustains
///  until <see cref="Release"/>.
/// </summary>
public sealed class ToneVoice : IVoice
{
    private readonly Oscillator _oscillator;
    private readonly Envelope _envelope;
    private readonly float _volume;
    private long _gateSamplesLeft;

    /// <summary>
    ///  Creates a tone.
    /// </summary>
    /// <param name="sampleRate">Engine sample rate.</param>
    /// <param name="patch">Waveform, envelope, pulse width, volume.</param>
    /// <param name="frequency">Pitch in Hz.</param>
    /// <param name="gate">How long the key is held; <see langword="null"/> means until <see cref="Release"/>.</param>
    /// <param name="velocity">Loudness multiplier, 0..1.</param>
    public ToneVoice(int sampleRate, VoicePatch patch, double frequency, TimeSpan? gate = null, float velocity = 1f)
    {
        _oscillator = new Oscillator(sampleRate)
        {
            Frequency = frequency,
            Waveform = patch.Waveform,
            PulseWidth = patch.PulseWidth
        };

        _envelope = new Envelope(patch.Envelope, sampleRate);
        _volume = patch.Volume * Math.Clamp(velocity, 0f, 1.5f);
        _gateSamplesLeft = gate is { } g ? (long)(g.TotalSeconds * sampleRate) : long.MaxValue;
    }

    /// <summary>
    ///  The oscillator, for pitch changes (vibrato, trills) while sounding.
    /// </summary>
    public Oscillator Oscillator
        => _oscillator;

    /// <inheritdoc/>
    public bool IsFinished
        => _envelope.IsFinished;

    /// <inheritdoc/>
    public float Next()
    {
        if (_gateSamplesLeft > 0 && --_gateSamplesLeft == 0)
        {
            _envelope.Release();
        }

        return _oscillator.Next() * _envelope.Next() * _volume;
    }

    /// <inheritdoc/>
    public void Release()
        => _envelope.Release();
}

/// <summary>
///  A recorded sound played once at engine rate. Release fades out over 5 ms instead of cutting.
/// </summary>
public sealed class SampleVoice(Sample sample, float volume = 1f) : IVoice
{
    private const int FadeSamples = 240;
    private int _position;
    private int _fadeLeft = -1;
    private volatile bool _releaseRequested;

    /// <inheritdoc/>
    public bool IsFinished
        => _position >= sample.Data.Length || _fadeLeft == 0;

    /// <inheritdoc/>
    public float Next()
    {
        if (IsFinished)
        {
            return 0f;
        }

        float value = sample.Data[_position++] * volume;

        if (_releaseRequested && _fadeLeft < 0)
        {
            _fadeLeft = FadeSamples;
        }

        if (_fadeLeft > 0)
        {
            value *= _fadeLeft / (float)FadeSamples;
            _fadeLeft--;
        }

        return value;
    }

    /// <inheritdoc/>
    public void Release()
        => _releaseRequested = true;
}

/// <summary>
///  The sound of one flap hitting the stop: a filtered noise burst with a fast exponential decay,
///  plus a tiny pitched tick so it has a "body". Every clack is slightly different, because the
///  real ones were.
/// </summary>
public sealed class ClackVoice : IVoice
{
    private readonly NoiseSource _noise = new();
    private readonly OnePoleFilter _filter;
    private readonly Oscillator _tick;
    private readonly float _noiseDecay;
    private readonly float _tickDecay;
    private readonly float _volume;
    private readonly int _attackSamples;
    private int _startDelaySamples;
    private int _ageSamples;
    private float _noiseLevel = 1f;
    private float _tickLevel = 0.6f;

    /// <summary>
    ///  Creates one clack.
    /// </summary>
    /// <param name="sampleRate">Engine sample rate.</param>
    /// <param name="volume">Loudness, 0..1. Forty of these at once add up; 0.3 is plenty.</param>
    /// <param name="startDelay">
    ///  Silence before the strike. This permits sample-accurate staggering even when several
    ///  voices enter the same engine buffer.
    /// </param>
    /// <param name="attackMilliseconds">
    ///  Time used to ramp into the strike. A very short attack keeps the mechanical transient
    ///  while avoiding the unnatural full-scale first noise sample.
    /// </param>
    public ClackVoice(
        int sampleRate,
        float volume = 0.3f,
        TimeSpan startDelay = default,
        float attackMilliseconds = 1.5f)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);

        float variance = 0.85f + (float)Random.Shared.NextDouble() * 0.3f;

        _filter = new OnePoleFilter(sampleRate)
        {
            HighPassHz = 1_400f * variance,
            LowPassHz = 7_000f
        };

        _tick = new Oscillator(sampleRate)
        {
            Waveform = Waveform.Sine,
            Frequency = 2_300 * variance
        };

        _noiseDecay = DecayPerSample(6f * variance, sampleRate);
        _tickDecay = DecayPerSample(2.5f, sampleRate);
        _volume = volume * variance;
        _startDelaySamples = Math.Max(0, (int)Math.Round(startDelay.TotalSeconds * sampleRate));
        _attackSamples = Math.Max(
            1,
            (int)Math.Round(Math.Max(0, attackMilliseconds) * sampleRate / 1000f));
    }

    /// <inheritdoc/>
    public bool IsFinished
        => _startDelaySamples <= 0 && _noiseLevel < 0.001f;

    /// <inheritdoc/>
    public float Next()
    {
        if (_startDelaySamples > 0)
        {
            _startDelaySamples--;
            return 0f;
        }

        // A half-sine eases in more naturally than a linear ramp. At 48 kHz, the default
        // 1.5 ms attack spans 72 samples: fast enough to remain a clack, but not a hard edge.
        float attackProgress = Math.Min(1f, ++_ageSamples / (float)_attackSamples);
        float attack = MathF.Sin(attackProgress * MathF.PI / 2f);
        float noise = _filter.Next(_noise.Next()) * _noiseLevel;
        float tick = _tick.Next() * _tickLevel;

        _noiseLevel *= _noiseDecay;
        _tickLevel *= _tickDecay;

        return (noise + tick) * _volume * attack;
    }

    /// <inheritdoc/>
    public void Release()
    {
        // A clack can't be interrupted. It's 30 ms long; it will be gone before you notice.
    }

    private static float DecayPerSample(float tauMilliseconds, int sampleRate)
        => MathF.Exp(-1f / (tauMilliseconds * sampleRate / 1000f));
}
