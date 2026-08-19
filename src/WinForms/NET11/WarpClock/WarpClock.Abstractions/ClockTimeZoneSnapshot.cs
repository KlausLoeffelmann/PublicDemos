namespace WarpClock.Abstractions;

/// <summary>
///  Immutable snapshot of the displayed time-zone data for the current frame or tick.
/// </summary>
public readonly record struct ClockTimeZoneSnapshot
{
    /// <summary>The displayed time zone.</summary>
    public required TimeZoneInfo TimeZone { get; init; }

    /// <summary>The zone's base UTC offset.</summary>
    public required TimeSpan BaseUtcOffset { get; init; }

    /// <summary>The zone's effective UTC offset at the displayed local time.</summary>
    public required TimeSpan UtcOffset { get; init; }

    /// <summary>Whether the zone supports daylight saving time.</summary>
    public required bool SupportsDaylightSavingTime { get; init; }

    /// <summary>Whether the displayed local time is in daylight saving time.</summary>
    public required bool IsDaylightSavingTime { get; init; }

    /// <summary>The time-zone identifier.</summary>
    public string Id => TimeZone.Id;

    /// <summary>The full display name.</summary>
    public string DisplayName => TimeZone.DisplayName;

    /// <summary>The standard-time name.</summary>
    public string StandardName => TimeZone.StandardName;

    /// <summary>The daylight-time name.</summary>
    public string DaylightName => TimeZone.DaylightName;

    /// <summary>The effective current name for the displayed local time.</summary>
    public string EffectiveName
        => IsDaylightSavingTime && !string.IsNullOrWhiteSpace(TimeZone.DaylightName)
            ? TimeZone.DaylightName
            : TimeZone.StandardName;

    /// <summary>Creates a snapshot for <paramref name="timeZone"/> at <paramref name="localTime"/>.</summary>
    public static ClockTimeZoneSnapshot Create(TimeZoneInfo timeZone, DateTime localTime)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        return new ClockTimeZoneSnapshot
        {
            TimeZone = timeZone,
            BaseUtcOffset = timeZone.BaseUtcOffset,
            UtcOffset = timeZone.GetUtcOffset(localTime),
            SupportsDaylightSavingTime = timeZone.SupportsDaylightSavingTime,
            IsDaylightSavingTime = timeZone.IsDaylightSavingTime(localTime),
        };
    }
}
