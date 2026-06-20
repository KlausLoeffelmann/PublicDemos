using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.LoseHour;

/// <summary>Draws the Lose-Hour theme's elements.</summary>
internal sealed class LoseHourRenderer : IClockElementRenderer
{
    private static readonly Color s_faceColor = Color.FromArgb(255, 14, 16, 22);
    private static readonly Color s_numeralColor = Color.FromArgb(255, 235, 240, 255);
    private static readonly Color s_handColor = Color.FromArgb(255, 120, 200, 255);
    private static readonly Color s_secondColor = Color.FromArgb(255, 255, 120, 120);

    private readonly string[] _labels;

    public LoseHourRenderer(string[] labels) => _labels = labels;

    public void DrawElement(ID2DGraphics g, IClockRenderContext ctx)
    {
        g.AntialiasMode = D2DAntialiasMode.PerPrimitive;

        switch (ctx.Id.Kind)
        {
            case ClockElementKind.Face:
                DrawFace(g, ctx);
                break;
            case ClockElementKind.HourMarker:
                DrawNumeral(g, ctx);
                break;
            case ClockElementKind.HourHand:
                DrawNeedle(g, ctx, ClockHandKind.Hour, s_handColor);
                break;
            case ClockElementKind.MinuteHand:
                DrawNeedle(g, ctx, ClockHandKind.Minute, s_handColor);
                break;
            case ClockElementKind.SecondHand:
                DrawNeedle(g, ctx, ClockHandKind.Second, s_secondColor);
                break;
            case ClockElementKind.Arbour:
                float r = ctx.ContentSize.Width / 2f;
                g.FillEllipse(s_handColor, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                break;
        }
    }

    private static void DrawFace(ID2DGraphics g, IClockRenderContext ctx)
    {
        float radius = 490f * ctx.Scale;
        PointF c = ctx.Pivot;
        g.FillEllipse(s_faceColor, c.X - radius, c.Y - radius, radius * 2f, radius * 2f);
    }

    private void DrawNumeral(ID2DGraphics g, IClockRenderContext ctx)
    {
        int index = ((ctx.Id.Index % 12) + 12) % 12;
        string text = ctx.Parameters.Text ?? _labels[index];
        int alpha = (int)(Math.Clamp(ctx.Parameters.Opacity, 0f, 1f) * 255f);
        if (alpha <= 0)
        {
            return;
        }

        float fontSize = ctx.ContentSize.Height * 0.62f;
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(Color.FromArgb(alpha, s_numeralColor));
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
    }

    private static void DrawNeedle(ID2DGraphics g, IClockRenderContext ctx, ClockHandKind hand, Color color)
    {
        NeedleSpec spec = NeedleSpec.For(hand);
        using var brush = new SolidBrush(color);
        g.FillPolygon(brush, spec.BuildPolygon(ctx.Scale));
    }
}
