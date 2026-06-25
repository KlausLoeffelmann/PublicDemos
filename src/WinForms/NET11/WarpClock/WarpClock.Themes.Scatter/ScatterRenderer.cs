using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.Scatter;

/// <summary>
///  Draws the Scatter theme's parts: a dark dial backdrop, the hour numerals as round
///  "magnets", three plain tapered needles of distinct lengths, and a center cap.
///  Transparent / Invisible numerals are simply not drawn (the engine hides their visuals).
/// </summary>
internal sealed class ScatterRenderer : IClockElementRenderer
{
    private static readonly Color s_face = Color.FromArgb(255, 14, 16, 22);
    private static readonly Color s_magnetFill = Color.FromArgb(255, 210, 60, 60);
    private static readonly Color s_magnetRim = Color.FromArgb(255, 250, 210, 120);
    private static readonly Color s_label = Color.FromArgb(255, 250, 245, 235);
    private static readonly Color s_hand = Color.FromArgb(255, 235, 235, 245);
    private static readonly Color s_second = Color.FromArgb(255, 240, 130, 60);
    private static readonly Color s_arbour = Color.FromArgb(255, 250, 210, 120);

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
            case ClockElementKind.HourHand:
                DrawNeedle(g, ctx, s_hand, 0.42f);
                break;
            case ClockElementKind.MinuteHand:
                DrawNeedle(g, ctx, s_hand, 0.30f);
                break;
            case ClockElementKind.SecondHand:
                DrawNeedle(g, ctx, s_second, 0.22f);
                break;
            case ClockElementKind.Arbour:
                float r = ctx.ContentSize.Width / 2f;
                g.FillEllipse(s_arbour, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                break;
        }
    }

    private static void DrawFace(ID2DGraphics g, IClockRenderContext ctx)
    {
        float radius = 495f * ctx.Scale;
        g.FillEllipse(s_face, ctx.Pivot.X - radius, ctx.Pivot.Y - radius, radius * 2f, radius * 2f);
    }

    private static void DrawMagnet(ID2DGraphics g, IClockRenderContext ctx)
    {
        float cx = ctx.ContentSize.Width / 2f;
        float cy = ctx.ContentSize.Height / 2f;
        float radius = MathF.Min(cx, cy) * 0.82f;

        // A bright rim and a filled disc give the numeral a "magnet" look.
        g.FillEllipse(s_magnetRim, cx - radius, cy - radius, radius * 2f, radius * 2f);
        float inner = radius - 6f * ctx.Scale;
        g.FillEllipse(s_magnetFill, cx - inner, cy - inner, inner * 2f, inner * 2f);

        int index = ((ctx.Id.Index % 12) + 12) % 12;
        string text = (index == 0 ? 12 : index).ToString();

        float fontSize = ctx.ContentSize.Height * 0.5f;
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(s_label);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
    }

    /// <summary>
    ///  Draws a tapered needle pointing up from the pivot to near the top of the content
    ///  box. <paramref name="baseHalfFraction"/> sets how wide the base is relative to the
    ///  content width (hour wide, second thin).
    /// </summary>
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
            new PointF(cx, pivotY + 24f * ctx.Scale),
            new PointF(cx - baseHalf, pivotY),
        ]);
    }
}
