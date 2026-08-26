using System.Drawing;

using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.Engine.Tests;

/// <summary>
///  Aiming coverage for the Scatter theme across the whole pipeline the renderer runs:
///  the real animator wanders the numerals, the real
///  <see cref="ClockElementAnchorResolver"/> reports where they landed, and the real
///  <see cref="HandRotationSolver"/> aims the hands.
/// </summary>
/// <remarks>
///  Asserting the theme's <see cref="ClockElementParameters.HandTargetMode"/> alone is
///  not enough: the regression these tests guard was a theme that asked for
///  <see cref="ClockHandTargetMode.FreeFloating"/> and got radial-looking minute
///  pointing, because free-floating minute targeting resolves the engine's default
///  minute-tick ring that Scatter never materializes.
/// </remarks>
public sealed class ScatterMagneticAimingTests
{
    private static readonly DateTime Start = new(2026, 8, 19, 10, 0, 0, DateTimeKind.Unspecified);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScatterHands_AimAtLiveHourNumerals_RegardlessOfTheGlobalMagneticSwitch(bool magneticNumeralsEnabled)
    {
        ScatterAimingRig rig = new(magneticNumeralsEnabled);

        // Shuffling begins 15 simulated seconds in; three minutes leaves the numerals
        // well away from their radial home positions.
        rig.AdvanceTo(Start.AddMinutes(3));

        Assert.Equal(ClockHandTargetMode.MagneticNumerals, rig.EffectiveTargetMode(ClockElementId.HourHand));
        Assert.Equal(ClockHandTargetMode.MagneticNumerals, rig.EffectiveTargetMode(ClockElementId.MinuteHand));
        Assert.Equal(ClockHandTargetMode.Radial, rig.EffectiveTargetMode(ClockElementId.SecondHand));

        // Both hands use their current live numeral plus authoritative clockwise
        // compensation within that numeral's interval.
        AssertNumeralHasWandered(rig, 0);
        AssertNumeralHasWandered(rig, 10);
        AssertAngle(rig.ExpectedMagneticAngle(ClockHandKind.Minute), rig.MinuteRotation);
        AssertAngle(rig.ExpectedMagneticAngle(ClockHandKind.Hour), rig.HourRotation);

        // The second hand keeps the authoritative radial angle and never consults a numeral.
        AssertAngle(rig.ExpectedRadialAngle(ClockHandKind.Second), rig.SecondRotation);

        // Free-floating minute targeting — the mode this theme used to request — would
        // have produced the engine's default minute-tick ring instead.
        float freeFloatingMinute = rig.FreeFloatingMinuteAngle();
        Assert.True(
            MathF.Abs(ClockMath.ShortestDelta(rig.MinuteRotation, freeFloatingMinute)) > 5f,
            $"The magnetic minute angle {rig.MinuteRotation} must differ from the free-floating "
                + $"minute-tick angle {freeFloatingMinute}.");
    }

    [Fact]
    public void ScatterMinuteHand_StepsToTheNextLiveNumeralEveryFiveMinutes()
    {
        ScatterAimingRig rig = new(magneticNumeralsEnabled: false);

        (int Minute, int Numeral)[] samples =
        [
            (3, 0),
            (7, 1),
            (12, 2),
            (23, 4),
            (58, 11),
        ];

        foreach ((int minute, int numeral) in samples)
        {
            rig.AdvanceTo(Start.AddMinutes(minute));

            AssertNumeralHasWandered(rig, numeral);
            Assert.Equal(numeral, MagneticNumeralPosition.Resolve(ClockHandKind.Minute, rig.Time).NumeralIndex);
            AssertAngle(rig.ExpectedMagneticAngle(ClockHandKind.Minute), rig.MinuteRotation);
            AssertAngle(rig.ExpectedRadialAngle(ClockHandKind.Second), rig.SecondRotation);
            Assert.Equal(ClockHandTargetMode.Radial, rig.EffectiveTargetMode(ClockElementId.SecondHand));
        }
    }

