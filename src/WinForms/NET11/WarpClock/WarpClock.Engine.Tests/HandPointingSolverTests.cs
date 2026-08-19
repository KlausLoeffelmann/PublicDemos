using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine.Tests;

public sealed class HandPointingSolverTests
{
    [Theory]
    [InlineData(ClockHandMotion.Crawling, 12.6f)]
    [InlineData(ClockHandMotion.Tick, 12f)]
    [InlineData(ClockHandMotion.FastTick, 12f)]
    [InlineData(ClockHandMotion.Sweep, 6.192f)]
    public void RadialTargetAngle_HonorsAllMotions(ClockHandMotion motion, float expectedDegrees)
    {
        ClockTimeSnapshot time = CreateTimeSnapshot(new DateTime(2024, 01, 01, 12, 00, 02, 100));

        float angle = HandPointingSolver.RadialTargetAngle(time, ClockHandKind.Second, motion, 0.5f);

        Assert.Equal(expectedDegrees, angle, 3);
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
    [InlineData(ClockHandKind.Hour, ClockHandTargetMode.ThemeDefault, false, ClockHandTargetMode.Radial)]
    [InlineData(ClockHandKind.Hour, ClockHandTargetMode.ThemeDefault, true, ClockHandTargetMode.FreeFloating)]
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.FreeFloating, false, ClockHandTargetMode.Radial)]
    [InlineData(ClockHandKind.Minute, ClockHandTargetMode.Radial, true, ClockHandTargetMode.Radial)]
    [InlineData(ClockHandKind.SubSecond, ClockHandTargetMode.FreeFloating, true, ClockHandTargetMode.Radial)]
    public void HandTargetModeResolver_ChoosesSafeEffectiveModes(
        ClockHandKind hand,
        ClockHandTargetMode requested,
        bool themeSupportsFreeFloating,
        ClockHandTargetMode expected)
    {
        ClockHandTargetMode actual = HandTargetModeResolver.Resolve(hand, requested, themeSupportsFreeFloating);

        Assert.Equal(expected, actual);
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
