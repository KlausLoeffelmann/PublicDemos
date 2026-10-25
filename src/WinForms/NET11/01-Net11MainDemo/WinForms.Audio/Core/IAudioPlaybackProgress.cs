namespace WinForms.Audio.Core;

/// <summary>
///  Optional device-clock information for displays that must follow consumed, not just queued, audio.
/// </summary>
public interface IAudioPlaybackProgress
{
    /// <summary>
    ///  Gets the cached number of frames in completely played buffers since this sink was opened.
    /// </summary>
    /// <remarks>
    ///  Reads must be nonblocking and must not inspect native memory. The cursor is monotonic;
    ///  buffers returned by reset or discarded during disposal do not count as played.
    /// </remarks>
    long CompletedFrames { get; }

    /// <summary>
    ///  Gets the maximum number of frames that the sink can queue at once.
    /// </summary>
    int BufferCapacityFrames { get; }
}
