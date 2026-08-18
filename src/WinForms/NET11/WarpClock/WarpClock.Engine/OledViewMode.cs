namespace WarpClock.Engine;

/// <summary>
///  Scene-wide view transforms the engine can apply independently of any theme.
/// </summary>
public enum OledViewMode
{
    /// <summary>Render the clock on the full surface with no anti-burn-in transform.</summary>
    Off,

    /// <summary>
    ///  Slowly move and gently shrink/regrow the complete scene to reduce OLED burn-in
    ///  risk while keeping the engine's geometry and hand-pointing time-correct.
    /// </summary>
    General,
}
