namespace WarpClock.App;

/// <summary>
///  Persisted automatic theme-rotation configuration.
/// </summary>
public sealed class ThemeScheduleDocument
{
    public const int CurrentSchemaVersion = 3;
    public static readonly TimeOnly DefaultDayStartsAt = new(7, 0);
    public static readonly TimeOnly DefaultNightStartsAt = new(19, 0);
    public const int DefaultRotationMinutes = 30;

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    public string Name { get; set; } = string.Empty;

    public bool AutoRotate { get; set; } = true;

    public TimeOnly? DayStartsAt { get; set; } = DefaultDayStartsAt;

    public TimeOnly? NightStartsAt { get; set; } = DefaultNightStartsAt;

    public int? RotationMinutes { get; set; } = DefaultRotationMinutes;

    public List<ThemeScheduleEntry> Entries { get; set; } = [];

    public void Normalize()
    {
        SchemaVersion = CurrentSchemaVersion;
        Name = Name?.Trim() ?? string.Empty;

        if (DayStartsAt is null)
        {
            DayStartsAt = DefaultDayStartsAt;
        }

        if (NightStartsAt is null)
        {
            NightStartsAt = DefaultNightStartsAt;
        }

        if (!AutoRotate)
        {
            RotationMinutes = null;
        }
        else if (RotationMinutes is null || RotationMinutes <= 0)
        {
            RotationMinutes = DefaultRotationMinutes;
        }

        Entries ??= [];

        foreach (ThemeScheduleEntry entry in Entries)
        {
            entry.Normalize();
        }
    }

    public ThemeScheduleDocument Clone()
        => new()
        {
            SchemaVersion = SchemaVersion,
            Name = Name,
            AutoRotate = AutoRotate,
            DayStartsAt = DayStartsAt,
            NightStartsAt = NightStartsAt,
            RotationMinutes = RotationMinutes,
            Entries = Entries.Select(entry => entry.Clone()).ToList(),
        };
}

public sealed class ThemeScheduleEntry
{
    public ThemeReference Theme { get; set; } = new();

    public string DisplayName { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public bool EligibleDuringDay { get; set; } = true;

    public bool EligibleDuringNight { get; set; } = true;

    public void Normalize()
    {
        Theme ??= new ThemeReference();
        ThemeReferenceUtility.Normalize(Theme);
        DisplayName ??= string.Empty;
        Source ??= string.Empty;
    }

    public ThemeScheduleEntry Clone()
        => new()
        {
            Theme = ThemeReferenceUtility.Clone(Theme) ?? new ThemeReference(),
            DisplayName = DisplayName,
            Source = Source,
            Enabled = Enabled,
            EligibleDuringDay = EligibleDuringDay,
            EligibleDuringNight = EligibleDuringNight,
        };
}
