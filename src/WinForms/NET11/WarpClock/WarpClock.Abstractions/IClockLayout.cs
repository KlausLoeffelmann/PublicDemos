using System.Drawing;

namespace WarpClock.Abstractions;

/// <summary>
///  Supplies anchor positions for clock elements. The engine provides a default
///  radial layout; a theme overrides only the anchors it wants to relocate by
///  returning <see langword="true"/> from <see cref="TryGetAnchor"/>.
/// </summary>
public interface IClockLayout
{
    /// <summary>
    ///  Attempts to position <paramref name="id"/> within a dial of the given
    ///  <paramref name="surface"/> size (pixels). Return <see langword="false"/>
    ///  to defer to the engine's default radial placement.
    /// </summary>
    /// <param name="id">The element to place.</param>
    /// <param name="surface">The clock surface size in pixels.</param>
    /// <param name="anchor">The anchor point (pixels) the element's pivot sits on.</param>
    bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor);
}
