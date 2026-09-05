using SplitFlap.Audio.Percussion;

namespace SplitFlap.Audio.Sequencing;

/// <summary>
///  An immutable settings handoff; score indexing and object allocation occur only on the caller.
/// </summary>
internal sealed class PercussionSettings
{
    private readonly float[] _instrumentVolumes;

    /// <summary>
    ///  Starts with unity percussion/master gains, looping, and a zero-level enabled metallic layer.
    /// </summary>
    internal PercussionSettings(PercussionScore score, Tempo tempo)
        : this(score, tempo, true, 1f, CreateDefaultVolumes(), true, 0f, 1)
    {
    }

    private PercussionSettings(
        PercussionScore score, Tempo tempo, bool loop, float masterVolume, float[] instrumentVolumes,
        bool metallicEnabled, float metallicLevel, long revision)
    {
        Score = score;
        Tempo = tempo;
        Loop = loop;
        MasterVolume = masterVolume;
        _instrumentVolumes = instrumentVolumes;
        MetallicEnabled = metallicEnabled;
        MetallicLevel = metallicLevel;
        Revision = revision;
    }

    /// <summary>
    ///  Gets the already compiled immutable score.
    /// </summary>
    internal PercussionScore Score { get; }

    /// <summary>
    ///  Gets the requested tempo.
    /// </summary>
    internal Tempo Tempo { get; }

    /// <summary>
    ///  Gets whether the end of the score should wrap.
    /// </summary>
    internal bool Loop { get; }

    /// <summary>
    ///  Gets the final player-local gain, independent of the engine's master.
    /// </summary>
    internal float MasterVolume { get; }

    /// <summary>
    ///  Gets the thirteen immutable gains in Cr78Kit.Instruments order.
    /// </summary>
    internal ReadOnlySpan<float> InstrumentVolumes => _instrumentVolumes;

    /// <summary>
    ///  Gets whether automatic hi-hat/cymbal metallic layering is enabled.
    /// </summary>
    internal bool MetallicEnabled { get; }

    /// <summary>
    ///  Gets the hi-hat/cymbal metallic-layer level.
    /// </summary>
    internal float MetallicLevel { get; }

    /// <summary>
    ///  Gets the score/tempo revision used to report pending audible edits.
    /// </summary>
    internal long Revision { get; }

    /// <summary>
    ///  Copies the private gains for a caller-side single-fader edit.
    /// </summary>
    internal float[] CopyInstrumentVolumes() => (float[])_instrumentVolumes.Clone();

    /// <summary>
    ///  Creates a caller-side snapshot, sharing only immutable owned arrays and compiled scores.
    /// </summary>
    internal PercussionSettings With(
        PercussionScore? score = null, Tempo? tempo = null, bool? loop = null, float? masterVolume = null,
        float[]? instrumentVolumes = null, bool? metallicEnabled = null, float? metallicLevel = null)
    {
        score ??= Score;
        bool scoreChanged = !Score.ContentEquals(score);
        Tempo nextTempo = tempo ?? Tempo;
        bool nextLoop = loop ?? Loop;
        float nextMaster = masterVolume ?? MasterVolume;
        bool nextMetallicEnabled = metallicEnabled ?? MetallicEnabled;
        float nextMetallicLevel = metallicLevel ?? MetallicLevel;
        bool gainsChanged = instrumentVolumes is not null && !_instrumentVolumes.AsSpan().SequenceEqual(instrumentVolumes);
        if (!scoreChanged && nextTempo == Tempo && nextLoop == Loop && nextMaster == MasterVolume &&
            !gainsChanged && nextMetallicEnabled == MetallicEnabled && nextMetallicLevel == MetallicLevel)
        {
            return this;
        }

        return new PercussionSettings(scoreChanged ? score : Score, nextTempo, nextLoop, nextMaster,
            gainsChanged ? instrumentVolumes! : _instrumentVolumes, nextMetallicEnabled, nextMetallicLevel,
            Revision + (scoreChanged || nextTempo != Tempo ? 1 : 0));
    }

    private static float[] CreateDefaultVolumes()
    {
        float[] volumes = new float[Cr78Kit.Instruments.Count];
        Array.Fill(volumes, 1f);
        return volumes;
    }
}

/// <summary>
///  The bounded transport actions whose relative order must not be lost.
/// </summary>
internal enum PercussionCommandKind
{
    /// <summary>
    ///  Starts a stopped score or resumes a paused score.
    /// </summary>
    Start,

