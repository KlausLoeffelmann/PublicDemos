using System.ComponentModel;
using System.Drawing.Text;

namespace CameraControlDemo;

/// <summary>
///  Windows symbol-font glyphs used by the camera toolbar.
/// </summary>
internal enum CameraSymbol
{
    Camera = 0xE722,
    Resolution = 0xE91B,
    FitFrame = 0xE9A6,
    Refresh = 0xE72C,
    FullScreen = 0xE740,
    BackToWindow = 0xE73F
}

/// <summary>
///  Renders and owns toolbar images at the current monitor DPI and theme color.
/// </summary>
internal sealed class ToolbarImageFactory : Component
{
    private readonly Dictionary<ToolStripItem, Bitmap> _images = [];
    private readonly string _fontName = ResolveFontName();

    /// <summary>
    ///  Initializes an image factory owned by the form's component container.
    /// </summary>
    /// <param name="container">The container that disposes the generated images.</param>
    public ToolbarImageFactory(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.Add(this);
    }

    /// <summary>
    ///  Replaces an item's image with a freshly rendered symbol.
    /// </summary>
    /// <param name="item">The toolbar item receiving the image.</param>
    /// <param name="symbol">The Windows symbol to render.</param>
    /// <param name="dpi">The current monitor DPI.</param>
    public void SetImage(ToolStripItem item, CameraSymbol symbol, int dpi)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dpi);

        int pixelSize = (int)Math.Round(20 * dpi / 96D);
        Bitmap image = new(pixelSize, pixelSize);

        try
        {
            image.SetResolution(dpi, dpi);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Transparent);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            using Font font = new(_fontName, pixelSize * 0.8F, FontStyle.Regular, GraphicsUnit.Pixel);
            using SolidBrush brush = new(item.ForeColor);
            using StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.DrawString(
                char.ConvertFromUtf32((int)symbol),
                font,
                brush,
                new RectangleF(0, 0, pixelSize, pixelSize),
                format);
        }
        catch
        {
            image.Dispose();
            throw;
        }

        _images.Remove(item, out Bitmap? previous);
        _images.Add(item, image);
        item.Image = image;
        previous?.Dispose();
    }

    private static string ResolveFontName()
    {
        // These glyphs share code points in the Windows 11 and Windows 10 icon fonts.
        foreach (string name in new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" })
        {
            try
            {
                using FontFamily family = new(name);
                if (string.Equals(family.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return name;
                }
            }
            catch (ArgumentException)
            {
                // Try the other Windows symbol font rather than rendering missing glyphs.
            }
        }

        throw new InvalidOperationException("A Windows symbol font is required for the camera toolbar.");
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach ((ToolStripItem item, Bitmap image) in _images)
            {
                if (!item.IsDisposed && ReferenceEquals(item.Image, image))
                {
                    item.Image = null;
                }

                image.Dispose();
            }

            _images.Clear();
        }

        base.Dispose(disposing);
    }
}
