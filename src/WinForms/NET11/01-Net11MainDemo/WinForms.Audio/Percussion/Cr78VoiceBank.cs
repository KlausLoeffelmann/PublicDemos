namespace WinForms.Audio.Percussion;

/// <summary>
///  Owns one preallocated retriggerable channel per percussion sound, plus the metallic layer.
/// </summary>
internal sealed class Cr78VoiceBank
{
    private readonly Cr78Generator[] _generators = new Cr78Generator[14];
    private readonly GainRamp[] _gains = new GainRamp[14];
    private readonly int _rampSamples;
    private GainRamp _master = new(1f);
    private float _metallicLevel = 1f;
    private bool _metallicEnabled = true;
    private bool _metallicAudition;

    /// <summary>
    ///  Prepares the whole palette outside the audio thread with independently continuing noise streams.
    /// </summary>
    internal Cr78VoiceBank(int sampleRate, uint noiseSeed)
    {
        Cr78Kit.ValidateSampleRate(sampleRate);
        ArgumentOutOfRangeException.ThrowIfZero(noiseSeed);
        _rampSamples = Math.Max(1, sampleRate / 200);
        for (int i = 0; i < _generators.Length; i++)
        {
            uint seed = (noiseSeed ^ (0x9E3779B9u * (uint)(i + 1))) | 1u;
            _generators[i] = new Cr78Generator(sampleRate, (Cr78Instrument)i, seed);
            _gains[i] = new GainRamp(1f);
        }
    }

    /// <summary>
    ///  Gets whether any strike or release needs advancing, including a currently muted channel.
    /// </summary>
    internal bool IsActive
    {
        get
        {
            foreach (Cr78Generator generator in _generators)
            {
                if (generator.IsActive)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    ///  Excites a single prepared sound; the caller decides when to trigger the shared metallic layer.
    /// </summary>
    internal void Trigger(Cr78Instrument instrument, float velocity, long gateSamples)
    {
        if (instrument == Cr78Instrument.MetallicBeat && velocity > 0)
        {
            // An explicit metallic-only audition bypasses enable/amount, but not the master.
            // It still occupies the same prepared channel as the automatic HH/CY layer.
            _metallicAudition = true;
            _gains[(int)Cr78Instrument.MetallicBeat].SetTarget(1f,
                _generators[(int)Cr78Instrument.MetallicBeat].IsActive ? _rampSamples : 0);
        }

        _generators[(int)instrument].Trigger(velocity, gateSamples);
    }

    /// <summary>
    ///  Excites the shared HH/CY layer once, with strike velocity independent of its mix amount.
    /// </summary>
    internal void TriggerMetallicLayer(float velocity, long gateSamples)
    {
        if (velocity <= 0 || !_metallicEnabled || _metallicLevel <= 0)
        {
            return;
        }

        _metallicAudition = false;
        _gains[(int)Cr78Instrument.MetallicBeat].SetTarget(_metallicLevel,
            _generators[(int)Cr78Instrument.MetallicBeat].IsActive ? _rampSamples : 0);
        _generators[(int)Cr78Instrument.MetallicBeat].Trigger(velocity, gateSamples);
    }

    /// <summary>
    ///  Applies a coherent mix to prepared ramps; inactive channels can change gain immediately.
    /// </summary>
    internal void SetMix(
        float masterVolume, ReadOnlySpan<float> instrumentVolumes,
        bool metallicEnabled, float metallicLevel, bool initialize = false)
    {
        for (int i = 0; i < instrumentVolumes.Length; i++)
        {
            int channel = (int)Cr78Kit.Instruments[i];
            _gains[channel].SetTarget(instrumentVolumes[i],
                initialize || !_generators[channel].IsActive ? 0 : _rampSamples);
        }

        // There is no waveform to de-click before the first strike. In particular a mute
        // set while stopped must not leak the first 5 ms of a subsequently queued audition.
        _master.SetTarget(masterVolume, initialize || !IsActive ? 0 : _rampSamples);
        _metallicEnabled = metallicEnabled;
        _metallicLevel = metallicLevel;
        _gains[(int)Cr78Instrument.MetallicBeat].SetTarget(
            _metallicAudition ? 1f : metallicEnabled ? metallicLevel : 0f,
            initialize || !_generators[(int)Cr78Instrument.MetallicBeat].IsActive ? 0 : _rampSamples);
    }

    /// <summary>
    ///  Gracefully cancels every current strike without making future auditions impossible.
    /// </summary>
    internal void ReleaseAll()
    {
        foreach (Cr78Generator generator in _generators)
        {
            generator.Release();
        }
    }

    /// <summary>
    ///  Advances even muted channels, then sums their ramped outputs through the player-local master.
    /// </summary>
    internal float Next()
    {
        float sample = 0;
        for (int i = 0; i < _generators.Length; i++)
        {
            sample += _generators[i].Next() * _gains[i].Next();
        }

        if (_metallicAudition && !_generators[(int)Cr78Instrument.MetallicBeat].IsActive)
        {
            _metallicAudition = false;
            _gains[(int)Cr78Instrument.MetallicBeat].SetTarget(
                _metallicEnabled ? _metallicLevel : 0f, _rampSamples);
        }

        return sample * _master.Next();
    }

    private struct GainRamp(float value)
    {
        private float _current = value;
        private float _target = value;
        private float _increment;
        private int _remaining;

        internal void SetTarget(float target, int samples)
        {
            if (samples == 0)
            {
                _current = _target = target;
                _remaining = 0;
            }
            else if (_target != target)
            {
                _target = target;
                _remaining = samples;
                _increment = (target - _current) / samples;
            }
        }

        internal float Next()
        {
            if (_remaining > 0)
            {
                _current = --_remaining == 0
                    ? _target
                    : Math.Clamp(_current + _increment, 0f, 1f);
            }

            return _current;
        }
    }
}
