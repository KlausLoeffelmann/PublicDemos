using WarpClock.Abstractions;

namespace WarpClock.App.Tests;

public sealed class ThemeCatalogInfoTests
{
    [Theory]
    [InlineData("Railway Classic", "railway-classic")]
    [InlineData("Railway Classic - Night", "railway-classic")]
    [InlineData("stock|Railway Classic|WarpClock.Themes.Builtin.StandardClockTheme", "railway-classic")]
    [InlineData("Nerd.dll|Nerd|WarpClock.Themes.Nerd.NerdTheme", "nerd")]
    [InlineData("Scatter.dll|Scatter|WarpClock.Themes.Scatter.ScatterTheme", "scatter")]
    public void NormalizeThemeKey_MigratesLegacyAndVariantQualifiedKeys(string rawKey, string expectedStableKey)
    {
        Assert.Equal(expectedStableKey, ThemeCatalogInfo.NormalizeThemeKey(rawKey));
    }

    [Fact]
    public void ResolveVariant_PrefersOledVariantsWhenEnabled()
    {
        ThemeCatalogInfo info = new()
        {
            ThemeKey = "logical",
            FamilyName = "Logical",
            Source = "stock",
            SupportedVariants = ClockThemeVariants.DayNightOled,
        };

        Assert.Equal(
            ClockThemeVariantKind.OledDay,
            info.ResolveVariant(requestedVariant: null, ThemeSchedulePeriod.Day, preferOledVariants: true));

        Assert.Equal(
            ClockThemeVariantKind.OledNight,
            info.ResolveVariant(requestedVariant: null, ThemeSchedulePeriod.Night, preferOledVariants: true));
    }

    [Fact]
    public void SupportsPeriod_RejectsPinnedDayVariantAtNight()
    {
        ThemeCatalogInfo info = new()
        {
            ThemeKey = "railway-classic",
            FamilyName = "Railway Classic",
            Source = "stock",
            SupportedVariants = ClockThemeVariants.DayNight,
        };

        Assert.False(info.SupportsPeriod(ThemeSchedulePeriod.Night, ClockThemeVariantKind.Day));
        Assert.True(info.SupportsPeriod(ThemeSchedulePeriod.Night, ClockThemeVariantKind.Night));
    }
}
