namespace WarpClock.Abstractions;

/// <summary>
///  Identifies the time target a hand visual tracks. The engine derives a hand's
///  rotation from this target so the hand always points at the authoritative
///  position — a theme can never set a hand angle directly.
/// </summary>
public enum ClockHandKind
{
    /// <summary>Not a hand.</summary>
    None,

    /// <summary>Tracks the hour target (0..12h, continuous).</summary>
    Hour,

    /// <summary>Tracks the minute target (0..60m, continuous).</summary>
    Minute,

    /// <summary>Tracks the second target (0..60s, continuous).</summary>
    Second,

    /// <summary>Tracks the fractional-second target (0..1s, continuous).</summary>
    SubSecond
}
