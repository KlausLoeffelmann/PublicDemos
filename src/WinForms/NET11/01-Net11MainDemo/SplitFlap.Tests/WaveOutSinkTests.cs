using Microsoft.Win32.SafeHandles;
using SplitFlap.Audio.Core;
using System.Runtime.InteropServices;

namespace SplitFlap.Tests;

/// <summary>
///  Exercises the real sink's PCM storage and lifetime rules without opening an audio endpoint.
/// </summary>
public sealed class WaveOutSinkTests
{
    private static readonly TimeSpan s_timeout = TimeSpan.FromSeconds(10);

    /// <summary>
    ///  Keeps the established 48 kHz mono, four-buffer, 20 ms defaults.
    /// </summary>
    [Fact]
    public void Constructor_UsesDefaultFormatAndBuffering()
    {
        using FakeWaveOut native = new();
        using WaveOutSink sink = new(native);

        Assert.Equal(AudioFormat.Default, sink.Format);
        Assert.Equal(960, sink.FramesPerBuffer);
        Assert.Equal(4, native.PrepareCount);
        Assert.Equal((ushort)1, native.Format.FormatTag);
        Assert.Equal((ushort)1, native.Format.Channels);
        Assert.Equal(48_000u, native.Format.SamplesPerSecond);
        Assert.Equal(96_000u, native.Format.AverageBytesPerSecond);
        Assert.Equal((ushort)2, native.Format.BlockAlign);
        Assert.Equal((ushort)16, native.Format.BitsPerSample);
        Assert.Equal((ushort)0, native.Format.ExtraSize);
    }

    /// <summary>
    ///  Validates unsupported formats before allocating a native output device.
    /// </summary>
    [Theory]
    [InlineData(0, 1)]
    [InlineData(7_999, 1)]
    [InlineData(384_001, 1)]
    [InlineData(48_000, 0)]
    [InlineData(48_000, 3)]
    public void Constructor_RejectsUnsupportedFormats(int sampleRate, int channels)
    {
        using FakeWaveOut native = new();

        ArgumentOutOfRangeException error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new WaveOutSink(native, new AudioFormat(sampleRate, channels)));

