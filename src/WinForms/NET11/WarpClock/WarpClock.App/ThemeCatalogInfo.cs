using WarpClock.Abstractions;

namespace WarpClock.App;

/// <summary>
///  Serializable theme-family metadata used by persistence, scheduling, and tests.
/// </summary>
public sealed class ThemeCatalogInfo
{
    public string ThemeKey { get; init; } = string.Empty;

    public string FamilyName { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants { get; init; } = ClockThemeVariants.DayOnly;

    public bool SupportsVariant(ClockThemeVariantKind variant)
        => ClockThemeVariants.Supports(SupportedVariants, variant);

    public bool SupportsPeriod(ThemeSchedulePeriod period, ClockThemeVariantKind? explicitVariant = null)
    {
        if (explicitVariant is ClockThemeVariantKind pinnedVariant)
        {
            return SupportsVariant(pinnedVariant)
                && ClockThemeVariants.IsNight(pinnedVariant) == (period == ThemeSchedulePeriod.Night);
        }

        return SupportedVariants.Any(variant => ClockThemeVariants.IsNight(variant) == (period == ThemeSchedulePeriod.Night));
    }

    public ClockThemeVariantKind ResolveVariant(ClockThemeVariantKind? requestedVariant, ThemeSchedulePeriod currentPeriod)
        => ResolveVariant(requestedVariant, currentPeriod, preferOledVariants: false);

    public ClockThemeVariantKind ResolveVariant(
        ClockThemeVariantKind? requestedVariant,
        ThemeSchedulePeriod currentPeriod,
        bool preferOledVariants)
    {
        if (requestedVariant is ClockThemeVariantKind explicitVariant && SupportsVariant(explicitVariant))
        {
            return explicitVariant;
        }

        bool night = currentPeriod == ThemeSchedulePeriod.Night;
        ClockThemeVariantKind preferred = ClockThemeVariants.Compose(night, preferOledVariants);
        if (SupportsVariant(preferred))
        {
            return preferred;
        }

        ClockThemeVariantKind samePeriodAlternate = ClockThemeVariants.Compose(night, !preferOledVariants);
        if (samePeriodAlternate != preferred && SupportsVariant(samePeriodAlternate))
        {
            return samePeriodAlternate;
        }

        for (int i = 0; i < SupportedVariants.Count; i++)
        {
            ClockThemeVariantKind variant = SupportedVariants[i];
            if (ClockThemeVariants.IsNight(variant) == night)
            {
                return variant;
            }
        }

        return SupportedVariants.Count == 0
            ? ClockThemeVariantKind.Day
            : SupportedVariants[0];
    }

    public string GetConcreteDisplayName(ClockThemeVariantKind? requestedVariant, ThemeSchedulePeriod currentPeriod)
    {
        ClockThemeVariantKind resolved = ResolveVariant(requestedVariant, currentPeriod);

        return SupportedVariants.Count > 1
            ? ClockThemeVariants.FormatDisplayName(FamilyName, resolved)
            : FamilyName;
    }

    public static string CreateThemeKey(string familyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName);

        string logicalName = StripKnownVariantSuffix(familyName.Trim());
        Span<char> buffer = stackalloc char[logicalName.Length];
        int index = 0;
        bool pendingDash = false;

        foreach (char c in logicalName)
        {
            if (char.IsLetterOrDigit(c))
            {
                if (pendingDash && index > 0)
                {
                    buffer[index++] = '-';
                }

                buffer[index++] = char.ToLowerInvariant(c);
                pendingDash = false;
            }
            else if (index > 0)
            {
                pendingDash = true;
            }
        }

        return index == 0
            ? "theme"
            : new string(buffer[..index]);
    }

    public static string CreateThemeKey(string source, string familyName, Type themeType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName);
        ArgumentNullException.ThrowIfNull(themeType);

        return CreateThemeKey(familyName);
    }

    public static string NormalizeThemeKey(string? themeKey)
    {
        if (string.IsNullOrWhiteSpace(themeKey))
        {
            return string.Empty;
        }

        string trimmed = themeKey.Trim();
        string candidate = trimmed;

        string[] parts = trimmed.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]))
        {
            candidate = parts[1];
        }

        return CreateThemeKey(candidate);
    }

    public static bool ThemeKeysMatch(string? left, string? right)
        => string.Equals(
            NormalizeThemeKey(left),
            NormalizeThemeKey(right),
            StringComparison.OrdinalIgnoreCase);

    private static string StripKnownVariantSuffix(string familyName)
    {
        foreach (ClockThemeVariantKind variant in Enum.GetValues<ClockThemeVariantKind>())
        {
            string suffix = " - " + ClockThemeVariants.GetLabel(variant);
            if (familyName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return familyName[..^suffix.Length];
            }
        }

        return familyName;
    }
}

/// <summary>
///  A persisted reference to a theme family, optionally pinned to a concrete variant.
/// </summary>
public sealed class ThemeReference
{
    public string ThemeKey { get; set; } = string.Empty;

    public ClockThemeVariantKind? Variant { get; set; }
}

public static class ThemeReferenceUtility
{
    public static void Normalize(ThemeReference? reference)
    {
        if (reference is null)
        {
            return;
        }

        reference.ThemeKey = ThemeCatalogInfo.NormalizeThemeKey(reference.ThemeKey);
    }

    public static ThemeReference? Clone(ThemeReference? reference)
        => reference is null
            ? null
            : new ThemeReference
            {
                ThemeKey = ThemeCatalogInfo.NormalizeThemeKey(reference.ThemeKey),
                Variant = reference.Variant,
            };

    public static bool Equals(ThemeReference? left, ThemeReference? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return ThemeCatalogInfo.ThemeKeysMatch(left.ThemeKey, right.ThemeKey)
            && left.Variant == right.Variant;
    }
}
