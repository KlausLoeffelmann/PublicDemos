using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.SunFlower;

/// <summary>
///  Hand-draws the SunFlower theme: a layered sunflower dial, bee numerals, branch hands
///  and a woody center cap. Everything is drawn in each element's local pixel space
///  (already scaled by the engine), so sizes are expressed as fractions of the element's
///  content box and scale automatically with the window.
/// </summary>
internal sealed class SunFlowerRenderer : IClockElementRenderer
{
    // ── Palette ──────────────────────────────────────────────────────────────────────
    private static readonly Color s_sky = Color.FromArgb(255, 138, 196, 232);     // soft daytime sky
    private static readonly Color s_petalOuter = Color.FromArgb(255, 250, 200, 40);
    private static readonly Color s_petalInner = Color.FromArgb(255, 240, 150, 30);
    private static readonly Color s_petalShade = Color.FromArgb(255, 214, 138, 22);
    private static readonly Color s_seedDisc = Color.FromArgb(255, 96, 60, 30);
    private static readonly Color s_seedRim = Color.FromArgb(255, 70, 42, 20);
    private static readonly Color s_seedA = Color.FromArgb(255, 58, 36, 18);
    private static readonly Color s_seedB = Color.FromArgb(255, 120, 84, 40);

    private static readonly Color s_beeBody = Color.FromArgb(255, 247, 198, 52);
    private static readonly Color s_beeStripe = Color.FromArgb(255, 40, 32, 24);
    private static readonly Color s_beeHead = Color.FromArgb(255, 38, 30, 22);
    private static readonly Color s_beeWing = Color.FromArgb(150, 226, 240, 252);
    private static readonly Color s_beeWingEdge = Color.FromArgb(180, 170, 200, 224);
    private static readonly Color s_beeBadge = Color.FromArgb(255, 252, 248, 232);
    private static readonly Color s_beeNumber = Color.FromArgb(255, 60, 44, 24);
    private static readonly Color s_white = Color.FromArgb(255, 245, 245, 245);

    private static readonly Color s_bark = Color.FromArgb(255, 116, 78, 46);
    private static readonly Color s_barkDark = Color.FromArgb(255, 88, 58, 34);
    private static readonly Color s_leaf = Color.FromArgb(255, 86, 162, 70);
    private static readonly Color s_leafDark = Color.FromArgb(255, 60, 124, 52);

    public void DrawElement(ID2DGraphics g, IClockRenderContext ctx)
    {
        g.AntialiasMode = D2DAntialiasMode.PerPrimitive;

        switch (ctx.Id.Kind)
        {
            case ClockElementKind.Face:
                DrawSunflower(g, ctx);
                break;
            case ClockElementKind.HourMarker:
                DrawBee(g, ctx);
                break;
            case ClockElementKind.HourHand:
                DrawBranch(g, ctx, baseHalf: 0.085f, leafScale: 1.15f, twigCount: 2);
                break;
            case ClockElementKind.MinuteHand:
                DrawBranch(g, ctx, baseHalf: 0.065f, leafScale: 1.0f, twigCount: 3);
                break;
            case ClockElementKind.SecondHand:
                DrawBranch(g, ctx, baseHalf: 0.040f, leafScale: 0.7f, twigCount: 2);
                break;
            case ClockElementKind.Arbour:
                DrawArbour(g, ctx);
                break;
        }
    }

    // ── Sunflower dial ────────────────────────────────────────────────────────────────

