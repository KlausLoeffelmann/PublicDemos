namespace SplitFlap.Demo;

/// <summary>
///  Edits application-wide timetable behavior.
/// </summary>
internal partial class OptionsDialog : Form
{
    /// <summary>
    ///  Initializes the dialog with the current timetable update interval.
    /// </summary>
    public OptionsDialog(int updateIntervalSeconds)
    {
        InitializeComponent();
        _updateIntervalTrackBar.Value = Math.Clamp(
            UpdateInterval.Normalize(updateIntervalSeconds) / UpdateInterval.StepSeconds,
            _updateIntervalTrackBar.Minimum,
            _updateIntervalTrackBar.Maximum);
        UpdateIntervalLabel();
    }

    /// <summary>
    ///  Gets the selected timetable update interval in seconds.
    /// </summary>
    public int UpdateIntervalSeconds
        => _updateIntervalTrackBar.Value * UpdateInterval.StepSeconds;

    private void UpdateIntervalTrackBar_ValueChanged(object? sender, EventArgs e)
        => UpdateIntervalLabel();

    private void UpdateIntervalLabel()
        => _currentIntervalLabel.Text = $"{UpdateIntervalSeconds} seconds";
}
