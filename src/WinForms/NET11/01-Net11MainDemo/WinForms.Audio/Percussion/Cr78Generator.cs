namespace WinForms.Audio.Percussion;

/// <summary>
///  A reusable percussion channel with two prepared strikes and a five-millisecond retrigger fade.
/// </summary>
/// <remarks>
///  Only the rendering thread calls this type. The public one-shot and player marshal release
///  requests to that thread instead of racing changes to filters, envelopes, or oscillator state.
/// </remarks>
internal sealed class Cr78Generator
{
    private readonly float _level;
    private readonly int _fadeSamples;
    private SoundState _current;
    private SoundState _previous;
    private int _crossfadeLeft;
    private int _correctionLeft;
    private int _releaseLeft = -1;
    private float _correction;
    private float _lastSample;
    private float[]? _sampleOverride;

    /// <summary>
    ///  Prepares both strikes and shared procedural waveforms without allocating on later hits.
    /// </summary>
    internal Cr78Generator(int sampleRate, Cr78Instrument instrument, uint noiseSeed)
    {
        Cr78Kit.ValidateSampleRate(sampleRate);
        Cr78Kit.ValidateInstrument(instrument);
        ArgumentOutOfRangeException.ThrowIfZero(noiseSeed);
        Cr78Preset preset = Cr78Preset.For(instrument);
        _level = preset.Level * (Cr78Kit.Instruments.Contains(instrument) ? 0.63f : 1f);
        _fadeSamples = Math.Max(1, sampleRate / 200);
        bool pulse = instrument is Cr78Instrument.Cowbell or Cr78Instrument.Tambourine or Cr78Instrument.MetallicBeat;
        Cr78WaveTable? first = CreateTable(preset.Frequency);
        Cr78WaveTable? second = CreateTable(preset.SecondFrequency);
        Cr78WaveTable? third = CreateTable(preset.ThirdFrequency);
        Cr78WaveTable? scrape = instrument == Cr78Instrument.Guiro
            ? new Cr78WaveTable(sampleRate, 125, pulse: false)
            : null;
        _current = new SoundState(sampleRate, instrument, preset, first, second, third, scrape, noiseSeed);
        _previous = new SoundState(sampleRate, instrument, preset, first, second, third, scrape,
            (noiseSeed ^ 0x9E3779B9u) | 1u);

        Cr78WaveTable? CreateTable(double frequency)
            => frequency > 0 ? new Cr78WaveTable(sampleRate, frequency, pulse) : null;
    }

    /// <summary>
    ///  Gets whether any strike, retrigger correction, or explicit release still needs rendering.
    /// </summary>
    internal bool IsActive
        => _current.IsActive || _previous.IsActive || _correctionLeft > 0;

    /// <summary>
    ///  Retriggers a prepared channel; zero velocity is a silent event, not a tail cancellation.
    /// </summary>
    internal void Trigger(float velocity, long gateSamples)
    {
        if (velocity <= 0)
        {
            return;
        }

        bool wasActive = IsActive;
        (_current, _previous) = (_previous, _current);
        _current.Start(velocity, gateSamples, _sampleOverride);
        _crossfadeLeft = wasActive ? _fadeSamples : 0;

        // Usually the old strike simply becomes the fading lane. If another hit arrives
        // inside those 5 ms, preserve the discarded lane's last contribution as a short
        // de-click correction rather than either allocating a third voice or making an edge.
        _correction = wasActive ? _lastSample - _previous.LastSample : 0;
        _correctionLeft = _correction == 0 ? 0 : _fadeSamples;
        _releaseLeft = -1;
    }

    /// <summary>
    ///  Selects owned, rate-matched PCM for subsequent hits without mutating an existing tail.
    /// </summary>
    internal void SetSample(float[]? sample)
        => _sampleOverride = sample;

    /// <summary>
    ///  Starts a short release of all overlapping strikes without abruptly clearing their output.
    /// </summary>
    internal void Release()
    {
        if (IsActive && _releaseLeft < 0)
        {
            _releaseLeft = _fadeSamples;
        }
    }

    /// <summary>
    ///  Renders a bounded sample and completely clears inaudible state when the sound ends.
    /// </summary>
    internal float Next()
    {
        if (!IsActive)
        {
            _lastSample = 0;
            return 0;
        }

        float sample = _current.Next();
        if (_crossfadeLeft > 0)
        {
            float oldWeight = _crossfadeLeft / (float)_fadeSamples;
            sample = sample * (1 - oldWeight) + _previous.Next() * oldWeight;
            if (--_crossfadeLeft == 0)
            {
                _previous.Clear();
            }
        }

        if (_correctionLeft > 0)
        {
            sample += _correction * (_correctionLeft-- / (float)_fadeSamples);
        }

        if (_releaseLeft > 0)
        {
            sample *= _releaseLeft-- / (float)_fadeSamples;
            if (_releaseLeft == 0)
            {
                _current.Clear();
                _previous.Clear();
                _crossfadeLeft = _correctionLeft = 0;
            }
        }

        // Normal complementary crossfades stay inside the bound. The guard also bounds a
        // deliberately abusive stream of faster-than-fade retriggers and its correction.
        _lastSample = Math.Clamp(sample, -1f, 1f);
        return _lastSample * _level;
    }

