using SplitFlap.Audio.Percussion;

namespace SplitFlap.Audio.Sequencing;

/// <summary>
///  Plays an editable percussion score from the engine's sample clock through one reusable voice.
/// </summary>
/// <remarks>
///  The caller owns the engine and observes its Completion task. Score/tempo changes are
///  coalesced at bar boundaries while running, or accepted at the next block while stopped.
///  Commands are bounded; flooding more than 128 unconsumed transport requests throws instead
///  of silently losing their order. No wall-clock timer determines musical onsets.
/// </remarks>
public sealed class DrumMachinePlayer : IDisposable
{
    private readonly AudioEngine _engine;
    private readonly PercussionMailbox _mailbox;
    private readonly PercussionRenderer _renderer;
    private readonly PlayerVoice _voice;
    private int _disposed;

    /// <summary>
    ///  Prepares a dry kit and admits one persistent voice without taking ownership of the engine.
    /// </summary>
    public DrumMachinePlayer(AudioEngine engine, PercussionScore score, Tempo tempo)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(score);
        Cr78Kit.ValidateSampleRate(engine.SampleRate);
        PercussionClock.ValidateTempo(tempo);
        _engine = engine;
        _mailbox = new PercussionMailbox(score, tempo);
        _renderer = new PercussionRenderer(engine.SampleRate, _mailbox, (uint)Random.Shared.Next(1, int.MaxValue));
        _voice = new PlayerVoice(engine, _mailbox, _renderer);
        engine.Trigger(_voice, reverbSend: 0);
    }

    /// <summary>
    ///  Gets the latest requested immutable score, including an edit waiting for the next bar.
    /// </summary>
    public PercussionScore Score
        => _mailbox.Settings.Score;

    /// <summary>
    ///  Gets or requests a tempo from 1 to 1,000 BPM, applied at a bar boundary while playing.
    /// </summary>
    public Tempo Tempo
    {
        get => _mailbox.Settings.Tempo;
        set => _mailbox.SetTempo(value);
    }

    /// <summary>
    ///  Gets or sets whether the score wraps; the next block observes the switch. Initially true.
    /// </summary>
    public bool Loop
    {
        get => _mailbox.Settings.Loop;
        set => _mailbox.SetLoop(value);
    }

    /// <summary>
    ///  Gets or sets the zero-to-one CY/HH metallic level for future hits. Initially zero.
    /// </summary>
    public float MetallicLevel
    {
        get => _mailbox.Settings.MetallicLevel;
        set => _mailbox.SetMetallicLevel(value);
    }

    /// <summary>
    ///  Gets the latest accepted Play/Stop state, cleared on natural completion or engine shutdown.
    /// </summary>
    /// <remarks>
    ///  This controls transport buttons. GetPlaybackSnapshot reports the separately delayed,
    ///  played position, which may still be sounding previously queued output after Stop.
    /// </remarks>
    public bool IsPlaying
        => Volatile.Read(ref _disposed) == 0 && !_engine.Completion.IsCompleted && _mailbox.IsPlaying;

    /// <summary>
    ///  Requests an already validated/indexed score without modifying the bar currently rendering.
    /// </summary>
    public void SetScore(PercussionScore score)
        => _mailbox.SetScore(score);

    /// <summary>
    ///  Starts at the first bar on the next audio block; repeated Start while playing is harmless.
    /// </summary>
    public void Start()
        => _mailbox.Start();

    /// <summary>
    ///  Stops future score hits on the next block and releases current sounds over five milliseconds.
    /// </summary>
    public void Stop()
        => _mailbox.Stop();

    /// <summary>
    ///  Auditions a prepared sound while running or stopped, coalescing repeated pending clicks.
    /// </summary>
    public void Audition(Cr78Instrument instrument, float velocity = 1f)
        => _mailbox.Audition(instrument, velocity);

    /// <summary>
    ///  Reads the played playhead using completed device buffers, or labels submitted-stream fallback.
    /// </summary>
    /// <remarks>
    ///  Completed buffers are not a measurement at the loudspeaker. Device, buffer, and UI
    ///  refresh latency remain. Exceptionally stale or concurrently overwritten history is
    ///  reported as unsynchronized rather than displaying a falsely precise rendered playhead.
    /// </remarks>
    public DrumPlaybackSnapshot GetPlaybackSnapshot()
    {
        bool synchronized = _engine.TryGetPlaybackPosition(out long frames);
        return _renderer.GetSnapshot(frames, synchronized);
    }

    /// <summary>
    ///  Releases the player's engine voice after its short tail, without disposing the supplied engine.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            _voice.Release();
        }
    }

    private sealed class PlayerVoice(
        AudioEngine engine,
        PercussionMailbox mailbox,
        PercussionRenderer renderer) : IVoice
    {
        private long _frame;
        private long _blockOrigin = -1;

        /// <summary>
        ///  Gets whether disposal's graceful release has finished.
        /// </summary>
        public bool IsFinished
            => renderer.IsFinished;

        /// <summary>
        ///  Captures the engine's actual admission frame, then advances exactly once per rendered sample.
        /// </summary>
        public float Next()
        {
            long block = engine.RenderedFrames;
            if (block != _blockOrigin)
            {
                if (_blockOrigin < 0)
                {
                    _frame = block;
                }

                _blockOrigin = block;
                renderer.BeginBlock(_frame);
            }

            return renderer.Next(_frame++);
        }

        /// <summary>
        ///  Requests release through the bounded handoff without touching audio-thread DSP state.
        /// </summary>
        public void Release()
            => mailbox.Release();
    }
}
