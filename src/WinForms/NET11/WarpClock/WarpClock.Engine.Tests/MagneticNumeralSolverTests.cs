using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine.Tests;

public sealed class MagneticNumeralSolverTests
{
    [Theory]
    [InlineData(ClockHandKind.Hour, 20, 9, 0, 8, 4.5f)]
    [InlineData(ClockHandKind.Minute, 20, 9, 0, 1, 24f)]
    [InlineData(ClockHandKind.Minute, 20, 10, 0, 2, 0f)]
    [InlineData(ClockHandKind.Second, 20, 9, 4, 0, 24f)]
    [InlineData(ClockHandKind.Second, 20, 9, 5, 1, 0f)]
    [InlineData(ClockHandKind.Second, 20, 9, 59, 11, 24f)]
    public void MagneticPosition_UsesCurrentNumeralAndClockwiseCompensation(
        ClockHandKind hand,
        int hour,
        int minute,
        int second,
        int expectedNumeral,
        float expectedCompensation)
    {
        MagneticNumeralPosition actual = MagneticNumeralPosition.Resolve(
            hand,
            CreateTime(hour, minute, second));

        Assert.Equal(expectedNumeral, actual.NumeralIndex);
        Assert.Equal(expectedCompensation, actual.CompensationDegrees, 3);
    }

    [Theory]
    [InlineData(ClockHandKind.Hour, 20, 9, 30, 244.75f)]
    [InlineData(ClockHandKind.Minute, 20, 9, 30, 57f)]
    [InlineData(ClockHandKind.Second, 20, 9, 59, 354f)]
    public void RadialNumerals_ReproduceAuthoritativeAngles(
        ClockHandKind hand,
        int hour,
        int minute,
        int second,
        float expectedAngle)
    {
        MagneticNumeralSolver solver = new();
        PointF[] anchors = CreateAnchors(index => index * 30f);

        float? actual = Solve(solver, hand, CreateTime(hour, minute, second), anchors);

        Assert.NotNull(actual);
        Assert.Equal(expectedAngle, actual.Value, 3);
    }

    [Fact]
    public void MinuteHand_UsesOnlyCurrentScatteredNumeral()
    {
        MagneticNumeralSolver solver = new();
        PointF[] anchors = CreateAnchors(index => index switch
        {
            1 => 100f,
            2 => 280f,
            _ => index * 30f,
        });

        float? actual = Solve(solver, ClockHandKind.Minute, CreateTime(20, 9, 0), anchors);

        Assert.NotNull(actual);
        Assert.Equal(124f, actual.Value, 3);

        anchors[2] = ClockMath.PointOnDial(PointF.Empty, 100f, 10f);
        float? afterLaterNumeralMoved = Solve(
            solver,
            ClockHandKind.Minute,
            CreateTime(20, 9, 0),
            anchors);

        Assert.Equal(actual.Value, afterLaterNumeralMoved!.Value, 3);
    }

    [Fact]
    public void CurrentNumeralMovement_IsTrackedWithCompensation()
    {
        MagneticNumeralSolver solver = new();
        PointF[] anchors = CreateAnchors(index => index * 30f);

        Solve(solver, ClockHandKind.Minute, CreateTime(20, 9, 0), anchors);
        anchors[1] = ClockMath.PointOnDial(PointF.Empty, 100f, 80f);

        float? actual = Solve(solver, ClockHandKind.Minute, CreateTime(20, 9, 0), anchors);

        Assert.Equal(104f, actual!.Value, 3);
    }

    [Fact]
    public void InvisibleCurrentNumeral_HoldsLastAnchorAndKeepsAdvancing()
    {
        MagneticNumeralSolver solver = new();
        PointF[] anchors = CreateAnchors(index => index * 30f);
        ClockNumeralVisibility[] visibility = VisibleNumerals();

        Solve(solver, ClockHandKind.Minute, CreateTime(20, 5, 0), anchors, visibility);
        visibility[2] = ClockNumeralVisibility.Invisible;

        float? actual = Solve(
            solver,
            ClockHandKind.Minute,
            CreateTime(20, 10, 0),
            anchors,
            visibility);

        Assert.Equal(60f, actual!.Value, 3);
    }

    [Fact]
    public void NoValidNumeral_ReturnsNoMagneticTarget()
    {
        MagneticNumeralSolver solver = new();
        PointF[] anchors = CreateAnchors(index => index * 30f);
        ClockNumeralVisibility[] visibility =
            Enumerable.Repeat(ClockNumeralVisibility.Invisible, 12).ToArray();

        float? actual = Solve(
            solver,
            ClockHandKind.Second,
            CreateTime(20, 9, 17),
            anchors,
            visibility);

        Assert.Null(actual);
    }

    [Fact]
    public void TransparentNumeral_RemainsTargetable()
    {
        MagneticNumeralSolver solver = new();
        PointF[] anchors = CreateAnchors(index => index * 30f);
        ClockNumeralVisibility[] visibility = VisibleNumerals();
        visibility[1] = ClockNumeralVisibility.Transparent;

        float? actual = Solve(
            solver,
            ClockHandKind.Second,
            CreateTime(20, 9, 5),
            anchors,
            visibility);

        Assert.Equal(30f, actual!.Value, 3);
    }

    private static float? Solve(
        MagneticNumeralSolver solver,
        ClockHandKind hand,
        ClockTimeSnapshot time,
        PointF[] anchors,
        ClockNumeralVisibility[]? visibility = null)
        => solver.Solve(
            hand,
            PointF.Empty,
            time,
            index => (visibility ?? VisibleNumerals())[index],
            index => anchors[index]);

    private static PointF[] CreateAnchors(Func<int, float> angleOf)
        => Enumerable.Range(0, 12)
            .Select(index => ClockMath.PointOnDial(PointF.Empty, 100f, angleOf(index)))
            .ToArray();

    private static ClockNumeralVisibility[] VisibleNumerals()
        => Enumerable.Repeat(ClockNumeralVisibility.Visible, 12).ToArray();

    private static ClockTimeSnapshot CreateTime(
        int hour,
        int minute,
        int second,
        int millisecond = 0)
    {
        DateTime now = new(2026, 8, 19, hour, minute, second, millisecond, DateTimeKind.Unspecified);
        float fractionalSecond = second + (millisecond / 1000f);
        float fractionalMinute = minute + (fractionalSecond / 60f);
        float fractionalHour = (hour % 12) + (fractionalMinute / 60f);

        return new ClockTimeSnapshot
        {
            Now = now,
            HourAngle = fractionalHour * 30f,
            MinuteAngle = fractionalMinute * 6f,
            SecondAngle = fractionalSecond * 6f,
            SubSecondAngle = (millisecond / 1000f) * 360f,
        };
    }
}
