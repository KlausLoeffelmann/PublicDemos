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
        float tipY = 8f * scale;
        float pivotY = ctx.Pivot.Y;
        float bladeHalf = 16f * scale;

        using (var blade = new SolidBrush(palette.Blade))
        {
            g.FillPolygon(blade,
            [
                new PointF(cx, tipY),
                new PointF(cx + bladeHalf, pivotY),
                new PointF(cx, pivotY + 36f * scale),
                new PointF(cx - bladeHalf, pivotY),
            ]);
        }

        int hour = ctx.Time.Now.Hour;
        int minute = ctx.Time.Now.Minute;

        float dotR = 11f * scale;
        float top = tipY + dotR + 6f * scale;
        float bottom = pivotY - dotR - 6f * scale;

        const int minuteBits = 6;
        const int hourBits = 5;
        float gap = 34f * scale;
        float step = (bottom - top - gap) / (minuteBits - 1 + hourBits - 1);

        int slot = 0;
        for (int b = minuteBits - 1; b >= 0; b--, slot++)
        {
            DrawBit(g, cx, top + step * slot, dotR, (minute & (1 << b)) != 0);
        }

        float hourTop = top + step * (minuteBits - 1) + gap;
        for (int b = hourBits - 1, h = 0; b >= 0; b--, h++)
        {
            DrawBit(g, cx, hourTop + step * h, dotR, (hour & (1 << b)) != 0);
        }
    }

    private void DrawBit(ID2DGraphics g, float cx, float cy, float r, bool on)
        => g.FillEllipse(on ? palette.On : palette.Off, cx - r, cy - r, r * 2f, r * 2f);
}