    /// <summary>
    ///  Holds the musical position and releases active sounds.
    /// </summary>
    Pause,

    /// <summary>
    ///  Stops future events and releases current sounds.
    /// </summary>
    Stop,

    /// <summary>
    ///  Applies the captured configuration and stops/resets as one ordered transaction.
    /// </summary>
    ApplyConfiguration,

    /// <summary>
    ///  Auditions an instrument independently of the score transport.
    /// </summary>
    Audition
}

/// <summary>
///  A command copied into prepared audio-thread storage with its immutable caller-side settings.
/// </summary>
internal readonly struct PercussionCommand
{
    /// <summary>
    ///  Captures one ordered transport or audition request.
    /// </summary>
    internal PercussionCommand(
        long sequence, PercussionCommandKind kind, PercussionSettings settings,
        Cr78Instrument instrument = default, float velocity = 1)
    {
        Sequence = sequence;
        Kind = kind;
        Settings = settings;
        Instrument = instrument;
        Velocity = velocity;
    }

    /// <summary>
    ///  Gets the monotonically increasing caller-side ordering number.
    /// </summary>
    internal long Sequence { get; }

    /// <summary>
    ///  Gets the action to perform.
    /// </summary>
    internal PercussionCommandKind Kind { get; }

    /// <summary>
    ///  Gets the settings at this command's admission, not later coalesced edits.
    /// </summary>
    internal PercussionSettings Settings { get; }

    /// <summary>
    ///  Gets the audition instrument when this is an audition request.
    /// </summary>
    internal Cr78Instrument Instrument { get; }

    /// <summary>
    ///  Gets the audition velocity.
    /// </summary>
    internal float Velocity { get; }
}

/// <summary>
///  Coalesces edits and auditions while preserving ordered transport/configuration requests.
/// </summary>
internal sealed class PercussionMailbox
{
    private readonly object _sync = new();
    private readonly List<PercussionCommand> _commands = new(MaximumCommands);
    private PercussionSettings _settings;
    private long _sequence;
    private long _transportRequest;
    private int _releaseRequested;

    /// <summary>
    ///  Bounds both caller admission and the amount of command work at any audio-block boundary.
    /// </summary>
    internal const int MaximumCommands = 128;

    /// <summary>
    ///  Starts with a compiled score, a validated tempo, looping enabled, and no metallic layer.
    /// </summary>
    internal PercussionMailbox(PercussionScore score, Tempo tempo)
    {
        ArgumentNullException.ThrowIfNull(score);
        PercussionClock.ValidateTempo(tempo);
        _settings = new PercussionSettings(score, tempo);
    }

    /// <summary>
    ///  Gets a coherent latest request without taking a lock.
    /// </summary>
    internal PercussionSettings Settings
        => Volatile.Read(ref _settings);

    /// <summary>
    ///  Gets the latest accepted transport request, also cleared on natural score completion.
    /// </summary>
    internal bool IsPlaying
        => State == DrumTransportState.Playing;

    /// <summary>
    ///  Gets the latest accepted state, separately from delayed rendered/played history.
    /// </summary>
    internal DrumTransportState State
        => (DrumTransportState)(Volatile.Read(ref _transportRequest) & 3);

    /// <summary>
    ///  Gets whether disposal has closed command admission and requested the voice's release.
    /// </summary>
    internal bool ReleaseRequested
        => Volatile.Read(ref _releaseRequested) != 0;

    /// <summary>
    ///  Publishes a score revision, replacing older unconsumed score edits.
    /// </summary>
    internal void SetScore(PercussionScore score)
    {
        ArgumentNullException.ThrowIfNull(score);
        lock (_sync)
        {
            ThrowIfReleased();
            Volatile.Write(ref _settings, _settings.With(score: score));
        }
    }

    /// <summary>
    ///  Publishes a tempo revision without modifying an in-progress audio bar.
    /// </summary>
    internal void SetTempo(Tempo tempo)
    {
        PercussionClock.ValidateTempo(tempo);
        lock (_sync)
        {
            ThrowIfReleased();
            Volatile.Write(ref _settings, _settings.With(tempo: tempo));
        }
    }

