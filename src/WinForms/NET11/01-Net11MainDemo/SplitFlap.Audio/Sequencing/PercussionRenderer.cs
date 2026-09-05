using SplitFlap.Audio.Percussion;

namespace SplitFlap.Audio.Sequencing;

/// <summary>
///  The single-audio-thread score renderer, separated from the engine for endpoint-free timing tests.
/// </summary>
internal sealed class PercussionRenderer
{
    private readonly int _sampleRate;
    private readonly PercussionMailbox _mailbox;
    private readonly Cr78VoiceBank _bank;
    private readonly PercussionClock _clock;
    private readonly PercussionHistory _history;
    private readonly PercussionCommand[] _commands = new PercussionCommand[PercussionMailbox.MaximumCommands];
    private readonly Action<long, PercussionHit>? _hitObserver;
    private readonly long _initialRevision;
    private PercussionSettings _activeSettings;
    private PercussionSettings _pendingSettings;
    private decimal _barOrigin;
    private long _startSequence;
    private int _bar;
    private int _step;
    private int _lastRenderedStep;
    private bool _firstBar;
    private bool _playing;
    private bool _disposing;
    private bool _hasBlock;

    /// <summary>
    ///  Prepares all channels, command storage, and transport history before joining an engine.
    /// </summary>
    internal PercussionRenderer(
        int sampleRate,
        PercussionMailbox mailbox,
        uint noiseSeed,
        Action<long, PercussionHit>? hitObserver = null,
        int historyCapacity = 4_096)
    {
        _sampleRate = sampleRate;
        _mailbox = mailbox;
        _activeSettings = _pendingSettings = mailbox.Settings;
        _initialRevision = _activeSettings.Revision;
        _clock = new PercussionClock(sampleRate, _activeSettings.Tempo);
        _bank = new Cr78VoiceBank(sampleRate, noiseSeed);
        _history = new PercussionHistory(historyCapacity);
        _hitObserver = hitObserver;
    }

    /// <summary>
    ///  Gets whether a terminal release has finished; ordinary Stop leaves auditions available.
    /// </summary>
    internal bool IsFinished
        => _disposing && !_bank.IsActive;

    /// <summary>
    ///  Gets the total number of scheduled hits, including deliberately zero-velocity events.
    /// </summary>
    internal long RenderedHitCount { get; private set; }

    /// <summary>
    ///  Gets the most recent scheduled onset's absolute sample, or minus one before the first hit.
    /// </summary>
    internal long LastHitFrame { get; private set; } = -1;

    /// <summary>
    ///  Consumes a bounded command batch only at a new engine block, never halfway through mixing one.
    /// </summary>
    internal void BeginBlock(long frame)
    {
        if (!_hasBlock)
        {
            _hasBlock = true;
            Record(frame);
        }

        if (_mailbox.ReleaseRequested)
        {
            if (!_disposing)
            {
                _disposing = true;
                _playing = false;
                _bank.ReleaseAll();
                Record(frame);
            }

            return;
        }

        int count = _mailbox.TryDrain(_commands, out PercussionSettings? requested);
        if (count < 0)
        {
            return;
        }

        _pendingSettings = requested!;
        if (!_playing && ApplyPending())
        {
            Record(frame);
        }

        for (int i = 0; i < count; i++)
        {
            PercussionCommand command = _commands[i];
            switch (command.Kind)
            {
                case PercussionCommandKind.Start:
                    if (!_playing)
                    {
                        ApplyPending();
                        _clock.Reset(frame);
                        _barOrigin = frame;
                        _bar = _step = _lastRenderedStep = 0;
                        _firstBar = true;
                        _playing = true;
                        _startSequence = command.Sequence;
                        Record(frame);
                    }
                    break;

                case PercussionCommandKind.Stop:
                    _playing = false;
                    _bank.ReleaseAll();
                    ApplyPending();
                    Record(frame);
                    break;

                case PercussionCommandKind.Audition:
                    _bank.Trigger(command.Instrument, command.Velocity, _sampleRate / 4);
                    if (command.Instrument is Cr78Instrument.HiHat or Cr78Instrument.Cymbal)
                    {
                        _bank.Trigger(Cr78Instrument.MetallicBeat,
                            command.Velocity * _pendingSettings.MetallicLevel, _sampleRate / 4);
                    }
                    break;
            }
        }
    }

