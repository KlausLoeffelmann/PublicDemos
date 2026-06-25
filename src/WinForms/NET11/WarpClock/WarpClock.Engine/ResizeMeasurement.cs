using System.Drawing;

namespace WarpClock.Engine;

/// <summary>
///  Aggregate timing for one resize interaction, raised by
///  <see cref="WarpClockControl.ResizeMeasured"/> once a burst of size changes settles.
///  Built from <see cref="System.Diagnostics.Stopwatch"/> spans accumulated while the
///  surface was being resized, so it quantifies the resize sluggishness directly instead
///  of relying on a visual read-out.
/// </summary>
/// <param name="Duration">Wall-clock time from the first to the last size change of the burst.</param>
/// <param name="SizeChanges">How many <c>OnSizeChanged</c> events the burst produced.</param>
/// <param name="Frames">How many frames were actually rendered during the burst.</param>
/// <param name="AvgFrameMs">Average total frame time across the burst.</param>
/// <param name="MaxFrameMs">Worst single frame time in the burst.</param>
/// <param name="AvgUpdateMs">Average time spent positioning and re-rasterizing elements.</param>
/// <param name="MaxUpdateMs">Worst element-update time in the burst.</param>
/// <param name="AvgCommitMs">Average time the UI thread blocked on the visual commit.</param>
/// <param name="MaxCommitMs">Worst commit block in the burst.</param>
/// <param name="AvgRedraws">Average number of elements re-rasterized per frame.</param>
/// <param name="FinalSurface">The surface size the burst ended at.</param>
public readonly record struct ResizeMeasurement(
    TimeSpan Duration,
    int SizeChanges,
    int Frames,
    double AvgFrameMs,
    double MaxFrameMs,
    double AvgUpdateMs,
    double MaxUpdateMs,
    double AvgCommitMs,
    double MaxCommitMs,
    double AvgRedraws,
    SizeF FinalSurface)
{
    /// <summary>
    ///  Frames rendered per size change. Below 1.0 means the surface could not keep up —
    ///  several <c>WM_SIZE</c> messages were coalesced into a single rendered frame, which
    ///  is exactly the "visuals lag behind the new dimensions" effect.
    /// </summary>
    public double FramesPerSizeChange => SizeChanges > 0 ? (double)Frames / SizeChanges : 0d;

    /// <summary>A compact one-line summary suitable for a status bar.</summary>
    public override string ToString()
        => $"Last resize: {Duration.TotalSeconds:0.0}s · {SizeChanges} size-changes · " +
           $"{Frames} frames ({FramesPerSizeChange:0.00} frame/size) · " +
           $"frame avg {AvgFrameMs:0.0}/max {MaxFrameMs:0.0} ms · " +
           $"update avg {AvgUpdateMs:0.0} ms · commit avg {AvgCommitMs:0.0}/max {MaxCommitMs:0.0} ms · " +
           $"redraw avg {AvgRedraws:0}/frame · {FinalSurface.Width:0}×{FinalSurface.Height:0}";
}
