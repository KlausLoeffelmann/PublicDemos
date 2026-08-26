using System.Drawing;

namespace WarpClock.Themes.Builtin;

/// <summary>Hour numeral system.</summary>
public enum HourCulture
{
    /// <summary>Arabic numerals (12, 1, 2, …).</summary>
    Arabic,

    /// <summary>Roman numerals (XII, I, II, …).</summary>
    Roman,
}

/// <summary>Which hour positions show a numeral.</summary>
public enum HourMarkerStyle
{
    /// <summary>All twelve.</summary>
    All,

    /// <summary>12, 3, 6, 9.</summary>
    Quadrants,
}

/// <summary>The minute-tick treatment.</summary>
public enum MinuteTickStyle
{
    /// <summary>No minute ticks (only hour-position dots).</summary>
    None,

    /// <summary>A full 60-tick track.</summary>
    Track,

    /// <summary>Bold ticks with dots at the hour positions.</summary>
    Prominent,
}

/// <summary>
///  The full visual specification for a built-in radial clock theme. Built-in themes
///  differ only in the values of this record.
/// </summary>
public sealed record StandardClockDesign
{
    /// <summary>The theme name.</summary>
    public required string Name { get; init; }

    /// <summary>The theme description.</summary>
    public required string Description { get; init; }

    /// <summary>Dial face fill color.</summary>
    public Color FaceColor { get; init; } = Color.FromArgb(245, 245, 240);

    /// <summary>Dial border / bezel color.</summary>
    public Color FaceBorderColor { get; init; } = Color.FromArgb(60, 60, 60);

    /// <summary>Dial border width in design units.</summary>
    public float FaceBorderWidth { get; init; } = 18f;

    /// <summary>Second-hand color.</summary>
    public Color SecondHandColor { get; init; } = Color.FromArgb(220, 40, 30);

    /// <summary>Minute-hand color.</summary>
    public Color MinuteHandColor { get; init; } = Color.FromArgb(30, 30, 30);

    /// <summary>Hour-hand color.</summary>
    public Color HourHandColor { get; init; } = Color.FromArgb(30, 30, 30);

    /// <summary>Hour numeral color.</summary>
    public Color HourMarkerColor { get; init; } = Color.FromArgb(30, 30, 30);

    /// <summary>Minute-tick color.</summary>
    public Color MinuteTickColor { get; init; } = Color.FromArgb(80, 80, 80);

    /// <summary>Center cap color.</summary>
    public Color ArbourColor { get; init; } = Color.FromArgb(30, 30, 30);

    /// <summary>The hand silhouette.</summary>
    public HandStyle HandStyle { get; init; } = HandStyle.Railway;

    /// <summary>The numeral system.</summary>
    public HourCulture HourCulture { get; init; } = HourCulture.Arabic;

    /// <summary>Which hours show numerals.</summary>
    public HourMarkerStyle HourMarkerStyle { get; init; } = HourMarkerStyle.All;

    /// <summary>The minute-tick treatment.</summary>
    public MinuteTickStyle MinuteTickStyle { get; init; } = MinuteTickStyle.Track;

    /// <summary>The numeral font family.</summary>
    public string FontFamily { get; init; } = "Segoe UI";

    /// <summary>Use layered period ornamentation for face, numerals, and hands.</summary>
    public bool Ornate { get; init; }

    /// <summary>Add deterministic patina and wear to the day face.</summary>
    public bool AgedSurface { get; init; }

    /// <summary>Accent color for ornamental bands and flourishes.</summary>
    public Color OrnamentColor { get; init; } = Color.FromArgb(142, 55, 49);
}
