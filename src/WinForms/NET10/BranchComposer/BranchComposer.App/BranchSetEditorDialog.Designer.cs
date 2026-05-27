namespace BranchComposer.App;

partial class BranchSetEditorDialog
{
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
        _mainLayoutPanel = new TableLayoutPanel();
        _headerLabel = new Label();
        _metadataGroupBox = new GroupBox();
        _metadataLayoutPanel = new TableLayoutPanel();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _repositoryLabel = new Label();
        _repositoryLinkLabel = new LinkLabel();
        _fetchButton = new Button();
        _baseBranchLabel = new Label();
        _baseBranchComboBox = new ComboBox();
        _sourceBranchesGroupBox = new GroupBox();
        _sourceBranchesLayoutPanel = new TableLayoutPanel();
        _sourceBranchesHelpLabel = new Label();
        _sourceBranchesFilterLayoutPanel = new TableLayoutPanel();
        _sourceBranchFilterLabel = new Label();
        _sourceBranchFilterTextBox = new TextBox();
        _selectedSourceBranchesLabel = new Label();
        _sourceBranchesDataGridView = new BranchSelectionDataGridView();
        _selectSourceBranchColumn = new DataGridViewCheckBoxColumn();
        _sourceBranchNameColumn = new DataGridViewTextBoxColumn();
        _sourceBranchLastCommitDateColumn = new DataGridViewTextBoxColumn();
        _sourceBranchCommitMessageColumn = new DataGridViewTextBoxColumn();
        _targetBranchGroupBox = new GroupBox();
        _targetBranchLayoutPanel = new TableLayoutPanel();
        _targetBranchLabel = new Label();
        _targetBranchTextBox = new TextBox();
        _namingModeLabel = new Label();
        _namingModeComboBox = new ComboBox();
        _numberWidthLabel = new Label();
        _numberWidthNumericUpDown = new NumericUpDown();
        _overwriteCheckBox = new CheckBox();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        _mainLayoutPanel.SuspendLayout();
        _metadataGroupBox.SuspendLayout();
        _metadataLayoutPanel.SuspendLayout();
        _sourceBranchesGroupBox.SuspendLayout();
        _sourceBranchesLayoutPanel.SuspendLayout();
        _sourceBranchesFilterLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_sourceBranchesDataGridView).BeginInit();
        _targetBranchGroupBox.SuspendLayout();
        _targetBranchLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_numberWidthNumericUpDown).BeginInit();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        //
        // _mainLayoutPanel
        //
        _mainLayoutPanel.ColumnCount = 1;
        _mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _mainLayoutPanel.Controls.Add(_headerLabel, 0, 0);
        _mainLayoutPanel.Controls.Add(_metadataGroupBox, 0, 1);
        _mainLayoutPanel.Controls.Add(_sourceBranchesGroupBox, 0, 2);
        _mainLayoutPanel.Controls.Add(_targetBranchGroupBox, 0, 3);
        _mainLayoutPanel.Controls.Add(_buttonPanel, 0, 4);
        _mainLayoutPanel.Dock = DockStyle.Fill;
        _mainLayoutPanel.Location = new Point(0, 0);
        _mainLayoutPanel.Name = "_mainLayoutPanel";
        _mainLayoutPanel.Padding = new Padding(12);
        _mainLayoutPanel.RowCount = 5;
        _mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _mainLayoutPanel.Size = new Size(984, 760);
        _mainLayoutPanel.TabIndex = 0;
        //
        // _headerLabel
        //
        _headerLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _headerLabel.AutoSize = true;
        _headerLabel.Location = new Point(15, 12);
        _headerLabel.Name = "_headerLabel";
        _headerLabel.Size = new Size(954, 15);
        _headerLabel.TabIndex = 0;
        _headerLabel.Text = "Create a reusable Branch-Set by choosing a base branch and the feature branches to replay into a target branch.";
        //
        // _metadataGroupBox
        //
        _metadataGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _metadataGroupBox.Controls.Add(_metadataLayoutPanel);
        _metadataGroupBox.Location = new Point(15, 37);
        _metadataGroupBox.Margin = new Padding(3, 10, 3, 3);
        _metadataGroupBox.Name = "_metadataGroupBox";
        _metadataGroupBox.Padding = new Padding(10);
        _metadataGroupBox.Size = new Size(954, 124);
        _metadataGroupBox.TabIndex = 1;
        _metadataGroupBox.TabStop = false;
        _metadataGroupBox.Text = "Branch-Set and baseline";
        //
        // _metadataLayoutPanel
        //
        _metadataLayoutPanel.ColumnCount = 3;
        _metadataLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _metadataLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _metadataLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _metadataLayoutPanel.Controls.Add(_nameLabel, 0, 0);
        _metadataLayoutPanel.Controls.Add(_nameTextBox, 1, 0);
        _metadataLayoutPanel.Controls.Add(_repositoryLabel, 0, 1);
        _metadataLayoutPanel.Controls.Add(_repositoryLinkLabel, 1, 1);
        _metadataLayoutPanel.Controls.Add(_fetchButton, 2, 1);
        _metadataLayoutPanel.Controls.Add(_baseBranchLabel, 0, 2);
        _metadataLayoutPanel.Controls.Add(_baseBranchComboBox, 1, 2);
        _metadataLayoutPanel.Dock = DockStyle.Fill;
        _metadataLayoutPanel.Location = new Point(10, 26);
        _metadataLayoutPanel.Name = "_metadataLayoutPanel";
        _metadataLayoutPanel.RowCount = 3;
        _metadataLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _metadataLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _metadataLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _metadataLayoutPanel.Size = new Size(934, 88);
        _metadataLayoutPanel.TabIndex = 0;
        _metadataLayoutPanel.SetColumnSpan(_nameTextBox, 2);
        _metadataLayoutPanel.SetColumnSpan(_baseBranchComboBox, 2);
        //
        // _nameLabel
        //
        _nameLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _nameLabel.AutoSize = true;
        _nameLabel.Location = new Point(3, 7);
        _nameLabel.Name = "_nameLabel";
        _nameLabel.Size = new Size(113, 15);
        _nameLabel.TabIndex = 0;
        _nameLabel.Text = "Branch-Set name:";
        //
        // _nameTextBox
        //
        _nameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _nameTextBox.Location = new Point(122, 3);
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.Size = new Size(809, 23);
        _nameTextBox.TabIndex = 1;
        //
        // _repositoryLabel
        //
        _repositoryLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _repositoryLabel.AutoSize = true;
        _repositoryLabel.Location = new Point(3, 36);
        _repositoryLabel.Name = "_repositoryLabel";
        _repositoryLabel.Size = new Size(113, 15);
        _repositoryLabel.TabIndex = 2;
        _repositoryLabel.Text = "Repository:";
        //
        // _repositoryLinkLabel
        //
        _repositoryLinkLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _repositoryLinkLabel.AutoEllipsis = true;
        _repositoryLinkLabel.Location = new Point(122, 33);
        _repositoryLinkLabel.Margin = new Padding(3, 4, 3, 0);
        _repositoryLinkLabel.Name = "_repositoryLinkLabel";
        _repositoryLinkLabel.Size = new Size(681, 20);
        _repositoryLinkLabel.TabIndex = 3;
        _repositoryLinkLabel.TabStop = true;
        _repositoryLinkLabel.Text = "Repository";
        _repositoryLinkLabel.LinkClicked += RepositoryLinkLabel_LinkClicked;
        //
        // _fetchButton
        //
        _fetchButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _fetchButton.AutoSize = true;
        _fetchButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _fetchButton.Location = new Point(809, 32);
        _fetchButton.Name = "_fetchButton";
        _fetchButton.Size = new Size(122, 25);
        _fetchButton.TabIndex = 4;
        _fetchButton.Text = "Fetch latest branches";
        _fetchButton.UseVisualStyleBackColor = true;
        _fetchButton.Click += FetchButton_Click;
        //
        // _baseBranchLabel
        //
        _baseBranchLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _baseBranchLabel.AutoSize = true;
        _baseBranchLabel.Location = new Point(3, 66);
        _baseBranchLabel.Name = "_baseBranchLabel";
        _baseBranchLabel.Size = new Size(113, 15);
        _baseBranchLabel.TabIndex = 5;
        _baseBranchLabel.Text = "Base branch:";
        //
        // _baseBranchComboBox
        //
        _baseBranchComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _baseBranchComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _baseBranchComboBox.FormattingEnabled = true;
        _baseBranchComboBox.Location = new Point(122, 62);
        _baseBranchComboBox.Name = "_baseBranchComboBox";
        _baseBranchComboBox.Size = new Size(809, 23);
        _baseBranchComboBox.TabIndex = 6;
        _baseBranchComboBox.SelectedIndexChanged += BaseBranchComboBox_SelectedIndexChanged;
        //
        // _sourceBranchesGroupBox
        //
        _sourceBranchesGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchesGroupBox.Controls.Add(_sourceBranchesLayoutPanel);
        _sourceBranchesGroupBox.Location = new Point(15, 174);
        _sourceBranchesGroupBox.Margin = new Padding(3, 10, 3, 3);
        _sourceBranchesGroupBox.Name = "_sourceBranchesGroupBox";
        _sourceBranchesGroupBox.Padding = new Padding(10);
        _sourceBranchesGroupBox.Size = new Size(954, 347);
        _sourceBranchesGroupBox.TabIndex = 2;
        _sourceBranchesGroupBox.TabStop = false;
        _sourceBranchesGroupBox.Text = "Branches to replay";
        //
        // _sourceBranchesLayoutPanel
        //
        _sourceBranchesLayoutPanel.ColumnCount = 1;
        _sourceBranchesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _sourceBranchesLayoutPanel.Controls.Add(_sourceBranchesHelpLabel, 0, 0);
        _sourceBranchesLayoutPanel.Controls.Add(_sourceBranchesFilterLayoutPanel, 0, 1);
        _sourceBranchesLayoutPanel.Controls.Add(_sourceBranchesDataGridView, 0, 2);
        _sourceBranchesLayoutPanel.Dock = DockStyle.Fill;
        _sourceBranchesLayoutPanel.Location = new Point(10, 26);
        _sourceBranchesLayoutPanel.Name = "_sourceBranchesLayoutPanel";
        _sourceBranchesLayoutPanel.RowCount = 3;
        _sourceBranchesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _sourceBranchesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _sourceBranchesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _sourceBranchesLayoutPanel.Size = new Size(934, 311);
        _sourceBranchesLayoutPanel.TabIndex = 0;
        //
        // _sourceBranchesHelpLabel
        //
        _sourceBranchesHelpLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchesHelpLabel.AutoSize = true;
        _sourceBranchesHelpLabel.Location = new Point(3, 0);
        _sourceBranchesHelpLabel.Name = "_sourceBranchesHelpLabel";
        _sourceBranchesHelpLabel.Size = new Size(928, 15);
        _sourceBranchesHelpLabel.TabIndex = 0;
        _sourceBranchesHelpLabel.Text = "Selected branches must already be rebased on the selected base branch before composition.";
        //
        // _sourceBranchesFilterLayoutPanel
        //
        _sourceBranchesFilterLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchesFilterLayoutPanel.ColumnCount = 3;
        _sourceBranchesFilterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _sourceBranchesFilterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _sourceBranchesFilterLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _sourceBranchesFilterLayoutPanel.Controls.Add(_sourceBranchFilterLabel, 0, 0);
        _sourceBranchesFilterLayoutPanel.Controls.Add(_sourceBranchFilterTextBox, 1, 0);
        _sourceBranchesFilterLayoutPanel.Controls.Add(_selectedSourceBranchesLabel, 2, 0);
        _sourceBranchesFilterLayoutPanel.Location = new Point(0, 23);
        _sourceBranchesFilterLayoutPanel.Margin = new Padding(0, 8, 0, 6);
        _sourceBranchesFilterLayoutPanel.Name = "_sourceBranchesFilterLayoutPanel";
        _sourceBranchesFilterLayoutPanel.RowCount = 1;
        _sourceBranchesFilterLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _sourceBranchesFilterLayoutPanel.Size = new Size(934, 29);
        _sourceBranchesFilterLayoutPanel.TabIndex = 1;
        //
        // _sourceBranchFilterLabel
        //
        _sourceBranchFilterLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchFilterLabel.AutoSize = true;
        _sourceBranchFilterLabel.Location = new Point(3, 7);
        _sourceBranchFilterLabel.Name = "_sourceBranchFilterLabel";
        _sourceBranchFilterLabel.Size = new Size(36, 15);
        _sourceBranchFilterLabel.TabIndex = 0;
        _sourceBranchFilterLabel.Text = "Filter:";
        //
        // _sourceBranchFilterTextBox
        //
        _sourceBranchFilterTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchFilterTextBox.Location = new Point(45, 3);
        _sourceBranchFilterTextBox.Name = "_sourceBranchFilterTextBox";
        _sourceBranchFilterTextBox.Size = new Size(787, 23);
        _sourceBranchFilterTextBox.TabIndex = 1;
        _sourceBranchFilterTextBox.TextChanged += SourceBranchFilterTextBox_TextChanged;
        //
        // _selectedSourceBranchesLabel
        //
        _selectedSourceBranchesLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _selectedSourceBranchesLabel.AutoSize = true;
        _selectedSourceBranchesLabel.Location = new Point(838, 7);
        _selectedSourceBranchesLabel.Name = "_selectedSourceBranchesLabel";
        _selectedSourceBranchesLabel.Size = new Size(93, 15);
        _selectedSourceBranchesLabel.TabIndex = 2;
        _selectedSourceBranchesLabel.Text = "0 selected";
        //
        // _sourceBranchesDataGridView
        //
        _sourceBranchesDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _sourceBranchesDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _sourceBranchesDataGridView.Columns.AddRange(new DataGridViewColumn[] { _selectSourceBranchColumn, _sourceBranchNameColumn, _sourceBranchLastCommitDateColumn, _sourceBranchCommitMessageColumn });
        _sourceBranchesDataGridView.Location = new Point(3, 61);
        _sourceBranchesDataGridView.Name = "_sourceBranchesDataGridView";
        _sourceBranchesDataGridView.Size = new Size(928, 247);
        _sourceBranchesDataGridView.TabIndex = 2;
        _sourceBranchesDataGridView.CellValueChanged += SourceBranchesDataGridView_CellValueChanged;
        _sourceBranchesDataGridView.CurrentCellDirtyStateChanged += SourceBranchesDataGridView_CurrentCellDirtyStateChanged;
        //
        // _selectSourceBranchColumn
        //
        _selectSourceBranchColumn.HeaderText = "Select";
        _selectSourceBranchColumn.Name = "_selectSourceBranchColumn";
        _selectSourceBranchColumn.Width = 64;
        //
        // _sourceBranchNameColumn
        //
        _sourceBranchNameColumn.HeaderText = "Branch";
        _sourceBranchNameColumn.Name = "_sourceBranchNameColumn";
        _sourceBranchNameColumn.ReadOnly = true;
        _sourceBranchNameColumn.Width = 260;
        //
        // _sourceBranchLastCommitDateColumn
        //
        _sourceBranchLastCommitDateColumn.HeaderText = "Last commit";
        _sourceBranchLastCommitDateColumn.Name = "_sourceBranchLastCommitDateColumn";
        _sourceBranchLastCommitDateColumn.ReadOnly = true;
        _sourceBranchLastCommitDateColumn.Width = 150;
        //
        // _sourceBranchCommitMessageColumn
        //
        _sourceBranchCommitMessageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _sourceBranchCommitMessageColumn.HeaderText = "Latest commit";
        _sourceBranchCommitMessageColumn.Name = "_sourceBranchCommitMessageColumn";
        _sourceBranchCommitMessageColumn.ReadOnly = true;
        //
        // _targetBranchGroupBox
        //
        _targetBranchGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _targetBranchGroupBox.Controls.Add(_targetBranchLayoutPanel);
        _targetBranchGroupBox.Location = new Point(15, 534);
        _targetBranchGroupBox.Margin = new Padding(3, 10, 3, 3);
        _targetBranchGroupBox.Name = "_targetBranchGroupBox";
        _targetBranchGroupBox.Padding = new Padding(10);
        _targetBranchGroupBox.Size = new Size(954, 154);
        _targetBranchGroupBox.TabIndex = 3;
        _targetBranchGroupBox.TabStop = false;
        _targetBranchGroupBox.Text = "Target branch";
        //
        // _targetBranchLayoutPanel
        //
        _targetBranchLayoutPanel.ColumnCount = 2;
        _targetBranchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _targetBranchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _targetBranchLayoutPanel.Controls.Add(_targetBranchLabel, 0, 0);
        _targetBranchLayoutPanel.Controls.Add(_targetBranchTextBox, 1, 0);
        _targetBranchLayoutPanel.Controls.Add(_namingModeLabel, 0, 1);
        _targetBranchLayoutPanel.Controls.Add(_namingModeComboBox, 1, 1);
        _targetBranchLayoutPanel.Controls.Add(_numberWidthLabel, 0, 2);
        _targetBranchLayoutPanel.Controls.Add(_numberWidthNumericUpDown, 1, 2);
        _targetBranchLayoutPanel.Controls.Add(_overwriteCheckBox, 1, 3);
        _targetBranchLayoutPanel.Dock = DockStyle.Fill;
        _targetBranchLayoutPanel.Location = new Point(10, 26);
        _targetBranchLayoutPanel.Name = "_targetBranchLayoutPanel";
        _targetBranchLayoutPanel.RowCount = 4;
        _targetBranchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _targetBranchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _targetBranchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _targetBranchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _targetBranchLayoutPanel.Size = new Size(934, 118);
        _targetBranchLayoutPanel.TabIndex = 0;
        //
        // _targetBranchLabel
        //
        _targetBranchLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _targetBranchLabel.AutoSize = true;
        _targetBranchLabel.Location = new Point(3, 7);
        _targetBranchLabel.Name = "_targetBranchLabel";
        _targetBranchLabel.Size = new Size(135, 15);
        _targetBranchLabel.TabIndex = 0;
        _targetBranchLabel.Text = "Target branch name:";
        //
        // _targetBranchTextBox
        //
        _targetBranchTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _targetBranchTextBox.Location = new Point(144, 3);
        _targetBranchTextBox.Name = "_targetBranchTextBox";
        _targetBranchTextBox.Size = new Size(787, 23);
        _targetBranchTextBox.TabIndex = 1;
        //
        // _namingModeLabel
        //
        _namingModeLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _namingModeLabel.AutoSize = true;
        _namingModeLabel.Location = new Point(3, 36);
        _namingModeLabel.Name = "_namingModeLabel";
        _namingModeLabel.Size = new Size(135, 15);
        _namingModeLabel.TabIndex = 2;
        _namingModeLabel.Text = "Target naming:";
        //
        // _namingModeComboBox
        //
        _namingModeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _namingModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _namingModeComboBox.FormattingEnabled = true;
        _namingModeComboBox.Items.AddRange(new object[] { "Fixed target branch", "Date folder", "Increasing numeric suffix" });
        _namingModeComboBox.Location = new Point(144, 32);
        _namingModeComboBox.Name = "_namingModeComboBox";
        _namingModeComboBox.Size = new Size(787, 23);
        _namingModeComboBox.TabIndex = 3;
        _namingModeComboBox.SelectedIndexChanged += NamingModeComboBox_SelectedIndexChanged;
        //
        // _numberWidthLabel
        //
        _numberWidthLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _numberWidthLabel.AutoSize = true;
        _numberWidthLabel.Location = new Point(3, 65);
        _numberWidthLabel.Name = "_numberWidthLabel";
        _numberWidthLabel.Size = new Size(135, 15);
        _numberWidthLabel.TabIndex = 4;
        _numberWidthLabel.Text = "Numeric suffix width:";
        //
        // _numberWidthNumericUpDown
        //
        _numberWidthNumericUpDown.Location = new Point(144, 61);
        _numberWidthNumericUpDown.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
        _numberWidthNumericUpDown.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        _numberWidthNumericUpDown.Name = "_numberWidthNumericUpDown";
        _numberWidthNumericUpDown.Size = new Size(70, 23);
        _numberWidthNumericUpDown.TabIndex = 5;
        _numberWidthNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
        //
        // _overwriteCheckBox
        //
        _overwriteCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _overwriteCheckBox.AutoSize = true;
        _overwriteCheckBox.Location = new Point(144, 90);
        _overwriteCheckBox.Name = "_overwriteCheckBox";
        _overwriteCheckBox.Size = new Size(787, 19);
        _overwriteCheckBox.TabIndex = 6;
        _overwriteCheckBox.Text = "Allow overwriting an existing target branch when the naming strategy resolves to one.";
        _overwriteCheckBox.UseVisualStyleBackColor = true;
        //
        // _buttonPanel
        //
        _buttonPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        _buttonPanel.Location = new Point(767, 669);
        _buttonPanel.Margin = new Padding(3, 9, 0, 0);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(202, 30);
        _buttonPanel.TabIndex = 4;
        _buttonPanel.WrapContents = false;
        //
        // _okButton
        //
        _okButton.AutoSize = true;
        _okButton.DialogResult = DialogResult.None;
        _okButton.Location = new Point(3, 3);
        _okButton.MinimumSize = new Size(94, 0);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(94, 25);
        _okButton.TabIndex = 0;
        _okButton.Text = "Create Branch-Set";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OkButton_Click;
        //
        // _cancelButton
        //
        _cancelButton.AutoSize = true;
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(103, 3);
        _cancelButton.MinimumSize = new Size(94, 0);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(96, 25);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        //
        // BranchSetEditorDialog
        //
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(984, 760);
        Controls.Add(_mainLayoutPanel);
        Margin = new Padding(3);
        MinimizeBox = false;
        MinimumSize = new Size(760, 600);
        Name = "BranchSetEditorDialog";
        ShowIcon = false;
        SizeGripStyle = SizeGripStyle.Show;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Create Branch-Set";
        _mainLayoutPanel.ResumeLayout(false);
        _mainLayoutPanel.PerformLayout();
        _metadataGroupBox.ResumeLayout(false);
        _metadataLayoutPanel.ResumeLayout(false);
        _metadataLayoutPanel.PerformLayout();
        _sourceBranchesGroupBox.ResumeLayout(false);
        _sourceBranchesLayoutPanel.ResumeLayout(false);
        _sourceBranchesLayoutPanel.PerformLayout();
        _sourceBranchesFilterLayoutPanel.ResumeLayout(false);
        _sourceBranchesFilterLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_sourceBranchesDataGridView).EndInit();
        _targetBranchGroupBox.ResumeLayout(false);
        _targetBranchLayoutPanel.ResumeLayout(false);
        _targetBranchLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_numberWidthNumericUpDown).EndInit();
        _buttonPanel.ResumeLayout(false);
        _buttonPanel.PerformLayout();
        ResumeLayout(false);
    }

    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel _mainLayoutPanel;
    private Label _headerLabel;
    private GroupBox _metadataGroupBox;
    private TableLayoutPanel _metadataLayoutPanel;
    private Label _nameLabel;
    private TextBox _nameTextBox;
    private Label _repositoryLabel;
    private LinkLabel _repositoryLinkLabel;
    private Button _fetchButton;
    private Label _baseBranchLabel;
    private ComboBox _baseBranchComboBox;
    private GroupBox _sourceBranchesGroupBox;
    private TableLayoutPanel _sourceBranchesLayoutPanel;
    private Label _sourceBranchesHelpLabel;
    private TableLayoutPanel _sourceBranchesFilterLayoutPanel;
    private Label _sourceBranchFilterLabel;
    private TextBox _sourceBranchFilterTextBox;
    private Label _selectedSourceBranchesLabel;
    private BranchSelectionDataGridView _sourceBranchesDataGridView;
    private DataGridViewCheckBoxColumn _selectSourceBranchColumn;
    private DataGridViewTextBoxColumn _sourceBranchNameColumn;
    private DataGridViewTextBoxColumn _sourceBranchLastCommitDateColumn;
    private DataGridViewTextBoxColumn _sourceBranchCommitMessageColumn;
    private GroupBox _targetBranchGroupBox;
    private TableLayoutPanel _targetBranchLayoutPanel;
    private Label _targetBranchLabel;
    private TextBox _targetBranchTextBox;
    private Label _namingModeLabel;
    private ComboBox _namingModeComboBox;
    private Label _numberWidthLabel;
    private NumericUpDown _numberWidthNumericUpDown;
    private CheckBox _overwriteCheckBox;
    private FlowLayoutPanel _buttonPanel;
    private Button _okButton;
    private Button _cancelButton;
}
