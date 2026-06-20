namespace WarpClock.Abstractions;

/// <summary>
///  A read-only snapshot of the authoritative time and hand angles for the current
///  frame. Produced solely by the engine; themes consume it but cannot alter it.
/// </summary>
/// <remarks>
///  All angles are in degrees, measured clockwise from the 12 o'clock position,
///  and are continuous (sub-second precise) so themes can drive smooth effects.
/// </remarks>
public readonly record struct ClockTimeSnapshot
{
    /// <summary>The effective wall-clock time being displayed (may include a demo offset).</summary>
    public required DateTime Now { get; init; }

    /// <summary>Authoritative hour-hand angle (clockwise from 12), continuous.</summary>
    public required float HourAngle { get; init; }

    /// <summary>Authoritative minute-hand angle (clockwise from 12), continuous.</summary>
    public required float MinuteAngle { get; init; }

    /// <summary>Authoritative second-hand angle (clockwise from 12), continuous.</summary>
    public required float SecondAngle { get; init; }

    /// <summary>Authoritative fractional-second angle (clockwise from 12), continuous.</summary>
    public required float SubSecondAngle { get; init; }

    /// <summary>Hour in 12-hour form (1..12).</summary>
    public int Hour12 => ((Now.Hour + 11) % 12) + 1;

    /// <summary>Minute (0..59).</summary>
    public int Minute => Now.Minute;

    /// <summary>Second (0..59).</summary>
    public int Second => Now.Second;

    /// <summary>Millisecond (0..999).</summary>
    public int Millisecond => Now.Millisecond;
}
