using WarpToolkit.ComponentModel;
using WinBaas.Models;

namespace WinBaas.Dialogs;

/// <summary>
///  Lets the user configure WinBaas options (SQL discovery, roaming catalog
///  path, default backup mode). OK persists immediately through the
///  <see cref="IUserSettingsService"/>.
/// </summary>
public sealed class DlgOptions : Form
{
    /// <summary>Settings key controlling whether SQL discovery is enabled.</summary>
    public const string KeySqlDiscovery = "WinBaas.SqlDiscoveryEnabled";

    /// <summary>Settings key for the roaming catalog path override.</summary>
    public const string KeyRoamingCatalogPath = "WinBaas.RoamingCatalogPath";

    /// <summary>Settings key for the default backup mode.</summary>
    public const string KeyBackupMode = "WinBaas.BackupMode";

    /// <summary>
    ///  Settings key controlling whether the tree automatically expands the
    ///  affected (non-empty) nodes after a scan and collapses the rest.
    /// </summary>
    public const string KeyExpandAffectedAfterScan = "WinBaas.ExpandAffectedAfterScan";

    private readonly IUserSettingsService _settings;
    private readonly CheckBox _sqlCheck;
    private readonly TextBox _roamingPathBox;
    private readonly ComboBox _backupModeCombo;
    private readonly CheckBox _expandAffectedCheck;

    public DlgOptions(IUserSettingsService settings)
    {
        _settings = settings;
        Font = new Font("Segoe UI", 11F);
        Text = "WinBaas - Options";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ClientSize = new Size(520, 284);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(12),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "SQL discovery:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
        _sqlCheck = new CheckBox
        {
            Text = "Enable LocalDB / SQL Express discovery",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Checked = settings.Get(KeySqlDiscovery, true),
        };
        layout.Controls.Add(_sqlCheck, 1, 0);

        layout.Controls.Add(new Label { Text = "Roaming catalog path:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
        _roamingPathBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = settings.Get(KeyRoamingCatalogPath, string.Empty),
            PlaceholderText = "(leave empty to use the default user-settings store)",
        };
        layout.Controls.Add(_roamingPathBox, 1, 1);

        layout.Controls.Add(new Label { Text = "Default backup mode:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
        _backupModeCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _backupModeCombo.Items.AddRange([BackupMode.CopyToFolder, BackupMode.ZipArchive]);
        _backupModeCombo.SelectedItem = settings.Get(KeyBackupMode, BackupMode.CopyToFolder);
        layout.Controls.Add(_backupModeCombo, 1, 2);

        layout.Controls.Add(new Label { Text = "After a scan:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
        _expandAffectedCheck = new CheckBox
        {
            Text = "Expand affected TreeView nodes (collapse the rest)",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Checked = settings.Get(KeyExpandAffectedAfterScan, true),
        };
        layout.Controls.Add(_expandAffectedCheck, 1, 3);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(12),
        };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 100 };
        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 100 };
        ok.Click += Ok_Click;
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(ok);

        Controls.Add(layout);
        Controls.Add(buttons);
        AcceptButton = ok;
        CancelButton = cancel;
    }

    private void Ok_Click(object? sender, EventArgs e)
    {
        _settings.Set(KeySqlDiscovery, _sqlCheck.Checked);
        _settings.Set(KeyRoamingCatalogPath, _roamingPathBox.Text.Trim());
        _settings.Set(KeyBackupMode, (BackupMode)(_backupModeCombo.SelectedItem ?? BackupMode.CopyToFolder));
        _settings.Set(KeyExpandAffectedAfterScan, _expandAffectedCheck.Checked);
        _settings.Flush();
    }
}
