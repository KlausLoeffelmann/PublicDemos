using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.Tests;

public sealed class NerdThemeTests
{
    [Fact]
    public void NerdPaletteUsesDistinctHourMinuteAndSecondLedBanks()
    {
        NerdThemePalette day = NerdTheme.CreatePalette(ClockThemeVariantKind.Day);
        NerdThemePalette night = NerdTheme.CreatePalette(ClockThemeVariantKind.Night);

        Assert.NotEqual(day.HourOn.ToArgb(), day.MinuteOn.ToArgb());
        Assert.NotEqual(day.MinuteOn.ToArgb(), day.SecondOn.ToArgb());
        Assert.NotEqual(night.HourOn.ToArgb(), night.MinuteOn.ToArgb());
        Assert.NotEqual(night.MinuteOn.ToArgb(), night.SecondOn.ToArgb());
        Assert.NotEqual(day.HourOn.ToArgb(), day.HourOff.ToArgb());
        Assert.NotEqual(day.MinuteOn.ToArgb(), day.MinuteOff.ToArgb());
        Assert.NotEqual(day.SecondOn.ToArgb(), day.SecondOff.ToArgb());
    }

    [Fact]
    public void NerdPublishesOneBinaryHandAndFourIndependentSleds()
    {
        IReadOnlyList<ClockElementDescriptor> elements = new NerdTheme().CreateElements();
        ClockElementDescriptor secondHand = Assert.Single(
            elements,
            element => element.Id == ClockElementId.SecondHand);
        ClockElementDescriptor[] slides = elements
            .Where(element => element.Id.Kind == ClockElementKind.Custom)
            .ToArray();

        Assert.Equal(7, elements.Count);
        Assert.Equal(4, slides.Length);
        Assert.DoesNotContain(elements, element => element.Id.Kind == ClockElementKind.HourMarker);
        Assert.Equal(ClockHandKind.Second, secondHand.Hand);
        Assert.Equal(NerdThemeGeometry.SecondHandContentSize, secondHand.ContentSize);
        Assert.Equal(NerdThemeGeometry.SecondHandPivot, secondHand.Pivot);
        Assert.All(slides, slide =>
        {
            Assert.Equal(ClockHandKind.None, slide.Hand);
            Assert.True(slide.RedrawPerFrame);
            Assert.Equal(NerdThemeGeometry.SledContentSize, slide.ContentSize);
            Assert.Equal(NerdThemeGeometry.SledPivot, slide.Pivot);
        });
    }

    [Fact]
    public void NerdAnimatorSpawnsAtMostFourSlidesThenReturnsToSolo()
    {
        NerdTheme theme = new()
        {
            AddSlideEveryMin = 1,
            SoloRecoveryMin = 1,
            MaximumSlides = 4,
        };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());

        animator.Initialize(context);
        Assert.Equal(1, ActiveSlides(context));

        Advance(animator, context, TimeSpan.FromMinutes(3.1), TimeSpan.FromSeconds(1));
        Assert.Equal(4, ActiveSlides(context));

