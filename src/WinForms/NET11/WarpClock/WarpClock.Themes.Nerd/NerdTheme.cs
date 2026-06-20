using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Nerd;

/// <summary>
///  A minimalist nerd dial: there is only a second hand, and that hand <i>is</i> the
///  display. Its blade carries two columns of bit dots — the hour in binary near the tip
///  and the minute in binary near the pivot — while it still sweeps the seconds (so the
///  time is read three ways at once). The hour markers around the dial are shown in octal.
/// </summary>
public sealed partial class NerdTheme : IClockTheme
{
    /// <inheritdoc/>
    public string Name 
        => "NERD";

    /// <inheritdoc/>
    public string Description 
        => "One second hand encoding hour & minute in binary; octal hour markers.";

    /// <inheritdoc/>
    public string Author 
        => "WarpClock sample plug-in";

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
                ContentSize = new SizeF(1000, 1000), 
                Pivot = new PointF(500, 500), 
                ZOrder = 0 
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
            ZOrder = 40 
        });

        return elements;
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new RadialLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new NerdRenderer();

    /// <inheritdoc/>
    public IThemeAnimator? CreateAnimator() => null;
}
