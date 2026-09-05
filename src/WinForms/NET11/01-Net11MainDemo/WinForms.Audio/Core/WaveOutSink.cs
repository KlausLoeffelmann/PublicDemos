using Microsoft.Win32.SafeHandles;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace WinForms.Audio.Core;

/// <summary>
///  Plays PCM through Windows' WinMM API using a few reusable native buffers.
///  Four 20 ms buffers hold 80 ms of audio when full; device buffering adds to that,
///  so queue capacity is not a promise of end-to-end latency.
/// </summary>
public sealed class WaveOutSink : IAudioSink, IAudioPlaybackProgress
{
    private const int WhdrDone = 0x0000_0001;
    private static readonly int s_headerSize = Marshal.SizeOf<WaveHeader>();
    private static readonly int s_flagsOffset = Marshal.OffsetOf<WaveHeader>(nameof(WaveHeader.Flags)).ToInt32();

    private readonly IWaveOutApi _native;
    private readonly AutoResetEvent _bufferReturned;
    private readonly ManualResetEvent _disposeRequested;
    private readonly WaitHandle[] _waitHandles;
    private readonly Lock _nativeSync = new();
    private readonly Lock _disposeSync = new();
    private readonly BufferSlot[] _buffers;
    private readonly short[] _staging;
    private SafeWaitHandle? _callbackHandle;
    private bool _callbackReferenceHeld;
    private IntPtr _device;
    private bool _stopping;
    private bool _disposed;
    private long _submittedFrames;
    private long _completedFrames;

    /// <summary>
    ///  Opens the default output device.
    /// </summary>
    /// <param name="format">Sample rate and channels.</param>
    /// <param name="bufferMilliseconds">Block size; smaller is lower latency but more fragile.</param>
    /// <param name="bufferCount">Three to sixteen blocks in flight; more buffering gives scheduling headroom at the cost of latency.</param>
    public WaveOutSink(AudioFormat format = default, int bufferMilliseconds = 20, int bufferCount = 4)
        : this(WinMm.Instance, format, bufferMilliseconds, bufferCount)
    {
    }

    /// <summary>
    ///  Opens an output device through an injectable native-call boundary for endpoint-free tests.
    /// </summary>
    internal WaveOutSink(
        IWaveOutApi native,
        AudioFormat format = default,
        int bufferMilliseconds = 20,
        int bufferCount = 4)
    {
        ArgumentNullException.ThrowIfNull(native);
        _native = native;
        Format = format == default ? AudioFormat.Default : format;

        if (Format.SampleRate is < 8_000 or > 384_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(format),
                Format.SampleRate,
                "The sample rate must be between 8,000 and 384,000 Hz.");
        }

