namespace WarpClock.Abstractions;

/// <summary>
///  The contract a clock theme implements. Built-in themes implement it in-process;
///  plug-in themes implement it in a drop-in assembly discovered from the plugins
///  directory. A theme describes its elements, supplies a layout and a renderer, and
///  optionally an animator — the engine owns composition, time, and hand pointing.
/// </summary>
public interface IClockTheme
{
    /// <summary>A short, unique, human-readable theme name.</summary>
    string Name { get; }

    /// <summary>A one-line description of the theme.</summary>
    string Description { get; }

    /// <summary>The theme author.</summary>
    string Author { get; }

    /// <summary>What the theme needs from the engine.</summary>
    ThemeCapabilities Capabilities { get; }

    /// <summary>
    ///  Returns the set of elements the engine should materialize, each as its own
    ///  visual. Called once when the theme is activated.
    /// </summary>
    IReadOnlyList<ClockElementDescriptor> CreateElements();

    /// <summary>Creates the layout that positions the elements.</summary>
    IClockLayout CreateLayout();

    /// <summary>Creates the renderer that draws the elements' content.</summary>
    IClockElementRenderer CreateRenderer();

    /// <summary>Creates the optional per-tick animator, or <see langword="null"/> for a static theme.</summary>
    IThemeAnimator? CreateAnimator();
}
