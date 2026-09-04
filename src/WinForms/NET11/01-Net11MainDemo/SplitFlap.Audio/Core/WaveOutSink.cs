using System.Runtime.InteropServices;

namespace SplitFlap.Audio.Core;

/// <summary>
///  The oldest audio API Windows has (winmm, 1991) and still the least ceremony: open a device,
///  prepare a few buffers, hand them over, wait for the event that says one came back. ~60 ms latency
///  with four 20 ms buffers; fine for a departure board, not for playing keys. That's what
///  <see cref="IAudioSink"/> is for: this file is one implementation, not the design.
/// </summary>
public sealed unsafe partial class WaveOutSink : IAudioSink
{
    private const uint WaveMapper = 0xFFFFFFFF;
    private const uint CallbackEvent = 0x0005_0000;
    private const uint WhdrDone = 0x0000_0001;

    private readonly AutoResetEvent _bufferReturned = new(false);
    private readonly ManualResetEvent _disposeRequested = new(false);
    private readonly Lock _nativeSync = new();
    private readonly WaveHdr*[] _headers;
    private readonly bool[] _queued;
    private IntPtr _device;
    private bool _disposed;

    /// <summary>
    ///  Opens the default output device.
    /// </summary>
    /// <param name="format">Sample rate and channels.</param>
    /// <param name="bufferMilliseconds">Block size; smaller is lower latency but more fragile.</param>
    /// <param name="bufferCount">Blocks in flight; three is the minimum for glitch-free playback.</param>
    public WaveOutSink(AudioFormat format = default, int bufferMilliseconds = 20, int bufferCount = 4)
    {
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
        _headers = new WaveHdr*[bufferCount];
        _queued = new bool[bufferCount];

        // WAVEFORMATEX describes the byte layout consumed by WinMM. In particular, its
        // block/byte rates must agree with the PCM buffers or waveOutOpen rejects the format.
        WaveFormatEx wfx = new()
        {
            FormatTag = 1,
            Channels = (ushort)Format.Channels,
            SamplesPerSecond = (uint)Format.SampleRate,
            BitsPerSample = 16,
            BlockAlign = (ushort)Format.BlockAlign,
            AverageBytesPerSecond = (uint)Format.BytesPerSecond,
            ExtraSize = 0
        };

        try
        {
            Check(
                "waveOutOpen",
                WaveOutOpen(
                    out _device,
                    WaveMapper,
                    in wfx,
                    _bufferReturned.SafeWaitHandle.DangerousGetHandle(),
                    IntPtr.Zero,
                    CallbackEvent));

            uint bytes = (uint)(FramesPerBuffer * Format.BlockAlign);

            for (int i = 0; i < bufferCount; i++)
            {
                WaveHdr* header = (WaveHdr*)NativeMemory.AllocZeroed((nuint)sizeof(WaveHdr));
                _headers[i] = header;
                header->Data = (IntPtr)NativeMemory.AllocZeroed(bytes);
                header->BufferLength = bytes;
                Check(
                    "waveOutPrepareHeader",
                    WaveOutPrepareHeader(_device, header, (uint)sizeof(WaveHdr)));
            }
        }
        catch
        {
            ReleaseNativeResources();
            _disposeRequested.Dispose();
            _bufferReturned.Dispose();
            throw;
        }
    }

    /// <inheritdoc/>
    public AudioFormat Format { get; }

    /// <inheritdoc/>
    public int FramesPerBuffer { get; }

    /// <inheritdoc/>
    public void Write(ReadOnlySpan<short> pcm)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int expected = FramesPerBuffer * Format.Channels;

        if (pcm.Length != expected)
        {
            throw new ArgumentException($"Expected {expected} samples, got {pcm.Length}.", nameof(pcm));
        }

        int index = AcquireFreeBuffer();

