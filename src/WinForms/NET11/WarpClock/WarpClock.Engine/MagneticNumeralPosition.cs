using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Maps authoritative time to the current magnetic numeral and the clockwise
///  compensation within that numeral's 30-degree interval.
/// </summary>
internal readonly record struct MagneticNumeralPosition(int NumeralIndex, float CompensationDegrees)
{
    private const float DegreesPerNumeral = 30f;

    public static MagneticNumeralPosition Resolve(
        ClockHandKind hand,
        ClockTimeSnapshot time,
        ClockHandMotion motion = ClockHandMotion.Sweep,
        float glideDurationSeconds = 0.5f)
    {
        float unitPosition = HandPointingSolver.SelectUnitPosition(
            time,
            hand,
            motion,
            glideDurationSeconds);
        float numeralPosition = hand switch
        {
            ClockHandKind.Hour => unitPosition,
            ClockHandKind.Minute => unitPosition / 5f,
            ClockHandKind.Second => unitPosition / 5f,
            _ => throw new ArgumentOutOfRangeException(nameof(hand), hand, "Only hour, minute, and second hands can target magnetic numerals."),
        };

        float whole = MathF.Floor(numeralPosition);
        int index = (int)whole % 12;
        return new MagneticNumeralPosition(
            index,
            (numeralPosition - whole) * DegreesPerNumeral);
    }
}
