namespace DrumMachine.Demo.Controls;

/// <summary>
///  Identifies an editor command independently of a particular symbol font.
/// </summary>
internal enum ToolbarSymbol
{
    /// <summary>
    ///  Creates a new loop document.
    /// </summary>
    New,

    /// <summary>
    ///  Opens a loop document.
    /// </summary>
    Open,

    /// <summary>
    ///  Saves the current document.
    /// </summary>
    Save,

    /// <summary>
    ///  Starts or resumes playback.
    /// </summary>
    Play,

    /// <summary>
    ///  Pauses playback.
    /// </summary>
    Pause,

    /// <summary>
    ///  Stops and resets playback.
    /// </summary>
    Stop,

    /// <summary>
    ///  Toggles repetition of the complete loop.
    /// </summary>
    Loop,

    /// <summary>
    ///  Toggles the shared metallic sound layer.
    /// </summary>
    Metallic,

    /// <summary>
    ///  Auditions a sound.
    /// </summary>
    Audition,

    /// <summary>
    ///  Opens application options.
    /// </summary>
    Options,

    /// <summary>
    ///  Undoes a document edit.
    /// </summary>
    Undo,

    /// <summary>
    ///  Redoes a document edit.
    /// </summary>
    Redo,

    /// <summary>
    ///  Closes the application.
    /// </summary>
    Quit
}

/// <summary>
///  Maps commands to the documented glyphs of the two supported Windows icon fonts.
/// </summary>
/// <remarks>
///  These particular mappings are shared by both fonts, not by arbitrary PUA fonts.
///  Names and code points were checked against the Microsoft Learn catalogs.
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/windows/apps/design/iconography/segoe-fluent-icons-font"/>
/// <seealso href="https://learn.microsoft.com/en-us/windows/apps/design/iconography/segoe-ui-symbol-font"/>
internal static class ToolbarGlyphCatalog
{
    /// <summary>
    ///  Gets the preferred Windows 11 symbol-font family name.
    /// </summary>
    internal const string FluentFontFamilyName = "Segoe Fluent Icons";

    /// <summary>
    ///  Gets the explicitly supported earlier Windows symbol-font family name.
    /// </summary>
    internal const string FallbackFontFamilyName = "Segoe MDL2 Assets";

    /// <summary>
    ///  Gets the official glyph name and code point for a supported font.
    /// </summary>
    internal static (string Name, char CodePoint) GetGlyph(ToolbarSymbol symbol, string fontFamilyName)
    {
        if (!string.Equals(fontFamilyName, FluentFontFamilyName, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(fontFamilyName, FallbackFontFamilyName, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The glyph catalog only supports Segoe Fluent Icons and Segoe MDL2 Assets.",
                nameof(fontFamilyName));
        }

        return symbol switch
        {
            ToolbarSymbol.New => ("Page", '\uE7C3'),
            ToolbarSymbol.Open => ("FolderOpen", '\uE838'),
            ToolbarSymbol.Save => ("Save", '\uE74E'),
            ToolbarSymbol.Play => ("Play", '\uE768'),
            ToolbarSymbol.Pause => ("Pause", '\uE769'),
            ToolbarSymbol.Stop => ("Stop", '\uE71A'),
            ToolbarSymbol.Loop => ("RepeatAll", '\uE8EE'),
            ToolbarSymbol.Metallic => ("MapLayers", '\uE81E'),
            ToolbarSymbol.Audition => ("Volume", '\uE767'),
            ToolbarSymbol.Options => (fontFamilyName.Equals(FluentFontFamilyName, StringComparison.OrdinalIgnoreCase)
                ? "Settings" : "Setting", '\uE713'),
            ToolbarSymbol.Undo => ("Undo", '\uE7A7'),
            ToolbarSymbol.Redo => ("Redo", '\uE7A6'),
            ToolbarSymbol.Quit => ("Cancel", '\uE711'),
            _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, "Unknown toolbar symbol.")
        };
    }
}
