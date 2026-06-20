using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

/// <summary>
///  A classic radial analog clock theme parameterized by a <see cref="StandardClockDesign"/>.
///  Used for all built-in themes; demonstrates that the plug-in contract is rich enough to
///  express the original GDI+ clock's looks.
/// </summary>
public sealed class StandardClockTheme : IClockTheme
{
    private readonly StandardClockDesign _design;

    public StandardClockTheme(StandardClockDesign design) => _design = design;

    /// <inheritdoc/>
    public string Name => _design.Name;

    /// <inheritdoc/>
    public string Description => _design.Description;

    /// <inheritdoc/>
    public string Author => "WarpClock built-in";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

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
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(hour),
                ContentSize = new SizeF(150f, 150f),
                Pivot = new PointF(75f, 75f),
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
