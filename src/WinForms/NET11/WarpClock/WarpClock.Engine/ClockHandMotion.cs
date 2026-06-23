namespace WarpClock.Engine;

/// <summary>
///  How a hand advances toward its authoritative target in a radial layout.
/// </summary>
public enum ClockHandMotion
{
    /// <summary>Continuous smooth sweep — the hand tracks the exact sub-second angle.</summary>
    Crawling,

    /// <summary>
    ///  Glides to the next mark in one accelerated-then-decelerated move (ease-in-out)
    ///  that completes within <c>GlideDurationSeconds</c>, then rests on the mark until
    ///  the next unit. For the second hand this looks like a quick "swing" each second
    ///  followed by a brief pause on the numeral.
    /// </summary>
    Sweep,

    /// <summary>Several discrete steps per unit — a high-end sweep movement feel (what used to be called "Sweep").</summary>
    FastTick,

    /// <summary>One discrete step per unit (classic quartz tick).</summary>
    Tick,
}
