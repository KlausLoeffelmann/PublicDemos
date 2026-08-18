using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed record ScatterThemePalette(
    Color Face,
    Color MagnetFill,
    Color MagnetRim,
    Color Label,
    Color Hand,
    Color Second,
    Color Arbour);

/// <summary>
///  A demo for the engine's <b>Magnetic numerals</b> mode (which this theme turns on by
///  default). The twelve hour numerals begin in their normal clock positions and then
///  wander: at most four at a time slide smoothly to a new spot somewhere on the dial and
///  rest there before drifting again. Because the numerals are the "leading" instances the
///  hands point at, every hand behaves like a compass needle — it sweeps to wherever the
///  next numeral now sits and then keeps following it. One numeral periodically goes
///  <see cref="ClockNumeralVisibility.Invisible"/> to show that the hands skip a missing
///  numeral and stay put.
/// </summary>
public sealed class ScatterTheme : IClockTheme
{
    private const string BaseName = "Scatter (Magnetic)";
    private const string BaseDescription =
        "Numerals start home, then wander (max 4 at a time) while the hands magnetically chase them.";
    private const float DesignRadius = 500f;
    private const float HomeRadius = DesignRadius * 0.78f;

    private readonly ClockThemeVariantKind _variant;
    private readonly ScatterThemePalette _palette;

    public ScatterTheme()
        : this(ClockThemeVariantKind.Day)
    {
    }

    internal ScatterTheme(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(ClockThemeVariants.DayNight, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant);
        }

