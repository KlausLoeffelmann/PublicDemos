namespace Winget_Package_Editor;

partial class OptionsDialog
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
        components = new System.ComponentModel.Container();
        _layoutPanel = new TableLayoutPanel();
        _fontFamilyLabel = new Label();
        _fontFamilyTextBox = new TextBox();
        _menuFontSizeLabel = new Label();
        _menuFontSizeUpDown = new NumericUpDown();
        _standardFontSizeLabel = new Label();
        _standardFontSizeUpDown = new NumericUpDown();
        _treeDeltaLabel = new Label();
        _treeDeltaUpDown = new NumericUpDown();
        _treeBoldCheckBox = new CheckBox();
        _statusFontSizeLabel = new Label();
        _statusFontSizeUpDown = new NumericUpDown();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)_menuFontSizeUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_standardFontSizeUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_treeDeltaUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_statusFontSizeUpDown).BeginInit();
        _layoutPanel.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 2;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_fontFamilyLabel, 0, 0);
        _layoutPanel.Controls.Add(_fontFamilyTextBox, 1, 0);
        _layoutPanel.Controls.Add(_menuFontSizeLabel, 0, 1);
        _layoutPanel.Controls.Add(_menuFontSizeUpDown, 1, 1);
        _layoutPanel.Controls.Add(_standardFontSizeLabel, 0, 2);
        _layoutPanel.Controls.Add(_standardFontSizeUpDown, 1, 2);
        _layoutPanel.Controls.Add(_treeDeltaLabel, 0, 3);
        _layoutPanel.Controls.Add(_treeDeltaUpDown, 1, 3);
        _layoutPanel.Controls.Add(_treeBoldCheckBox, 1, 4);
        _layoutPanel.Controls.Add(_statusFontSizeLabel, 0, 5);
        _layoutPanel.Controls.Add(_statusFontSizeUpDown, 1, 5);
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
        _layoutPanel.Size = new Size(458, 274);
        _layoutPanel.TabIndex = 0;
        // 
        // _fontFamilyLabel
        // 
        _fontFamilyLabel.Anchor = AnchorStyles.Left;
        _fontFamilyLabel.AutoSize = true;
        _fontFamilyLabel.Location = new Point(15, 18);
        _fontFamilyLabel.Name = "_fontFamilyLabel";
        _fontFamilyLabel.Size = new Size(90, 20);
        _fontFamilyLabel.TabIndex = 0;
        _fontFamilyLabel.Text = "Font family:";
        // 
        // _fontFamilyTextBox
        // 
        _fontFamilyTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _fontFamilyTextBox.Location = new Point(185, 15);
        _fontFamilyTextBox.Name = "_fontFamilyTextBox";
        _fontFamilyTextBox.Size = new Size(258, 27);
        _fontFamilyTextBox.TabIndex = 1;
        // 
        // _menuFontSizeLabel
        // 
        _menuFontSizeLabel.Anchor = AnchorStyles.Left;
        _menuFontSizeLabel.AutoSize = true;
        _menuFontSizeLabel.Location = new Point(15, 50);
        _menuFontSizeLabel.Name = "_menuFontSizeLabel";
        _menuFontSizeLabel.Size = new Size(139, 20);
        _menuFontSizeLabel.TabIndex = 2;
        _menuFontSizeLabel.Text = "MenuStrip size (pt):";
        // 
        // _menuFontSizeUpDown
        // 
        _menuFontSizeUpDown.DecimalPlaces = 1;
        _menuFontSizeUpDown.Increment = 0.5M;
        _menuFontSizeUpDown.Location = new Point(185, 48);
        _menuFontSizeUpDown.Maximum = 24M;
        _menuFontSizeUpDown.Minimum = 6M;
        _menuFontSizeUpDown.Name = "_menuFontSizeUpDown";
        _menuFontSizeUpDown.Size = new Size(90, 27);
        _menuFontSizeUpDown.TabIndex = 3;
        _menuFontSizeUpDown.Value = 11M;
        // 
        // _standardFontSizeLabel
        // 
        _standardFontSizeLabel.Anchor = AnchorStyles.Left;
        _standardFontSizeLabel.AutoSize = true;
        _standardFontSizeLabel.Location = new Point(15, 83);
        _standardFontSizeLabel.Name = "_standardFontSizeLabel";
        _standardFontSizeLabel.Size = new Size(151, 20);
        _standardFontSizeLabel.TabIndex = 4;
        _standardFontSizeLabel.Text = "Standard UI size (pt):";
        // 
        // _standardFontSizeUpDown
        // 
        _standardFontSizeUpDown.DecimalPlaces = 1;
        _standardFontSizeUpDown.Increment = 0.5M;
        _standardFontSizeUpDown.Location = new Point(185, 81);
        _standardFontSizeUpDown.Maximum = 24M;
        _standardFontSizeUpDown.Minimum = 6M;
        _standardFontSizeUpDown.Name = "_standardFontSizeUpDown";
        _standardFontSizeUpDown.Size = new Size(90, 27);
        _standardFontSizeUpDown.TabIndex = 5;
        _standardFontSizeUpDown.Value = 10M;
        // 
        // _treeDeltaLabel
        // 
        _treeDeltaLabel.Anchor = AnchorStyles.Left;
        _treeDeltaLabel.AutoSize = true;
        _treeDeltaLabel.Location = new Point(15, 116);
        _treeDeltaLabel.Name = "_treeDeltaLabel";
        _treeDeltaLabel.Size = new Size(164, 20);
        _treeDeltaLabel.TabIndex = 6;
        _treeDeltaLabel.Text = "Tree root size delta (pt):";
        // 
        // _treeDeltaUpDown
        // 
        _treeDeltaUpDown.DecimalPlaces = 1;
        _treeDeltaUpDown.Increment = 0.5M;
        _treeDeltaUpDown.Location = new Point(185, 114);
        _treeDeltaUpDown.Maximum = 6M;
        _treeDeltaUpDown.Name = "_treeDeltaUpDown";
        _treeDeltaUpDown.Size = new Size(90, 27);
        _treeDeltaUpDown.TabIndex = 7;
        _treeDeltaUpDown.Value = 1M;
        // 
        // _treeBoldCheckBox
        // 
        _treeBoldCheckBox.AutoSize = true;
        _treeBoldCheckBox.Checked = true;
        _treeBoldCheckBox.CheckState = CheckState.Checked;
        _treeBoldCheckBox.Location = new Point(185, 147);
        _treeBoldCheckBox.Name = "_treeBoldCheckBox";
        _treeBoldCheckBox.Size = new Size(181, 24);
        _treeBoldCheckBox.TabIndex = 8;
        _treeBoldCheckBox.Text = "Tree root nodes bold";
        _treeBoldCheckBox.UseVisualStyleBackColor = true;
        // 
        // _statusFontSizeLabel
        // 
        _statusFontSizeLabel.Anchor = AnchorStyles.Left;
        _statusFontSizeLabel.AutoSize = true;
        _statusFontSizeLabel.Location = new Point(15, 181);
        _statusFontSizeLabel.Name = "_statusFontSizeLabel";
        _statusFontSizeLabel.Size = new Size(141, 20);
        _statusFontSizeLabel.TabIndex = 9;
        _statusFontSizeLabel.Text = "StatusStrip size (pt):";
        // 
        // _statusFontSizeUpDown
        // 
        _statusFontSizeUpDown.DecimalPlaces = 1;
        _statusFontSizeUpDown.Increment = 0.5M;
        _statusFontSizeUpDown.Location = new Point(185, 178);
        _statusFontSizeUpDown.Maximum = 24M;
        _statusFontSizeUpDown.Minimum = 6M;
        _statusFontSizeUpDown.Name = "_statusFontSizeUpDown";
        _statusFontSizeUpDown.Size = new Size(90, 27);
        _statusFontSizeUpDown.TabIndex = 10;
        _statusFontSizeUpDown.Value = 10M;
        // 
        // _buttonPanel
        // 
        _buttonPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        _buttonPanel.Location = new Point(281, 230);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(162, 29);
        _buttonPanel.TabIndex = 11;
        // 
        // _okButton
        // 
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Location = new Point(3, 3);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(75, 23);
        _okButton.TabIndex = 0;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OkButton_Click;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(84, 3);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(75, 23);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // OptionsDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(458, 274);
        Controls.Add(_layoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OptionsDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Options";
        ((System.ComponentModel.ISupportInitialize)_menuFontSizeUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)_standardFontSizeUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)_treeDeltaUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)_statusFontSizeUpDown).EndInit();
        _layoutPanel.ResumeLayout(false);
        _layoutPanel.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _layoutPanel = null!;
    private Label _fontFamilyLabel = null!;
    private TextBox _fontFamilyTextBox = null!;
    private Label _menuFontSizeLabel = null!;
    private NumericUpDown _menuFontSizeUpDown = null!;
    private Label _standardFontSizeLabel = null!;
    private NumericUpDown _standardFontSizeUpDown = null!;
    private Label _treeDeltaLabel = null!;
    private NumericUpDown _treeDeltaUpDown = null!;
    private CheckBox _treeBoldCheckBox = null!;
    private Label _statusFontSizeLabel = null!;
    private NumericUpDown _statusFontSizeUpDown = null!;
    private FlowLayoutPanel _buttonPanel = null!;
    private Button _okButton = null!;
    private Button _cancelButton = null!;
}
