namespace WarpClock.Abstractions;

/// <summary>
///  How a hand advances toward its authoritative target.
/// </summary>
public enum ClockHandMotion
{
    /// <summary>
    ///  Crawls to the next step with an eased movement, then rests there until the
    ///  next step begins.
    /// </summary>
    Crawling,

    /// <summary>
    ///  Glides continuously without stopping and tracks the exact fractional time.
    /// </summary>
    Sweep,

    /// <summary>Several discrete steps per unit for a high-end mechanical feel.</summary>
    FastTick,

    /// <summary>One discrete step per unit (classic quartz tick).</summary>
    Tick,
}
