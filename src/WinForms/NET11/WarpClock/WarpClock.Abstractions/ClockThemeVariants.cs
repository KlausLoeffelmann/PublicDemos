namespace WarpClock.Abstractions;

/// <summary>
///  Shared logical-variant lists and helper methods for hosts and theme authors. These
///  helpers keep naming, support checks, and family expansion consistent across stock and
///  plug-in themes.
/// </summary>
public static class ClockThemeVariants
{
    /// <summary>
    ///  The legacy/default family shape: a theme exposes only its Day presentation.
    /// </summary>
    public static IReadOnlyList<ClockThemeVariantKind> DayOnly { get; } =
        Array.AsReadOnly([ClockThemeVariantKind.Day]);

    /// <summary>
    ///  A two-palette Day/Night family.
    /// </summary>
    public static IReadOnlyList<ClockThemeVariantKind> DayNight { get; } =
        Array.AsReadOnly([ClockThemeVariantKind.Day, ClockThemeVariantKind.Night]);

    /// <summary>
    ///  A full Day/Night/OLED-Day/OLED-Night family.
    /// </summary>
    public static IReadOnlyList<ClockThemeVariantKind> DayNightOled { get; } =
        Array.AsReadOnly(
        [
            ClockThemeVariantKind.Day,
            ClockThemeVariantKind.Night,
            ClockThemeVariantKind.OledDay,
            ClockThemeVariantKind.OledNight,
        ]);

    /// <summary>Returns <see langword="true"/> when the variant is one of the night palettes.</summary>
    public static bool IsNight(ClockThemeVariantKind variant)
        => variant is ClockThemeVariantKind.Night or ClockThemeVariantKind.OledNight;

    /// <summary>Returns <see langword="true"/> when the variant is OLED-tuned.</summary>
    public static bool IsOled(ClockThemeVariantKind variant)
        => variant is ClockThemeVariantKind.OledDay or ClockThemeVariantKind.OledNight;

    /// <summary>
    ///  Composes a logical variant from its day/night and OLED dimensions.
    /// </summary>
    public static ClockThemeVariantKind Compose(bool night, bool oled)
        => (night, oled) switch
        {
            (false, false) => ClockThemeVariantKind.Day,
            (true, false) => ClockThemeVariantKind.Night,
            (false, true) => ClockThemeVariantKind.OledDay,
            _ => ClockThemeVariantKind.OledNight,
        };

    /// <summary>A short human-readable label for a logical variant.</summary>
    public static string GetLabel(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => "Day",
            ClockThemeVariantKind.Night => "Night",
            ClockThemeVariantKind.OledDay => "OLED-Day",
            ClockThemeVariantKind.OledNight => "OLED-Night",
            _ => variant.ToString(),
        };

    /// <summary>
    ///  Formats a concrete display name for a base theme name and logical variant.
    /// </summary>
    public static string FormatDisplayName(string baseName, ClockThemeVariantKind variant)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseName);
        return $"{baseName} - {GetLabel(variant)}";
    }

    /// <summary>
    ///  Returns <see langword="true"/> when <paramref name="supportedVariants"/> contains
    ///  <paramref name="variant"/>.
    /// </summary>
    public static bool Supports(IReadOnlyList<ClockThemeVariantKind> supportedVariants, ClockThemeVariantKind variant)
    {
        ArgumentNullException.ThrowIfNull(supportedVariants);

        for (int i = 0; i < supportedVariants.Count; i++)
        {
            if (supportedVariants[i] == variant)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Resolves every logical variant exposed by <paramref name="theme"/> into concrete
    ///  theme instances, preserving the family-declared order.
    /// </summary>
    public static IReadOnlyList<IClockTheme> ResolveSupportedVariants(IClockTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        IClockTheme[] resolved = new IClockTheme[theme.SupportedVariants.Count];
        for (int i = 0; i < resolved.Length; i++)
        {
            resolved[i] = theme.ResolveVariant(theme.SupportedVariants[i]);
        }

        return resolved;
    }

    /// <summary>
    ///  Creates the standard exception for an unsupported variant request.
    /// </summary>
    public static NotSupportedException CreateUnsupportedVariantException(
        string themeName,
        IReadOnlyList<ClockThemeVariantKind> supportedVariants,
        ClockThemeVariantKind requestedVariant)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeName);
        ArgumentNullException.ThrowIfNull(supportedVariants);

        string supported = supportedVariants.Count == 0
            ? "<none>"
            : string.Join(", ", supportedVariants.Select(GetLabel));

        return new NotSupportedException(
            $"Theme '{themeName}' does not support the {GetLabel(requestedVariant)} variant. Supported variants: {supported}.");
    }
}
