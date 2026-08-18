using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  Factory for the stock theme catalog. The radial GDI+-inspired families are joined by
///  the converted Nerd and Scatter themes plus the OLED-capable Logical family, and every
///  stock family publishes explicit logical variants.
/// </summary>
public static class BuiltInThemes
{
    /// <summary>Swiss-railway look: cream face, bold numerals, red lollipop second hand.</summary>
    public static IClockTheme RailwayClassic() => RailwayClassic(ClockThemeVariantKind.Day);

    /// <summary>Swiss-railway look with an explicit palette variant.</summary>
    public static IClockTheme RailwayClassic(ClockThemeVariantKind variant)
        => new StandardClockTheme(CreateRailwayClassicDesign(variant), variant, ClockThemeVariants.DayNight, RailwayClassic);

    /// <summary>Minimal face with cyan accents and quadrant numerals.</summary>
    public static IClockTheme ModernMinimal() => ModernMinimal(ClockThemeVariantKind.Day);

    /// <summary>Modern minimal look with an explicit palette variant.</summary>
    public static IClockTheme ModernMinimal(ClockThemeVariantKind variant)
        => new StandardClockTheme(CreateModernMinimalDesign(variant), variant, ClockThemeVariants.DayNight, ModernMinimal);

    /// <summary>Aged face with Roman numerals and spade hands.</summary>
    public static IClockTheme AntiqueWorn() => AntiqueWorn(ClockThemeVariantKind.Day);

    /// <summary>Antique dial with an explicit palette variant.</summary>
    public static IClockTheme AntiqueWorn(ClockThemeVariantKind variant)
        => new StandardClockTheme(CreateAntiqueWornDesign(variant), variant, ClockThemeVariants.DayNight, AntiqueWorn);

    /// <summary>The stock binary/octal second-hand theme.</summary>
    public static IClockTheme Nerd() => new NerdTheme();

    /// <summary>The stock binary/octal second-hand theme with an explicit palette variant.</summary>
    public static IClockTheme Nerd(ClockThemeVariantKind variant)
        => variant == ClockThemeVariantKind.Day ? new NerdTheme() : new NerdTheme(variant);

    /// <summary>The stock magnetic free-floating numeral theme.</summary>
    public static IClockTheme Scatter() => new ScatterTheme();

    /// <summary>The stock magnetic free-floating numeral theme with an explicit palette variant.</summary>
    public static IClockTheme Scatter(ClockThemeVariantKind variant)
        => variant == ClockThemeVariantKind.Day ? new ScatterTheme() : new ScatterTheme(variant);

    /// <summary>The stock modern clock that storms apart and reassembles at safe offsets.</summary>
    public static IClockTheme Logical() => new LogicalTheme();

    /// <summary>The stock modern clock with an explicit logical variant.</summary>
    public static IClockTheme Logical(ClockThemeVariantKind variant)
        => variant == ClockThemeVariantKind.Day ? new LogicalTheme() : new LogicalTheme(variant);

    /// <summary>
    ///  All stock theme families in display order. Each returned instance is the family's
    ///  default/catalog entry and advertises its logical variants via
    ///  <see cref="IClockTheme.SupportedVariants"/> / <see cref="IClockTheme.ResolveVariant(ClockThemeVariantKind)"/>.
    /// </summary>
    public static IReadOnlyList<IClockTheme> All()
        => [RailwayClassic(), ModernMinimal(), AntiqueWorn(), Nerd(), Scatter(), Logical()];

    private static StandardClockDesign CreateRailwayClassicDesign(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new StandardClockDesign
            {
                Name = ClockThemeVariants.FormatDisplayName("Railway Classic", variant),
                Description = "Swiss railway station clock with a lollipop second hand.",
                FaceColor = Color.FromArgb(245, 245, 240),
                FaceBorderColor = Color.FromArgb(60, 60, 60),
                FaceBorderWidth = 22f,
                SecondHandColor = Color.FromArgb(220, 40, 30),
                MinuteHandColor = Color.FromArgb(30, 30, 30),
                HourHandColor = Color.FromArgb(30, 30, 30),
                HourMarkerColor = Color.FromArgb(30, 30, 30),
                MinuteTickColor = Color.FromArgb(70, 70, 70),
                ArbourColor = Color.FromArgb(30, 30, 30),
                HandStyle = HandStyle.Railway,
                HourCulture = HourCulture.Arabic,
                HourMarkerStyle = HourMarkerStyle.All,
                MinuteTickStyle = MinuteTickStyle.Track,
            },
            ClockThemeVariantKind.Night => new StandardClockDesign
            {
                Name = ClockThemeVariants.FormatDisplayName("Railway Classic", variant),
                Description = "Swiss railway station clock with a lollipop second hand.",
                FaceColor = Color.FromArgb(34, 38, 44),
                FaceBorderColor = Color.FromArgb(92, 97, 104),
                FaceBorderWidth = 22f,
                SecondHandColor = Color.FromArgb(188, 92, 86),
                MinuteHandColor = Color.FromArgb(214, 214, 206),
                HourHandColor = Color.FromArgb(214, 214, 206),
                HourMarkerColor = Color.FromArgb(223, 218, 205),
                MinuteTickColor = Color.FromArgb(122, 126, 132),
                ArbourColor = Color.FromArgb(205, 205, 198),
                HandStyle = HandStyle.Railway,
                HourCulture = HourCulture.Arabic,
                HourMarkerStyle = HourMarkerStyle.All,
                MinuteTickStyle = MinuteTickStyle.Track,
            },
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException("Railway Classic", ClockThemeVariants.DayNight, variant),
        };

