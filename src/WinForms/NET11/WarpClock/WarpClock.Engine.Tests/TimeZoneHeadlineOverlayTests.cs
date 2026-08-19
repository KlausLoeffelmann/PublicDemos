using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine.Tests;

public sealed class TimeZoneHeadlineOverlayTests
{
    [Fact]
    public void ShouldRender_RequiresEnabledNonEmptyTextAndNoThemeTimeZoneVisual()
    {
        IReadOnlyList<ClockElementDescriptor> withoutTimeZone =
        [
            new ClockElementDescriptor
            {
                Id = ClockElementId.Face,
                ContentSize = new SizeF(100f, 100f),
                Pivot = new PointF(50f, 50f),
            },
        ];

        IReadOnlyList<ClockElementDescriptor> withTimeZone =
        [
            new ClockElementDescriptor
            {
                Id = ClockElementId.TimeZone,
                ContentSize = new SizeF(100f, 20f),
                Pivot = new PointF(50f, 10f),
            },
        ];

        Assert.False(TimeZoneHeadlineOverlay.ShouldRender(false, "UTC", withoutTimeZone));
        Assert.False(TimeZoneHeadlineOverlay.ShouldRender(true, "", withoutTimeZone));
        Assert.False(TimeZoneHeadlineOverlay.ShouldRender(true, "UTC", withTimeZone));
        Assert.True(TimeZoneHeadlineOverlay.ShouldRender(true, "UTC", withoutTimeZone));
    }

    [Fact]
    public void GetBaseColor_UsesSubduedDarkBlueAtNight()
    {
        Color night = TimeZoneHeadlineOverlay.GetBaseColor(true);
        Color day = TimeZoneHeadlineOverlay.GetBaseColor(false);

        Assert.Equal(Color.FromArgb(48, 66, 104).ToArgb(), night.ToArgb());
        Assert.NotEqual(day.ToArgb(), night.ToArgb());
        Assert.True(night.B >= night.R);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(1f)]
    [InlineData(5f)]
    public void ComputeFade_StaysWithinReadableBounds(float elapsedSeconds)
    {
        float fade = TimeZoneHeadlineOverlay.ComputeFade(elapsedSeconds);

        Assert.InRange(fade, 0.42f, 0.74f);
    }
}
