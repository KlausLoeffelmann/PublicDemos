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
        IReadOnlyList<ClockElementDescriptor> elements = new LogicalTheme().CreateElements();

        foreach (ClockElementDescriptor element in elements)
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

        ClockElementDescriptor marker = elements.Single(
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
    public void ClockTickContextSurfaceSizeDefaultsForExistingImplementations()
    {
        IClockTickContext context = new LegacyTickContext();

        Assert.Equal(new SizeF(1000f, 1000f), context.SurfaceSize);
    }

    private static void AdvancePastCurrentPhase(LogicalThemeStateMachine machine)
        => machine.Advance(machine.CurrentPhaseDuration + TimeSpan.FromMilliseconds(1));

    private static void AdvanceToNextPhase(LogicalThemeStateMachine machine)
        => machine.Advance(machine.CurrentPhaseDuration - machine.Snapshot.PhaseElapsed);

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

    private static PointF Scale(PointF point, float scale)
        => new(point.X * scale, point.Y * scale);

    private static void AssertPointEqual(PointF expected, PointF actual)
    {
        Assert.Equal(expected.X, actual.X, 3);
        Assert.Equal(expected.Y, actual.Y, 3);
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