    private static void DrawSunflower(ID2DGraphics g, IClockRenderContext ctx)
    {
        PointF c = ctx.Pivot;
        float r = 490f * ctx.Scale;

        // A round sky backdrop so the flower reads against the black window.
        g.FillEllipse(s_sky, c.X - r, c.Y - r, r * 2f, r * 2f);

        float outerInner = r * 0.50f;   // where outer petals begin
        float outerOuter = r * 0.985f;  // outer petal tips
        float innerInner = r * 0.46f;
        float innerOuter = r * 0.82f;
        float seedRadius = r * 0.52f;

        // Two interleaved rings of petals; the back ring is drawn first and slightly
        // rotated so the front ring fills the gaps for a fuller flower.
        using (var outerBrush = new SolidBrush(s_petalOuter))
        using (var shadeBrush = new SolidBrush(s_petalShade))
        using (var innerBrush = new SolidBrush(s_petalInner))
        {
            const int petals = 24;
            float step = 360f / petals;

            // Back ring (shaded), offset by half a step.
            for (int i = 0; i < petals; i++)
            {
                float a = i * step + step * 0.5f;
                g.FillClosedCurve(shadeBrush, (ReadOnlySpan<PointF>)Petal(c, outerInner * 0.96f, outerOuter, 40f * ctx.Scale, a), 0.3f);
            }

            // Front outer ring (bright).
            for (int i = 0; i < petals; i++)
            {
                float a = i * step;
                g.FillClosedCurve(outerBrush, (ReadOnlySpan<PointF>)Petal(c, outerInner, outerOuter, 42f * ctx.Scale, a), 0.3f);
            }

            // Inner ring of shorter, deeper-orange petals overlapping the seed head.
            for (int i = 0; i < petals; i++)
            {
                float a = i * step + step * 0.5f;
                g.FillClosedCurve(innerBrush, (ReadOnlySpan<PointF>)Petal(c, innerInner, innerOuter, 30f * ctx.Scale, a), 0.3f);
            }
        }

        // Seed head: a brown disc, a darker rim, then a phyllotaxis spray of seeds.
        g.FillEllipse(s_seedDisc, c.X - seedRadius, c.Y - seedRadius, seedRadius * 2f, seedRadius * 2f);
        using (var rim = new Pen(s_seedRim, 10f * ctx.Scale))
        {
            float rr = seedRadius - 5f * ctx.Scale;
            g.DrawEllipse(rim, new RectangleF(c.X - rr, c.Y - rr, rr * 2f, rr * 2f));
        }

        DrawSeeds(g, c, seedRadius * 0.94f, ctx.Scale);
    }

    /// <summary>Lays out seeds on a Fermat (sunflower) spiral using the golden angle.</summary>
    private static void DrawSeeds(ID2DGraphics g, PointF center, float maxRadius, float scale)
    {
        const int count = 360;
        const float goldenAngle = 137.507764f;

        using var a = new SolidBrush(s_seedA);
        using var b = new SolidBrush(s_seedB);

        for (int n = 1; n <= count; n++)
        {
            float t = n / (float)count;
            float radius = maxRadius * MathF.Sqrt(t);
            float angle = n * goldenAngle;
            PointF p = Polar(center, radius, angle);

            // Seeds grow a touch toward the rim; alternate two browns for texture.
            float dot = (1.4f + 2.2f * t) * scale;
            SolidBrush brush = (n & 1) == 0 ? a : b;
            g.FillEllipse(brush, p.X - dot, p.Y - dot, dot * 2f, dot * 2f);
        }
    }

    // ── Bee numeral ───────────────────────────────────────────────────────────────────

    private static void DrawBee(ID2DGraphics g, IClockRenderContext ctx)
    {
        float w = ctx.ContentSize.Width;
        float cx = w / 2f;
        float cy = ctx.ContentSize.Height / 2f;

        float abdHalfW = w * 0.21f;     // abdomen half-width
        float abdHalfH = w * 0.30f;     // abdomen half-height
        float abdCy = cy + w * 0.06f;   // abdomen sits a little low; head goes above

        // Wings first (behind the body), as two translucent angled teardrops.
        using (var wing = new SolidBrush(s_beeWing))
        using (var wingEdge = new Pen(s_beeWingEdge, 1.6f * ctx.Scale))
        {
            PointF lw = new(cx - w * 0.12f, abdCy - w * 0.16f);
            PointF rw = new(cx + w * 0.12f, abdCy - w * 0.16f);
            PointF[] left = Leaf(lw, w * 0.34f, w * 0.20f, -52f);
            PointF[] right = Leaf(rw, w * 0.34f, w * 0.20f, 52f);
            g.FillClosedCurve(wing, (ReadOnlySpan<PointF>)left, 0.3f);
            g.FillClosedCurve(wing, (ReadOnlySpan<PointF>)right, 0.3f);
            g.DrawPolygon(wingEdge, left);
            g.DrawPolygon(wingEdge, right);
        }

        // Abdomen.
        using (var body = new SolidBrush(s_beeBody))
        {
            g.FillEllipse(body, cx - abdHalfW, abdCy - abdHalfH, abdHalfW * 2f, abdHalfH * 2f);
        }

        // Three black stripes, kept inside the abdomen contour by sampling the ellipse width.
        using (var stripe = new SolidBrush(s_beeStripe))
        {
            float[] stripeYs = [-0.42f, -0.05f, 0.32f];
            foreach (float fy in stripeYs)
            {
                float dy = fy * abdHalfH;
                float halfAtY = abdHalfW * MathF.Sqrt(MathF.Max(0f, 1f - (dy / abdHalfH) * (dy / abdHalfH)));
                float sh = w * 0.045f;
                g.FillEllipse(stripe, cx - halfAtY, abdCy + dy - sh, halfAtY * 2f, sh * 2f);
            }
        }

        // Stinger.
        using (var sting = new SolidBrush(s_beeStripe))
        {
            float ty = abdCy + abdHalfH;
            g.FillPolygon(sting,
            [
                new PointF(cx - w * 0.03f, ty - w * 0.02f),
                new PointF(cx + w * 0.03f, ty - w * 0.02f),
                new PointF(cx, ty + w * 0.07f),
            ]);
        }

        // Head with eyes and antennae.
        float headR = w * 0.14f;
        float headCy = abdCy - abdHalfH - headR * 0.55f;
        using (var head = new SolidBrush(s_beeHead))
        using (var eye = new SolidBrush(s_white))
        using (var antenna = new Pen(s_beeHead, 2.4f * ctx.Scale))
        {
            // Antennae (drawn from the head, curving up and out).
            g.DrawLine(antenna, cx - headR * 0.4f, headCy - headR * 0.5f, cx - headR * 1.1f, headCy - headR * 1.6f);
            g.DrawLine(antenna, cx + headR * 0.4f, headCy - headR * 0.5f, cx + headR * 1.1f, headCy - headR * 1.6f);
            float tip = w * 0.022f;
            g.FillEllipse(s_beeHead, cx - headR * 1.1f - tip, headCy - headR * 1.6f - tip, tip * 2f, tip * 2f);
            g.FillEllipse(s_beeHead, cx + headR * 1.1f - tip, headCy - headR * 1.6f - tip, tip * 2f, tip * 2f);

            g.FillEllipse(head, cx - headR, headCy - headR, headR * 2f, headR * 2f);

            float er = w * 0.028f;
            g.FillEllipse(eye, cx - headR * 0.45f - er, headCy - er, er * 2f, er * 2f);
            g.FillEllipse(eye, cx + headR * 0.45f - er, headCy - er, er * 2f, er * 2f);
        }

        // Number badge on the abdomen so the bee stays clock-readable.
        DrawBeeNumber(g, ctx, cx, abdCy, w);
    }

