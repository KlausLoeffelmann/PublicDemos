using WarpClock.Abstractions;

namespace WarpClock.App.Tests;

public sealed class ThemeListDefaultsTests
{
    [Fact]
    public void CreateDefault_UsesExpectedScheduleDefaults()
    {
        ThemeCatalogInfo[] catalog =
        [
            new()
            {
                ThemeKey = "railway-classic",
                FamilyName = "Railway Classic",
                Source = "stock",
                SupportedVariants = ClockThemeVariants.DayNight,
            },
            new()
            {
                ThemeKey = "sunflower",
                FamilyName = "SunFlower",
                Source = "SunFlower.dll",
                SupportedVariants = ClockThemeVariants.DayOnly,
            },
        ];

        ThemeScheduleDocument document = ThemeListDefaults.CreateDefault(catalog);

        Assert.True(document.AutoRotate);
        Assert.Equal("WarpClock Default Themelist", document.Name);
        Assert.Equal(new TimeOnly(7, 0), document.DayStartsAt);
        Assert.Equal(new TimeOnly(19, 0), document.NightStartsAt);
        Assert.Equal(30, document.RotationMinutes);
        ThemeScheduleEntry railway = Assert.Single(document.Entries);
        Assert.True(railway.Enabled);
        Assert.True(railway.EligibleDuringDay);
        Assert.True(railway.EligibleDuringNight);
    }

    [Fact]
    public void CreateDefault_PreservesCatalogOrder()
    {
        ThemeCatalogInfo[] catalog =
        [
            new() { ThemeKey = "a", FamilyName = "A", Source = "stock", SupportedVariants = ClockThemeVariants.DayNight },
            new() { ThemeKey = "b", FamilyName = "B", Source = "stock", SupportedVariants = ClockThemeVariants.DayNight },
            new() { ThemeKey = "c", FamilyName = "C", Source = "stock", SupportedVariants = ClockThemeVariants.DayNight },
        ];

        ThemeScheduleDocument document = ThemeListDefaults.CreateDefault(catalog);

        Assert.Equal(["a", "b", "c"], document.Entries.Select(entry => entry.Theme.ThemeKey));
    }
}
