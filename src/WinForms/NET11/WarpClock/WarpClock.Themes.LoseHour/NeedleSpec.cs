using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.LoseHour;

/// <summary>An up-pointing needle authored in design units, with its pivot below the tip.</summary>
internal sealed class NeedleSpec
{
    public required SizeF Size { get; init; }
    public required PointF Pivot { get; init; }
    public required float Length { get; init; }
    public required float Tail { get; init; }
    public required float HalfWidth { get; init; }

    public static NeedleSpec For(ClockHandKind hand)
    {
        (float length, float tail, float half) = hand switch
        {
            ClockHandKind.Hour => (250f, 40f, 16f),
            ClockHandKind.Minute => (380f, 50f, 11f),
            _ => (430f, 80f, 5f),
        };

        float width = half * 2f + 16f;
        float height = length + tail + 16f;
        return new NeedleSpec
        {
            Size = new SizeF(width, height),
            Pivot = new PointF(width / 2f, length + 8f),
            Length = length,
            Tail = tail,
            HalfWidth = half,
        };
    }

    /// <summary>Returns the needle polygon scaled to pixels.</summary>
    public PointF[] BuildPolygon(float scale)
    {
        float cx = Size.Width / 2f;
        float tipY = 8f;
        float pivotY = Length + 8f;
        float tailY = pivotY + Tail;

        return
        [
            new PointF(cx * scale, tipY * scale),
            new PointF((cx + HalfWidth) * scale, pivotY * scale),
            new PointF((cx + HalfWidth * 0.6f) * scale, tailY * scale),
            new PointF((cx - HalfWidth * 0.6f) * scale, tailY * scale),
            new PointF((cx - HalfWidth) * scale, pivotY * scale),
        ];
    }
}
