namespace WarpClock.Engine;

/// <summary>
///  Where the theme-info text sits for the fixed-position render modes.
/// </summary>
public enum ThemeInfoPlacement
{
    /// <summary>Down the left edge, rotated 90° counter-clockwise (reads bottom-to-top).</summary>
    LeftScreenSide,

    /// <summary>Down the right edge, rotated 90° clockwise (reads top-to-bottom).</summary>
    RightScreenSide,

    /// <summary>Centered on the clock face on two lines: theme name above, author below.</summary>
    OnClockFace,
}
