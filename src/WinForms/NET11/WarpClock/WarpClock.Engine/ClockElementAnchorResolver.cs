using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Resolves where an element currently sits on the canvas: the theme's layout anchor
///  (or the engine default when the theme declines the element), shifted by the
///  animator's live <see cref="ClockElementParameters.AnchorOffset"/> and orbited by
///  the theme's face rotation.
/// </summary>
/// <remarks>
///  Hands are excluded from the face-rotation orbit because they pivot at their own
///  anchor; rotating them about the dial center would move the pivot itself.
/// </remarks>
internal static class ClockElementAnchorResolver
{
    public static PointF Resolve(
        ClockElementId id,
        ClockGeometry geometry,
        IClockLayout? themeLayout,
        PointF anchorOffset,
        float faceRotationDegrees)
    {
        PointF anchor;

        if (themeLayout is not null && themeLayout.TryGetAnchor(id, geometry.Surface, out anchor))
        {
            anchor = new PointF(anchor.X + geometry.Origin.X, anchor.Y + geometry.Origin.Y);
        }
        else
        {
            anchor = DefaultClockLayout.ResolveAnchor(id, geometry);
        }

        float scale = geometry.DesignScale;
        anchor = new PointF(anchor.X + anchorOffset.X * scale, anchor.Y + anchorOffset.Y * scale);

        if (faceRotationDegrees != 0f && !IsHand(id.Kind))
        {
            anchor = ClockMath.RotateAbout(anchor, geometry.Center, faceRotationDegrees);
        }

        return anchor;
    }

    private static bool IsHand(ClockElementKind kind)
        => kind is ClockElementKind.HourHand
            or ClockElementKind.MinuteHand
            or ClockElementKind.SecondHand
            or ClockElementKind.SubSecondHand;
}
