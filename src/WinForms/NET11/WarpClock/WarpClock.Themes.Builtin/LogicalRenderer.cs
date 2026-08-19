using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.Builtin;

internal sealed class LogicalRenderer(LogicalThemePalette palette) : IClockElementRenderer
{
    private static readonly string[] s_labels =
        ["12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"];

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
            case ClockElementKind.Weekday:
                DrawAuxiliaryLabel(graphics, context, palette.Numeral, 0.56f, FontStyle.Bold);
                break;
            case ClockElementKind.Day:
                DrawAuxiliaryLabel(graphics, context, palette.Numeral, 0.54f, FontStyle.Bold);
                break;
            case ClockElementKind.TimeZone:
                DrawAuxiliaryLabel(graphics, context, palette.FaceRing, 0.50f, FontStyle.Regular);
                break;
            case ClockElementKind.HourMarker:
                DrawHourMarker(graphics, context);
                break;
            case ClockElementKind.HourHand:
                DrawHand(graphics, context, HandSlot.Hour, palette.HourHand);
                break;
            case ClockElementKind.MinuteHand:
                DrawHand(graphics, context, HandSlot.Minute, palette.MinuteHand);
                break;
            case ClockElementKind.SecondHand:
                DrawHand(graphics, context, HandSlot.Second, palette.SecondHand);
                break;
            case ClockElementKind.Arbour:
                DrawArbour(graphics, context);
                break;
        }
    }

    private void DrawCase(ID2DGraphics graphics, IClockRenderContext context)
    {
        PointF center = context.Pivot;
        Color shell = ApplyOpacity(Lerp(palette.FaceFill, palette.FaceRing, 0.18f), context.Parameters.Opacity);
        float outerRadius = 468f * context.Scale;
        float innerCutRadius = 438f * context.Scale;

        graphics.FillEllipse(shell, center.X - outerRadius, center.Y - outerRadius, outerRadius * 2f, outerRadius * 2f);

        Color innerCut = ApplyOpacity(Color.FromArgb(235, 6, 10, 16), context.Parameters.Opacity);
        graphics.FillEllipse(
            innerCut,
            center.X - innerCutRadius,
            center.Y - innerCutRadius,
            innerCutRadius * 2f,
            innerCutRadius * 2f);

        using var outerPen = new Pen(ApplyOpacity(palette.FaceRing, context.Parameters.Opacity), 12f * context.Scale);
        float strokeRadius = 462f * context.Scale;
        graphics.DrawEllipse(
            outerPen,
            new RectangleF(center.X - strokeRadius, center.Y - strokeRadius, strokeRadius * 2f, strokeRadius * 2f));
    }

    private void DrawFace(ID2DGraphics graphics, IClockRenderContext context)
    {
        PointF center = context.Pivot;
        float radius = 420f * context.Scale;
        Color fill = ApplyOpacity(palette.FaceFill, context.Parameters.Opacity);

        graphics.FillEllipse(fill, center.X - radius, center.Y - radius, radius * 2f, radius * 2f);

        float outerStroke = 14f * context.Scale;
        using var outerPen = new Pen(ApplyOpacity(palette.FaceRing, context.Parameters.Opacity), outerStroke);
        float outerRadius = radius - (outerStroke * 0.5f);
        graphics.DrawEllipse(
            outerPen,
            new RectangleF(center.X - outerRadius, center.Y - outerRadius, outerRadius * 2f, outerRadius * 2f));

        using var innerPen = new Pen(ApplyOpacity(palette.FaceInnerRing, context.Parameters.Opacity), 3.5f * context.Scale);
        float innerRadius = radius * 0.78f;
        graphics.DrawEllipse(
            innerPen,
            new RectangleF(center.X - innerRadius, center.Y - innerRadius, innerRadius * 2f, innerRadius * 2f));
    }

    private void DrawAuxiliaryLabel(
        ID2DGraphics graphics,
        IClockRenderContext context,
        Color baseColor,
        float fontScale,
        FontStyle style)
    {
        string text = context.Parameters.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        Color color = AnimatedColor(baseColor, context);
        float fontSize = context.ContentSize.Height * fontScale;
        using var font = new Font("Segoe UI", fontSize, style, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(color);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
        };

        graphics.DrawString(
            text,
            font,
            brush,
            new RectangleF(0f, 0f, context.ContentSize.Width, context.ContentSize.Height),
            format);
    }

    private void DrawHourMarker(ID2DGraphics graphics, IClockRenderContext context)
    {
        int index = ((context.Id.Index % 12) + 12) % 12;
        string text = s_labels[index];

        Color color = AnimatedColor(palette.Numeral, context);
        float fontSize = context.ContentSize.Height * 0.60f;
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(color);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        graphics.DrawString(
            text,
            font,
            brush,
            new RectangleF(0f, 0f, context.ContentSize.Width, context.ContentSize.Height),
            format);
    }

    private void DrawHand(ID2DGraphics graphics, IClockRenderContext context, HandSlot slot, Color baseColor)
    {
        HandShape shape = HandGeometry.Build(HandStyle.Modern, slot);
        Color color = AnimatedColor(baseColor, context);
        using var brush = new SolidBrush(color);

        foreach (PointF[] polygon in shape.Polygons)
        {
            PointF[] scaled = new PointF[polygon.Length];
            for (int i = 0; i < polygon.Length; i++)
            {
                scaled[i] = new PointF(polygon[i].X * context.Scale, polygon[i].Y * context.Scale);
            }

            graphics.FillPolygon(brush, scaled);
        }
    }

    private void DrawArbour(ID2DGraphics graphics, IClockRenderContext context)
    {
        Color outer = AnimatedColor(palette.Arbour, context);
        float radius = 21f * context.Scale;
        PointF center = context.Pivot;

        graphics.FillEllipse(outer, center.X - radius, center.Y - radius, radius * 2f, radius * 2f);

        Color highlight = Color.FromArgb(
            Math.Clamp((int)Math.Round(outer.A * 0.34f), 0, 255),
            255,
            255,
            255);
        graphics.FillEllipse(
            highlight,
            center.X - (radius * 0.45f),
            center.Y - (radius * 0.70f),
            radius * 0.90f,
            radius * 0.65f);
    }

    private Color AnimatedColor(Color baseColor, IClockRenderContext context)
    {
        float opacity = Math.Clamp(context.Parameters.Opacity, 0f, 1f);
        float flash = Math.Clamp(context.Parameters.Progress, 0f, 1f) * palette.FlashCeiling;
        if (flash <= 0.001f)
        {
            return ApplyOpacity(baseColor, opacity);
        }

        int seed = GetAnimationSeed(context.Id);
        double totalSeconds = context.Time.Now.TimeOfDay.TotalSeconds;
        float beat = 0.5f + (0.5f * MathF.Sin((float)(totalSeconds * (5.1d + (seed * 0.08d))) + (seed * 0.97f)));
        int paletteIndex = Math.Abs(seed + (int)MathF.Floor((float)(totalSeconds * 2.2d))) % palette.FlashColors.Length;
        Color flashColor = palette.FlashColors[paletteIndex];
        float mix = flash * (0.38f + (0.62f * beat));

        return ApplyOpacity(Lerp(baseColor, flashColor, mix), opacity);
    }

    private static int GetAnimationSeed(ClockElementId id)
        => id.Kind switch
        {
            ClockElementKind.HourMarker => id.Index,
            ClockElementKind.HourHand => 13,
            ClockElementKind.MinuteHand => 14,
            ClockElementKind.SecondHand => 15,
            ClockElementKind.Arbour => 16,
            ClockElementKind.Case => 17,
            ClockElementKind.Weekday => 18,
            ClockElementKind.TimeZone => 19,
            ClockElementKind.Day => 20,
            _ => 0,
        };

    private static Color ApplyOpacity(Color color, float opacity)
        => Color.FromArgb(
            Math.Clamp((int)Math.Round(color.A * Math.Clamp(opacity, 0f, 1f)), 0, 255),
            color.R,
            color.G,
            color.B);

    private static Color Lerp(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return Color.FromArgb(
            (int)Math.Round(from.A + ((to.A - from.A) * amount)),
            (int)Math.Round(from.R + ((to.R - from.R) * amount)),
            (int)Math.Round(from.G + ((to.G - from.G) * amount)),
            (int)Math.Round(from.B + ((to.B - from.B) * amount)));
    }
}
