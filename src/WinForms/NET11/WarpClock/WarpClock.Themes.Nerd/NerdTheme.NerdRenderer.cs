using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.Nerd;

public sealed partial class NerdTheme
{
    private sealed class NerdRenderer : IClockElementRenderer
    {
        private static readonly Color s_face = Color.FromArgb(255, 8, 12, 10);
        private static readonly Color s_grid = Color.FromArgb(255, 80, 220, 140);
        private static readonly Color s_blade = Color.FromArgb(110, 40, 90, 60);
        private static readonly Color s_on = Color.FromArgb(255, 120, 255, 170);
        private static readonly Color s_off = Color.FromArgb(255, 30, 70, 50);

        public void DrawElement(ID2DGraphics g, IClockRenderContext ctx)
        {
            g.AntialiasMode = D2DAntialiasMode.PerPrimitive;

            switch (ctx.Id.Kind)
            {
                case ClockElementKind.Face:
                    float radius = 490f * ctx.Scale;
                    g.FillEllipse(s_face, ctx.Pivot.X - radius, ctx.Pivot.Y - radius, radius * 2f, radius * 2f);
                    break;
                case ClockElementKind.HourMarker:
                    DrawOctal(g, ctx);
                    break;
                case ClockElementKind.SecondHand:
                    DrawBinaryHand(g, ctx);
                    break;
                case ClockElementKind.Arbour:
                    float r = ctx.ContentSize.Width / 2f;
                    g.FillEllipse(s_grid, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                    break;
            }
        }

        private static void DrawOctal(ID2DGraphics g, IClockRenderContext ctx)
        {
            int index = ((ctx.Id.Index % 12) + 12) % 12;
            int hourValue = index == 0 ? 12 : index;
            string text = Convert.ToString(hourValue, 8);

            float fontSize = ctx.ContentSize.Height * 0.7f;
            using var font = new Font("Consolas", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(s_grid);
            using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, font, brush, new RectangleF(0, 0, ctx.ContentSize.Width, ctx.ContentSize.Height), format);
        }

        private static void DrawBinaryHand(ID2DGraphics g, IClockRenderContext ctx)
        {
            float scale = ctx.Scale;
            float cx = ctx.ContentSize.Width / 2f;
            float tipY = 8f * scale;
            float pivotY = ctx.Pivot.Y;
            float bladeHalf = 16f * scale;

            // The blade itself.
            using (var blade = new SolidBrush(s_blade))
            {
                g.FillPolygon(blade,
                [
                    new PointF(cx, tipY),
                    new PointF(cx + bladeHalf, pivotY),
                    new PointF(cx, pivotY + 36f * scale),
                    new PointF(cx - bladeHalf, pivotY),
                ]);
            }

            int hour = ctx.Time.Now.Hour;   // 0..23
            int minute = ctx.Time.Now.Minute; // 0..59

            float dotR = 11f * scale;
            float top = tipY + dotR + 6f * scale;
            float bottom = pivotY - dotR - 6f * scale;

            // Two visually separated groups along the blade: the minute (6 bits) is read
            // toward the tip (outer) and the hour (5 bits) toward the pivot (inner), with a
            // clear gap between them so the two readouts never blur together. Both groups
            // share the same dot spacing; the gap simply consumes extra vertical room.
            const int minuteBits = 6;
            const int hourBits = 5;
            float gap = 34f * scale;
            float step = (bottom - top - gap) / (minuteBits - 1 + hourBits - 1);

            int slot = 0;

            // Minute, MSB first, starting at the tip end.
            for (int b = minuteBits - 1; b >= 0; b--, slot++)
            {
                DrawBit(g, cx, top + step * slot, dotR, (minute & (1 << b)) != 0);
            }

            // Gap, then the hour, MSB first, ending near the pivot.
            float hourTop = top + step * (minuteBits - 1) + gap;
            for (int b = hourBits - 1, h = 0; b >= 0; b--, h++)
            {
                DrawBit(g, cx, hourTop + step * h, dotR, (hour & (1 << b)) != 0);
            }
        }

        private static void DrawBit(ID2DGraphics g, float cx, float cy, float r, bool on)
        {
            g.FillEllipse(on ? s_on : s_off, cx - r, cy - r, r * 2f, r * 2f);
        }
    }
}
