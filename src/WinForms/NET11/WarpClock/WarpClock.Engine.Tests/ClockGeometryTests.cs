using System.Drawing;

namespace WarpClock.Engine.Tests;

public sealed class ClockGeometryTests
{
    [Fact]
    public void ForSurface_WithIdentityTransform_UsesFullHostSurface()
    {
        SizeF hostSurface = new(800f, 600f);

        ClockGeometry geometry = ClockGeometry.ForSurface(hostSurface);

        Assert.Equal(hostSurface.Width, geometry.Surface.Width, 3);
        Assert.Equal(hostSurface.Height, geometry.Surface.Height, 3);
        Assert.Equal(0f, geometry.Origin.X, 3);
        Assert.Equal(0f, geometry.Origin.Y, 3);
        Assert.Equal(400f, geometry.Center.X, 3);
        Assert.Equal(300f, geometry.Center.Y, 3);
        Assert.Equal(300f, geometry.Radius, 3);
        Assert.Equal(800f, geometry.Bounds.Width, 3);
        Assert.Equal(600f, geometry.Bounds.Height, 3);
    }

    [Fact]
    public void ForSurface_WithOledTransform_ScalesAndOffsetsSceneWithinHost()
    {
        ClockGeometry geometry = ClockGeometry.ForSurface(
            new SizeF(800f, 600f),
            new OledSceneTransform(0.97f, new Point(7, -5)));

        Assert.Equal(776f, geometry.Surface.Width, 3);
        Assert.Equal(582f, geometry.Surface.Height, 3);
        Assert.Equal(19f, geometry.Origin.X, 3);
        Assert.Equal(4f, geometry.Origin.Y, 3);
        Assert.Equal(407f, geometry.Center.X, 3);
        Assert.Equal(295f, geometry.Center.Y, 3);
        Assert.Equal(291f, geometry.Radius, 3);
        Assert.Equal(19f, geometry.Bounds.Left, 3);
        Assert.Equal(4f, geometry.Bounds.Top, 3);
        Assert.Equal(795f, geometry.Bounds.Right, 3);
        Assert.Equal(586f, geometry.Bounds.Bottom, 3);
    }
}
