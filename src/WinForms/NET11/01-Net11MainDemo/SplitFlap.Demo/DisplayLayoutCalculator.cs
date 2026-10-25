namespace SplitFlap.Demo;

/// <summary>
///  Provides deterministic calculations used by aspect-ratio and fit-to-screen commands.
/// </summary>
internal static class DisplayLayoutCalculator
{
    /// <summary>
    ///  Describes a grid and font combination selected for an available screen area.
    /// </summary>
    internal readonly record struct DisplayFit(int Rows, int Columns, float FontSize);

    /// <summary>
    ///  Scales a preferred display size to fit inside a target while preserving its proportions.
    /// </summary>
    public static Size ScaleToFit(Size preferred, Size target)
    {
        if (preferred.Width <= 0 || preferred.Height <= 0 || target.Width <= 0 || target.Height <= 0)
        {
            return Size.Empty;
        }

        double scale = Math.Min(
            target.Width / (double)preferred.Width,
            target.Height / (double)preferred.Height);

        return new Size(
            Math.Max(1, (int)Math.Round(preferred.Width * scale)),
            Math.Max(1, (int)Math.Round(preferred.Height * scale)));
    }

    /// <summary>
    ///  Calculates a font size that fits the current grid into the requested target size.
    /// </summary>
    public static float CalculateFontSize(
        float currentFontSize,
        Size currentPreferredSize,
        Size targetSize,
        float minimum = 4f,
        float maximum = 400f)
    {
        Size fitted = ScaleToFit(currentPreferredSize, targetSize);

        if (fitted.IsEmpty || currentPreferredSize.Width <= 0)
        {
            return Math.Clamp(currentFontSize, minimum, maximum);
        }

        double scale = fitted.Width / (double)currentPreferredSize.Width;
        return Math.Clamp((float)(currentFontSize * scale), minimum, maximum);
    }

    /// <summary>
    ///  Selects row and column counts at the current readable size, then scales the font so the
    ///  resulting grid fills the target without changing the inferred character-cell proportions.
    /// </summary>
    public static DisplayFit CalculateGridFit(
        int currentRows,
        int currentColumns,
        float currentFontSize,
        Size currentPreferredSize,
        Size targetSize)
    {
        if (currentRows <= 0
            || currentColumns <= 0
            || currentPreferredSize.Width <= 0
            || currentPreferredSize.Height <= 0
            || targetSize.Width <= 0
            || targetSize.Height <= 0)
        {
            return new DisplayFit(
                Math.Clamp(currentRows, 1, 64),
                Math.Clamp(currentColumns, 1, 256),
                Math.Clamp(currentFontSize, 4f, 400f));
        }

        double cellWidth = currentPreferredSize.Width / (double)currentColumns;
        double cellHeight = currentPreferredSize.Height / (double)currentRows;
        int rows = Math.Clamp((int)Math.Round(targetSize.Height / cellHeight), 4, 16);
        int columns = Math.Clamp((int)Math.Round(targetSize.Width / cellWidth), 32, 80);

        Size estimated = new(
            Math.Max(1, (int)Math.Round(columns * cellWidth)),
            Math.Max(1, (int)Math.Round(rows * cellHeight)));
        float fontSize = CalculateFontSize(currentFontSize, estimated, targetSize);

        return new DisplayFit(rows, columns, fontSize);
    }
}
