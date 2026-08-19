using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.Builtin;

internal sealed class NerdRenderer(NerdThemePalette palette) : IClockElementRenderer
{
    public void DrawElement(ID2DGraphics g, IClockRenderContext ctx)
    {
        g.AntialiasMode = D2DAntialiasMode.PerPrimitive;

        switch (ctx.Id.Kind)
        {
            case ClockElementKind.Face:
                float radius = 490f * ctx.Scale;
                g.FillEllipse(palette.Face, ctx.Pivot.X - radius, ctx.Pivot.Y - radius, radius * 2f, radius * 2f);
                break;
            case ClockElementKind.HourMarker:
                DrawOctal(g, ctx);
                break;
            case ClockElementKind.SecondHand:
                DrawBinaryHand(g, ctx);
                break;
            case ClockElementKind.Arbour:
                float r = ctx.ContentSize.Width / 2f;
                g.FillEllipse(palette.Grid, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                break;
        }
    }

    private void DrawOctal(ID2DGraphics g, IClockRenderContext ctx)
    {
        int index = ((ctx.Id.Index % 12) + 12) % 12;
        int hourValue = index == 0 ? 12 : index;
        string text = Convert.ToString(hourValue, 8);

        float fontSize = ctx.ContentSize.Height * 0.7f;
        using var font = new Font("Consolas", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(palette.Grid);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
    }

    private void DrawBinaryHand(ID2DGraphics g, IClockRenderContext ctx)
    {
        float scale = ctx.Scale;
        float cx = ctx.ContentSize.Width / 2f;
        float tipY = NerdThemeGeometry.TipInset * scale;
        float pivotY = ctx.Pivot.Y;
        float shoulderY = tipY + (NerdThemeGeometry.ShoulderInset * scale);
        float lowerY = pivotY - (58f * scale);
        float shoulderHalf = NerdThemeGeometry.ShoulderHalfWidth * scale;
        float lowerHalf = NerdThemeGeometry.LowerHalfWidth * scale;
        float tailHalf = NerdThemeGeometry.TailHalfWidth * scale;

        using (var blade = new SolidBrush(palette.Blade))
        {
            g.FillPolygon(blade,
            [
                new PointF(cx, tipY),
                new PointF(cx + shoulderHalf, shoulderY),
                new PointF(cx + lowerHalf, lowerY),
                new PointF(cx + tailHalf, pivotY + (NerdThemeGeometry.TailDepth * scale)),
                new PointF(cx - tailHalf, pivotY + (NerdThemeGeometry.TailDepth * scale)),
                new PointF(cx - lowerHalf, lowerY),
                new PointF(cx - shoulderHalf, shoulderY),
            ]);
        }

        int hour = ctx.Time.Now.Hour;
        int minute = ctx.Time.Now.Minute;

        float dotR = NerdThemeGeometry.DotRadius * scale;
        float top = NerdThemeGeometry.DotTop * scale;
        float bottom = NerdThemeGeometry.DotBottom * scale;
        float offset = NerdThemeGeometry.BitColumnOffset * scale;

        DrawBitColumn(g, cx - offset, top, bottom, dotR, minute, NerdThemeGeometry.MinuteBitCount, palette.MinuteOn, palette.MinuteOff);
        DrawBitColumn(g, cx + offset, top, bottom, dotR, hour, NerdThemeGeometry.HourBitCount, palette.HourOn, palette.HourOff);
    }

    private static void DrawBitColumn(
        ID2DGraphics g,
        float cx,
        float top,
        float bottom,
        float r,
        int value,
        int bitCount,
        Color on,
        Color off)
    {
        float step = bitCount > 1 ? (bottom - top) / (bitCount - 1) : 0f;
        for (int b = bitCount - 1, slot = 0; b >= 0; b--, slot++)
        {
            g.FillEllipse(
                (value & (1 << b)) != 0 ? on : off,
                cx - r,
                top + (step * slot) - r,
                r * 2f,
                r * 2f);
        }
    }
}
