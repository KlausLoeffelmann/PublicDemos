using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.Controls;

namespace WarpClock.Engine;

/// <summary>
///  Per-element runtime state the engine keeps for each materialized clock element:
///  its descriptor, its mutable parameters, the backing visual, and content-cache
///  bookkeeping.
/// </summary>
internal sealed class ElementRuntime
{
    public required ClockElementDescriptor Descriptor { get; init; }

    public ClockElementParameters Parameters { get; } = new();

    public D2DVisual? Visual { get; set; }

    /// <summary>Whether the visual's cached content has been drawn at <see cref="ContentPixelSize"/>.</summary>
    public bool ContentDrawn { get; set; }

    /// <summary>The pixel size the content was last drawn at (re-draw when it changes).</summary>
    public Size ContentPixelSize { get; set; }

    /// <summary>The pivot in pixels within the current content size.</summary>
    public PointF PivotPixels { get; set; }

    /// <summary>The scale used to render the current content.</summary>
    public float ContentScale { get; set; } = 1f;

    /// <summary>The clock-time snapshot used when the current content is drawn.</summary>
    public ClockTimeSnapshot ContentTime { get; set; }
}
