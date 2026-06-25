using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  Draws the elements of a <see cref="StandardClockDesign"/> into their per-element
///  Direct2D surfaces. All authoring is in design units (dial radius 500); the renderer
///  scales by <see cref="IClockRenderContext.Scale"/> to pixels.
/// </summary>
public sealed class StandardClockRenderer : IClockElementRenderer
{
    private static readonly string[] s_arabic =
        ["12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"];

    private static readonly string[] s_roman =
        ["XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI"];

    private readonly StandardClockDesign _design;

    public StandardClockRenderer(StandardClockDesign design) => _design = design;

    /// <inheritdoc/>
    public void DrawElement(ID2DGraphics graphics, IClockRenderContext context)
    {
        graphics.AntialiasMode = D2DAntialiasMode.PerPrimitive;

        switch (context.Id.Kind)
        {
            case ClockElementKind.Face:
                DrawFace(graphics, context);
                break;
            case ClockElementKind.HourMarker:
                DrawHourMarker(graphics, context);
                break;
            case ClockElementKind.MinuteTick:
                DrawMinuteTick(graphics, context);
                break;
            case ClockElementKind.HourHand:
                DrawHand(graphics, context, HandSlot.Hour, _design.HourHandColor);
                break;
            case ClockElementKind.MinuteHand:
                DrawHand(graphics, context, HandSlot.Minute, _design.MinuteHandColor);
                break;
            case ClockElementKind.SecondHand:
                DrawHand(graphics, context, HandSlot.Second, _design.SecondHandColor);
                break;
            case ClockElementKind.Arbour:
                DrawArbour(graphics, context);
                break;
        }
    }

    private void DrawFace(ID2DGraphics g, IClockRenderContext ctx)
    {
        float scale = ctx.Scale;
        float radius = 490f * scale;
        PointF c = ctx.Pivot;

        g.FillEllipse(_design.FaceColor, c.X - radius, c.Y - radius, radius * 2f, radius * 2f);

        if (_design.FaceBorderWidth > 0f)
        {
            float bw = _design.FaceBorderWidth * scale;
            float r = radius - bw;
            using var pen = new Pen(_design.FaceBorderColor, bw);
            g.DrawEllipse(pen, new RectangleF(c.X - r, c.Y - r, r * 2f, r * 2f));
        }
    }

    private void DrawHourMarker(ID2DGraphics g, IClockRenderContext ctx)
    {
        string[] labels = _design.HourCulture == HourCulture.Roman ? s_roman : s_arabic;
        int index = ((ctx.Id.Index % 12) + 12) % 12;
        string text = labels[index];

        float fontSize = ctx.ContentSize.Height * 0.6f;
        using var font = new Font(_design.FontFamily, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(_design.HourMarkerColor);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
    }

    private void DrawMinuteTick(ID2DGraphics g, IClockRenderContext ctx)
    {
        int index = ((ctx.Id.Index % 60) + 60) % 60;
        bool hourPos = index % 5 == 0;

        PointF pivot = ctx.Pivot;
        float angle = index * 6f; // radial orientation baked here

        bool prominent = _design.MinuteTickStyle == MinuteTickStyle.Prominent;
        float length = (hourPos ? 26f : 14f) * ctx.Scale;
        float width = (hourPos ? (prominent ? 5f : 3f) : (prominent ? 2.2f : 1.4f)) * ctx.Scale;

        // The tick points radially outward from the pivot; pivot sits on the dial circle,
        // so draw a short segment straddling the pivot along the baked angle.
        PointF inner = PointAt(pivot, -length, angle);
        PointF outer = PointAt(pivot, 0f, angle);

        using var pen = new Pen(_design.MinuteTickColor, width);
        g.DrawLine(pen, inner.X, inner.Y, outer.X, outer.Y);

        if (prominent && hourPos)
        {
            float dotR = 4f * ctx.Scale;
            g.FillEllipse(_design.MinuteTickColor, pivot.X - dotR, pivot.Y - dotR, dotR * 2f, dotR * 2f);
        }
    }

    private void DrawHand(ID2DGraphics g, IClockRenderContext ctx, HandSlot slot, Color color)
    {
        HandShape shape = HandGeometry.Build(_design.HandStyle, slot);
        float scale = ctx.Scale;

        using var brush = new SolidBrush(color);
        foreach (PointF[] polygon in shape.Polygons)
        {
            PointF[] scaled = new PointF[polygon.Length];
            for (int i = 0; i < polygon.Length; i++)
            {
                scaled[i] = new PointF(polygon[i].X * scale, polygon[i].Y * scale);
            }

            g.FillPolygon(brush, scaled);
        }

        foreach ((PointF center, float r) in shape.Discs)
        {
            float rr = r * scale;
            g.FillEllipse(color, center.X * scale - rr, center.Y * scale - rr, rr * 2f, rr * 2f);
        }

        foreach ((PointF center, float r, float stroke) in shape.Rings)
        {
            float rr = r * scale;
            using var pen = new Pen(color, stroke * scale);
            g.DrawEllipse(pen, new RectangleF(center.X * scale - rr, center.Y * scale - rr, rr * 2f, rr * 2f));
        }
    }

    private void DrawArbour(ID2DGraphics g, IClockRenderContext ctx)
    {
        float r = 20f * ctx.Scale;
        PointF c = ctx.Pivot;
        g.FillEllipse(_design.ArbourColor, c.X - r, c.Y - r, r * 2f, r * 2f);
        g.FillEllipse(Color.FromArgb(70, 255, 255, 255), c.X - r * 0.5f, c.Y - r * 0.7f, r * 0.9f, r * 0.7f);
    }

    private static PointF PointAt(PointF origin, float distance, float angleDegrees)
    {
        float rad = angleDegrees * (MathF.PI / 180f);
        return new PointF(
            origin.X + MathF.Sin(rad) * distance,
            origin.Y - MathF.Cos(rad) * distance);
    }
}
