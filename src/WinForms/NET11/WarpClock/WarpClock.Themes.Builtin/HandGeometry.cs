using System.Drawing;

namespace WarpClock.Themes.Builtin;

/// <summary>The built-in hand silhouettes.</summary>
public enum HandStyle
{
    /// <summary>Swiss-railway rod with a lollipop disc on the second hand.</summary>
    Railway,

    /// <summary>Clean tapered triangles.</summary>
    Modern,

    /// <summary>Ornate spade / lance shapes.</summary>
    Antique,
}

/// <summary>Which target a hand tracks (drives its proportions).</summary>
public enum HandSlot
{
    /// <summary>The hour hand.</summary>
    Hour,

    /// <summary>The minute hand.</summary>
    Minute,

    /// <summary>The second hand.</summary>
    Second,
}

/// <summary>
///  A hand silhouette authored in element-local design space, <b>pointing straight up</b>
///  (toward 12) from <see cref="Pivot"/>. The engine rotates the whole visual to aim it.
/// </summary>
public sealed class HandShape
{
    /// <summary>The content box size in design units.</summary>
    public required SizeF Size { get; init; }

    /// <summary>The pivot (rotation center) in design units within <see cref="Size"/>.</summary>
    public required PointF Pivot { get; init; }

    /// <summary>Filled polygons making up the hand.</summary>
    public required IReadOnlyList<PointF[]> Polygons { get; init; }

    /// <summary>Filled discs (center, radius) — e.g. the railway lollipop.</summary>
    public IReadOnlyList<(PointF Center, float Radius)> Discs { get; init; } = [];

    /// <summary>Open (ring) discs (center, radius, stroke) — e.g. an open moon near the tip.</summary>
    public IReadOnlyList<(PointF Center, float Radius, float Stroke)> Rings { get; init; } = [];
}

/// <summary>
///  Factory for built-in hand silhouettes. Each hand is authored in a content box with
///  the tip near the top (y = 0) and the pivot lower down, pointing up.
/// </summary>
public static class HandGeometry
{
    private const float DialRadius = 500f;

    public static HandShape Build(HandStyle style, HandSlot slot)
    {
        (float length, float tail, float halfWidth) = slot switch
        {
            HandSlot.Hour => (DialRadius * 0.55f, 50f, 18f),
            HandSlot.Minute => (DialRadius * 0.80f, 60f, 13f),
            _ => (DialRadius * 0.90f, 95f, 6f),
        };

        return style switch
        {
            HandStyle.Railway => BuildRailway(slot, length, tail, halfWidth),
            HandStyle.Antique => BuildAntique(length, tail, halfWidth),
            _ => BuildModern(length, tail, halfWidth),
        };
    }

    private static (SizeF Size, PointF Pivot, float Cx, float TipY, float PivotY, float TailY) Frame(
        float length, float tail, float maxHalfWidth)
    {
        float pad = maxHalfWidth + 8f;
        float width = maxHalfWidth * 2f + 16f;
        float height = length + tail + 16f;
        float cx = width / 2f;
        float pivotY = length + 8f;
        return (new SizeF(width, height), new PointF(cx, pivotY), cx, 8f, pivotY, pivotY + tail);
    }

    private static HandShape BuildModern(float length, float tail, float halfWidth)
    {
        var f = Frame(length, tail, halfWidth);
        // Tapered triangle from a wide base at the pivot to a fine tip, plus a stub tail.
        PointF[] body =
        [
            new(f.Cx - halfWidth, f.PivotY),
            new(f.Cx - halfWidth * 0.25f, f.TipY),
            new(f.Cx + halfWidth * 0.25f, f.TipY),
            new(f.Cx + halfWidth, f.PivotY),
        ];
        PointF[] tailPoly =
        [
            new(f.Cx - halfWidth * 0.7f, f.PivotY),
            new(f.Cx + halfWidth * 0.7f, f.PivotY),
            new(f.Cx + halfWidth * 0.5f, f.TailY),
            new(f.Cx - halfWidth * 0.5f, f.TailY),
        ];
        return new HandShape { Size = f.Size, Pivot = f.Pivot, Polygons = [body, tailPoly] };
    }

    private static HandShape BuildRailway(HandSlot slot, float length, float tail, float halfWidth)
    {
        var f = Frame(length, tail, MathF.Max(halfWidth, 14f));

        if (slot == HandSlot.Second)
        {
            // Thin rod with a lollipop disc near the tip and a counterweight.
            float discR = 14f;
            float discY = f.TipY + discR + 18f;
            PointF[] rod =
            [
                new(f.Cx - 2.5f, f.TailY),
                new(f.Cx + 2.5f, f.TailY),
                new(f.Cx + 2.5f, f.TipY),
                new(f.Cx - 2.5f, f.TipY),
            ];
            return new HandShape
            {
                Size = f.Size,
                Pivot = f.Pivot,
                Polygons = [rod],
                Discs = [(new PointF(f.Cx, discY), discR), (new PointF(f.Cx, f.TailY - 10f), 9f)],
            };
        }

        // Blunt rounded bar for hour/minute.
        PointF[] bar =
        [
            new(f.Cx - halfWidth, f.TailY),
            new(f.Cx + halfWidth, f.TailY),
            new(f.Cx + halfWidth, f.TipY + halfWidth),
            new(f.Cx, f.TipY),
            new(f.Cx - halfWidth, f.TipY + halfWidth),
        ];
        return new HandShape { Size = f.Size, Pivot = f.Pivot, Polygons = [bar] };
    }

    private static HandShape BuildAntique(float length, float tail, float halfWidth)
    {
        float w = (halfWidth + 6f) * 1.65f;
        var f = Frame(length, tail, w);
        float shoulderY = f.PivotY - (f.PivotY - f.TipY) * 0.42f;
        float neckY = f.PivotY - (f.PivotY - f.TipY) * 0.70f;
        PointF[] body =
        [
            new(f.Cx - w * 0.28f, f.TailY),
            new(f.Cx + w * 0.28f, f.TailY),
            new(f.Cx + w * 0.55f, f.PivotY),
            new(f.Cx + w, shoulderY),
            new(f.Cx + w * 0.42f, neckY),
            new(f.Cx, f.TipY),
            new(f.Cx - w * 0.42f, neckY),
            new(f.Cx - w, shoulderY),
            new(f.Cx - w * 0.55f, f.PivotY),
        ];
        PointF[] rightWing =
        [
            new(f.Cx + w * 0.18f, f.PivotY - 8f),
            new(f.Cx + w * 1.15f, f.PivotY - 48f),
            new(f.Cx + w * 0.62f, f.PivotY - 88f),
            new(f.Cx + w * 0.25f, f.PivotY - 58f),
        ];
        PointF[] leftWing = rightWing
            .Select(point => new PointF((2f * f.Cx) - point.X, point.Y))
            .ToArray();
        float ringY = f.PivotY - (f.PivotY - f.TipY) * 0.50f;
        return new HandShape
        {
            Size = f.Size,
            Pivot = f.Pivot,
            Polygons = [body, rightWing, leftWing],
            Rings = [(new PointF(f.Cx, ringY), w * 0.34f, 3f)],
        };
    }
}
