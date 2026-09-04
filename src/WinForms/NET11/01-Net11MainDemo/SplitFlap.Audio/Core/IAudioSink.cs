namespace SplitFlap.Audio.Core;

/// <summary>
///  PCM format of an audio stream: how many numbers per second, and how many channels they interleave.
/// </summary>
/// <param name="SampleRate">Samples per second per channel. 48 000 is what Windows mixes at.</param>
/// <param name="Channels">1 for mono, 2 for stereo. The engine is mono; the sink may duplicate.</param>
public readonly record struct AudioFormat(int SampleRate = 48_000, int Channels = 1)
{
    /// <summary>
    ///  Gets the standard mono, 48 kHz engine format.
    /// </summary>
    public static AudioFormat Default { get; } = new(48_000, 1);

    /// <summary>
    ///  Gets the bytes per interleaved frame for 16-bit PCM.
    /// </summary>
    public int BlockAlign
        => Channels * 2;

    /// <summary>
    ///  Gets the bytes per second for 16-bit PCM.
    /// </summary>
    public int BytesPerSecond
        => SampleRate * BlockAlign;
}

/// <summary>
///  Where the numbers go. The engine produces 16-bit PCM in fixed-size blocks and hands them to a sink;
///  the sink blocks until the device has room. That blocking is the clock of the whole engine.
/// </summary>
/// <remarks>
///  Three members. Everything device-specific (waveOut headers, WASAPI COM, a WAV file for tests)
///  lives behind this interface, so swapping the backend is one line.
/// </remarks>
public interface IAudioSink : IDisposable
{
    /// <summary>
    ///  The format this sink was opened with.
    /// </summary>
    AudioFormat Format { get; }

    /// <summary>
    ///  The block size the sink expects per <see cref="Write"/>, in frames.
    /// </summary>
    int FramesPerBuffer { get; }

    /// <summary>
    ///  Queues one block for playback. Blocks until a device buffer is free.
    /// </summary>
    /// <param name="pcm">Exactly <see cref="FramesPerBuffer"/> x <see cref="AudioFormat.Channels"/> samples.</param>
    void Write(ReadOnlySpan<short> pcm);
}

/// <summary>
///  A voice is anything that produces the next sample when asked. Tones, noise, samples, drums:
///  all the same contract, all mixed the same way.
/// </summary>
public interface IVoice
{
    /// <summary>
    ///  <see langword="true"/> once the voice has nothing more to say and can be dropped.
    /// </summary>
    bool IsFinished { get; }

    /// <summary>
    ///  Produces the next sample in -1..1. Called on the engine thread only.
    /// </summary>
    float Next();

    /// <summary>
    ///  Asks the voice to end gracefully (start its release). May be called from any thread.
    /// </summary>
    void Release();
}
