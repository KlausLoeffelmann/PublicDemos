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
        Format = format == default ? new AudioFormat() : format;
        bufferCount = Math.Clamp(bufferCount, 3, 16);
        FramesPerBuffer = Math.Max(64, Format.SampleRate * Math.Clamp(bufferMilliseconds, 5, 200) / 1000);

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

        Check(WaveOutOpen(
            out _device,
            WaveMapper,
            in wfx,
            _bufferReturned.SafeWaitHandle.DangerousGetHandle(),
            IntPtr.Zero,
            CallbackEvent));

        uint bytes = (uint)(FramesPerBuffer * Format.BlockAlign);
        _headers = new WaveHdr*[bufferCount];
        _queued = new bool[bufferCount];

        for (int i = 0; i < bufferCount; i++)
        {
            WaveHdr* header = (WaveHdr*)NativeMemory.AllocZeroed((nuint)sizeof(WaveHdr));
            header->Data = (IntPtr)NativeMemory.AllocZeroed(bytes);
            header->BufferLength = bytes;
            Check(WaveOutPrepareHeader(_device, header, (uint)sizeof(WaveHdr)));
            _headers[i] = header;
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
        WaveHdr* header = _headers[index];

        pcm.CopyTo(new Span<short>((void*)header->Data, expected));
        header->Flags &= ~WhdrDone;
        _queued[index] = true;

        Check(WaveOutWrite(_device, header, (uint)sizeof(WaveHdr)));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_device != IntPtr.Zero)
        {
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

        _bufferReturned.Dispose();
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
            _bufferReturned.WaitOne(100);
        }
    }

    private static void Check(uint result)
    {
        if (result != 0)
        {
            throw new InvalidOperationException($"waveOut call failed with MMRESULT {result}.");
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
}
