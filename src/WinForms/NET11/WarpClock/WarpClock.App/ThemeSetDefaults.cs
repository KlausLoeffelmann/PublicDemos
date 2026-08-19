using WarpClock.Abstractions;

namespace WarpClock.App;

/// <summary>
///  Provides the first-run theme-set document and per-theme default eligibility.
/// </summary>
internal static class ThemeSetDefaults
{
    public static ThemeScheduleDocument CreateDefault(IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        List<ThemeCatalogInfo> defaults = catalog
            .Where(IsDefaultEntry)
            .ToList();

        if (defaults.Count == 0)
        {
            defaults = catalog
                .Where(info => info.SupportsPeriod(ThemeSchedulePeriod.Day))
                .ToList();
        }

        var document = new ThemeScheduleDocument
        {
            Name = "WarpClock Default Themeset",
            Entries = defaults.Select(CreateDefaultEntry).ToList(),
        };

        document.Normalize();
        return document;
    }

    public static ThemeScheduleEntry CreateDefaultEntry(ThemeCatalogInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        (bool day, bool night) = GetEligibility(info);

        return new ThemeScheduleEntry
        {
            Theme = new ThemeReference
            {
                ThemeKey = info.ThemeKey,
            },
            DisplayName = info.FamilyName,
            Source = info.Source,
            Enabled = true,
            EligibleDuringDay = day,
            EligibleDuringNight = night,
        };
    }

    private static (bool Day, bool Night) GetEligibility(ThemeCatalogInfo info)
        => (info.SupportsPeriod(ThemeSchedulePeriod.Day), info.SupportsPeriod(ThemeSchedulePeriod.Night));

    private static bool IsDefaultEntry(ThemeCatalogInfo info)
        => string.Equals(info.Source, "stock", StringComparison.OrdinalIgnoreCase)
            && info.SupportsPeriod(ThemeSchedulePeriod.Day)
            && info.SupportsPeriod(ThemeSchedulePeriod.Night);
}
