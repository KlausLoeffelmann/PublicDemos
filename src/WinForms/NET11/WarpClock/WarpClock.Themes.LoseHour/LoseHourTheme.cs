using System.Drawing;

using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Themes.LoseHour;

/// <summary>
///  A free-floating demo theme: the twelve hour positions are arranged in a vertical
///  column down the left side of the dial, but only the three numerals nearest the
///  current hour are shown. As time passes the bright window slides down the column,
///  each numeral "falling" and fading into the next — and because the hour hand always
///  aims at the (relocated) hour anchors, its tip follows the column. Crawling is
///  disabled by the engine in this free-floating layout; grace catch-up eases the hands.
/// </summary>
public sealed class LoseHourTheme : IClockTheme
{
    private static readonly string[] s_labels =
        ["12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"];

    /// <inheritdoc/>
    public string Name => "Lose-Hour";

    /// <inheritdoc/>
    public string Description => "Hours stacked in a left column; only three show at a time and fall as time passes.";

    /// <inheritdoc/>
    public string Author => "WarpClock sample plug-in";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = new()
    {
        FreeFloating = true,
        HandsFollowFaceRotation = true,
        VisibleHourCount = 3,
    };

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        var elements = new List<ClockElementDescriptor>
        {
            new() { Id = ClockElementId.Face, ContentSize = new SizeF(1000, 1000), Pivot = new PointF(500, 500), ZOrder = 0 },
        };

        for (int i = 0; i < 12; i++)
        {
            elements.Add(new ClockElementDescriptor
            {
                Id = ClockElementId.HourMarker(i),
                ContentSize = new SizeF(150, 150),
                Pivot = new PointF(75, 75),
                ZOrder = 20,
                RedrawPerFrame = true,
            });
        }

        elements.Add(HandDescriptor(ClockElementId.HourHand, ClockHandKind.Hour));
        elements.Add(HandDescriptor(ClockElementId.MinuteHand, ClockHandKind.Minute));
        elements.Add(HandDescriptor(ClockElementId.SecondHand, ClockHandKind.Second));
        elements.Add(new ClockElementDescriptor { Id = ClockElementId.Arbour, ContentSize = new SizeF(70, 70), Pivot = new PointF(35, 35), ZOrder = 40 });

        return elements;
    }

    private static ClockElementDescriptor HandDescriptor(ClockElementId id, ClockHandKind hand)
    {
        NeedleSpec spec = NeedleSpec.For(hand);
        return new ClockElementDescriptor
        {
            Id = id,
            ContentSize = spec.Size,
            Pivot = spec.Pivot,
            Hand = hand,
            ZOrder = hand == ClockHandKind.Second ? 32 : hand == ClockHandKind.Minute ? 31 : 30,
        };
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new LeftColumnLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new LoseHourRenderer(s_labels);

    /// <inheritdoc/>
    public IThemeAnimator CreateAnimator() => new LoseHourAnimator();

    /// <summary>Stacks the twelve hour anchors in a vertical column on the left.</summary>
    private sealed class LeftColumnLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            if (id.Kind != ClockElementKind.HourMarker)
            {
                anchor = default;
                return false;
            }

            int i = ((id.Index % 12) + 12) % 12;
            float top = surface.Height * 0.14f;
            float span = surface.Height * 0.72f;
            float x = surface.Width * 0.18f;
            anchor = new PointF(x, top + span * (i / 11f));
            return true;
        }
    }

    /// <summary>Drives the falling / fading of the hour column.</summary>
    private sealed class LoseHourAnimator : IThemeAnimator
    {
        public void OnTick(IClockTickContext context)
        {
            float fractionalHour = (context.Time.Now.Hour % 12)
                + context.Time.Now.Minute / 60f
                + context.Time.Now.Second / 3600f;

            foreach (ClockElementDescriptor descriptor in context.Elements)
            {
                if (descriptor.Id.Kind != ClockElementKind.HourMarker)
                {
                    continue;
                }

                int i = descriptor.Id.Index;
                float distance = CircularDistance(i, fractionalHour, 12f);
                ClockElementParameters p = context.GetParameters(descriptor.Id);

                // Show only the three nearest hours; fade at the edges.
                p.Visible = distance <= 1.5f;
                p.Opacity = Math.Clamp(1.2f - distance, 0f, 1f);

                // "Fall" bob as the bright window passes a numeral.
                float frac = distance - MathF.Floor(distance);
                p.AnchorOffset = new PointF(0f, MathF.Sin(frac * MathF.PI) * 18f);
                p.RedrawRequested = true;
            }
        }

        private static float CircularDistance(float a, float b, float modulo)
        {
            float d = MathF.Abs(a - b) % modulo;
            return MathF.Min(d, modulo - d);
        }
    }
}
