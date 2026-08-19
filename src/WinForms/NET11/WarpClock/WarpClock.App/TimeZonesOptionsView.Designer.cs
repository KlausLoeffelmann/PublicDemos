using System.ComponentModel;

namespace WarpClock.App;

partial class TimeZonesOptionsView
{
    private IContainer components;
    private TableLayoutPanel _layoutPanel;
    private GroupBox _settingsGroupBox;
    private TableLayoutPanel _settingsLayoutPanel;
    private CheckBox _enabledCheckBox;
    private Label _changeEveryLabel;
    private NumericUpDown _changeEveryNumericUpDown;
    private Label _changeEverySuffixLabel;
    private Label _returnLabel;
    private NumericUpDown _returnNumericUpDown;
    private Label _returnSuffixLabel;
    private CheckBox _showOnClockFaceCheckBox;
    private CheckBox _showOnlyWhenAlternateCheckBox;
    private CheckBox _showHeadlineFallbackCheckBox;
    private GroupBox _zonesGroupBox;
    private TableLayoutPanel _zonesLayoutPanel;
    private Label _defaultHeaderLabel;
    private Label _timeZoneHeaderLabel;
    private Label _aliasHeaderLabel;
    private Label _row1Label;
    private Label _row2Label;
    private Label _row3Label;
    private Label _row4Label;
    private Label _row5Label;
    private Label _row6Label;
    private RadioButton _default1RadioButton;
    private RadioButton _default2RadioButton;
    private RadioButton _default3RadioButton;
    private RadioButton _default4RadioButton;
    private RadioButton _default5RadioButton;
    private RadioButton _default6RadioButton;
    private ComboBox _timeZone1ComboBox;
    private ComboBox _timeZone2ComboBox;
    private ComboBox _timeZone3ComboBox;
    private ComboBox _timeZone4ComboBox;
    private ComboBox _timeZone5ComboBox;
    private ComboBox _timeZone6ComboBox;
    private TextBox _alias1TextBox;
    private TextBox _alias2TextBox;
    private TextBox _alias3TextBox;
    private TextBox _alias4TextBox;
    private TextBox _alias5TextBox;
    private TextBox _alias6TextBox;

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
        components = new Container();
        _layoutPanel = new TableLayoutPanel();
        _settingsGroupBox = new GroupBox();
        _settingsLayoutPanel = new TableLayoutPanel();
        _enabledCheckBox = new CheckBox();
        _changeEveryLabel = new Label();
        _changeEveryNumericUpDown = new NumericUpDown();
        _changeEverySuffixLabel = new Label();
        _returnLabel = new Label();
        _returnNumericUpDown = new NumericUpDown();
        _returnSuffixLabel = new Label();
        _showOnClockFaceCheckBox = new CheckBox();
        _showOnlyWhenAlternateCheckBox = new CheckBox();
        _showHeadlineFallbackCheckBox = new CheckBox();
        _zonesGroupBox = new GroupBox();
        _zonesLayoutPanel = new TableLayoutPanel();
        _defaultHeaderLabel = new Label();
        _timeZoneHeaderLabel = new Label();
        _aliasHeaderLabel = new Label();
        _row1Label = new Label();
        _row2Label = new Label();
        _row3Label = new Label();
        _row4Label = new Label();
        _row5Label = new Label();
        _row6Label = new Label();
        _default1RadioButton = new RadioButton();
        _default2RadioButton = new RadioButton();
        _default3RadioButton = new RadioButton();
        _default4RadioButton = new RadioButton();
        _default5RadioButton = new RadioButton();
        _default6RadioButton = new RadioButton();
        _timeZone1ComboBox = new ComboBox();
        _timeZone2ComboBox = new ComboBox();
        _timeZone3ComboBox = new ComboBox();
        _timeZone4ComboBox = new ComboBox();
        _timeZone5ComboBox = new ComboBox();
        _timeZone6ComboBox = new ComboBox();
        _alias1TextBox = new TextBox();
        _alias2TextBox = new TextBox();
        _alias3TextBox = new TextBox();
        _alias4TextBox = new TextBox();
        _alias5TextBox = new TextBox();
        _alias6TextBox = new TextBox();
        _layoutPanel.SuspendLayout();
        _settingsGroupBox.SuspendLayout();
        _settingsLayoutPanel.SuspendLayout();
        ((ISupportInitialize)_changeEveryNumericUpDown).BeginInit();
        ((ISupportInitialize)_returnNumericUpDown).BeginInit();
        _zonesGroupBox.SuspendLayout();
        _zonesLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 1;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_settingsGroupBox, 0, 0);
        _layoutPanel.Controls.Add(_zonesGroupBox, 0, 1);
        _layoutPanel.Dock = DockStyle.Top;
        _layoutPanel.Location = new Point(12, 12);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.RowCount = 2;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.Size = new Size(876, 596);
        _layoutPanel.TabIndex = 0;
        // 
        // _settingsGroupBox
        // 
        _settingsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _settingsGroupBox.Controls.Add(_settingsLayoutPanel);
        _settingsGroupBox.Location = new Point(3, 3);
        _settingsGroupBox.Name = "_settingsGroupBox";
        _settingsGroupBox.Padding = new Padding(12);
        _settingsGroupBox.Size = new Size(870, 177);
        _settingsGroupBox.TabIndex = 0;
        _settingsGroupBox.TabStop = false;
        _settingsGroupBox.Text = "Rotation settings";
        // 
        // _settingsLayoutPanel
        // 
        _settingsLayoutPanel.ColumnCount = 6;
        _settingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _settingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _settingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _settingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _settingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _settingsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _settingsLayoutPanel.Controls.Add(_enabledCheckBox, 0, 0);
        _settingsLayoutPanel.Controls.Add(_changeEveryLabel, 0, 1);
        _settingsLayoutPanel.Controls.Add(_changeEveryNumericUpDown, 1, 1);
        _settingsLayoutPanel.Controls.Add(_changeEverySuffixLabel, 2, 1);
        _settingsLayoutPanel.Controls.Add(_returnLabel, 3, 1);
        _settingsLayoutPanel.Controls.Add(_returnNumericUpDown, 4, 1);
        _settingsLayoutPanel.Controls.Add(_returnSuffixLabel, 5, 1);
        _settingsLayoutPanel.Controls.Add(_showOnClockFaceCheckBox, 0, 2);
        _settingsLayoutPanel.Controls.Add(_showOnlyWhenAlternateCheckBox, 1, 2);
        _settingsLayoutPanel.Controls.Add(_showHeadlineFallbackCheckBox, 2, 2);
        _settingsLayoutPanel.Dock = DockStyle.Fill;
        _settingsLayoutPanel.Location = new Point(12, 28);
        _settingsLayoutPanel.Name = "_settingsLayoutPanel";
        _settingsLayoutPanel.RowCount = 3;
        _settingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _settingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _settingsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _settingsLayoutPanel.Size = new Size(846, 137);
        _settingsLayoutPanel.TabIndex = 0;
        // 
        // _enabledCheckBox
        // 
        _enabledCheckBox.AutoSize = true;
        _settingsLayoutPanel.SetColumnSpan(_enabledCheckBox, 6);
        _enabledCheckBox.Location = new Point(3, 3);
        _enabledCheckBox.Name = "_enabledCheckBox";
        _enabledCheckBox.Size = new Size(164, 19);
        _enabledCheckBox.TabIndex = 0;
        _enabledCheckBox.Text = "Rotate between time zones";
        _enabledCheckBox.UseVisualStyleBackColor = true;
        _enabledCheckBox.CheckedChanged += OnEnabledCheckBoxCheckedChanged;
        // 
        // _changeEveryLabel
        // 
        _changeEveryLabel.Anchor = AnchorStyles.Left;
        _changeEveryLabel.AutoSize = true;
        _changeEveryLabel.Location = new Point(3, 31);
        _changeEveryLabel.Name = "_changeEveryLabel";
        _changeEveryLabel.Size = new Size(81, 15);
        _changeEveryLabel.TabIndex = 1;
        _changeEveryLabel.Text = "Change every:";
        // 
        // _changeEveryNumericUpDown
        // 
        _changeEveryNumericUpDown.Anchor = AnchorStyles.Left;
        _changeEveryNumericUpDown.Increment = new decimal(new int[] { 10, 0, 0, 0 });
        _changeEveryNumericUpDown.Location = new Point(90, 27);
        _changeEveryNumericUpDown.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
        _changeEveryNumericUpDown.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
        _changeEveryNumericUpDown.Name = "_changeEveryNumericUpDown";
        _changeEveryNumericUpDown.Size = new Size(72, 23);
        _changeEveryNumericUpDown.TabIndex = 2;
        _changeEveryNumericUpDown.Value = new decimal(new int[] { 60, 0, 0, 0 });
        // 
        // _changeEverySuffixLabel
        // 
        _changeEverySuffixLabel.Anchor = AnchorStyles.Left;
        _changeEverySuffixLabel.AutoSize = true;
        _changeEverySuffixLabel.Location = new Point(168, 31);
        _changeEverySuffixLabel.Name = "_changeEverySuffixLabel";
        _changeEverySuffixLabel.Size = new Size(49, 15);
        _changeEverySuffixLabel.TabIndex = 3;
        _changeEverySuffixLabel.Text = "seconds";
        // 
        // _returnLabel
        // 
        _returnLabel.Anchor = AnchorStyles.Left;
        _returnLabel.AutoSize = true;
        _returnLabel.Location = new Point(223, 31);
        _returnLabel.Name = "_returnLabel";
        _returnLabel.Size = new Size(100, 15);
        _returnLabel.TabIndex = 4;
        _returnLabel.Text = "Return to default:";
        // 
        // _returnNumericUpDown
        // 
        _returnNumericUpDown.Anchor = AnchorStyles.Left;
        _returnNumericUpDown.Increment = new decimal(new int[] { 5, 0, 0, 0 });
        _returnNumericUpDown.Location = new Point(329, 27);
        _returnNumericUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        _returnNumericUpDown.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
        _returnNumericUpDown.Name = "_returnNumericUpDown";
        _returnNumericUpDown.Size = new Size(72, 23);
        _returnNumericUpDown.TabIndex = 5;
        _returnNumericUpDown.Value = new decimal(new int[] { 20, 0, 0, 0 });
        // 
        // _returnSuffixLabel
        // 
        _returnSuffixLabel.Anchor = AnchorStyles.Left;
        _returnSuffixLabel.AutoSize = true;
        _returnSuffixLabel.Location = new Point(407, 31);
        _returnSuffixLabel.Name = "_returnSuffixLabel";
        _returnSuffixLabel.Size = new Size(49, 15);
        _returnSuffixLabel.TabIndex = 6;
        _returnSuffixLabel.Text = "seconds";
        // 
        // _showOnClockFaceCheckBox
        // 
        _showOnClockFaceCheckBox.AutoSize = true;
        _showOnClockFaceCheckBox.Location = new Point(3, 56);
        _showOnClockFaceCheckBox.Name = "_showOnClockFaceCheckBox";
        _showOnClockFaceCheckBox.Size = new Size(132, 19);
        _showOnClockFaceCheckBox.TabIndex = 7;
        _showOnClockFaceCheckBox.Text = "Show on clock face";
        _showOnClockFaceCheckBox.UseVisualStyleBackColor = true;
        _showOnClockFaceCheckBox.CheckedChanged += OnShowOnClockFaceCheckedChanged;
        // 
        // _showOnlyWhenAlternateCheckBox
        // 
        _showOnlyWhenAlternateCheckBox.AutoSize = true;
        _showOnlyWhenAlternateCheckBox.Location = new Point(141, 56);
        _showOnlyWhenAlternateCheckBox.Name = "_showOnlyWhenAlternateCheckBox";
        _showOnlyWhenAlternateCheckBox.Size = new Size(162, 19);
        _showOnlyWhenAlternateCheckBox.TabIndex = 8;
        _showOnlyWhenAlternateCheckBox.Text = "Only when alternate zone";
        _showOnlyWhenAlternateCheckBox.UseVisualStyleBackColor = true;
        // 
        // _showHeadlineFallbackCheckBox
        // 
        _showHeadlineFallbackCheckBox.AutoSize = true;
        _showHeadlineFallbackCheckBox.Location = new Point(309, 56);
        _showHeadlineFallbackCheckBox.Name = "_showHeadlineFallbackCheckBox";
        _showHeadlineFallbackCheckBox.Size = new Size(153, 19);
        _showHeadlineFallbackCheckBox.TabIndex = 9;
        _showHeadlineFallbackCheckBox.Text = "Show headline fallback";
        _showHeadlineFallbackCheckBox.UseVisualStyleBackColor = true;
        // 
        // _zonesGroupBox
        // 
        _zonesGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _zonesGroupBox.Controls.Add(_zonesLayoutPanel);
        _zonesGroupBox.Location = new Point(3, 186);
        _zonesGroupBox.Name = "_zonesGroupBox";
        _zonesGroupBox.Padding = new Padding(12);
        _zonesGroupBox.Size = new Size(870, 407);
        _zonesGroupBox.TabIndex = 1;
        _zonesGroupBox.TabStop = false;
        _zonesGroupBox.Text = "Configured time zones (up to six)";
        // 
        // _zonesLayoutPanel
        // 
        _zonesLayoutPanel.ColumnCount = 4;
        _zonesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _zonesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _zonesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        _zonesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        _zonesLayoutPanel.Controls.Add(_defaultHeaderLabel, 1, 0);
        _zonesLayoutPanel.Controls.Add(_timeZoneHeaderLabel, 2, 0);
        _zonesLayoutPanel.Controls.Add(_aliasHeaderLabel, 3, 0);
        _zonesLayoutPanel.Controls.Add(_row1Label, 0, 1);
        _zonesLayoutPanel.Controls.Add(_row2Label, 0, 2);
        _zonesLayoutPanel.Controls.Add(_row3Label, 0, 3);
        _zonesLayoutPanel.Controls.Add(_row4Label, 0, 4);
        _zonesLayoutPanel.Controls.Add(_row5Label, 0, 5);
        _zonesLayoutPanel.Controls.Add(_row6Label, 0, 6);
        _zonesLayoutPanel.Controls.Add(_default1RadioButton, 1, 1);
        _zonesLayoutPanel.Controls.Add(_default2RadioButton, 1, 2);
        _zonesLayoutPanel.Controls.Add(_default3RadioButton, 1, 3);
        _zonesLayoutPanel.Controls.Add(_default4RadioButton, 1, 4);
        _zonesLayoutPanel.Controls.Add(_default5RadioButton, 1, 5);
        _zonesLayoutPanel.Controls.Add(_default6RadioButton, 1, 6);
        _zonesLayoutPanel.Controls.Add(_timeZone1ComboBox, 2, 1);
        _zonesLayoutPanel.Controls.Add(_timeZone2ComboBox, 2, 2);
        _zonesLayoutPanel.Controls.Add(_timeZone3ComboBox, 2, 3);
        _zonesLayoutPanel.Controls.Add(_timeZone4ComboBox, 2, 4);
        _zonesLayoutPanel.Controls.Add(_timeZone5ComboBox, 2, 5);
        _zonesLayoutPanel.Controls.Add(_timeZone6ComboBox, 2, 6);
        _zonesLayoutPanel.Controls.Add(_alias1TextBox, 3, 1);
        _zonesLayoutPanel.Controls.Add(_alias2TextBox, 3, 2);
        _zonesLayoutPanel.Controls.Add(_alias3TextBox, 3, 3);
        _zonesLayoutPanel.Controls.Add(_alias4TextBox, 3, 4);
        _zonesLayoutPanel.Controls.Add(_alias5TextBox, 3, 5);
        _zonesLayoutPanel.Controls.Add(_alias6TextBox, 3, 6);
        _zonesLayoutPanel.Dock = DockStyle.Fill;
        _zonesLayoutPanel.Location = new Point(12, 28);
        _zonesLayoutPanel.Name = "_zonesLayoutPanel";
        _zonesLayoutPanel.RowCount = 7;
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _zonesLayoutPanel.Size = new Size(846, 367);
        _zonesLayoutPanel.TabIndex = 0;
        // 
        // _defaultHeaderLabel
        // 
        _defaultHeaderLabel.Anchor = AnchorStyles.Left;
        _defaultHeaderLabel.AutoSize = true;
        _defaultHeaderLabel.Location = new Point(30, 3);
        _defaultHeaderLabel.Name = "_defaultHeaderLabel";
        _defaultHeaderLabel.Size = new Size(45, 15);
        _defaultHeaderLabel.TabIndex = 0;
        _defaultHeaderLabel.Text = "Default";
        // 
        // _timeZoneHeaderLabel
        // 
        _timeZoneHeaderLabel.Anchor = AnchorStyles.Left;
        _timeZoneHeaderLabel.AutoSize = true;
        _timeZoneHeaderLabel.Location = new Point(84, 3);
        _timeZoneHeaderLabel.Name = "_timeZoneHeaderLabel";
        _timeZoneHeaderLabel.Size = new Size(63, 15);
        _timeZoneHeaderLabel.TabIndex = 1;
        _timeZoneHeaderLabel.Text = "Time zone";
        // 
        // _aliasHeaderLabel
        // 
        _aliasHeaderLabel.Anchor = AnchorStyles.Left;
        _aliasHeaderLabel.AutoSize = true;
        _aliasHeaderLabel.Location = new Point(520, 3);
        _aliasHeaderLabel.Name = "_aliasHeaderLabel";
        _aliasHeaderLabel.Size = new Size(31, 15);
        _aliasHeaderLabel.TabIndex = 2;
        _aliasHeaderLabel.Text = "Alias";
        // 
        // _row1Label
        // 
        _row1Label.Anchor = AnchorStyles.Left;
        _row1Label.AutoSize = true;
        _row1Label.Location = new Point(3, 31);
        _row1Label.Name = "_row1Label";
        _row1Label.Size = new Size(21, 15);
        _row1Label.TabIndex = 3;
        _row1Label.Text = "1.";
        // 
        // _row2Label
        // 
        _row2Label.Anchor = AnchorStyles.Left;
        _row2Label.AutoSize = true;
        _row2Label.Location = new Point(3, 60);
        _row2Label.Name = "_row2Label";
        _row2Label.Size = new Size(21, 15);
        _row2Label.TabIndex = 4;
        _row2Label.Text = "2.";
        // 
        // _row3Label
        // 
        _row3Label.Anchor = AnchorStyles.Left;
        _row3Label.AutoSize = true;
        _row3Label.Location = new Point(3, 89);
        _row3Label.Name = "_row3Label";
        _row3Label.Size = new Size(21, 15);
        _row3Label.TabIndex = 5;
        _row3Label.Text = "3.";
        // 
        // _row4Label
        // 
        _row4Label.Anchor = AnchorStyles.Left;
        _row4Label.AutoSize = true;
        _row4Label.Location = new Point(3, 118);
        _row4Label.Name = "_row4Label";
        _row4Label.Size = new Size(21, 15);
        _row4Label.TabIndex = 6;
        _row4Label.Text = "4.";
        // 
        // _row5Label
        // 
        _row5Label.Anchor = AnchorStyles.Left;
        _row5Label.AutoSize = true;
        _row5Label.Location = new Point(3, 147);
        _row5Label.Name = "_row5Label";
        _row5Label.Size = new Size(21, 15);
        _row5Label.TabIndex = 7;
        _row5Label.Text = "5.";
        // 
        // _row6Label
        // 
        _row6Label.Anchor = AnchorStyles.Left;
        _row6Label.AutoSize = true;
        _row6Label.Location = new Point(3, 176);
        _row6Label.Name = "_row6Label";
        _row6Label.Size = new Size(21, 15);
        _row6Label.TabIndex = 8;
        _row6Label.Text = "6.";
        // 
        // _default1RadioButton
        // 
        _default1RadioButton.Anchor = AnchorStyles.Left;
        _default1RadioButton.AutoSize = true;
        _default1RadioButton.Location = new Point(30, 29);
        _default1RadioButton.Name = "_default1RadioButton";
        _default1RadioButton.Size = new Size(14, 13);
        _default1RadioButton.TabIndex = 9;
        _default1RadioButton.TabStop = true;
        _default1RadioButton.UseVisualStyleBackColor = true;
        // 
        // _default2RadioButton
        // 
        _default2RadioButton.Anchor = AnchorStyles.Left;
        _default2RadioButton.AutoSize = true;
        _default2RadioButton.Location = new Point(30, 58);
        _default2RadioButton.Name = "_default2RadioButton";
        _default2RadioButton.Size = new Size(14, 13);
        _default2RadioButton.TabIndex = 10;
        _default2RadioButton.TabStop = true;
        _default2RadioButton.UseVisualStyleBackColor = true;
        // 
        // _default3RadioButton
        // 
        _default3RadioButton.Anchor = AnchorStyles.Left;
        _default3RadioButton.AutoSize = true;
        _default3RadioButton.Location = new Point(30, 87);
        _default3RadioButton.Name = "_default3RadioButton";
        _default3RadioButton.Size = new Size(14, 13);
        _default3RadioButton.TabIndex = 11;
        _default3RadioButton.TabStop = true;
        _default3RadioButton.UseVisualStyleBackColor = true;
        // 
        // _default4RadioButton
        // 
        _default4RadioButton.Anchor = AnchorStyles.Left;
        _default4RadioButton.AutoSize = true;
        _default4RadioButton.Location = new Point(30, 116);
        _default4RadioButton.Name = "_default4RadioButton";
        _default4RadioButton.Size = new Size(14, 13);
        _default4RadioButton.TabIndex = 12;
        _default4RadioButton.TabStop = true;
        _default4RadioButton.UseVisualStyleBackColor = true;
        // 
        // _default5RadioButton
        // 
        _default5RadioButton.Anchor = AnchorStyles.Left;
        _default5RadioButton.AutoSize = true;
        _default5RadioButton.Location = new Point(30, 145);
        _default5RadioButton.Name = "_default5RadioButton";
        _default5RadioButton.Size = new Size(14, 13);
        _default5RadioButton.TabIndex = 13;
        _default5RadioButton.TabStop = true;
        _default5RadioButton.UseVisualStyleBackColor = true;
        // 
        // _default6RadioButton
        // 
        _default6RadioButton.Anchor = AnchorStyles.Left;
        _default6RadioButton.AutoSize = true;
        _default6RadioButton.Location = new Point(30, 174);
        _default6RadioButton.Name = "_default6RadioButton";
        _default6RadioButton.Size = new Size(14, 13);
        _default6RadioButton.TabIndex = 14;
        _default6RadioButton.TabStop = true;
        _default6RadioButton.UseVisualStyleBackColor = true;
        // 
        // _timeZone1ComboBox
        // 
        _timeZone1ComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZone1ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZone1ComboBox.FormattingEnabled = true;
        _timeZone1ComboBox.Location = new Point(84, 27);
        _timeZone1ComboBox.Name = "_timeZone1ComboBox";
        _timeZone1ComboBox.Size = new Size(430, 23);
        _timeZone1ComboBox.TabIndex = 15;
        // 
        // _timeZone2ComboBox
        // 
        _timeZone2ComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZone2ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZone2ComboBox.FormattingEnabled = true;
        _timeZone2ComboBox.Location = new Point(84, 56);
        _timeZone2ComboBox.Name = "_timeZone2ComboBox";
        _timeZone2ComboBox.Size = new Size(430, 23);
        _timeZone2ComboBox.TabIndex = 16;
        // 
        // _timeZone3ComboBox
        // 
        _timeZone3ComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZone3ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZone3ComboBox.FormattingEnabled = true;
        _timeZone3ComboBox.Location = new Point(84, 85);
        _timeZone3ComboBox.Name = "_timeZone3ComboBox";
        _timeZone3ComboBox.Size = new Size(430, 23);
        _timeZone3ComboBox.TabIndex = 17;
        // 
        // _timeZone4ComboBox
        // 
        _timeZone4ComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZone4ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZone4ComboBox.FormattingEnabled = true;
        _timeZone4ComboBox.Location = new Point(84, 114);
        _timeZone4ComboBox.Name = "_timeZone4ComboBox";
        _timeZone4ComboBox.Size = new Size(430, 23);
        _timeZone4ComboBox.TabIndex = 18;
        // 
        // _timeZone5ComboBox
        // 
        _timeZone5ComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZone5ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZone5ComboBox.FormattingEnabled = true;
        _timeZone5ComboBox.Location = new Point(84, 143);
        _timeZone5ComboBox.Name = "_timeZone5ComboBox";
        _timeZone5ComboBox.Size = new Size(430, 23);
        _timeZone5ComboBox.TabIndex = 19;
        // 
        // _timeZone6ComboBox
        // 
        _timeZone6ComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZone6ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZone6ComboBox.FormattingEnabled = true;
        _timeZone6ComboBox.Location = new Point(84, 172);
        _timeZone6ComboBox.Name = "_timeZone6ComboBox";
        _timeZone6ComboBox.Size = new Size(430, 23);
        _timeZone6ComboBox.TabIndex = 20;
        // 
        // _alias1TextBox
        // 
        _alias1TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _alias1TextBox.Location = new Point(520, 27);
        _alias1TextBox.Name = "_alias1TextBox";
        _alias1TextBox.Size = new Size(323, 23);
        _alias1TextBox.TabIndex = 21;
        // 
        // _alias2TextBox
        // 
        _alias2TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _alias2TextBox.Location = new Point(520, 56);
        _alias2TextBox.Name = "_alias2TextBox";
        _alias2TextBox.Size = new Size(323, 23);
        _alias2TextBox.TabIndex = 22;
        // 
        // _alias3TextBox
        // 
        _alias3TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _alias3TextBox.Location = new Point(520, 85);
        _alias3TextBox.Name = "_alias3TextBox";
        _alias3TextBox.Size = new Size(323, 23);
        _alias3TextBox.TabIndex = 23;
        // 
        // _alias4TextBox
        // 
        _alias4TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _alias4TextBox.Location = new Point(520, 114);
        _alias4TextBox.Name = "_alias4TextBox";
        _alias4TextBox.Size = new Size(323, 23);
        _alias4TextBox.TabIndex = 24;
        // 
        // _alias5TextBox
        // 
        _alias5TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _alias5TextBox.Location = new Point(520, 143);
        _alias5TextBox.Name = "_alias5TextBox";
        _alias5TextBox.Size = new Size(323, 23);
        _alias5TextBox.TabIndex = 25;
        // 
        // _alias6TextBox
        // 
        _alias6TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _alias6TextBox.Location = new Point(520, 172);
        _alias6TextBox.Name = "_alias6TextBox";
        _alias6TextBox.Size = new Size(323, 23);
        _alias6TextBox.TabIndex = 26;
        // 
        // TimeZonesOptionsView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_layoutPanel);
        Name = "TimeZonesOptionsView";
        Padding = new Padding(12);
        Size = new Size(900, 620);
        _layoutPanel.ResumeLayout(false);
        _settingsGroupBox.ResumeLayout(false);
        _settingsLayoutPanel.ResumeLayout(false);
        _settingsLayoutPanel.PerformLayout();
        ((ISupportInitialize)_changeEveryNumericUpDown).EndInit();
        ((ISupportInitialize)_returnNumericUpDown).EndInit();
        _zonesGroupBox.ResumeLayout(false);
        _zonesLayoutPanel.ResumeLayout(false);
        _zonesLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
}
