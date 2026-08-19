using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed record NerdThemePalette(
    Color Face,
    Color Grid,
    Color Blade,
    Color HourOn,
    Color HourOff,
    Color MinuteOn,
    Color MinuteOff);

internal static class NerdThemeGeometry
{
    public static readonly SizeF SecondHandContentSize = new(160f, 380f);
    public static readonly PointF SecondHandPivot = new(80f, 336f);

    public const int HourBitCount = 5;
    public const int MinuteBitCount = 6;

    public const float TipInset = 32f;
    public const float TailDepth = 28f;
    public const float ShoulderInset = 30f;
    public const float ShoulderHalfWidth = 26f;
    public const float LowerHalfWidth = 34f;
    public const float TailHalfWidth = 18f;

    public const float BitColumnOffset = 16f;
    public const float DotRadius = 10.5f;
    public const float DotTop = 66f;
    public const float DotBottom = 310f;
}

/// <summary>
///  A minimalist nerd dial: there is only a second hand, and that hand <i>is</i> the
///  display. Its shortened blade carries separate binary LED columns — blue for hours and
///  red for minutes — while it still sweeps the authoritative seconds. The hour markers
///  around the dial are shown in octal.
/// </summary>
public sealed class NerdTheme : IClockTheme
{
    private const string BaseName = "NERD";
    private const string BaseDescription =
        "Short binary second hand with blue hour LEDs, red minute LEDs, and octal hour markers.";

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
            ContentSize = NerdThemeGeometry.SecondHandContentSize,
            Pivot = NerdThemeGeometry.SecondHandPivot,
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

    internal static NerdThemePalette CreatePalette(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new NerdThemePalette(
                Face: Color.FromArgb(243, 246, 249),
                Grid: Color.FromArgb(72, 84, 100),
                Blade: Color.FromArgb(116, 104, 116, 134),
                HourOn: Color.FromArgb(132, 211, 255),
                HourOff: Color.FromArgb(206, 229, 244),
                MinuteOn: Color.FromArgb(246, 156, 156),
                MinuteOff: Color.FromArgb(241, 210, 210)),
            ClockThemeVariantKind.Night => new NerdThemePalette(
                Face: Color.FromArgb(13, 16, 21),
                Grid: Color.FromArgb(112, 122, 136),
                Blade: Color.FromArgb(108, 48, 56, 68),
                HourOn: Color.FromArgb(102, 176, 216),
                HourOff: Color.FromArgb(36, 55, 68),
                MinuteOn: Color.FromArgb(204, 122, 122),
                MinuteOff: Color.FromArgb(70, 41, 45)),
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant),
        };
}
