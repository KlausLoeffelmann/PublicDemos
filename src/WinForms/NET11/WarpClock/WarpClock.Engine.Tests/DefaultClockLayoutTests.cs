using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine.Tests;

public sealed class DefaultClockLayoutTests
{
    [Fact]
    public void ResolveAnchor_PlacesAuxiliaryElementsAtStableDefaultLocations()
    {
        ClockGeometry geometry = ClockGeometry.ForSurface(new SizeF(1000f, 1000f));

        PointF center = geometry.Center;
        PointF timeZone = DefaultClockLayout.ResolveAnchor(ClockElementId.TimeZone, geometry);
        PointF day = DefaultClockLayout.ResolveAnchor(ClockElementId.Day, geometry);
        PointF weekday = DefaultClockLayout.ResolveAnchor(ClockElementId.Weekday, geometry);
        PointF overlay = DefaultClockLayout.ResolveAnchor(ClockElementId.OverlayMessage, geometry);
        PointF fractionDial = DefaultClockLayout.ResolveAnchor(ClockElementId.FractionSecondDial, geometry);
        PointF indexed0 = DefaultClockLayout.ResolveAnchor(ClockElementId.IndexedImage(0), geometry);
        PointF indexed1 = DefaultClockLayout.ResolveAnchor(ClockElementId.IndexedImage(1), geometry);
        PointF subSecondHand = DefaultClockLayout.ResolveAnchor(new ClockElementId(ClockElementKind.SubSecondHand), geometry);

        Assert.True(timeZone.Y < center.Y);
        Assert.True(day.X > center.X);
        Assert.True(weekday.X < center.X);
        Assert.True(overlay.Y > center.Y);
        Assert.Equal(subSecondHand.X, fractionDial.X, 3);
        Assert.Equal(subSecondHand.Y, fractionDial.Y, 3);
        Assert.NotEqual(indexed0, indexed1);
    }

    [Fact]
    public void AuxiliaryVisibility_GatesFractionSecondDialAndSubSecondHandTogether()
    {
        ClockAuxiliaryVisibility visibility = ClockAuxiliaryVisibility.Default with
        {
            ShowFractionSecond = false,
        };

        Assert.False(WarpClockControl.IsAuxiliaryVisible(ClockElementKind.FractionSecondDial, visibility));
        Assert.False(WarpClockControl.IsAuxiliaryVisible(ClockElementKind.SubSecondHand, visibility));
        Assert.True(WarpClockControl.IsAuxiliaryVisible(ClockElementKind.TimeZone, visibility));
    }
}