    private static void DrawBeeNumber(ID2DGraphics g, IClockRenderContext ctx, float cx, float cy, float w)
    {
        float badgeR = w * 0.16f;
        using (var badge = new SolidBrush(s_beeBadge))
        {
            g.FillEllipse(badge, cx - badgeR, cy - badgeR, badgeR * 2f, badgeR * 2f);
        }

        int index = ((ctx.Id.Index % 12) + 12) % 12;
        string text = (index == 0 ? 12 : index).ToString();

        float fontSize = w * 0.20f;
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, s_beeNumber, new RectangleF(cx - badgeR, cy - badgeR, badgeR * 2f, badgeR * 2f), format);
    }

    // ── Branch hands ──────────────────────────────────────────────────────────────────

    private static void DrawBranch(ID2DGraphics g, IClockRenderContext ctx, float baseHalf, float leafScale, int twigCount)
    {
        float w = ctx.ContentSize.Width;
        float h = ctx.ContentSize.Height;
        float cx = w / 2f;
        float bottomY = ctx.Pivot.Y + h * 0.04f;
        float topY = h * 0.05f;
        float bhw = w * baseHalf;          // base half-width of the stem
        float bend = w * 0.06f;            // a gentle sideways bend for an organic look

        // Tapered, slightly bent stem as a smooth closed curve.
        float midY = (bottomY + topY) * 0.5f;
        using (var bark = new SolidBrush(s_bark))
        {
            PointF[] stem =
            [
                new PointF(cx - bhw, bottomY),
                new PointF(cx - bhw * 0.55f + bend, midY),
                new PointF(cx - bhw * 0.22f + bend, topY + (bottomY - topY) * 0.12f),
                new PointF(cx, topY),
                new PointF(cx + bhw * 0.22f + bend, topY + (bottomY - topY) * 0.12f),
                new PointF(cx + bhw * 0.55f + bend, midY),
                new PointF(cx + bhw, bottomY),
            ];
            g.FillClosedCurve(bark, (ReadOnlySpan<PointF>)stem, 0.25f);
        }

        // Twigs with a leaf at each tip, alternating sides up the stem.
        using var twigPen = new Pen(s_barkDark, MathF.Max(2f, bhw * 0.5f));
        for (int i = 0; i < twigCount; i++)
        {
            float t = (i + 1) / (float)(twigCount + 1);          // position along the stem
            float y = bottomY + (topY - bottomY) * t;
            float side = (i % 2 == 0) ? 1f : -1f;
            float baseX = cx + bend * t;

            float twigLen = w * 0.30f * (0.8f + 0.4f * t);
            float twigAngle = side * (38f + 12f * t);            // degrees from straight up
            PointF start = new(baseX, y);
            PointF end = Offset(start, twigLen, twigAngle);

            g.DrawLine(twigPen, start.X, start.Y, end.X, end.Y);
            DrawLeaf(g, end, w * 0.26f * leafScale, w * 0.13f * leafScale, twigAngle, (i % 2 == 0) ? s_leaf : s_leafDark);
        }

        // A leaf cluster crowning the tip.
        DrawLeaf(g, new PointF(cx + bend, topY + h * 0.02f), w * 0.24f * leafScale, w * 0.12f * leafScale, 0f, s_leaf);
    }

    private static void DrawLeaf(ID2DGraphics g, PointF tipBase, float length, float width, float angleDeg, Color color)
    {
        using var brush = new SolidBrush(color);
        using var vein = new Pen(Color.FromArgb(140, 30, 80, 28), MathF.Max(1.2f, length * 0.04f));

        PointF[] leaf = Leaf(Offset(tipBase, length * 0.5f, angleDeg), length, width, angleDeg);
        g.FillClosedCurve(brush, (ReadOnlySpan<PointF>)leaf, 0.3f);

        // A central vein for a little realism.
        PointF a = Offset(tipBase, 0f, angleDeg);
        PointF b = Offset(tipBase, length, angleDeg);
        g.DrawLine(vein, a.X, a.Y, b.X, b.Y);
    }

    private static void DrawArbour(ID2DGraphics g, IClockRenderContext ctx)
    {
        PointF c = ctx.Pivot;
        float r = ctx.ContentSize.Width / 2f;

        g.FillEllipse(s_bark, c.X - r, c.Y - r, r * 2f, r * 2f);
        using (var rim = new Pen(s_barkDark, r * 0.22f))
        {
            float rr = r * 0.8f;
            g.DrawEllipse(rim, new RectangleF(c.X - rr, c.Y - rr, rr * 2f, rr * 2f));
        }

        // A tiny sprout so the hub looks alive.
        DrawLeaf(g, new PointF(c.X, c.Y - r * 0.2f), r * 0.9f, r * 0.5f, 18f, s_leaf);
    }

    // ── Geometry helpers ────────────────────────────────────────────────────────────────

    /// <summary>A point at clock-angle <paramref name="angleDeg"/> (clockwise from up) and radius from <paramref name="c"/>.</summary>
    private static PointF Polar(PointF c, float radius, float angleDeg)
    {
        float rad = angleDeg * (MathF.PI / 180f);
        return new PointF(c.X + MathF.Sin(rad) * radius, c.Y - MathF.Cos(rad) * radius);
    }

    /// <summary>A point offset from <paramref name="origin"/> by <paramref name="distance"/> along a clock-angle.</summary>
    private static PointF Offset(PointF origin, float distance, float angleDeg) => Polar(origin, distance, angleDeg);

    /// <summary>
    ///  Builds a petal outline (for <c>FillClosedCurve</c>) pointing along clock-angle
    ///  <paramref name="angleDeg"/>, spanning radii <paramref name="innerR"/>..<paramref name="outerR"/>
    ///  from <paramref name="c"/> with the given lateral half-width.
    /// </summary>
    private static PointF[] Petal(PointF c, float innerR, float outerR, float halfWidth, float angleDeg)
    {
        float rad = angleDeg * (MathF.PI / 180f);
        float rx = MathF.Sin(rad), ry = -MathF.Cos(rad);   // radial (outward) direction
        float tx = MathF.Cos(rad), ty = MathF.Sin(rad);    // tangential (sideways) direction
        float len = outerR - innerR;

        PointF P(float r, float w) => new(c.X + rx * r + tx * w, c.Y + ry * r + ty * w);

        return
        [
            P(innerR, 0f),
            P(innerR + len * 0.18f, halfWidth),
            P(innerR + len * 0.55f, halfWidth * 0.82f),
            P(outerR, 0f),
            P(innerR + len * 0.55f, -halfWidth * 0.82f),
            P(innerR + len * 0.18f, -halfWidth),
        ];
    }

    /// <summary>Builds a teardrop leaf/wing outline centered on <paramref name="center"/>, aligned to a clock-angle.</summary>
    private static PointF[] Leaf(PointF center, float length, float width, float angleDeg)
    {
        float rad = angleDeg * (MathF.PI / 180f);
        float dx = MathF.Sin(rad), dy = -MathF.Cos(rad);   // long axis
        float px = MathF.Cos(rad), py = MathF.Sin(rad);    // short axis
        float half = length * 0.5f;

        PointF P(float along, float across) => new(center.X + dx * along + px * across, center.Y + dy * along + py * across);

        return
        [
            P(half, 0f),                 // tip
            P(half * 0.35f, width * 0.5f),
            P(-half * 0.45f, width * 0.42f),
            P(-half, 0f),                // stalk end
            P(-half * 0.45f, -width * 0.42f),
            P(half * 0.35f, -width * 0.5f),
        ];
    }
}
