// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace VisualStylesModeDemo.Components;

internal enum SymbolGlyph
{
    Paste = 0xE77F,
    Edit = 0xE70F,
    Save = 0xE74E,
    Cut = 0xE8C6,
    Copy = 0xE8C8,
    Italic = 0xE8DB,
    Underline = 0xE8DC,
    Bold = 0xE8DD,
    SelectAll = 0xE8B3,
    OpenFile = 0xE8E5,
    ClearSelection = 0xE8E6,
}

/// <summary>
///  Creates and owns toolbar images rendered from the Windows symbol fonts.
/// </summary>
internal sealed class IconFactoryComponent : Component
{
    private const string FluentFontName = "Segoe Fluent Icons";
    private const string Mdl2FontName = "Segoe MDL2 Assets";
    private const string SymbolFontName = "Segoe UI Symbol";

    private readonly List<Image> _ownedImages = [];
    private readonly string _fontName = ResolveFontName();

    public IconFactoryComponent()
    {
    }

    public IconFactoryComponent(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.Add(this);
    }

    /// <summary>
    ///  Replaces a ToolStrip item's image with a newly rendered symbol owned by this component.
    /// </summary>
    public void SetImage(ToolStripItem item, SymbolGlyph glyph, int logicalSize, int dpi, Color color)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(logicalSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dpi);

        if (item.Image is Image previous && _ownedImages.Remove(previous))
        {
            previous.Dispose();
        }

        Image image = CreateImage(glyph, logicalSize, dpi, color);
        _ownedImages.Add(image);
        item.Image = image;
    }

    private Image CreateImage(SymbolGlyph glyph, int logicalSize, int dpi, Color color)
    {
        int pixelSize = (int)Math.Round(logicalSize * dpi / 96D);
        Bitmap bitmap = new(pixelSize, pixelSize);
        bitmap.SetResolution(dpi, dpi);

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        using Font font = new(_fontName, pixelSize * 0.72F, FontStyle.Regular, GraphicsUnit.Pixel);
        using SolidBrush brush = new(color);
        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        string text = char.ConvertFromUtf32((int)glyph);
        graphics.DrawString(text, font, brush, new RectangleF(0, 0, pixelSize, pixelSize), format);
        return bitmap;
    }

    private static string ResolveFontName()
    {
        foreach (string candidate in new[] { FluentFontName, Mdl2FontName, SymbolFontName })
        {
            if (IsFontInstalled(candidate))
            {
                return candidate;
            }
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (Image image in _ownedImages)
            {
                image.Dispose();
            }

            _ownedImages.Clear();
        }

        base.Dispose(disposing);
    }
}
