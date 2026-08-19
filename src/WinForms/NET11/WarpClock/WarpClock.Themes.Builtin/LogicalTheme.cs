using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed record LogicalThemePalette(
    Color FaceFill,
    Color FaceRing,
    Color FaceInnerRing,
    Color Numeral,
    Color HourHand,
    Color MinuteHand,
    Color SecondHand,
    Color Arbour,
    Color[] FlashColors,
    float FlashCeiling,
    float MotionCeiling);

/// <summary>
///  A modern stock clock that periodically destabilizes, flies apart, and rebuilds at a
///  burn-in-safe offset without ever taking control of the authoritative hand angles.
/// </summary>
public sealed class LogicalTheme : IClockTheme
{
    internal const string BaseName = "Logical";
    internal const float HourMarkerRadius = 390f;

    private const string BaseDescription =
        "Modern dial whose elements storm between opposite corners, rebuild, and keep the time truthful.";

    private readonly ClockThemeVariantKind _variant;
    private readonly LogicalThemePalette _palette;

    public LogicalTheme()
        : this(ClockThemeVariantKind.Day)
    {
    }

    internal LogicalTheme(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(ClockThemeVariants.DayNightOled, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNightOled, variant);
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
    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => ClockThemeVariants.DayNightOled;

    /// <inheritdoc/>
    public IClockTheme ResolveVariant(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(SupportedVariants, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, SupportedVariants, variant);
        }

        return variant == _variant ? this : new LogicalTheme(variant);
    }

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new()
            {
                Id = new ClockElementId(ClockElementKind.Case),
                ContentSize = new SizeF(1000f, 1000f),
                Pivot = new PointF(500f, 500f),
                ZOrder = -10,
            },
            new()
            {
                Id = ClockElementId.Face,
                ContentSize = new SizeF(1000f, 1000f),
                Pivot = new PointF(500f, 500f),
                ZOrder = 0,
            },
            new()
            {
                Id = ClockElementId.Weekday,
                ContentSize = new SizeF(276f, 54f),
                Pivot = new PointF(138f, 27f),
                ZOrder = 16,
            },
            new()
            {
                Id = ClockElementId.TimeZone,
                ContentSize = new SizeF(320f, 46f),
                Pivot = new PointF(160f, 23f),
                ZOrder = 17,
            },
            new()
            {
                Id = ClockElementId.Day,
                ContentSize = new SizeF(296f, 54f),
                Pivot = new PointF(148f, 27f),
                ZOrder = 18,
            },
        };

        for (int i = 0; i < 12; i++)
        {
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(i),
                ContentSize = new SizeF(148f, 112f),
                Pivot = new PointF(74f, 56f),
                ZOrder = 20,
            });
        }

        elements.Add(HandDescriptor(ClockElementId.HourHand, ClockHandKind.Hour, HandSlot.Hour, 30));
        elements.Add(HandDescriptor(ClockElementId.MinuteHand, ClockHandKind.Minute, HandSlot.Minute, 31));
        elements.Add(HandDescriptor(ClockElementId.SecondHand, ClockHandKind.Second, HandSlot.Second, 32));

        elements.Add(new ClockElementDescriptor
        {
            Id = ClockElementId.Arbour,
            ContentSize = new SizeF(72f, 72f),
            Pivot = new PointF(36f, 36f),
            ZOrder = 40,
        });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new LogicalLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new LogicalRenderer(_palette);

    /// <inheritdoc/>
    public IThemeAnimator CreateAnimator() => new LogicalThemeAnimator(_palette);

    internal static LogicalThemePalette CreatePalette(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new LogicalThemePalette(
                FaceFill: Color.FromArgb(24, 34, 48),
                FaceRing: Color.FromArgb(92, 156, 204),
                FaceInnerRing: Color.FromArgb(58, 80, 103),
                Numeral: Color.FromArgb(228, 236, 244),
                HourHand: Color.FromArgb(216, 227, 238),
                MinuteHand: Color.FromArgb(208, 220, 232),
                SecondHand: Color.FromArgb(244, 132, 92),
                Arbour: Color.FromArgb(118, 197, 224),
                FlashColors:
                [
                    Color.FromArgb(76, 214, 255),
                    Color.FromArgb(236, 102, 198),
                    Color.FromArgb(255, 188, 84),
                ],
                FlashCeiling: 0.94f,
                MotionCeiling: 1.00f),

            ClockThemeVariantKind.Night => new LogicalThemePalette(
                FaceFill: Color.FromArgb(12, 16, 22),
                FaceRing: Color.FromArgb(56, 92, 120),
                FaceInnerRing: Color.FromArgb(32, 44, 54),
                Numeral: Color.FromArgb(178, 191, 202),
                HourHand: Color.FromArgb(170, 183, 194),
                MinuteHand: Color.FromArgb(164, 176, 186),
                SecondHand: Color.FromArgb(164, 98, 72),
                Arbour: Color.FromArgb(96, 126, 142),
                FlashColors:
                [
                    Color.FromArgb(64, 150, 180),
                    Color.FromArgb(118, 96, 148),
                    Color.FromArgb(164, 118, 82),
                ],
                FlashCeiling: 0.54f,
                MotionCeiling: 0.74f),

            ClockThemeVariantKind.OledDay => new LogicalThemePalette(
                FaceFill: Color.Black,
                FaceRing: Color.FromArgb(72, 138, 182),
                FaceInnerRing: Color.FromArgb(40, 68, 90),
                Numeral: Color.FromArgb(212, 224, 232),
                HourHand: Color.FromArgb(202, 216, 228),
                MinuteHand: Color.FromArgb(196, 210, 222),
                SecondHand: Color.FromArgb(224, 126, 86),
                Arbour: Color.FromArgb(104, 180, 208),
                FlashColors:
                [
                    Color.FromArgb(72, 206, 236),
                    Color.FromArgb(218, 108, 176),
                    Color.FromArgb(236, 170, 82),
                ],
                FlashCeiling: 0.80f,
                MotionCeiling: 0.90f),

            ClockThemeVariantKind.OledNight => new LogicalThemePalette(
                FaceFill: Color.Black,
                FaceRing: Color.FromArgb(38, 68, 92),
                FaceInnerRing: Color.FromArgb(20, 32, 42),
                Numeral: Color.FromArgb(148, 160, 170),
                HourHand: Color.FromArgb(140, 152, 162),
                MinuteHand: Color.FromArgb(136, 146, 156),
                SecondHand: Color.FromArgb(138, 90, 64),
                Arbour: Color.FromArgb(76, 104, 118),
                FlashColors:
                [
                    Color.FromArgb(52, 122, 146),
                    Color.FromArgb(96, 82, 124),
                    Color.FromArgb(140, 102, 68),
                ],
                FlashCeiling: 0.38f,
                MotionCeiling: 0.62f),

            _ => throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNightOled, variant),
        };

    private static ClockElementDescriptor HandDescriptor(ClockElementId id, ClockHandKind hand, HandSlot slot, int zOrder)
    {
        HandShape shape = HandGeometry.Build(HandStyle.Modern, slot);
        return new ClockElementDescriptor
        {
            Id = id,
            ContentSize = shape.Size,
            Pivot = shape.Pivot,
            Hand = hand,
            ZOrder = zOrder,
        };
    }

    private sealed class LogicalLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            // Labels are centered in pixel space; all caption motion is design-unit
            // AnchorOffset scaled exactly once by the engine's DesignScale.
            if (id.Kind is ClockElementKind.Weekday or ClockElementKind.Day or ClockElementKind.TimeZone)
            {
                anchor = new PointF(surface.Width / 2f, surface.Height / 2f);
                return true;
            }

            anchor = default;
            return false;
        }
    }
}
