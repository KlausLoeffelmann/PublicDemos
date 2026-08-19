namespace WarpClock.Abstractions;

/// <summary>
///  Immutable host-supplied ambient content available to renderers and animators.
/// </summary>
public readonly record struct ClockAmbientSnapshot
{
    /// <summary>An empty ambient snapshot.</summary>
    public static ClockAmbientSnapshot Empty { get; } = new()
    {
        IndexedImages = Array.Empty<ClockIndexedImageSnapshot>(),
    };

    /// <summary>An optional transient overlay message.</summary>
    public string? OverlayMessage { get; init; }

    /// <summary>An optional ticker-text payload distinct from <see cref="OverlayMessage"/>.</summary>
    public string? TickerText { get; init; }

    /// <summary>An optional host-supplied alias for the displayed time zone.</summary>
    public string? TimeZoneAlias { get; init; }

    /// <summary>An optional compact time-zone designation or abbreviation.</summary>
    public string? TimeZoneDesignation { get; init; }

    /// <summary>
    ///  Whether the host is presenting its alternate ambient/time-zone state instead
    ///  of the default state, enabling alternate-only theme visibility decisions.
    /// </summary>
    public ClockAmbientPresentationState PresentationState { get; init; }

    /// <summary>Optional host-supplied indexed images or image references.</summary>
    public IReadOnlyList<ClockIndexedImageSnapshot> IndexedImages { get; init; }
}
