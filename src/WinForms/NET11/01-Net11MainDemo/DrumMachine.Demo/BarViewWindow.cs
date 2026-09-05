using SplitFlap.Audio.Sequencing;

namespace DrumMachine.Demo;

/// <summary>
///  Maps the visible score page to musical bars without modifying the underlying loop.
/// </summary>
internal readonly record struct BarViewWindow
{
    /// <summary>
    ///  Creates a one- or two-bar viewport over a valid score.
    /// </summary>
    public BarViewWindow(int totalBars, int barsPerView, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalBars);
        if (barsPerView is not (1 or 2))
        {
            throw new ArgumentOutOfRangeException(nameof(barsPerView));
        }

        int pages = (totalBars + barsPerView - 1) / barsPerView;
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(page, pages);
        TotalBars = totalBars;
        BarsPerView = barsPerView;
        FirstBar = page * barsPerView;
    }

    /// <summary>
    ///  Gets the score length, including bars outside this view.
    /// </summary>
    public int TotalBars { get; }

    /// <summary>
    ///  Gets the number of displayed bars, including disabled padding at the end.
    /// </summary>
    public int BarsPerView { get; }

    /// <summary>
    ///  Gets the zero-based first displayed bar.
    /// </summary>
    public int FirstBar { get; }

    /// <summary>
    ///  Resolves a displayed step, returning false for the absent second bar of a one-bar loop.
    /// </summary>
    public bool TryGetPosition(int displayedStep, out int bar, out int step)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(displayedStep);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(displayedStep, BarsPerView * PercussionScore.StepsPerBar);
        bar = FirstBar + displayedStep / PercussionScore.StepsPerBar;
        step = displayedStep % PercussionScore.StepsPerBar;
        return bar < TotalBars;
    }

    /// <summary>
    ///  Finds a musical position in this view, or returns minus one when it is not visible.
    /// </summary>
    public int GetDisplayedStep(int bar, int step)
        => bar >= FirstBar && bar < Math.Min(TotalBars, FirstBar + BarsPerView)
            && step >= 0 && step < PercussionScore.StepsPerBar
            ? (bar - FirstBar) * PercussionScore.StepsPerBar + step : -1;

    /// <inheritdoc/>
    public override string ToString()
        => BarsPerView == 1 || FirstBar + 1 >= TotalBars
            ? $"Bar {FirstBar + 1}"
            : $"Bars {FirstBar + 1}-{FirstBar + 2}";
}
