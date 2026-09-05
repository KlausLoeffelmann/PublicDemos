namespace DrumMachine.Demo;

/// <summary>
///  Edits application preferences without changing the musical document.
/// </summary>
internal partial class OptionsDialog : Form
{
    private readonly AppSettings _original;

    /// <summary>
    ///  Creates an options dialog without reading or writing settings in its constructor.
    /// </summary>
    public OptionsDialog() : this(new AppSettings())
    {
    }

    /// <summary>
    ///  Displays a copy of the current application preferences.
    /// </summary>
    internal OptionsDialog(AppSettings settings)
    {
        _original = settings;
        InitializeComponent();
        _theme.SelectedIndex = settings.Theme switch { AppTheme.Classic => 0, AppTheme.Dark => 1, _ => 2 };
        _icons.SelectedIndex = settings.IconSize switch { ToolbarIconSize.Medium => 1, ToolbarIconSize.Large => 2, _ => 0 };
        _folder.Text = settings.DefaultFolder;
        Result = settings;
    }

    /// <summary>
    ///  Gets the validated selection after the dialog returns OK.
    /// </summary>
    public AppSettings Result { get; private set; }

    private void OptionsDialog_Disposed(object? sender, EventArgs e) => _folderPicker.Dispose();

    private void Browse_Click(object? sender, EventArgs e)
    {
        _folderPicker.SelectedPath = _folder.Text;
        if (_folderPicker.ShowDialog(this) == DialogResult.OK)
        {
            _folder.Text = _folderPicker.SelectedPath;
        }
    }

    private void Ok_Click(object? sender, EventArgs e)
    {
        try
        {
            string enteredPath = _folder.Text.Trim();
            if (!Path.IsPathFullyQualified(enteredPath))
            {
                _error.Text = "Enter a full folder path or choose Browse.";
                return;
            }

            string path = Path.GetFullPath(enteredPath);
            if (!Directory.Exists(path))
            {
                _error.Text = "Choose an existing folder for loop files.";
                return;
            }

            Result = _original with
            {
                Theme = _theme.SelectedIndex switch { 0 => AppTheme.Classic, 1 => AppTheme.Dark, _ => AppTheme.System },
                IconSize = _icons.SelectedIndex switch { 1 => ToolbarIconSize.Medium, 2 => ToolbarIconSize.Large, _ => ToolbarIconSize.Small },
                DefaultFolder = path
            };
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or IOException or UnauthorizedAccessException)
        {
            _error.Text = $"The folder is not usable: {ex.Message}";
        }
    }
}
