using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.SunFlower;

/// <summary>
///  A whimsical-but-earnest garden clock. The dial is a hand-drawn <b>sunflower</b>
///  (two rings of petals around a phyllotaxis seed head); the twelve hour numerals are
///  <b>bees</b> that buzz a full 360° spin whenever a hand sweeps over them; and the
///  hands are little <b>branches</b> — a stubby twig for the hour, a leafier bough for
///  the minute, and a long whippy shoot for the second.
/// </summary>
/// <remarks>
///  Radial theme: the bees sit evenly around the dial (engine-default placement) and the
///  branches point at the authoritative time. The bees are non-hand elements, so their
///  full-turn spin is driven through <see cref="ClockElementParameters.ExtraRotationDegrees"/>
///  (which is only clamped for hands, never for ordinary elements).
/// </remarks>
public sealed class SunFlowerTheme : IClockTheme
{
    /// <inheritdoc/>
    public string Name => "SunFlower";

    /// <inheritdoc/>
    public string Description => "A sunflower dial with bee numerals that spin when a branch-hand passes them.";

    /// <inheritdoc/>
    public string Author => "WarpClock sample plug-in";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new() { Id = ClockElementId.Face, ContentSize = new SizeF(1000, 1000), Pivot = new PointF(500, 500), ZOrder = 0 },
        };

        // Twelve bees, one per hour position, each spinning about its own center.
        for (int i = 0; i < 12; i++)
        {
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(i),
                ContentSize = new SizeF(190, 190),
                Pivot = new PointF(95, 95),
                ZOrder = 20,
            });
        }

        // Branch hands of clearly distinct sizes (authored pointing straight up).
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.HourHand, ContentSize = new SizeF(210, 300), Pivot = new PointF(105, 250), Hand = ClockHandKind.Hour, ZOrder = 30 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.MinuteHand, ContentSize = new SizeF(190, 430), Pivot = new PointF(95, 372), Hand = ClockHandKind.Minute, ZOrder = 31 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.SecondHand, ContentSize = new SizeF(150, 500), Pivot = new PointF(75, 440), Hand = ClockHandKind.Second, ZOrder = 32 });
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.Arbour, ContentSize = new SizeF(96, 96), Pivot = new PointF(48, 48), ZOrder = 40 });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new RadialLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new SunFlowerRenderer();

    /// <inheritdoc/>
    public IThemeAnimator CreateAnimator() => new SunFlowerAnimator();

    /// <summary>Defers every element to the engine's default radial placement.</summary>
    private sealed class RadialLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            anchor = default;
            return false;
        }
    }
}
