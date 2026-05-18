using System.Drawing.Text;

namespace Northwind.App;

/// <summary>
/// Renders glyph-based icons from the Segoe Fluent Icons font (or Segoe MDL2 Assets as fallback)
/// as Bitmap images, so ToolStrip buttons can display icons without referencing image files.
/// </summary>
internal static class SegoeFluentIconFactory
{
    private const string FluentFontName = "Segoe Fluent Icons";
    private const string FallbackFontName = "Segoe MDL2 Assets";

    // Shared glyph code points (compatible across both font families)
    public const string AddGlyph = "\uE710";
    public const string EditGlyph = "\uE70F";
    public const string CancelGlyph = "\uE711";
    public const string SaveGlyph = "\uE74E";

    /// <summary>
    /// Creates a square bitmap with the given glyph rendered centred, with padding around it.
    /// </summary>
    /// <param name="glyph">Unicode character(s) to draw.</param>
    /// <param name="size">Total bitmap size in pixels (width and height).</param>
    /// <param name="foreColor">Glyph foreground colour.</param>
    /// <param name="padding">Pixel padding to leave inside each edge.</param>
    public static Bitmap CreateIconBitmap(string glyph, int size, Color foreColor, int padding = 6)
    {
        var bitmap = new Bitmap(size, size);

        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        string fontName = IsFontInstalled(FluentFontName) ? FluentFontName : FallbackFontName;
        float fontSize = size - (padding * 2);

        using var font = new Font(fontName, fontSize, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(foreColor);

        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        graphics.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), format);

        return bitmap;
    }

    private static bool IsFontInstalled(string fontName)
    {
        using var collection = new InstalledFontCollection();

        foreach (var family in collection.Families)
        {
            if (family.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
