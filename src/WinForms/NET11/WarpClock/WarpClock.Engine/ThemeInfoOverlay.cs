using System.Diagnostics;
using System.Drawing;

using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Engine;

/// <summary>
///  Renders the "<c>{theme name} - {author}</c>" overlay and runs its character-wise
///  fade animation. The text can sit statically, fade in/out at a fixed placement, or
///  alternate between the left and right screen edges (rotated 90°). Fade-in and fade-out
///  each pick one of several per-character reveal effects.
/// </summary>
/// <remarks>
///  This is a pure drawing/animation helper driven once per frame from the clock control's
///  foreground render pass; it owns no DirectComposition visuals.
/// </remarks>
internal sealed class ThemeInfoOverlay : IDisposable
{
    // Phase durations (seconds).
    private const float FadeInSeconds = 2.0f;
    private const float HoldSeconds = 10.0f;
    private const float FadeOutSeconds = 2.0f;
    private const float GapSeconds = 5.0f;

    // Per-character reveal window as a fraction of the phase (lets characters overlap).
    private const float RevealWindow = 0.4f;

    private enum Phase { FadeIn, Hold, FadeOut, Gap }

    private enum Effect { FirstToLast, LastToFirst, MidToOuter, OuterToMid, Random }

    private enum DrawKind { LeftRotated, RightRotated, Face }

    private readonly record struct Glyph(char Ch, float Along, int Line, int Index);

    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly Random _rng = new();

    private string _name = string.Empty;
    private string _author = string.Empty;

    // Cached layout.
    private readonly List<Glyph> _glyphs = [];
    private float[] _lineWidths = [];
    private int _lineCount;
    private Font? _font;
    private float _lineHeight;
    private string _cacheKey = string.Empty;

    // Animation state.
    private long _lastTimestamp;
    private Phase _phase = Phase.FadeIn;
    private float _phaseTime;
    private bool _rightSide;            // current side for the alternating mode
    private Effect _effectIn = Effect.FirstToLast;
    private Effect _effectOut = Effect.LastToFirst;
    private float[] _ordersIn = [];
    private float[] _ordersOut = [];
    private RenderThemeInfo _lastMode = RenderThemeInfo.Never;
    private bool _animationInitialized;

    /// <summary>Sets the theme name and author; resets the animation when they change.</summary>
    public void Configure(string themeName, string author)
    {
        if (_name == themeName && _author == author)
        {
            return;
        }

        _name = themeName ?? string.Empty;
        _author = author ?? string.Empty;
        _animationInitialized = false; // restart the cycle for the new text
    }

