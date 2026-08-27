using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed class StandardClockLayout(StandardClockDesign design) : IClockLayout
{
    public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
    {
        float ratio;
        float angle;

        switch (id.Kind)
        {
            case ClockElementKind.HourMarker:
                ratio = design.HourMarkerRadiusRatio;
                angle = NormalizeIndex(id.Index, 12) * 30f;
                break;
            case ClockElementKind.MinuteTick:
                ratio = design.MinuteTickRadiusRatio;
                angle = NormalizeIndex(id.Index, 60) * 6f;
                break;
            default:
                anchor = default;
                return false;
        }

        PointF center = new(surface.Width / 2f, surface.Height / 2f);
        float radius = MathF.Min(surface.Width, surface.Height) / 2f;
        float radians = angle * (MathF.PI / 180f);
        float distance = radius * ratio;
        anchor = new PointF(
            center.X + (MathF.Sin(radians) * distance),
            center.Y - (MathF.Cos(radians) * distance));
        return true;
    }

    private static int NormalizeIndex(int index, int count)
    {
        index %= count;
        return index < 0 ? index + count : index;
    }
}