    [Fact]
    public void ScatterMinuteHand_HoldsItsNumeralWhileTheNumeralKeepsMoving()
    {
        ScatterAimingRig rig = new(magneticNumeralsEnabled: false);

        // Both instants map to numeral 2 (minutes 10..14), so any change in the minute
        // hand's angle can only come from the numeral itself having moved.
        rig.AdvanceTo(Start.AddMinutes(10).AddSeconds(30));
        PointF firstAnchor = rig.NumeralAnchor(2);
        float firstAngle = rig.MinuteRotation;

        rig.AdvanceTo(Start.AddMinutes(14).AddSeconds(30));
        PointF secondAnchor = rig.NumeralAnchor(2);

        AssertAngle(rig.ExpectedMagneticAngle(ClockHandKind.Minute), rig.MinuteRotation);
        Assert.True(
            Distance(firstAnchor, secondAnchor) > 1f,
            "Numeral 2 must have moved between the two samples for this test to prove tracking.");
        Assert.True(
            MathF.Abs(ClockMath.ShortestDelta(firstAngle, rig.MinuteRotation)) > 0.01f,
            "The minute hand must follow its numeral's live anchor between five-minute steps.");
    }

    private static void AssertAngle(float expected, float actual, float toleranceDegrees = 0.05f)
        => Assert.True(
            MathF.Abs(ClockMath.ShortestDelta(expected, actual)) <= toleranceDegrees,
            $"Expected {expected}° but found {actual}° (tolerance {toleranceDegrees}°).");

    private static void AssertNumeralHasWandered(ScatterAimingRig rig, int numeral)
        => Assert.True(
            Distance(rig.NumeralOffset(numeral), PointF.Empty) > 25f,
            $"Numeral {numeral} must be away from its home position so the assertion proves "
                + "live-anchor aiming rather than radial aiming.");

    private static float Distance(PointF left, PointF right)
    {
        float dx = left.X - right.X;
        float dy = left.Y - right.Y;
        return MathF.Sqrt((dx * dx) + (dy * dy));
    }

    /// <summary>
    ///  A headless stand-in for <see cref="WarpClockControl"/>'s per-frame element loop:
    ///  it feeds the theme animator the same tick context the engine uses and then aims
    ///  the three hands through the same anchor resolver and hand-rotation solver.
    /// </summary>
    private sealed class ScatterAimingRig
    {
        private const float GraceSeconds = 5f;
        private const float GlideDurationSeconds = 0.5f;
        private static readonly TimeSpan FrameStep = TimeSpan.FromSeconds(0.25d);

        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];
        private readonly HashSet<ClockElementId> _declared;
        private readonly ScatterTheme _theme = new();
        private readonly IClockLayout _layout;
        private readonly IThemeAnimator _animator;
        private readonly ThemeTickContext _context;
        private readonly HandRotationSolver _rotation = new();
        private readonly ClockGeometry _geometry = ClockGeometry.ForSurface(new SizeF(1000f, 1000f));
        private readonly bool _magneticNumeralsEnabled;

        public ScatterAimingRig(bool magneticNumeralsEnabled)
        {
            _magneticNumeralsEnabled = magneticNumeralsEnabled;
            _layout = _theme.CreateLayout();
            _animator = _theme.CreateAnimator();

            IReadOnlyList<ClockElementDescriptor> elements = _theme.CreateElements();
            _declared = [.. elements.Select(element => element.Id)];

            Now = Start;
            _context = new ThemeTickContext(elements, ParametersFor)
            {
                SurfaceSize = _geometry.Surface,
                Time = CreateTime(Now),
                TimeZone = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Utc, Now),
            };

