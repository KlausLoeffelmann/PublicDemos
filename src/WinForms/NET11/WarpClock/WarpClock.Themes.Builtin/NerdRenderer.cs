using System.Drawing;
using System.Globalization;

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

        NerdHandRenderState cheat = ctx.Parameters.Tag is NerdHandRenderState state
            ? state
            : new NerdHandRenderState(ctx.Time.SecondAngle, 0f, 0f);
        DrawHandCheatLabels(g, ctx, cheat, cx, pivotY, scale);
    }

    private void DrawSecondsSled(ID2DGraphics g, IClockRenderContext ctx)
    {
        NerdSlideRenderState state = ctx.Parameters.Tag is NerdSlideRenderState renderState
            ? renderState
            : new NerdSlideRenderState(
                ctx.Parameters.ExtraRotationDegrees,
                NerdThemeGeometry.SledRadius,
                1f,
                NerdBinaryLayout.SecondAtAngle(ctx.Parameters.ExtraRotationDegrees),
                0f);
        NerdThemePalette displayPalette = ApplyOpacity(palette, ctx.Parameters.Opacity);
        float cx = ctx.ContentSize.Width / 2f;
        float pivotY = ctx.Pivot.Y;
        float scale = ctx.Scale;
        using var blade = new SolidBrush(displayPalette.Blade);
        g.FillPolygon(
            blade,
            CreateSledPolygon(cx, pivotY, scale, state.TrackRadius, state.BeamScale));
        DrawSecondsLeds(
            g,
            cx,
            pivotY,
            scale,
            state.TrackRadius,
            state.BeamScale,
            state.PositionSecond,
            displayPalette);
        DrawSledCheatLabel(g, state, cx, pivotY, scale);
    }

    private static PointF[] CreateSledPolygon(
        float cx,
        float pivotY,
        float scale,
        float trackRadius,
        float beamScale)
    {
        const int segments = 12;
        var points = new PointF[(segments + 1) * 2];
        float halfThickness = NerdThemeGeometry.SledHalfThickness * beamScale;
        float innerRadius = (trackRadius - halfThickness) * scale;
        float outerRadius = (trackRadius + halfThickness) * scale;
        float halfSpan = NerdThemeGeometry.SledHalfSpanDegrees * beamScale;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = -halfSpan + (2f * halfSpan * t);
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
        float trackRadius,
        float beamScale,
        int second,
        NerdThemePalette displayPalette)
    {
        float radius = trackRadius * scale;
        float ledRadius = NerdThemeGeometry.LedRadius * beamScale * scale;
        bool lsbFirst = NerdBinaryLayout.SecondsUseLeastSignificantBitFirst(second);

        for (int slot = 0; slot < NerdThemeGeometry.SecondBitCount; slot++)
        {
            float t = slot / (float)(NerdThemeGeometry.SecondBitCount - 1);
            float halfSpan = NerdThemeGeometry.SledLedHalfSpanDegrees * beamScale;
            float angle = -halfSpan + (2f * halfSpan * t);
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

    private void DrawHandCheatLabels(
        ID2DGraphics g,
        IClockRenderContext ctx,
        NerdHandRenderState state,
        float cx,
        float pivotY,
        float scale)
    {
        float hourRadius = NerdThemeGeometry.HourBankInnerRadius
            + (((NerdThemeGeometry.HourBitCount - 1) * NerdThemeGeometry.BladeLedPitch) / 2f);
        DrawDecimalLabel(
            g,
            ctx.Time.Now.Hour.ToString("00", CultureInfo.InvariantCulture),
            new PointF(cx, pivotY - (hourRadius * scale)),
            new SizeF(72f * scale, 38f * scale),
            palette.HourOn,
            state.HourCheatOpacity,
            rotationDegrees: IsLowerHalf(state.Angle) ? 180f : 0f);

        float minuteRadius = NerdThemeGeometry.MinuteBankInnerRadius
            + (((NerdThemeGeometry.MinuteBitCount - 1) * NerdThemeGeometry.BladeLedPitch) / 2f);
        bool lowerHalf = IsLowerHalf(state.Angle);
        DrawDecimalLabel(
            g,
            ctx.Time.Now.Minute.ToString("00", CultureInfo.InvariantCulture),
            new PointF(cx + (38f * scale), pivotY - (minuteRadius * scale)),
            new SizeF(76f * scale, 38f * scale),
            palette.MinuteOn,
            state.MinuteCheatOpacity,
            rotationDegrees: 90f + (lowerHalf ? 180f : 0f));
    }

    private void DrawSledCheatLabel(
        ID2DGraphics g,
        NerdSlideRenderState state,
        float cx,
        float pivotY,
        float scale)
    {
        float labelRadius =
            state.TrackRadius - NerdThemeGeometry.SledHalfThickness - 28f;
        DrawDecimalLabel(
            g,
            state.PositionSecond.ToString("00", CultureInfo.InvariantCulture),
            new PointF(cx, pivotY - (labelRadius * scale)),
            new SizeF(72f * scale, 36f * scale),
            palette.SecondOn,
            state.CheatOpacity,
            IsLowerHalf(state.Angle) ? 180f : 0f);
    }

    private static void DrawDecimalLabel(
        ID2DGraphics g,
        string text,
        PointF center,
        SizeF size,
        Color color,
        float opacity,
        float rotationDegrees)
    {
        opacity = Math.Clamp(opacity, 0f, 1f);
        if (opacity <= 0.001f)
        {
            return;
        }

        using var font = new Font(
            "Consolas",
            size.Height * 0.72f,
            FontStyle.Bold,
            GraphicsUnit.Pixel);
        using var shadow = new SolidBrush(Color.FromArgb(
            (int)MathF.Round(150f * opacity),
            5,
            8,
            12));
        using var brush = new SolidBrush(ApplyOpacity(color, opacity));
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
        };

        var bounds = new RectangleF(
            center.X - (size.Width / 2f),
            center.Y - (size.Height / 2f),
            size.Width,
            size.Height);

        g.ResetTransform();
        g.TranslateTransform(center.X, center.Y);
        g.RotateTransform(rotationDegrees);
        g.TranslateTransform(-center.X, -center.Y);
        g.DrawString(
            text,
            font,
            shadow,
            new RectangleF(bounds.X + 2f, bounds.Y + 2f, bounds.Width, bounds.Height),
            format);
        g.DrawString(text, font, brush, bounds, format);
        g.ResetTransform();
    }

    private static bool IsLowerHalf(float angle)
    {
        angle %= 360f;
        if (angle < 0f)
        {
            angle += 360f;
        }

        return angle > 90f && angle < 270f;
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

    private static Color ApplyOpacity(Color color, float opacity)
        => Color.FromArgb(
            Math.Clamp((int)MathF.Round(color.A * Math.Clamp(opacity, 0f, 1f)), 0, 255),
            color.R,
            color.G,
            color.B);
}
