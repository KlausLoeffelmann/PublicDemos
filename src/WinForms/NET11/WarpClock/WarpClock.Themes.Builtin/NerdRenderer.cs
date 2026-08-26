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
                DrawAnimatedFace(g, ctx);
                break;
            case ClockElementKind.SecondHand:
                DrawDisplayHand(g, ctx);
                break;
            case ClockElementKind.Custom:
                DrawSecondsSled(g, ctx);
                break;
            case ClockElementKind.Arbour:
                float r = ctx.ContentSize.Width / 2f;
                g.FillEllipse(palette.Grid, ctx.Pivot.X - r, ctx.Pivot.Y - r, r * 2f, r * 2f);
                break;
        }
    }

    private void DrawAnimatedFace(ID2DGraphics g, IClockRenderContext ctx)
    {
        const float holdSeconds = 7f;
        const float transitionSeconds = 7f;
        const float segmentSeconds = holdSeconds + transitionSeconds;
        Color[] colors = [palette.FaceBlue, palette.FaceRed, palette.FaceGreen];
        float progress = ctx.Parameters.Progress % (segmentSeconds * colors.Length);
        int index = (int)(progress / segmentSeconds) % colors.Length;
        float local = progress - (index * segmentSeconds);
        float transition = Math.Clamp((local - holdSeconds) / transitionSeconds, 0f, 1f);
        Color current = colors[index];
        Color next = colors[(index + 1) % colors.Length];
        float radius = 490f * ctx.Scale;

        g.FillEllipse(current, ctx.Pivot.X - radius, ctx.Pivot.Y - radius, radius * 2f, radius * 2f);

        const int rings = 28;
        for (int ring = 0; ring < rings; ring++)
        {
            float inward = ring / (float)(rings - 1);
            float wave = Math.Clamp((transition - (inward * 0.72f)) / 0.28f, 0f, 1f);
            float eased = wave * wave * (3f - (2f * wave));
            float ringRadius = radius * (1f - (inward * 0.92f));
            Color color = Blend(current, next, eased);
            g.FillEllipse(
                color,
                ctx.Pivot.X - ringRadius,
                ctx.Pivot.Y - ringRadius,
                ringRadius * 2f,
                ringRadius * 2f);
        }
    }

    private void DrawDisplayHand(ID2DGraphics g, IClockRenderContext ctx)
    {
        float cx = ctx.ContentSize.Width / 2f;
        float pivotY = ctx.Pivot.Y;
        float scale = ctx.Scale;
        float bladeTop = pivotY - (NerdThemeGeometry.BladeTopRadius * scale);
        float bladeHalf = NerdThemeGeometry.BladeHalfWidth * scale;

        using (var blade = new SolidBrush(palette.Blade))
        {
            g.FillPolygon(blade,
            [
                new PointF(cx, bladeTop),
                new PointF(cx + (bladeHalf * 0.35f), bladeTop + (34f * scale)),
                new PointF(cx + bladeHalf, pivotY),
                new PointF(cx + (bladeHalf * 0.55f), pivotY + (NerdThemeGeometry.BladeTailDepth * scale)),
                new PointF(cx - (bladeHalf * 0.55f), pivotY + (NerdThemeGeometry.BladeTailDepth * scale)),
                new PointF(cx - bladeHalf, pivotY),
                new PointF(cx - (bladeHalf * 0.35f), bladeTop + (34f * scale)),
            ]);
        }

        DrawRadialBank(
            g,
            cx,
            pivotY,
            scale,
            ctx.Time.Now.Hour,
            NerdThemeGeometry.HourBitCount,
            NerdThemeGeometry.HourBankInnerRadius,
            palette.HourOn,
            palette.HourOff);
        DrawRadialBank(
            g,
            cx,
            pivotY,
            scale,
            ctx.Time.Now.Minute,
            NerdThemeGeometry.MinuteBitCount,
            NerdThemeGeometry.MinuteBankInnerRadius,
            palette.MinuteOn,
            palette.MinuteOff);
    }

    private void DrawSecondsSled(ID2DGraphics g, IClockRenderContext ctx)
    {
        NerdThemePalette displayPalette = ApplyOpacity(palette, ctx.Parameters.Opacity);
        float cx = ctx.ContentSize.Width / 2f;
        float pivotY = ctx.Pivot.Y;
        float scale = ctx.Scale;
        using var blade = new SolidBrush(displayPalette.Blade);
        g.FillPolygon(blade, CreateSledPolygon(cx, pivotY, scale));
        DrawSecondsLeds(g, cx, pivotY, scale, ctx.Time.Now.Second, displayPalette);
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

    private void DrawSecondsLeds(
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

    private static Color Blend(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        int Mix(int left, int right) => (int)MathF.Round(left + ((right - left) * amount));
        return Color.FromArgb(
            Mix(from.A, to.A),
            Mix(from.R, to.R),
            Mix(from.G, to.G),
            Mix(from.B, to.B));
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
