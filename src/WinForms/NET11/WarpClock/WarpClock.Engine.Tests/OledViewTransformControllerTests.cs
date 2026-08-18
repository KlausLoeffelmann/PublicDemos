using System.Drawing;

namespace WarpClock.Engine.Tests;

public sealed class OledViewTransformControllerTests
{
    [Fact]
    public void SampleGeneralTransform_ProducesDeterministicOffsetsAndScale()
    {
        SizeF hostSurface = new(800f, 600f);

        Assert.Equal(OledSceneTransform.Identity, OledViewTransformController.SampleGeneralTransform(hostSurface, TimeSpan.Zero));

        OledSceneTransform quarterSweep = OledViewTransformController.SampleGeneralTransform(hostSurface, TimeSpan.FromMinutes(4.5));
        Assert.Equal(new Point(7, -5), quarterSweep.Offset);
        Assert.Equal(0.97f, quarterSweep.Scale, 3);

        OledSceneTransform halfHorizontalSweep = OledViewTransformController.SampleGeneralTransform(hostSurface, TimeSpan.FromMinutes(9));
        Assert.Equal(new Point(0, -4), halfHorizontalSweep.Offset);
        Assert.Equal(0.985f, halfHorizontalSweep.Scale, 3);
    }

    [Fact]
    public void SampleGeneralTransform_StaysWithinHostBounds_WhenAppliedToGeometry()
    {
        SizeF hostSurface = new(1920f, 1080f);

        for (int minutes = 0; minutes <= 72; minutes += 3)
        {
            OledSceneTransform transform = OledViewTransformController.SampleGeneralTransform(hostSurface, TimeSpan.FromMinutes(minutes));
            ClockGeometry geometry = ClockGeometry.ForSurface(hostSurface, transform);

            Assert.InRange(transform.Scale, 0.90f, 1f);
            Assert.InRange(geometry.Bounds.Left, 0f, hostSurface.Width);
            Assert.InRange(geometry.Bounds.Top, 0f, hostSurface.Height);
            Assert.InRange(geometry.Bounds.Right, 0f, hostSurface.Width);
            Assert.InRange(geometry.Bounds.Bottom, 0f, hostSurface.Height);
        }
    }

    [Fact]
    public void Advance_BlendsBackToIdentity_WhenDisabled()
    {
        var controller = new OledViewTransformController();
        SizeF hostSurface = new(800f, 600f);

        OledSceneTransform active = controller.Advance(TimeSpan.FromMinutes(6), hostSurface, OledViewMode.General);
        Assert.Equal(new Point(5, -7), active.Offset);
        Assert.Equal(0.97f, active.Scale, 3);

        OledSceneTransform fading = controller.Advance(TimeSpan.FromSeconds(0.75), hostSurface, OledViewMode.Off);
        Assert.True(fading.Scale > active.Scale);
        Assert.True(Math.Abs(fading.Offset.X) <= Math.Abs(active.Offset.X));
        Assert.True(Math.Abs(fading.Offset.Y) <= Math.Abs(active.Offset.Y));

        OledSceneTransform off = controller.Advance(TimeSpan.FromSeconds(2), hostSurface, OledViewMode.Off);
        Assert.Equal(OledSceneTransform.Identity, off);
    }
}
