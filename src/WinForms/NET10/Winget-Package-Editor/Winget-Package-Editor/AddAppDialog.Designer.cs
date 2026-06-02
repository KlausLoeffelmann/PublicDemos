namespace Winget_Package_Editor;

partial class AddAppDialog
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        _layoutPanel = new TableLayoutPanel();
        _appLabel = new Label();
        _appComboBox = new ComboBox();
        _actionLabel = new Label();
        _actionComboBox = new ComboBox();
        _sourceLabel = new Label();
        _sourceComboBox = new ComboBox();
        _scopeLabel = new Label();
        _scopeComboBox = new ComboBox();
        _versionLabel = new Label();
        _versionTextBox = new TextBox();
        _allowPrereleaseCheckBox = new CheckBox();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        _layoutPanel.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 2;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_appLabel, 0, 0);
        _layoutPanel.Controls.Add(_appComboBox, 1, 0);
        _layoutPanel.Controls.Add(_actionLabel, 0, 1);
        _layoutPanel.Controls.Add(_actionComboBox, 1, 1);
        _layoutPanel.Controls.Add(_sourceLabel, 0, 2);
        _layoutPanel.Controls.Add(_sourceComboBox, 1, 2);
        _layoutPanel.Controls.Add(_scopeLabel, 0, 3);
        _layoutPanel.Controls.Add(_scopeComboBox, 1, 3);
        _layoutPanel.Controls.Add(_versionLabel, 0, 4);
        _layoutPanel.Controls.Add(_versionTextBox, 1, 4);
        _layoutPanel.Controls.Add(_allowPrereleaseCheckBox, 1, 5);
        _layoutPanel.Controls.Add(_buttonPanel, 1, 6);
        _layoutPanel.Dock = DockStyle.Fill;
        _layoutPanel.Location = new Point(0, 0);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.Padding = new Padding(12);
        _layoutPanel.RowCount = 7;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layoutPanel.Size = new Size(458, 320);
        _layoutPanel.TabIndex = 0;
        // 
        // _appLabel
        // 
        _appLabel.Anchor = AnchorStyles.Left;
        _appLabel.AutoSize = true;
        _appLabel.Name = "_appLabel";
        _appLabel.Text = "App:";
        // 
        // _appComboBox
        // 
        _appComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _appComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _appComboBox.Name = "_appComboBox";
        _appComboBox.TabIndex = 0;
        // 
        // _actionLabel
        // 
        _actionLabel.Anchor = AnchorStyles.Left;
        _actionLabel.AutoSize = true;
        _actionLabel.Name = "_actionLabel";
        _actionLabel.Text = "Action:";
        // 
        // _actionComboBox
        // 
        _actionComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _actionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _actionComboBox.Name = "_actionComboBox";
        _actionComboBox.TabIndex = 1;
        // 
        // _sourceLabel
        // 
        _sourceLabel.Anchor = AnchorStyles.Left;
        _sourceLabel.AutoSize = true;
        _sourceLabel.Name = "_sourceLabel";
        _sourceLabel.Text = "Source:";
        // 
        // _sourceComboBox
        // 
        _sourceComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _sourceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _sourceComboBox.Name = "_sourceComboBox";
        _sourceComboBox.TabIndex = 2;
        // 
        // _scopeLabel
        // 
        _scopeLabel.Anchor = AnchorStyles.Left;
        _scopeLabel.AutoSize = true;
        _scopeLabel.Name = "_scopeLabel";
        _scopeLabel.Text = "Scope:";
        // 
        // _scopeComboBox
        // 
        _scopeComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _scopeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _scopeComboBox.Name = "_scopeComboBox";
        _scopeComboBox.TabIndex = 3;
        // 
        // _versionLabel
        // 
        _versionLabel.Anchor = AnchorStyles.Left;
        _versionLabel.AutoSize = true;
        _versionLabel.Name = "_versionLabel";
        _versionLabel.Text = "Version (optional):";
        // 
        // _versionTextBox
        // 
        _versionTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _versionTextBox.Name = "_versionTextBox";
        _versionTextBox.TabIndex = 4;
        // 
        // _allowPrereleaseCheckBox
        // 
        _allowPrereleaseCheckBox.Anchor = AnchorStyles.Left;
        _allowPrereleaseCheckBox.AutoSize = true;
        _allowPrereleaseCheckBox.Name = "_allowPrereleaseCheckBox";
        _allowPrereleaseCheckBox.TabIndex = 5;
        _allowPrereleaseCheckBox.Text = "Allow prerelease";
        _allowPrereleaseCheckBox.UseVisualStyleBackColor = true;
        // 
        // _buttonPanel
        // 
        _buttonPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.TabIndex = 6;
        // 
        // _okButton
        // 
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(90, 27);
        _okButton.TabIndex = 0;
        _okButton.Text = "Add";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OkButton_Click;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(90, 27);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // AddAppDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(458, 320);
        Controls.Add(_layoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AddAppDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add App";
        _layoutPanel.ResumeLayout(false);
        _layoutPanel.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _layoutPanel = null!;
    private Label _appLabel = null!;
    private ComboBox _appComboBox = null!;
    private Label _actionLabel = null!;
    private ComboBox _actionComboBox = null!;
    private Label _sourceLabel = null!;
    private ComboBox _sourceComboBox = null!;
    private Label _scopeLabel = null!;
    private ComboBox _scopeComboBox = null!;
    private Label _versionLabel = null!;
    private TextBox _versionTextBox = null!;
    private CheckBox _allowPrereleaseCheckBox = null!;
    private FlowLayoutPanel _buttonPanel = null!;
    private Button _okButton = null!;
    private Button _cancelButton = null!;
}
