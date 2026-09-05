using DrumMachine.Demo;

namespace SplitFlap.Tests;

public sealed class BarViewWindowTests
{
    [Theory]
    [InlineData(4, 1, 3, 7, 3, 7)]
    [InlineData(4, 2, 1, 0, 2, 0)]
    [InlineData(4, 2, 1, 31, 3, 15)]
    [InlineData(2, 2, 0, 17, 1, 1)]
    public void DisplayedSteps_MapToAbsoluteBars(int bars, int width, int page, int displayed, int expectedBar, int expectedStep)
    {
        BarViewWindow view = new(bars, width, page);
        Assert.True(view.TryGetPosition(displayed, out int bar, out int step));
        Assert.Equal(expectedBar, bar);
        Assert.Equal(expectedStep, step);
        Assert.Equal(displayed, view.GetDisplayedStep(bar, step));
    }

    [Fact]
    public void ViewingTwoBars_DoesNotInventAMissingBar()
    {
        BarViewWindow view = new(1, 2, 0);
        Assert.True(view.TryGetPosition(15, out _, out _));
        Assert.False(view.TryGetPosition(16, out _, out _));
        Assert.Equal(-1, view.GetDisplayedStep(1, 0));
    }

    [Fact]
    public void InvalidView_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BarViewWindow(4, 3, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new BarViewWindow(4, 2, 2));
    }
}
