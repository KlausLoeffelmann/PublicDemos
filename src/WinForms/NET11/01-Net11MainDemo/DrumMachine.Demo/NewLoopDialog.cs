namespace DrumMachine.Demo;

/// <summary>
///  Asks for the musical loop length independently of the one-/two-bar view setting.
/// </summary>
internal partial class NewLoopDialog : Form
{
    /// <summary>
    ///  Creates a Designer-compatible blank-loop dialog.
    /// </summary>
    public NewLoopDialog()
    {
        InitializeComponent();
        _bars.SelectedIndex = 1;
    }

    /// <summary>
    ///  Gets the chosen length in bars.
    /// </summary>
    public int BarCount => _bars.SelectedIndex switch
    {
        0 => 1, 1 => 2, 2 => 4,
        _ => throw new InvalidOperationException("Select a supported loop length.")
    };
}
