using System.Drawing;
using System.Drawing.Text;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using DrumMachine.Demo.Controls;

namespace SplitFlap.Tests;

/// <summary>
///  Verifies font selection, glyph rendering, DPI sizing, and image ownership without an audio device.
/// </summary>
[Collection("Rhythm UI")]
public sealed class SymbolIconFactoryTests
{
    /// <summary>
    ///  Checks all three toolbar sizes and the independent menu size at common monitor resolutions.
    /// </summary>
    [Theory]
    [InlineData(16, 96, 16)]
    [InlineData(16, 144, 24)]
    [InlineData(16, 192, 32)]
    [InlineData(32, 96, 32)]
    [InlineData(32, 144, 48)]
    [InlineData(32, 192, 64)]
    [InlineData(48, 96, 48)]
    [InlineData(48, 144, 72)]
    [InlineData(48, 192, 96)]
    [InlineData(64, 96, 64)]
    [InlineData(64, 144, 96)]
    [InlineData(64, 192, 128)]
    public void Render_AllSymbolsHaveTransparentMarginsAndCenteredInk(int logicalSize, int dpi, int expectedPixels)
    {
        using SymbolIconFactory factory = new();
        bool hasAntialiasedPixel = false;
        foreach (ToolbarSymbol symbol in Enum.GetValues<ToolbarSymbol>())
        {
            using Bitmap image = factory.Create(symbol, logicalSize, dpi, Color.Black);
            Assert.Equal(expectedPixels, image.Width);
            Assert.Equal(expectedPixels, image.Height);
            Assert.Equal((float)dpi, image.HorizontalResolution);
            Assert.Equal((float)dpi, image.VerticalResolution);

            Rectangle ink = FindInk(image);
            Assert.False(ink.IsEmpty);
            int minimumMargin = Math.Max(1, expectedPixels / 8 - 1);
            Assert.True(ink.Left >= minimumMargin && ink.Top >= minimumMargin);
            Assert.True(expectedPixels - ink.Right >= minimumMargin && expectedPixels - ink.Bottom >= minimumMargin);
            Assert.InRange(Math.Abs(ink.Left - (expectedPixels - ink.Right)), 0, 2);
            Assert.InRange(Math.Abs(ink.Top - (expectedPixels - ink.Bottom)), 0, 2);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.A > 0)
                    {
                        Assert.Equal(0, pixel.R);
                        Assert.Equal(0, pixel.G);
                        Assert.Equal(0, pixel.B);
                        hasAntialiasedPixel |= pixel.A < 255;
                    }
                }
            }
        }

        Assert.True(hasAntialiasedPixel, "Glyph edges should use grayscale coverage, not opaque or ClearType pixels.");
    }

    /// <summary>
    ///  Checks that foreground changes do not introduce white or black edge mattes.
    /// </summary>
    [Fact]
    public void Render_UsesTheSuppliedThemeColorAndAlpha()
    {
        using SymbolIconFactory factory = new();
        Color foreground = Color.FromArgb(180, 40, 160, 220);
        using Bitmap image = factory.Create(ToolbarSymbol.Play, 64, 144, foreground);
        bool foundInterior = false;
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                Assert.InRange(pixel.A, 0, foreground.A);
                if (pixel.A >= 100)
                {
                    Assert.InRange(Math.Abs(pixel.R - foreground.R), 0, 3);
                    Assert.InRange(Math.Abs(pixel.G - foreground.G), 0, 3);
                    Assert.InRange(Math.Abs(pixel.B - foreground.B), 0, 3);
                    foundInterior = true;
                }
            }
        }

        Assert.True(foundInterior);
    }

    /// <summary>
    ///  Ensures selection prefers Fluent even when the installed list is in another order.
    /// </summary>
    [Fact]
    public void FontSelection_PrefersFluentAndUsesOnlyTheExplicitMdl2Fallback()
    {
        Assert.Equal(ToolbarGlyphCatalog.FluentFontFamilyName,
            SymbolIconFactory.SelectFontFamilyName(["Arial", "Segoe MDL2 Assets", "segoe fluent icons"]));
        Assert.Equal(ToolbarGlyphCatalog.FallbackFontFamilyName,
            SymbolIconFactory.SelectFontFamilyName(["Segoe UI", "Segoe MDL2 Assets"]));

        using SymbolIconFactory factory = new();
        Assert.Equal(SymbolIconFactory.SelectFontFamilyName(InstalledFontNames()), factory.FontFamilyName);
        Assert.Equal(factory.FontFamilyName == ToolbarGlyphCatalog.FallbackFontFamilyName, factory.UsesFallback);
    }

    /// <summary>
    ///  Rejects body fonts, fuzzy family names, and an unavailable symbol-font selection.
    /// </summary>
    [Fact]
    public void FontSelection_NeverSilentlyAcceptsABodyFont()
    {
        InvalidOperationException missing = Assert.Throws<InvalidOperationException>(
            () => SymbolIconFactory.SelectFontFamilyName(["Segoe UI", "Segoe UI Symbol", "Segoe Fluent Icons Extra"]));
        Assert.Contains(ToolbarGlyphCatalog.FluentFontFamilyName, missing.Message);
        Assert.Contains(ToolbarGlyphCatalog.FallbackFontFamilyName, missing.Message);
        Assert.Throws<ArgumentException>(() => new SymbolIconFactory("Segoe UI"));
        Assert.Throws<ArgumentException>(() => ToolbarGlyphCatalog.GetGlyph(ToolbarSymbol.Play, "Arial"));
        Assert.Throws<ArgumentNullException>(() => SymbolIconFactory.SelectFontFamilyName(null!));
    }

    /// <summary>
    ///  Verifies the actual installed preferred and fallback fonts contain distinct command glyphs.
    /// </summary>
    [Theory]
    [InlineData("Segoe Fluent Icons", false)]
    [InlineData("Segoe MDL2 Assets", true)]
    public void InstalledFonts_RenderDistinctGlyphsWithoutMissingCharacterBoxes(string familyName, bool fallback)
    {
        Assert.SkipUnless(InstalledFontNames().Contains(familyName, StringComparer.OrdinalIgnoreCase),
            $"The optional '{familyName}' font is not installed.");
        using SymbolIconFactory factory = new(familyName);
        Assert.Equal(familyName, factory.FontFamilyName);
        Assert.Equal(fallback, factory.UsesFallback);

        HashSet<string> outlines = [];
        foreach (ToolbarSymbol symbol in Enum.GetValues<ToolbarSymbol>())
        {
            using Bitmap image = factory.Create(symbol, 32, 96, Color.Black);
            Assert.False(FindInk(image).IsEmpty);
            byte[] alpha = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    alpha[y * image.Width + x] = image.GetPixel(x, y).A;
                }
            }

            Assert.True(outlines.Add(Convert.ToBase64String(alpha)), $"{familyName}: {symbol} repeated another glyph.");
        }
    }

    /// <summary>
    ///  Locks down the code points verified in the two official Microsoft glyph catalogs.
    /// </summary>
    [Fact]
    public void Catalog_UsesNamedDocumentedMappingsForBothSupportedFonts()
    {
        Dictionary<ToolbarSymbol, char> expected = new()
        {
            [ToolbarSymbol.New] = '\uE7C3',
            [ToolbarSymbol.Open] = '\uE838',
            [ToolbarSymbol.Save] = '\uE74E',
            [ToolbarSymbol.Play] = '\uE768',
            [ToolbarSymbol.Pause] = '\uE769',
            [ToolbarSymbol.Stop] = '\uE71A',
            [ToolbarSymbol.Loop] = '\uE8EE',
            [ToolbarSymbol.Metallic] = '\uE81E',
            [ToolbarSymbol.Audition] = '\uE767',
            [ToolbarSymbol.Options] = '\uE713',
            [ToolbarSymbol.Undo] = '\uE7A7',
            [ToolbarSymbol.Redo] = '\uE7A6',
            [ToolbarSymbol.Quit] = '\uE711'
        };

        foreach (string family in new[] { ToolbarGlyphCatalog.FluentFontFamilyName, ToolbarGlyphCatalog.FallbackFontFamilyName })
        {
            foreach ((ToolbarSymbol symbol, char codePoint) in expected)
            {
                var glyph = ToolbarGlyphCatalog.GetGlyph(symbol, family);
                Assert.Equal(codePoint, glyph.CodePoint);
                Assert.False(string.IsNullOrWhiteSpace(glyph.Name));
            }
        }
    }

    /// <summary>
    ///  Rejects invalid dimensions before allocating a bitmap.
    /// </summary>
    [Theory]
    [InlineData(0, 96)]
    [InlineData(-1, 96)]
    [InlineData(32, 0)]
    [InlineData(32, -1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void PixelSize_RejectsInvalidOrOverflowingDimensions(int logicalSize, int dpi)
        => Assert.Throws<ArgumentOutOfRangeException>(() => SymbolIconFactory.GetPixelSize(logicalSize, dpi));

    /// <summary>
    ///  Applies deterministic rounding at non-integral DPI scales.
    /// </summary>
    [Fact]
    public void PixelSize_RoundsMidpointsUp()
    {
        Assert.Equal(17, SymbolIconFactory.GetPixelSize(16, 99));
        Assert.Equal(34, SymbolIconFactory.GetPixelSize(32, 101));
    }

    /// <summary>
    ///  Rejects unknown icons and preserves caller-owned images after the factory is disposed.
    /// </summary>
    [Fact]
    public void Factory_DisposalAndInvalidSymbolsAreExplicit()
    {
        using SymbolIconFactory factory = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.Create((ToolbarSymbol)999, 32, 96, Color.Black));
        using Bitmap image = factory.Create(ToolbarSymbol.Save, 32, 96, Color.Black);
        factory.Dispose();
        factory.Dispose();
        Assert.False(FindInk(image).IsEmpty);
        Assert.Throws<ObjectDisposedException>(() => factory.Create(ToolbarSymbol.Save, 32, 96, Color.Black));
    }

    /// <summary>
    ///  Avoids rerendering identical icon keys and preserves the item's text and accessibility metadata.
    /// </summary>
    [Fact]
    public void IconSet_IdenticalKeysReuseTheImageAndDoNotChangeLabels()
        => OnStaThread(() =>
        {
            using SymbolIconFactory factory = new();
            using ToolbarIconSet icons = new(factory);
            using ToolStripButton item = new("Play") { AccessibleName = "Play the loop", ToolTipText = "Play or resume" };
            icons.Apply(item, ToolbarSymbol.Play, 32, 144, SystemColors.ControlText);
            Image first = Assert.IsType<Bitmap>(item.Image);
            icons.Apply(item, ToolbarSymbol.Play, 32, 144, Color.FromArgb(SystemColors.ControlText.ToArgb()));

            Assert.Same(first, item.Image);
            Assert.Equal(ToolStripItemImageScaling.None, item.ImageScaling);
            Assert.Equal("Play", item.Text);
            Assert.Equal("Play the loop", item.AccessibleName);
            Assert.Equal("Play or resume", item.ToolTipText);
        });

    /// <summary>
    ///  Replaces the active image for every part of the render key and disposes each retired bitmap.
    /// </summary>
    [Fact]
    public void IconSet_ReplacesAndDisposesOnSymbolSizeDpiAndColorChanges()
        => OnStaThread(() =>
        {
            using SymbolIconFactory factory = new();
            using ToolbarIconSet icons = new(factory);
            using ToolStripButton item = new();
            icons.Apply(item, ToolbarSymbol.Play, 32, 96, Color.Black);
            foreach (var key in new[]
            {
                (ToolbarSymbol.Pause, 32, 96, Color.Black),
                (ToolbarSymbol.Pause, 48, 96, Color.Black),
                (ToolbarSymbol.Pause, 48, 144, Color.Black),
                (ToolbarSymbol.Pause, 48, 144, Color.White)
            })
            {
                Image previous = Assert.IsType<Bitmap>(item.Image);
                icons.Apply(item, key.Item1, key.Item2, key.Item3, key.Item4);
                Assert.NotSame(previous, item.Image);
                AssertDisposed(previous);
            }

            Image current = Assert.IsType<Bitmap>(item.Image);
            Assert.Throws<ArgumentOutOfRangeException>(() => icons.Apply(item, (ToolbarSymbol)999, 32, 96, Color.Black));
            Assert.Same(current, item.Image);
            Assert.Equal(72, current.Width);
        });

    /// <summary>
    ///  Keeps menu dimensions independent and gives each item its own safe bitmap lifetime.
    /// </summary>
    [Fact]
    public void IconSet_ItemsAndMenuSizesAreIndependent()
        => OnStaThread(() =>
        {
            using SymbolIconFactory factory = new();
            using ToolbarIconSet icons = new(factory);
            using ToolStripButton first = new();
            using ToolStripButton second = new();
            using ToolStripMenuItem menu = new();
            icons.Apply(first, ToolbarSymbol.Save, 64, 192, Color.Black);
            icons.Apply(second, ToolbarSymbol.Save, 64, 192, Color.Black);
            icons.Apply(menu, ToolbarSymbol.Save, 16, 192, Color.Black);
            Image firstImage = Assert.IsType<Bitmap>(first.Image);
            Assert.NotSame(firstImage, second.Image);
            Assert.Equal(128, second.Image!.Width);
            Assert.Equal(32, menu.Image!.Width);

            first.Dispose();
            AssertDisposed(firstImage);
            Assert.Equal(128, second.Image.Width);
            Assert.Equal(32, menu.Image.Width);
        });

    /// <summary>
    ///  Detaches its own images without disposing the caller's factory, items, or unrelated images.
    /// </summary>
    [Fact]
    public void IconSet_DisposalRespectsOwnershipBoundaries()
        => OnStaThread(() =>
        {
            using SymbolIconFactory factory = new();
            using ToolbarIconSet icons = new(factory);
            using Bitmap callerImage = new(8, 8);
            using ToolStripButton first = new() { Image = callerImage };
            using ToolStripButton second = new();
            icons.Apply(first, ToolbarSymbol.Play, 32, 96, Color.Black);
            icons.Apply(second, ToolbarSymbol.Stop, 32, 96, Color.Black);
            Image firstOwned = Assert.IsType<Bitmap>(first.Image);
            Image secondOwned = Assert.IsType<Bitmap>(second.Image);
            second.Image = callerImage;
            icons.Dispose();
            icons.Dispose();

            Assert.Null(first.Image);
            Assert.Same(callerImage, second.Image);
            AssertDisposed(firstOwned);
            AssertDisposed(secondOwned);
            Assert.Equal(8, callerImage.Width);
            Assert.False(first.IsDisposed);
            Assert.False(second.IsDisposed);
            using Bitmap independent = factory.Create(ToolbarSymbol.New, 16, 96, Color.Black);
            Assert.Equal(16, independent.Width);
            Assert.Throws<ObjectDisposedException>(() => icons.Apply(first, ToolbarSymbol.New, 32, 96, Color.Black));
        });

    private static Rectangle FindInk(Bitmap image)
    {
        int left = image.Width;
        int top = image.Height;
        int right = -1;
        int bottom = -1;
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if (image.GetPixel(x, y).A > 0)
                {
                    left = Math.Min(left, x);
                    top = Math.Min(top, y);
                    right = Math.Max(right, x);
                    bottom = Math.Max(bottom, y);
                }
            }
        }

        return right < left ? Rectangle.Empty : Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
    }

    private static string[] InstalledFontNames()
    {
        using InstalledFontCollection installed = new();
        FontFamily[] families = installed.Families;
        try
        {
            return families.Select(family => family.Name).ToArray();
        }
        finally
        {
            foreach (FontFamily family in families)
            {
                family.Dispose();
            }
        }
    }

    private static void AssertDisposed(Image image)
        => Assert.ThrowsAny<ArgumentException>(() => image.Width);

    private static void OnStaThread(Action action)
    {
        ExceptionDispatchInfo? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "The icon ownership operation did not finish.");
        failure?.Throw();
    }
}
