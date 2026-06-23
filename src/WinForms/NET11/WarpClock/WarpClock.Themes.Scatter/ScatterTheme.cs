using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Scatter;

/// <summary>
///  A free-floating demo that throws the twelve hour numerals at random positions all
///  over the canvas — and a few of them keep drifting. It exists to show off the engine's
///  <b>Magnetic numerals</b> mode (toggle it from <i>View ▸ Magnetic numerals</i>): with
///  magnetism on, every hand "finds" the next hour numeral wherever it landed and swings
///  to it, so the second hand whips back and forth and never quite rests (because some
///  numerals keep moving). Two numerals demonstrate the tri-state visibility: one is
///  <see cref="ClockNumeralVisibility.Transparent"/> (hidden but still a magnet) and one is
///  <see cref="ClockNumeralVisibility.Invisible"/> (hidden and skipped — the hands jump
///  over it).
/// </summary>
public sealed class ScatterTheme : IClockTheme
{
    // Indices chosen to demonstrate the two non-Visible states.
    internal const int TransparentIndex = 4;  // hidden, but still a valid magnet target
    internal const int InvisibleIndex = 9;    // hidden AND skipped by the hands

    /// <inheritdoc/>
    public string Name => "Scatter (Magnetic)";

    /// <inheritdoc/>
    public string Description =>
        "Hour numerals scattered (and drifting) across the canvas — turn on View ▸ Magnetic numerals.";

    /// <inheritdoc/>
    public string Author => "WarpClock sample plug-in";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = new()
    {
        // Free-floating so the layout may place numerals anywhere on the surface.
        FreeFloating = true,
        HandsFollowFaceRotation = true,
    };

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new() { Id = ClockElementId.Face, ContentSize = new SizeF(1000, 1000), Pivot = new PointF(500, 500), ZOrder = 0 },
        };

        // Twelve hour numerals ("magnets"). Positions come from ScatterLayout.
        for (int i = 0; i < 12; i++)
        {
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(i),
                ContentSize = new SizeF(150, 150),
                Pivot = new PointF(75, 75),
                ZOrder = 20,
            });
        }

        // Three branch-free needles of distinct lengths (authored pointing up).
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.HourHand, ContentSize = new SizeF(46, 320), Pivot = new PointF(23, 264), Hand = ClockHandKind.Hour, ZOrder = 30 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.MinuteHand, ContentSize = new SizeF(34, 440), Pivot = new PointF(17, 372), Hand = ClockHandKind.Minute, ZOrder = 31 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.SecondHand, ContentSize = new SizeF(20, 500), Pivot = new PointF(10, 430), Hand = ClockHandKind.Second, ZOrder = 32 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.Arbour, ContentSize = new SizeF(64, 64), Pivot = new PointF(32, 32), ZOrder = 40 });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new ScatterLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new ScatterRenderer();

    /// <inheritdoc/>
    public IThemeAnimator CreateAnimator() => new ScatterAnimator();

    /// <summary>
    ///  Scatters the twelve hour numerals at stable pseudo-random positions across the
    ///  surface (a fixed seed keeps them put between frames; positions are recomputed only
    ///  when the surface size changes). Everything else defers to the engine's centered
    ///  default placement.
    /// </summary>
    private sealed class ScatterLayout : IClockLayout
    {
        private readonly PointF[] _positions = new PointF[12];
        private SizeF _cachedSurface;
        private bool _cached;

        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            if (id.Kind != ClockElementKind.HourMarker)
            {
                anchor = default;
                return false;
            }

            EnsurePositions(surface);
            anchor = _positions[((id.Index % 12) + 12) % 12];
            return true;
        }

        private void EnsurePositions(SizeF surface)
        {
            if (_cached && surface == _cachedSurface)
            {
                return;
            }

            _cachedSurface = surface;

            // Fixed seed → the same "random" scatter every run, so the demo is reproducible.
            var rng = new Random(20260623);
            float marginX = surface.Width * 0.10f;
            float marginY = surface.Height * 0.12f;

            for (int i = 0; i < 12; i++)
            {
                float x = marginX + (float)rng.NextDouble() * (surface.Width - 2f * marginX);
                float y = marginY + (float)rng.NextDouble() * (surface.Height - 2f * marginY);
                _positions[i] = new PointF(x, y);
            }

            _cached = true;
        }
    }

    /// <summary>
    ///  Sets the two demonstration visibility states once, then keeps a handful of numerals
    ///  gently drifting so that — with magnetism on — the hands keep chasing moving targets
    ///  and never settle.
    /// </summary>
    private sealed class ScatterAnimator : IThemeAnimator
    {
        private double _phase;

        public void Initialize(IClockTickContext context)
        {
            // One numeral hidden-but-targetable, one hidden-and-skipped.
            context.GetParameters(ClockElementId.HourMarker(TransparentIndex)).Visibility = ClockNumeralVisibility.Transparent;
            context.GetParameters(ClockElementId.HourMarker(InvisibleIndex)).Visibility = ClockNumeralVisibility.Invisible;
        }

        public void OnTick(IClockTickContext context)
        {
            _phase += context.FrameDelta.TotalSeconds;

            // Drift a few numerals along lazy Lissajous paths (design-unit offsets). The
            // magnetic hands track these live positions, so they keep moving too.
            DriftNumeral(context, 1, 70f, 0.6f, 0.0f, 50f, 0.9f, 1.1f);
            DriftNumeral(context, 6, 90f, 0.4f, 1.3f, 60f, 0.7f, 0.3f);
            DriftNumeral(context, 11, 60f, 0.8f, 0.5f, 80f, 0.5f, 2.0f);
        }

        private void DriftNumeral(
            IClockTickContext context,
            int index,
            float ampX, float freqX, float phaseX,
            float ampY, float freqY, float phaseY)
        {
            float ox = ampX * MathF.Sin((float)(_phase * freqX) + phaseX);
            float oy = ampY * MathF.Sin((float)(_phase * freqY) + phaseY);
            context.GetParameters(ClockElementId.HourMarker(index)).AnchorOffset = new PointF(ox, oy);
        }
    }
}
