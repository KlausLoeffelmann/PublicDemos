namespace WarpClock.Abstractions;

/// <summary>
///  The logical visual palettes a clock theme family may expose. Themes still never own
///  time or hand pointing; a variant only changes presentation.
/// </summary>
public enum ClockThemeVariantKind
{
    /// <summary>The default light/day presentation.</summary>
    Day,

    /// <summary>A dark, muted night presentation.</summary>
    Night,

    /// <summary>An OLED-tuned day presentation.</summary>
    OledDay,

    /// <summary>An OLED-tuned night presentation.</summary>
    OledNight,
}
