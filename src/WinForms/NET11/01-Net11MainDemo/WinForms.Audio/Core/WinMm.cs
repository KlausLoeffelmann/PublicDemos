using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace WinForms.Audio.Core;

/// <summary>
///  Isolates native calls without replacing the sink's allocation, copying, or lifetime logic in tests.
/// </summary>
internal interface IWaveOutApi
{
    /// <summary>
    ///  Opens the default PCM device with a buffer-returned event.
    /// </summary>
    uint Open(out IntPtr device, in WaveFormatEx format, SafeWaitHandle callback);

    /// <summary>
    ///  Prepares a header whose address remains stable until unprepared.
    /// </summary>
    uint PrepareHeader(IntPtr device, IntPtr header, uint size);

    /// <summary>
    ///  Queues a prepared header and its PCM storage.
    /// </summary>
    uint Write(IntPtr device, IntPtr header, uint size);

    /// <summary>
    ///  Stops playback and returns pending buffers.
    /// </summary>
    uint Reset(IntPtr device);

    /// <summary>
    ///  Releases native ownership of a prepared header.
    /// </summary>
    uint UnprepareHeader(IntPtr device, IntPtr header, uint size);

    /// <summary>
    ///  Closes the device after its headers have been unprepared.
    /// </summary>
    uint Close(IntPtr device);

    /// <summary>
    ///  Retrieves WinMM's diagnostic text for an MMRESULT.
    /// </summary>
    string GetErrorText(uint result);
}

/// <summary>
///  Calls WinMM with managed declarations and persistent native addresses instead of unsafe pointers.
/// </summary>
internal sealed class WinMm : IWaveOutApi
{
    private const uint WaveMapper = 0xFFFF_FFFF;
    private const uint CallbackEvent = 0x0005_0000;

    /// <summary>
    ///  Gets the stateless native implementation shared by output sinks.
    /// </summary>
    internal static WinMm Instance { get; } = new();

    private WinMm()
    {
    }

    /// <summary>
    ///  Opens the default output device using the caller's retained callback handle.
    /// </summary>
    public uint Open(out IntPtr device, in WaveFormatEx format, SafeWaitHandle callback)
        => WaveOutOpen(out device, WaveMapper, in format, callback.DangerousGetHandle(), IntPtr.Zero, CallbackEvent);

    /// <summary>
    ///  Prepares the persistent header and PCM storage.
    /// </summary>
    public uint PrepareHeader(IntPtr device, IntPtr header, uint size)
        => WaveOutPrepareHeader(device, header, size);

    /// <summary>
    ///  Submits a prepared header without temporarily marshaling it.
    /// </summary>
    public uint Write(IntPtr device, IntPtr header, uint size)
        => WaveOutWrite(device, header, size);

    /// <summary>
    ///  Returns all pending device buffers.
    /// </summary>
    public uint Reset(IntPtr device)
        => WaveOutReset(device);

    /// <summary>
    ///  Releases WinMM's ownership of a header.
    /// </summary>
    public uint UnprepareHeader(IntPtr device, IntPtr header, uint size)
        => WaveOutUnprepareHeader(device, header, size);

    /// <summary>
    ///  Closes the output handle.
    /// </summary>
    public uint Close(IntPtr device)
        => WaveOutClose(device);

    /// <summary>
    ///  Returns native error text, or the diagnostic failure's own MMRESULT.
    /// </summary>
    public string GetErrorText(uint result)
    {
        StringBuilder text = new(256);
        uint textResult = WaveOutGetErrorText(result, text, (uint)text.Capacity);
        return textResult == 0
            ? text.ToString()
            : $"waveOutGetErrorTextW failed with MMRESULT {textResult}";
    }

    // DllImport is intentional: LibraryImport's generated stubs require unsafe compilation.
    [DllImport("winmm.dll", EntryPoint = "waveOutOpen", ExactSpelling = true)]
    private static extern uint WaveOutOpen(
        out IntPtr device, uint deviceId, in WaveFormatEx format, IntPtr callback, IntPtr instance, uint flags);

    [DllImport("winmm.dll", EntryPoint = "waveOutPrepareHeader", ExactSpelling = true)]
    private static extern uint WaveOutPrepareHeader(IntPtr device, IntPtr header, uint size);

    [DllImport("winmm.dll", EntryPoint = "waveOutWrite", ExactSpelling = true)]
    private static extern uint WaveOutWrite(IntPtr device, IntPtr header, uint size);

    [DllImport("winmm.dll", EntryPoint = "waveOutReset", ExactSpelling = true)]
    private static extern uint WaveOutReset(IntPtr device);

    [DllImport("winmm.dll", EntryPoint = "waveOutUnprepareHeader", ExactSpelling = true)]
    private static extern uint WaveOutUnprepareHeader(IntPtr device, IntPtr header, uint size);

    [DllImport("winmm.dll", EntryPoint = "waveOutClose", ExactSpelling = true)]
    private static extern uint WaveOutClose(IntPtr device);

    [DllImport("winmm.dll", EntryPoint = "waveOutGetErrorTextW", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern uint WaveOutGetErrorText(uint result, StringBuilder text, uint textLength);
}

/// <summary>
///  Describes the packed WAVEFORMATEX layout for interleaved signed 16-bit PCM.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct WaveFormatEx
{
    /// <summary>
    ///  Identifies PCM with the value one.
    /// </summary>
    public ushort FormatTag;

    /// <summary>
    ///  Specifies the number of interleaved channels.
    /// </summary>
    public ushort Channels;

    /// <summary>
    ///  Specifies frames per second.
    /// </summary>
    public uint SamplesPerSecond;

    /// <summary>
    ///  Specifies the byte rate across all channels.
    /// </summary>
    public uint AverageBytesPerSecond;

    /// <summary>
    ///  Specifies the byte count of one interleaved frame.
    /// </summary>
    public ushort BlockAlign;

    /// <summary>
    ///  Specifies bits per channel sample.
    /// </summary>
    public ushort BitsPerSample;

    /// <summary>
    ///  Specifies additional format data, unused for PCM.
    /// </summary>
    public ushort ExtraSize;
}

/// <summary>
///  Describes WAVEHDR with pointer-sized fields and the platform's native alignment.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct WaveHeader
{
    /// <summary>
    ///  Points to persistent native PCM storage.
    /// </summary>
    public IntPtr Data;

    /// <summary>
    ///  Specifies the PCM buffer length in bytes.
    /// </summary>
    public uint BufferLength;

    /// <summary>
    ///  Receives the recorded byte count for input devices.
    /// </summary>
    public uint BytesRecorded;

    /// <summary>
    ///  Reserves pointer-sized application data.
    /// </summary>
    public IntPtr User;

    /// <summary>
    ///  Contains preparation, queue, and completion flags maintained by WinMM.
    /// </summary>
    public uint Flags;

    /// <summary>
    ///  Specifies a repeat count when loop playback is requested.
    /// </summary>
    public uint Loops;

    /// <summary>
    ///  Reserves the driver's next-header link.
    /// </summary>
    public IntPtr Next;

    /// <summary>
    ///  Reserves pointer-sized driver data.
    /// </summary>
    public IntPtr Reserved;
}
