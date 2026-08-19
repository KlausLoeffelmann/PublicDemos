namespace WarpClock.Abstractions;

/// <summary>
///  A stable identity for a single clock element: its <see cref="ClockElementKind"/>
///  plus an index that disambiguates repeated kinds (e.g. the twelve
///  <see cref="ClockElementKind.HourMarker"/>s or sixty
///  <see cref="ClockElementKind.MinuteTick"/>s).
/// </summary>
public readonly record struct ClockElementId(ClockElementKind Kind, int Index = 0)
{
    /// <summary>The hour-hand element id.</summary>
    public static ClockElementId HourHand { get; } = new(ClockElementKind.HourHand);

    /// <summary>The minute-hand element id.</summary>
    public static ClockElementId MinuteHand { get; } = new(ClockElementKind.MinuteHand);

    /// <summary>The second-hand element id.</summary>
    public static ClockElementId SecondHand { get; } = new(ClockElementKind.SecondHand);

    /// <summary>The arbour element id.</summary>
    public static ClockElementId Arbour { get; } = new(ClockElementKind.Arbour);

    /// <summary>The face element id.</summary>
    public static ClockElementId Face { get; } = new(ClockElementKind.Face);

    /// <summary>The background element id.</summary>
    public static ClockElementId Background { get; } = new(ClockElementKind.Background);

    /// <summary>The time-zone element id.</summary>
    public static ClockElementId TimeZone { get; } = new(ClockElementKind.TimeZone);

    /// <summary>The day element id.</summary>
    public static ClockElementId Day { get; } = new(ClockElementKind.Day);

    /// <summary>The weekday element id.</summary>
    public static ClockElementId Weekday { get; } = new(ClockElementKind.Weekday);

    /// <summary>The overlay-message element id.</summary>
    public static ClockElementId OverlayMessage { get; } = new(ClockElementKind.OverlayMessage);

    /// <summary>The fractional-second dial element id.</summary>
    public static ClockElementId FractionSecondDial { get; } = new(ClockElementKind.FractionSecondDial);

    /// <summary>Creates an hour-marker id (index 0..11, where 0 = the 12 position).</summary>
    public static ClockElementId HourMarker(int index) => new(ClockElementKind.HourMarker, index);

    /// <summary>Creates a minute-tick id (index 0..59).</summary>
    public static ClockElementId MinuteTick(int index) => new(ClockElementKind.MinuteTick, index);

    /// <summary>Creates an indexed-image id.</summary>
    public static ClockElementId IndexedImage(int index) => new(ClockElementKind.IndexedImage, index);

    /// <summary>Creates a custom element id.</summary>
    public static ClockElementId CustomElement(int index) => new(ClockElementKind.Custom, index);

    /// <inheritdoc/>
    public override string ToString() => Index == 0 ? Kind.ToString() : $"{Kind}[{Index}]";
}
