using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.EmberClock;

/// <summary>
///  Draws the ember clock. Each element is drawn in its own local pixel space; flame and numeral
///  intensity come from the element's burn level (<see cref="ClockElementParameters.Progress"/>).
/// </summary>
internal sealed class EmberClockRenderer : IClockElementRenderer
{
    private static readonly Color s_space = Color.FromArgb(255, 18, 16, 14);
    private static readonly Color s_spaceRim = Color.FromArgb(255, 70, 58, 44);
    private static readonly Color s_ringOuter = Color.FromArgb(130, 150, 120, 70);
    private static readonly Color s_ringInner = Color.FromArgb(100, 130, 104, 60);
    private static readonly Color s_ember = Color.FromArgb(70, 120, 80, 40);
    private static readonly Color s_gold = Color.FromArgb(255, 246, 202, 78);
    private static readonly Color s_goldDark = Color.FromArgb(255, 200, 150, 40);

    // Roman numerals, hour 0 (12 o'clock) = XII, clockwise.
    private static readonly string[] s_marks =
        ["XII", "I", "II", "III", "IV", "V",
         "VI", "VII", "VIII", "IX", "X", "XI"];

    public void DrawElement(ID2DGraphics g, IClockRenderContext ctx)
    {
        g.AntialiasMode = D2DAntialiasMode.PerPrimitive;

        switch (ctx.Id.Kind)
        {
            case ClockElementKind.Face:
                DrawFace(g, ctx);
                break;
            case ClockElementKind.HourMarker:
                DrawFlame(g, ctx);
                break;
            case ClockElementKind.Custom:
                DrawNumeral(g, ctx);
                break;
            case ClockElementKind.HourHand:
                DrawHand(g, ctx, widthFactor: 0.55f, tipFactor: 0.16f);
                break;
            case ClockElementKind.MinuteHand:
                DrawHand(g, ctx, widthFactor: 0.42f, tipFactor: 0.12f);
                break;
            case ClockElementKind.SecondHand:
                DrawHand(g, ctx, widthFactor: 0.30f, tipFactor: 0.08f);
                break;
            case ClockElementKind.Arbour:
                DrawHub(g, ctx);
                break;
        }
    }

    private static void DrawFace(ID2DGraphics g, IClockRenderContext ctx)
    {
        PointF c = ctx.Pivot;
        float r = MathF.Min(ctx.ContentSize.Width, ctx.ContentSize.Height) * 0.5f;

        g.FillEllipse(s_spaceRim, c.X - r, c.Y - r, r * 2f, r * 2f);
        float ri = r * 0.965f;
        g.FillEllipse(s_space, c.X - ri, c.Y - ri, ri * 2f, ri * 2f);

        using (var outer = new Pen(s_ringOuter, r * 0.010f))
        {
            float ro = r * 0.78f;
            g.DrawEllipse(outer, new RectangleF(c.X - ro, c.Y - ro, ro * 2f, ro * 2f));
        }
        using (var inner = new Pen(s_ringInner, r * 0.008f))
        {
            float rin = r * EmberClockTheme.NumeralRingRadius;
            g.DrawEllipse(inner, new RectangleF(c.X - rin, c.Y - rin, rin * 2f, rin * 2f));
        }
    }

    private static void DrawFlame(ID2DGraphics g, IClockRenderContext ctx)
    {
        float w = ctx.ContentSize.Width;
        float h = ctx.ContentSize.Height;
        float cx = w / 2f;
        float cy = h * 0.55f;
        float burn = Math.Clamp(ctx.Parameters.Progress, 0f, 1f);

        if (burn <= 0.03f)
        {
            float er = w * 0.07f;   // burned out: a dim ember
            g.FillEllipse(s_ember, cx - er, cy - er, er * 2f, er * 2f);
            return;
        }

        // Amber halo, honey mid, hot core; alpha and size track burn.
        float baseR = w * 0.34f;
        DrawGlow(g, cx, cy, baseR * (0.85f + 0.15f * burn), Color.FromArgb((int)(80 * burn), 255, 130, 36));
        DrawGlow(g, cx, cy - h * 0.05f * burn, baseR * 0.62f, Color.FromArgb((int)(160 * burn), 255, 182, 86));
        DrawGlow(g, cx, cy - h * 0.09f * burn, baseR * 0.34f, Color.FromArgb((int)(235 * burn), 255, 242, 210));

        using var tongue = new SolidBrush(Color.FromArgb((int)(210 * burn), 255, 198, 112));
        PointF[] flame =
        [
            new(cx, cy - h * (0.16f + 0.14f * burn)),
            new(cx + w * 0.11f, cy),
            new(cx, cy + h * 0.07f),
            new(cx - w * 0.11f, cy),
        ];
        g.FillClosedCurve(tongue, (ReadOnlySpan<PointF>)flame, 0.35f);
    }

    private static void DrawNumeral(ID2DGraphics g, IClockRenderContext ctx)
    {
        int hour = ((ctx.Id.Index % 12) + 12) % 12;
        float w = ctx.ContentSize.Width;
        float h = ctx.ContentSize.Height;
        float burn = Math.Clamp(ctx.Parameters.Progress, 0f, 1f);

        // Brightens with its ember; a burned-out hour keeps a faint mark.
        Color color = burn <= 0.03f
            ? Color.FromArgb(60, 150, 120, 80)
            : Color.FromArgb(Math.Clamp((int)(70 + 185 * burn), 0, 255), 255, 224, 170);

        using var font = new Font("Segoe UI", h * 0.46f, FontStyle.Bold, GraphicsUnit.Pixel);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(s_marks[hour], font, color, new RectangleF(0, 0, w, h), format);
    }

    private static void DrawGlow(ID2DGraphics g, float cx, float cy, float r, Color color)
        => g.FillEllipse(color, cx - r, cy - r, r * 2f, r * 2f);

    private static void DrawHand(ID2DGraphics g, IClockRenderContext ctx, float widthFactor, float tipFactor)
    {
        float w = ctx.ContentSize.Width;
        float cx = ctx.Pivot.X;
        float baseY = ctx.Pivot.Y;
        float tipY = ctx.ContentSize.Height * 0.05f;
        float halfBase = w * widthFactor;
        float halfTip = w * tipFactor;

        using var gold = new SolidBrush(s_gold);
        g.FillPolygon(gold,
        [
            new PointF(cx - halfBase, baseY),
            new PointF(cx + halfBase, baseY),
            new PointF(cx + halfTip, tipY),
            new PointF(cx - halfTip, tipY),
        ]);

        using var goldDark = new SolidBrush(s_goldDark);
        float tailY = baseY + ctx.ContentSize.Height * 0.06f;
        g.FillPolygon(goldDark,
        [
            new PointF(cx - halfBase * 0.7f, baseY),
            new PointF(cx + halfBase * 0.7f, baseY),
            new PointF(cx, tailY),
        ]);
        g.FillEllipse(s_gold, cx - halfTip, tipY - halfTip, halfTip * 2f, halfTip * 2f);
    }

    /// <summary>Plain gold center hub.</summary>
    private static void DrawHub(ID2DGraphics g, IClockRenderContext ctx)
    {
        PointF c = ctx.Pivot;
        float w = ctx.ContentSize.Width;
        float r = w * 0.22f;

        g.FillEllipse(s_goldDark, c.X - r, c.Y - r, r * 2f, r * 2f);
        float ri = r * 0.62f;
        g.FillEllipse(s_gold, c.X - ri, c.Y - ri, ri * 2f, ri * 2f);
    }
}
