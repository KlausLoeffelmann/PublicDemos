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
        _fontSize.SelectedIndex = settings.FontSize switch
        {
            AppFontSize.Normal => 1,
            AppFontSize.Large => 2,
            AppFontSize.Xxl => 3,
            _ => 0
        };
        _folder.Text = settings.DefaultFolder;
        _recordingsFolder.Text = settings.RecordingsFolder;
        _presetFolder.Text = settings.SoundPresetFolder;
        _importFolder.Text = settings.LastWavImportFolder;
        Result = settings;
    }

    /// <summary>
    ///  Gets the validated selection after the dialog returns OK.
    /// </summary>
    public AppSettings Result { get; private set; }

    private void OptionsDialog_Disposed(object? sender, EventArgs e) => _folderPicker.Dispose();

    private void Browse_Click(object? sender, EventArgs e)
        => BrowseInto(_folder, "Default folder for percussion loop files");

    private void BrowseRecordings_Click(object? sender, EventArgs e)
        => BrowseInto(_recordingsFolder, "Default folder for recordings");

    private void BrowsePresetFolder_Click(object? sender, EventArgs e)
        => BrowseInto(_presetFolder, "Default folder for portable sound presets");

    private void BrowseImportFolder_Click(object? sender, EventArgs e)
        => BrowseInto(_importFolder, "Folder for the next WAV import");

    private void BrowseInto(TextBox target, string description)
    {
        _folderPicker.Description = description;
        _folderPicker.SelectedPath = target.Text;
        if (_folderPicker.ShowDialog(this) == DialogResult.OK)
        {
            target.Text = _folderPicker.SelectedPath;
        }
    }

    private void Ok_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!TryExistingFolder(_folder, "loop files", out string loopFolder)
                || !TryExistingFolder(_recordingsFolder, "recordings", out string recordingsFolder)
                || !TryExistingFolder(_presetFolder, "sound presets", out string presetFolder)
                || !TryExistingFolder(_importFolder, "WAV imports", out string importFolder))
            {
                return;
            }

            Result = _original with
            {
                Theme = _theme.SelectedIndex switch { 0 => AppTheme.Classic, 1 => AppTheme.Dark, _ => AppTheme.System },
                IconSize = _icons.SelectedIndex switch { 1 => ToolbarIconSize.Medium, 2 => ToolbarIconSize.Large, _ => ToolbarIconSize.Small },
                FontSize = _fontSize.SelectedIndex switch
                {
                    1 => AppFontSize.Normal,
                    2 => AppFontSize.Large,
                    3 => AppFontSize.Xxl,
                    _ => AppFontSize.Small
                },
                DefaultFolder = loopFolder,
                RecordingsFolder = recordingsFolder,
                SoundPresetFolder = presetFolder,
                LastWavImportFolder = importFolder
            };
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or IOException or UnauthorizedAccessException)
        {
            _error.Text = $"The folder is not usable: {ex.Message}";
        }
    }

    private bool TryExistingFolder(TextBox input, string purpose, out string path)
    {
        string enteredPath = input.Text.Trim();
        if (!Path.IsPathFullyQualified(enteredPath))
        {
            _error.Text = $"Enter a full folder path for {purpose}, or choose Browse.";
            path = "";
            return false;
        }

        path = Path.GetFullPath(enteredPath);
        if (Directory.Exists(path))
        {
            return true;
        }

        _error.Text = $"Choose an existing folder for {purpose}.";
        path = "";
        return false;
    }
}
