namespace WarpClock.Engine;

/// <summary>
///  How a hand advances toward its authoritative target in a radial layout.
/// </summary>
public enum ClockHandMotion
{
    /// <summary>Continuous smooth sweep — the hand tracks the exact sub-second angle.</summary>
    Crawling,

    /// <summary>Several discrete steps per second (a high-end sweep movement feel).</summary>
    Sweep,

    /// <summary>One discrete step per unit (classic quartz tick).</summary>
    Tick,
}
