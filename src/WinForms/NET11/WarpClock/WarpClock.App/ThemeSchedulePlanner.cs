using WarpClock.Abstractions;

namespace WarpClock.App;

/// <summary>
///  Resolves the current day/night slot and the scheduled family for that slot.
/// </summary>
internal static class ThemeSchedulePlanner
{
    public static ThemeSchedulePeriod GetCurrentPeriod(
        DateTime nowLocal,
        TimeOnly dayStartsAt,
        TimeOnly nightStartsAt)
    {
        TimeOnly now = TimeOnly.FromDateTime(nowLocal);
        return IsWithinPeriod(now, dayStartsAt, nightStartsAt)
            ? ThemeSchedulePeriod.Day
            : ThemeSchedulePeriod.Night;
    }

    public static ThemeReference? SelectTheme(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> availableCatalog,
        DateTime nowLocal)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(availableCatalog);

        document.Normalize();

        TimeOnly dayStartsAt = document.DayStartsAt ?? ThemeScheduleDocument.DefaultDayStartsAt;
        TimeOnly nightStartsAt = document.NightStartsAt ?? ThemeScheduleDocument.DefaultNightStartsAt;
        ThemeSchedulePeriod period = GetCurrentPeriod(nowLocal, dayStartsAt, nightStartsAt);
        List<ThemeScheduleEntry> eligible = GetEligibleEntries(document, availableCatalog, period);
        if (eligible.Count == 0)
        {
            return null;
        }

        if (!document.AutoRotate)
        {
            return ThemeReferenceUtility.Clone(eligible[0].Theme);
        }

        int rotationMinutes = Math.Max(1, document.RotationMinutes ?? ThemeScheduleDocument.DefaultRotationMinutes);
        DateTime periodStart = GetMostRecentOccurrence(
            nowLocal,
            period == ThemeSchedulePeriod.Day ? dayStartsAt : nightStartsAt);
        int slot = Math.Max(0, (int)((nowLocal - periodStart).TotalMinutes / rotationMinutes));

        ThemeScheduleEntry selectedEntry = eligible[slot % eligible.Count];
        ThemeCatalogInfo? info = availableCatalog.FirstOrDefault(item => ThemeCatalogInfo.ThemeKeysMatch(item.ThemeKey, selectedEntry.Theme.ThemeKey));
        if (info is null)
        {
            return ThemeReferenceUtility.Clone(selectedEntry.Theme);
        }

        return new ThemeReference
        {
            ThemeKey = info.ThemeKey,
            Variant = selectedEntry.Theme.Variant,
        };
    }

    public static DateTime? GetNextChangeTime(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> availableCatalog,
        DateTime nowLocal)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(availableCatalog);

        document.Normalize();

        TimeOnly dayStartsAt = document.DayStartsAt ?? ThemeScheduleDocument.DefaultDayStartsAt;
        TimeOnly nightStartsAt = document.NightStartsAt ?? ThemeScheduleDocument.DefaultNightStartsAt;
        ThemeSchedulePeriod period = GetCurrentPeriod(nowLocal, dayStartsAt, nightStartsAt);
        List<ThemeScheduleEntry> eligible = GetEligibleEntries(document, availableCatalog, period);
        DateTime nextBoundary = GetNextOccurrence(
            nowLocal,
            period == ThemeSchedulePeriod.Day ? nightStartsAt : dayStartsAt);

        if (eligible.Count == 0)
        {
            return HasAnyEligibleEntries(document, availableCatalog)
                ? nextBoundary
                : null;
        }

        if (!document.AutoRotate)
        {
            return nextBoundary;
        }

        int rotationMinutes = Math.Max(1, document.RotationMinutes ?? ThemeScheduleDocument.DefaultRotationMinutes);
        DateTime periodStart = GetMostRecentOccurrence(
            nowLocal,
            period == ThemeSchedulePeriod.Day ? dayStartsAt : nightStartsAt);
        int completedSlots = Math.Max(0, (int)((nowLocal - periodStart).TotalMinutes / rotationMinutes));
        DateTime nextSlot = periodStart.AddMinutes((completedSlots + 1) * rotationMinutes);

        return nextSlot < nextBoundary ? nextSlot : nextBoundary;
    }

    private static List<ThemeScheduleEntry> GetEligibleEntries(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> availableCatalog,
        ThemeSchedulePeriod period)
    {
        Dictionary<string, ThemeCatalogInfo> available = availableCatalog
            .GroupBy(item => ThemeCatalogInfo.NormalizeThemeKey(item.ThemeKey), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);

        return document.Entries
            .Where(entry =>
                entry.Enabled
                && !string.IsNullOrWhiteSpace(entry.Theme.ThemeKey)
                && available.TryGetValue(ThemeCatalogInfo.NormalizeThemeKey(entry.Theme.ThemeKey), out ThemeCatalogInfo? info)
                && info.SupportsPeriod(period, entry.Theme.Variant)
                && (period == ThemeSchedulePeriod.Day ? entry.EligibleDuringDay : entry.EligibleDuringNight))
            .ToList();
    }

    private static bool HasEligibleEntries(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> availableCatalog,
        ThemeSchedulePeriod period)
        => GetEligibleEntries(document, availableCatalog, period).Count > 0;

    private static bool HasAnyEligibleEntries(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> availableCatalog)
        => HasEligibleEntries(document, availableCatalog, ThemeSchedulePeriod.Day)
            || HasEligibleEntries(document, availableCatalog, ThemeSchedulePeriod.Night);

    private static bool IsWithinPeriod(TimeOnly now, TimeOnly start, TimeOnly end)
    {
        if (start == end)
        {
            return true;
        }

        return start < end
            ? now >= start && now < end
            : now >= start || now < end;
    }

    private static DateTime GetMostRecentOccurrence(DateTime nowLocal, TimeOnly time)
    {
        DateTime candidate = nowLocal.Date + time.ToTimeSpan();
        return candidate <= nowLocal ? candidate : candidate.AddDays(-1);
    }

    private static DateTime GetNextOccurrence(DateTime nowLocal, TimeOnly time)
    {
        DateTime candidate = nowLocal.Date + time.ToTimeSpan();
        return candidate > nowLocal ? candidate : candidate.AddDays(1);
    }
}

public enum ThemeSchedulePeriod
{
    Day,
    Night,
}
