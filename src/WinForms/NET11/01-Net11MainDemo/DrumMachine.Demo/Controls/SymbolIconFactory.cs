using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;

namespace DrumMachine.Demo.Controls;

/// <summary>
///  Renders installed Windows symbol-font glyphs into caller-owned transparent bitmaps.
/// </summary>
/// <remarks>
///  Create and dispose this factory on the UI thread. It owns its font family, not the
///  returned images. No fonts or images are downloaded, and body-font substitution is rejected.
/// </remarks>
internal sealed class SymbolIconFactory : IDisposable
{
    private readonly FontFamily _fontFamily;
    private bool _disposed;

    /// <summary>
    ///  Selects Segoe Fluent Icons, or explicitly falls back to installed Segoe MDL2 Assets.
    /// </summary>
    /// <exception cref="InvalidOperationException">Neither supported font is installed or its glyphs are unavailable.</exception>
    internal SymbolIconFactory()
        : this(SelectInstalledFontFamilyName())
    {
    }

    /// <summary>
    ///  Selects one of the two supported installed families explicitly, including for fallback verification.
    /// </summary>
    internal SymbolIconFactory(string fontFamilyName)
    {
        ArgumentNullException.ThrowIfNull(fontFamilyName);
        ToolbarGlyphCatalog.GetGlyph(ToolbarSymbol.New, fontFamilyName);

        try
        {
            _fontFamily = new FontFamily(fontFamilyName);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"The symbol font '{fontFamilyName}' is not installed.", ex);
        }

