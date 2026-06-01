namespace Winget_Package_Editor;

internal partial class OptionsDialog : Form
{
    private readonly UiFontSettings _settings;

    public OptionsDialog(UiFontSettings settings)
    {
        _settings = settings.Clone();
        InitializeComponent();
        LoadSettings();
    }

    public UiFontSettings Settings => _settings.Clone();

    private void LoadSettings()
    {
        _fontFamilyTextBox.Text = _settings.FontFamily;
        _menuFontSizeUpDown.Value = (decimal)_settings.MenuStripSize;
        _standardFontSizeUpDown.Value = (decimal)_settings.StandardSize;
        _treeDeltaUpDown.Value = (decimal)_settings.TreeMainNodeDelta;
        _statusFontSizeUpDown.Value = (decimal)_settings.StatusStripSize;
        _treeBoldCheckBox.Checked = _settings.TreeMainNodeBold;
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        _settings.FontFamily = string.IsNullOrWhiteSpace(_fontFamilyTextBox.Text)
            ? "Segoe UI"
            : _fontFamilyTextBox.Text.Trim();
        _settings.MenuStripSize = (float)_menuFontSizeUpDown.Value;
        _settings.StandardSize = (float)_standardFontSizeUpDown.Value;
        _settings.TreeMainNodeDelta = (float)_treeDeltaUpDown.Value;
        _settings.StatusStripSize = (float)_statusFontSizeUpDown.Value;
        _settings.TreeMainNodeBold = _treeBoldCheckBox.Checked;
    }
}
