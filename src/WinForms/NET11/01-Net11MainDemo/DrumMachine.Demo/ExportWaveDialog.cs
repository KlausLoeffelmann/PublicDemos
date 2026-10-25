using WinForms.Audio.Sequencing;

namespace DrumMachine.Demo;

/// <summary>
///  Chooses a current bar, a full loop, or repeated loops for an offline WAV export.
/// </summary>
internal partial class ExportWaveDialog : Form
{
    private readonly int _currentBarIndex;
    private readonly int _scoreBars;
    private readonly int _tempoBpm;

    /// <summary>
    ///  Creates a Designer-compatible dialog with a short example loop.
    /// </summary>
    public ExportWaveDialog()
        : this(currentBar: 0, barCount: 2, tempoBpm: 92)
    {
    }

    /// <summary>
    ///  Creates an export dialog for the loop that is open when the command starts.
    /// </summary>
    /// <param name="currentBar">The zero-based bar selected in the editor.</param>
    /// <param name="barCount">The number of bars in the snapshot being exported.</param>
    /// <param name="tempoBpm">The snapshot tempo in quarter notes per minute.</param>
    internal ExportWaveDialog(int currentBar, int barCount, int tempoBpm)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(barCount);
        ArgumentOutOfRangeException.ThrowIfNegative(currentBar);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(currentBar, barCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tempoBpm);
        _currentBarIndex = currentBar;
        _scoreBars = barCount;
        _tempoBpm = tempoBpm;
        InitializeComponent();
        _repetitions.Maximum = PercussionWaveExporter.MaximumRepetitions;
        _currentBar.Text = $"Current &bar (bar {currentBar + 1} of {barCount})";
        _currentBar.AccessibleName = $"Current bar, bar {currentBar + 1} of {barCount}";
        _fullLoop.Text = $"&Full loop ({barCount} {(barCount == 1 ? "bar" : "bars")})";
        _fullLoop.AccessibleName = $"Full loop, {barCount} bars";
        RefreshEstimate();
    }

    /// <summary>
    ///  Gets the selected export range.
    /// </summary>
    public WaveExportScope Scope
        => _currentBar.Checked ? WaveExportScope.CurrentBar
            : _fullLoop.Checked ? WaveExportScope.FullLoop
            : _repeated.Checked ? WaveExportScope.RepeatedLoop
            : throw new InvalidOperationException("Select an export scope.");

    /// <summary>
    ///  Gets the zero-based first bar. A full or repeated loop starts at the beginning.
    /// </summary>
    public int FirstBar
        => Scope == WaveExportScope.CurrentBar ? _currentBarIndex : 0;

    /// <summary>
    ///  Gets one bar for the current-bar scope, or <see langword="null"/> for every remaining bar.
    /// </summary>
    public int? BarCount
        => Scope == WaveExportScope.CurrentBar ? 1 : null;

    /// <summary>
    ///  Gets how many times to render the selected range. Only a repeated loop uses the numeric value.
    /// </summary>
    public int Repetitions
        => Scope == WaveExportScope.RepeatedLoop
            ? Math.Clamp((int)_repetitions.Value, 1, PercussionWaveExporter.MaximumRepetitions)
            : 1;

    /// <summary>
    ///  Gets whether to append the natural decay after the musical end.
    /// </summary>
    public bool IncludeTail
        => _includeTail.Checked;

    /// <summary>
    ///  Gets the musical length of the current selection, excluding any final sound tail.
    /// </summary>
    public decimal MusicalSeconds
        => GetMusicalSeconds(_tempoBpm, ResolvedBars, Repetitions);

    /// <summary>
    ///  Gets whether the musical length is over the exporter's one-hour bound.
    /// </summary>
    public bool ExceedsMusicalLimit
        => MusicalLimitExceeded(_tempoBpm, ResolvedBars, Repetitions);

    /// <summary>
    ///  Calculates musical seconds the same way the exporter counts frames, independent of sample rate.
    /// </summary>
    /// <param name="tempoBpm">Quarter notes per minute.</param>
    /// <param name="bars">Contiguous bars in one pass.</param>
    /// <param name="repetitions">How many times that range is rendered.</param>
    /// <returns>The exact musical duration in seconds, without a final tail.</returns>
    public static decimal GetMusicalSeconds(int tempoBpm, int bars, int repetitions)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tempoBpm);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bars);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(repetitions);
        return 15m * PercussionScore.StepsPerBar * bars * repetitions / tempoBpm;
    }

    /// <summary>
    ///  Tests the musical length against <see cref="PercussionWaveExporter.MaximumDurationSeconds"/>.
    /// </summary>
    /// <param name="tempoBpm">Quarter notes per minute.</param>
    /// <param name="bars">Contiguous bars in one pass.</param>
    /// <param name="repetitions">How many times that range is rendered.</param>
    /// <returns><see langword="true"/> when the musical length is over one hour.</returns>
    public static bool MusicalLimitExceeded(int tempoBpm, int bars, int repetitions)
        => GetMusicalSeconds(tempoBpm, bars, repetitions) > PercussionWaveExporter.MaximumDurationSeconds;

    /// <summary>
    ///  Formats a musical duration for the dialog and the status line.
    /// </summary>
    /// <param name="tempoBpm">Quarter notes per minute.</param>
    /// <param name="bars">Contiguous bars in one pass.</param>
    /// <param name="repetitions">How many times that range is rendered.</param>
    /// <returns>A short hours, minutes, and seconds description.</returns>
    public static string FormatMusicalDuration(int tempoBpm, int bars, int repetitions)
        => FormatSeconds(GetMusicalSeconds(tempoBpm, bars, repetitions));

    /// <summary>
    ///  Widens the wrapping labels after DPI scaling. <see cref="Control.MaximumSize"/> is not scaled.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        int wrap = Math.Max(280, LogicalToDeviceUnits(440));
        _explanation.MaximumSize = new Size(wrap, 0);
        _duration.MaximumSize = new Size(wrap, 0);
    }

    private int ResolvedBars
        => Scope == WaveExportScope.CurrentBar ? 1 : _scoreBars;

    private void Scope_CheckedChanged(object? sender, EventArgs e)
    {
        bool repeated = _repeated.Checked;
        _repetitions.Enabled = repeated;
        _repetitionsCaption.Enabled = repeated;
        RefreshEstimate();
    }

    private void Repetitions_ValueChanged(object? sender, EventArgs e)
        => RefreshEstimate();

    private void Export_Click(object? sender, EventArgs e)
    {
        if (!_currentBar.Checked && !_fullLoop.Checked && !_repeated.Checked)
        {
            DialogResult = DialogResult.None;
            RefreshEstimate();
            return;
        }

        if (ExceedsMusicalLimit)
        {
            DialogResult = DialogResult.None;
            RefreshEstimate();
            return;
        }

        DialogResult = DialogResult.OK;
    }

    private void RefreshEstimate()
    {
        if (!_currentBar.Checked && !_fullLoop.Checked && !_repeated.Checked)
        {
            _duration.Text = "Select what to export.";
            _export.Enabled = false;
            return;
        }

        int bars = ResolvedBars;
        int repetitions = _repeated.Checked
            ? Math.Clamp((int)_repetitions.Value, 1, PercussionWaveExporter.MaximumRepetitions)
            : 1;
        bool tooLong = MusicalLimitExceeded(_tempoBpm, bars, repetitions);
        string length = FormatMusicalDuration(_tempoBpm, bars, repetitions);
        string range = repetitions == 1
            ? $"{bars} {(bars == 1 ? "bar" : "bars")} at {_tempoBpm} BPM"
            : $"{bars} {(bars == 1 ? "bar" : "bars")} x {repetitions} at {_tempoBpm} BPM";
        _duration.Text = tooLong
            ? $"Estimated musical duration: {length} ({range}). That is over 1 hour, so export is unavailable."
            : $"Estimated musical duration: {length} ({range}). This does not include a final sound tail.";
        _export.Enabled = !tooLong;
    }

    private static string FormatSeconds(decimal seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }

        long tenths = (long)decimal.Round(seconds * 10m, 0, MidpointRounding.AwayFromZero);
        long whole = tenths / 10;
        int tenth = (int)(tenths % 10);
        int hours = (int)(whole / 3600);
        int minutes = (int)(whole % 3600 / 60);
        int secs = (int)(whole % 60);
        if (hours > 0)
        {
            return $"{hours} h {minutes:00} min {secs:00}.{tenth} s";
        }

        return minutes > 0
            ? $"{minutes} min {secs:00}.{tenth} s"
            : $"{secs}.{tenth} s";
    }
}

/// <summary>
///  The range selected in <see cref="ExportWaveDialog"/>.
/// </summary>
internal enum WaveExportScope
{
    /// <summary>
    ///  The editor's current bar, once.
    /// </summary>
    CurrentBar,

    /// <summary>
    ///  Every bar of the loop, once.
    /// </summary>
    FullLoop,

    /// <summary>
    ///  Every bar of the loop, repeated continuously.
    /// </summary>
    RepeatedLoop
}
