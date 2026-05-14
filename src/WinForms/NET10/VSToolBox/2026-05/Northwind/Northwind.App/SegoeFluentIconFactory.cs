using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Northwind.App;

internal static class SegoeFluentIconFactory
{
    private static readonly string[] FontCandidates = ["Segoe Fluent Icons", "Segoe MDL2 Assets"];

    public static class Glyphs
    {
        public const string Add    = "\uE710";
        public const string Edit   = "\uE70F";
        public const string Cancel = "\uE711";
        public const string Save   = "\uE74E";
    }

    /// <summary>
    /// Renders a single glyph from Segoe Fluent Icons (or MDL2 fallback) onto a
    /// transparent <see cref="Bitmap"/> of the requested <paramref name="size"/>.
    /// </summary>
    public static Image CreateIcon(string glyph, int size = 36, Color? foreColor = null)
    {
        var color = foreColor ?? Color.FromArgb(50, 50, 50);
        var bmp   = new Bitmap(size, size, PixelFormat.Format32bppArgb);

        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        const int padding = 6;
        float fontSize = size - padding * 2;

        using var font  = ResolveFont(fontSize);
        using var brush = new SolidBrush(color);
        using var sf    = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        g.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), sf);
        return bmp;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static Font ResolveFont(float sizeInPixels)
    {
        foreach (var name in FontCandidates)
        {
            try
            {
                var f = new Font(name, sizeInPixels, GraphicsUnit.Pixel);
                if (string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase))
                    return f;
                f.Dispose();
            }
            catch { /* font not installed */ }
        }

        // ultimate fallback — system default (icon will look wrong, but won't crash)
        return new Font(SystemFonts.DefaultFont.FontFamily, sizeInPixels, GraphicsUnit.Pixel);
    }
}