        lock (_nativeSync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            WaveHdr* header = _headers[index];

            // Copy managed PCM into the unmanaged block that remains pinned by ownership rather
            // than by a GC handle. WinMM owns this block until it sets WHDR_DONE.
            pcm.CopyTo(new Span<short>((void*)header->Data, expected));
            header->Flags &= ~WhdrDone;
            _queued[index] = true;

            Check(
                "waveOutWrite",
                WaveOutWrite(_device, header, (uint)sizeof(WaveHdr)));
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _disposeRequested.Set();

        lock (_nativeSync)
        {
            ReleaseNativeResources();
        }

        _bufferReturned.Dispose();
        _disposeRequested.Dispose();
    }

    private int AcquireFreeBuffer()
    {
        while (true)
        {
            for (int i = 0; i < _headers.Length; i++)
            {
                if (!_queued[i] || (_headers[i]->Flags & WhdrDone) != 0)
                {
                    return i;
                }
            }

            // Every buffer is with the device. Sleep until one comes back (or time out and re-check).
            int signaled = WaitHandle.WaitAny([_bufferReturned, _disposeRequested], 100);

            if (signaled == 1)
            {
                throw new ObjectDisposedException(nameof(WaveOutSink));
            }
        }
    }

    private void ReleaseNativeResources()
    {
        if (_device == IntPtr.Zero)
        {
            foreach (WaveHdr* header in _headers)
            {
                if (header is not null)
                {
                    NativeMemory.Free((void*)header->Data);
                    NativeMemory.Free(header);
                }
            }

            return;
        }

        WaveOutReset(_device);

        foreach (WaveHdr* header in _headers)
        {
            if (header is null)
            {
                continue;
            }

            WaveOutUnprepareHeader(_device, header, (uint)sizeof(WaveHdr));
            NativeMemory.Free((void*)header->Data);
            NativeMemory.Free(header);
        }

        WaveOutClose(_device);
        _device = IntPtr.Zero;
    }

    private static void Check(string operation, uint result)
    {
        if (result != 0)
        {
            Span<char> buffer = stackalloc char[256];
            uint textResult = WaveOutGetErrorText(result, buffer, (uint)buffer.Length);
            int terminator = buffer.IndexOf('\0');
            string details = textResult == 0
                ? new string(buffer[..(terminator >= 0 ? terminator : buffer.Length)])
                : "Unknown WinMM error";

            throw new InvalidOperationException(
                $"{operation} failed with MMRESULT {result} ({details}).");
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    private struct WaveFormatEx
    {
        public ushort FormatTag;
        public ushort Channels;
        public uint SamplesPerSecond;
        public uint AverageBytesPerSecond;
        public ushort BlockAlign;
        public ushort BitsPerSample;
        public ushort ExtraSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WaveHdr
    {
        public IntPtr Data;
        public uint BufferLength;
        public uint BytesRecorded;
        public IntPtr User;
        public uint Flags;
        public uint Loops;
        public IntPtr Next;
        public IntPtr Reserved;
    }

    [LibraryImport("winmm.dll", EntryPoint = "waveOutOpen")]
    private static partial uint WaveOutOpen(out IntPtr device, uint deviceId, in WaveFormatEx format, IntPtr callback, IntPtr instance, uint flags);

    [LibraryImport("winmm.dll", EntryPoint = "waveOutPrepareHeader")]
    private static partial uint WaveOutPrepareHeader(IntPtr device, WaveHdr* header, uint size);

    [LibraryImport("winmm.dll", EntryPoint = "waveOutUnprepareHeader")]
    private static partial uint WaveOutUnprepareHeader(IntPtr device, WaveHdr* header, uint size);

    [LibraryImport("winmm.dll", EntryPoint = "waveOutWrite")]
    private static partial uint WaveOutWrite(IntPtr device, WaveHdr* header, uint size);

    [LibraryImport("winmm.dll", EntryPoint = "waveOutReset")]
    private static partial uint WaveOutReset(IntPtr device);

    [LibraryImport("winmm.dll", EntryPoint = "waveOutClose")]
    private static partial uint WaveOutClose(IntPtr device);

    [LibraryImport("winmm.dll", EntryPoint = "waveOutGetErrorTextW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial uint WaveOutGetErrorText(uint error, Span<char> text, uint textLength);
}
