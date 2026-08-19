using System.Drawing;
using System.Globalization;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  Draws the Scatter theme's parts: a dial backdrop, the hour numerals as round
///  "magnets", matching auxiliary badges for weekday/day/time-zone context, three plain
///  tapered needles of distinct lengths, and a center cap. Transparent / Invisible
///  numerals are simply not drawn (the engine hides their visuals).
/// </summary>
internal sealed class ScatterRenderer(ScatterThemePalette palette) : IClockElementRenderer
{
    public void DrawElement(ID2DGraphics g, IClockRenderContext ctx)
    {
        g.AntialiasMode = D2DAntialiasMode.PerPrimitive;

        switch (ctx.Id.Kind)
        {
            case ClockElementKind.Face:
                DrawFace(g, ctx);
                break;
            case ClockElementKind.HourMarker:
                DrawMagnet(g, ctx);
                break;
            case ClockElementKind.TimeZone:
                DrawBadge(g, ctx, ComposeTimeZoneLabel(ctx), 0.27f);
                break;
            case ClockElementKind.Day:
                DrawBadge(g, ctx, ctx.Time.Now.ToString("dd", CultureInfo.InvariantCulture), 0.48f);
                break;
            case ClockElementKind.Weekday:
                DrawBadge(g, ctx, ctx.Time.Now.ToString("ddd", CultureInfo.InvariantCulture).ToUpperInvariant(), 0.34f);
                break;
            case ClockElementKind.HourHand:
                DrawNeedle(g, ctx, palette.Hand, 0.42f);
                break;
            case ClockElementKind.MinuteHand:
                DrawNeedle(g, ctx, palette.Hand, 0.30f);
                break;
            case ClockElementKind.SecondHand:
                DrawNeedle(g, ctx, palette.Second, 0.22f);
                break;
            case ClockElementKind.Arbour:
                float r = ctx.ContentSize.Width / 2f;
                g.FillEllipse(palette.Arbour, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                break;
        }
    }

    internal static string ComposeTimeZoneLabel(IClockRenderContext ctx)
    {
        string? alias = Normalize(ctx.Ambient.TimeZoneAlias);
        string? designation = Normalize(ctx.Ambient.TimeZoneDesignation);

        if (alias is not null && designation is not null)
        {
            return string.Equals(alias, designation, StringComparison.OrdinalIgnoreCase)
                ? alias
                : $"{alias} · {designation}";
        }

        if (alias is not null)
        {
            return alias;
        }

        if (designation is not null)
        {
            return designation;
        }

        return Normalize(ctx.TimeZone.EffectiveName)
            ?? Normalize(ctx.TimeZone.Id)
            ?? "Local";
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void DrawBadge(ID2DGraphics g, IClockRenderContext ctx, string text, float fontFraction)
    {
        float cx = ctx.ContentSize.Width / 2f;
        float cy = ctx.ContentSize.Height / 2f;
        float radiusX = (ctx.ContentSize.Width * 0.5f) - (4f * ctx.Scale);
        float radiusY = (ctx.ContentSize.Height * 0.5f) - (4f * ctx.Scale);

        g.FillEllipse(palette.MagnetRim, cx - radiusX, cy - radiusY, radiusX * 2f, radiusY * 2f);
        float innerX = radiusX - (5f * ctx.Scale);
        float innerY = radiusY - (5f * ctx.Scale);
        g.FillEllipse(palette.MagnetFill, cx - innerX, cy - innerY, innerX * 2f, innerY * 2f);

        float fontSize = MathF.Max(14f * ctx.Scale, ctx.ContentSize.Height * fontFraction);
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(palette.Label);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap,
        };

        g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
    }

    private void DrawFace(ID2DGraphics g, IClockRenderContext ctx)
    {
        float radius = 495f * ctx.Scale;
        g.FillEllipse(palette.Face, ctx.Pivot.X - radius, ctx.Pivot.Y - radius, radius * 2f, radius * 2f);
    }

    private void DrawMagnet(ID2DGraphics g, IClockRenderContext ctx)
    {
        float cx = ctx.ContentSize.Width / 2f;
        float cy = ctx.ContentSize.Height / 2f;
        float radius = MathF.Min(cx, cy) * 0.82f;

        g.FillEllipse(palette.MagnetRim, cx - radius, cy - radius, radius * 2f, radius * 2f);
        float inner = radius - (6f * ctx.Scale);
        g.FillEllipse(palette.MagnetFill, cx - inner, cy - inner, inner * 2f, inner * 2f);

        int index = ((ctx.Id.Index % 12) + 12) % 12;
        string text = (index == 0 ? 12 : index).ToString(CultureInfo.InvariantCulture);

        float fontSize = ctx.ContentSize.Height * 0.5f;
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(palette.Label);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
    }

    private static void DrawNeedle(ID2DGraphics g, IClockRenderContext ctx, Color color, float baseHalfFraction)
    {
        float cx = ctx.ContentSize.Width / 2f;
        float tipY = 6f * ctx.Scale;
        float pivotY = ctx.Pivot.Y;
        float baseHalf = ctx.ContentSize.Width * baseHalfFraction;

        using var brush = new SolidBrush(color);
        g.FillPolygon(brush,
        [
            new PointF(cx, tipY),
            new PointF(cx + baseHalf, pivotY),
            new PointF(cx, pivotY + (24f * ctx.Scale)),
            new PointF(cx - baseHalf, pivotY),
        ]);
    }
}