        Advance(animator, context, TimeSpan.FromSeconds(54), TimeSpan.FromSeconds(1));
        Advance(animator, context, TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2));
        float beamOutOpacity = context.GetParameters(ClockElementId.CustomElement(1)).Opacity;
        Assert.InRange(beamOutOpacity, 0.01f, 0.99f);

        Advance(animator, context, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.2));
        Assert.Equal(1, ActiveSlides(context));

        Advance(animator, context, TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(1));
        Assert.Equal(1, ActiveSlides(context));
    }

    [Fact]
    public void NerdSledAlwaysGlidesWhileSecondHandUsesConfiguredMotion()
    {
        NerdTheme theme = new() { SecondHandMotion = ClockHandMotion.Tick };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);
        Advance(animator, context, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));

        Assert.Equal(3f, context.GetParameters(ClockElementId.CustomElement(0)).ExtraRotationDegrees, 3);
        Assert.Equal(ClockHandMotion.Tick, context.GetParameters(ClockElementId.SecondHand).HandMotion);
    }

    [Theory]
    [InlineData(0f, 0, "000000")]
    [InlineData(90f, 15, "001111")]
    [InlineData(240f, 40, "101000")]
    [InlineData(354f, 59, "111011")]
    [InlineData(360f, 0, "000000")]
    [InlineData(-6f, 59, "111011")]
    public void NerdSledValueComesFromItsAngularPosition(
        float angle,
        int expectedSecond,
        string expectedBinary)
    {
        int second = NerdBinaryLayout.SecondAtAngle(angle);

        Assert.Equal(expectedSecond, second);
        Assert.Equal(expectedBinary, Convert.ToString(second, 2).PadLeft(6, '0'));
    }

    [Fact]
    public void NerdSledAtTripleSpeedAdvancesThreeDisplayedPositionsPerSecond()
    {
        NerdTheme theme = new()
        {
            SpeedUpAfterMin = 1,
            MinimumFastMultiplier = 3f,
            MaximumFastMultiplier = 3f,
        };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);

        Advance(animator, context, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1));
        int before = SledState(context, 0).PositionSecond;
        Advance(animator, context, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        int after = SledState(context, 0).PositionSecond;

        Assert.Equal(3, (after - before + 60) % 60);
    }

    [Fact]
    public void NerdCompanionSledUsesAnIndependentSpeed()
    {
        NerdTheme theme = new()
        {
            AddSlideEveryMin = 1,
            SpeedUpAfterMin = 5,
        };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);
        Advance(animator, context, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1));

        float primaryBefore = context.GetParameters(ClockElementId.CustomElement(0)).ExtraRotationDegrees;
        float companionBefore = context.GetParameters(ClockElementId.CustomElement(1)).ExtraRotationDegrees;
        Advance(animator, context, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        float primaryDelta = AngularDelta(
            primaryBefore,
            context.GetParameters(ClockElementId.CustomElement(0)).ExtraRotationDegrees);
        float companionDelta = AngularDelta(
            companionBefore,
            context.GetParameters(ClockElementId.CustomElement(1)).ExtraRotationDegrees);

        Assert.NotEqual(primaryDelta, companionDelta, precision: 2);
    }

    [Fact]
    public void NerdBackgroundAnimationAdvancesContinuously()
    {
        NerdTheme theme = new();
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);

        Advance(animator, context, TimeSpan.FromSeconds(2.5), TimeSpan.FromSeconds(0.5));

        Assert.Equal(2.5f, context.GetParameters(ClockElementId.Face).Progress, 3);
    }

    [Fact]
    public void NerdPropertiesClampAndFlowToSiblingVariant()
    {
        NerdTheme theme = new()
        {
            SpeedUpAfterMin = 0,
            FastDurationMin = 0,
            AddSlideEveryMin = 0,
            SoloRecoveryMin = 0,
            MaximumSlides = 9,
            MinimumFastMultiplier = 1f,
            MaximumFastMultiplier = 9f,
            SecondHandMotion = ClockHandMotion.Sweep,
            CheatMode = true,
        };

        NerdTheme night = Assert.IsType<NerdTheme>(theme.ResolveVariant(ClockThemeVariantKind.Night));

        Assert.Equal(1, night.SpeedUpAfterMin);
        Assert.Equal(1, night.FastDurationMin);
        Assert.Equal(1, night.AddSlideEveryMin);
        Assert.Equal(1, night.SoloRecoveryMin);
        Assert.Equal(4, night.MaximumSlides);
        Assert.Equal(1.5f, night.MinimumFastMultiplier);
        Assert.Equal(5f, night.MaximumFastMultiplier);
        Assert.Equal(ClockHandMotion.Sweep, night.SecondHandMotion);
        Assert.True(night.CheatMode);
    }

    [Fact]
    public void NerdTrackPlannerMovesTheFasterSledInwardUntilItClears()
    {
        NerdSlideTrackPlanner planner = new();
        NerdSlideSnapshot[] approaching =
        [
            new(0, true, 0f, 2f),
            new(1, true, 25f, 1f),
            new(2, false, 0f, 0f),
            new(3, false, 0f, 0f),
        ];

        int[] passingTracks = planner.Plan(approaching);

        Assert.Equal(1, passingTracks[0]);
        Assert.Equal(0, passingTracks[1]);
        Assert.True(planner.IsPassing(0, 1));

        NerdSlideSnapshot[] cleared =
        [
            new(0, true, 62f, 2f),
            new(1, true, 25f, 1f),
            new(2, false, 0f, 0f),
            new(3, false, 0f, 0f),
        ];

        int[] clearedTracks = planner.Plan(cleared);

        Assert.Equal(0, clearedTracks[0]);
        Assert.False(planner.IsPassing(0, 1));
    }

    [Fact]
    public void NerdTrackPlannerSupportsNestedOvertakes()
    {
        NerdSlideTrackPlanner planner = new();
        NerdSlideSnapshot[] slides =
        [
            new(0, true, 0f, 3f),
            new(1, true, 20f, 2f),
            new(2, true, 40f, 1f),
            new(3, false, 0f, 0f),
        ];

        int[] tracks = planner.Plan(slides);

        Assert.Equal(2, tracks[0]);
        Assert.Equal(1, tracks[1]);
        Assert.Equal(0, tracks[2]);
    }

    [Fact]
    public void NerdTrackPlannerDoesNotSwapTracksWhenSpeedsReverseMidPass()
    {
        NerdSlideTrackPlanner planner = new();
        planner.Plan(
        [
            new(0, true, 0f, 2f),
            new(1, true, 24f, 1f),
            new(2, false, 0f, 0f),
            new(3, false, 0f, 0f),
        ]);

        int[] reversedWhileClose = planner.Plan(
        [
            new(0, true, 8f, 1f),
            new(1, true, 20f, 2f),
            new(2, false, 0f, 0f),
            new(3, false, 0f, 0f),
        ]);

        Assert.Equal(1, reversedWhileClose[0]);
        Assert.Equal(0, reversedWhileClose[1]);
        Assert.True(planner.IsPassing(0, 1));
        Assert.False(planner.IsPassing(1, 0));

        int[] separated = planner.Plan(
        [
            new(0, true, 0f, 1f),
            new(1, true, 40f, 2f),
            new(2, false, 0f, 0f),
            new(3, false, 0f, 0f),
        ]);

        Assert.Equal(0, separated[0]);
        Assert.Equal(0, separated[1]);
    }

    [Fact]
    public void NerdTrackPlannerRejectsCyclicPassesAfterFourSledSpeedReversal()
    {
        NerdSlideTrackPlanner planner = new();
        planner.Plan(
        [
            new(0, true, 0f, 4f),
            new(1, true, 11f, 3f),
            new(2, true, 22f, 2f),
            new(3, true, 34f, 1f),
        ]);
        planner.Plan(
        [
            new(0, true, 34f, 4f),
            new(1, true, 23f, 3f),
            new(2, true, 12f, 2f),
            new(3, true, 0f, 1f),
        ]);

        int[] reversed = planner.Plan(
        [
            new(0, true, 34f, 0.82f),
            new(1, true, 23f, 0.90f),
            new(2, true, 12f, 1.00f),
            new(3, true, 0f, 1.24f),
        ]);

        Assert.Equal([3, 2, 1, 0], reversed);
        Assert.False(planner.IsPassing(3, 0));
        Assert.False(NerdSlideTrackPlanner.SledsOverlap(
            34f,
            NerdThemeGeometry.GetSledTrackRadius(reversed[0]),
            23f,
            NerdThemeGeometry.GetSledTrackRadius(reversed[1])));
    }

    [Fact]
    public void NerdSpawnAngleUsesTheCenterOfTheLargestAvailableGap()
    {
        NerdSlideSnapshot[] slides =
        [
            new(0, true, 0f, 1f),
            new(1, true, 90f, 1f),
            new(2, true, 180f, 1f),
            new(3, false, 0f, 0f),
        ];

        Assert.Equal(270f, NerdSlideTrackPlanner.FindSafeSpawnAngle(slides), 3);
    }

    [Fact]
    public void NerdAnimatorKeepsVisibleSledGeometrySeparated()
    {
        NerdTheme theme = new()
        {
            AddSlideEveryMin = 1,
            SpeedUpAfterMin = 1,
            FastDurationMin = 1,
            SoloRecoveryMin = 1,
            MaximumSlides = 4,
        };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);

        for (int frame = 0; frame < 144000; frame++)
        {
            Advance(animator, context, TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05));
            NerdSlideRenderState[] active = Enumerable.Range(0, 4)
                .Where(index => context.GetParameters(ClockElementId.CustomElement(index)).Visible)
                .Select(index => SledState(context, index))
                .ToArray();

            for (int left = 0; left < active.Length; left++)
            {
                for (int right = left + 1; right < active.Length; right++)
                {
                    Assert.False(NerdSlideTrackPlanner.SledsOverlap(
                        active[left].Angle,
                        active[left].TrackRadius,
                        active[right].Angle,
                        active[right].TrackRadius));
                }
            }
        }
    }

    [Fact]
    public void NerdCheatSequenceUsesWallClockHalfMinuteWindows()
    {
        DateTime start = new(2026, 8, 25, 12, 30, 0, DateTimeKind.Unspecified);

        NerdCheatSample hour = NerdCheatSequence.Sample(start, enabled: true);
        NerdCheatSample crossFade = NerdCheatSequence.Sample(
            start.AddSeconds(2),
            enabled: true);
        NerdCheatSample minute = NerdCheatSequence.Sample(
            start.AddSeconds(3),
            enabled: true);
        NerdCheatSample sled = NerdCheatSequence.Sample(
            start.AddSeconds(5),
            enabled: true);
        NerdCheatSample off = NerdCheatSequence.Sample(
            start.AddSeconds(6),
            enabled: true);

        Assert.Equal(1f, hour.HourOpacity);
        Assert.True(crossFade.HourOpacity > 0f);
        Assert.True(crossFade.MinuteOpacity > 0f);
        Assert.Equal(1f, minute.MinuteOpacity);
        Assert.Equal(1f, sled.SledOpacity);
        Assert.Equal(default, off);
        Assert.Equal(default, NerdCheatSequence.Sample(start, enabled: false));
    }

    [Fact]
    public void NerdCheatStateUsesTwoDigitClockAndPerSledPositionValues()
    {
        NerdTheme theme = new() { CheatMode = true };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements())
        {
            Time = new ClockTimeSnapshot
            {
                Now = new DateTime(2026, 8, 25, 3, 7, 4, 500, DateTimeKind.Unspecified),
                HourAngle = 93.5f,
                MinuteAngle = 42.45f,
                SecondAngle = 27f,
                SubSecondAngle = 0f,
            },
        };

        animator.Initialize(context);

        NerdHandRenderState hand = Assert.IsType<NerdHandRenderState>(
            context.GetParameters(ClockElementId.SecondHand).Tag);
        NerdSlideRenderState sled = SledState(context, 0);

        Assert.Equal(0f, hand.HourCheatOpacity);
        Assert.Equal(0f, hand.MinuteCheatOpacity);
        Assert.Equal(1f, sled.CheatOpacity);
        Assert.Equal("03", context.Time.Now.Hour.ToString("00"));
        Assert.Equal("07", context.Time.Now.Minute.ToString("00"));
        Assert.Equal("04", sled.PositionSecond.ToString("00"));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(14, true)]
    [InlineData(15, false)]
    [InlineData(44, false)]
    [InlineData(45, true)]
    [InlineData(59, true)]
    public void SecondsBitOrderFlipsAtThreeAndNine(int second, bool expectedLsbFirst)
        => Assert.Equal(
            expectedLsbFirst,
            NerdBinaryLayout.SecondsUseLeastSignificantBitFirst(second));

    [Fact]
    public void BinarySlotsSupportAllThreeBanks()
    {
        Assert.Equal(
            [false, true, false, false, false, false],
            Slots(value: 2, bitCount: 6, lsbFirst: true));
        Assert.Equal(
            [false, false, false, false, false, true],
            Slots(value: 32, bitCount: 6, lsbFirst: true));
        Assert.Equal(
            [true, true, true, false, true],
            Slots(value: 23, bitCount: 5, lsbFirst: true));
        Assert.Equal(
            [true, true, false, true, true, true],
            Slots(value: 59, bitCount: 6, lsbFirst: true));
        Assert.Equal(
            [true, false, false, false, false, false],
            Slots(value: 32, bitCount: 6, lsbFirst: false));
    }

    [Fact]
    public void NerdGeometryKeepsLedsSeparatedAndClearOfArbour()
    {
        float minimumCenterDistance =
            NerdThemeGeometry.ArbourRadius
            + NerdThemeGeometry.ArbourClearance
            + NerdThemeGeometry.LedRadius;
        float outermostHourCenter =
            NerdThemeGeometry.HourBankInnerRadius
            + ((NerdThemeGeometry.HourBitCount - 1) * NerdThemeGeometry.BladeLedPitch);
        float outermostMinuteCenter =
            NerdThemeGeometry.MinuteBankInnerRadius
            + ((NerdThemeGeometry.MinuteBitCount - 1) * NerdThemeGeometry.BladeLedPitch);
        float sledPitch = 2f
            * NerdThemeGeometry.SledRadius
            * MathF.Sin(
                (NerdThemeGeometry.SledLedHalfSpanDegrees
                    * 2f
                    / (NerdThemeGeometry.SecondBitCount - 1))
                * (MathF.PI / 360f));

        Assert.True(NerdThemeGeometry.HourBankInnerRadius >= minimumCenterDistance);
        Assert.True(NerdThemeGeometry.BladeLedPitch >= NerdThemeGeometry.LedRadius * 2f);
        Assert.True(
            NerdThemeGeometry.MinuteBankInnerRadius - outermostHourCenter
            >= NerdThemeGeometry.LedRadius * 2f);
        Assert.True(
            outermostMinuteCenter + NerdThemeGeometry.LedRadius
            <= NerdThemeGeometry.BladeTopRadius);
        Assert.True(sledPitch >= NerdThemeGeometry.LedRadius * 2f);
        Assert.True(NerdThemeGeometry.SledHalfSpanDegrees * 2f < 30f);
        Assert.True(
            NerdThemeGeometry.SledTrackSpacing
            >= NerdThemeGeometry.SledCollisionRadialSpan);
        Assert.True(NerdThemeGeometry.GetSledTrackRadius(3f) > 0f);
    }

    private static bool[] Slots(int value, int bitCount, bool lsbFirst)
        => Enumerable.Range(0, bitCount)
            .Select(slot => NerdBinaryLayout.IsBitOn(value, slot, bitCount, lsbFirst))
            .ToArray();

    private static float AngularDelta(float before, float after)
    {
        float delta = (after - before) % 360f;
        return delta < 0f ? delta + 360f : delta;
    }

    private static int ActiveSlides(TestTickContext context)
        => Enumerable.Range(0, 4)
            .Count(index => context.GetParameters(ClockElementId.CustomElement(index)).Visible);

    private static NerdSlideRenderState SledState(TestTickContext context, int index)
        => Assert.IsType<NerdSlideRenderState>(
            context.GetParameters(ClockElementId.CustomElement(index)).Tag);

    private static void Advance(
        IThemeAnimator animator,
        TestTickContext context,
        TimeSpan duration,
        TimeSpan step)
    {
        int count = (int)Math.Ceiling(duration.TotalSeconds / step.TotalSeconds);
        for (int i = 0; i < count; i++)
        {
            context.FrameDelta = step;
            DateTime now = context.Time.Now + step;
            float second = now.Second + (now.Millisecond / 1000f);
            context.Time = context.Time with
            {
                Now = now,
                SecondAngle = second * 6f,
            };
            animator.OnTick(context);
        }
    }

    private sealed class TestTickContext(IReadOnlyList<ClockElementDescriptor> elements) : IClockTickContext
    {
        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];

        public ClockTimeSnapshot Time { get; set; } = new()
        {
            Now = new DateTime(2026, 8, 25, 12, 0, 0, DateTimeKind.Unspecified),
            HourAngle = 0f,
            MinuteAngle = 0f,
            SecondAngle = 0f,
            SubSecondAngle = 0f,
        };

        public ClockTimeZoneSnapshot TimeZone { get; set; }

        public ClockAmbientSnapshot Ambient { get; set; } = ClockAmbientSnapshot.Empty;

        public TimeSpan FrameDelta { get; set; }

        public IReadOnlyList<ClockElementDescriptor> Elements { get; } = elements;

        public SizeF SurfaceSize { get; set; } = new(1000f, 1000f);

        public float FaceRotationDegrees { get; set; }

        public ClockElementParameters GetParameters(ClockElementId id)
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