    private sealed class SoundState
    {
        private readonly int _sampleRate;
        private readonly Cr78Instrument _instrument;
        private readonly NoiseSource _noise;
        private readonly float[]? _modeledStrike;
        private float[]? _activeData;
        private bool _imported;
        private bool _extendedScrape;
        private readonly double _decay;
        private readonly double _fastDecay;
        private readonly int _attackSamples;
        private readonly long _maximumDecaySamples;
        private readonly Cr78WaveTable? _scrapeWave;
        private Cr78Oscillator _first;
        private Cr78Oscillator _second;
        private Cr78Oscillator _third;
        private Cr78NoiseFilter _filter;
        private double _envelope;
        private double _fastEnvelope;
        private double _scrapePhase;
        private double _scrapeIncrement;
        private double _scrapeIncrementChange;
        private long _gateLeft;
        private long _age;
        private long _decayAge;
        private float _velocity;

        /// <summary>
        ///  Prepares modal carriers, a noise stream, and all fixed envelope/filter coefficients.
        /// </summary>
        internal SoundState(
            int sampleRate,
            Cr78Instrument instrument,
            Cr78Preset preset,
            Cr78WaveTable? first,
            Cr78WaveTable? second,
            Cr78WaveTable? third,
            Cr78WaveTable? scrape,
            uint seed)
        {
            _sampleRate = sampleRate;
            _instrument = instrument;
            _noise = new NoiseSource(seed);
            _modeledStrike = MeasuredPercussionModel.Prepare(instrument, sampleRate, seed);
            _first = new Cr78Oscillator(first, preset.Frequency, sampleRate);
            _second = new Cr78Oscillator(second, preset.SecondFrequency, sampleRate);
            _third = new Cr78Oscillator(third, preset.ThirdFrequency, sampleRate);
            _filter = new Cr78NoiseFilter(sampleRate, preset.HighPassHz, preset.LowPassHz);
            _scrapeWave = scrape;

            // The service diagram measures V -> V/10, not V/e. Therefore after D samples
            // the envelope must satisfy r^D = 0.1: r = exp(log(0.1) / D).
            _decay = Math.Exp(Math.Log(0.1) / (preset.DecayMilliseconds * sampleRate / 1_000));
            double fastMilliseconds = Math.Min(12, preset.DecayMilliseconds * 0.28);
            _fastDecay = Math.Exp(Math.Log(0.1) / (fastMilliseconds * sampleRate / 1_000));
            _attackSamples = Math.Max(1, (int)Math.Round(preset.AttackMilliseconds * sampleRate / 1_000));
            _maximumDecaySamples = (long)Math.Ceiling(preset.DecayMilliseconds * sampleRate / 1_000 * 5);
        }

        /// <summary>
        ///  Gets whether the strike is above its final, minus-100-dB envelope cutoff.
        /// </summary>
        internal bool IsActive { get; private set; }

        /// <summary>
        ///  Gets the last normalized output for continuous rapid-retrigger handoff.
        /// </summary>
        internal float LastSample { get; private set; }

        /// <summary>
        ///  Excites fresh modal/envelope state without resetting the continuing random stream.
        /// </summary>
        internal void Start(float velocity, long gateSamples, float[]? sampleOverride)
        {
            _activeData = sampleOverride ?? _modeledStrike;
            _imported = sampleOverride is not null;
            _extendedScrape = false;
            _first.Reset();
            _second.Reset();
            _third.Reset();
            _filter.Clear();
            _envelope = _fastEnvelope = 1;
            _age = _decayAge = 0;
            _gateLeft = Math.Max(1, gateSamples);
            _scrapePhase = 0;
            _scrapeIncrement = 125d / _sampleRate;
            _scrapeIncrementChange = -48d / (_sampleRate * (double)_gateLeft);
            _velocity = velocity * (0.985f + _noise.Next() * 0.015f);
            LastSample = 0;
            IsActive = true;
        }