    /// <summary>Draws the overlay for this frame.</summary>
    public void Render(ID2DGraphics g, Size client, RenderThemeInfo mode, ThemeInfoPlacement placement, RectangleF faceBounds)
    {
        if (mode == RenderThemeInfo.Never || client.Width < 8 || client.Height < 8)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_name) && string.IsNullOrWhiteSpace(_author))
        {
            return;
        }

        bool twoLines = placement == ThemeInfoPlacement.OnClockFace
            && mode != RenderThemeInfo.FadeAlternateScreenSides;

        Size layoutArea = twoLines ? Size.Ceiling(faceBounds.Size) : client;
        if (layoutArea.Width < 8 || layoutArea.Height < 8)
        {
            return;
        }

        EnsureLayout(g, layoutArea, twoLines);
        if (_glyphs.Count == 0 || _font is null)
        {
            return;
        }

        DrawKind kind = ResolveDrawKind(mode, placement);

        // Compute the per-character opacity for this frame.
        Span<float> alpha = _glyphs.Count <= 128 ? stackalloc float[_glyphs.Count] : new float[_glyphs.Count];
        ComputeAlpha(mode, alpha);

        Draw(g, client, kind, alpha, faceBounds);
    }

    private DrawKind ResolveDrawKind(RenderThemeInfo mode, ThemeInfoPlacement placement)
    {
        if (mode == RenderThemeInfo.FadeAlternateScreenSides)
        {
            return _rightSide ? DrawKind.RightRotated : DrawKind.LeftRotated;
        }

        return placement switch
        {
            ThemeInfoPlacement.LeftScreenSide => DrawKind.LeftRotated,
            ThemeInfoPlacement.RightScreenSide => DrawKind.RightRotated,
            _ => DrawKind.Face,
        };
    }

    // ── Animation ──────────────────────────────────────────────────────────────────────

    private void ComputeAlpha(RenderThemeInfo mode, Span<float> alpha)
    {
        // The static mode is always fully visible.
        if (mode == RenderThemeInfo.FixedPosition)
        {
            alpha.Fill(1f);
            ResetTimebase();
            return;
        }

        AdvancePhases(mode);

        switch (_phase)
        {
            case Phase.Hold:
                alpha.Fill(1f);
                break;

            case Phase.Gap:
                alpha.Clear();
                break;

            case Phase.FadeIn:
            {
                float p = _phaseTime / FadeInSeconds;
                for (int i = 0; i < alpha.Length; i++)
                {
                    alpha[i] = Reveal(p, _ordersIn[i]);
                }

                break;
            }

            case Phase.FadeOut:
            {
                float p = _phaseTime / FadeOutSeconds;
                for (int i = 0; i < alpha.Length; i++)
                {
                    alpha[i] = 1f - Reveal(p, _ordersOut[i]);
                }

                break;
            }
        }
    }

    private void AdvancePhases(RenderThemeInfo mode)
    {
        long now = Stopwatch.GetTimestamp();
        float dt = _lastTimestamp == 0 ? 0f : (float)Stopwatch.GetElapsedTime(_lastTimestamp).TotalSeconds;
        _lastTimestamp = now;

        // (Re)initialize the cycle on first use or when the mode/text changed.
        if (!_animationInitialized || _lastMode != mode)
        {
            _lastMode = mode;
            _animationInitialized = true;
            _phase = Phase.FadeIn;
            _phaseTime = 0f;
            _rightSide = false;
            BeginFadeIn();
            return;
        }

        // Clamp pathological deltas (e.g. after the app was paused) so we don't skip a phase.
        dt = Math.Clamp(dt, 0f, 0.25f);
        _phaseTime += dt;

        switch (_phase)
        {
            case Phase.FadeIn when _phaseTime >= FadeInSeconds:
                _phase = Phase.Hold;
                _phaseTime = 0f;
                break;

            case Phase.Hold when _phaseTime >= HoldSeconds:
                _phase = Phase.FadeOut;
                _phaseTime = 0f;
                _effectOut = PickEffect(exclude: _effectIn);
                _ordersOut = BuildOrders(_effectOut, _glyphs.Count);
                break;

            case Phase.FadeOut when _phaseTime >= FadeOutSeconds:
                _phase = Phase.Gap;
                _phaseTime = 0f;
                break;

            case Phase.Gap when _phaseTime >= GapSeconds:
                // Next cycle: alternate the side (only matters for the alternating mode).
                _rightSide = !_rightSide;
                _phase = Phase.FadeIn;
                _phaseTime = 0f;
                BeginFadeIn();
                break;
        }
    }

    private void BeginFadeIn()
    {
        _effectIn = PickEffect(exclude: null);
        _ordersIn = BuildOrders(_effectIn, _glyphs.Count);
        _effectOut = PickEffect(exclude: _effectIn);
        _ordersOut = BuildOrders(_effectOut, _glyphs.Count);
    }

    private void ResetTimebase() => _lastTimestamp = Stopwatch.GetTimestamp();

    /// <summary>A character's opacity given the phase progress and its reveal order.</summary>
    private static float Reveal(float progress, float order)
    {
        float start = order * (1f - RevealWindow);
        return Math.Clamp((progress - start) / RevealWindow, 0f, 1f);
    }

    private Effect PickEffect(Effect? exclude)
    {
        Effect e;
        do
        {
            e = (Effect)_rng.Next(5);
        }
        while (exclude is { } x && e == x);

        return e;
    }

    /// <summary>Builds the per-character reveal order (0 = first revealed, 1 = last) for an effect.</summary>
    private float[] BuildOrders(Effect effect, int n)
    {
        var orders = new float[n];
        if (n == 0)
        {
            return orders;
        }

        float last = Math.Max(1, n - 1);
        float mid = last / 2f;

        switch (effect)
        {
            case Effect.FirstToLast:
                for (int i = 0; i < n; i++)
                {
                    orders[i] = i / last;
                }

                break;

            case Effect.LastToFirst:
                for (int i = 0; i < n; i++)
                {
                    orders[i] = (last - i) / last;
                }

                break;

            case Effect.MidToOuter:
                for (int i = 0; i < n; i++)
                {
                    orders[i] = MathF.Abs(i - mid) / MathF.Max(mid, 1f);
                }

                break;

            case Effect.OuterToMid:
                for (int i = 0; i < n; i++)
                {
                    orders[i] = 1f - MathF.Abs(i - mid) / MathF.Max(mid, 1f);
                }

                break;

            case Effect.Random:
                // A random permutation mapped into [0,1].
                int[] perm = new int[n];
                for (int i = 0; i < n; i++)
                {
                    perm[i] = i;
                }

                for (int i = n - 1; i > 0; i--)
                {
                    int j = _rng.Next(i + 1);
                    (perm[i], perm[j]) = (perm[j], perm[i]);
                }

                for (int i = 0; i < n; i++)
                {
                    orders[perm[i]] = i / last;
                }

                break;
        }

        return orders;
    }

    // ── Layout ─────────────────────────────────────────────────────────────────────────

    private void EnsureLayout(ID2DGraphics g, Size client, bool twoLines)
    {
        string key = $"{client.Width}x{client.Height}|{(twoLines ? "2" : "1")}|{_name}|{_author}";
        if (key == _cacheKey && _font is not null)
        {
            return;
        }

        _cacheKey = key;
        _glyphs.Clear();

        string[] lines = twoLines
            ? [_name, $"{_author}"]
            : [$"{_name} - {_author}"];

        // Pick a font size that fits the available length (the screen height for the rotated
        // side placements, the width for the on-face placement).
        const float probe = 48f;
        using var probeFont = new Font("Segoe UI", probe, FontStyle.Bold, GraphicsUnit.Pixel);

        float maxLineWidth = 1f;
        foreach (string line in lines)
        {
            maxLineWidth = MathF.Max(maxLineWidth, g.MeasureString(line, probeFont).Width);
        }

        float availAlong = twoLines ? client.Width * 0.70f : client.Height * 0.82f;
        float maxFont = twoLines ? client.Height * 0.06f : client.Width * 0.07f;
        float fontSize = Math.Clamp(probe * (availAlong / maxLineWidth), 11f, MathF.Max(12f, maxFont));

        _font?.Dispose();
        _font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        _lineHeight = g.MeasureString("Ag", _font).Height;
        _lineCount = lines.Length;
        _lineWidths = new float[lines.Length];

        int index = 0;
        for (int li = 0; li < lines.Length; li++)
        {
            string line = lines[li];
            _lineWidths[li] = MeasurePrefix(g, line, line.Length);

            for (int c = 0; c < line.Length; c++)
            {
                float along = MeasurePrefix(g, line, c);
                _glyphs.Add(new Glyph(line[c], along, li, index++));
            }
        }

        // Orders depend on the glyph count, which may have changed.
        _animationInitialized = false;
    }

    /// <summary>
    ///  Measures the advance width of the first <paramref name="count"/> characters of
    ///  <paramref name="line"/> in a way that is robust to trailing-whitespace trimming.
    /// </summary>
    /// <remarks>
    ///  Both GDI+ and DirectWrite drop the width of <i>trailing</i> spaces by default (and
    ///  the WARP DirectWrite-backed <c>MeasureString</c> ignores
    ///  <see cref="StringFormatFlags.MeasureTrailingSpaces"/>). When the prefix ends in a
    ///  space — e.g. positioning the 'C' in "Railway Classic" — that lost space width would
    ///  shift every following glyph one slot early ("RailwayC lassic"). Appending a sentinel
    ///  glyph makes any trailing spaces interior, so they are measured; we then subtract the
    ///  sentinel's own width to recover the true prefix advance.
    /// </remarks>
    private float MeasurePrefix(ID2DGraphics g, string line, int count)
    {
        if (count <= 0 || _font is null)
        {
            return 0f;
        }

        const string sentinel = "I";
        float sentinelWidth = g.MeasureString(sentinel, _font).Width;
        float withSentinel = g.MeasureString(string.Concat(line.AsSpan(0, count), sentinel), _font).Width;
        return MathF.Max(0f, withSentinel - sentinelWidth);
    }

    // ── Drawing ────────────────────────────────────────────────────────────────────────

    private void Draw(ID2DGraphics g, Size client, DrawKind kind, ReadOnlySpan<float> alpha, RectangleF faceBounds)
    {
        if (kind == DrawKind.Face)
        {
            DrawFace(g, faceBounds, alpha);
            return;
        }

        DrawRotatedSide(g, client, kind == DrawKind.RightRotated, alpha);
    }

    private void DrawFace(ID2DGraphics g, RectangleF faceBounds, ReadOnlySpan<float> alpha)
    {
        float totalHeight = _lineHeight * _lineCount;
        float topY = faceBounds.Top + faceBounds.Height / 2f - totalHeight / 2f;

        foreach (Glyph glyph in _glyphs)
        {
            float startX = faceBounds.Left + faceBounds.Width / 2f - _lineWidths[glyph.Line] / 2f;
            float x = startX + glyph.Along;
            float y = topY + glyph.Line * _lineHeight;
            DrawGlyph(g, glyph.Ch, x, y, alpha[glyph.Index]);
        }
    }

    private void DrawRotatedSide(ID2DGraphics g, Size client, bool rightSide, ReadOnlySpan<float> alpha)
    {
        float halfBand = _lineHeight / 2f;
        float total = _lineWidths.Length > 0 ? _lineWidths[0] : 0f;
        float margin = MathF.Max(10f, halfBand * 0.6f);

        g.ResetTransform();

        if (rightSide)
        {
            // Right edge, rotated 90° clockwise → reads top-to-bottom.
            g.TranslateTransform(client.Width - margin - halfBand, client.Height / 2f - total / 2f);
            g.RotateTransform(90f);
        }
        else
        {
            // Left edge, rotated 90° counter-clockwise → reads bottom-to-top.
            g.TranslateTransform(margin + halfBand, client.Height / 2f + total / 2f);
            g.RotateTransform(-90f);
        }

        foreach (Glyph glyph in _glyphs)
        {
            DrawGlyph(g, glyph.Ch, glyph.Along, -halfBand, alpha[glyph.Index]);
        }

        g.ResetTransform();
    }

    private void DrawGlyph(ID2DGraphics g, char ch, float x, float y, float a)
    {
        if (a <= 0.02f || ch == ' ' || _font is null)
        {
            return;
        }

        a = Math.Clamp(a, 0f, 1f);
        string s = ch.ToString();

        // A soft dark shadow keeps the text legible over any theme, then the bright glyph.
        Color shadow = Color.FromArgb((int)(a * 150), 8, 10, 14);
        Color fill = Color.FromArgb((int)(a * 255), 248, 248, 240);

        float o = MathF.Max(1.5f, _lineHeight * 0.03f);
        g.DrawString(s, _font, shadow, x + o, y + o);
        g.DrawString(s, _font, fill, x, y);
    }

    public void Dispose()
    {
        _font?.Dispose();
        _font = null;
    }
}
