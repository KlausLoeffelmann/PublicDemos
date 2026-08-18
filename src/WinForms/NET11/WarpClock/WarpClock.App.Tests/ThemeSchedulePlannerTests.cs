using WarpClock.Abstractions;

namespace WarpClock.App.Tests;

public sealed class ThemeSchedulePlannerTests
{
    private static readonly ThemeCatalogInfo[] s_catalog =
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
            ThemeKey = "modern-minimal",
            FamilyName = "Modern Minimal",
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

    [Fact]
    public void GetCurrentPeriod_UsesConfiguredBoundaries()
    {
        Assert.Equal(
            ThemeSchedulePeriod.Day,
            ThemeSchedulePlanner.GetCurrentPeriod(
                new DateTime(2026, 8, 17, 7, 0, 0),
                ThemeScheduleDocument.DefaultDayStartsAt,
                ThemeScheduleDocument.DefaultNightStartsAt));

        Assert.Equal(
            ThemeSchedulePeriod.Night,
            ThemeSchedulePlanner.GetCurrentPeriod(
                new DateTime(2026, 8, 17, 19, 0, 0),
                ThemeScheduleDocument.DefaultDayStartsAt,
                ThemeScheduleDocument.DefaultNightStartsAt));
    }

    [Fact]
    public void SelectTheme_RotatesByThirtyMinuteSlotsWithinTheCurrentPeriod()
    {
        ThemeScheduleDocument document = new()
        {
            Entries =
            [
                Entry("railway-classic", day: true, night: true),
                Entry("modern-minimal", day: true, night: true),
                Entry("sunflower", day: true, night: false),
            ],
        };

        ThemeReference? first = ThemeSchedulePlanner.SelectTheme(document, s_catalog, new DateTime(2026, 8, 17, 7, 0, 0));
        ThemeReference? second = ThemeSchedulePlanner.SelectTheme(document, s_catalog, new DateTime(2026, 8, 17, 7, 30, 0));
        ThemeReference? third = ThemeSchedulePlanner.SelectTheme(document, s_catalog, new DateTime(2026, 8, 17, 8, 0, 0));
        ThemeReference? wrapped = ThemeSchedulePlanner.SelectTheme(document, s_catalog, new DateTime(2026, 8, 17, 8, 30, 0));

        Assert.Equal("railway-classic", first?.ThemeKey);
        Assert.Equal("modern-minimal", second?.ThemeKey);
        Assert.Equal("sunflower", third?.ThemeKey);
        Assert.Equal("railway-classic", wrapped?.ThemeKey);
    }

    [Fact]
    public void SelectTheme_ReturnsNullWhenNoThemeIsEligibleForTheCurrentPeriod()
    {
        ThemeScheduleDocument document = new()
        {
            Entries =
            [
                Entry("sunflower", day: true, night: false),
            ],
        };

        ThemeReference? selected = ThemeSchedulePlanner.SelectTheme(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 22, 0, 0));

        Assert.Null(selected);

        DateTime? nextChange = ThemeSchedulePlanner.GetNextChangeTime(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 22, 0, 0));

        Assert.Equal(new DateTime(2026, 8, 18, 7, 0, 0), nextChange);
    }

    [Fact]
    public void SelectTheme_WhenAutoRotateIsDisabled_UsesTheFirstEligibleThemeUntilTheBoundary()
    {
        ThemeScheduleDocument document = new()
        {
            AutoRotate = false,
            Entries =
            [
                Entry("railway-classic", day: true, night: true),
                Entry("modern-minimal", day: true, night: true),
            ],
        };

        ThemeReference? morning = ThemeSchedulePlanner.SelectTheme(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 7, 0, 0));

        ThemeReference? later = ThemeSchedulePlanner.SelectTheme(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 12, 45, 0));

        DateTime? nextChange = ThemeSchedulePlanner.GetNextChangeTime(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 12, 45, 0));

        Assert.Equal("railway-classic", morning?.ThemeKey);
        Assert.Equal("railway-classic", later?.ThemeKey);
        Assert.Equal(new DateTime(2026, 8, 17, 19, 0, 0), nextChange);
    }

    [Fact]
    public void GetNextChangeTime_TriggersImmediatelyAtTheDayNightBoundary()
    {
        ThemeScheduleDocument document = new()
        {
            AutoRotate = false,
            Entries =
            [
                Entry("railway-classic", day: true, night: true),
            ],
        };

        DateTime? nextChange = ThemeSchedulePlanner.GetNextChangeTime(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 18, 59, 30));

        Assert.Equal(new DateTime(2026, 8, 17, 19, 0, 0), nextChange);
    }

    [Fact]
    public void SelectTheme_DoesNotUseExplicitDayVariantAtNight()
    {
        ThemeScheduleDocument document = new()
        {
            Entries =
            [
                new ThemeScheduleEntry
                {
                    Theme = new ThemeReference
                    {
                        ThemeKey = "railway-classic",
                        Variant = ClockThemeVariantKind.Day,
                    },
                    DisplayName = "Railway Classic",
                    Source = "stock",
                    Enabled = true,
                    EligibleDuringDay = true,
                    EligibleDuringNight = true,
                },
            ],
        };

        ThemeReference? selected = ThemeSchedulePlanner.SelectTheme(
            document,
            s_catalog,
            new DateTime(2026, 8, 17, 22, 0, 0));

        Assert.Null(selected);
    }

    private static ThemeScheduleEntry Entry(string key, bool day, bool night)
        => new()
        {
            Theme = new ThemeReference
            {
                ThemeKey = key,
            },
            DisplayName = key,
            Source = "test",
            Enabled = true,
            EligibleDuringDay = day,
            EligibleDuringNight = night,
        };
}
