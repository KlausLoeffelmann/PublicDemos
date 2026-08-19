using System.ComponentModel;
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

internal sealed record ScatterThemeColorOverrides(
    Color? Face = null,
    Color? Hand = null,
    Color? MagnetFill = null,
    Color? MagnetRim = null,
    Color? Label = null);

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
        "Numerals, weekday/day badges, and timezone wander while the hands magnetically chase the face.";
    private const float DesignRadius = 500f;
    private const float HomeRadius = DesignRadius * 0.78f;

    private readonly ClockThemeVariantKind _variant;
    private ScatterThemeColorOverrides _overrides;
    private ScatterThemePalette _palette;

    public ScatterTheme()
        : this(ClockThemeVariantKind.Day)
    {
    }

    internal ScatterTheme(ClockThemeVariantKind variant)
        : this(variant, overrides: null)
    {
    }

    internal ScatterTheme(ClockThemeVariantKind variant, ScatterThemeColorOverrides? overrides)
    {
        if (!ClockThemeVariants.Supports(ClockThemeVariants.DayNightOled, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNightOled, variant);
        }

        _variant = variant;
        _overrides = overrides ?? new();
        _palette = CreatePalette(variant, _overrides);
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
    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => ClockThemeVariants.DayNightOled;

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Clock-Face Background")]
    [Description("Fill color for Scatter's clock-face background.")]
    public Color ClockFaceBackground
    {
        get => _palette.Face;
        set => SetOverrides(_overrides with { Face = value });
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Hands")]
    [Description("Primary hand color for the Scatter hour and minute hands.")]
    public Color Hands
    {
        get => _palette.Hand;
        set => SetOverrides(_overrides with { Hand = value });
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Numeral Background")]
    [Description("Background color for Scatter numerals and auxiliary badges.")]
    public Color NumeralBackground
    {
        get => _palette.MagnetFill;
        set => SetOverrides(_overrides with { MagnetFill = value });
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Numeral Border")]
    [Description("Border color for Scatter numerals and auxiliary badges.")]
    public Color NumeralBorder
    {
        get => _palette.MagnetRim;
        set => SetOverrides(_overrides with { MagnetRim = value });
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Numeral Foreground")]
    [Description("Foreground color for Scatter numerals and auxiliary badge text.")]
    public Color NumeralForeground
    {
        get => _palette.Label;
        set => SetOverrides(_overrides with { Label = value });
    }

    /// <inheritdoc/>
    public IClockTheme ResolveVariant(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(SupportedVariants, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, SupportedVariants, variant);
        }

        return variant == _variant ? this : new ScatterTheme(variant, _overrides);
    }

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new() { Id = ClockElementId.Face, ContentSize = new SizeF(1000, 1000), Pivot = new PointF(500, 500), ZOrder = 0 },
            new() { Id = ClockElementId.TimeZone, ContentSize = new SizeF(250, 78), Pivot = new PointF(125, 39), ZOrder = 18, RedrawPerFrame = true },
            new() { Id = ClockElementId.Day, ContentSize = new SizeF(120, 72), Pivot = new PointF(60, 36), ZOrder = 18, RedrawPerFrame = true },
            new() { Id = ClockElementId.Weekday, ContentSize = new SizeF(156, 72), Pivot = new PointF(78, 36), ZOrder = 18, RedrawPerFrame = true },
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

    internal ScatterThemePalette Palette => _palette;

    internal static PointF GetHomePosition(int index) => Polar(HomeRadius, index * 30f);

    internal static ScatterThemePalette CreatePalette(
        ClockThemeVariantKind variant,
        ScatterThemeColorOverrides? overrides = null)
    {
        ScatterThemePalette palette = variant switch
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
            ClockThemeVariantKind.OledDay => new ScatterThemePalette(
                Face: Color.FromArgb(12, 32, 86),
                MagnetFill: Color.FromArgb(152, 89, 104),
                MagnetRim: Color.FromArgb(104, 143, 219),
                Label: Color.FromArgb(232, 239, 250),
                Hand: Color.FromArgb(220, 228, 240),
                Second: Color.FromArgb(192, 120, 148),
                Arbour: Color.FromArgb(118, 157, 226)),
            ClockThemeVariantKind.OledNight => new ScatterThemePalette(
                Face: Color.Black,
                MagnetFill: Color.FromArgb(120, 74, 94),
                MagnetRim: Color.FromArgb(92, 116, 152),
                Label: Color.FromArgb(220, 230, 244),
                Hand: Color.FromArgb(205, 214, 227),
                Second: Color.FromArgb(162, 92, 120),
                Arbour: Color.FromArgb(104, 120, 146)),
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNightOled, variant),
        };

        if (overrides is null)
        {
            return palette;
        }

        return palette with
        {
            Face = overrides.Face ?? palette.Face,
            Hand = overrides.Hand ?? palette.Hand,
            MagnetFill = overrides.MagnetFill ?? palette.MagnetFill,
            MagnetRim = overrides.MagnetRim ?? palette.MagnetRim,
            Label = overrides.Label ?? palette.Label,
        };
    }

    private static PointF Polar(float radius, float angleDegrees)
    {
        float rad = angleDegrees * (MathF.PI / 180f);
        return new PointF(MathF.Sin(rad) * radius, -MathF.Cos(rad) * radius);
    }

    private void SetOverrides(ScatterThemeColorOverrides overrides)
    {
        _overrides = overrides;
        _palette = CreatePalette(_variant, overrides);
    }

    /// <summary>
    ///  Defers to the engine's default radial placement so the numerals begin in their
    ///  canonical positions and the auxiliary elements start at the engine's default
    ///  weekday/day/time-zone anchors. Motion is layered on with
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
    ///  Moves the numerals and auxiliary badges around with a small scheduler: never more
    ///  than <see cref="MaxConcurrentMoves"/> ordinary wanderers are started at once, yet
    ///  a time-zone change can retarget all numerals together so they flow into their new
    ///  counterpart positions.
    /// </summary>
    private sealed class ScatterAnimator : IThemeAnimator
    {
        private const int MaxConcurrentMoves = 4;
        private const int InvisibleNumeral = 9;
        private const float CounterpartMoveDurationSeconds = 1.8f;
        private const float FractionalShiftThreshold = 0.001f;

        private sealed class MoverState
        {
            public required ClockElementId Id { get; init; }

            public PointF Offset;
            public PointF MoveStart;
            public PointF MoveTarget;
            public bool Moving;
            public float Elapsed;
            public float Duration;
            public float Dwell;
            public SizeF WanderBounds;
        }

        private readonly MoverState[] _numerals = new MoverState[12];
        private readonly MoverState[] _auxiliaries =
        [
            new() { Id = ClockElementId.TimeZone, WanderBounds = new SizeF(170f, 70f) },
            new() { Id = ClockElementId.Day, WanderBounds = new SizeF(90f, 64f) },
            new() { Id = ClockElementId.Weekday, WanderBounds = new SizeF(110f, 64f) },
        ];

        private readonly Random _rng = new(0xC0FFEE);
        private bool _initialized;
        private double _visibilityPhase;

        public void Initialize(IClockTickContext context)
        {
            EnsureInitialized();
            ConfigureHandTargets(context);
        }

        public void OnTimeZoneChanged(
            IClockTickContext context,
            ClockTimeZoneSnapshot previous,
            ClockTimeZoneSnapshot current)
        {
            EnsureInitialized();
            ConfigureHandTargets(context);

            float deltaHours = (float)(current.UtcOffset - previous.UtcOffset).TotalHours;
            if (MathF.Abs(deltaHours) < FractionalShiftThreshold)
            {
                return;
            }

            PointF[] liveWorldPositions = new PointF[_numerals.Length];
            for (int i = 0; i < _numerals.Length; i++)
            {
                liveWorldPositions[i] = Add(ScatterTheme.GetHomePosition(i), _numerals[i].Offset);
            }

            for (int i = 0; i < _numerals.Length; i++)
            {
                MoverState state = _numerals[i];
                PointF targetWorld = SampleWorldPosition(liveWorldPositions, i - deltaHours);
                PointF home = ScatterTheme.GetHomePosition(i);
                PointF targetOffset = new(targetWorld.X - home.X, targetWorld.Y - home.Y);

                state.MoveStart = state.Offset;
                state.MoveTarget = targetOffset;
                state.Elapsed = 0f;
                state.Duration = CounterpartMoveDurationSeconds;
                state.Moving = Distance(state.MoveStart, targetOffset) >= 0.01f;
                state.Dwell = 3f + (float)_rng.NextDouble() * 3f;

                if (!state.Moving)
                {
                    state.Offset = targetOffset;
                }
            }
        }

        public void OnTick(IClockTickContext context)
        {
            EnsureInitialized();
            ConfigureHandTargets(context);

            float dt = (float)Math.Max(context.FrameDelta.TotalSeconds, 0d);
            int movingCount = 0;

            movingCount += UpdateStates(_numerals, dt, context);
            movingCount += UpdateStates(_auxiliaries, dt, context);

            ScheduleWanders(_numerals, dt, ref movingCount, PickNumeralTarget, 1.6f, 2.2f);
            ScheduleWanders(_auxiliaries, dt, ref movingCount, PickAuxiliaryTarget, 1.4f, 1.8f);

            UpdateVisibilityDemo(context, dt);
        }

        private static PointF Add(PointF left, PointF right) => new(left.X + right.X, left.Y + right.Y);

        private static float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt((dx * dx) + (dy * dy));
        }

        private static float EaseInOut(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return t < 0.5f ? 4f * t * t * t : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
        }

        private static PointF Lerp(PointF a, PointF b, float t)
            => new(
                a.X + ((b.X - a.X) * t),
                a.Y + ((b.Y - a.Y) * t));

        private static PointF SampleWorldPosition(PointF[] positions, float fractionalIndex)
        {
            float wrapped = fractionalIndex % positions.Length;
            if (wrapped < 0f)
            {
                wrapped += positions.Length;
            }

            int lower = (int)MathF.Floor(wrapped) % positions.Length;
            int upper = (lower + 1) % positions.Length;
            float t = wrapped - MathF.Floor(wrapped);
            return Lerp(positions[lower], positions[upper], t);
        }

        private void ConfigureHandTargets(IClockTickContext context)
        {
            context.GetParameters(ClockElementId.HourHand).HandTargetMode = ClockHandTargetMode.FreeFloating;
            context.GetParameters(ClockElementId.MinuteHand).HandTargetMode = ClockHandTargetMode.FreeFloating;
            context.GetParameters(ClockElementId.SecondHand).HandTargetMode = ClockHandTargetMode.FreeFloating;
        }

        private void EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }

            for (int i = 0; i < _numerals.Length; i++)
            {
                _numerals[i] = new MoverState
                {
                    Id = ClockElementId.HourMarker(i),
                    Dwell = 1.5f + ((float)_rng.NextDouble() * 6f),
                    WanderBounds = new SizeF(230f, 230f),
                };
            }

            foreach (MoverState state in _auxiliaries)
            {
                state.Dwell = 0.8f + ((float)_rng.NextDouble() * 4f);
            }

            _initialized = true;
        }

        private PointF PickAuxiliaryTarget(MoverState state)
        {
            if (_rng.Next(5) == 0)
            {
                return PointF.Empty;
            }

            float x = (((float)_rng.NextDouble() * 2f) - 1f) * state.WanderBounds.Width;
            float y = (((float)_rng.NextDouble() * 2f) - 1f) * state.WanderBounds.Height;
            return new PointF(x, y);
        }

        private PointF PickNumeralTarget(MoverState state)
        {
            if (_rng.Next(4) == 0)
            {
                return PointF.Empty;
            }

            int index = state.Id.Index;
            PointF home = ScatterTheme.GetHomePosition(index);

            float radius = 230f + ((float)_rng.NextDouble() * 230f);
            float angle = (float)_rng.NextDouble() * 360f;
            PointF target = Polar(radius, angle);
            return new PointF(target.X - home.X, target.Y - home.Y);
        }

        private void ScheduleWanders(
            IReadOnlyList<MoverState> states,
            float dt,
            ref int movingCount,
            Func<MoverState, PointF> targetPicker,
            float minimumDuration,
            float durationSpread)
        {
            for (int i = 0; i < states.Count && movingCount < MaxConcurrentMoves; i++)
            {
                MoverState state = states[i];
                if (state.Moving)
                {
                    continue;
                }

                state.Dwell -= dt;
                if (state.Dwell > 0f)
                {
                    continue;
                }

                PointF target = targetPicker(state);
                state.MoveStart = state.Offset;
                state.MoveTarget = target;
                state.Elapsed = 0f;
                state.Duration = minimumDuration + ((float)_rng.NextDouble() * durationSpread);
                state.Moving = Distance(state.MoveStart, target) >= 0.01f;
                state.Dwell = 2.3f + ((float)_rng.NextDouble() * 4.7f);

                if (state.Moving)
                {
                    movingCount++;
                }
                else
                {
                    state.Offset = target;
                }
            }
        }

        private int UpdateStates(IReadOnlyList<MoverState> states, float dt, IClockTickContext context)
        {
            int movingCount = 0;

            for (int i = 0; i < states.Count; i++)
            {
                MoverState state = states[i];
                if (state.Moving)
                {
                    state.Elapsed += dt;
                    float t = Math.Clamp(state.Elapsed / MathF.Max(state.Duration, 0.001f), 0f, 1f);
                    float e = EaseInOut(t);
                    state.Offset = Lerp(state.MoveStart, state.MoveTarget, e);

                    if (t >= 1f)
                    {
                        state.Moving = false;
                        state.Offset = state.MoveTarget;
                    }
                    else
                    {
                        movingCount++;
                    }
                }

                context.GetParameters(state.Id).AnchorOffset = state.Offset;
            }

            return movingCount;
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
    }
}
