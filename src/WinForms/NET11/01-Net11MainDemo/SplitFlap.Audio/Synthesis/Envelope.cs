namespace SplitFlap.Audio.Synthesis;

/// <summary>
///  Attack / Decay / Sustain / Release, in milliseconds and a level. Exists mostly because starting
///  or stopping a waveform mid-cycle clicks; even 2 ms on each end fixes that.
/// </summary>
/// <param name="AttackMs">Time from silence to full level.</param>
/// <param name="DecayMs">Time from full level down to <paramref name="SustainLevel"/>.</param>
/// <param name="SustainLevel">Level held while the key is down, 0..1.</param>
/// <param name="ReleaseMs">Time from the current level to silence after note-off.</param>
public readonly record struct EnvelopeSettings(float AttackMs = 5, float DecayMs = 40, float SustainLevel = 0.7f, float ReleaseMs = 120)
{
    /// <summary>
    ///  Snappy, click-free. The fallback for everything.
    /// </summary>
    public static EnvelopeSettings Default
        // A record struct's parameterless construction is all-zero initialization; spell out
        // these values so "Default" really means the documented musical envelope.
        => new(5, 40, 0.7f, 120);

    /// <summary>
    ///  Slow swell, long tail. Strings and pads.
    /// </summary>
    public static EnvelopeSettings Pad
        => new(400, 200, 0.8f, 900);

    /// <summary>
    ///  No sustain: the note dies on its own. Plucked and struck things.
    /// </summary>
    public static EnvelopeSettings Pluck
        => new(2, 250, 0f, 80);

    /// <summary>
    ///  Organ: instant on, instant off.
    /// </summary>
    public static EnvelopeSettings Organ
        => new(3, 0, 1f, 15);
}

/// <summary>
///  Runs an <see cref="EnvelopeSettings"/> as a per-sample multiplier.
/// </summary>
public sealed class Envelope
{
    private enum Stage { Attack, Decay, Sustain, Release, Done }

    private readonly float _attackStep;
    private readonly float _decayStep;
    private readonly float _sustain;
    private readonly float _releaseSamples;
    private float _level;
    private float _releaseStep;
    private Stage _stage;
    private volatile bool _releaseRequested;

    /// <summary>
    ///  Prepares the envelope for a sample rate.
    /// </summary>
    public Envelope(EnvelopeSettings settings, int sampleRate)
    {
        _attackStep = 1f / Samples(settings.AttackMs, sampleRate);
        _sustain = Math.Clamp(settings.SustainLevel, 0f, 1f);
        _decayStep = (1f - _sustain) / Samples(settings.DecayMs, sampleRate);
        _releaseSamples = Samples(settings.ReleaseMs, sampleRate);
    }

    /// <summary>
    ///  <see langword="true"/> once the release has reached silence.
    /// </summary>
    public bool IsFinished
        => _stage is Stage.Done;

    /// <summary>
    ///  Current level, 0..1.
    /// </summary>
    public float Level
        => _level;

    /// <summary>
    ///  Note-off. Thread-safe; takes effect on the next sample.
    /// </summary>
    public void Release()
        => _releaseRequested = true;

    /// <summary>
    ///  Returns the multiplier for the next sample and advances the state machine.
    /// </summary>
    public float Next()
    {
        if (_releaseRequested && _stage < Stage.Release)
        {
            _releaseRequested = false;
            _stage = Stage.Release;
            _releaseStep = _level / _releaseSamples;
        }

        switch (_stage)
        {
            case Stage.Attack:
                _level += _attackStep;

                if (_level >= 1f)
                {
                    _level = 1f;
                    _stage = Stage.Decay;
                }

                break;

            case Stage.Decay:
                _level -= _decayStep;

                if (_level <= _sustain)
                {
                    _level = _sustain;
                    _stage = _sustain <= 0f ? Stage.Done : Stage.Sustain;
                }

                break;

            case Stage.Release:
                _level -= _releaseStep;

                if (_level <= 0f)
                {
                    _level = 0f;
                    _stage = Stage.Done;
                }

                break;
        }

        return _level;
    }

    private static float Samples(float milliseconds, int sampleRate)
        => Math.Max(1f, milliseconds * sampleRate / 1000f);
}
