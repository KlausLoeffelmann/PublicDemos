namespace WarpClock.Abstractions;

/// <summary>
///  The kind of a clock element. The engine owns the meaning of each kind; a theme
///  decides which kinds it materializes, how they look, and (within limits) where
///  they sit — but never what time a hand points at.
/// </summary>
public enum ClockElementKind
{
    /// <summary>Full-surface backdrop drawn behind everything.</summary>
    Background,

    /// <summary>The stylized case / bezel around the dial.</summary>
    Case,

    /// <summary>The dial face (filled disc) the markers and hands sit on.</summary>
    Face,

    /// <summary>An hour numeral / index marker. Index 0..11 maps to 12,1,2,...,11.</summary>
    HourMarker,

    /// <summary>A minute tick. Index 0..59.</summary>
    MinuteTick,

    /// <summary>The hour hand.</summary>
    HourHand,

    /// <summary>The minute hand.</summary>
    MinuteHand,

    /// <summary>The second hand.</summary>
    SecondHand,

    /// <summary>An auxiliary sub-second / fractional dial hand.</summary>
    SubSecondHand,

    /// <summary>The center cap covering the hand pivots.</summary>
    Arbour,

    /// <summary>The date drum / aperture.</summary>
    DateDrum,

    /// <summary>The AM/PM drum / aperture.</summary>
    AmPmDrum,

    /// <summary>A theme-specific custom element. Disambiguate via <see cref="ClockElementId.Index"/>.</summary>
    Custom,

    /// <summary>An auxiliary dial for a fractional-second / sub-second hand.</summary>
    FractionSecondDial,

    /// <summary>A rendered time-zone label or badge.</summary>
    TimeZone,

    /// <summary>A rendered day-of-month indicator.</summary>
    Day,

    /// <summary>A rendered weekday indicator.</summary>
    Weekday,

    /// <summary>A transient overlay message supplied by the host.</summary>
    OverlayMessage,

    /// <summary>A host-supplied indexed auxiliary image.</summary>
    IndexedImage
}