        if (Format.Channels is < 1 or > 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(format),
                Format.Channels,
                "The channel count must be one or two.");
        }

        bufferCount = Math.Clamp(bufferCount, 3, 16);
        FramesPerBuffer = Math.Max(64, Format.SampleRate * Math.Clamp(bufferMilliseconds, 5, 200) / 1000);
        _staging = new short[FramesPerBuffer * Format.Channels];
        _buffers = new BufferSlot[bufferCount];

        for (int i = 0; i < _buffers.Length; i++)
        {
            _buffers[i] = new();
        }

        WaveFormatEx nativeFormat = new()
        {
            FormatTag = 1,
            Channels = (ushort)Format.Channels,
            SamplesPerSecond = (uint)Format.SampleRate,
            BitsPerSample = 16,
            BlockAlign = (ushort)Format.BlockAlign,
            AverageBytesPerSecond = (uint)Format.BytesPerSecond,
            ExtraSize = 0
        };

        AutoResetEvent? bufferReturned = null;
        ManualResetEvent? disposeRequested = null;

        try
        {
            _bufferReturned = bufferReturned = new(false);
            _disposeRequested = disposeRequested = new(false);
            _waitHandles = [_bufferReturned, _disposeRequested];

            // WinMM retains this event handle beyond waveOutOpen. Keep a native-lifetime
            // reference even if a failed close forces us to dispose the managed events first.
            _callbackHandle = _bufferReturned.SafeWaitHandle;
            _callbackHandle.DangerousAddRef(ref _callbackReferenceHeld);

            uint openResult = _native.Open(out IntPtr device, in nativeFormat, _callbackHandle);
            if (openResult == 0)
            {
                _device = device;
            }

            Check("waveOutOpen", openResult);

            int bytes = _staging.Length * sizeof(short);
            foreach (BufferSlot slot in _buffers)
            {
                // Record each allocation immediately: even a failure allocating PCM must
                // leave enough managed ownership information to free its unfinished header.
                slot.Header = Marshal.AllocHGlobal(s_headerSize);
                slot.Data = Marshal.AllocHGlobal(bytes);
                WaveHeader header = new()
                {
                    Data = slot.Data,
                    BufferLength = (uint)bytes
                };

                Marshal.StructureToPtr(header, slot.Header, false);
                Marshal.Copy(_staging, 0, slot.Data, _staging.Length);
                Check("waveOutPrepareHeader", _native.PrepareHeader(_device, slot.Header, (uint)s_headerSize));
                slot.Prepared = true;
            }
        }
        catch (Exception error)
        {
            List<Exception> cleanupErrors = [];
            try
            {
                ReleaseNativeResources(cleanupErrors);
            }
            finally
            {
                disposeRequested?.Dispose();
                bufferReturned?.Dispose();
            }

            if (cleanupErrors.Count != 0)
            {
                // If the driver refuses to relinquish ownership, even a failed constructor
                // must retain that native storage and callback rather than free live resources.
                cleanupErrors.Insert(0, error);
                throw new AggregateException("Opening WinMM output and releasing its resources failed.", cleanupErrors);
            }

            throw;
        }
    }

    /// <summary>
    ///  Gets the PCM format accepted by this output device.
    /// </summary>
    public AudioFormat Format { get; }

    /// <summary>
    ///  Gets the fixed number of interleaved frames in each output block.
    /// </summary>
    public int FramesPerBuffer { get; }

    /// <summary>
    ///  Gets the last observed whole-buffer completion; reading it never touches native memory.
    /// </summary>
    public long CompletedFrames
        => Volatile.Read(ref _completedFrames);

    /// <summary>
    ///  Gets the maximum number of queued frames, excluding the engine's not-yet-submitted block.
    /// </summary>
    public int BufferCapacityFrames
        => _buffers.Length * FramesPerBuffer;

    /// <summary>
    ///  Copies and queues one complete PCM block, waiting for a returned buffer when necessary.
    /// </summary>
    /// <param name="pcm">Exactly <see cref="FramesPerBuffer"/> times the channel count samples.</param>
    public void Write(ReadOnlySpan<short> pcm)
    {
        lock (_nativeSync)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _stopping), this);

            if (pcm.Length != _staging.Length)
            {
                throw new ArgumentException($"Expected {_staging.Length} samples, got {pcm.Length}.", nameof(pcm));
            }

            // The lock covers scanning, waiting, copying, and queuing. Neither another writer
            // nor teardown can reuse the staging array or free a header while we inspect it.
            BufferSlot slot = AcquireFreeBuffer();

            // Marshal.Copy accepts an array, not a span. This reusable staging step adds
            // one managed copy (96,000 bytes per audio second at the default format).
            pcm.CopyTo(_staging);
            Marshal.Copy(_staging, 0, slot.Data, _staging.Length);

            // Do not marshal the whole prepared header back: WinMM owns its other fields.
            int flags = Marshal.ReadInt32(slot.Header, s_flagsOffset);
            Marshal.WriteInt32(slot.Header, s_flagsOffset, flags & ~WhdrDone);
            Check("waveOutWrite", _native.Write(_device, slot.Header, (uint)s_headerSize));
            _submittedFrames += FramesPerBuffer;
            slot.EndFrame = _submittedFrames;
            slot.Queued = true;
            ObserveReturnedBuffers();
        }
    }

    /// <summary>
    ///  Wakes blocked writers, returns device-owned buffers, and releases the output device.
    ///  If native cleanup fails, a later call can retry resources that are still owned by WinMM.
    /// </summary>
    public void Dispose()
    {
        lock (_disposeSync)
        {
            if (_disposed)
            {
                return;
            }

            if (!Volatile.Read(ref _stopping))
            {
                Volatile.Write(ref _stopping, true);

                // A writer can be sleeping while holding _nativeSync. Signal before taking
                // that lock, and serialize disposers so nobody signals an already-closed event.
                _disposeRequested.Set();
            }

            List<Exception> errors = [];
            lock (_nativeSync)
            {
                try
                {
                    ReleaseNativeResources(errors);
                }
                finally
                {
                    // No writer can still be in WaitAny; future writers only throw.
                    _disposeRequested.Dispose();
                    _bufferReturned.Dispose();
                }

                _disposed = _device == IntPtr.Zero;
            }

            ThrowCleanupErrors(errors);
        }
    }

    private BufferSlot AcquireFreeBuffer()
    {
        while (true)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _stopping), this);
            ObserveReturnedBuffers();

            foreach (BufferSlot slot in _buffers)
            {
                if (!slot.Queued)
                {
                    return slot;
                }
            }

            // Reuse the handle array as well as the audio buffers; this is the pacing hot path.
            WaitHandle.WaitAny(_waitHandles, 100);
        }
    }

    private void ObserveReturnedBuffers()
    {
        long completed = _completedFrames;
        foreach (BufferSlot slot in _buffers)
        {
            if (slot.Queued && (Marshal.ReadInt32(slot.Header, s_flagsOffset) & WhdrDone) != 0)
            {
                // WinMM plays submissions in order, even when their header slots are reused
                // in a different order. Scan every returned slot before choosing a free one.
                completed = Math.Max(completed, slot.EndFrame);
                slot.Queued = false;
            }
        }

        Volatile.Write(ref _completedFrames, completed);
    }

    private void ReleaseNativeResources(List<Exception> errors)
    {
        // Never observe WHDR_DONE here: reset marks discarded buffers done, not played.
        if (_device != IntPtr.Zero)
        {
            TryCleanup("waveOutReset", () => _native.Reset(_device), errors);
        }

        bool allUnprepared = true;
        foreach (BufferSlot slot in _buffers)
        {
            if (slot.Prepared &&
                TryCleanup("waveOutUnprepareHeader", () => _native.UnprepareHeader(_device, slot.Header, (uint)s_headerSize), errors))
            {
                slot.Prepared = false;
            }

            if (slot.Prepared)
            {
                // A reset failure does not imply ownership returned. Only successful unprepare
                // permits freeing this storage; retaining it is safer than a driver use-after-free.
                allUnprepared = false;
                continue;
            }

            if (slot.Data != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(slot.Data);
                slot.Data = IntPtr.Zero;
            }

            if (slot.Header != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(slot.Header);
                slot.Header = IntPtr.Zero;
            }

            slot.Queued = false;
        }

        // Keep the device open for a possible retry if any header could not be unprepared.
        if (_device != IntPtr.Zero && allUnprepared &&
            TryCleanup("waveOutClose", () => _native.Close(_device), errors))
        {
            _device = IntPtr.Zero;
        }

        if (_device == IntPtr.Zero && _callbackReferenceHeld)
        {
            _callbackReferenceHeld = false;
            _callbackHandle!.DangerousRelease();
        }
    }

    private bool TryCleanup(string operation, Func<uint> call, List<Exception> errors)
    {
        try
        {
            Check(operation, call());
            return true;
        }
        catch (Exception error)
        {
            // Continue releasing independent resources, but report every failure to the caller.
            errors.Add(error);
            return false;
        }
    }

    private void Check(string operation, uint result)
    {
        if (result == 0)
        {
            return;
        }

        string details;
        Exception? textError = null;
        try
        {
            details = _native.GetErrorText(result);
        }
        catch (Exception error)
        {
            details = "Retrieving the WinMM error text also failed";
            textError = error;
        }

        throw new InvalidOperationException(
            $"{operation} failed with MMRESULT {result} ({details}).",
            textError);
    }

    private static void ThrowCleanupErrors(List<Exception> errors)
    {
        if (errors.Count == 1)
        {
            ExceptionDispatchInfo.Capture(errors[0]).Throw();
        }

        if (errors.Count > 1)
        {
            throw new AggregateException("Releasing WinMM output resources failed.", errors);
        }
    }

    private sealed class BufferSlot
    {
        /// <summary>
        ///  Stores the stable native header address, including during partial initialization.
        /// </summary>
        internal IntPtr Header;

        /// <summary>
        ///  Stores the separately owned native PCM address.
        /// </summary>
        internal IntPtr Data;

        /// <summary>
        ///  Tracks successful preparation until WinMM successfully unprepares the header.
        /// </summary>
        internal bool Prepared;

        /// <summary>
        ///  Tracks successful submission until a writer observes WHDR_DONE.
        /// </summary>
        internal bool Queued;

        /// <summary>
        ///  Stores the exclusive stream end of the most recent successful submission.
        /// </summary>
        internal long EndFrame;
    }
}
