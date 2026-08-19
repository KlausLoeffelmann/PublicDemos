namespace WarpClock.Abstractions;

/// <summary>
///  Visibility gates for optional auxiliary visuals the host may want to suppress.
/// </summary>
public readonly record struct ClockAuxiliaryVisibility
{
    /// <summary>Initializes the default all-visible state.</summary>
    public ClockAuxiliaryVisibility()
    {
        ShowTimeZone = true;
        ShowDay = true;
        ShowWeekday = true;
        ShowFractionSecond = true;
        ShowOverlayMessage = true;
        ShowIndexedImages = true;
    }

    /// <summary>The default auxiliary-visibility set.</summary>
    public static ClockAuxiliaryVisibility Default { get; } = new()
    {
        ShowTimeZone = true,
        ShowDay = true,
        ShowWeekday = true,
        ShowFractionSecond = true,
        ShowOverlayMessage = true,
        ShowIndexedImages = true,
    };

    /// <summary>Whether time-zone visuals are shown.</summary>
    public bool ShowTimeZone { get; init; }

    /// <summary>Whether day-of-month visuals are shown.</summary>
    public bool ShowDay { get; init; }

    /// <summary>Whether weekday visuals are shown.</summary>
    public bool ShowWeekday { get; init; }

    /// <summary>Whether fractional-second auxiliaries are shown.</summary>
    public bool ShowFractionSecond { get; init; }

    /// <summary>Whether overlay-message visuals are shown.</summary>
    public bool ShowOverlayMessage { get; init; }

    /// <summary>Whether indexed-image visuals are shown.</summary>
    public bool ShowIndexedImages { get; init; }
}