    private static StandardClockDesign CreateModernMinimalDesign(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new StandardClockDesign
            {
                Name = ClockThemeVariants.FormatDisplayName("Modern Minimal", variant),
                Description = "Minimalist dial with cyan second hand and quadrant numerals.",
                FaceColor = Color.FromArgb(242, 245, 248),
                FaceBorderColor = Color.FromArgb(110, 124, 138),
                FaceBorderWidth = 10f,
                SecondHandColor = Color.FromArgb(72, 148, 186),
                MinuteHandColor = Color.FromArgb(42, 52, 62),
                HourHandColor = Color.FromArgb(42, 52, 62),
                HourMarkerColor = Color.FromArgb(64, 74, 86),
                MinuteTickColor = Color.FromArgb(150, 159, 170),
                ArbourColor = Color.FromArgb(84, 103, 120),
                HandStyle = HandStyle.Modern,
                HourCulture = HourCulture.Arabic,
                HourMarkerStyle = HourMarkerStyle.Quadrants,
                MinuteTickStyle = MinuteTickStyle.Track,
            },
            ClockThemeVariantKind.Night => new StandardClockDesign
            {
                Name = ClockThemeVariants.FormatDisplayName("Modern Minimal", variant),
                Description = "Minimalist dial with cyan second hand and quadrant numerals.",
                FaceColor = Color.FromArgb(22, 25, 30),
                FaceBorderColor = Color.FromArgb(68, 74, 82),
                FaceBorderWidth = 10f,
                SecondHandColor = Color.FromArgb(84, 154, 194),
                MinuteHandColor = Color.FromArgb(216, 219, 224),
                HourHandColor = Color.FromArgb(216, 219, 224),
                HourMarkerColor = Color.FromArgb(188, 193, 200),
                MinuteTickColor = Color.FromArgb(96, 102, 110),
                ArbourColor = Color.FromArgb(158, 165, 173),
                HandStyle = HandStyle.Modern,
                HourCulture = HourCulture.Arabic,
                HourMarkerStyle = HourMarkerStyle.Quadrants,
                MinuteTickStyle = MinuteTickStyle.Track,
            },
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException("Modern Minimal", ClockThemeVariants.DayNight, variant),
        };

    private static StandardClockDesign CreateAntiqueWornDesign(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new StandardClockDesign
            {
                Name = ClockThemeVariants.FormatDisplayName("Antique Worn", variant),
                Description = "Aged parchment dial with Roman numerals and spade hands.",
                FaceColor = Color.FromArgb(240, 230, 200),
                FaceBorderColor = Color.FromArgb(100, 80, 50),
                FaceBorderWidth = 18f,
                SecondHandColor = Color.FromArgb(120, 40, 30),
                MinuteHandColor = Color.FromArgb(50, 40, 30),
                HourHandColor = Color.FromArgb(50, 40, 30),
                HourMarkerColor = Color.FromArgb(60, 50, 35),
                MinuteTickColor = Color.FromArgb(130, 120, 100),
                ArbourColor = Color.FromArgb(100, 80, 50),
                HandStyle = HandStyle.Antique,
                HourCulture = HourCulture.Roman,
                HourMarkerStyle = HourMarkerStyle.All,
                MinuteTickStyle = MinuteTickStyle.Prominent,
            },
            ClockThemeVariantKind.Night => new StandardClockDesign
            {
                Name = ClockThemeVariants.FormatDisplayName("Antique Worn", variant),
                Description = "Aged parchment dial with Roman numerals and spade hands.",
                FaceColor = Color.FromArgb(49, 43, 37),
                FaceBorderColor = Color.FromArgb(122, 101, 72),
                FaceBorderWidth = 18f,
                SecondHandColor = Color.FromArgb(143, 92, 74),
                MinuteHandColor = Color.FromArgb(197, 180, 151),
                HourHandColor = Color.FromArgb(197, 180, 151),
                HourMarkerColor = Color.FromArgb(179, 164, 134),
                MinuteTickColor = Color.FromArgb(116, 108, 92),
                ArbourColor = Color.FromArgb(122, 101, 72),
                HandStyle = HandStyle.Antique,
                HourCulture = HourCulture.Roman,
                HourMarkerStyle = HourMarkerStyle.All,
                MinuteTickStyle = MinuteTickStyle.Prominent,
            },
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException("Antique Worn", ClockThemeVariants.DayNight, variant),
        };
}
