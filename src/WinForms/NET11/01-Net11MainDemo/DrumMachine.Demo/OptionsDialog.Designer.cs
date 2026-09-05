namespace DrumMachine.Demo;

partial class OptionsDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _layout = new TableLayoutPanel();
        _themeLabel = new Label();
        _theme = new ComboBox();
        _iconsLabel = new Label();
        _icons = new ComboBox();
        _folderLabel = new Label();
        _folder = new TextBox();
        _browse = new Button();
        _restartNote = new Label();
        _error = new Label();
        _buttons = new FlowLayoutPanel();
        _ok = new Button();
        _cancel = new Button();
        _folderPicker = new FolderBrowserDialog();
        components = new System.ComponentModel.Container();
        SuspendLayout();
        _layout.SuspendLayout();
        _buttons.SuspendLayout();
        //
        // _layout
        //
        _layout.AutoSize = true;
        _layout.ColumnCount = 3;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layout.Controls.Add(_themeLabel, 0, 0);
        _layout.Controls.Add(_theme, 1, 0);
        _layout.Controls.Add(_iconsLabel, 0, 1);
        _layout.Controls.Add(_icons, 1, 1);
        _layout.Controls.Add(_folderLabel, 0, 2);
        _layout.Controls.Add(_folder, 1, 2);
        _layout.Controls.Add(_browse, 2, 2);
        _layout.Controls.Add(_restartNote, 1, 3);
        _layout.Controls.Add(_error, 1, 4);
        _layout.Controls.Add(_buttons, 1, 5);
        _layout.Dock = DockStyle.Fill;
        _layout.Name = "_layout";
        _layout.Padding = new Padding(16);
        _layout.RowCount = 6;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(630, 260);
        _layout.TabIndex = 0;
        //
        // Preferences
        //
        _themeLabel.Anchor = AnchorStyles.Left;
        _themeLabel.AutoSize = true;
        _themeLabel.Name = "_themeLabel";
        _themeLabel.TabIndex = 0;
        _themeLabel.Text = "&Theming:";
        _theme.AccessibleName = "Application color mode";
        _theme.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _theme.DropDownStyle = ComboBoxStyle.DropDownList;
        _theme.Items.AddRange(new object[] { "Classic", "Dark mode", "System" });
        _theme.Name = "_theme";
        _theme.TabIndex = 1;
        _iconsLabel.Anchor = AnchorStyles.Left;
        _iconsLabel.AutoSize = true;
        _iconsLabel.Name = "_iconsLabel";
        _iconsLabel.TabIndex = 2;
        _iconsLabel.Text = "&Icon size:";
        _icons.AccessibleName = "Toolbar icon size at 100 percent scaling";
        _icons.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _icons.DropDownStyle = ComboBoxStyle.DropDownList;
        _icons.Items.AddRange(new object[] { "Small (32x32)", "Medium (48x48)", "Large (64x64)" });
        _icons.Name = "_icons";
        _icons.TabIndex = 3;
        _folderLabel.Anchor = AnchorStyles.Left;
        _folderLabel.AutoSize = true;
        _folderLabel.Name = "_folderLabel";
        _folderLabel.TabIndex = 4;
        _folderLabel.Text = "Default &file folder:";
        _folder.AccessibleName = "Default folder for percussion loop files";
        _folder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _folder.Name = "_folder";
        _folder.TabIndex = 5;
        _browse.AutoSize = true;
        _browse.Name = "_browse";
        _browse.TabIndex = 6;
        _browse.Text = "&Browse...";
        _browse.UseVisualStyleBackColor = true;
        _browse.Click += Browse_Click;
        _restartNote.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _restartNote.AutoSize = true;
        _restartNote.Margin = new Padding(3, 12, 3, 3);
        _restartNote.MaximumSize = new Size(410, 0);
        _restartNote.Name = "_restartNote";
        _restartNote.TabIndex = 7;
        _restartNote.Text = "Theme changes require a restart. System follows Windows at launch. Icon size and folder changes apply immediately.";
        _error.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _error.AutoSize = true;
        _error.MaximumSize = new Size(410, 0);
        _error.Name = "_error";
        _error.TabIndex = 8;
        //
        // _buttons
        //
        _buttons.Anchor = AnchorStyles.Right;
        _buttons.AutoSize = true;
        _buttons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttons.Controls.Add(_ok);
        _buttons.Controls.Add(_cancel);
        _buttons.Margin = new Padding(3, 12, 3, 3);
        _buttons.Name = "_buttons";
        _buttons.TabIndex = 9;
        _ok.AutoSize = true;
        _ok.MinimumSize = new Size(75, 28);
        _ok.Name = "_ok";
        _ok.TabIndex = 0;
        _ok.Text = "OK";
        _ok.UseVisualStyleBackColor = true;
        _ok.Click += Ok_Click;
        _cancel.AutoSize = true;
        _cancel.DialogResult = DialogResult.Cancel;
        _cancel.MinimumSize = new Size(75, 28);
        _cancel.Name = "_cancel";
        _cancel.TabIndex = 1;
        _cancel.Text = "Cancel";
        _cancel.UseVisualStyleBackColor = true;
        _folderPicker.Description = "Default folder for percussion loop files";
        _folderPicker.UseDescriptionForTitle = true;
        //
        // OptionsDialog
        //
        AcceptButton = _ok;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = _cancel;
        ClientSize = new Size(630, 260);
        Controls.Add(_layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OptionsDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Options";
        Disposed += OptionsDialog_Disposed;
        _buttons.ResumeLayout(false);
        _buttons.PerformLayout();
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel _layout;
    private Label _themeLabel;
    private ComboBox _theme;
    private Label _iconsLabel;
    private ComboBox _icons;
    private Label _folderLabel;
    private TextBox _folder;
    private Button _browse;
    private Label _restartNote;
    private Label _error;
    private FlowLayoutPanel _buttons;
    private Button _ok;
    private Button _cancel;
    private FolderBrowserDialog _folderPicker;
}
