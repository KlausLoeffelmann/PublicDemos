using System.ComponentModel;

namespace WarpClock.App;

partial class ThemeListEditorDialog
{
    private IContainer components;

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
        _layoutPanel = new TableLayoutPanel();
        _introLabel = new Label();
        _settingsPanel = new TableLayoutPanel();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _currentPathLabel = new Label();
        _currentPathValueLabel = new Label();
        _useAsDefaultPathCheckBox = new CheckBox();
        _autoRotateCheckBox = new CheckBox();
        _dayStartLabel = new Label();
        _dayStartPicker = new DateTimePicker();
        _nightStartLabel = new Label();
        _nightStartPicker = new DateTimePicker();
        _rotationLabel = new Label();
        _rotationMinutesUpDown = new NumericUpDown();
        _rotationSuffixLabel = new Label();
        _validationLabel = new Label();
        _themeGrid = new DataGridView();
        _enabledColumn = new DataGridViewCheckBoxColumn();
        _dayColumn = new DataGridViewCheckBoxColumn();
        _nightColumn = new DataGridViewCheckBoxColumn();
        _themeColumn = new DataGridViewTextBoxColumn();
        _sourceColumn = new DataGridViewTextBoxColumn();
        _statusColumn = new DataGridViewTextBoxColumn();
        _buttonPanel = new FlowLayoutPanel();
        _resetDefaultsButton = new Button();
        _okButton = new Button();
        _cancelButton = new Button();
        _layoutPanel.SuspendLayout();
        _settingsPanel.SuspendLayout();
        ((ISupportInitialize)_rotationMinutesUpDown).BeginInit();
        ((ISupportInitialize)_themeGrid).BeginInit();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 1;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_introLabel, 0, 0);
        _layoutPanel.Controls.Add(_settingsPanel, 0, 1);
        _layoutPanel.Controls.Add(_themeGrid, 0, 2);
        _layoutPanel.Controls.Add(_buttonPanel, 0, 3);
        _layoutPanel.Dock = DockStyle.Fill;
        _layoutPanel.Location = new Point(0, 0);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.Padding = new Padding(12);
        _layoutPanel.RowCount = 4;
        _layoutPanel.RowStyles.Add(new RowStyle());
        _layoutPanel.RowStyles.Add(new RowStyle());
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layoutPanel.RowStyles.Add(new RowStyle());
        _layoutPanel.Size = new Size(1100, 760);
        _layoutPanel.TabIndex = 0;
        // 
        // _introLabel
        // 
        _introLabel.AutoSize = true;
        _introLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        _introLabel.Location = new Point(12, 15);
        _introLabel.Margin = new Padding(0, 3, 3, 10);
        _introLabel.Name = "_introLabel";
        _introLabel.Size = new Size(881, 32);
        _introLabel.TabIndex = 0;
        _introLabel.Text = "Choose which theme families participate during the day and night schedule.";
        // 
        // _settingsPanel
        // 
        _settingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _settingsPanel.AutoSize = true;
        _settingsPanel.ColumnCount = 7;
        _settingsPanel.ColumnStyles.Add(new ColumnStyle());
        _settingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        _settingsPanel.ColumnStyles.Add(new ColumnStyle());
        _settingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        _settingsPanel.ColumnStyles.Add(new ColumnStyle());
        _settingsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        _settingsPanel.ColumnStyles.Add(new ColumnStyle());
        _settingsPanel.Controls.Add(_nameLabel, 0, 0);
        _settingsPanel.Controls.Add(_nameTextBox, 1, 0);
        _settingsPanel.Controls.Add(_currentPathLabel, 0, 1);
        _settingsPanel.Controls.Add(_currentPathValueLabel, 1, 1);
        _settingsPanel.Controls.Add(_useAsDefaultPathCheckBox, 1, 2);
        _settingsPanel.Controls.Add(_autoRotateCheckBox, 1, 3);
        _settingsPanel.Controls.Add(_dayStartLabel, 0, 4);
        _settingsPanel.Controls.Add(_dayStartPicker, 1, 4);
        _settingsPanel.Controls.Add(_nightStartLabel, 2, 4);
        _settingsPanel.Controls.Add(_nightStartPicker, 3, 4);
        _settingsPanel.Controls.Add(_rotationLabel, 4, 4);
        _settingsPanel.Controls.Add(_rotationMinutesUpDown, 5, 4);
        _settingsPanel.Controls.Add(_rotationSuffixLabel, 6, 4);
        _settingsPanel.Controls.Add(_validationLabel, 0, 5);
        _settingsPanel.Location = new Point(15, 60);
        _settingsPanel.Name = "_settingsPanel";
        _settingsPanel.RowCount = 6;
        _settingsPanel.RowStyles.Add(new RowStyle());
        _settingsPanel.RowStyles.Add(new RowStyle());
        _settingsPanel.RowStyles.Add(new RowStyle());
        _settingsPanel.RowStyles.Add(new RowStyle());
        _settingsPanel.RowStyles.Add(new RowStyle());
        _settingsPanel.RowStyles.Add(new RowStyle());
        _settingsPanel.Size = new Size(1070, 273);
        _settingsPanel.TabIndex = 1;
        // 
        // _nameLabel
        // 
        _nameLabel.Anchor = AnchorStyles.Left;
        _nameLabel.AutoSize = true;
        _nameLabel.Location = new Point(3, 13);
        _nameLabel.Name = "_nameLabel";
        _nameLabel.Size = new Size(83, 32);
        _nameLabel.TabIndex = 0;
        _nameLabel.Text = "Name:";
        // 
        // _nameTextBox
        // 
        _settingsPanel.SetColumnSpan(_nameTextBox, 6);
        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Location = new Point(132, 3);
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.Size = new Size(935, 52);
        _nameTextBox.TabIndex = 1;
        // 
        // _currentPathLabel
        // 
        _currentPathLabel.Anchor = AnchorStyles.Left;
        _currentPathLabel.AutoSize = true;
        _currentPathLabel.Location = new Point(3, 63);
        _currentPathLabel.Margin = new Padding(3, 5, 3, 5);
        _currentPathLabel.Name = "_currentPathLabel";
        _currentPathLabel.Size = new Size(111, 32);
        _currentPathLabel.TabIndex = 2;
        _currentPathLabel.Text = "File path:";
        // 
        // _currentPathValueLabel
        // 
        _currentPathValueLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _currentPathValueLabel.AutoEllipsis = true;
        _currentPathValueLabel.AutoSize = true;
        _settingsPanel.SetColumnSpan(_currentPathValueLabel, 6);
        _currentPathValueLabel.Location = new Point(132, 63);
        _currentPathValueLabel.Margin = new Padding(3, 5, 3, 5);
        _currentPathValueLabel.Name = "_currentPathValueLabel";
        _currentPathValueLabel.Size = new Size(935, 32);
        _currentPathValueLabel.TabIndex = 3;
        _currentPathValueLabel.Text = "Unsaved";
        // 
        // _useAsDefaultPathCheckBox
        // 
        _useAsDefaultPathCheckBox.Appearance = Appearance.ToggleSwitch;
        _useAsDefaultPathCheckBox.AutoSize = true;
        _settingsPanel.SetColumnSpan(_useAsDefaultPathCheckBox, 6);
        _useAsDefaultPathCheckBox.Location = new Point(134, 105);
        _useAsDefaultPathCheckBox.Margin = new Padding(5);
        _useAsDefaultPathCheckBox.Name = "_useAsDefaultPathCheckBox";
        _useAsDefaultPathCheckBox.Size = new Size(464, 32);
        _useAsDefaultPathCheckBox.TabIndex = 4;
        _useAsDefaultPathCheckBox.Text = "Use this file as the default at startup";
        _useAsDefaultPathCheckBox.UseVisualStyleBackColor = true;
        // 
        // _autoRotateCheckBox
        // 
        _autoRotateCheckBox.Appearance = Appearance.ToggleSwitch;
        _autoRotateCheckBox.AutoSize = true;
        _settingsPanel.SetColumnSpan(_autoRotateCheckBox, 6);
        _autoRotateCheckBox.Location = new Point(134, 147);
        _autoRotateCheckBox.Margin = new Padding(5);
        _autoRotateCheckBox.Name = "_autoRotateCheckBox";
        _autoRotateCheckBox.Size = new Size(382, 32);
        _autoRotateCheckBox.TabIndex = 5;
        _autoRotateCheckBox.Text = "Rotate themes automatically";
        _autoRotateCheckBox.UseVisualStyleBackColor = true;
        _autoRotateCheckBox.CheckedChanged += OnAutoRotateCheckedChanged;
        // 
        // _dayStartLabel
        // 
        _dayStartLabel.Anchor = AnchorStyles.Left;
        _dayStartLabel.AutoSize = true;
        _dayStartLabel.Location = new Point(3, 195);
        _dayStartLabel.Name = "_dayStartLabel";
        _dayStartLabel.Size = new Size(123, 32);
        _dayStartLabel.TabIndex = 6;
        _dayStartLabel.Text = "Day starts:";
        // 
        // _dayStartPicker
        // 
        _dayStartPicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _dayStartPicker.Location = new Point(132, 191);
        _dayStartPicker.Name = "_dayStartPicker";
        _dayStartPicker.Size = new Size(149, 39);
        _dayStartPicker.TabIndex = 7;
        // 
        // _nightStartLabel
        // 
        _nightStartLabel.Anchor = AnchorStyles.Left;
        _nightStartLabel.AutoSize = true;
        _nightStartLabel.Location = new Point(287, 195);
        _nightStartLabel.Name = "_nightStartLabel";
        _nightStartLabel.Size = new Size(142, 32);
        _nightStartLabel.TabIndex = 8;
        _nightStartLabel.Text = "Night starts:";
        // 
        // _nightStartPicker
        // 
        _nightStartPicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _nightStartPicker.CalendarMonthBackground = Color.Wheat;
        _nightStartPicker.Location = new Point(435, 191);
        _nightStartPicker.Name = "_nightStartPicker";
        _nightStartPicker.Size = new Size(149, 39);
        _nightStartPicker.TabIndex = 9;
        // 
        // _rotationLabel
        // 
        _rotationLabel.Anchor = AnchorStyles.Left;
        _rotationLabel.AutoSize = true;
        _rotationLabel.Location = new Point(590, 195);
        _rotationLabel.Name = "_rotationLabel";
        _rotationLabel.Size = new Size(214, 32);
        _rotationLabel.TabIndex = 10;
        _rotationLabel.Text = "Rotate every (min):";
        // 
        // _rotationMinutesUpDown
        // 
        _rotationMinutesUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _rotationMinutesUpDown.Increment = new decimal(new int[] { 5, 0, 0, 0 });
        _rotationMinutesUpDown.Location = new Point(810, 187);
        _rotationMinutesUpDown.Maximum = new decimal(new int[] { 720, 0, 0, 0 });
        _rotationMinutesUpDown.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
        _rotationMinutesUpDown.Name = "_rotationMinutesUpDown";
        _rotationMinutesUpDown.Size = new Size(149, 48);
        _rotationMinutesUpDown.TabIndex = 11;
        _rotationMinutesUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
        // 
        // _rotationSuffixLabel
        // 
        _rotationSuffixLabel.Anchor = AnchorStyles.Left;
        _rotationSuffixLabel.AutoSize = true;
        _rotationSuffixLabel.Location = new Point(965, 195);
        _rotationSuffixLabel.Name = "_rotationSuffixLabel";
        _rotationSuffixLabel.Size = new Size(100, 32);
        _rotationSuffixLabel.TabIndex = 12;
        _rotationSuffixLabel.Text = "minutes";
        // 
        // _validationLabel
        // 
        _validationLabel.AutoSize = true;
        _settingsPanel.SetColumnSpan(_validationLabel, 7);
        _validationLabel.ForeColor = Color.Firebrick;
        _validationLabel.Location = new Point(3, 241);
        _validationLabel.Margin = new Padding(3, 3, 3, 0);
        _validationLabel.Name = "_validationLabel";
        _validationLabel.Size = new Size(0, 32);
        _validationLabel.TabIndex = 13;
        _validationLabel.Visible = false;
        // 
        // _themeGrid
        // 
        _themeGrid.AllowUserToAddRows = false;
        _themeGrid.AllowUserToDeleteRows = false;
        _themeGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _themeGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _themeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _themeGrid.Columns.AddRange(new DataGridViewColumn[] { _enabledColumn, _dayColumn, _nightColumn, _themeColumn, _sourceColumn, _statusColumn });
        _themeGrid.EditMode = DataGridViewEditMode.EditOnEnter;
        _themeGrid.Location = new Point(15, 339);
        _themeGrid.MultiSelect = false;
        _themeGrid.Name = "_themeGrid";
        _themeGrid.RowHeadersVisible = false;
        _themeGrid.RowHeadersWidth = 82;
        _themeGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _themeGrid.Size = new Size(1070, 330);
        _themeGrid.TabIndex = 2;
        // 
        // _enabledColumn
        // 
        _enabledColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        _enabledColumn.DataPropertyName = "Enabled";
        _enabledColumn.FillWeight = 70F;
        _enabledColumn.HeaderText = "Enabled";
        _enabledColumn.MinimumWidth = 120;
        _enabledColumn.Name = "_enabledColumn";
        _enabledColumn.Width = 120;
        // 
        // _dayColumn
        // 
        _dayColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        _dayColumn.DataPropertyName = "EligibleDuringDay";
        _dayColumn.FillWeight = 70F;
        _dayColumn.HeaderText = "Day";
        _dayColumn.MinimumWidth = 90;
        _dayColumn.Name = "_dayColumn";
        _dayColumn.Width = 90;
        // 
        // _nightColumn
        // 
        _nightColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        _nightColumn.DataPropertyName = "EligibleDuringNight";
        _nightColumn.FillWeight = 70F;
        _nightColumn.HeaderText = "Night";
        _nightColumn.MinimumWidth = 90;
        _nightColumn.Name = "_nightColumn";
        _nightColumn.Width = 90;
        // 
        // _themeColumn
        // 
        _themeColumn.DataPropertyName = "DisplayName";
        _themeColumn.FillWeight = 220F;
        _themeColumn.HeaderText = "Theme family";
        _themeColumn.MinimumWidth = 180;
        _themeColumn.Name = "_themeColumn";
        _themeColumn.ReadOnly = true;
        // 
        // _sourceColumn
        // 
        _sourceColumn.DataPropertyName = "Source";
        _sourceColumn.FillWeight = 140F;
        _sourceColumn.HeaderText = "Source";
        _sourceColumn.MinimumWidth = 120;
        _sourceColumn.Name = "_sourceColumn";
        _sourceColumn.ReadOnly = true;
        // 
        // _statusColumn
        // 
        _statusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        _statusColumn.DataPropertyName = "Status";
        _statusColumn.FillWeight = 120F;
        _statusColumn.HeaderText = "Status";
        _statusColumn.MinimumWidth = 130;
        _statusColumn.Name = "_statusColumn";
        _statusColumn.ReadOnly = true;
        _statusColumn.Width = 130;
        // 
        // _buttonPanel
        // 
        _buttonPanel.Anchor = AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.Controls.Add(_resetDefaultsButton);
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.Location = new Point(567, 675);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(518, 70);
        _buttonPanel.TabIndex = 3;
        _buttonPanel.WrapContents = false;
        // 
        // _resetDefaultsButton
        // 
        _resetDefaultsButton.AutoSize = true;
        _resetDefaultsButton.Location = new Point(3, 3);
        _resetDefaultsButton.Name = "_resetDefaultsButton";
        _resetDefaultsButton.Size = new Size(195, 64);
        _resetDefaultsButton.TabIndex = 0;
        _resetDefaultsButton.Text = "Reset defaults";
        _resetDefaultsButton.UseVisualStyleBackColor = true;
        _resetDefaultsButton.Click += OnResetDefaultsClick;
        // 
        // _okButton
        // 
        _okButton.AutoSize = true;
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Location = new Point(204, 3);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(141, 64);
        _okButton.TabIndex = 1;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OnOkClick;
        // 
        // _cancelButton
        // 
        _cancelButton.AutoSize = true;
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(351, 3);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(164, 64);
        _cancelButton.TabIndex = 2;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // ThemeListEditorDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(1100, 760);
        Controls.Add(_layoutPanel);
        MinimumSize = new Size(900, 660);
        Name = "ThemeListEditorDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Theme list";
        VisualStylesMode = VisualStylesMode.Net11;
        _layoutPanel.ResumeLayout(false);
        _layoutPanel.PerformLayout();
        _settingsPanel.ResumeLayout(false);
        _settingsPanel.PerformLayout();
        ((ISupportInitialize)_rotationMinutesUpDown).EndInit();
        ((ISupportInitialize)_themeGrid).EndInit();
        _buttonPanel.ResumeLayout(false);
        _buttonPanel.PerformLayout();
        ResumeLayout(false);
    }

    private TableLayoutPanel _layoutPanel;
    private Label _introLabel;
    private TableLayoutPanel _settingsPanel;
    private Label _nameLabel;
    private TextBox _nameTextBox;
    private Label _currentPathLabel;
    private Label _currentPathValueLabel;
    private CheckBox _useAsDefaultPathCheckBox;
    private CheckBox _autoRotateCheckBox;
    private Label _dayStartLabel;
    private DateTimePicker _dayStartPicker;
    private Label _nightStartLabel;
    private DateTimePicker _nightStartPicker;
    private Label _rotationLabel;
    private NumericUpDown _rotationMinutesUpDown;
    private Label _rotationSuffixLabel;
    private Label _validationLabel;
    private DataGridView _themeGrid;
    private DataGridViewCheckBoxColumn _enabledColumn;
    private DataGridViewCheckBoxColumn _dayColumn;
    private DataGridViewCheckBoxColumn _nightColumn;
    private DataGridViewTextBoxColumn _themeColumn;
    private DataGridViewTextBoxColumn _sourceColumn;
    private DataGridViewTextBoxColumn _statusColumn;
    private FlowLayoutPanel _buttonPanel;
    private Button _resetDefaultsButton;
    private Button _okButton;
    private Button _cancelButton;
}
