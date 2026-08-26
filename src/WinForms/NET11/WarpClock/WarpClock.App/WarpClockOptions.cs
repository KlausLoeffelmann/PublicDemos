using WarpClock.Abstractions;
using WarpClock.Engine;

namespace WarpClock.App;

/// <summary>
///  Editable application options shown by Tools ▸ Options.
/// </summary>
public sealed class WarpClockOptions
{
    public HandOptions Hands { get; set; } = new();

    public TimeZoneOptions TimeZones { get; set; } = new();

    public DisplayOptions Display { get; set; } = new();

    public FolderOptions Folders { get; set; } = new();

    public void Normalize()
    {
        Hands ??= new HandOptions();
        TimeZones ??= new TimeZoneOptions();
        Display ??= new DisplayOptions();
        Folders ??= new FolderOptions();

        Hands.Normalize();
        TimeZones.Normalize();
        Display.Normalize();
        Folders.Normalize();
    }

    public WarpClockOptions Clone()
        => new()
        {
            Hands = Hands.Clone(),
            TimeZones = TimeZones.Clone(),
            Display = Display.Clone(),
            Folders = Folders.Clone(),
        };
}

/// <summary>Persisted hand movement choices.</summary>
public sealed class HandOptions
{
    public ClockHandMotion HourMotion { get; set; } = ClockHandMotion.Crawling;

    public ClockHandMotion MinuteMotion { get; set; } = ClockHandMotion.Crawling;

    public ClockHandMotion SecondMotion { get; set; } = ClockHandMotion.Crawling;

    public int GraceSeconds { get; set; } = 5;

    public void Normalize()
        => GraceSeconds = Math.Clamp(GraceSeconds, 1, 30);

    public HandOptions Clone()
        => (HandOptions)MemberwiseClone();
}

/// <summary>One configured timezone and its user-facing alias.</summary>
public sealed class ConfiguredTimeZone
{
    public string TimeZoneId { get; set; } = TimeZoneInfo.Local.Id;

    public string DisplayName { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public void Normalize()
    {
        TimeZoneId = string.IsNullOrWhiteSpace(TimeZoneId)
            ? TimeZoneInfo.Local.Id
            : TimeZoneId.Trim();
        DisplayName = DisplayName?.Trim() ?? string.Empty;
    }

    public ConfiguredTimeZone Clone()
        => (ConfiguredTimeZone)MemberwiseClone();
}

/// <summary>Timezone rotation and presentation settings.</summary>
public sealed class TimeZoneOptions
{
    public const int MaximumTimeZoneCount = 6;

    public bool Enabled { get; set; }

    public int ChangeToNextSeconds { get; set; } = 60;

    public int ReturnToDefaultSeconds { get; set; } = 20;

    public bool ShowOnClockFace { get; set; } = true;

    public bool ShowOnlyWhenAlternate { get; set; }

    public bool ShowHeadlineFallback { get; set; } = true;

    public List<ConfiguredTimeZone> Entries { get; set; } =
    [
        new()
        {
            TimeZoneId = TimeZoneInfo.Local.Id,
            DisplayName = "Local",
            IsDefault = true,
        },
    ];

    public void Normalize()
    {
        ChangeToNextSeconds = Snap(
            Math.Clamp(ChangeToNextSeconds, 10, 120),
            step: 10);
        ReturnToDefaultSeconds = Snap(
            Math.Clamp(ReturnToDefaultSeconds, 5, 60),
            step: 5);
        Entries ??= [];

        var normalized = new List<ConfiguredTimeZone>(MaximumTimeZoneCount);
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (ConfiguredTimeZone entry in Entries)
        {
            if (entry is null)
            {
                continue;
            }

            entry.Normalize();
            if (ids.Add(entry.TimeZoneId))
            {
                normalized.Add(entry);
            }

            if (normalized.Count == MaximumTimeZoneCount)
            {
                break;
            }
        }

        int defaultIndex = normalized.FindIndex(entry => entry.IsDefault);
        if (defaultIndex < 0)
        {
            defaultIndex = normalized.FindIndex(
                entry => string.Equals(
                    entry.TimeZoneId,
                    TimeZoneInfo.Local.Id,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (defaultIndex < 0)
        {
            normalized.Insert(
                0,
                new ConfiguredTimeZone
                {
                    TimeZoneId = TimeZoneInfo.Local.Id,
                    DisplayName = "Local",
                    IsDefault = true,
                });
            if (normalized.Count > MaximumTimeZoneCount)
            {
                normalized.RemoveAt(normalized.Count - 1);
            }

            defaultIndex = 0;
        }

        for (int i = 0; i < normalized.Count; i++)
        {
            normalized[i].IsDefault = i == defaultIndex;
        }

        Entries = normalized;
        Enabled &= Entries.Count > 1;
    }

    public TimeZoneOptions Clone()
        => new()
        {
            Enabled = Enabled,
            ChangeToNextSeconds = ChangeToNextSeconds,
            ReturnToDefaultSeconds = ReturnToDefaultSeconds,
            ShowOnClockFace = ShowOnClockFace,
            ShowOnlyWhenAlternate = ShowOnlyWhenAlternate,
            ShowHeadlineFallback = ShowHeadlineFallback,
            Entries = Entries.Select(entry => entry.Clone()).ToList(),
        };

    private static int Snap(int value, int step)
        => (int)Math.Round(value / (double)step, MidpointRounding.AwayFromZero) * step;
}

/// <summary>Global ticker and optional theme-visual visibility settings.</summary>
public sealed class DisplayOptions
{
    private static readonly TickerContentSource[] s_defaultTickerOrder =
    [
        TickerContentSource.CustomMessage,
        TickerContentSource.CurrentDate,
        TickerContentSource.TimeZone,
        TickerContentSource.ThemeName,
    ];

    public bool TickerEnabled { get; set; }

    public string CustomTickerMessage { get; set; } = string.Empty;

    public List<TickerContentSource> TickerContentOrder { get; set; } = [.. s_defaultTickerOrder];

    public bool ShowThemeTickerVisual { get; set; } = true;

    public bool ShowFractionSecondVisual { get; set; } = true;

    public void Normalize()
    {
        CustomTickerMessage = CustomTickerMessage?.Trim() ?? string.Empty;
        TickerContentOrder ??= [];

        TickerContentOrder = TickerContentOrder
            .Where(source => Enum.IsDefined(source))
            .Distinct()
            .ToList();
    }

    public DisplayOptions Clone()
        => new()
        {
            TickerEnabled = TickerEnabled,
            CustomTickerMessage = CustomTickerMessage,
            TickerContentOrder = [.. TickerContentOrder],
            ShowThemeTickerVisual = ShowThemeTickerVisual,
            ShowFractionSecondVisual = ShowFractionSecondVisual,
        };
}

/// <summary>Configurable application content folders.</summary>
public sealed class FolderOptions
{
    public string ThemesFolder { get; set; } = string.Empty;

    public string CalendarFolder { get; set; } = string.Empty;

    public string ShortMessagesFolder { get; set; } = string.Empty;

    public string PicturesFolder { get; set; } = string.Empty;

    public void Normalize()
    {
        ThemesFolder = NormalizePath(ThemesFolder);
        CalendarFolder = NormalizePath(CalendarFolder);
        ShortMessagesFolder = NormalizePath(ShortMessagesFolder);
        PicturesFolder = NormalizePath(PicturesFolder);
    }

    public FolderOptions Clone()
        => (FolderOptions)MemberwiseClone();

    private static string NormalizePath(string? path)
        => string.IsNullOrWhiteSpace(path) ? string.Empty : path.Trim();
}
