using WingetPackageEditor.Core.Models;

namespace Winget_Package_Editor;

internal partial class NewFromExistingDialog : Form
{
    public NewFromExistingDialog(IReadOnlyList<WingetPackage> existingPackages)
    {
        ArgumentNullException.ThrowIfNull(existingPackages);
        InitializeComponent();

        _sourceComboBox.DisplayMember = nameof(WingetPackage.Name);
        _sourceComboBox.DataSource = existingPackages.ToList();
        if (existingPackages.Count > 0)
        {
            _sourceComboBox.SelectedIndex = 0;
        }
    }

    public string NewName => _nameTextBox.Text.Trim();

    public WingetPackage? SourcePackage => _sourceComboBox.SelectedItem as WingetPackage;

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewName))
        {
            MessageBox.Show(this, "Please enter a name for the new package.", "New from existing package",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.None;
            return;
        }

        if (SourcePackage is null)
        {
            MessageBox.Show(this, "Please select a source package to copy.", "New from existing package",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.None;
        }
    }
}
