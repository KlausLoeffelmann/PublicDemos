namespace BranchComposer.App;

partial class BranchSetEditorDialog
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

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _namePanel = new Panel();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _baseBranchPanel = new Panel();
        _baseBranchLabel = new Label();
        _baseBranchComboBox = new ComboBox();
        _sourceBranchesGroupBox = new GroupBox();
        _sourceBranchesListBox = new CheckedListBox();
        _targetBranchPanel = new Panel();
        _targetBranchLabel = new Label();
        _targetBranchTextBox = new TextBox();
        _namingModePanel = new Panel();
        _namingModeLabel = new Label();
        _namingModeComboBox = new ComboBox();
        _numberWidthPanel = new Panel();
        _numberWidthLabel = new Label();
        _numberWidthNumericUpDown = new NumericUpDown();
        _overwriteCheckBox = new CheckBox();
        _okButton = new Button();
        _cancelButton = new Button();
        _namePanel.SuspendLayout();
        _baseBranchPanel.SuspendLayout();
        _sourceBranchesGroupBox.SuspendLayout();
        _targetBranchPanel.SuspendLayout();
        _namingModePanel.SuspendLayout();
        _numberWidthPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_numberWidthNumericUpDown).BeginInit();
        SuspendLayout();
        // 
        // _namePanel
        // 
        _namePanel.Controls.Add(_nameLabel);
        _namePanel.Controls.Add(_nameTextBox);
        _namePanel.Location = new Point(15, 15);
        _namePanel.Name = "_namePanel";
        _namePanel.Size = new Size(690, 50);
        _namePanel.TabIndex = 0;
        // 
        // _nameLabel
        // 
        _nameLabel.AutoSize = true;
        _nameLabel.Location = new Point(0, 4);
        _nameLabel.Name = "_nameLabel";
        _nameLabel.Size = new Size(96, 15);
        _nameLabel.TabIndex = 0;
        _nameLabel.Text = "Branch-Set name:";
        // 
        // _nameTextBox
        // 
        _nameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _nameTextBox.Location = new Point(165, 0);
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.Size = new Size(500, 23);
        _nameTextBox.TabIndex = 1;
        // 
        // _baseBranchPanel
        // 
        _baseBranchPanel.Controls.Add(_baseBranchLabel);
        _baseBranchPanel.Controls.Add(_baseBranchComboBox);
        _baseBranchPanel.Location = new Point(15, 75);
        _baseBranchPanel.Name = "_baseBranchPanel";
        _baseBranchPanel.Size = new Size(690, 50);
        _baseBranchPanel.TabIndex = 1;
        // 
        // _baseBranchLabel
        // 
        _baseBranchLabel.AutoSize = true;
        _baseBranchLabel.Location = new Point(0, 4);
        _baseBranchLabel.Name = "_baseBranchLabel";
        _baseBranchLabel.Size = new Size(72, 15);
        _baseBranchLabel.TabIndex = 0;
        _baseBranchLabel.Text = "Base branch:";
        // 
        // _baseBranchComboBox
        // 
        _baseBranchComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _baseBranchComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _baseBranchComboBox.FormattingEnabled = true;
        _baseBranchComboBox.Location = new Point(165, 0);
        _baseBranchComboBox.Name = "_baseBranchComboBox";
        _baseBranchComboBox.Size = new Size(500, 23);
        _baseBranchComboBox.TabIndex = 1;
        // 
        // _sourceBranchesGroupBox
        // 
        _sourceBranchesGroupBox.Controls.Add(_sourceBranchesListBox);
        _sourceBranchesGroupBox.Location = new Point(15, 135);
        _sourceBranchesGroupBox.Name = "_sourceBranchesGroupBox";
        _sourceBranchesGroupBox.Size = new Size(690, 250);
        _sourceBranchesGroupBox.TabIndex = 2;
        _sourceBranchesGroupBox.TabStop = false;
        _sourceBranchesGroupBox.Text = "Source branches to replay";
        // 
        // _sourceBranchesListBox
        // 
        _sourceBranchesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchesListBox.CheckOnClick = true;
        _sourceBranchesListBox.FormattingEnabled = true;
        _sourceBranchesListBox.Location = new Point(15, 25);
        _sourceBranchesListBox.Name = "_sourceBranchesListBox";
        _sourceBranchesListBox.Size = new Size(660, 205);
        _sourceBranchesListBox.TabIndex = 0;
        // 
        // _targetBranchPanel
        // 
        _targetBranchPanel.Controls.Add(_targetBranchLabel);
        _targetBranchPanel.Controls.Add(_targetBranchTextBox);
        _targetBranchPanel.Location = new Point(15, 400);
        _targetBranchPanel.Name = "_targetBranchPanel";
        _targetBranchPanel.Size = new Size(690, 50);
        _targetBranchPanel.TabIndex = 3;
        // 
        // _targetBranchLabel
        // 
        _targetBranchLabel.AutoSize = true;
        _targetBranchLabel.Location = new Point(0, 4);
        _targetBranchLabel.Name = "_targetBranchLabel";
        _targetBranchLabel.Size = new Size(113, 15);
        _targetBranchLabel.TabIndex = 0;
        _targetBranchLabel.Text = "Target branch name:";
        // 
        // _targetBranchTextBox
        // 
        _targetBranchTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _targetBranchTextBox.Location = new Point(165, 0);
        _targetBranchTextBox.Name = "_targetBranchTextBox";
        _targetBranchTextBox.Size = new Size(500, 23);
        _targetBranchTextBox.TabIndex = 1;
        // 
        // _namingModePanel
        // 
        _namingModePanel.Controls.Add(_namingModeLabel);
        _namingModePanel.Controls.Add(_namingModeComboBox);
        _namingModePanel.Location = new Point(15, 460);
        _namingModePanel.Name = "_namingModePanel";
        _namingModePanel.Size = new Size(690, 50);
        _namingModePanel.TabIndex = 4;
        // 
        // _namingModeLabel
        // 
        _namingModeLabel.AutoSize = true;
        _namingModeLabel.Location = new Point(0, 4);
        _namingModeLabel.Name = "_namingModeLabel";
        _namingModeLabel.Size = new Size(84, 15);
        _namingModeLabel.TabIndex = 0;
        _namingModeLabel.Text = "Target naming:";
        // 
        // _namingModeComboBox
        // 
        _namingModeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _namingModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _namingModeComboBox.FormattingEnabled = true;
        _namingModeComboBox.Items.AddRange(new object[] { "Fixed / overwrite target", "Date folder", "Increasing numeric suffix" });
        _namingModeComboBox.Location = new Point(165, 0);
        _namingModeComboBox.Name = "_namingModeComboBox";
        _namingModeComboBox.Size = new Size(500, 23);
        _namingModeComboBox.TabIndex = 1;
        // 
        // _numberWidthPanel
        // 
        _numberWidthPanel.Controls.Add(_numberWidthLabel);
        _numberWidthPanel.Controls.Add(_numberWidthNumericUpDown);
        _numberWidthPanel.Location = new Point(15, 515);
        _numberWidthPanel.Name = "_numberWidthPanel";
        _numberWidthPanel.Size = new Size(690, 30);
        _numberWidthPanel.TabIndex = 5;
        // 
        // _numberWidthLabel
        // 
        _numberWidthLabel.AutoSize = true;
        _numberWidthLabel.Location = new Point(0, 4);
        _numberWidthLabel.Name = "_numberWidthLabel";
        _numberWidthLabel.Size = new Size(122, 15);
        _numberWidthLabel.TabIndex = 0;
        _numberWidthLabel.Text = "Numeric suffix width:";
        // 
        // _numberWidthNumericUpDown
        // 
        _numberWidthNumericUpDown.Location = new Point(165, 1);
        _numberWidthNumericUpDown.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
        _numberWidthNumericUpDown.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        _numberWidthNumericUpDown.Name = "_numberWidthNumericUpDown";
        _numberWidthNumericUpDown.Size = new Size(70, 23);
        _numberWidthNumericUpDown.TabIndex = 1;
        _numberWidthNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        // 
        // _overwriteCheckBox
        // 
        _overwriteCheckBox.Location = new Point(180, 555);
        _overwriteCheckBox.Name = "_overwriteCheckBox";
        _overwriteCheckBox.Size = new Size(500, 24);
        _overwriteCheckBox.TabIndex = 6;
        _overwriteCheckBox.Text = "Overwrite existing target branch when naming resolves to an existing branch";
        _overwriteCheckBox.UseVisualStyleBackColor = true;
        // 
        // _okButton
        // 
        _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _okButton.DialogResult = DialogResult.None;
        _okButton.Location = new Point(526, 604);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(85, 30);
        _okButton.TabIndex = 7;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OkButton_Click;
        // 
        // _cancelButton
        // 
        _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(620, 604);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(85, 30);
        _cancelButton.TabIndex = 8;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // BranchSetEditorDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(720, 650);
        Controls.Add(_cancelButton);
        Controls.Add(_okButton);
        Controls.Add(_overwriteCheckBox);
        Controls.Add(_numberWidthPanel);
        Controls.Add(_namingModePanel);
        Controls.Add(_targetBranchPanel);
        Controls.Add(_sourceBranchesGroupBox);
        Controls.Add(_baseBranchPanel);
        Controls.Add(_namePanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "BranchSetEditorDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Create Branch-Set";
        _namePanel.ResumeLayout(false);
        _namePanel.PerformLayout();
        _baseBranchPanel.ResumeLayout(false);
        _baseBranchPanel.PerformLayout();
        _sourceBranchesGroupBox.ResumeLayout(false);
        _targetBranchPanel.ResumeLayout(false);
        _targetBranchPanel.PerformLayout();
        _namingModePanel.ResumeLayout(false);
        _namingModePanel.PerformLayout();
        _numberWidthPanel.ResumeLayout(false);
        _numberWidthPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_numberWidthNumericUpDown).EndInit();
        ResumeLayout(false);
    }

    private Panel _namePanel;
    private Label _nameLabel;
    private TextBox _nameTextBox;
    private Panel _baseBranchPanel;
    private Label _baseBranchLabel;
    private ComboBox _baseBranchComboBox;
    private GroupBox _sourceBranchesGroupBox;
    private CheckedListBox _sourceBranchesListBox;
    private Panel _targetBranchPanel;
    private Label _targetBranchLabel;
    private TextBox _targetBranchTextBox;
    private Panel _namingModePanel;
    private Label _namingModeLabel;
    private ComboBox _namingModeComboBox;
    private Panel _numberWidthPanel;
    private Label _numberWidthLabel;
    private NumericUpDown _numberWidthNumericUpDown;
    private CheckBox _overwriteCheckBox;
    private Button _okButton;
    private Button _cancelButton;
}
