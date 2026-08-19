using System.ComponentModel;

namespace WarpClock.App;

partial class DisplayOptionsView
{
    private IContainer components;
    private TableLayoutPanel _layoutPanel;
    private GroupBox _tickerGroupBox;
    private TableLayoutPanel _tickerLayoutPanel;
    private CheckBox _tickerEnabledCheckBox;
    private Label _customTextLabel;
    private TextBox _customTextTextBox;
    private Label _sourcesLabel;
    private ListView _sourcesListView;
    private FlowLayoutPanel _reorderButtonPanel;
    private Button _moveUpButton;
    private Button _moveDownButton;
    private GroupBox _visualsGroupBox;
    private TableLayoutPanel _visualsLayoutPanel;
    private CheckBox _showThemeTickerVisualCheckBox;
    private CheckBox _showFractionSecondVisualCheckBox;

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
        _tickerGroupBox = new GroupBox();
        _tickerLayoutPanel = new TableLayoutPanel();
        _tickerEnabledCheckBox = new CheckBox();
        _customTextLabel = new Label();
        _customTextTextBox = new TextBox();
        _sourcesLabel = new Label();
        _sourcesListView = new ListView();
        _reorderButtonPanel = new FlowLayoutPanel();
        _moveUpButton = new Button();
        _moveDownButton = new Button();
        _visualsGroupBox = new GroupBox();
        _visualsLayoutPanel = new TableLayoutPanel();
        _showThemeTickerVisualCheckBox = new CheckBox();
        _showFractionSecondVisualCheckBox = new CheckBox();
        _layoutPanel.SuspendLayout();
        _tickerGroupBox.SuspendLayout();
        _tickerLayoutPanel.SuspendLayout();
        _reorderButtonPanel.SuspendLayout();
        _visualsGroupBox.SuspendLayout();
        _visualsLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 1;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_tickerGroupBox, 0, 0);
        _layoutPanel.Controls.Add(_visualsGroupBox, 0, 1);
        _layoutPanel.Dock = DockStyle.Top;
        _layoutPanel.Location = new Point(12, 12);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.RowCount = 2;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.Size = new Size(736, 442);
        _layoutPanel.TabIndex = 0;
        // 
        // _tickerGroupBox
        // 
        _tickerGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _tickerGroupBox.Controls.Add(_tickerLayoutPanel);
        _tickerGroupBox.Location = new Point(3, 3);
        _tickerGroupBox.Name = "_tickerGroupBox";
        _tickerGroupBox.Padding = new Padding(12);
        _tickerGroupBox.Size = new Size(730, 314);
        _tickerGroupBox.TabIndex = 0;
        _tickerGroupBox.TabStop = false;
        _tickerGroupBox.Text = "Global ticker";
        // 
        // _tickerLayoutPanel
        // 
        _tickerLayoutPanel.ColumnCount = 3;
        _tickerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _tickerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _tickerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _tickerLayoutPanel.Controls.Add(_tickerEnabledCheckBox, 0, 0);
        _tickerLayoutPanel.Controls.Add(_customTextLabel, 0, 1);
        _tickerLayoutPanel.Controls.Add(_customTextTextBox, 1, 1);
        _tickerLayoutPanel.Controls.Add(_sourcesLabel, 0, 2);
        _tickerLayoutPanel.Controls.Add(_sourcesListView, 1, 2);
        _tickerLayoutPanel.Controls.Add(_reorderButtonPanel, 2, 2);
        _tickerLayoutPanel.Dock = DockStyle.Fill;
        _tickerLayoutPanel.Location = new Point(12, 28);
        _tickerLayoutPanel.Name = "_tickerLayoutPanel";
        _tickerLayoutPanel.RowCount = 3;
        _tickerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _tickerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _tickerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _tickerLayoutPanel.Size = new Size(706, 274);
        _tickerLayoutPanel.TabIndex = 0;
        // 
        // _tickerEnabledCheckBox
        // 
        _tickerEnabledCheckBox.AutoSize = true;
        _tickerLayoutPanel.SetColumnSpan(_tickerEnabledCheckBox, 3);
        _tickerEnabledCheckBox.Location = new Point(3, 3);
        _tickerEnabledCheckBox.Name = "_tickerEnabledCheckBox";
        _tickerEnabledCheckBox.Size = new Size(102, 19);
        _tickerEnabledCheckBox.TabIndex = 0;
        _tickerEnabledCheckBox.Text = "Enable ticker";
        _tickerEnabledCheckBox.UseVisualStyleBackColor = true;
        _tickerEnabledCheckBox.CheckedChanged += OnTickerEnabledCheckedChanged;
        // 
        // _customTextLabel
        // 
        _customTextLabel.Anchor = AnchorStyles.Left;
        _customTextLabel.AutoSize = true;
        _customTextLabel.Location = new Point(3, 31);
        _customTextLabel.Name = "_customTextLabel";
        _customTextLabel.Size = new Size(74, 15);
        _customTextLabel.TabIndex = 1;
        _customTextLabel.Text = "Custom text:";
        // 
        // _customTextTextBox
        // 
        _tickerLayoutPanel.SetColumnSpan(_customTextTextBox, 2);
        _customTextTextBox.Location = new Point(83, 27);
        _customTextTextBox.Name = "_customTextTextBox";
        _customTextTextBox.Size = new Size(620, 23);
        _customTextTextBox.TabIndex = 2;
        // 
        // _sourcesLabel
        // 
        _sourcesLabel.Anchor = AnchorStyles.Left;
        _sourcesLabel.AutoSize = true;
        _sourcesLabel.Location = new Point(3, 133);
        _sourcesLabel.Name = "_sourcesLabel";
        _sourcesLabel.Size = new Size(48, 15);
        _sourcesLabel.TabIndex = 3;
        _sourcesLabel.Text = "Sources:";
        // 
        // _sourcesListView
        // 
        _sourcesListView.CheckBoxes = true;
        _sourcesListView.HideSelection = false;
        _sourcesListView.Location = new Point(83, 56);
        _sourcesListView.MultiSelect = false;
        _sourcesListView.Name = "_sourcesListView";
        _sourcesListView.Size = new Size(536, 170);
        _sourcesListView.TabIndex = 4;
        _sourcesListView.UseCompatibleStateImageBehavior = false;
        _sourcesListView.View = View.List;
        _sourcesListView.ItemChecked += OnSourcesListViewItemChecked;
        _sourcesListView.SelectedIndexChanged += OnSourcesListViewSelectedIndexChanged;
        // 
        // _reorderButtonPanel
        // 
        _reorderButtonPanel.AutoSize = true;
        _reorderButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _reorderButtonPanel.Controls.Add(_moveUpButton);
        _reorderButtonPanel.Controls.Add(_moveDownButton);
        _reorderButtonPanel.FlowDirection = FlowDirection.TopDown;
        _reorderButtonPanel.Location = new Point(625, 56);
        _reorderButtonPanel.Name = "_reorderButtonPanel";
        _reorderButtonPanel.Size = new Size(78, 64);
        _reorderButtonPanel.TabIndex = 5;
        // 
        // _moveUpButton
        // 
        _moveUpButton.Location = new Point(3, 3);
        _moveUpButton.Name = "_moveUpButton";
        _moveUpButton.Size = new Size(72, 26);
        _moveUpButton.TabIndex = 0;
        _moveUpButton.Text = "Move up";
        _moveUpButton.UseVisualStyleBackColor = true;
        _moveUpButton.Click += OnMoveUpButtonClick;
        // 
        // _moveDownButton
        // 
        _moveDownButton.Location = new Point(3, 35);
        _moveDownButton.Name = "_moveDownButton";
        _moveDownButton.Size = new Size(72, 26);
        _moveDownButton.TabIndex = 1;
        _moveDownButton.Text = "Move down";
        _moveDownButton.UseVisualStyleBackColor = true;
        _moveDownButton.Click += OnMoveDownButtonClick;
        // 
        // _visualsGroupBox
        // 
        _visualsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _visualsGroupBox.Controls.Add(_visualsLayoutPanel);
        _visualsGroupBox.Location = new Point(3, 323);
        _visualsGroupBox.Name = "_visualsGroupBox";
        _visualsGroupBox.Padding = new Padding(12);
        _visualsGroupBox.Size = new Size(730, 116);
        _visualsGroupBox.TabIndex = 1;
        _visualsGroupBox.TabStop = false;
        _visualsGroupBox.Text = "Theme-gated visuals";
        // 
        // _visualsLayoutPanel
        // 
        _visualsLayoutPanel.ColumnCount = 1;
        _visualsLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _visualsLayoutPanel.Controls.Add(_showThemeTickerVisualCheckBox, 0, 0);
        _visualsLayoutPanel.Controls.Add(_showFractionSecondVisualCheckBox, 0, 1);
        _visualsLayoutPanel.Dock = DockStyle.Fill;
        _visualsLayoutPanel.Location = new Point(12, 28);
        _visualsLayoutPanel.Name = "_visualsLayoutPanel";
        _visualsLayoutPanel.RowCount = 2;
        _visualsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _visualsLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _visualsLayoutPanel.Size = new Size(706, 76);
        _visualsLayoutPanel.TabIndex = 0;
        // 
        // _showThemeTickerVisualCheckBox
        // 
        _showThemeTickerVisualCheckBox.AutoSize = true;
        _showThemeTickerVisualCheckBox.Location = new Point(3, 3);
        _showThemeTickerVisualCheckBox.Name = "_showThemeTickerVisualCheckBox";
        _showThemeTickerVisualCheckBox.Size = new Size(193, 19);
        _showThemeTickerVisualCheckBox.TabIndex = 0;
        _showThemeTickerVisualCheckBox.Text = "Show theme ticker visual when available";
        _showThemeTickerVisualCheckBox.UseVisualStyleBackColor = true;
        // 
        // _showFractionSecondVisualCheckBox
        // 
        _showFractionSecondVisualCheckBox.AutoSize = true;
        _showFractionSecondVisualCheckBox.Location = new Point(3, 28);
        _showFractionSecondVisualCheckBox.Name = "_showFractionSecondVisualCheckBox";
        _showFractionSecondVisualCheckBox.Size = new Size(193, 19);
        _showFractionSecondVisualCheckBox.TabIndex = 1;
        _showFractionSecondVisualCheckBox.Text = "Show fraction-second visual when available";
        _showFractionSecondVisualCheckBox.UseVisualStyleBackColor = true;
        // 
        // DisplayOptionsView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_layoutPanel);
        Name = "DisplayOptionsView";
        Padding = new Padding(12);
        Size = new Size(760, 460);
        _layoutPanel.ResumeLayout(false);
        _tickerGroupBox.ResumeLayout(false);
        _tickerLayoutPanel.ResumeLayout(false);
        _tickerLayoutPanel.PerformLayout();
        _reorderButtonPanel.ResumeLayout(false);
        _visualsGroupBox.ResumeLayout(false);
        _visualsLayoutPanel.ResumeLayout(false);
        _visualsLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
}
