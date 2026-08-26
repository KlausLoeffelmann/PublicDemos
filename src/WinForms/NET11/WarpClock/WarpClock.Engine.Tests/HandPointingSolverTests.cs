using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine.Tests;

public sealed class HandPointingSolverTests
{
    [Theory]
    [InlineData(ClockHandMotion.Crawling, 6.192f)]
    [InlineData(ClockHandMotion.Tick, 12f)]
    [InlineData(ClockHandMotion.FastTick, 12f)]
    [InlineData(ClockHandMotion.Sweep, 12.6f)]
    public void RadialTargetAngle_HonorsAllMotions(ClockHandMotion motion, float expectedDegrees)
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 00, 02, 100));

        float angle = HandPointingSolver.RadialTargetAngle(time, ClockHandKind.Second, motion, 0.5f);

        Assert.Equal(expectedDegrees, angle, 3);
    }

    [Theory]
    [InlineData(ClockHandKind.Hour, ClockHandMotion.Tick, 244.5f)]
    [InlineData(ClockHandKind.Hour, ClockHandMotion.Sweep, 244.75f)]
    [InlineData(ClockHandKind.Minute, ClockHandMotion.Tick, 57f)]
    [InlineData(ClockHandKind.Minute, ClockHandMotion.Sweep, 57.05f)]
    public void HourAndMinuteHandsAdvanceInLowerUnitFractions(
        ClockHandKind hand,
        ClockHandMotion motion,
        float expectedDegrees)
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 20, 09, 30, 500));

        float angle = HandPointingSolver.RadialTargetAngle(time, hand, motion);

        Assert.Equal(expectedDegrees, angle, 2);
    }

    [Fact]
    public void HourCrawlEasesAcrossTheBeginningOfEachMinute()
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 20, 10, 0, 250));

        float angle = HandPointingSolver.RadialTargetAngle(
            time,
            ClockHandKind.Hour,
            ClockHandMotion.Crawling,
            glideDurationSeconds: 0.5f);

        Assert.InRange(angle, 244.73f, 244.78f);
    }

    [Theory]
    [InlineData(ClockHandMotion.Crawling)]
    [InlineData(ClockHandMotion.Tick)]
    [InlineData(ClockHandMotion.FastTick)]
    [InlineData(ClockHandMotion.Sweep)]
    public void FreeFloatingTargetAngle_HonorsAllMotionsAgainstRadialMinuteTicks(ClockHandMotion motion)
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 00, 02, 100));
        PointF pivot = new(0f, 0f);

        float radial = HandPointingSolver.RadialTargetAngle(time, ClockHandKind.Second, motion, 0.5f);
        float freeFloating = HandPointingSolver.FreeFloatingTargetAngle(
            ClockHandKind.Second,
            pivot,
            time,
            motion,
            0.5f,
            id => ResolveRadialAnchor(id, pivot, 100f));

        Assert.Equal(radial, freeFloating, 2);
    }

    [Theory]
    [InlineData(ClockHandMotion.Crawling)]
    [InlineData(ClockHandMotion.Tick)]
    [InlineData(ClockHandMotion.FastTick)]
    [InlineData(ClockHandMotion.Sweep)]
    public void SubSecondRemainsContinuousAcrossMotionSelections(ClockHandMotion motion)
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 00, 02, 250));

        float radial = HandPointingSolver.RadialTargetAngle(time, ClockHandKind.SubSecond, motion, 0.5f);
        float freeFloating = HandPointingSolver.FreeFloatingTargetAngle(
            ClockHandKind.SubSecond,
            PointF.Empty,
            time,
            motion,
            0.5f,
            _ => PointF.Empty);

        Assert.Equal(time.SubSecondAngle, radial, 3);
        Assert.Equal(time.SubSecondAngle, freeFloating, 3);
    }

    [Theory]
    [InlineData(ClockHandKind.Hour, ClockHandTargetMode.ThemeDefault, false, false, ClockHandTargetMode.Radial)]
    [InlineData(ClockHandKind.Hour, ClockHandTargetMode.ThemeDefault, true, false, ClockHandTargetMode.FreeFloating)]
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.FreeFloating, false, false, ClockHandTargetMode.Radial)]
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.Radial, true, false, ClockHandTargetMode.Radial)]
    [InlineData(ClockHandKind.SubSecond, ClockHandTargetMode.FreeFloating, true, false, ClockHandTargetMode.Radial)]
    // The global switch magnetizes every hand that did not opt out...
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.ThemeDefault, true, true, ClockHandTargetMode.MagneticNumerals)]
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.FreeFloating, true, true, ClockHandTargetMode.MagneticNumerals)]
    [InlineData(ClockHandKind.Second, ClockHandTargetMode.Radial, true, true, ClockHandTargetMode.Radial)]
    // ...and an explicit magnetic request survives a host that never switched it on.
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.MagneticNumerals, true, false, ClockHandTargetMode.MagneticNumerals)]
    [InlineData(ClockHandKind.Hour, ClockHandTargetMode.MagneticNumerals, false, false, ClockHandTargetMode.MagneticNumerals)]
    // Sub-second hands never chase numerals, however the mode is requested.
    [InlineData(ClockHandKind.SubSecond, ClockHandTargetMode.MagneticNumerals, true, true, ClockHandTargetMode.Radial)]
    public void HandTargetModeResolver_ChoosesSafeEffectiveModes(
        ClockHandKind hand,
        ClockHandTargetMode requested,
        bool themeSupportsFreeFloating,
        bool magneticNumeralsEnabled,
        ClockHandTargetMode expected)
    {
        ClockHandTargetMode actual = HandTargetModeResolver.Resolve(
            hand,
            requested,
            themeSupportsFreeFloating,
            magneticNumeralsEnabled);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MagneticHandWithoutNumerals_FallsBackToRadialTime()
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 09, 17, 500));
        HandRotationSolver solver = new();
        var request = new HandRotationRequest
        {
            Hand = ClockHandKind.Minute,
            Pivot = PointF.Empty,
            Time = time,
            RequestedTargetMode = ClockHandTargetMode.MagneticNumerals,
            Motion = ClockHandMotion.Crawling,
            ThemeSupportsFreeFloating = false,
            HandsFollowFaceRotation = true,
            MagneticNumeralsEnabled = true,
            AnchorOf = _ => PointF.Empty,
            NumeralVisibilityOf = _ => null,
            GlideDurationSeconds = 0.5f,
            DeltaSeconds = 1f,
        };

        float actual = solver.Solve(request);

        Assert.Equal(
            HandPointingSolver.RadialTargetAngle(
                time,
                ClockHandKind.Minute,
                ClockHandMotion.Crawling),
            actual,
            3);
    }

    [Fact]
    public void FreeFloatingContinuousGlideDoesNotAccumulateGraceLag()
    {
        HandRotationSolver solver = new();
        PointF pivot = PointF.Empty;
        HandRotationRequest request = new()
        {
            Hand = ClockHandKind.Second,
            Pivot = pivot,
            Time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 0, 2)),
            RequestedTargetMode = ClockHandTargetMode.FreeFloating,
            Motion = ClockHandMotion.Sweep,
            ThemeSupportsFreeFloating = true,
            HandsFollowFaceRotation = true,
            MagneticNumeralsEnabled = false,
            AnchorOf = id => ResolveRadialAnchor(id, pivot, 100f),
            NumeralVisibilityOf = _ => ClockNumeralVisibility.Visible,
            GraceSeconds = 5f,
            GlideDurationSeconds = 0.5f,
            DeltaSeconds = 1f,
        };

        solver.Solve(request);
        request = request with
        {
            Time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 0, 3)),
        };

        float actual = solver.Solve(request);

        Assert.Equal(18f, actual, 3);
    }

    private static ClockTimeSnapshot CreateTimeSnapshot(DateTime now)
    {
        float fractionalSecond = now.Millisecond / 1000f;
        float totalSeconds = now.Second + fractionalSecond;
        float totalMinutes = now.Minute + totalSeconds / 60f;
        float totalHours = (now.Hour % 12) + totalMinutes / 60f;

        return new ClockTimeSnapshot
        {
            Now = now,
            HourAngle = totalHours * 30f,
            MinuteAngle = totalMinutes * 6f,
            SecondAngle = totalSeconds * 6f,
            SubSecondAngle = fractionalSecond * 360f,
        };
    }

    private static PointF ResolveRadialAnchor(ClockElementId id, PointF center, float radius)
        => id.Kind switch
        {
            ClockElementKind.HourMarker => ClockMath.PointOnDial(center, radius, id.Index * 30f),
            ClockElementKind.MinuteTick => ClockMath.PointOnDial(center, radius, id.Index * 6f),
            _ => center,
        };
}