        try
        {
            if (!string.Equals(_fontFamily.Name, fontFamilyName, StringComparison.OrdinalIgnoreCase)
                || !_fontFamily.IsStyleAvailable(FontStyle.Regular))
            {
                throw new InvalidOperationException($"The symbol font '{fontFamilyName}' cannot be selected without substitution.");
            }

            FontFamilyName = _fontFamily.Name;
            UsesFallback = string.Equals(FontFamilyName, ToolbarGlyphCatalog.FallbackFontFamilyName,
                StringComparison.OrdinalIgnoreCase);
            ValidateInstalledGlyphs();
        }
        catch
        {
            _fontFamily.Dispose();
            throw;
        }
    }

    /// <summary>
    ///  Gets the actual, verified installed font family used for every icon.
    /// </summary>
    internal string FontFamilyName { get; }

    /// <summary>
    ///  Gets whether the explicitly supported Segoe MDL2 Assets fallback is in use.
    /// </summary>
    internal bool UsesFallback { get; }

    /// <summary>
    ///  Creates a newly rendered bitmap at the requested device resolution and foreground color.
    /// </summary>
    /// <param name="symbol">The command glyph to render.</param>
    /// <param name="logicalSize">The square icon size at 96 DPI, including 16 for menus and 32, 48, or 64 for toolbars.</param>
    /// <param name="dpi">The destination monitor's DPI.</param>
    /// <param name="foreground">The theme or high-contrast foreground color.</param>
    /// <returns>A transparent bitmap that the caller must dispose.</returns>
    internal Bitmap Create(ToolbarSymbol symbol, int logicalSize, int dpi, Color foreground)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        (string _, char codePoint) = ToolbarGlyphCatalog.GetGlyph(symbol, FontFamilyName);
        int pixelSize = GetPixelSize(logicalSize, dpi);

        using StringFormat format = StringFormat.GenericTypographic;
        format.FormatFlags |= StringFormatFlags.NoFontFallback | StringFormatFlags.NoWrap;
        using GraphicsPath outline = new();

        // AddString obtains the installed glyph's outline, not hand-drawn icon geometry.
        // Its em size is in device pixels; this never scales the application's UI font.
        outline.AddString(codePoint.ToString(), _fontFamily, (int)FontStyle.Regular,
            pixelSize, PointF.Empty, format);
        RectangleF ink = outline.GetBounds();
        if (ink.Width <= 0 || ink.Height <= 0)
        {
            throw new InvalidOperationException($"The symbol font '{FontFamilyName}' has no outline for U+{(int)codePoint:X4}.");
        }

        float padding = pixelSize / 8f;
        float scale = Math.Min(1f, (pixelSize - 2f * padding) / Math.Max(ink.Width, ink.Height));
        using Matrix position = new(scale, 0, 0, scale,
            (pixelSize - ink.Width * scale) / 2f - ink.X * scale,
            (pixelSize - ink.Height * scale) / 2f - ink.Y * scale);
        outline.Transform(position);

        Bitmap bitmap = new(pixelSize, pixelSize, PixelFormat.Format32bppPArgb);
        try
        {
            bitmap.SetResolution(dpi, dpi);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.PageUnit = GraphicsUnit.Pixel;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingMode = CompositingMode.SourceOver;
            using SolidBrush brush = new(foreground);
            graphics.FillPath(brush, outline);
            return bitmap;
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
    }

    /// <summary>
    ///  Converts a positive 96-DPI logical size into device pixels, rounding midpoint values up.
    /// </summary>
    internal static int GetPixelSize(int logicalSize, int dpi)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(logicalSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dpi);
        long pixels = ((long)logicalSize * dpi + 48) / 96;
        if (pixels > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(dpi), "The requested icon dimensions exceed the supported pixel range.");
        }

        return (int)Math.Max(1, pixels);
    }

    /// <summary>
    ///  Chooses a supported font by exact installed family name, never by PUA or body-font fallback.
    /// </summary>
    internal static string SelectFontFamilyName(IEnumerable<string> installedFamilyNames)
    {
        ArgumentNullException.ThrowIfNull(installedFamilyNames);
        HashSet<string> names = new(installedFamilyNames, StringComparer.OrdinalIgnoreCase);
        if (names.Contains(ToolbarGlyphCatalog.FluentFontFamilyName))
        {
            return ToolbarGlyphCatalog.FluentFontFamilyName;
        }

        if (names.Contains(ToolbarGlyphCatalog.FallbackFontFamilyName))
        {
            return ToolbarGlyphCatalog.FallbackFontFamilyName;
        }

        throw new InvalidOperationException(
            "Toolbar icons require an installed Segoe Fluent Icons or Segoe MDL2 Assets font. Keep the command text available.");
    }

    /// <summary>
    ///  Releases the selected font family without affecting any previously returned bitmap.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _fontFamily.Dispose();
    }

    private static string SelectInstalledFontFamilyName()
    {
        using InstalledFontCollection installed = new();
        FontFamily[] families = installed.Families;
        try
        {
            return SelectFontFamilyName(families.Select(family => family.Name));
        }
        finally
        {
            foreach (FontFamily family in families)
            {
                family.Dispose();
            }
        }
    }

    private void ValidateInstalledGlyphs()
    {
        ToolbarSymbol[] symbols = Enum.GetValues<ToolbarSymbol>();
        string glyphs = new(symbols.Select(symbol => ToolbarGlyphCatalog.GetGlyph(symbol, FontFamilyName).CodePoint).ToArray());
        ushort[] indices = new ushort[glyphs.Length];
        using Font font = new(_fontFamily, 32, FontStyle.Regular, GraphicsUnit.Pixel);
        using Bitmap surface = new(1, 1);
        using Graphics graphics = Graphics.FromImage(surface);
        nint hfont = font.ToHfont();
        nint hdc = 0;
        nint previousFont = 0;
        try
        {
            hdc = graphics.GetHdc();
            previousFont = SelectObject(hdc, hfont);
            if (previousFont == 0 || previousFont == -1)
            {
                throw new InvalidOperationException($"The symbol font '{FontFamilyName}' could not be selected.");
            }

            StringBuilder selectedName = new(64);
            if (GetTextFaceW(hdc, selectedName.Capacity, selectedName) == 0
                || !string.Equals(selectedName.ToString(), FontFamilyName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Windows substituted another font for '{FontFamilyName}'.");
            }

            // GGI_MARK_NONEXISTING_GLYPHS checks the selected font, without font linking.
            if (GetGlyphIndicesW(hdc, glyphs, glyphs.Length, indices, 1) == uint.MaxValue)
            {
                throw new InvalidOperationException($"The glyphs in '{FontFamilyName}' could not be verified.");
            }

            for (int index = 0; index < indices.Length; index++)
            {
                if (indices[index] is 0 or ushort.MaxValue)
                {
                    (string name, char codePoint) = ToolbarGlyphCatalog.GetGlyph(symbols[index], FontFamilyName);
                    throw new InvalidOperationException(
                        $"The installed '{FontFamilyName}' font is missing {name} (U+{(int)codePoint:X4}).");
                }
            }
        }
        finally
        {
            if (hdc != 0)
            {
                if (previousFont != 0 && previousFont != -1)
                {
                    SelectObject(hdc, previousFont);
                }

                graphics.ReleaseHdc(hdc);
            }

            DeleteObject(hfont);
        }
    }

    [DllImport("gdi32.dll", ExactSpelling = true)]
    private static extern nint SelectObject(nint hdc, nint handle);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint handle);

    [DllImport("gdi32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern int GetTextFaceW(nint hdc, int count, StringBuilder name);

    [DllImport("gdi32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern uint GetGlyphIndicesW(nint hdc, string text, int count, [Out] ushort[] indices, uint flags);
}
