using SplitFlap.Audio.Percussion;

namespace SplitFlap.Audio.Sequencing;

/// <summary>
///  An immutable settings handoff; score indexing and object allocation occur only on the caller.
/// </summary>
internal sealed class PercussionSettings
{
    /// <summary>
    ///  Captures the latest requested score, tempo, immediate controls, and score/tempo revision.
    /// </summary>
    internal PercussionSettings(PercussionScore score, Tempo tempo, bool loop, float metallicLevel, long revision)
    {
        Score = score;
        Tempo = tempo;
        Loop = loop;
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
    ///  Gets the hi-hat/cymbal metallic-layer level.
    /// </summary>
    internal float MetallicLevel { get; }

    /// <summary>
    ///  Gets the score/tempo revision used to report pending audible edits.
    /// </summary>
    internal long Revision { get; }
}

/// <summary>
///  The bounded transport actions whose relative order must not be lost.
/// </summary>
internal enum PercussionCommandKind
{
    /// <summary>
    ///  Starts or restarts the score from its first bar.
    /// </summary>
    Start,

    /// <summary>
    ///  Stops future events and releases current sounds.
    /// </summary>
    Stop,

    /// <summary>
    ///  Auditions an instrument independently of the score transport.
    /// </summary>
    Audition
}

/// <summary>
///  A value-only command copied into prepared audio-thread storage.
/// </summary>
internal readonly struct PercussionCommand
{
    /// <summary>
    ///  Captures one ordered transport or audition request.
    /// </summary>
    internal PercussionCommand(long sequence, PercussionCommandKind kind, Cr78Instrument instrument = default, float velocity = 1)
    {
        Sequence = sequence;
        Kind = kind;
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
    ///  Gets the audition instrument when this is an audition request.
    /// </summary>
    internal Cr78Instrument Instrument { get; }

    /// <summary>
    ///  Gets the audition velocity.
    /// </summary>
    internal float Velocity { get; }
}

/// <summary>
///  Coalesces edits and auditions while preserving ordered Start/Stop requests in bounded storage.
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
        _settings = new PercussionSettings(score, tempo, loop: true, metallicLevel: 0, revision: 1);
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
        => (Volatile.Read(ref _transportRequest) & 1) != 0;

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
            PercussionSettings old = _settings;
            if (!ReferenceEquals(old.Score, score))
            {
                Volatile.Write(ref _settings, new PercussionSettings(score, old.Tempo, old.Loop, old.MetallicLevel, old.Revision + 1));
            }
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
            PercussionSettings old = _settings;
            if (old.Tempo != tempo)
            {
                Volatile.Write(ref _settings, new PercussionSettings(old.Score, tempo, old.Loop, old.MetallicLevel, old.Revision + 1));
            }
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
            PercussionSettings old = _settings;
            Volatile.Write(ref _settings, new PercussionSettings(old.Score, old.Tempo, loop, old.MetallicLevel, old.Revision));
        }
    }

    /// <summary>
    ///  Publishes the next-block metallic-layer level while leaving current tails intact.
    /// </summary>
    internal void SetMetallicLevel(float level)
    {
        Cr78Kit.ValidateLevel(level, nameof(level));
        lock (_sync)
        {
            ThrowIfReleased();
            PercussionSettings old = _settings;
            Volatile.Write(ref _settings, new PercussionSettings(old.Score, old.Tempo, old.Loop, level, old.Revision));
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
            _commands.Add(new PercussionCommand(sequence, PercussionCommandKind.Start));
            Volatile.Write(ref _transportRequest, sequence * 2 + 1);
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
            _commands.Add(new PercussionCommand(sequence, PercussionCommandKind.Stop));
            Volatile.Write(ref _transportRequest, sequence * 2);
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
            _commands.Add(new PercussionCommand(++_sequence, PercussionCommandKind.Audition, instrument, velocity));
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
        => Interlocked.CompareExchange(ref _transportRequest, sequence * 2, sequence * 2 + 1);

    /// <summary>
    ///  Closes admission and requests a graceful terminal release; the supplied engine remains owned by its host.
    /// </summary>
    internal void Release()
    {
        lock (_sync)
        {
            Volatile.Write(ref _releaseRequested, 1);
            Volatile.Write(ref _transportRequest, _transportRequest & ~1L);
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
}
