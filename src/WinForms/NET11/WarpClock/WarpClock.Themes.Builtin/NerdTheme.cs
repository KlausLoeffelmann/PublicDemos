using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed record NerdThemePalette(Color Face, Color Grid, Color Blade, Color On, Color Off);

/// <summary>
///  A minimalist nerd dial: there is only a second hand, and that hand <i>is</i> the
///  display. Its blade carries two columns of bit dots — the hour in binary near the tip
///  and the minute in binary near the pivot — while it still sweeps the seconds (so the
///  time is read three ways at once). The hour markers around the dial are shown in octal.
/// </summary>
public sealed class NerdTheme : IClockTheme
{
    private const string BaseName = "NERD";
    private const string BaseDescription =
        "One second hand encoding hour & minute in binary; octal hour markers.";

    private readonly ClockThemeVariantKind _variant;
    private readonly NerdThemePalette _palette;

    public NerdTheme()
        : this(ClockThemeVariantKind.Day)
    {
    }

    internal NerdTheme(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(ClockThemeVariants.DayNight, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant);
        }

        _variant = variant;
        _palette = CreatePalette(variant);
    }

    /// <inheritdoc/>
    public string Name => ClockThemeVariants.FormatDisplayName(BaseName, _variant);

    /// <inheritdoc/>
    public string Description => BaseDescription;

    /// <inheritdoc/>
    public string Author => "stock theme";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

    /// <inheritdoc/>
    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => ClockThemeVariants.DayNight;

    /// <inheritdoc/>
    public IClockTheme ResolveVariant(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(SupportedVariants, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, SupportedVariants, variant);
        }

        return variant == _variant ? this : new NerdTheme(variant);
    }

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new()
            {
                Id = ClockElementId.Face,
                ContentSize = new SizeF(1000, 1000),
                Pivot = new PointF(500, 500),
                ZOrder = 0,
            },
        };

        for (int i = 0; i < 12; i++)
        {
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(i),
                ContentSize = new SizeF(170, 130),
                Pivot = new PointF(85, 65),
                ZOrder = 20,
            });
        }

        elements.Add(new ClockElementDescriptor
        {
            Id = ClockElementId.SecondHand,
            ContentSize = new SizeF(140, 520),
            Pivot = new PointF(70, 440),
            Hand = ClockHandKind.Second,
            ZOrder = 30,
            RedrawPerFrame = true,
        });

        elements.Add(new ClockElementDescriptor
        {
            Id = ClockElementId.Arbour,
            ContentSize = new SizeF(60, 60),
            Pivot = new PointF(30, 30),
            ZOrder = 40,
        });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new RadialLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new NerdRenderer(_palette);

    /// <inheritdoc/>
    public IThemeAnimator? CreateAnimator() => null;

    private static NerdThemePalette CreatePalette(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new NerdThemePalette(
                Face: Color.FromArgb(244, 248, 245),
                Grid: Color.FromArgb(55, 113, 82),
                Blade: Color.FromArgb(118, 120, 156, 132),
                On: Color.FromArgb(77, 149, 108),
                Off: Color.FromArgb(175, 204, 188)),
            ClockThemeVariantKind.Night => new NerdThemePalette(
                Face: Color.FromArgb(12, 16, 15),
                Grid: Color.FromArgb(94, 190, 136),
                Blade: Color.FromArgb(118, 47, 92, 68),
                On: Color.FromArgb(130, 232, 173),
                Off: Color.FromArgb(41, 74, 57)),
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant),
        };
}
