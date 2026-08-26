using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  A classic radial analog clock theme parameterized by a <see cref="StandardClockDesign"/>.
///  Used for the radial stock-theme families; demonstrates that the plug-in contract is
///  rich enough to express the original GDI+ clock's looks.
/// </summary>
public sealed class StandardClockTheme : IClockTheme
{
    private readonly StandardClockDesign _design;
    private readonly ClockThemeVariantKind _variant;
    private readonly IReadOnlyList<ClockThemeVariantKind> _supportedVariants;
    private readonly Func<ClockThemeVariantKind, IClockTheme>? _resolver;

    public StandardClockTheme(StandardClockDesign design)
        : this(design, ClockThemeVariantKind.Day, ClockThemeVariants.DayOnly, resolver: null)
    {
    }

    internal StandardClockTheme(
        StandardClockDesign design,
        ClockThemeVariantKind variant,
        IReadOnlyList<ClockThemeVariantKind> supportedVariants,
        Func<ClockThemeVariantKind, IClockTheme>? resolver)
    {
        _design = design;
        _variant = variant;
        _supportedVariants = supportedVariants;
        _resolver = resolver;
    }

    /// <inheritdoc/>
    public string Name => _design.Name;

    /// <inheritdoc/>
    public string Description => _design.Description;

    /// <inheritdoc/>
    public string Author => "stock theme";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

    /// <inheritdoc/>
    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => _supportedVariants;

    /// <inheritdoc/>
    public IClockTheme ResolveVariant(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(_supportedVariants, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(Name, _supportedVariants, variant);
        }

        return variant == _variant
            ? this
            : _resolver?.Invoke(variant)
                ?? throw new InvalidOperationException($"Theme '{Name}' cannot resolve sibling variants.");
    }

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new()
            {
                Id = ClockElementId.Face,
                ContentSize = new SizeF(1000f, 1000f),
                Pivot = new PointF(500f, 500f),
                ZOrder = 0,
            },
        };

        if (_design.MinuteTickStyle != MinuteTickStyle.None)
        {
            for (int i = 0; i < 60; i++)
            {
                elements.Add(new ClockElementDescriptor
                {
                    Id = ClockElementId.MinuteTick(i),
                    ContentSize = new SizeF(90f, 90f),
                    Pivot = new PointF(45f, 45f),
                    ZOrder = 10,
                });
            }
        }

        foreach (int hour in HourIndices())
        {
            float markerSize = _design.Ornate ? 190f : 150f;
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(hour),
                ContentSize = new SizeF(markerSize, markerSize),
                Pivot = new PointF(markerSize / 2f, markerSize / 2f),
                ZOrder = 20,
            });
        }

        elements.Add(HandDescriptor(ClockElementId.HourHand, ClockHandKind.Hour, HandSlot.Hour, 30));
        elements.Add(HandDescriptor(ClockElementId.MinuteHand, ClockHandKind.Minute, HandSlot.Minute, 31));
        elements.Add(HandDescriptor(ClockElementId.SecondHand, ClockHandKind.Second, HandSlot.Second, 32));

        elements.Add(new ClockElementDescriptor
        {
            Id = ClockElementId.Arbour,
            ContentSize = new SizeF(80f, 80f),
            Pivot = new PointF(40f, 40f),
            ZOrder = 40,
        });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new RadialLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new StandardClockRenderer(_design);

    /// <inheritdoc/>
    public IThemeAnimator? CreateAnimator() => null;

    private ClockElementDescriptor HandDescriptor(ClockElementId id, ClockHandKind hand, HandSlot slot, int z)
    {
        HandShape shape = HandGeometry.Build(_design.HandStyle, slot);
        return new ClockElementDescriptor
        {
            Id = id,
            ContentSize = shape.Size,
            Pivot = shape.Pivot,
            Hand = hand,
            ZOrder = z,
        };
    }

    private IEnumerable<int> HourIndices()
        => _design.HourMarkerStyle == HourMarkerStyle.Quadrants
            ? [0, 3, 6, 9]
            : Enumerable.Range(0, 12);
}
