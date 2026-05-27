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
        _repositoryPanel = new Panel();
        _repositoryLabel = new Label();
        _repositoryLinkLabel = new LinkLabel();
        _fetchButton = new Button();
        _baseBranchPanel = new Panel();
        _baseBranchLabel = new Label();
        _baseBranchComboBox = new ComboBox();
        _sourceBranchesGroupBox = new GroupBox();
        _sourceBranchesDataGridView = new BranchSelectionDataGridView();
        _selectSourceBranchColumn = new DataGridViewCheckBoxColumn();
        _sourceBranchNameColumn = new DataGridViewTextBoxColumn();
        _sourceBranchLastCommitDateColumn = new DataGridViewTextBoxColumn();
        _sourceBranchCommitMessageColumn = new DataGridViewTextBoxColumn();
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
        _repositoryPanel.SuspendLayout();
        _baseBranchPanel.SuspendLayout();
        _sourceBranchesGroupBox.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_sourceBranchesDataGridView).BeginInit();
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
        _baseBranchPanel.Location = new Point(15, 125);
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
        // _repositoryPanel
        //
        _repositoryPanel.Controls.Add(_repositoryLabel);
        _repositoryPanel.Controls.Add(_repositoryLinkLabel);
        _repositoryPanel.Controls.Add(_fetchButton);
        _repositoryPanel.Location = new Point(15, 75);
        _repositoryPanel.Name = "_repositoryPanel";
        _repositoryPanel.Size = new Size(690, 40);
        _repositoryPanel.TabIndex = 1;
        //
        // _repositoryLabel
        //
        _repositoryLabel.AutoSize = true;
        _repositoryLabel.Location = new Point(0, 7);
        _repositoryLabel.Name = "_repositoryLabel";
        _repositoryLabel.Size = new Size(65, 15);
        _repositoryLabel.TabIndex = 0;
        _repositoryLabel.Text = "Repository:";
        //
        // _repositoryLinkLabel
        //
        _repositoryLinkLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _repositoryLinkLabel.Location = new Point(165, 6);
        _repositoryLinkLabel.Name = "_repositoryLinkLabel";
        _repositoryLinkLabel.Size = new Size(405, 18);
        _repositoryLinkLabel.TabIndex = 1;
        _repositoryLinkLabel.TabStop = true;
        _repositoryLinkLabel.Text = "Repository";
        _repositoryLinkLabel.LinkClicked += RepositoryLinkLabel_LinkClicked;
        //
        // _fetchButton
        //
        _fetchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _fetchButton.Location = new Point(580, 0);
        _fetchButton.Name = "_fetchButton";
        _fetchButton.Size = new Size(85, 30);
        _fetchButton.TabIndex = 2;
        _fetchButton.Text = "Git Fetch";
        _fetchButton.UseVisualStyleBackColor = true;
        _fetchButton.Click += FetchButton_Click;
        // 
        // _sourceBranchesGroupBox
        // 
        _sourceBranchesGroupBox.Controls.Add(_sourceBranchesDataGridView);
        _sourceBranchesGroupBox.Location = new Point(15, 185);
        _sourceBranchesGroupBox.Name = "_sourceBranchesGroupBox";
        _sourceBranchesGroupBox.Size = new Size(690, 290);
        _sourceBranchesGroupBox.TabIndex = 2;
        _sourceBranchesGroupBox.TabStop = false;
        _sourceBranchesGroupBox.Text = "Source branches to replay";
        //
        // _sourceBranchesDataGridView
        //
        _sourceBranchesDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _sourceBranchesDataGridView.Columns.AddRange(new DataGridViewColumn[] { _selectSourceBranchColumn, _sourceBranchNameColumn, _sourceBranchLastCommitDateColumn, _sourceBranchCommitMessageColumn });
        _sourceBranchesDataGridView.Location = new Point(15, 25);
        _sourceBranchesDataGridView.Name = "_sourceBranchesDataGridView";
        _sourceBranchesDataGridView.Size = new Size(660, 245);
        _sourceBranchesDataGridView.TabIndex = 0;
        _sourceBranchesDataGridView.CurrentCellDirtyStateChanged += SourceBranchesDataGridView_CurrentCellDirtyStateChanged;
        //
        // _selectSourceBranchColumn
        //
        _selectSourceBranchColumn.HeaderText = "";
        _selectSourceBranchColumn.Name = "_selectSourceBranchColumn";
        _selectSourceBranchColumn.Width = 45;
        //
        // _sourceBranchNameColumn
        //
        _sourceBranchNameColumn.HeaderText = "BranchName";
        _sourceBranchNameColumn.Name = "_sourceBranchNameColumn";
        _sourceBranchNameColumn.ReadOnly = true;
        _sourceBranchNameColumn.Width = 205;
        //
        // _sourceBranchLastCommitDateColumn
        //
        _sourceBranchLastCommitDateColumn.HeaderText = "LastCommitDate";
        _sourceBranchLastCommitDateColumn.Name = "_sourceBranchLastCommitDateColumn";
        _sourceBranchLastCommitDateColumn.ReadOnly = true;
        _sourceBranchLastCommitDateColumn.Width = 145;
        //
        // _sourceBranchCommitMessageColumn
        //
        _sourceBranchCommitMessageColumn.HeaderText = "CommitMessage";
        _sourceBranchCommitMessageColumn.Name = "_sourceBranchCommitMessageColumn";
        _sourceBranchCommitMessageColumn.ReadOnly = true;
        _sourceBranchCommitMessageColumn.Width = 250;
        // 
        // _targetBranchPanel
        // 
        _targetBranchPanel.Controls.Add(_targetBranchLabel);
        _targetBranchPanel.Controls.Add(_targetBranchTextBox);
        _targetBranchPanel.Location = new Point(15, 490);
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
        _namingModePanel.Location = new Point(15, 550);
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
        _numberWidthPanel.Location = new Point(15, 605);
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
        _overwriteCheckBox.Location = new Point(180, 645);
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
        _okButton.Location = new Point(526, 694);
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
        _cancelButton.Location = new Point(620, 694);
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
        ClientSize = new Size(720, 740);
        Controls.Add(_cancelButton);
        Controls.Add(_okButton);
        Controls.Add(_overwriteCheckBox);
        Controls.Add(_numberWidthPanel);
        Controls.Add(_namingModePanel);
        Controls.Add(_targetBranchPanel);
        Controls.Add(_sourceBranchesGroupBox);
        Controls.Add(_baseBranchPanel);
        Controls.Add(_repositoryPanel);
        Controls.Add(_namePanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "BranchSetEditorDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Create Branch-Set";
        _namePanel.ResumeLayout(false);
        _namePanel.PerformLayout();
        _repositoryPanel.ResumeLayout(false);
        _repositoryPanel.PerformLayout();
        _baseBranchPanel.ResumeLayout(false);
        _baseBranchPanel.PerformLayout();
        _sourceBranchesGroupBox.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_sourceBranchesDataGridView).EndInit();
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
    private Panel _repositoryPanel;
    private Label _repositoryLabel;
    private LinkLabel _repositoryLinkLabel;
    private Button _fetchButton;
    private Panel _baseBranchPanel;
    private Label _baseBranchLabel;
    private ComboBox _baseBranchComboBox;
    private GroupBox _sourceBranchesGroupBox;
    private BranchSelectionDataGridView _sourceBranchesDataGridView;
    private DataGridViewCheckBoxColumn _selectSourceBranchColumn;
    private DataGridViewTextBoxColumn _sourceBranchNameColumn;
    private DataGridViewTextBoxColumn _sourceBranchLastCommitDateColumn;
    private DataGridViewTextBoxColumn _sourceBranchCommitMessageColumn;
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
