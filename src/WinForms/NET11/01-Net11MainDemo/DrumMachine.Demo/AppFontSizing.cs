namespace DrumMachine.Demo;

/// <summary>
///  Derives application fonts from the current WinForms default without cumulative resizing.
/// </summary>
internal static class AppFontSizing
{
    /// <summary>
    ///  Creates a font in the same family, style, and character set with the selected point increment.
    /// </summary>
    internal static Font CreateFont(Font currentFont, AppFontSize size)
    {
        ArgumentNullException.ThrowIfNull(currentFont);
        float increment = GetPointIncrement(size);
        return new Font(
            currentFont.FontFamily,
            currentFont.SizeInPoints + increment,
            currentFont.Style,
            GraphicsUnit.Point,
            currentFont.GdiCharSet,
            currentFont.GdiVerticalFont);
    }

    /// <summary>
    ///  Gets the additive point-size change for a persisted selection.
    /// </summary>
    internal static float GetPointIncrement(AppFontSize size)
        => size switch
        {
            AppFontSize.Small => 0f,
            AppFontSize.Normal => 2f,
            AppFontSize.Large => 4f,
            AppFontSize.Xxl => 6f,
            _ => throw new ArgumentOutOfRangeException(nameof(size))
        };
}
