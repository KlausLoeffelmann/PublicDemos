namespace WarpClock.App;

/// <summary>
///  Identifies a built-in source that can contribute text to the global ticker band.
/// </summary>
public enum TickerContentSource
{
    CustomMessage,
    CurrentDate,
    TimeZone,
    ThemeName,
}

/// <summary>
///  Composes the ordered global ticker text from enabled content sources.
/// </summary>
internal static class TickerContentComposer
{
    public static string Compose(
        IEnumerable<TickerContentSource> orderedSources,
        string? customMessage,
        DateTime displayedTime,
        string? timeZoneDisplayName,
        string? themeName)
    {
        ArgumentNullException.ThrowIfNull(orderedSources);

        var segments = new List<string>();
        var seen = new HashSet<TickerContentSource>();

        foreach (TickerContentSource source in orderedSources)
        {
            if (!seen.Add(source))
            {
                continue;
            }

            string? value = source switch
            {
                TickerContentSource.CustomMessage => Normalize(customMessage),
                TickerContentSource.CurrentDate => displayedTime.ToString("D"),
                TickerContentSource.TimeZone => Normalize(timeZoneDisplayName),
                TickerContentSource.ThemeName => Normalize(themeName),
                _ => null,
            };

            if (!string.IsNullOrWhiteSpace(value))
            {
                segments.Add(value);
            }
        }

        return string.Join("   |   ", segments);
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
