namespace WarpClock.Abstractions;

/// <summary>
///  Chooses how a hand element finds its authoritative target.
/// </summary>
public enum ClockHandTargetMode
{
    /// <summary>Use the theme/engine default for the active layout.</summary>
    ThemeDefault,

    /// <summary>Point at the authoritative radial dial angle.</summary>
    Radial,

    /// <summary>Point at free-floating interpolated target anchors when safely supported.</summary>
    FreeFloating
}
