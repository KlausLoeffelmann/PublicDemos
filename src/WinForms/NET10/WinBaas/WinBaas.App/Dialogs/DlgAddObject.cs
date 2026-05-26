using WinBaas.Models;

namespace WinBaas.Dialogs;

/// <summary>
///  Lets the user define a folder/file/env-var/SQL-Server catalog entry.
/// </summary>
public sealed class DlgAddObject : Form
{
    private static readonly string[] s_categories =
    [
        "User",
        "AI Tools",
        "Developer Tools",
        "Creator / Design / Photo",
        "Musician / Audio",
        "System",
    ];

    private readonly ComboBox _categoryCombo;
    private readonly ComboBox _kindCombo;
    private readonly TextBox _nameBox;
    private readonly TextBox _pathBox;
    private readonly TextBox _extensionsBox;
    private readonly CheckBox _recursiveCheck;
    private readonly Button _browseButton;
    private readonly Button _okButton;
    private readonly Button _cancelButton;

    /// <summary>The entry the user defined, populated on <see cref="DialogResult.OK"/>.</summary>
    public CatalogEntry? Result { get; private set; }

    public DlgAddObject()
    {
        Font = new Font("Segoe UI", 11F);
        Text = "WinBaas - Add object";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ClientSize = new Size(560, 400);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 7,
            Padding = new Padding(12),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        for (int r = 0; r < 6; r++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        }
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Category:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
        _categoryCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _categoryCombo.Items.AddRange([.. s_categories]);
        _categoryCombo.SelectedIndex = 0;
        layout.Controls.Add(_categoryCombo, 1, 0);
        layout.SetColumnSpan(_categoryCombo, 2);

        layout.Controls.Add(new Label { Text = "Kind:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
        _kindCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _kindCombo.Items.AddRange([CatalogEntryKind.Folder, CatalogEntryKind.File, CatalogEntryKind.EnvironmentVariable, CatalogEntryKind.SqlServer]);
        _kindCombo.SelectedIndex = 0;
        layout.Controls.Add(_kindCombo, 1, 1);
        layout.SetColumnSpan(_kindCombo, 2);

        layout.Controls.Add(new Label { Text = "Name:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
        _nameBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_nameBox, 1, 2);
        layout.SetColumnSpan(_nameBox, 2);

        layout.Controls.Add(new Label { Text = "Path / value:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
        _pathBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_pathBox, 1, 3);
        _browseButton = new Button { Text = "Browse\u2026", Dock = DockStyle.Fill };
        _browseButton.Click += BrowseButton_Click;
        layout.Controls.Add(_browseButton, 2, 3);

        layout.Controls.Add(new Label { Text = "Extensions:", AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 4);
        _extensionsBox = new TextBox { Dock = DockStyle.Fill, PlaceholderText = ".pdf .docx .md" };
        layout.Controls.Add(_extensionsBox, 1, 4);
        layout.SetColumnSpan(_extensionsBox, 2);

        _recursiveCheck = new CheckBox { Text = "Include subfolders", AutoSize = true, Anchor = AnchorStyles.Left, Checked = true };
        layout.Controls.Add(_recursiveCheck, 1, 5);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(12),
        };
        _cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 100 };
        _okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 100 };
        _okButton.Click += OkButton_Click;
        buttons.Controls.Add(_cancelButton);
        buttons.Controls.Add(_okButton);

        Controls.Add(layout);
        Controls.Add(buttons);
        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }

    private void BrowseButton_Click(object? sender, EventArgs e)
    {
        if (_kindCombo.SelectedItem is CatalogEntryKind.File)
        {
            using var dlg = new OpenFileDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _pathBox.Text = dlg.FileName;
            }
        }
        else if (_kindCombo.SelectedItem is CatalogEntryKind.Folder)
        {
            using var dlg = new FolderBrowserDialog { ShowNewFolderButton = false };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _pathBox.Text = dlg.SelectedPath;
            }
        }
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_nameBox.Text) || string.IsNullOrWhiteSpace(_pathBox.Text))
        {
            MessageBox.Show(this, "Please supply a name and a path/value.", "WinBaas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.None;
            return;
        }

        var kind = (CatalogEntryKind)(_kindCombo.SelectedItem ?? CatalogEntryKind.Folder);
        string[] exts = _extensionsBox.Text
            .Split([' ', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.StartsWith('.') ? x : "." + x)
            .ToArray();

        Result = new CatalogEntry
        {
            Category = _categoryCombo.SelectedItem as string ?? "User",
            Kind = kind,
            Name = _nameBox.Text.Trim(),
            Description = $"User-defined {kind} entry.",
            Paths = [_pathBox.Text.Trim()],
            Extensions = exts,
            IncludeSubfolders = _recursiveCheck.Checked,
            IsUserDefined = true,
        };
    }
}
