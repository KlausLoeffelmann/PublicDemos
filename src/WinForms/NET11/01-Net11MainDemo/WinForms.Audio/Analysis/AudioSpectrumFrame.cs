namespace WinForms.Audio.Analysis;

/// <summary>
///  Describes one coherent Hann-windowed spectrum of the final output stream.
/// </summary>
/// <param name="Sequence">Monotonically increasing snapshot sequence, starting at one.</param>
/// <param name="EndFrame">Exclusive absolute frame at the end of this analysis window.</param>
/// <param name="SampleRate">Frames per second; bin k represents k times SampleRate divided by FftSize Hz.</param>
/// <param name="FftSize">Number of frames in the window.</param>
/// <param name="PeakFrequency">Frequency of the strongest FFT bin, or zero for silence or DC.</param>
/// <param name="PeakLevel">Pre-clamp peak in dBFS over this window; it can exceed zero during overload.</param>
/// <param name="RmsLevel">RMS of the final, channel-averaged PCM in dBFS over this window.</param>
/// <param name="ClippedSamples">Interleaved samples clamped in this window, not a lifetime sum.</param>
/// <param name="DroppedBlocks">Cumulative visualization blocks missed by this subscription.</param>
/// <param name="IsPlaybackSynchronized">True for completed-device-buffer timing; false for submitted-stream timing.</param>
public readonly record struct AudioSpectrumFrame(
    long Sequence,
    long EndFrame,
    int SampleRate,
    int FftSize,
    float PeakFrequency,
    float PeakLevel,
    float RmsLevel,
    long ClippedSamples,
    long DroppedBlocks,
    bool IsPlaybackSynchronized);