        Assert.Equal("format", error.ParamName);
        Assert.Equal(0, native.OpenCount);
    }

    /// <summary>
    ///  Preserves buffer-duration, buffer-count, and minimum-frame clamping.
    /// </summary>
    [Theory]
    [InlineData(8_000, 1, int.MinValue, int.MinValue, 64, 3)]
    [InlineData(48_000, 2, 20, 4, 960, 4)]
    [InlineData(384_000, 2, int.MaxValue, int.MaxValue, 76_800, 16)]
    public void Constructor_ClampsBufferSettings(
        int sampleRate, int channels, int milliseconds, int bufferCount, int expectedFrames, int expectedBuffers)
    {
        using FakeWaveOut native = new();
        using WaveOutSink sink = new(native, new AudioFormat(sampleRate, channels), milliseconds, bufferCount);

        Assert.Equal(expectedFrames, sink.FramesPerBuffer);
        Assert.Equal(expectedBuffers, native.PrepareCount);
        Assert.Equal(expectedFrames * channels, native.LastPcm.Length);
        Assert.Equal((uint)(sampleRate * channels * sizeof(short)), native.Format.AverageBytesPerSecond);
        Assert.Equal((ushort)(channels * sizeof(short)), native.Format.BlockAlign);
    }

    /// <summary>
    ///  Checks the native layouts independently of any installed audio driver.
    /// </summary>
    [Fact]
    public void InteropLayouts_MatchWaveFormatAndHeader()
    {
        Assert.Equal(18, Marshal.SizeOf<WaveFormatEx>());
        Assert.Equal(IntPtr.Size == 8 ? 48 : 32, Marshal.SizeOf<WaveHeader>());
        Assert.Equal(IntPtr.Size == 8 ? 24 : 16, Marshal.OffsetOf<WaveHeader>(nameof(WaveHeader.Flags)).ToInt32());
    }

    /// <summary>
    ///  Requires precisely one complete block, including both channels for stereo.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Write_RejectsIncompleteOrOversizedBlocks(int channels)
    {
        using FakeWaveOut native = new();
        using WaveOutSink sink = new(native, new AudioFormat(48_000, channels));
        int expected = sink.FramesPerBuffer * channels;

        foreach (int length in new[] { 0, expected - 1, expected + 1 })
        {
            ArgumentException error = Assert.Throws<ArgumentException>(() => sink.Write(new short[length]));
            Assert.Equal("pcm", error.ParamName);
        }

        Assert.Equal(0, native.WriteCount);
        sink.Write(new short[expected]);
        Assert.Equal(1, native.WriteCount);
    }

    /// <summary>
    ///  Copies every signed sample into stable prepared storage without rewriting native-owned fields.
    /// </summary>
    [Fact]
    public void Write_CopiesPcmAndReusesOnlyReturnedHeaders()
    {
        using FakeWaveOut native = new() { CompleteWrites = false };
        using WaveOutSink sink = new(native, new AudioFormat(48_000, 2), bufferCount: 3);
        IntPtr[] headers = native.GetHeaderAddresses();
        Assert.Equal(3, headers.Distinct().Count());

        IntPtr[] data = headers.Select(header => Marshal.PtrToStructure<WaveHeader>(header).Data).ToArray();
        Assert.Equal(3, data.Distinct().Count());

        // Model driver bookkeeping after prepare. A complete StructureToPtr on each write
        // would erase these values even if the header and PCM addresses happened to stay stable.
        int reservedOffset = Marshal.OffsetOf<WaveHeader>(nameof(WaveHeader.Reserved)).ToInt32();
        int nextOffset = Marshal.OffsetOf<WaveHeader>(nameof(WaveHeader.Next)).ToInt32();
        Marshal.WriteIntPtr(headers[1], reservedOffset, new IntPtr(123));
        Marshal.WriteIntPtr(headers[1], nextOffset, new IntPtr(456));

        short[] pcm = new short[sink.FramesPerBuffer * sink.Format.Channels];
        for (int block = 0; block < 4; block++)
        {
            for (int sample = 0; sample < pcm.Length; sample++)
            {
                pcm[sample] = unchecked((short)(sample * 37 + block * 1_001));
            }

            pcm[0] = short.MinValue;
            pcm[1] = short.MaxValue;
            if (block == 3)
            {
                native.CompleteBuffer(1);
            }

            sink.Write(pcm);
            int index = block == 3 ? 1 : block;
            Assert.Equal(headers[index], native.LastHeader);
            Assert.Equal(pcm, native.LastPcm);
            WaveHeader submitted = Marshal.PtrToStructure<WaveHeader>(headers[index]);
            Assert.Equal(data[index], submitted.Data);
            Assert.Equal((uint)(pcm.Length * sizeof(short)), submitted.BufferLength);
        }

        Assert.Equal(new IntPtr(123), Marshal.ReadIntPtr(headers[1], reservedOffset));
        Assert.Equal(new IntPtr(456), Marshal.ReadIntPtr(headers[1], nextOffset));
        Assert.Equal(3, native.PrepareCount);
    }

    /// <summary>
    ///  Leaves unsuccessful submissions reusable and reports the original native error.
    /// </summary>
    [Fact]
    public void Write_FailureDoesNotStrandTheSelectedBuffer()
    {
        using FakeWaveOut native = new();
        using WaveOutSink sink = new(native, bufferCount: 3);
        short[] pcm = new short[sink.FramesPerBuffer];

        // Reuse an already-returned buffer, not just a never-submitted buffer.
        sink.Write(pcm);
        IntPtr firstHeader = native.LastHeader;
        native.WriteResult = 17;

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => sink.Write(pcm));
        AssertNativeError(error, "waveOutWrite", 17);

        native.WriteResult = 0;
        sink.Write(pcm);
        Assert.Equal(firstHeader, native.LastHeader);
        Assert.Equal(3, native.WriteCount);
    }

    /// <summary>
    ///  Releases callback resources when opening the native device fails.
    /// </summary>
    [Fact]
    public void Constructor_OpenFailureReleasesTheCallback()
    {
        using FakeWaveOut native = new() { OpenResult = 5 };

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => new WaveOutSink(native));

        AssertNativeError(error, "waveOutOpen", 5);
        Assert.Equal(["Open"], native.Calls);
        Assert.True(native.Callback!.IsClosed);
    }

    /// <summary>
    ///  Unwinds only successfully prepared headers after partial construction.
    /// </summary>
    [Fact]
    public void Constructor_PrepareFailureResetsUnpreparesAndCloses()
    {
        using FakeWaveOut native = new() { PrepareFailureIndex = 1 };

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() => new WaveOutSink(native));

        AssertNativeError(error, "waveOutPrepareHeader", 7);
        Assert.Equal(["Open", "Prepare", "Prepare", "Reset", "Unprepare", "Close"], native.Calls);
        Assert.Equal(0, native.PreparedCount);
        Assert.True(native.Callback!.IsClosed);
    }

    /// <summary>
    ///  Preserves the construction failure as well as cleanup errors while finishing independent cleanup.
    /// </summary>
    [Fact]
    public void Constructor_ReportsOriginalAndCleanupErrors()
    {
        using FakeWaveOut native = new() { PrepareFailureIndex = 1, ResetResult = 5 };

        AggregateException error = Assert.Throws<AggregateException>(() => new WaveOutSink(native));

        Assert.Equal(2, error.InnerExceptions.Count);
        AssertNativeError(error.InnerExceptions[0], "waveOutPrepareHeader", 7);
        AssertNativeError(error.InnerExceptions[1], "waveOutReset", 5);
        Assert.Equal(1, native.CloseCount);
        Assert.Equal(0, native.PreparedCount);
        Assert.True(native.Callback!.IsClosed);
    }

    /// <summary>
    ///  Wakes a writer sleeping inside the real sink's WaitAny before freeing headers or events.
    /// </summary>
    [Fact]
    public void Dispose_WakesBlockedWrite()
    {
        using FakeWaveOut native = new() { CompleteWrites = false };
        using WaveOutSink sink = new(native, bufferCount: 3);
        short[] pcm = FillQueue(sink);
        Worker writer = new(() => sink.Write(pcm));

        try
        {
            WaitUntilBlocked(writer);
            Worker disposer = new(sink.Dispose);

            Assert.Null(disposer.Join());
            Assert.IsType<ObjectDisposedException>(writer.Join());
            Assert.Equal(0, native.PreparedCount);
            Assert.Equal(1, native.CloseCount);
            Assert.True(native.Callback!.IsClosed);
        }
        finally
        {
            // This also releases a blocked writer if an assertion detects a shutdown regression.
            native.CompleteAll();
        }
    }

    /// <summary>
    ///  Keeps teardown outside an in-flight copy and native submission.
    /// </summary>
    [Fact]
    public void Dispose_WaitsForAnInFlightNativeWrite()
    {
        using ManualResetEventSlim writeEntered = new(false);
        using ManualResetEventSlim allowWrite = new(false);
        using FakeWaveOut native = new()
        {
            CompleteWrites = false,
            BeforeWrite = () =>
            {
                writeEntered.Set();
                Assert.True(allowWrite.Wait(s_timeout));
            }
        };
        using WaveOutSink sink = new(native, bufferCount: 3);
        short[] pcm = new short[sink.FramesPerBuffer];
        Worker writer = new(() => sink.Write(pcm));

        try
        {
            Assert.True(writeEntered.Wait(s_timeout, TestContext.Current.CancellationToken));
            Worker disposer = new(sink.Dispose);
            WaitUntilBlocked(disposer);
            Assert.Equal(0, Volatile.Read(ref native.ResetCount));
            Assert.False(native.Callback!.IsClosed);

            allowWrite.Set();
            Assert.Null(writer.Join());
            Assert.Null(disposer.Join());
            Assert.Equal(1, native.ResetCount);
            Assert.Equal(0, native.PreparedCount);
        }
        finally
        {
            allowWrite.Set();
        }
    }

    /// <summary>
    ///  Serializes concurrent disposers and performs successful native teardown only once.
    /// </summary>
    [Fact]
    public void Dispose_IsRepeatedAndConcurrentSafe()
    {
        using ManualResetEventSlim resetEntered = new(false);
        using ManualResetEventSlim allowReset = new(false);
        using FakeWaveOut native = new()
        {
            BeforeReset = () =>
            {
                resetEntered.Set();
                Assert.True(allowReset.Wait(s_timeout));
            }
        };
        using WaveOutSink sink = new(native, bufferCount: 3);
        Worker first = new(sink.Dispose);

        try
        {
            Assert.True(resetEntered.Wait(s_timeout, TestContext.Current.CancellationToken));
            Worker[] others = Enumerable.Range(0, 4).Select(_ => new Worker(sink.Dispose)).ToArray();
            foreach (Worker worker in others)
            {
                WaitUntilBlocked(worker);
            }

            allowReset.Set();
            Assert.Null(first.Join());
            foreach (Worker worker in others)
            {
                Assert.Null(worker.Join());
            }

            sink.Dispose();
            Assert.Equal(1, native.ResetCount);
            Assert.Equal(3, native.UnprepareCount);
            Assert.Equal(1, native.CloseCount);
            Assert.True(native.Callback!.IsClosed);
            Assert.Throws<ObjectDisposedException>(() => sink.Write([]));
        }
        finally
        {
            allowReset.Set();
        }
    }

    /// <summary>
    ///  Keeps failed-to-unprepare storage and the callback alive until a later cleanup succeeds.
    /// </summary>
    [Fact]
    public void Dispose_UnprepareFailureRetainsOwnershipForRetry()
    {
        using FakeWaveOut native = new() { CompleteWrites = false };
        using WaveOutSink sink = new(native, bufferCount: 3);
        FillQueue(sink);
        native.UnprepareFailureIndex = 1;

        try
        {
            InvalidOperationException error = Assert.Throws<InvalidOperationException>(sink.Dispose);

            AssertNativeError(error, "waveOutUnprepareHeader", 33);
            Assert.Equal(1, native.PreparedCount);
            Assert.Equal(0, native.CloseCount);
            Assert.False(native.Callback!.IsClosed);
            native.SignalCallback();
            Assert.Throws<ObjectDisposedException>(() => sink.Write([]));
        }
        finally
        {
            native.UnprepareFailureIndex = -1;
            sink.Dispose();
        }

        Assert.Equal(4, native.UnprepareCount);
        Assert.Equal(1, native.CloseCount);
        Assert.True(native.Callback!.IsClosed);
    }

    /// <summary>
    ///  Wakes blocked writes even when reset fails, without freeing buffers still owned by WinMM.
    /// </summary>
    [Fact]
    public void Dispose_ResetFailureStillWakesWriterAndRetainsQueuedBuffers()
    {
        using FakeWaveOut native = new() { CompleteWrites = false };
        using WaveOutSink sink = new(native, bufferCount: 3);
        short[] pcm = FillQueue(sink);
        native.ResetResult = 5;
        Worker writer = new(() => sink.Write(pcm));

        try
        {
            WaitUntilBlocked(writer);
            Worker disposer = new(sink.Dispose);
            AggregateException error = Assert.IsType<AggregateException>(disposer.Join());

            Assert.IsType<ObjectDisposedException>(writer.Join());
            Assert.Equal(4, error.InnerExceptions.Count);
            AssertNativeError(error.InnerExceptions[0], "waveOutReset", 5);
            Assert.All(error.InnerExceptions.Skip(1), item => AssertNativeError(item, "waveOutUnprepareHeader", 33));
            Assert.Equal(3, native.PreparedCount);
            Assert.Equal(0, native.CloseCount);
            Assert.False(native.Callback!.IsClosed);

            // These are still genuine, allocated native headers, not a duplicate fake sink.
            native.CompleteAll();
        }
        finally
        {
            native.ResetResult = 0;
            native.CompleteAll();
            sink.Dispose();
        }

        Assert.Equal(0, native.PreparedCount);
        Assert.True(native.Callback!.IsClosed);
    }

    /// <summary>
    ///  Retains the callback handle while a failed close can still leave a native device open.
    /// </summary>
    [Fact]
    public void Dispose_CloseFailureCanBeRetried()
    {
        using FakeWaveOut native = new() { CloseResult = 5 };
        using WaveOutSink sink = new(native, bufferCount: 3);

        try
        {
            InvalidOperationException error = Assert.Throws<InvalidOperationException>(sink.Dispose);

            AssertNativeError(error, "waveOutClose", 5);
            Assert.Equal(0, native.PreparedCount);
            Assert.False(native.Callback!.IsClosed);
            native.SignalCallback();
        }
        finally
        {
            native.CloseResult = 0;
            sink.Dispose();
        }

        Assert.Equal(
            ["Open", "Prepare", "Prepare", "Prepare", "Reset", "Unprepare", "Unprepare", "Unprepare", "Close", "Reset", "Close"],
            native.Calls);
        Assert.True(native.Callback!.IsClosed);
    }

    /// <summary>
    ///  Allocates neither a staging array nor boxed native headers on steady successful writes.
    /// </summary>
    [Fact]
    public void Write_SteadyStateDoesNotAllocate()
    {
        using FakeWaveOut native = new();
        using WaveOutSink sink = new(native);
        short[] pcm = new short[sink.FramesPerBuffer];
        for (int i = 0; i < 256; i++)
        {
            sink.Write(pcm);
        }

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1_024; i++)
        {
            sink.Write(pcm);
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
    }

    /// <summary>
    ///  Reuses the wait-handle array while a native-style consumer returns one buffer per write.
    /// </summary>
    [Fact]
    public void Write_SteadyStateWaitsDoNotAllocate()
    {
        const int warmup = 32;
        const int measured = 64;
        using AutoResetEvent writeQueued = new(false);
        using FakeWaveOut native = new() { CompleteWrites = false };
        using WaveOutSink sink = new(native, bufferCount: 3);
        short[] pcm = FillQueue(sink);
        native.WriteQueued = writeQueued;
        long allocated = -1;
        Worker writer = new(() =>
        {
            for (int i = 0; i < warmup; i++)
            {
                sink.Write(pcm);
            }

            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < measured; i++)
            {
                sink.Write(pcm);
            }

            allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        });

        try
        {
            for (int i = 0; i < warmup + measured; i++)
            {
                WaitUntilBlocked(writer);
                native.CompleteBuffer(0);
                Assert.True(writeQueued.WaitOne(s_timeout));
            }

            Assert.Null(writer.Join());
            Assert.Equal(0, allocated);
        }
        finally
        {
            native.CompleteAll();
        }
    }

    private static short[] FillQueue(WaveOutSink sink)
    {
        short[] pcm = new short[sink.FramesPerBuffer * sink.Format.Channels];
        for (int i = 0; i < 3; i++)
        {
            sink.Write(pcm);
        }

        return pcm;
    }

    private static void AssertNativeError(Exception error, string operation, uint result)
    {
        Assert.Contains(operation, error.Message);
        Assert.Contains($"MMRESULT {result}", error.Message);
        Assert.Contains($"Simulated native error {result}", error.Message);
    }

    private static void WaitUntilBlocked(Worker worker)
    {
        // Each worker has no unrelated waits: it is either in the sink's WaitAny or
        // waiting for its writer/disposer lock. Observe that state instead of sleeping.
        Assert.True(SpinWait.SpinUntil(
            () => (worker.Thread.ThreadState & ThreadState.WaitSleepJoin) != 0 || !worker.Thread.IsAlive,
            s_timeout));
        Assert.True(worker.Thread.IsAlive, "The operation completed instead of blocking.");
    }

    private sealed class Worker
    {
        private Exception? _error;

        /// <summary>
        ///  Starts a dedicated thread so test code can observe the sink's actual blocking state.
        /// </summary>
        internal Worker(Action action)
        {
            Thread = new(() =>
            {
                try
                {
                    action();
                }
                catch (Exception error)
                {
                    _error = error;
                }
            })
            {
                IsBackground = true
            };
            Thread.Start();
        }

        /// <summary>
        ///  Gets the worker whose only waits belong to the operation under test.
        /// </summary>
        internal Thread Thread { get; }

        /// <summary>
        ///  Joins with a timeout and returns the operation's exception to the asserting thread.
        /// </summary>
        internal Exception? Join()
        {
            Assert.True(Thread.Join(s_timeout), "The sink operation did not finish.");
            return _error;
        }
    }

    private sealed class FakeWaveOut : IWaveOutApi, IDisposable
    {
        private const int Done = 1;
        private const int Prepared = 2;
        private const int InQueue = 16;
        private static readonly int s_headerSize = Marshal.SizeOf<WaveHeader>();
        private static readonly int s_flagsOffset = Marshal.OffsetOf<WaveHeader>(nameof(WaveHeader.Flags)).ToInt32();
        private readonly Lock _sync = new();
        private readonly List<NativeBuffer> _buffers = new(16);
        private EventWaitHandle? _returned;

        /// <summary>
        ///  Configures errors returned by individual native operations.
        /// </summary>
        internal uint OpenResult, WriteResult, ResetResult, CloseResult;

        /// <summary>
        ///  Selects the prepare call that fails, or minus one for successful preparation.
        /// </summary>
        internal int PrepareFailureIndex = -1;

        /// <summary>
        ///  Selects the prepared buffer that refuses unprepare, or minus one for normal cleanup.
        /// </summary>
        internal int UnprepareFailureIndex = -1;

        /// <summary>
        ///  Selects immediate completion rather than retaining submitted buffers.
        /// </summary>
        internal bool CompleteWrites = true;

        /// <summary>
        ///  Counts native calls without allocating in the submission hot path.
        /// </summary>
        internal int OpenCount, PrepareCount, WriteCount, ResetCount, UnprepareCount, CloseCount;

        /// <summary>
        ///  Provides gates for in-flight write and reset tests.
        /// </summary>
        internal Action? BeforeWrite, BeforeReset;

        /// <summary>
        ///  Signals that a writer has submitted a replacement buffer.
        /// </summary>
        internal AutoResetEvent? WriteQueued;

        /// <summary>
        ///  Records initialization and cleanup ordering, without per-write logging allocations.
        /// </summary>
        internal List<string> Calls { get; } = [];

        /// <summary>
        ///  Captures the format passed to native open.
        /// </summary>
        internal WaveFormatEx Format;

        /// <summary>
        ///  Captures the actual callback handle to verify its retained native lifetime.
        /// </summary>
        internal SafeWaitHandle? Callback;

        /// <summary>
        ///  Reuses storage for the samples observed through the real native PCM address.
        /// </summary>
        internal short[] LastPcm = [];

        /// <summary>
        ///  Records the persistent header used by the latest submission attempt.
        /// </summary>
        internal IntPtr LastHeader;

        /// <summary>
        ///  Counts headers for which native ownership has not been released.
        /// </summary>
        internal int PreparedCount
        {
            get
            {
                lock (_sync)
                {
                    return _buffers.Count(buffer => buffer.Prepared);
                }
            }
        }

        /// <summary>
        ///  Captures the event and format instead of opening a real audio endpoint.
        /// </summary>
        public uint Open(out IntPtr device, in WaveFormatEx format, SafeWaitHandle callback)
        {
            OpenCount++;
            Calls.Add("Open");
            Format = format;
            Callback = callback;
            device = OpenResult == 0 ? new IntPtr(1) : IntPtr.Zero;
            if (OpenResult == 0)
            {
                // Borrow rather than own the callback, just as WinMM does. Dispose the
                // wrapper's initial handle so replacing it does not leak a test-created event.
                _returned = new EventWaitHandle(false, EventResetMode.AutoReset);
                SafeWaitHandle initialHandle = _returned.SafeWaitHandle;
                _returned.SafeWaitHandle = new SafeWaitHandle(callback.DangerousGetHandle(), ownsHandle: false);
                initialHandle.Dispose();
            }

            return OpenResult;
        }

        /// <summary>
        ///  Verifies a fully initialized header and PCM block before acquiring native ownership.
        /// </summary>
        public uint PrepareHeader(IntPtr device, IntPtr header, uint size)
        {
            Assert.Equal((uint)s_headerSize, size);
            WaveHeader value = Marshal.PtrToStructure<WaveHeader>(header);
            Assert.NotEqual(IntPtr.Zero, value.Data);
            Assert.Equal(0u, value.Flags);
            Assert.Equal(0u, value.BytesRecorded);
            Assert.Equal(IntPtr.Zero, value.User);
            Assert.Equal(0u, value.Loops);
            Assert.Equal(IntPtr.Zero, value.Next);
            Assert.Equal(IntPtr.Zero, value.Reserved);
            short[] samples = new short[value.BufferLength / sizeof(short)];
            Marshal.Copy(value.Data, samples, 0, samples.Length);
            Assert.False(samples.AsSpan().ContainsAnyExcept((short)0));
            LastPcm = samples;

            NativeBuffer buffer = new(header, value.Data);
            _buffers.Add(buffer);
            Calls.Add("Prepare");
            int index = PrepareCount++;
            if (index == PrepareFailureIndex)
            {
                return 7;
            }

            buffer.Prepared = true;
            Marshal.WriteInt32(header, s_flagsOffset, Prepared);
            return 0;
        }

        /// <summary>
        ///  Reads from the real native PCM buffer without allocating during successful writes.
        /// </summary>
        public uint Write(IntPtr device, IntPtr header, uint size)
        {
            lock (_sync)
            {
                NativeBuffer buffer = FindBuffer(header);
                if (!buffer.Prepared || buffer.Queued || size != s_headerSize ||
                    Marshal.ReadIntPtr(header) != buffer.Data ||
                    (Marshal.ReadInt32(header, s_flagsOffset) & Prepared) == 0)
                {
                    throw new InvalidOperationException("A native buffer was reused or changed while owned by the device.");
                }

                BeforeWrite?.Invoke();
                WriteCount++;
                LastHeader = header;
                Marshal.Copy(buffer.Data, LastPcm, 0, LastPcm.Length);
                if (WriteResult != 0)
                {
                    return WriteResult;
                }

                buffer.Queued = !CompleteWrites;
                int flags = Marshal.ReadInt32(header, s_flagsOffset);
                flags = CompleteWrites ? (flags | Done) & ~InQueue : (flags | InQueue) & ~Done;
                Marshal.WriteInt32(header, s_flagsOffset, flags);
                WriteQueued?.Set();
                return 0;
            }
        }

        /// <summary>
        ///  Returns queued buffers only when the configured native reset succeeds.
        /// </summary>
        public uint Reset(IntPtr device)
        {
            Interlocked.Increment(ref ResetCount);
            BeforeReset?.Invoke();
            lock (_sync)
            {
                Calls.Add("Reset");
                if (ResetResult != 0)
                {
                    return ResetResult;
                }

                CompleteAll();
                return 0;
            }
        }

        /// <summary>
        ///  Refuses to release queued headers and verifies their storage before returning ownership.
        /// </summary>
        public uint UnprepareHeader(IntPtr device, IntPtr header, uint size)
        {
            lock (_sync)
            {
                UnprepareCount++;
                Calls.Add("Unprepare");
                NativeBuffer buffer = FindBuffer(header);
                Assert.True(buffer.Prepared);
                Assert.Equal((uint)s_headerSize, size);
                Assert.Equal(buffer.Data, Marshal.ReadIntPtr(header));
                if (buffer.Queued || _buffers.IndexOf(buffer) == UnprepareFailureIndex)
                {
                    return 33;
                }

                buffer.Prepared = false;
                int flags = Marshal.ReadInt32(header, s_flagsOffset);
                Marshal.WriteInt32(header, s_flagsOffset, flags & ~Prepared);
                return 0;
            }
        }

        /// <summary>
        ///  Checks that all headers were unprepared before any close attempt.
        /// </summary>
        public uint Close(IntPtr device)
        {
            CloseCount++;
            Calls.Add("Close");
            Assert.Equal(0, PreparedCount);
            return CloseResult;
        }

        /// <summary>
        ///  Supplies deterministic native-style diagnostic text.
        /// </summary>
        public string GetErrorText(uint result)
            => $"Simulated native error {result}";

        /// <summary>
        ///  Snapshots the real native header addresses outside the allocation-sensitive path.
        /// </summary>
        internal IntPtr[] GetHeaderAddresses()
            => _buffers.Select(buffer => buffer.Header).ToArray();

        /// <summary>
        ///  Returns a particular submitted buffer and signals the sink's actual wait handle.
        /// </summary>
        internal void CompleteBuffer(int index)
        {
            lock (_sync)
            {
                NativeBuffer buffer = _buffers[index];
                Assert.True(buffer.Prepared);
                Assert.True(buffer.Queued);
                Complete(buffer);
                SignalCallback();
            }
        }

        /// <summary>
        ///  Returns remaining buffers, including when unwinding a failed threading assertion.
        /// </summary>
        internal void CompleteAll()
        {
            lock (_sync)
            {
                bool returned = false;
                foreach (NativeBuffer buffer in _buffers)
                {
                    if (buffer.Prepared && buffer.Queued)
                    {
                        Complete(buffer);
                        returned = true;
                    }
                }

                if (returned)
                {
                    SignalCallback();
                }
            }
        }

        /// <summary>
        ///  Signals the borrowed callback even after managed disposal if native close is pending.
        /// </summary>
        internal void SignalCallback()
            => _returned!.Set();

        /// <summary>
        ///  Releases only the fake's non-owning event wrapper.
        /// </summary>
        public void Dispose()
            => _returned?.Dispose();

        private NativeBuffer FindBuffer(IntPtr header)
        {
            foreach (NativeBuffer buffer in _buffers)
            {
                if (buffer.Header == header)
                {
                    return buffer;
                }
            }

            throw new InvalidOperationException("An unprepared header was passed to the device.");
        }

        private static void Complete(NativeBuffer buffer)
        {
            buffer.Queued = false;
            int flags = Marshal.ReadInt32(buffer.Header, s_flagsOffset);
            Marshal.WriteInt32(buffer.Header, s_flagsOffset, (flags | Done) & ~InQueue);
        }

        private sealed class NativeBuffer(IntPtr header, IntPtr data)
        {
            /// <summary>
            ///  Retains the addresses originally supplied during native preparation.
            /// </summary>
            internal readonly IntPtr Header = header, Data = data;

            /// <summary>
            ///  Models ownership transitions that the native API would enforce.
            /// </summary>
            internal bool Prepared, Queued;
        }
    }
}