    /// <summary>
    ///  Publishes the loop switch for the next block without inventing a pending score revision.
    /// </summary>
    internal void SetLoop(bool loop)
    {
        lock (_sync)
        {
            ThrowIfReleased();
            Volatile.Write(ref _settings, _settings.With(loop: loop));
        }
    }

    /// <summary>
    ///  Publishes a remembered metallic amount; existing layer tails follow the next-block gain ramp.
    /// </summary>
    internal void SetMetallicLevel(float level)
    {
        Cr78Kit.ValidateLevel(level, nameof(level));
        lock (_sync)
        {
            ThrowIfReleased();
            Volatile.Write(ref _settings, _settings.With(metallicLevel: level));
        }
    }

    /// <summary>
    ///  Toggles automatic metallic layering without changing its remembered amount.
    /// </summary>
    internal void SetMetallicEnabled(bool enabled)
    {
        lock (_sync)
        {
            ThrowIfReleased();
            Volatile.Write(ref _settings, _settings.With(metallicEnabled: enabled));
        }
    }

    /// <summary>
    ///  Publishes a finite player-local master target for the next audio block.
    /// </summary>
    internal void SetMasterVolume(float volume)
    {
        Cr78Kit.ValidateLevel(volume, nameof(volume));
        lock (_sync)
        {
            ThrowIfReleased();
            Volatile.Write(ref _settings, _settings.With(masterVolume: volume));
        }
    }

    /// <summary>
    ///  Reads a primary percussion fader; the metallic layer has separate controls.
    /// </summary>
    internal float GetInstrumentVolume(Cr78Instrument instrument)
    {
        Cr78Kit.ValidateInstrument(instrument, allowMetallic: false);
        return Settings.InstrumentVolumes[(int)instrument];
    }

    /// <summary>
    ///  Publishes an independently copied percussion fader without creating a musical revision.
    /// </summary>
    internal void SetInstrumentVolume(Cr78Instrument instrument, float volume)
    {
        Cr78Kit.ValidateInstrument(instrument, allowMetallic: false);
        Cr78Kit.ValidateLevel(volume, nameof(volume));
        lock (_sync)
        {
            ThrowIfReleased();
            if (_settings.InstrumentVolumes[(int)instrument] == volume)
            {
                return;
            }

            float[] volumes = _settings.CopyInstrumentVolumes();
            volumes[(int)instrument] = volume;
            Volatile.Write(ref _settings, _settings.With(instrumentVolumes: volumes));
        }
    }

    /// <summary>
    ///  Validates/copies a whole document on the caller, optionally reserving an ordered reset.
    /// </summary>
    internal void ApplyConfiguration(
        PercussionScore score, Tempo tempo, float masterVolume, IReadOnlyList<float> instrumentVolumes,
        bool loop, bool metallicEnabled, float metallicLevel, bool resetTransport = false)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentNullException.ThrowIfNull(instrumentVolumes);
        PercussionClock.ValidateTempo(tempo);
        Cr78Kit.ValidateLevel(masterVolume, nameof(masterVolume));
        Cr78Kit.ValidateLevel(metallicLevel, nameof(metallicLevel));
        if (instrumentVolumes.Count != Cr78Kit.Instruments.Count)
        {
            throw new ArgumentException("Supply one gain for each of the thirteen percussion instruments.", nameof(instrumentVolumes));
        }

        float[] volumes = new float[Cr78Kit.Instruments.Count];
        for (int i = 0; i < volumes.Length; i++)
        {
            float volume = instrumentVolumes[i];
            Cr78Kit.ValidateLevel(volume, nameof(instrumentVolumes));
            volumes[i] = volume;
        }

