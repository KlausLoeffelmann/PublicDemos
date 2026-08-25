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
            case ClockElementKind.Custom:
                DrawDisplayHand(g, ctx);
                break;
            case ClockElementKind.Arbour:
                float r = ctx.ContentSize.Width / 2f;
                g.FillEllipse(palette.Grid, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                break;
        }
    }

    private void DrawDisplayHand(ID2DGraphics g, IClockRenderContext ctx)
    {
        NerdThemePalette displayPalette = ApplyOpacity(palette, ctx.Parameters.Opacity);
        float cx = ctx.ContentSize.Width / 2f;
        float pivotY = ctx.Pivot.Y;
        float scale = ctx.Scale;
        float bladeTop = pivotY - (NerdThemeGeometry.BladeTopRadius * scale);
        float bladeHalf = NerdThemeGeometry.BladeHalfWidth * scale;

        using (var blade = new SolidBrush(displayPalette.Blade))
        {
            g.FillPolygon(blade,
            [
                new PointF(cx - (bladeHalf * 0.7f), bladeTop),
                new PointF(cx + (bladeHalf * 0.7f), bladeTop),
                new PointF(cx + bladeHalf, pivotY),
                new PointF(cx + (bladeHalf * 0.55f), pivotY + (NerdThemeGeometry.BladeTailDepth * scale)),
                new PointF(cx - (bladeHalf * 0.55f), pivotY + (NerdThemeGeometry.BladeTailDepth * scale)),
                new PointF(cx - bladeHalf, pivotY),
            ]);

            g.FillPolygon(blade, CreateSledPolygon(cx, pivotY, scale));
        }

        DrawRadialBank(
            g,
            cx,
            pivotY,
            scale,
            ctx.Time.Now.Hour,
            NerdThemeGeometry.HourBitCount,
            NerdThemeGeometry.HourBankInnerRadius,
            displayPalette.HourOn,
            displayPalette.HourOff);
        DrawRadialBank(
            g,
            cx,
            pivotY,
            scale,
            ctx.Time.Now.Minute,
            NerdThemeGeometry.MinuteBitCount,
            NerdThemeGeometry.MinuteBankInnerRadius,
            displayPalette.MinuteOn,
            displayPalette.MinuteOff);
        DrawSecondsSled(g, cx, pivotY, scale, ctx.Time.Now.Second, displayPalette);
    }

    private static PointF[] CreateSledPolygon(float cx, float pivotY, float scale)
    {
        const int segments = 12;
        var points = new PointF[(segments + 1) * 2];
        float innerRadius = (NerdThemeGeometry.SledRadius - NerdThemeGeometry.SledHalfThickness) * scale;
        float outerRadius = (NerdThemeGeometry.SledRadius + NerdThemeGeometry.SledHalfThickness) * scale;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = -NerdThemeGeometry.SledHalfSpanDegrees
                + (2f * NerdThemeGeometry.SledHalfSpanDegrees * t);
            points[i] = PointOnHandArc(cx, pivotY, outerRadius, angle);
            points[points.Length - 1 - i] = PointOnHandArc(cx, pivotY, innerRadius, angle);
        }

        return points;
    }

    private static void DrawRadialBank(
        ID2DGraphics g,
        float cx,
        float pivotY,
        float scale,
        int value,
        int bitCount,
        float innerRadius,
        Color on,
        Color off)
    {
        float radius = NerdThemeGeometry.LedRadius * scale;
        for (int slot = 0; slot < bitCount; slot++)
        {
            float distance = (innerRadius + (slot * NerdThemeGeometry.BladeLedPitch)) * scale;
            g.FillEllipse(
                NerdBinaryLayout.IsBitOn(value, slot, bitCount, leastSignificantBitFirst: true) ? on : off,
                cx - radius,
                pivotY - distance - radius,
                radius * 2f,
                radius * 2f);
        }
    }

    private void DrawSecondsSled(
        ID2DGraphics g,
        float cx,
        float pivotY,
        float scale,
        int second,
        NerdThemePalette displayPalette)
    {
        float radius = NerdThemeGeometry.SledRadius * scale;
        float ledRadius = NerdThemeGeometry.LedRadius * scale;
        bool lsbFirst = NerdBinaryLayout.SecondsUseLeastSignificantBitFirst(second);

        for (int slot = 0; slot < NerdThemeGeometry.SecondBitCount; slot++)
        {
            float t = slot / (float)(NerdThemeGeometry.SecondBitCount - 1);
            float angle = -NerdThemeGeometry.SledLedHalfSpanDegrees
                + (2f * NerdThemeGeometry.SledLedHalfSpanDegrees * t);
            PointF center = PointOnHandArc(cx, pivotY, radius, angle);
            g.FillEllipse(
                NerdBinaryLayout.IsBitOn(second, slot, NerdThemeGeometry.SecondBitCount, lsbFirst)
                    ? displayPalette.SecondOn
                    : displayPalette.SecondOff,
                center.X - ledRadius,
                center.Y - ledRadius,
                ledRadius * 2f,
                ledRadius * 2f);
        }
    }

    private static PointF PointOnHandArc(float cx, float pivotY, float radius, float angleDegrees)
    {
        float radians = angleDegrees * (MathF.PI / 180f);
        return new PointF(
            cx + (MathF.Sin(radians) * radius),
            pivotY - (MathF.Cos(radians) * radius));
    }

    private static NerdThemePalette ApplyOpacity(NerdThemePalette source, float opacity)
    {
        int alpha = (int)MathF.Round(Math.Clamp(opacity, 0f, 1f) * 255f);
        Color Fade(Color color) => Color.FromArgb(alpha * color.A / 255, color);

        return source with
        {
            Blade = Fade(source.Blade),
            HourOn = Fade(source.HourOn),
            HourOff = Fade(source.HourOff),
            MinuteOn = Fade(source.MinuteOn),
            MinuteOff = Fade(source.MinuteOff),
            SecondOn = Fade(source.SecondOn),
            SecondOff = Fade(source.SecondOff),
        };
    }
}