    /// <summary>
    ///  Renders one consecutive absolute frame, using only the sample clock to trigger score events.
    /// </summary>
    internal float Next(long frame)
    {
        if (_playing && frame >= _clock.NextFrame)
        {
            RenderStep(frame);
        }

        return _bank.Next();
    }

    /// <summary>
    ///  Resolves a completed-frame count through retained bar/tempo history instead of rendered state.
    /// </summary>
    internal DrumPlaybackSnapshot GetSnapshot(long completedFrames, bool deviceSynchronized)
    {
        // A clock reports a count, not a sample index: completed N means frame N-1 was heard.
        long playedFrame = Math.Max(-1, completedFrames - 1);
        long requestedRevision = _mailbox.Settings.Revision;
        if (!_history.TryRead(playedFrame, out PercussionHistoryPoint point, out bool unavailable))
        {
            return new DrumPlaybackSnapshot(0, 0, isPlaying: false,
                hasPendingChanges: requestedRevision != _initialRevision,
                isPlaybackSynchronized: deviceSynchronized && !unavailable);
        }

        return new DrumPlaybackSnapshot(point.Bar, point.StepAt(playedFrame), point.Playing,
            requestedRevision != point.Revision, deviceSynchronized);
    }

    private void RenderStep(long frame)
    {
        if (_step == 0)
        {
            if (!_firstBar)
            {
                ApplyPending();
                _bar++;
                if (_bar >= _activeSettings.Score.BarCount)
                {
                    if (!_pendingSettings.Loop)
                    {
                        _bar = _activeSettings.Score.BarCount - 1;
                        _playing = false;
                        _mailbox.CompleteStart(_startSequence);
                        Record(frame);
                        return;
                    }

                    _bar = 0;
                }
            }

            _firstBar = false;
            _barOrigin = _clock.ExactFrame;
            Record(frame);
        }

        float metallicVelocity = 0;
        foreach (PercussionHit hit in _activeSettings.Score.GetStepHits(_bar, _step))
        {
            _bank.Trigger(hit.Instrument, hit.Velocity, _clock.GateFrames(hit.GateSteps));
            if (hit.Instrument is Cr78Instrument.HiHat or Cr78Instrument.Cymbal)
            {
                metallicVelocity = Math.Max(metallicVelocity, hit.Velocity);
            }

            RenderedHitCount++;
            LastHitFrame = frame;
            _hitObserver?.Invoke(frame, hit);
        }

        // HH and CY are independent noise voices, not an 808 open/closed-hat choke pair.
        // Simultaneous HH/CY events excite their one shared metallic layer once, at the
        // stronger velocity. Its own decay continues independently of either noise tail.
        _bank.Trigger(Cr78Instrument.MetallicBeat,
            metallicVelocity * _pendingSettings.MetallicLevel, _sampleRate / 4);
        _lastRenderedStep = _step;
        _step = (_step + 1) % PercussionScore.StepsPerBar;
        _clock.Advance();
    }

    private bool ApplyPending()
    {
        if (_activeSettings.Revision == _pendingSettings.Revision)
        {
            return false;
        }

        _activeSettings = _pendingSettings;
        _clock.SetTempo(_activeSettings.Tempo);
        if (!_playing)
        {
            _bar = Math.Min(_bar, _activeSettings.Score.BarCount - 1);
        }

        return true;
    }

    private void Record(long frame)
        => _history.Write(new PercussionHistoryPoint(frame, _bar, _lastRenderedStep, _playing,
            _barOrigin, _clock.FramesPerStep, _activeSettings.Revision));
}