        /// <summary>
        ///  Renders the instrument family, then its damped amplitude and short excitation ramp.
        /// </summary>
        internal float Next()
        {
            if (!IsActive)
            {
                return 0;
            }

            if (_activeData is not null)
            {
                return NextModeled();
            }

            float first = _first.Next();
            float second = _second.Next();
            float third = _third.Next();
            float fast = (float)_fastEnvelope;
            float raw;

            if (_instrument == Cr78Instrument.Cowbell)
            {
                raw = 2 * _filter.Next(first * 0.55f + second * 0.45f);
            }
            else if (_instrument == Cr78Instrument.MetallicBeat)
            {
                raw = 2 * _filter.Next(first * 0.35f + second * 0.35f + third * 0.30f);
            }
            else
            {
                float noise = _filter.Next(_noise.Next());
                raw = _instrument switch
                {
                    Cr78Instrument.BassDrum =>
                        first * 0.90f + second * fast * 0.08f + noise * fast * 0.02f,
                    Cr78Instrument.SnareDrum =>
                        first * 0.32f + second * fast * 0.08f + noise * 2.4f,
                    Cr78Instrument.RimShot =>
                        first * 0.72f + second * fast * 0.24f + noise * fast * 0.04f,
                    Cr78Instrument.Claves =>
                        first * 0.86f + second * fast * 0.14f,
                    Cr78Instrument.HighBongo or Cr78Instrument.LowBongo or Cr78Instrument.LowConga =>
                        first * 0.86f + second * fast * 0.11f + noise * fast * 0.03f,
                    Cr78Instrument.HiHat or Cr78Instrument.Cymbal or Cr78Instrument.Maracas =>
                        noise * 4,
                    Cr78Instrument.Tambourine =>
                        (noise * 2.4f + (first + second * 0.8f + third * 0.7f) * 0.16f) * (0.75f + fast * 0.25f),
                    Cr78Instrument.Guiro =>
                        Scrape() * (noise * 2.6f + first * 0.23f + second * 0.12f),
                    _ => 0
                };
            }

            float attack = _age < _attackSamples ? (_age + 1f) / _attackSamples : 1f;
            LastSample = Math.Clamp(raw, -1f, 1f) * (float)_envelope * attack * _velocity;
            _age++;
            _fastEnvelope *= _fastDecay;
            if (_fastEnvelope < 1e-12)
            {
                _fastEnvelope = 0;
            }

            if (_instrument != Cr78Instrument.Guiro || --_gateLeft <= 0)
            {
                _envelope *= _decay;
                if (++_decayAge >= _maximumDecaySamples)
                {
                    Clear();
                }
            }

            return LastSample;
        }

        private float NextModeled()
        {
            bool scrape = !_imported && _instrument is (Cr78Instrument.Guiro or Cr78Instrument.Guiro2);
            long age = _age;
            int position;
            if (scrape)
            {
                long sustainEnd = (long)(_sampleRate * (_instrument == Cr78Instrument.Guiro ? 0.25 : 0.8));
                _extendedScrape |= age >= sustainEnd && _gateLeft > 0;
                if (_extendedScrape)
                {
                    int loop = Math.Max(1, _sampleRate / 10);
                    int fade = Math.Min(_sampleRate / 200, loop / 2);
                    position = (int)(sustainEnd - loop + fade + (age - sustainEnd) % (loop - fade));
                    int remaining = (int)sustainEnd - position;
                    if (remaining <= fade)
                    {
                        float weight = remaining / (float)fade;
                        LastSample = _activeData![position] * weight
                            + _activeData[(int)sustainEnd - loop + fade - remaining] * (1 - weight);
                    }
                    else
                    {
                        LastSample = _activeData![position];
                    }
                }
                else
                {
                    LastSample = age < _activeData!.Length ? _activeData[(int)age] : 0;
                    int fade = _sampleRate / 200;
                    long remaining = sustainEnd - age;
                    if (remaining > 0 && remaining <= fade && _gateLeft > fade)
                    {
                        float weight = remaining / (float)fade;
                        int loopStart = (int)sustainEnd - _sampleRate / 10;
                        LastSample = LastSample * weight
                            + _activeData[loopStart + fade - (int)remaining] * (1 - weight);
                    }
                }

                if (--_gateLeft <= 0)
                {
                    _envelope *= _decay;
                    if (++_decayAge >= _maximumDecaySamples)
                    {
                        Clear();
                        return 0;
                    }
                }
            }
            else
            {
                if (age >= _activeData!.Length)
                {
                    Clear();
                    return 0;
                }

                LastSample = _activeData[(int)age];
            }

            LastSample *= _velocity * (float)_envelope;
            _age++;
            return LastSample;
        }

        /// <summary>
        ///  Removes all feedback and envelope residue while preserving the next hit's noise variation.
        /// </summary>
        internal void Clear()
        {
            IsActive = false;
            _activeData = null;
            _envelope = _fastEnvelope = 0;
            _filter.Clear();
            LastSample = 0;
        }

        private float Scrape()
        {
            // A half-wave sine raised to the fourth power is a smooth train of scrapes:
            // unlike a hard pulse train, its edges have no abrupt step. The 125 -> 77 Hz
            // speed sweep is our playable interpretation of the table's two guiro settings.
            float positive = Math.Max(0, _scrapeWave!.At(_scrapePhase));
            float squared = positive * positive;
            _scrapePhase += _scrapeIncrement;
            if (_scrapePhase >= 1)
            {
                _scrapePhase -= 1;
            }

            if (_gateLeft > 0)
            {
                _scrapeIncrement = Math.Max(77d / _sampleRate, _scrapeIncrement + _scrapeIncrementChange);
            }

            return squared * squared;
        }
    }
}
