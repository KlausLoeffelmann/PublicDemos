using System.Drawing;

using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.Tests;

public sealed class LogicalThemeTests
{
    [Fact]
    public void LogicalPaletteMapsOledVariantsToPitchBlackAndDimsNightVariants()
    {
        LogicalThemePalette day = LogicalTheme.CreatePalette(ClockThemeVariantKind.Day);
        LogicalThemePalette night = LogicalTheme.CreatePalette(ClockThemeVariantKind.Night);
        LogicalThemePalette oledDay = LogicalTheme.CreatePalette(ClockThemeVariantKind.OledDay);
        LogicalThemePalette oledNight = LogicalTheme.CreatePalette(ClockThemeVariantKind.OledNight);

        Assert.NotEqual(Color.Black.ToArgb(), day.FaceFill.ToArgb());
        Assert.NotEqual(Color.Black.ToArgb(), night.FaceFill.ToArgb());
        Assert.Equal(Color.Black.ToArgb(), oledDay.FaceFill.ToArgb());
        Assert.Equal(Color.Black.ToArgb(), oledNight.FaceFill.ToArgb());

        Assert.True(night.FlashCeiling < day.FlashCeiling);
        Assert.True(oledNight.FlashCeiling < oledDay.FlashCeiling);
        Assert.True(night.MotionCeiling < day.MotionCeiling);
        Assert.True(oledNight.MotionCeiling < oledDay.MotionCeiling);
        Assert.True(AverageBrightness(night.FlashColors) < AverageBrightness(day.FlashColors));
        Assert.True(AverageBrightness(oledNight.FlashColors) < AverageBrightness(oledDay.FlashColors));
        Assert.True(AverageSaturation(night.FlashColors) < AverageSaturation(day.FlashColors));
        Assert.True(AverageSaturation(oledNight.FlashColors) < AverageSaturation(oledDay.FlashColors));
    }