            _animator.Initialize(_context);
        }

        public DateTime Now { get; private set; }

        public ClockTimeSnapshot Time => _context.Time;

        public float HourRotation { get; private set; }

        public float MinuteRotation { get; private set; }

        public float SecondRotation { get; private set; }

        public void AdvanceTo(DateTime target)
        {
            while (Now < target)
            {
                Now += FrameStep;
                _context.Time = CreateTime(Now);
                _context.FrameDelta = FrameStep;
                _animator.OnTick(_context);

                float dt = (float)FrameStep.TotalSeconds;
                HourRotation = Solve(ClockHandKind.Hour, ClockElementId.HourHand, dt);
                MinuteRotation = Solve(ClockHandKind.Minute, ClockElementId.MinuteHand, dt);
                SecondRotation = Solve(ClockHandKind.Second, ClockElementId.SecondHand, dt);
            }
        }

        public ClockHandTargetMode EffectiveTargetMode(ClockElementId id)
            => HandRotationSolver.ResolveTargetMode(BuildRequest(HandOf(id), id, dt: 0f));

        public PointF NumeralAnchor(int index) => AnchorOf(ClockElementId.HourMarker(index));

        public PointF NumeralOffset(int index) => ParametersFor(ClockElementId.HourMarker(index)).AnchorOffset;

        public float AngleToNumeral(int index)
            => ClockMath.AngleTo(AnchorOf(ClockElementId.MinuteHand), NumeralAnchor(index));

        public float ExpectedMagneticAngle(ClockHandKind hand)
        {
            MagneticNumeralPosition position = MagneticNumeralPosition.Resolve(
                hand,
                _context.Time,
                ClockHandMotion.Crawling,
                GlideDurationSeconds);
            ClockElementId handId = hand switch
            {
                ClockHandKind.Hour => ClockElementId.HourHand,
                ClockHandKind.Minute => ClockElementId.MinuteHand,
                ClockHandKind.Second => ClockElementId.SecondHand,
                _ => throw new ArgumentOutOfRangeException(nameof(hand)),
            };

            return ClockMath.Normalize360(
                ClockMath.AngleTo(AnchorOf(handId), NumeralAnchor(position.NumeralIndex))
                + position.CompensationDegrees);
        }

        public float ExpectedRadialAngle(ClockHandKind hand)
            => HandPointingSolver.RadialTargetAngle(
                _context.Time,
                hand,
                ClockHandMotion.Crawling,
                GlideDurationSeconds);

        /// <summary>The angle the discarded free-floating minute targeting would produce.</summary>
        public float FreeFloatingMinuteAngle()
            => HandPointingSolver.FreeFloatingTargetAngle(
                ClockHandKind.Minute,
                AnchorOf(ClockElementId.MinuteHand),
                _context.Time,
                ClockHandMotion.Crawling,
                GlideDurationSeconds,
                AnchorOf);

        private static ClockHandKind HandOf(ClockElementId id)
            => id.Kind switch
            {
                ClockElementKind.HourHand => ClockHandKind.Hour,
                ClockElementKind.MinuteHand => ClockHandKind.Minute,
                ClockElementKind.SecondHand => ClockHandKind.Second,
                _ => ClockHandKind.None,
            };

        private static ClockTimeSnapshot CreateTime(DateTime now)
        {
            float fractionalSecond = now.Millisecond / 1000f;
            float totalSeconds = now.Second + fractionalSecond;
            float totalMinutes = now.Minute + (totalSeconds / 60f);
            float totalHours = (now.Hour % 12) + (totalMinutes / 60f);

            return new ClockTimeSnapshot
            {
                Now = now,
                SecondAngle = ClockMath.Normalize360(totalSeconds * 6f),
                MinuteAngle = ClockMath.Normalize360(totalMinutes * 6f),
                HourAngle = ClockMath.Normalize360(totalHours * 30f),
                SubSecondAngle = ClockMath.Normalize360(fractionalSecond * 360f),
            };
        }

        private float Solve(ClockHandKind hand, ClockElementId id, float dt)
            => _rotation.Solve(BuildRequest(hand, id, dt));

        private HandRotationRequest BuildRequest(ClockHandKind hand, ClockElementId id, float dt)
        {
            ClockElementParameters parameters = ParametersFor(id);
            ThemeCapabilities capabilities = _theme.Capabilities;

            return new HandRotationRequest
            {
                Hand = hand,
                Pivot = AnchorOf(id),
                Time = _context.Time,
                RequestedTargetMode = parameters.HandTargetMode,
                Motion = ClockHandMotion.Crawling,
                ThemeSupportsFreeFloating = capabilities.FreeFloating,
                HandsFollowFaceRotation = capabilities.HandsFollowFaceRotation,
                MagneticNumeralsEnabled = _magneticNumeralsEnabled,
                AnchorOf = AnchorOf,
                NumeralVisibilityOf = NumeralVisibilityAt,
                FaceRotationDegrees = _context.FaceRotationDegrees,
                ExtraRotationDegrees = parameters.ExtraRotationDegrees,
                GraceSeconds = GraceSeconds,
                GlideDurationSeconds = GlideDurationSeconds,
                DeltaSeconds = dt,
            };
        }

        private PointF AnchorOf(ClockElementId id)
            => ClockElementAnchorResolver.Resolve(
                id,
                _geometry,
                _layout,
                _declared.Contains(id) ? ParametersFor(id).AnchorOffset : PointF.Empty,
                _context.FaceRotationDegrees);

        private ClockNumeralVisibility? NumeralVisibilityAt(int index)
        {
            ClockElementId id = ClockElementId.HourMarker(index);
            return _declared.Contains(id) ? ParametersFor(id).Visibility : null;
        }

        private ClockElementParameters ParametersFor(ClockElementId id)
        {
            if (!_parameters.TryGetValue(id, out ClockElementParameters? parameters))
            {
                parameters = new ClockElementParameters();
                _parameters[id] = parameters;
            }

            return parameters;
        }
    }
}
