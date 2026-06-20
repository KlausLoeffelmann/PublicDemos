using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  A radial layout that defers every element to the engine's default dial placement.
///  Built-in themes are classic round clocks, so they need no custom anchors.
/// </summary>
public sealed class RadialLayout : IClockLayout
{
    /// <inheritdoc/>
    public bool TryGetAnchor(ClockElementId id, System.Drawing.SizeF surface, out System.Drawing.PointF anchor)
    {
        anchor = default;
        return false;
    }
}
