namespace WarpClock.App;

internal sealed class TimeZoneEditorRow
{
    public string TimeZoneId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}

internal sealed class TickerSourceEditorItem
{
    public required TickerContentSource Source { get; init; }

    public required string DisplayName { get; init; }

    public bool Enabled { get; set; }
}

internal static class OptionsDialogModelMapper
{
    private static readonly TickerContentSource[] s_allTickerSources =
    [
        TickerContentSource.CustomMessage,
        TickerContentSource.CurrentDate,
        TickerContentSource.TimeZone,
        TickerContentSource.ThemeName,
    ];

    public static IReadOnlyList<TimeZoneEditorRow> CreateTimeZoneRows(TimeZoneOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        TimeZoneOptions clone = options.Clone();
        clone.Normalize();

        List<TimeZoneEditorRow> rows =
        [
            .. clone.Entries.Select(entry => new TimeZoneEditorRow
            {
                TimeZoneId = entry.TimeZoneId,
                DisplayName = entry.DisplayName,
                IsDefault = entry.IsDefault,
            }),
        ];

        while (rows.Count < TimeZoneOptions.MaximumTimeZoneCount)
        {
            rows.Add(new TimeZoneEditorRow());
        }

        return rows;
    }

    public static bool TryCreateTimeZoneOptions(
        bool enabled,
        int changeToNextSeconds,
        int returnToDefaultSeconds,
        bool showOnClockFace,
        bool showOnlyWhenAlternate,
        bool showHeadlineFallback,
        IEnumerable<TimeZoneEditorRow> rows,
        out TimeZoneOptions options,
        out string? validationMessage)
    {
        ArgumentNullException.ThrowIfNull(rows);

        options = new TimeZoneOptions
        {
            Enabled = enabled,
            ChangeToNextSeconds = changeToNextSeconds,
            ReturnToDefaultSeconds = returnToDefaultSeconds,
            ShowOnClockFace = showOnClockFace,
            ShowOnlyWhenAlternate = showOnlyWhenAlternate,
            ShowHeadlineFallback = showHeadlineFallback,
            Entries = [],
        };

        var configuredRows = new List<ConfiguredTimeZone>(TimeZoneOptions.MaximumTimeZoneCount);
        var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (TimeZoneEditorRow row in rows.Take(TimeZoneOptions.MaximumTimeZoneCount))
        {
            string timeZoneId = row.TimeZoneId?.Trim() ?? string.Empty;
            string displayName = row.DisplayName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(timeZoneId))
            {
                if (!string.IsNullOrWhiteSpace(displayName) || row.IsDefault)
                {
                    validationMessage = "Select a time zone for every populated row.";
                    return false;
                }

                continue;
            }

            if (!seenIds.Add(timeZoneId))
            {
                validationMessage = $"The time zone '{timeZoneId}' is selected more than once.";
                return false;
            }

            configuredRows.Add(new ConfiguredTimeZone
            {
                TimeZoneId = timeZoneId,
                DisplayName = displayName,
                IsDefault = row.IsDefault,
            });
        }

        if (configuredRows.Count == 0)
        {
            configuredRows.Add(new ConfiguredTimeZone
            {
                TimeZoneId = TimeZoneInfo.Local.Id,
                DisplayName = "Local",
                IsDefault = true,
            });
        }
        else if (!configuredRows.Any(entry => entry.IsDefault))
        {
            configuredRows[0].IsDefault = true;
        }

        if (enabled && configuredRows.Count < 2)
        {
            validationMessage = "Enable at least two different time zones before turning rotation on.";
            return false;
        }

        options.Entries = configuredRows;
        options.Normalize();
        validationMessage = null;
        return true;
    }

    public static IReadOnlyList<TickerSourceEditorItem> CreateTickerItems(DisplayOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        DisplayOptions clone = options.Clone();
        clone.Normalize();

        var enabled = new HashSet<TickerContentSource>(clone.TickerContentOrder);
        List<TickerSourceEditorItem> items =
        [
            .. clone.TickerContentOrder.Select(source => new TickerSourceEditorItem
            {
                Source = source,
                DisplayName = GetTickerSourceDisplayName(source),
                Enabled = true,
            }),
        ];

        foreach (TickerContentSource source in s_allTickerSources)
        {
            if (enabled.Contains(source))
            {
                continue;
            }

            items.Add(new TickerSourceEditorItem
            {
                Source = source,
                DisplayName = GetTickerSourceDisplayName(source),
                Enabled = false,
            });
        }

        return items;
    }

    public static DisplayOptions CreateDisplayOptions(
        bool tickerEnabled,
        string? customTickerMessage,
        IEnumerable<TickerSourceEditorItem> items,
        bool showThemeTickerVisual,
        bool showFractionSecondVisual)
    {
        ArgumentNullException.ThrowIfNull(items);

        var options = new DisplayOptions
        {
            TickerEnabled = tickerEnabled,
            CustomTickerMessage = customTickerMessage ?? string.Empty,
            TickerContentOrder =
            [
                .. items
                    .Where(item => item.Enabled)
                    .Select(item => item.Source),
            ],
            ShowThemeTickerVisual = showThemeTickerVisual,
            ShowFractionSecondVisual = showFractionSecondVisual,
        };

        options.Normalize();
        return options;
    }

    public static string GetTickerSourceDisplayName(TickerContentSource source)
        => source switch
        {
            TickerContentSource.CustomMessage => "Custom message",
            TickerContentSource.CurrentDate => "Current date",
            TickerContentSource.TimeZone => "Time zone",
            TickerContentSource.ThemeName => "Theme name",
            _ => source.ToString(),
        };
}
