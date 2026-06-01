using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;

namespace Winget_Package_Editor;

internal partial class AddAppDialog : Form
{
    public AddAppDialog(IReadOnlyList<AppEntry> wellKnownApps)
    {
        ArgumentNullException.ThrowIfNull(wellKnownApps);
        InitializeComponent();

        _appComboBox.DisplayMember = nameof(AppEntry.DisplayName);
        _appComboBox.DataSource = wellKnownApps.ToList();
        _actionComboBox.DataSource = Enum.GetValues<AppAction>();
        _sourceComboBox.DataSource = Enum.GetValues<AppSource>();
        _scopeComboBox.DataSource = Enum.GetValues<AppScope>();

        if (wellKnownApps.Count > 0)
        {
            _appComboBox.SelectedIndex = 0;
        }
    }

    public AppEntry? Result { get; private set; }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (_appComboBox.SelectedItem is not AppEntry template)
        {
            MessageBox.Show(this, "Please select an app.", "Add App",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.None;
            return;
        }

        // Clone the template so the catalog entry is never mutated.
        AppEntry configured = WingetPackage.Clone(new WingetPackage { Apps = [template] }).Apps[0];
        configured.Action = (AppAction)_actionComboBox.SelectedItem!;
        configured.Source = (AppSource)_sourceComboBox.SelectedItem!;
        configured.Scope = (AppScope)_scopeComboBox.SelectedItem!;
        configured.AllowPrerelease = _allowPrereleaseCheckBox.Checked;
        configured.Version = string.IsNullOrWhiteSpace(_versionTextBox.Text) ? null : _versionTextBox.Text.Trim();

        Result = configured;
    }
}
