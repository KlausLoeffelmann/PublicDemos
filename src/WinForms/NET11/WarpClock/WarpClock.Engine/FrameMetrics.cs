using System.Drawing;

namespace WarpClock.Engine;

/// <summary>
///  A snapshot of how much work one rendered frame cost. Raised by
///  <see cref="WarpClockControl.FrameMeasured"/> when
///  <see cref="WarpClockControl.DiagnosticsEnabled"/> is on, so a host can show where
///  the per-frame time goes — especially during a resize, when every element
///  re-rasterizes and the synchronous visual commit dominates.
/// </summary>
/// <param name="FrameMs">Total wall-clock time spent building this frame (animator + update + commit).</param>
/// <param name="AnimatorMs">Time spent in the theme animator tick.</param>
/// <param name="UpdateMs">Time spent positioning every visual and re-rasterizing the ones that changed.</param>
/// <param name="CommitMs">Time the UI thread blocked waiting for the DirectComposition visual commit.</param>
/// <param name="ElementCount">Number of clock element visuals in the scene.</param>
/// <param name="RedrawCount">How many of those elements were re-rasterized this frame (all of them while resizing).</param>
/// <param name="Fps">A smoothed frames-per-second estimate from the measured frame interval.</param>
/// <param name="Surface">The clock surface size this frame was built for.</param>
public readonly record struct FrameMetrics(
    double FrameMs,
    double AnimatorMs,
    double UpdateMs,
    double CommitMs,
    int ElementCount,
    int RedrawCount,
    double Fps,
    SizeF Surface)
{
    /// <summary>A compact single-line summary suitable for a status bar.</summary>
    public override string ToString()
        => $"{Fps,5:0.0} fps | frame {FrameMs,5:0.0} ms " +
           $"(anim {AnimatorMs,4:0.0} | update {UpdateMs,5:0.0} | commit {CommitMs,5:0.0}) | " +
           $"redraw {RedrawCount}/{ElementCount} | {Surface.Width:0}×{Surface.Height:0}";
}
