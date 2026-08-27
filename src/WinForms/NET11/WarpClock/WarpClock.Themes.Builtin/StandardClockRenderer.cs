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
            case ClockElementKind.Case:
                DrawCase(graphics, context);
                break;
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
        float radius = _design.FaceRadius * scale;
        PointF c = ctx.Pivot;

        g.FillEllipse(_design.FaceColor, c.X - radius, c.Y - radius, radius * 2f, radius * 2f);

        if (_design.Ornate)
        {
            DrawOrnateFace(g, c, radius, scale);
        }

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

        using var font = CreateHourMarkerFont(g, text, ctx.ContentSize);
        using var brush = new SolidBrush(_design.HourMarkerColor);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
        };

        RectangleF bounds = new(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height);
        if (_design.Ornate)
        {
            using var shadow = new SolidBrush(Color.FromArgb(90, _design.OrnamentColor));
            g.DrawString(text, font, shadow, new RectangleF(2f, 3f, bounds.Width, bounds.Height), format);
        }

        g.DrawString(text, font, brush, bounds, format);
    }

    private Font CreateHourMarkerFont(ID2DGraphics g, string text, SizeF contentSize)
    {
        float fontSize = contentSize.Height * _design.HourMarkerFontScale;
        var font = new Font(
            _design.FontFamily,
            fontSize,
            _design.FontStyle,
            GraphicsUnit.Pixel);
        SizeF measured = g.MeasureString(text, font);
        float widthScale = measured.Width > 0f
            ? (contentSize.Width * 0.84f) / measured.Width
            : 1f;
        float heightScale = measured.Height > 0f
            ? (contentSize.Height * 0.84f) / measured.Height
            : 1f;
        float fitScale = Math.Min(1f, Math.Min(widthScale, heightScale));

        if (fitScale >= 0.999f)
        {
            return font;
        }

        font.Dispose();
        return new Font(
            _design.FontFamily,
            MathF.Max(1f, fontSize * fitScale),
            _design.FontStyle,
            GraphicsUnit.Pixel);
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

            if (_design.Ornate)
            {
                PointF[] shadow = scaled
                    .Select(point => new PointF(point.X + (3f * scale), point.Y + (4f * scale)))
                    .ToArray();
                using var shadowBrush = new SolidBrush(Color.FromArgb(95, 20, 14, 10));
                g.FillPolygon(shadowBrush, shadow);
            }

            g.FillPolygon(brush, scaled);

            if (_design.Ornate)
            {
                using var outline = new Pen(_design.OrnamentColor, 1.8f * scale);
                for (int i = 0; i < scaled.Length; i++)
                {
                    PointF from = scaled[i];
                    PointF to = scaled[(i + 1) % scaled.Length];
                    g.DrawLine(outline, from.X, from.Y, to.X, to.Y);
                }
            }

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

    private void DrawCase(ID2DGraphics g, IClockRenderContext ctx)
    {
        float scale = ctx.Scale;
        PointF center = ctx.Pivot;
        float outerRadius = _design.CaseOuterRadius * scale;
        float middleRadius = outerRadius - (13f * scale);
        float innerRadius = outerRadius - (35f * scale);

        g.FillEllipse(
            _design.FaceBorderColor,
            center.X - outerRadius,
            center.Y - outerRadius,
            outerRadius * 2f,
            outerRadius * 2f);
        g.FillEllipse(
            _design.OrnamentColor,
            center.X - middleRadius,
            center.Y - middleRadius,
            middleRadius * 2f,
            middleRadius * 2f);
        g.FillEllipse(
            _design.FaceBorderColor,
            center.X - innerRadius,
            center.Y - innerRadius,
            innerRadius * 2f,
            innerRadius * 2f);

        using var highlight = new Pen(Color.FromArgb(150, 230, 190, 125), 3f * scale);
        using var shadow = new Pen(Color.FromArgb(150, 48, 30, 20), 4f * scale);
        float highlightRadius = outerRadius - (7f * scale);
        float shadowRadius = innerRadius + (8f * scale);
        g.DrawEllipse(
            highlight,
            new RectangleF(
                center.X - highlightRadius,
                center.Y - highlightRadius,
                highlightRadius * 2f,
                highlightRadius * 2f));
        g.DrawEllipse(
            shadow,
            new RectangleF(
                center.X - shadowRadius,
                center.Y - shadowRadius,
                shadowRadius * 2f,
                shadowRadius * 2f));

        for (int index = 0; index < 12; index++)
        {
            DrawCaseFlourish(g, center, outerRadius, index * 30f, scale);
        }

        for (int index = 0; index < 4; index++)
        {
            DrawCaseCartouche(g, center, outerRadius, index * 90f, scale);
        }
    }

    private void DrawCaseFlourish(
        ID2DGraphics g,
        PointF center,
        float outerRadius,
        float angle,
        float scale)
    {
        PointF root = PointAt(center, outerRadius - (46f * scale), angle);
        PointF left = PointAt(root, 29f * scale, angle - 62f);
        PointF right = PointAt(root, 29f * scale, angle + 62f);
        PointF leftCurl = PointAt(left, 17f * scale, angle - 120f);
        PointF rightCurl = PointAt(right, 17f * scale, angle + 120f);

        using var pen = new Pen(Color.FromArgb(220, _design.OrnamentColor), 3.2f * scale);
        g.DrawLine(pen, root.X, root.Y, left.X, left.Y);
        g.DrawLine(pen, root.X, root.Y, right.X, right.Y);
        g.DrawLine(pen, left.X, left.Y, leftCurl.X, leftCurl.Y);
        g.DrawLine(pen, right.X, right.Y, rightCurl.X, rightCurl.Y);

        float curlRadius = 7f * scale;
        g.DrawEllipse(
            pen,
            new RectangleF(
                leftCurl.X - curlRadius,
                leftCurl.Y - curlRadius,
                curlRadius * 2f,
                curlRadius * 2f));
        g.DrawEllipse(
            pen,
            new RectangleF(
                rightCurl.X - curlRadius,
                rightCurl.Y - curlRadius,
                curlRadius * 2f,
                curlRadius * 2f));
    }

    private void DrawCaseCartouche(
        ID2DGraphics g,
        PointF center,
        float outerRadius,
        float angle,
        float scale)
    {
        PointF crown = PointAt(center, outerRadius - (3f * scale), angle);
        PointF shoulder = PointAt(center, outerRadius - (31f * scale), angle);
        PointF left = PointAt(shoulder, 42f * scale, angle - 90f);
        PointF inset = PointAt(center, outerRadius - (62f * scale), angle);
        PointF right = PointAt(shoulder, 42f * scale, angle + 90f);

        using var fill = new SolidBrush(Color.FromArgb(235, _design.OrnamentColor));
        PointF[] points = [crown, left, inset, right];
        g.FillPolygon(fill, points);
        using var outline = new Pen(Color.FromArgb(190, 55, 34, 22), 3f * scale);
        for (int index = 0; index < points.Length; index++)
        {
            PointF from = points[index];
            PointF to = points[(index + 1) % points.Length];
            g.DrawLine(outline, from.X, from.Y, to.X, to.Y);
        }

        float jewelRadius = 6f * scale;
        g.FillEllipse(
            Color.FromArgb(210, 218, 177, 102),
            inset.X - jewelRadius,
            inset.Y - jewelRadius,
            jewelRadius * 2f,
            jewelRadius * 2f);
    }

    private void DrawOrnateFace(ID2DGraphics g, PointF center, float radius, float scale)
    {
        float bandOuter = radius - (32f * scale);
        float bandInner = radius - (82f * scale);
        using var ornamentPen = new Pen(_design.OrnamentColor, 8f * scale);
        using var darkPen = new Pen(_design.FaceBorderColor, 3f * scale);
        g.DrawEllipse(ornamentPen, new RectangleF(
            center.X - bandOuter,
            center.Y - bandOuter,
            bandOuter * 2f,
            bandOuter * 2f));
        g.DrawEllipse(darkPen, new RectangleF(
            center.X - bandInner,
            center.Y - bandInner,
            bandInner * 2f,
            bandInner * 2f));

        for (int index = 0; index < 12; index++)
        {
            float angle = index * 30f;
            PointF root = PointAt(center, bandInner - (10f * scale), angle);
            PointF left = PointAt(root, 28f * scale, angle - 68f);
            PointF right = PointAt(root, 28f * scale, angle + 68f);
            PointF tip = PointAt(root, 42f * scale, angle + 180f);
            using var flourish = new Pen(_design.OrnamentColor, 3f * scale);
            g.DrawLine(flourish, root.X, root.Y, left.X, left.Y);
            g.DrawLine(flourish, root.X, root.Y, right.X, right.Y);
            g.DrawLine(flourish, root.X, root.Y, tip.X, tip.Y);
            float berry = 5f * scale;
            g.FillEllipse(_design.OrnamentColor, left.X - berry, left.Y - berry, berry * 2f, berry * 2f);
            g.FillEllipse(_design.OrnamentColor, right.X - berry, right.Y - berry, berry * 2f, berry * 2f);
        }

        DrawCenterScrollwork(g, center, scale);
        if (_design.AgedSurface)
        {
            DrawAgedSurface(g, center, radius, scale);
        }
    }

    private void DrawCenterScrollwork(ID2DGraphics g, PointF center, float scale)
    {
        using var pen = new Pen(Color.FromArgb(125, _design.FaceBorderColor), 3f * scale);
        for (int side = -1; side <= 1; side += 2)
        {
            for (int row = 0; row < 4; row++)
            {
                float y = center.Y + ((row - 1.5f) * 54f * scale);
                float x = center.X + (side * (55f + (row * 18f)) * scale);
                float r = (20f + (row * 5f)) * scale;
                g.DrawEllipse(pen, new RectangleF(x - r, y - r, r * 2f, r * 2f));
                g.DrawLine(pen, center.X, y, x, y - (18f * scale));
            }
        }
    }

    private static void DrawAgedSurface(ID2DGraphics g, PointF center, float radius, float scale)
    {
        for (int index = 0; index < 34; index++)
        {
            float angle = (index * 137.50777f) % 360f;
            float distance = radius * (0.12f + (((index * 47) % 73) / 100f));
            PointF spot = PointAt(center, distance, angle);
            float spotRadius = (2f + ((index * 11) % 13)) * scale;
            int alpha = 12 + ((index * 17) % 25);
            Color stain = index % 3 == 0
                ? Color.FromArgb(alpha, 116, 54, 35)
                : Color.FromArgb(alpha, 72, 53, 35);
            g.FillEllipse(
                stain,
                spot.X - spotRadius,
                spot.Y - (spotRadius * 0.65f),
                spotRadius * 2f,
                spotRadius * 1.3f);
        }

        for (int ring = 0; ring < 5; ring++)
        {
            float r = radius - ((94f + (ring * 13f)) * scale);
            using var wear = new Pen(Color.FromArgb(22 - (ring * 3), 105, 66, 39), (2f + ring) * scale);
            g.DrawEllipse(wear, new RectangleF(center.X - r, center.Y - r, r * 2f, r * 2f));
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
