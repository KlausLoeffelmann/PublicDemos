namespace SplitFlap.Audio.Core;

/// <summary>
///  Retains a bounded final-PCM history without making the audio producer wait for a reader.
/// </summary>
internal sealed class AudioOutputMonitor
{
    private readonly object _copySync = new();
    private readonly short[] _pcm;
    private readonly float[] _preClamp;
    private readonly int _windowSize;
    private long _firstFrame;
    private long _endFrame;
    private long _nextAttemptFrame = -1;
    private long _droppedBlocks;
    private bool _hasData;
    private bool _stopped;

    /// <summary>
    ///  Allocates enough history for the output queue, one FFT window, and two scheduling-margin blocks.
    /// </summary>
    internal AudioOutputMonitor(AudioFormat format, int framesPerBlock, int bufferCapacityFrames, int windowSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(format.SampleRate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(format.Channels);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(framesPerBlock);
        ArgumentOutOfRangeException.ThrowIfNegative(bufferCapacityFrames);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowSize);
        Format = format;
        FramesPerBlock = framesPerBlock;
        _windowSize = windowSize;
        CapacityFrames = checked(windowSize + bufferCapacityFrames + framesPerBlock * 2);
        _pcm = new short[checked(CapacityFrames * format.Channels)];
        _preClamp = new float[CapacityFrames];
    }

    /// <summary>
    ///  Gets the format of the interleaved PCM history.
    /// </summary>
    internal AudioFormat Format { get; }

    /// <summary>
    ///  Gets the producer's fixed block length.
    /// </summary>
    internal int FramesPerBlock { get; }

    /// <summary>
    ///  Gets the bounded history capacity in frames, not interleaved samples.
    /// </summary>
    internal int CapacityFrames { get; }

    /// <summary>
    ///  Gets the number of visualization blocks dropped through contention or skipped frame ranges.
    /// </summary>
    internal long DroppedBlocks
        => Volatile.Read(ref _droppedBlocks);

    /// <summary>
    ///  Gets whether this subscription has detached or its engine has stopped.
    /// </summary>
    internal bool IsStopped
        => Volatile.Read(ref _stopped);

    /// <summary>
    ///  Gets the short-copy gate, also used to model a slow reader deterministically in tests.
    /// </summary>
    internal object CopySynchronization
        => _copySync;

    /// <summary>
    ///  Offers one successfully submitted block; only this visualization copy may be dropped.
    /// </summary>
    /// <param name="startFrame">Absolute first frame, using the engine's unchanged render timeline.</param>
    /// <param name="pcm">Final interleaved signed 16-bit PCM.</param>
    /// <param name="preClamp">One pre-clamp sample per frame, in signed 16-bit sample units.</param>
    internal bool TryWrite(long startFrame, ReadOnlySpan<short> pcm, ReadOnlySpan<float> preClamp)
    {
        if (IsStopped)
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(startFrame);
        if (pcm.Length != FramesPerBlock * Format.Channels || preClamp.Length != FramesPerBlock)
        {
            throw new ArgumentException("An output-monitor write must contain one complete output block.");
        }

        // These frame IDs belong to one producer. Updating the attempted end even on a drop
        // avoids counting the same missing block again when the next copy succeeds.
        if (_nextAttemptFrame >= 0 && startFrame != _nextAttemptFrame)
        {
            if (startFrame < _nextAttemptFrame)
            {
                Interlocked.Increment(ref _droppedBlocks);
                return false;
            }

            Interlocked.Add(ref _droppedBlocks, (startFrame - _nextAttemptFrame + FramesPerBlock - 1) / FramesPerBlock);
        }

        _nextAttemptFrame = startFrame + FramesPerBlock;
        if (!Monitor.TryEnter(_copySync))
        {
            Interlocked.Increment(ref _droppedBlocks);
            return false;
        }

        try
        {
            if (IsStopped)
            {
                return false;
            }

            int offset = (int)(startFrame % CapacityFrames);
            int first = Math.Min(FramesPerBlock, CapacityFrames - offset);
            int samples = first * Format.Channels;
            pcm[..samples].CopyTo(_pcm.AsSpan(offset * Format.Channels));
            pcm[samples..].CopyTo(_pcm);
            preClamp[..first].CopyTo(_preClamp.AsSpan(offset));
            preClamp[first..].CopyTo(_preClamp);

            // A gap starts a new contiguous run. Do not splice samples from before and after
            // a missed copy into an FFT: wait until an entire new window is available.
            if (!_hasData || startFrame != _endFrame)
            {
                _firstFrame = startFrame;
            }

            Volatile.Write(ref _endFrame, startFrame + FramesPerBlock);
            _firstFrame = Math.Max(_firstFrame, _endFrame - CapacityFrames);
            _hasData = true;
            return true;
        }
        finally
        {
            Monitor.Exit(_copySync);
        }
    }

    /// <summary>
    ///  Copies exactly the requested played window, or rejects a gap, overwrite, or not-yet-captured end.
    /// </summary>
    internal bool TryCopyWindow(
        long endFrame, Span<short> pcm, Span<float> preClamp, out AudioOutputWindow window)
    {
        window = default;
        if (pcm.Length != _windowSize * Format.Channels || preClamp.Length != _windowSize)
        {
            throw new ArgumentException("An output-monitor read must have exactly one analysis window of storage.");
        }

        // Submission is published just before its history copy. Reject that brief future
        // cursor without taking the copy gate and unnecessarily making the producer drop it.
        if (IsStopped || endFrame < _windowSize || endFrame > Volatile.Read(ref _endFrame))
        {
            return false;
        }

        lock (_copySync)
        {
            long startFrame = endFrame - _windowSize;
            if (IsStopped || !_hasData || startFrame < _firstFrame || endFrame > _endFrame)
            {
                return false;
            }

            int offset = (int)(startFrame % CapacityFrames);
            int first = Math.Min(_windowSize, CapacityFrames - offset);
            int samples = first * Format.Channels;
            _pcm.AsSpan(offset * Format.Channels, samples).CopyTo(pcm);
            _pcm.AsSpan(0, pcm.Length - samples).CopyTo(pcm[samples..]);
            _preClamp.AsSpan(offset, first).CopyTo(preClamp);
            _preClamp.AsSpan(0, preClamp.Length - first).CopyTo(preClamp[first..]);
            window = new(endFrame, DroppedBlocks);
            return true;
        }
    }

    /// <summary>
    ///  Invalidates the subscription without waiting for either thread or freeing shared managed arrays.
    /// </summary>
    internal void Stop()
        => Volatile.Write(ref _stopped, true);
}

/// <summary>
///  Identifies one coherent copied window and the subscription's cumulative visualization-drop count.
/// </summary>
/// <param name="EndFrame">Exclusive absolute end frame.</param>
/// <param name="DroppedBlocks">Cumulative blocks missed by this subscription.</param>
internal readonly record struct AudioOutputWindow(long EndFrame, long DroppedBlocks);
