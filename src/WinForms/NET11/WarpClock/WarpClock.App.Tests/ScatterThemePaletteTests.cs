using System.Drawing;
using System.Reflection;
using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.App.Tests;

public sealed class ScatterThemePaletteTests
{
    [Fact]
    public void NightPalette_UsesDarkNonYellowAccentColors()
    {
        IClockTheme theme = new ScatterTheme().ResolveVariant(ClockThemeVariantKind.Night);
        object palette = typeof(ScatterTheme)
            .GetField("_palette", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(theme)!;

        Assert.Equal(Color.FromArgb(110, 126, 148), GetColor(palette, "MagnetRim"));
        Assert.Equal(Color.FromArgb(176, 106, 126), GetColor(palette, "Second"));
        Assert.Equal(Color.FromArgb(132, 141, 156), GetColor(palette, "Arbour"));
    }

    [Fact]
    public void OledPalettes_UseDarkBlueAndBlackFaces()
    {
        object oledDayPalette = typeof(ScatterTheme)
            .GetProperty("Palette", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(new ScatterTheme().ResolveVariant(ClockThemeVariantKind.OledDay))!;
        object oledNightPalette = typeof(ScatterTheme)
            .GetProperty("Palette", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(new ScatterTheme().ResolveVariant(ClockThemeVariantKind.OledNight))!;

        Assert.Equal(Color.FromArgb(12, 32, 86), GetColor(oledDayPalette, "Face"));
        Assert.Equal(Color.Black, GetColor(oledNightPalette, "Face"));
    }

    private static Color GetColor(object palette, string propertyName)
        => (Color)(palette.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(palette) ?? throw new InvalidOperationException($"Palette property '{propertyName}' was null."));
}
