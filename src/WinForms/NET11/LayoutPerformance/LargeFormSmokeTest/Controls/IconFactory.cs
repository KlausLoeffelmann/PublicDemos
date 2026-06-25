namespace LargeFormSmokeTest.Controls;

using System.Drawing.Drawing2D;
using System.Drawing.Text;

/// <summary>
///  Well-known Segoe Fluent Icons glyph code points used by the app's toolbars. Values are the
///  Unicode private-use code points of the glyphs (also present in Segoe MDL2 Assets).
/// </summary>
public enum FluentGlyph
{
    /// <summary>Contact / person glyph — used for "Edit person".</summary>
    Contact = 0xE77B,

    /// <summary>Open-file glyph — used for "Open declaration".</summary>
    OpenFile = 0xE8E5,

    /// <summary>Pencil glyph — used for "Edit tax form".</summary>
    Edit = 0xE70F,

    /// <summary>Save (floppy) glyph.</summary>
    Save = 0xE74E,

    /// <summary>Save-and-close composite glyph (uses Accept).</summary>
    Accept = 0xE73E,

    /// <summary>Export / share glyph.</summary>
    Export = 0xEDE1,

    /// <summary>Cancel / close glyph.</summary>
    Cancel = 0xE711
}

/// <summary>
///  Renders Segoe Fluent Icons (falling back to Segoe MDL2 Assets) glyphs into crisp, DPI-aware
///  bitmaps. Avoids shipping any bitmap resources — every toolbar icon is drawn from a font.
/// </summary>
public static class IconFactory
{
    private const string PrimaryFontName = "Segoe Fluent Icons";
    private const string FallbackFontName = "Segoe MDL2 Assets";

    private static readonly string s_glyphFontName = ResolveGlyphFont();

    /// <summary>
    ///  Produces a square icon of <paramref name="size"/> pixels containing the given
    ///  <paramref name="glyph"/> painted in <paramref name="color"/> on a transparent background.
    /// </summary>
    public static Image GetIcon(FluentGlyph glyph, int size, Color color)
    {
        Bitmap bitmap = new(size, size);
        bitmap.MakeTransparent();

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

        // The glyph is drawn at ~70% of the icon box so it has a little breathing room.
        float emSize = size * 0.7f;

        using Font font = new(s_glyphFontName, emSize, FontStyle.Regular, GraphicsUnit.Pixel);
        using SolidBrush brush = new(color);

        string text = char.ConvertFromUtf32((int)glyph);
        StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        graphics.DrawString(text, font, brush, new RectangleF(0, 0, size, size), format);

        return bitmap;
    }

    /// <summary>
    ///  Picks the best available glyph font, preferring Segoe Fluent Icons and falling back to
    ///  Segoe MDL2 Assets, then to a generic sans-serif so rendering never throws.
    /// </summary>
    private static string ResolveGlyphFont()
    {
        if (IsFontInstalled(PrimaryFontName))
        {
            return PrimaryFontName;
        }

        if (IsFontInstalled(FallbackFontName))
        {
            return FallbackFontName;
        }

        return FontFamily.GenericSansSerif.Name;
    }

    private static bool IsFontInstalled(string name)
    {
        try
        {
            using FontFamily family = new(name);

            return string.Equals(family.Name, name, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
