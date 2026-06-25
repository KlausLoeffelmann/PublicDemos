namespace WarpClock.Engine;

/// <summary>
///  How the clock renders the "<c>{theme name} - {author}</c>" info text over the dial.
/// </summary>
public enum RenderThemeInfo
{
    /// <summary>Never show the theme info.</summary>
    Never,

    /// <summary>Show it statically at the position given by <see cref="ThemeInfoPlacement"/>.</summary>
    FixedPosition,

    /// <summary>
    ///  Repeatedly fade it in (character-wise), hold, and fade it out at the position given
    ///  by <see cref="ThemeInfoPlacement"/>.
    /// </summary>
    FadeInAndOutAtFixedPosition,

    /// <summary>
    ///  Fade it in on the left edge (rotated 90° counter-clockwise), hold, fade out; after a
    ///  pause repeat on the right edge (rotated 90° clockwise), and so on — ignoring
    ///  <see cref="ThemeInfoPlacement"/>.
    /// </summary>
    FadeAlternateScreenSides,
}