        _variant = variant;
        _palette = CreatePalette(variant);
    }

    /// <inheritdoc/>
    public string Name => ClockThemeVariants.FormatDisplayName(BaseName, _variant);

    /// <inheritdoc/>
    public string Description => BaseDescription;

    /// <inheritdoc/>
    public string Author => "Klaus Loeffelmann";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = new()
    {
        FreeFloating = true,
        HandsFollowFaceRotation = true,
        MagneticByDefault = true,
    };

    /// <inheritdoc/>
    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => ClockThemeVariants.DayNight;

    /// <inheritdoc/>
    public IClockTheme ResolveVariant(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(SupportedVariants, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, SupportedVariants, variant);
        }

        return variant == _variant ? this : new ScatterTheme(variant);
    }

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new() { Id = ClockElementId.Face, ContentSize = new SizeF(1000, 1000), Pivot = new PointF(500, 500), ZOrder = 0 },
        };

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

        elements.Add(new ClockElementDescriptor { Id = ClockElementId.HourHand, ContentSize = new SizeF(46, 320), Pivot = new PointF(23, 264), Hand = ClockHandKind.Hour, ZOrder = 30 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.MinuteHand, ContentSize = new SizeF(34, 440), Pivot = new PointF(17, 372), Hand = ClockHandKind.Minute, ZOrder = 31 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.SecondHand, ContentSize = new SizeF(20, 500), Pivot = new PointF(10, 430), Hand = ClockHandKind.Second, ZOrder = 32 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.Arbour, ContentSize = new SizeF(64, 64), Pivot = new PointF(32, 32), ZOrder = 40 });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new HomeLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new ScatterRenderer(_palette);

    /// <inheritdoc/>
    public IThemeAnimator CreateAnimator() => new ScatterAnimator();

    private static ScatterThemePalette CreatePalette(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new ScatterThemePalette(
                Face: Color.FromArgb(244, 244, 240),
                MagnetFill: Color.FromArgb(176, 94, 88),
                MagnetRim: Color.FromArgb(214, 184, 132),
                Label: Color.FromArgb(43, 45, 50),
                Hand: Color.FromArgb(68, 73, 82),
                Second: Color.FromArgb(181, 116, 71),
                Arbour: Color.FromArgb(196, 167, 120)),
            ClockThemeVariantKind.Night => new ScatterThemePalette(
                Face: Color.FromArgb(18, 20, 26),
                MagnetFill: Color.FromArgb(146, 82, 96),
                MagnetRim: Color.FromArgb(110, 126, 148),
                Label: Color.FromArgb(226, 229, 236),
                Hand: Color.FromArgb(214, 216, 221),
                Second: Color.FromArgb(176, 106, 126),
                Arbour: Color.FromArgb(132, 141, 156)),
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant),
        };

    /// <summary>
    ///  Defers to the engine's default radial placement, so every numeral starts in its
    ///  normal clock position. All movement is then layered on via the animator's
    ///  <see cref="ClockElementParameters.AnchorOffset"/>.
    /// </summary>
    private sealed class HomeLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            anchor = default;
            return false;
        }
    }

    /// <summary>
    ///  Moves the numerals around with a small scheduler: never more than
    ///  <see cref="MaxConcurrentMoves"/> numerals are in motion at once, each easing
    ///  smoothly from its current offset to a freshly chosen target offset and then
    ///  resting for a random dwell before becoming eligible to move again.
    /// </summary>
    private sealed class ScatterAnimator : IThemeAnimator
    {
        private const int MaxConcurrentMoves = 4;
        private const int InvisibleNumeral = 9;

        private sealed class NumeralState
        {
            public PointF Offset;
            public PointF MoveStart;
            public PointF MoveTarget;
            public bool Moving;
            public float Elapsed;
            public float Duration;
            public float Dwell;
        }

        private readonly NumeralState[] _numerals = new NumeralState[12];
        private readonly Random _rng = new(0xC0FFEE);
        private double _visibilityPhase;

        public void Initialize(IClockTickContext context)
        {
            for (int i = 0; i < 12; i++)
            {
                _numerals[i] = new NumeralState { Dwell = 1.5f + (float)_rng.NextDouble() * 6f };
            }
        }

        public void OnTick(IClockTickContext context)
        {
            float dt = (float)context.FrameDelta.TotalSeconds;
            int movingCount = 0;

            for (int i = 0; i < 12; i++)
            {
                NumeralState n = _numerals[i];
                if (n.Moving)
                {
                    n.Elapsed += dt;
                    float t = Math.Clamp(n.Elapsed / MathF.Max(n.Duration, 0.001f), 0f, 1f);
                    float e = EaseInOut(t);
                    n.Offset = new PointF(
                        n.MoveStart.X + (n.MoveTarget.X - n.MoveStart.X) * e,
                        n.MoveStart.Y + (n.MoveTarget.Y - n.MoveStart.Y) * e);

                    if (t >= 1f)
                    {
                        n.Moving = false;
                        n.Offset = n.MoveTarget;
                        n.Dwell = 2.5f + (float)_rng.NextDouble() * 5f;
                    }
                    else
                    {
                        movingCount++;
                    }
                }

                context.GetParameters(ClockElementId.HourMarker(i)).AnchorOffset = n.Offset;
            }

            for (int i = 0; i < 12 && movingCount < MaxConcurrentMoves; i++)
            {
                NumeralState n = _numerals[i];
                if (n.Moving)
                {
                    continue;
                }

                n.Dwell -= dt;
                if (n.Dwell <= 0f)
                {
                    StartMove(i, n);
                    movingCount++;
                }
            }

            UpdateVisibilityDemo(context, dt);
        }

        private void StartMove(int index, NumeralState n)
        {
            n.MoveStart = n.Offset;
            n.MoveTarget = PickTargetOffset(index);
            n.Elapsed = 0f;
            n.Duration = 1.6f + (float)_rng.NextDouble() * 2.2f;
            n.Moving = true;
        }

        private PointF PickTargetOffset(int index)
        {
            if (_rng.Next(4) == 0)
            {
                return PointF.Empty;
            }

            float homeAngle = index * 30f;
            PointF home = Polar(HomeRadius, homeAngle);

            float radius = 230f + (float)_rng.NextDouble() * 230f;
            float angle = (float)_rng.NextDouble() * 360f;
            PointF target = Polar(radius, angle);
            return new PointF(target.X - home.X, target.Y - home.Y);
        }

        private void UpdateVisibilityDemo(IClockTickContext context, float dt)
        {
            _visibilityPhase += dt;
            bool hidden = (_visibilityPhase % 18.0) >= 12.0;
            context.GetParameters(ClockElementId.HourMarker(InvisibleNumeral)).Visibility =
                hidden ? ClockNumeralVisibility.Invisible : ClockNumeralVisibility.Visible;
        }

        private static PointF Polar(float radius, float angleDegrees)
        {
            float rad = angleDegrees * (MathF.PI / 180f);
            return new PointF(MathF.Sin(rad) * radius, -MathF.Cos(rad) * radius);
        }

        private static float EaseInOut(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return t < 0.5f ? 4f * t * t * t : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
        }
    }
}
