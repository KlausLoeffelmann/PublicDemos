using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.EmberClock;

/// <summary>
///  A hand-less ember clock: twelve flames on the outer ring, Roman numerals on an inner ring, a
///  gold hub at the center. The second hand toggles each hour's ember (lit/out) as it sweeps past.
/// </summary>
public sealed class EmberClockTheme : IClockTheme
{
    internal const int HourCount = 12;

    /// <summary>Inner numeral ring radius as a fraction of the dial (flames sit at ~0.78).</summary>
    internal const float NumeralRingRadius = 0.50f;

    public string Name => "Ember Clock";

    public string Description =>
        "Twelve embers, marked with Roman numerals, that burn out as the second hand sweeps past each hour.";

    public string Author => "Ricardo Bossan";

    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var els = new List<ClockElementDescriptor>
        {
            new() { Id = ClockElementId.Face, ContentSize = new SizeF(1000, 1000), Pivot = new PointF(500, 500), ZOrder = 0 },
        };

        // Outer ring: twelve flames (engine-default radial placement at 0.78).
        for (int i = 0; i < HourCount; i++)
        {
            els.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(i),
                ContentSize = new SizeF(150, 190),
                Pivot = new PointF(75, 95),
                ZOrder = 20,
            });
        }

        // Inner ring: twelve Roman numerals, relocated by this theme's layout.
        for (int i = 0; i < HourCount; i++)
        {
            els.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.CustomElement(i),
                ContentSize = new SizeF(140, 140),
                Pivot = new PointF(70, 70),
                ZOrder = 25,
            });
        }

        // No hands: the burning wave marks the seconds. The animator reads the second angle directly.
        els.Add(new() { Id = ClockElementId.Arbour, ContentSize = new SizeF(150, 150), Pivot = new PointF(75, 75), ZOrder = 40 });

        return els;
    }

    public IClockLayout CreateLayout() => new EmberLayout();

    public IClockElementRenderer CreateRenderer() => new EmberClockRenderer();

    public IThemeAnimator CreateAnimator() => new EmberClockAnimator();

    /// <summary>Puts the numerals (custom elements) on an inner ring; everything else uses the default layout.</summary>
    private sealed class EmberLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            if (id.Kind == ClockElementKind.Custom)
            {
                // Engine geometry: centre = surface/2, dial radius = min(w,h)/2, angle clockwise from 12.
                float radius = MathF.Min(surface.Width, surface.Height) / 2f * NumeralRingRadius;
                float deg = (((id.Index % HourCount) + HourCount) % HourCount) * 30f;
                float rad = deg * (MathF.PI / 180f);
                anchor = new PointF(
                    surface.Width / 2f + MathF.Sin(rad) * radius,
                    surface.Height / 2f - MathF.Cos(rad) * radius);
                return true;
            }

            anchor = default;
            return false;
        }
    }
}