        lock (_sync)
        {
            ThrowIfReleased();
            if (resetTransport)
            {
                EnsureRoom();
            }

            PercussionSettings settings = _settings.With(score, tempo, loop, masterVolume, volumes, metallicEnabled, metallicLevel);
            if (resetTransport)
            {
                long sequence = ++_sequence;
                _commands.Add(new PercussionCommand(sequence, PercussionCommandKind.ApplyConfiguration, settings));
                Volatile.Write(ref _transportRequest, TransportRequest(sequence, DrumTransportState.Stopped));
            }

            Volatile.Write(ref _settings, settings);
        }
    }

    /// <summary>
    ///  Queues an idempotent start; a preceding stop remains ordered before a restart.
    /// </summary>
    internal void Start()
    {
        lock (_sync)
        {
            ThrowIfReleased();
            if (IsPlaying)
            {
                return;
            }

            EnsureRoom();
            long sequence = ++_sequence;
            _commands.Add(new PercussionCommand(sequence, PercussionCommandKind.Start, _settings));
            Volatile.Write(ref _transportRequest, TransportRequest(sequence, DrumTransportState.Playing));
        }
    }

    /// <summary>
    ///  Queues an idempotent pause only for a requested running transport.
    /// </summary>
    internal void Pause()
    {
        lock (_sync)
        {
            ThrowIfReleased();
            if (!IsPlaying)
            {
                return;
            }

            EnsureRoom();
            long sequence = ++_sequence;
            _commands.Add(new PercussionCommand(sequence, PercussionCommandKind.Pause, _settings));
            Volatile.Write(ref _transportRequest, TransportRequest(sequence, DrumTransportState.Paused));
        }
    }

    /// <summary>
    ///  Queues release even while stopped, permitting Stop to cancel a pending audition.
    /// </summary>
    internal void Stop()
    {
        lock (_sync)
        {
            ThrowIfReleased();
            if (_commands.Count > 0 && _commands[^1].Kind == PercussionCommandKind.Stop)
            {
                return;
            }

            EnsureRoom();
            long sequence = ++_sequence;
            _commands.Add(new PercussionCommand(sequence, PercussionCommandKind.Stop, _settings));
            Volatile.Write(ref _transportRequest, TransportRequest(sequence, DrumTransportState.Stopped));
        }
    }

    /// <summary>
    ///  Coalesces pending auditions per instrument, retaining the newest audition's command order.
    /// </summary>
    internal void Audition(Cr78Instrument instrument, float velocity)
    {
        Cr78Kit.ValidateInstrument(instrument);
        Cr78Kit.ValidateLevel(velocity, nameof(velocity));
        lock (_sync)
        {
            ThrowIfReleased();
            for (int i = 0; i < _commands.Count; i++)
            {
                if (_commands[i].Kind == PercussionCommandKind.Audition && _commands[i].Instrument == instrument)
                {
                    _commands.RemoveAt(i);
                    break;
                }
            }

            EnsureRoom();
            _commands.Add(new PercussionCommand(++_sequence, PercussionCommandKind.Audition, _settings, instrument, velocity));
        }
    }

    /// <summary>
    ///  Copies a bounded batch once per audio block, or defers it rather than waiting for a caller.
    /// </summary>
    internal int TryDrain(PercussionCommand[] destination, out PercussionSettings? settings)
    {
        settings = null;
        if (!Monitor.TryEnter(_sync))
        {
            return -1;
        }

        try
        {
            settings = _settings;
            int count = _commands.Count;
            _commands.CopyTo(destination);
            _commands.Clear();
            return count;
        }
        finally
        {
            Monitor.Exit(_sync);
        }
    }

    /// <summary>
    ///  Clears a naturally completed start only if no newer Stop/Start request has superseded it.
    /// </summary>
    internal void CompleteStart(long sequence)
        => CompleteTransport(sequence, DrumTransportState.Playing);

    /// <summary>
    ///  Reconciles a pause that arrived after natural completion without overwriting a newer command.
    /// </summary>
    internal void CompletePause(long sequence)
        => CompleteTransport(sequence, DrumTransportState.Paused);

    /// <summary>
    ///  Closes admission and requests a graceful terminal release; the supplied engine remains owned by its host.
    /// </summary>
    internal void Release()
    {
        lock (_sync)
        {
            Volatile.Write(ref _releaseRequested, 1);
            Volatile.Write(ref _transportRequest, Volatile.Read(ref _transportRequest) & ~3L);
            _commands.Clear();
        }
    }

    private void EnsureRoom()
    {
        if (_commands.Count == MaximumCommands)
        {
            throw new InvalidOperationException("The transport command queue is full; let the audio thread consume a block before retrying.");
        }
    }

    private void ThrowIfReleased()
        => ObjectDisposedException.ThrowIf(ReleaseRequested, this);

    private void CompleteTransport(long sequence, DrumTransportState expectedState)
        => Interlocked.CompareExchange(ref _transportRequest,
            TransportRequest(sequence, DrumTransportState.Stopped), TransportRequest(sequence, expectedState));

    private static long TransportRequest(long sequence, DrumTransportState state)
        => (sequence << 2) | (long)state;
}
