using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Maps authoritative time to the current magnetic numeral and the clockwise
///  compensation within that numeral's 30-degree interval.
/// </summary>
internal readonly record struct MagneticNumeralPosition(int NumeralIndex, float CompensationDegrees)
{
    private const float DegreesPerNumeral = 30f;

    public static MagneticNumeralPosition Resolve(ClockHandKind hand, ClockTimeSnapshot time)
    {
        DateTime now = time.Now;
        float fractionalSecond = now.Second + (now.Millisecond / 1000f);
        float fractionalMinute = now.Minute + (fractionalSecond / 60f);
        float numeralPosition = hand switch
        {
            ClockHandKind.Hour => (now.Hour % 12) + (fractionalMinute / 60f),
            ClockHandKind.Minute => fractionalMinute / 5f,
            ClockHandKind.Second => fractionalSecond / 5f,
            _ => throw new ArgumentOutOfRangeException(nameof(hand), hand, "Only hour, minute, and second hands can target magnetic numerals."),
        };

        float whole = MathF.Floor(numeralPosition);
        int index = (int)whole % 12;
        return new MagneticNumeralPosition(
            index,
            (numeralPosition - whole) * DegreesPerNumeral);
    }
}
