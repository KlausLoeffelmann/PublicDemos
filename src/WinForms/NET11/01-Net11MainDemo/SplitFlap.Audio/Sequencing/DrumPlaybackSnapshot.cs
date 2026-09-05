namespace SplitFlap.Audio.Sequencing;

/// <summary>
///  An immutable playhead reading aligned to completed output, or explicitly approximate submitted output.
/// </summary>
public readonly record struct DrumPlaybackSnapshot
{
    /// <summary>
    ///  Creates one coherent reading; bar and step are always zero-based.
    /// </summary>
    public DrumPlaybackSnapshot(int bar, int step, bool isPlaying, bool hasPendingChanges, bool isPlaybackSynchronized)
    {
        Bar = bar;
        Step = step;
        IsPlaying = isPlaying;
        HasPendingChanges = hasPendingChanges;
        IsPlaybackSynchronized = isPlaybackSynchronized;
    }

    /// <summary>
    ///  Gets the played bar, not the editor's selected bar.
    /// </summary>
    public int Bar { get; }

    /// <summary>
    ///  Gets the last played sixteenth-note step within the bar.
    /// </summary>
    public int Step { get; }

    /// <summary>
    ///  Gets whether the score was playing at this output position.
    /// </summary>
    public bool IsPlaying { get; }

    /// <summary>
    ///  Gets whether the latest requested score/tempo revision has not yet reached this output position.
    /// </summary>
    public bool HasPendingChanges { get; }

    /// <summary>
    ///  Gets whether a real completed-buffer clock and coherent retained transport history were available.
    /// </summary>
    public bool IsPlaybackSynchronized { get; }
}

/// <summary>
///  Describes one bar or transport transition without allocating a history object on the audio thread.
/// </summary>
internal readonly struct PercussionHistoryPoint
{
    /// <summary>
    ///  Captures the exact musical bar origin together with its effective absolute output frame.
    /// </summary>
    internal PercussionHistoryPoint(
        long frame, int bar, int stoppedStep, bool playing, decimal barOrigin, decimal framesPerStep, long revision)
    {
        Frame = frame;
        Bar = bar;
        StoppedStep = stoppedStep;
        Playing = playing;
        BarOrigin = barOrigin;
        FramesPerStep = framesPerStep;
        Revision = revision;
    }

    /// <summary>
    ///  Gets the first rendered frame governed by this state.
    /// </summary>
    internal long Frame { get; }

    /// <summary>
    ///  Gets the zero-based score bar.
    /// </summary>
    internal int Bar { get; }

    /// <summary>
    ///  Gets the retained playhead when this point stops the transport.
    /// </summary>
    internal int StoppedStep { get; }

    /// <summary>
    ///  Gets whether subsequent frames belong to a running bar.
    /// </summary>
    internal bool Playing { get; }

    /// <summary>
    ///  Gets the fractional origin needed to reproduce the renderer's onset rounding.
    /// </summary>
    internal decimal BarOrigin { get; }

    /// <summary>
    ///  Gets this bar's fractional sixteenth-note length.
    /// </summary>
    internal decimal FramesPerStep { get; }

    /// <summary>
    ///  Gets the score/tempo revision applied to these frames.
    /// </summary>
    internal long Revision { get; }

    /// <summary>
    ///  Locates the last triggered step using the renderer's identical nearest-sample convention.
    /// </summary>
    internal int StepAt(long frame)
    {
        if (!Playing)
        {
            return StoppedStep;
        }

        int step = 0;
        while (step + 1 < PercussionScore.StepsPerBar &&
            PercussionClock.RoundFrame(BarOrigin + (step + 1) * FramesPerStep) <= frame)
        {
            step++;
        }

        return step;
    }
}

/// <summary>
///  A bounded single-writer history whose optimistic readers can never block rendering.
/// </summary>
internal sealed class PercussionHistory
{
    private readonly PercussionHistoryPoint[] _points;
    private long _version;
    private long _firstFrame = long.MaxValue;
    private int _next;
    private int _count;
    private bool _overwritten;

    /// <summary>
    ///  Reserves ample bar/transport history for queued output without allocating as a loop plays.
    /// </summary>
    internal PercussionHistory(int capacity = 4_096)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 2);
        _points = new PercussionHistoryPoint[capacity];
    }

    /// <summary>
    ///  Publishes a transition; multiple commands affecting the same sample leave only their final state.
    /// </summary>
    internal void Write(PercussionHistoryPoint point)
    {
        // The odd version announces a write before any multi-field value can be changed.
        // Readers verify the even version again after copying; torn data is discarded.
        long writing = Interlocked.Increment(ref _version);
        if (_count > 0 && _points[(_next + _points.Length - 1) % _points.Length].Frame == point.Frame)
        {
            _points[(_next + _points.Length - 1) % _points.Length] = point;
        }
        else
        {
            _firstFrame = Math.Min(_firstFrame, point.Frame);
            _points[_next] = point;
            _next = (_next + 1) % _points.Length;
            if (_count < _points.Length)
            {
                _count++;
            }
            else
            {
                _overwritten = true;
            }
        }

        Volatile.Write(ref _version, writing + 1);
    }

    /// <summary>
    ///  Reads the state of a played frame, explicitly reporting overwritten or contended history.
    /// </summary>
    internal bool TryRead(long frame, out PercussionHistoryPoint point, out bool historyUnavailable)
    {
        for (int attempt = 0; attempt < 4; attempt++)
        {
            long before = Volatile.Read(ref _version);
            if ((before & 1) != 0)
            {
                continue;
            }

            int count = _count;
            int next = _next;
            bool found = false;
            PercussionHistoryPoint candidate = default;
            for (int i = 0; i < count; i++)
            {
                candidate = _points[(next + _points.Length - 1 - i) % _points.Length];
                if (candidate.Frame <= frame)
                {
                    found = true;
                    break;
                }
            }

            bool unavailable = !found && _overwritten && frame >= _firstFrame;
            if (before == Volatile.Read(ref _version))
            {
                point = found ? candidate : default;
                historyUnavailable = unavailable;
                return found;
            }
        }

        point = default;
        historyUnavailable = true;
        return false;
    }
}
