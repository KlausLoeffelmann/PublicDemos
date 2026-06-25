using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  Factory for the three built-in analog themes ported from the original GDI+ clock:
///  Railway Classic, Modern Minimal, and Antique Worn.
/// </summary>
public static class BuiltInThemes
{
    /// <summary>Swiss-railway look: cream face, bold numerals, red lollipop second hand.</summary>
    public static IClockTheme RailwayClassic() => new StandardClockTheme(new StandardClockDesign
    {
        Name = "Railway Classic",
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
    });

    /// <summary>Dark, minimal face with cyan accents and quadrant numerals.</summary>
    public static IClockTheme ModernMinimal() => new StandardClockTheme(new StandardClockDesign
    {
        Name = "Modern Minimal",
        Description = "Dark minimalist dial with cyan second hand.",
        FaceColor = Color.FromArgb(20, 22, 25),
        FaceBorderColor = Color.FromArgb(60, 65, 70),
        FaceBorderWidth = 10f,
        SecondHandColor = Color.FromArgb(0, 180, 255),
        MinuteHandColor = Color.FromArgb(220, 220, 225),
        HourHandColor = Color.FromArgb(220, 220, 225),
        HourMarkerColor = Color.FromArgb(200, 200, 205),
        MinuteTickColor = Color.FromArgb(90, 90, 95),
        ArbourColor = Color.FromArgb(180, 180, 185),
        HandStyle = HandStyle.Modern,
        HourCulture = HourCulture.Arabic,
        HourMarkerStyle = HourMarkerStyle.Quadrants,
        MinuteTickStyle = MinuteTickStyle.Track,
    });

    /// <summary>Aged parchment face with Roman numerals and spade hands.</summary>
    public static IClockTheme AntiqueWorn() => new StandardClockTheme(new StandardClockDesign
    {
        Name = "Antique Worn",
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
    });

    /// <summary>All built-in themes in display order.</summary>
    public static IReadOnlyList<IClockTheme> All() => [RailwayClassic(), ModernMinimal(), AntiqueWorn()];
}
