using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  The engine's default radial dial layout: hour markers and minute ticks on
///  concentric circles, hands pivoting at the center, drums in the lower quadrants.
///  Used for any element a theme's own layout does not relocate.
/// </summary>
public sealed class DefaultClockLayout : IClockLayout
{
    private const float HourMarkerRadius = 0.78f;
    private const float MinuteTickRadius = 0.93f;
    private const float DrumRadius = 0.45f;
    private const float SubSecondRadius = 0.50f;

    /// <inheritdoc/>
    public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
    {
        ClockGeometry geometry = ClockGeometry.ForSurface(surface);
        anchor = ResolveAnchor(id, geometry);
        return true;
    }

    /// <summary>Resolves the default anchor for an element in the given geometry.</summary>
    public static PointF ResolveAnchor(ClockElementId id, ClockGeometry geometry)
    {
        PointF center = geometry.Center;
        float radius = geometry.Radius;

        return id.Kind switch
        {
            ClockElementKind.HourMarker
                => ClockMath.PointOnDial(center, radius * HourMarkerRadius, NormalizeIndex(id.Index, 12) * 30f),

            ClockElementKind.MinuteTick
                => ClockMath.PointOnDial(center, radius * MinuteTickRadius, NormalizeIndex(id.Index, 60) * 6f),

            ClockElementKind.DateDrum
                => ClockMath.PointOnDial(center, radius * DrumRadius, 90f),

            ClockElementKind.AmPmDrum
                => ClockMath.PointOnDial(center, radius * DrumRadius, 270f),

            ClockElementKind.SubSecondHand
                => ClockMath.PointOnDial(center, radius * SubSecondRadius, 180f),

            // Background, Face, Case, Arbour, and the main hands pivot at the center.
            _ => center,
        };
    }

    private static int NormalizeIndex(int index, int count)
    {
        index %= count;
        return index < 0 ? index + count : index;
    }
}