    [Fact]
    public void LogicalLabelsFormatExpectedTextAndHideTimezoneWithoutHostAliasOrDesignation()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(7));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty,
            new SizeF(1000f, 1000f));

        animator.Initialize(context);

        Assert.Equal("Friday", context.GetParameters(ClockElementId.Weekday).Text);
        Assert.Equal("August, 21st", context.GetParameters(ClockElementId.Day).Text);
        Assert.False(context.GetParameters(ClockElementId.TimeZone).Visible);
        Assert.Null(context.GetParameters(ClockElementId.TimeZone).Text);

        context.Ambient = ClockAmbientSnapshot.Empty with
        {
            TimeZoneAlias = "Pacific",
            TimeZoneDesignation = "UTC-07:00",
        };
        animator.OnTick(context.Advance(TimeSpan.Zero));

        Assert.True(context.GetParameters(ClockElementId.TimeZone).Visible);
        Assert.Equal("Pacific · UTC-07:00", context.GetParameters(ClockElementId.TimeZone).Text);
    }

    [Fact]
    public void LogicalCaptionSlotsStayOutsideFaceAndUseAllowedUpperPositions()
    {
        SizeF viewport = new(1600f, 1000f);
        PointF weekdayLeft = LogicalThemeAnimator.GetPaddedScreenTopAnchor(
            viewport,
            ClockElementId.Weekday,
            LogicalLabelSlot.Left);
        PointF weekdayRight = LogicalThemeAnimator.GetPaddedScreenTopAnchor(
            viewport,
            ClockElementId.Weekday,
            LogicalLabelSlot.Right);

        Assert.True(weekdayLeft.X < 0f);
        Assert.True(weekdayRight.X > 0f);
        Assert.Equal(weekdayLeft.Y, weekdayRight.Y);
        Assert.True(weekdayLeft.Y < -400f);

        PointF timeZoneMiddle = LogicalThemeAnimator.GetPaddedScreenTopAnchor(
            viewport,
            ClockElementId.TimeZone,
            LogicalLabelSlot.Middle);
        Assert.Equal(0f, timeZoneMiddle.X, 3);

        // Shared-corner timezone is stacked on a second row so it cannot cover weekday/date.
        int[] permutation = [(int)LogicalLabelSlot.Left, (int)LogicalLabelSlot.Left, (int)LogicalLabelSlot.Right];
        PointF stackedTimeZone = LogicalThemeAnimator.GetPaddedScreenTopAnchor(
            viewport,
            ClockElementId.TimeZone,
            LogicalLabelSlot.Left,
            permutation);
        Assert.True(stackedTimeZone.Y > weekdayLeft.Y);
    }

    [Fact]
    public void LogicalLayoutReturnsPixelCenterOnlyForCaptions()
    {
        IClockLayout layout = new LogicalTheme().CreateLayout();
        SizeF surface = new(1920f, 1080f);

        Assert.True(layout.TryGetAnchor(ClockElementId.Weekday, surface, out PointF weekday));
        Assert.True(layout.TryGetAnchor(ClockElementId.Day, surface, out PointF day));
        Assert.True(layout.TryGetAnchor(ClockElementId.TimeZone, surface, out PointF timeZone));
        Assert.False(layout.TryGetAnchor(ClockElementId.HourMarker(0), surface, out _));

        AssertPointEqual(new PointF(960f, 540f), weekday);
        AssertPointEqual(new PointF(960f, 540f), day);
        AssertPointEqual(new PointF(960f, 540f), timeZone);
    }

    [Theory]
    [InlineData(800f, 600f)]
    [InlineData(1920f, 1080f)]
    [InlineData(3840f, 2160f)]
    public void LogicalCaptionEngineCompositionHonorsDesignScale(float width, float height)
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(9));
        SizeF surface = new(width, height);
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty with { TimeZoneAlias = "Tokyo" },
            surface);

        animator.Initialize(context);

        float designScale = MathF.Min(width, height) / 1000f;
        PointF center = new(width / 2f, height / 2f);
        PointF expectedDesign = LogicalThemeAnimator.GetPaddedScreenTopAnchor(
            animator.Snapshot.ViewportSize,
            ClockElementId.Weekday,
            LogicalLabelSlot.Left);
        PointF expectedPixel = Add(center, Scale(expectedDesign, designScale));
        PointF actualPixel = ComposeEngineAnchor(
            theme.CreateLayout(),
            ClockElementId.Weekday,
            surface,
            context.GetParameters(ClockElementId.Weekday).AnchorOffset);

        AssertPointEqual(expectedPixel, actualPixel);
    }

    [Fact]
    public void LogicalReassemblyKeepsArrivedPartsNearDestinationWithoutDoubleOffset()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(37));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty,
            new SizeF(1600f, 1000f));

        animator.Initialize(context);
        AdvanceAnimatorToPhase(animator, context, LogicalThemePhase.FlyingOff);

        int moverIndex = animator.CurrentDetachPlan.FirstWaveNumerals[0];
        ClockElementId id = ClockElementId.HourMarker(moverIndex - 1);

        // Finish FlyingOff and enter Reassembling.
        animator.OnTick(context.Advance(animator.Snapshot.PhaseDuration - animator.Snapshot.PhaseElapsed + TimeSpan.FromMilliseconds(1)));
        Assert.Equal(LogicalThemePhase.Reassembling, animator.Snapshot.Phase);

        PointF expected = Add(
            animator.Snapshot.DestinationOffset,
            Scale(LogicalThemeAnimator.GetHomeVector(id), LogicalThemeStateMachine.ZoomedOutSceneScale));
        PointF actual = ElementWorld(context, id);

        Assert.True(Distance(actual, expected) < 25f);

        animator.OnTick(context.Advance(TimeSpan.FromMilliseconds(200)));
        Assert.True(Distance(ElementWorld(context, id), expected) < 25f);
    }

    [Fact]
    public void LogicalCameraStaysAtSourceWhilePartsOccupyDestinationDuringReassembly()
    {
        var machine = new LogicalThemeStateMachine(new Random(11));
        machine.SetViewport(new SizeF(1600f, 1000f));
        AdvanceToPhase(machine, LogicalThemePhase.Reassembling);

        Assert.Equal(machine.DestinationOffset, machine.Snapshot.SceneOffset);
        AssertPointEqual(machine.SourceStagingOffset, LogicalThemeAnimator.GetCameraOffset(machine.Snapshot));
    }

    [Fact]
    public void LogicalCaptionsStartAtPaddedScreenTopAnchors()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(5));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty with { TimeZoneAlias = "Tokyo" },
            new SizeF(1600f, 1000f));

        animator.Initialize(context);

        AssertPointEqual(
            LogicalThemeAnimator.GetPaddedScreenTopAnchor(
                animator.Snapshot.ViewportSize,
                ClockElementId.Weekday,
                LogicalLabelSlot.Left),
            ElementWorld(context, ClockElementId.Weekday));
        AssertPointEqual(
            LogicalThemeAnimator.GetPaddedScreenTopAnchor(
                animator.Snapshot.ViewportSize,
                ClockElementId.Day,
                LogicalLabelSlot.Right),
            ElementWorld(context, ClockElementId.Day));
        AssertPointEqual(
            LogicalThemeAnimator.GetPaddedScreenTopAnchor(
                animator.Snapshot.ViewportSize,
                ClockElementId.TimeZone,
                LogicalLabelSlot.Middle),
            ElementWorld(context, ClockElementId.TimeZone));
    }

    [Fact]
    public void LogicalCaptionPermutationAlternatesDateCornersAndCyclesTimezoneUpperSlots()
    {
        int[] current = [0, 1, 2];
        var random = new Random(41);

        for (int cycle = 0; cycle < 12; cycle++)
        {
            int[] next = LogicalThemeAnimator.PickNextLabelPermutation(random, current);

            Assert.Equal(
                current[0] == (int)LogicalLabelSlot.Left
                    ? (int)LogicalLabelSlot.Right
                    : (int)LogicalLabelSlot.Left,
                next[0]);
            Assert.Equal(
                next[0] == (int)LogicalLabelSlot.Left
                    ? (int)LogicalLabelSlot.Right
                    : (int)LogicalLabelSlot.Left,
                next[2]);
            Assert.InRange(next[1], (int)LogicalLabelSlot.Left, (int)LogicalLabelSlot.Right);
            Assert.NotEqual(current[1], next[1]);

            current = next;
        }
    }

    [Fact]
    public void LogicalMotionSchedulesRandomDurationsWithinApprovedBounds()
    {
        for (int seed = 0; seed < 64; seed++)
        {
            var machine = new LogicalThemeStateMachine(new Random(seed));

            Assert.Equal(LogicalThemePhase.Calm, machine.Phase);
            Assert.InRange(
                machine.CurrentPhaseDuration,
                LogicalThemeStateMachine.CalmMinDuration,
                LogicalThemeStateMachine.CalmMaxDuration);

            AdvancePastCurrentPhase(machine);

            Assert.Equal(LogicalThemePhase.Escalating, machine.Phase);
            Assert.InRange(
                machine.CurrentPhaseDuration,
                LogicalThemeStateMachine.EscalationMinDuration,
                LogicalThemeStateMachine.EscalationMaxDuration);
        }
    }

    [Fact]
    public void LogicalMotionBeginsStagingPanDuringEscalation()
    {
        var machine = new LogicalThemeStateMachine(new Random(2718));
        machine.SetViewport(new SizeF(1600f, 1000f));
        AdvancePastCurrentPhase(machine);

        LogicalThemeSnapshot midpoint = machine.Advance(TimeSpan.FromTicks(machine.CurrentPhaseDuration.Ticks / 2));

        Assert.Equal(LogicalThemePhase.Escalating, midpoint.Phase);
        Assert.True(Distance(midpoint.SceneOffset, machine.CurrentSafeOffset) > 0.5f);
        Assert.True(
            Distance(midpoint.SceneOffset, machine.SourceStagingOffset)
            < Distance(machine.CurrentSafeOffset, machine.SourceStagingOffset));
        Assert.True(midpoint.SceneScale < LogicalThemeStateMachine.BaseSceneScale);
    }

    [Fact]
    public void LogicalLabelsRemainSceneElementsDuringStagingPanAndZoom()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(19));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty with { TimeZoneAlias = "Tokyo" },
            new SizeF(1600f, 1000f));

        animator.Initialize(context);
        animator.OnTick(context.Advance(animator.Snapshot.PhaseDuration + TimeSpan.FromMilliseconds(1)));
        PointF originalWorldAnchor = animator.GetLabelSceneAnchor(
            animator.Snapshot,
            ClockElementId.Weekday);
        animator.OnTick(context.Advance(TimeSpan.FromTicks(animator.Snapshot.PhaseDuration.Ticks / 2)));

        Assert.Equal(LogicalThemePhase.Escalating, animator.Snapshot.Phase);
        AssertPointEqual(
            originalWorldAnchor,
            animator.GetLabelSceneAnchor(animator.Snapshot, ClockElementId.Weekday));
        Assert.All(
            new[] { ClockElementId.Weekday, ClockElementId.TimeZone, ClockElementId.Day },
            id =>
            {
                Assert.True(context.GetParameters(id).Visible);
                Assert.Equal(1f, context.GetParameters(id).Opacity, 3);
            });

        animator.OnTick(context.Advance(
            animator.Snapshot.PhaseDuration - animator.Snapshot.PhaseElapsed + TimeSpan.FromMilliseconds(1)));

        Assert.Equal(LogicalThemePhase.ZoomingOut, animator.Snapshot.Phase);
        AssertPointEqual(
            originalWorldAnchor,
            animator.GetLabelSceneAnchor(animator.Snapshot, ClockElementId.Weekday));
        Assert.True(context.GetParameters(ClockElementId.Weekday).Visible);
        Assert.Equal(1f, context.GetParameters(ClockElementId.Weekday).Opacity, 3);
    }

    [Fact]
    public void LogicalCaptionsRemainInSceneThroughFlightAndFollowTheRecenterTransform()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(29));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty with { TimeZoneAlias = "Tokyo" },
            new SizeF(1600f, 1000f));

        animator.Initialize(context);
        AdvanceAnimatorToPhase(animator, context, LogicalThemePhase.FlyingOff);
        PointF sourceWorld = ElementWorld(context, ClockElementId.Weekday);

        animator.OnTick(context.Advance(TimeSpan.FromSeconds(2)));
        AssertPointEqual(sourceWorld, ElementWorld(context, ClockElementId.Weekday));
        Assert.True(context.GetParameters(ClockElementId.Weekday).Visible);

        AdvanceAnimatorToPhase(animator, context, LogicalThemePhase.Reassembling);
        AssertPointEqual(sourceWorld, ElementWorld(context, ClockElementId.Weekday));

        AdvanceAnimatorToPhase(animator, context, LogicalThemePhase.ZoomingIn);
        AssertPointEqual(sourceWorld, ElementWorld(context, ClockElementId.Weekday));

        animator.OnTick(context.Advance(TimeSpan.FromTicks(animator.Snapshot.PhaseDuration.Ticks / 2)));
        PointF recenterWorld = ElementWorld(context, ClockElementId.Weekday);

        AssertPointEqual(
            animator.GetLabelViewportPosition(animator.Snapshot, ClockElementId.Weekday),
            recenterWorld);
        Assert.NotEqual(sourceWorld, recenterWorld);
        Assert.True(context.GetParameters(ClockElementId.Weekday).Visible);
    }

    [Fact]
    public void LogicalCaptionReturnStartsContinuouslyAfterRecenterCompletes()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(31));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty with { TimeZoneAlias = "Tokyo" },
            new SizeF(1600f, 1000f));

        animator.Initialize(context);
        AdvanceAnimatorToPhase(animator, context, LogicalThemePhase.ZoomingIn);
        animator.OnTick(context.Advance(animator.Snapshot.PhaseDuration - TimeSpan.FromMilliseconds(10)));
        PointF beforeBoundary = ElementWorld(context, ClockElementId.Day);

        animator.OnTick(context.Advance(TimeSpan.FromMilliseconds(20)));
        PointF afterBoundary = ElementWorld(context, ClockElementId.Day);

        Assert.Equal(LogicalThemePhase.Calm, animator.Snapshot.Phase);
        Assert.True(Distance(beforeBoundary, afterBoundary) < 30f);
        Assert.Equal(1f, context.GetParameters(ClockElementId.Day).Opacity, 3);
    }

    [Fact]
    public void LogicalArrivedClockPartsRemainAtDestinationWhileSettling()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(37));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty,
            new SizeF(1600f, 1000f));

        animator.Initialize(context);
        AdvanceAnimatorToPhase(animator, context, LogicalThemePhase.FlyingOff);

        int moverIndex = animator.CurrentDetachPlan.FirstWaveNumerals[0];
        ClockElementId id = ClockElementId.HourMarker(moverIndex - 1);
        animator.OnTick(context.Advance(TimeSpan.FromTicks(
            (long)(animator.Snapshot.PhaseDuration.Ticks * 0.97))));

        PointF expected = Add(
            animator.Snapshot.DestinationOffset,
            Scale(
                LogicalThemeAnimator.GetHomeVector(id),
                LogicalThemeStateMachine.ZoomedOutSceneScale));
        PointF firstSettled = ElementWorld(context, id);
        Assert.True(Distance(firstSettled, expected) < 10f);

        animator.OnTick(context.Advance(TimeSpan.FromMilliseconds(150)));
        PointF stillSettled = ElementWorld(context, id);
        Assert.True(Distance(stillSettled, expected) < 10f);
        Assert.True(Distance(firstSettled, stillSettled) < 12f);
    }

    [Fact]
    public void LogicalMotionWalksApprovedStateOrderAndCompletesAtItsSelectedOffset()
    {
        var machine = new LogicalThemeStateMachine(new Random(31415));
        machine.SetViewport(new SizeF(1600f, 1000f));
        PointF expectedOffset = machine.TargetSafeOffset;
        PointF source = machine.SourceStagingOffset;
        PointF destination = machine.DestinationOffset;

        List<LogicalThemePhase> phases = [machine.Phase];
        for (int i = 0; i < 6; i++)
        {
            AdvancePastCurrentPhase(machine);
            phases.Add(machine.Phase);
        }

        Assert.Equal(
        [
            LogicalThemePhase.Calm,
            LogicalThemePhase.Escalating,
            LogicalThemePhase.ZoomingOut,
            LogicalThemePhase.FlyingOff,
            LogicalThemePhase.Reassembling,
            LogicalThemePhase.ZoomingIn,
            LogicalThemePhase.Calm,
        ],
        phases);

        Assert.Equal(1, machine.CompletedCycles);
        Assert.Equal(expectedOffset, machine.CurrentSafeOffset);
        Assert.Contains(machine.CurrentSafeOffset, LogicalThemeStateMachine.SafeOffsets);
        Assert.NotEqual(PointF.Empty, machine.CurrentSafeOffset);
        Assert.NotEqual(machine.CurrentSafeOffset, machine.TargetSafeOffset);

        Assert.Equal(-source.X, destination.X, 3);
        Assert.Equal(-source.Y, destination.Y, 3);
        Assert.True(Distance(source, destination) > 500f);
    }

    [Fact]
    public void LogicalDetachPlanStagesWavesInRequiredOrder()
    {
        LogicalDetachPlan plan = LogicalDetachPlan.Create(new Random(23));

        Assert.InRange(plan.FirstWaveNumerals.Count, 1, 3);
        Assert.Equal(
            (int)Math.Round((12 - plan.FirstWaveNumerals.Count) / 2f, MidpointRounding.AwayFromZero),
            plan.SecondWaveNumerals.Count);
        Assert.All(plan.FirstWaveNumerals, mover => Assert.InRange(mover, 1, 12));
        Assert.All(plan.SecondWaveNumerals, mover => Assert.InRange(mover, 1, 12));
        Assert.Contains(LogicalThemeAnimator.HourHandMoverIndex, plan.ThirdWaveMovers);
        Assert.Contains(LogicalThemeAnimator.MinuteHandMoverIndex, plan.ThirdWaveMovers);
        Assert.Contains(LogicalThemeAnimator.SecondHandMoverIndex, plan.ThirdWaveMovers);
        Assert.Contains(LogicalThemeAnimator.ArbourMoverIndex, plan.ThirdWaveMovers);
        Assert.Equal([LogicalThemeAnimator.FaceMoverIndex, LogicalThemeAnimator.CaseMoverIndex], plan.FourthWaveMovers);

        float secondStart = plan.GetWindow(plan.SecondWaveNumerals[0]).Start;
        float thirdStart = plan.GetWindow(plan.ThirdWaveMovers[0]).Start;
        float fourthStart = plan.GetWindow(plan.FourthWaveMovers[0]).Start;
        Assert.All(plan.FirstWaveNumerals, mover => Assert.True(plan.GetWindow(mover).Start < secondStart));
        Assert.All(plan.SecondWaveNumerals, mover => Assert.True(plan.GetWindow(mover).Start < thirdStart));
        Assert.All(plan.ThirdWaveMovers, mover => Assert.True(plan.GetWindow(mover).Start < fourthStart));
    }

    [Fact]
    public void LogicalMotionKeepsChoosingDistinctSafeOffsetsWithinBoundsAcrossCycles()
    {
        for (int seed = 0; seed < 16; seed++)
        {
            var machine = new LogicalThemeStateMachine(new Random(seed));
            machine.SetViewport(new SizeF(1600f, 1000f));

            for (int cycle = 0; cycle < 5; cycle++)
            {
                Assert.Contains(machine.TargetSafeOffset, LogicalThemeStateMachine.SafeOffsets);
                Assert.True(
                    Distance(machine.CurrentSafeOffset, machine.TargetSafeOffset)
                    >= LogicalThemeStateMachine.MinimumSafeMoveDistance);

                Assert.Equal(-machine.SourceStagingOffset.X, machine.DestinationOffset.X, 3);
                Assert.Equal(-machine.SourceStagingOffset.Y, machine.DestinationOffset.Y, 3);
                Assert.True(
                    Distance(machine.SourceStagingOffset, machine.DestinationOffset)
                    > Distance(machine.CurrentSafeOffset, machine.TargetSafeOffset));

                AdvanceFullCycle(machine);
            }
        }
    }

    [Fact]
    public void LogicalMotionFliesElementsCornerToCornerBeforePanningAndZoomingToSafeOffset()
    {
        var machine = new LogicalThemeStateMachine(new Random(8675309));
        machine.SetViewport(new SizeF(1600f, 1000f));

        AdvanceToNextPhase(machine);
        AdvanceToNextPhase(machine);
        AdvanceToNextPhase(machine);

        Assert.Equal(LogicalThemePhase.FlyingOff, machine.Phase);
        Assert.Equal(machine.SourceStagingOffset, machine.Snapshot.SceneOffset);

        machine.Advance(machine.CurrentPhaseDuration - machine.Snapshot.PhaseElapsed);
        Assert.Equal(LogicalThemePhase.Reassembling, machine.Phase);
        Assert.Equal(machine.DestinationOffset, machine.Snapshot.SceneOffset);

        LogicalThemeSnapshot midwayThroughReassembly = machine.Advance(
            TimeSpan.FromTicks(machine.CurrentPhaseDuration.Ticks / 2));
        Assert.Equal(machine.DestinationOffset, midwayThroughReassembly.SceneOffset);

        machine.Advance(machine.CurrentPhaseDuration - machine.Snapshot.PhaseElapsed);
        Assert.Equal(LogicalThemePhase.ZoomingIn, machine.Phase);
        Assert.Equal(machine.DestinationOffset, machine.Snapshot.SceneOffset);

        LogicalThemeSnapshot midwayThroughPan = machine.Advance(
            TimeSpan.FromTicks(machine.CurrentPhaseDuration.Ticks / 2));
        Assert.True(
            Distance(midwayThroughPan.SceneOffset, machine.TargetSafeOffset)
            < Distance(machine.DestinationOffset, machine.TargetSafeOffset));
        Assert.True(midwayThroughPan.SceneScale > LogicalThemeStateMachine.ZoomedOutSceneScale);
    }

    [Fact]
    public void LogicalTravelProgressAndIntensityStaySmoothAcrossReassemblyBoundary()
    {
        var machine = new LogicalThemeStateMachine(new Random(73));
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(73));
        machine.SetViewport(new SizeF(1600f, 1000f));

        AdvanceToPhase(machine, LogicalThemePhase.FlyingOff);
        LogicalThemeSnapshot beforeBoundary = machine.Advance(machine.CurrentPhaseDuration - machine.Snapshot.PhaseElapsed - TimeSpan.FromMilliseconds(20));
        LogicalThemeSnapshot afterBoundary = machine.Advance(TimeSpan.FromMilliseconds(40));

        float beforeProgress = animator.GetTravelProgress(beforeBoundary, LogicalThemeAnimator.CaseMoverIndex);
        float afterProgress = animator.GetTravelProgress(afterBoundary, LogicalThemeAnimator.CaseMoverIndex);

        Assert.Equal(LogicalThemePhase.Reassembling, afterBoundary.Phase);
        Assert.True(afterProgress >= beforeProgress);

        ClockElementDescriptor casing = new LogicalTheme().CreateElements().Single(
            element => element.Id.Kind == ClockElementKind.Case);
        PointF beforeWorld = Add(
            LogicalThemeAnimator.GetHomeVector(casing.Id),
            LogicalThemeAnimator.SampleFlightAnchorOffset(casing, beforeBoundary, LogicalThemeAnimator.CaseMoverIndex, beforeProgress));
        PointF afterWorld = Add(
            LogicalThemeAnimator.GetHomeVector(casing.Id),
            LogicalThemeAnimator.SampleFlightAnchorOffset(casing, afterBoundary, LogicalThemeAnimator.CaseMoverIndex, afterProgress));
        Assert.True(Distance(beforeWorld, afterWorld) < 90f);

        LogicalThemeSnapshot endOfReassembly = machine.Advance(machine.CurrentPhaseDuration - machine.Snapshot.PhaseElapsed);
        Assert.Equal(LogicalThemePhase.ZoomingIn, endOfReassembly.Phase);
        Assert.Equal(0.25f, endOfReassembly.StormIntensity, 3);
        Assert.Equal(0.20f, endOfReassembly.FlashIntensity, 3);
    }

    [Theory]
    [InlineData(1000f, 1000f)]
    [InlineData(1777f, 1000f)]
    [InlineData(1000f, 1777f)]
    public void LogicalElementFlightPathsStayVisibleAndFinishAtTheirDestinationPositions(
        float viewportWidth,
        float viewportHeight)
    {
        var machine = new LogicalThemeStateMachine(new Random(42));
        machine.SetViewport(new SizeF(viewportWidth, viewportHeight));
        AdvanceToNextPhase(machine);
        AdvanceToNextPhase(machine);
        AdvanceToNextPhase(machine);
        LogicalThemeSnapshot snapshot = machine.Snapshot;
        IReadOnlyList<ClockElementDescriptor> movers = new LogicalTheme().CreateElements()
            .Where(element => LogicalThemeAnimator.GetMoverIndex(element.Id) >= 0)
            .ToList();

        foreach (ClockElementDescriptor element in movers)
        {
            int moverIndex = LogicalThemeAnimator.GetMoverIndex(element.Id);
            Assert.True(moverIndex >= 0);

            PointF home = LogicalThemeAnimator.GetHomeVector(element.Id);
            PointF start = LogicalThemeAnimator.SampleFlightAnchorOffset(element, snapshot, moverIndex, 0f);
            PointF end = LogicalThemeAnimator.SampleFlightAnchorOffset(element, snapshot, moverIndex, 1f);
            AssertPointEqual(
                Add(snapshot.SourceStagingOffset, Scale(home, snapshot.SceneScale)),
                Add(home, start));
            AssertPointEqual(
                Add(snapshot.DestinationOffset, Scale(home, snapshot.SceneScale)),
                Add(home, end));

            (float horizontal, float vertical) =
                LogicalThemeAnimator.GetSafeElementExtents(element, snapshot.SceneScale);
            for (int sample = 0; sample <= 40; sample++)
            {
                PointF offset = LogicalThemeAnimator.SampleFlightAnchorOffset(
                    element,
                    snapshot,
                    moverIndex,
                    sample / 40f);
                PointF world = Add(home, offset);
                Assert.InRange(
                    world.X,
                    -snapshot.ViewportSize.Width / 2f + horizontal - 0.01f,
                    snapshot.ViewportSize.Width / 2f - horizontal + 0.01f);
                Assert.InRange(
                    world.Y,
                    -snapshot.ViewportSize.Height / 2f + vertical - 0.01f,
                    snapshot.ViewportSize.Height / 2f - vertical + 0.01f);
            }
        }

        ClockElementDescriptor marker = movers.Single(
            element => element.Id == ClockElementId.HourMarker(0));
        int markerIndex = LogicalThemeAnimator.GetMoverIndex(marker.Id);
        PointF curvedMidpoint = Add(
            LogicalThemeAnimator.GetHomeVector(marker.Id),
            LogicalThemeAnimator.SampleFlightAnchorOffset(marker, snapshot, markerIndex, 0.5f));
        PointF linearMidpoint = Scale(
            Add(
                Add(snapshot.SourceStagingOffset, Scale(LogicalThemeAnimator.GetHomeVector(marker.Id), snapshot.SceneScale)),
                Add(snapshot.DestinationOffset, Scale(LogicalThemeAnimator.GetHomeVector(marker.Id), snapshot.SceneScale))),
            0.5f);
        Assert.True(Distance(curvedMidpoint, linearMidpoint) > 10f);
    }

    [Fact]
    public void LogicalMotionRetargetsCornerRouteWhenViewportChangesMidFlight()
    {
        var machine = new LogicalThemeStateMachine(new Random(73));
        machine.SetViewport(new SizeF(1777f, 1000f));
        AdvanceToNextPhase(machine);
        AdvanceToNextPhase(machine);
        AdvanceToNextPhase(machine);
        PointF landscapeDestination = machine.DestinationOffset;

        machine.SetViewport(new SizeF(1000f, 1777f));

        Assert.NotEqual(landscapeDestination, machine.DestinationOffset);
        Assert.Equal(new SizeF(1000f, 1777f), machine.Snapshot.ViewportSize);
        Assert.Equal(machine.SourceStagingOffset, machine.Snapshot.SceneOffset);
        Assert.Equal(-machine.SourceStagingOffset.X, machine.DestinationOffset.X, 3);
        Assert.Equal(-machine.SourceStagingOffset.Y, machine.DestinationOffset.Y, 3);
    }

    [Fact]
    public void LogicalLabelsReturnOnlyAfterPostPanAndPermuteSlots()
    {
        LogicalTheme theme = new();
        var animator = new LogicalThemeAnimator(LogicalTheme.CreatePalette(ClockThemeVariantKind.Day), new Random(11));
        var context = new TestTickContext(
            theme.CreateElements(),
            CreateTime(2026, 8, 21, 9, 15, 0),
            ClockAmbientSnapshot.Empty with
            {
                TimeZoneAlias = "Tokyo",
                TimeZoneDesignation = "UTC+09:00",
            },
            new SizeF(1600f, 1000f));

        animator.Initialize(context);
        LogicalLabelSlot initialDaySlot = animator.GetAssignedLabelSlot(ClockElementId.Day);
        while (animator.Snapshot.CompletedCycles == 0 && animator.Snapshot.Phase != LogicalThemePhase.ZoomingIn)
        {
            context.Time = context.Time with { Now = context.Time.Now.AddSeconds(5) };
            animator.OnTick(context.Advance(TimeSpan.FromSeconds(5)));
        }

        while (animator.Snapshot.CompletedCycles == 0)
        {
            context.Time = context.Time with { Now = context.Time.Now.AddSeconds(1) };
            animator.OnTick(context.Advance(TimeSpan.FromSeconds(1)));
        }

        Assert.Equal(LogicalThemePhase.Calm, animator.Snapshot.Phase);
        Assert.True(animator.LabelReturnSeconds <= 1.1d);
        Assert.Equal(1f, context.GetParameters(ClockElementId.Day).Opacity, 3);

        for (int i = 0; i < 4; i++)
        {
            context.Time = context.Time with { Now = context.Time.Now.AddSeconds(1) };
            animator.OnTick(context.Advance(TimeSpan.FromSeconds(1)));
        }

        LogicalLabelSlot finalDaySlot = animator.GetAssignedLabelSlot(ClockElementId.Day);
        PointF expectedDayWorld = LogicalThemeAnimator.GetPaddedScreenTopAnchor(
            animator.Snapshot.ViewportSize,
            ClockElementId.Day,
            finalDaySlot);

        Assert.True(finalDaySlot != initialDaySlot || animator.GetAssignedLabelSlot(ClockElementId.Weekday) != LogicalLabelSlot.Left);
        Assert.Equal(1f, context.GetParameters(ClockElementId.Day).Opacity, 3);
        AssertPointEqual(expectedDayWorld, ElementWorld(context, ClockElementId.Day));
    }

    [Fact]
    public void ClockTickContextSurfaceSizeDefaultsForExistingImplementations()
    {
        IClockTickContext context = new LegacyTickContext();

        Assert.Equal(new SizeF(1000f, 1000f), context.SurfaceSize);
    }

    private static ClockTimeSnapshot CreateTime(int year, int month, int day, int hour, int minute, int second)
    {
        DateTime now = new(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
        return new ClockTimeSnapshot
        {
            Now = now,
            HourAngle = 0f,
            MinuteAngle = 0f,
            SecondAngle = 0f,
            SubSecondAngle = 0f,
        };
    }

    private static void AdvancePastCurrentPhase(LogicalThemeStateMachine machine)
        => machine.Advance(machine.CurrentPhaseDuration + TimeSpan.FromMilliseconds(1));

    private static void AdvanceToNextPhase(LogicalThemeStateMachine machine)
        => machine.Advance(machine.CurrentPhaseDuration - machine.Snapshot.PhaseElapsed);

    private static void AdvanceToPhase(LogicalThemeStateMachine machine, LogicalThemePhase phase)
    {
        while (machine.Phase != phase)
        {
            AdvanceToNextPhase(machine);
        }
    }

    private static void AdvanceAnimatorToPhase(
        LogicalThemeAnimator animator,
        TestTickContext context,
        LogicalThemePhase phase)
    {
        while (animator.Snapshot.Phase != phase)
        {
            TimeSpan remaining = animator.Snapshot.PhaseDuration - animator.Snapshot.PhaseElapsed;
            animator.OnTick(context.Advance(remaining));
        }
    }

    private static PointF ElementWorld(TestTickContext context, ClockElementId id)
        => Add(LogicalThemeAnimator.GetHomeVector(id), context.GetParameters(id).AnchorOffset);

    private static PointF ComposeEngineAnchor(
        IClockLayout layout,
        ClockElementId id,
        SizeF surface,
        PointF anchorOffsetDesign)
    {
        float designScale = MathF.Min(surface.Width, surface.Height) / 1000f;
        PointF layoutAnchor = layout.TryGetAnchor(id, surface, out PointF custom)
            ? custom
            : new PointF(surface.Width / 2f, surface.Height / 2f);
        return new PointF(
            layoutAnchor.X + (anchorOffsetDesign.X * designScale),
            layoutAnchor.Y + (anchorOffsetDesign.Y * designScale));
    }

    private static void AdvanceFullCycle(LogicalThemeStateMachine machine)
    {
        for (int i = 0; i < 6; i++)
        {
            AdvancePastCurrentPhase(machine);
        }
    }

    private static float AverageBrightness(IEnumerable<Color> colors)
        => colors.Average(color => color.GetBrightness());

    private static float AverageSaturation(IEnumerable<Color> colors)
        => colors.Average(color => color.GetSaturation());

    private static float Distance(PointF a, PointF b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return MathF.Sqrt((dx * dx) + (dy * dy));
    }

    private static PointF Add(PointF left, PointF right)
        => new(left.X + right.X, left.Y + right.Y);

    private static PointF Subtract(PointF left, PointF right)
        => new(left.X - right.X, left.Y - right.Y);

    private static PointF Scale(PointF point, float scale)
        => new(point.X * scale, point.Y * scale);

    private static void AssertPointEqual(PointF expected, PointF actual)
    {
        Assert.Equal(expected.X, actual.X, 3);
        Assert.Equal(expected.Y, actual.Y, 3);
    }

    private sealed class TestTickContext : IClockTickContext
    {
        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];

        public TestTickContext(
            IReadOnlyList<ClockElementDescriptor> elements,
            ClockTimeSnapshot time,
            ClockAmbientSnapshot ambient,
            SizeF surfaceSize)
        {
            Elements = elements;
            Time = time;
            Ambient = ambient;
            SurfaceSize = surfaceSize;
            TimeZone = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Local, time.Now);
        }

        public ClockTimeSnapshot Time { get; set; }

        public ClockTimeZoneSnapshot TimeZone { get; set; }

        public ClockAmbientSnapshot Ambient { get; set; }

        public TimeSpan FrameDelta { get; private set; }

        public IReadOnlyList<ClockElementDescriptor> Elements { get; }

        public SizeF SurfaceSize { get; }

        public float FaceRotationDegrees { get; set; }

        public ClockElementParameters GetParameters(ClockElementId id)
        {
            if (!_parameters.TryGetValue(id, out ClockElementParameters? parameters))
            {
                parameters = new ClockElementParameters();
                _parameters.Add(id, parameters);
            }

            return parameters;
        }

        public TestTickContext Advance(TimeSpan frameDelta)
        {
            FrameDelta = frameDelta;
            TimeZone = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Local, Time.Now);
            return this;
        }
    }

    private sealed class LegacyTickContext : IClockTickContext
    {
        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];

        public ClockTimeSnapshot Time => default;

        public TimeSpan FrameDelta => TimeSpan.Zero;

        public IReadOnlyList<ClockElementDescriptor> Elements => [];

        public float FaceRotationDegrees { get; set; }

        public ClockElementParameters GetParameters(ClockElementId id)
        {
            if (!_parameters.TryGetValue(id, out ClockElementParameters? parameters))
            {
                parameters = new ClockElementParameters();
                _parameters.Add(id, parameters);
            }

            return parameters;
        }
    }
}
