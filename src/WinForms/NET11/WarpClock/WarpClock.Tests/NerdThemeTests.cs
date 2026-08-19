using System.Drawing;

using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.Tests;

public sealed class NerdThemeTests
{
    [Fact]
    public void NerdPaletteUsesSeparateBlueHourAndRedMinuteLedBanksForDayAndNight()
    {
        NerdThemePalette day = NerdTheme.CreatePalette(ClockThemeVariantKind.Day);
        NerdThemePalette night = NerdTheme.CreatePalette(ClockThemeVariantKind.Night);

        Assert.Equal(Color.FromArgb(132, 211, 255), day.HourOn);
        Assert.Equal(Color.FromArgb(206, 229, 244), day.HourOff);
        Assert.Equal(Color.FromArgb(246, 156, 156), day.MinuteOn);
        Assert.Equal(Color.FromArgb(241, 210, 210), day.MinuteOff);

        Assert.Equal(Color.FromArgb(102, 176, 216), night.HourOn);
        Assert.Equal(Color.FromArgb(36, 55, 68), night.HourOff);
        Assert.Equal(Color.FromArgb(204, 122, 122), night.MinuteOn);
        Assert.Equal(Color.FromArgb(70, 41, 45), night.MinuteOff);
    }

    [Fact]
    public void NerdSecondHandStaysAuthoritativePerFrameAndClearsTheOctalMarkerRing()
    {
        IReadOnlyList<ClockElementDescriptor> elements = new NerdTheme().CreateElements();
        ClockElementDescriptor secondHand = elements.Single(element => element.Id == ClockElementId.SecondHand);
        ClockElementDescriptor topMarker = elements.Single(element => element.Id == ClockElementId.HourMarker(0));

        Assert.Equal(ClockHandKind.Second, secondHand.Hand);
        Assert.True(secondHand.RedrawPerFrame);
        Assert.Equal(NerdThemeGeometry.SecondHandContentSize, secondHand.ContentSize);
        Assert.Equal(NerdThemeGeometry.SecondHandPivot, secondHand.Pivot);
        Assert.Equal(5, NerdThemeGeometry.HourBitCount);
        Assert.Equal(6, NerdThemeGeometry.MinuteBitCount);

        float bladeTipReachFromCenter = secondHand.Pivot.Y - NerdThemeGeometry.TipInset;
        float markerInnerEdgeFromCenter = (500f * 0.78f) - topMarker.Pivot.Y;

        Assert.True(
            bladeTipReachFromCenter <= markerInnerEdgeFromCenter - 10f,
            $"Expected at least 10 design units of clearance, but tip reach was {bladeTipReachFromCenter} and marker inner edge was {markerInnerEdgeFromCenter}.");
    }
}
