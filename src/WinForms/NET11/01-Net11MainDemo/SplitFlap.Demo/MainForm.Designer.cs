namespace SplitFlap.Demo;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    /// <inheritdoc/>
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
        _layout = new TableLayoutPanel();
        _board = new SplitFlapCharacterDisplay();
        _bottomBar = new FlowLayoutPanel();
        _clock = new SplitFlapCharacterDisplay();
        _updateButton = new Button();
        _jamButton = new Button();
        _autoSizeCheckBox = new CheckBox();
        _speedComboBox = new ComboBox();
        _soundCheckBox = new CheckBox();
        _tuneButton = new Button();
        _menuStrip = new MenuStrip();
        _fileMenuItem = new ToolStripMenuItem();
        _autoSaveSettingsMenuItem = new ToolStripMenuItem();
        _saveSettingsMenuItem = new ToolStripMenuItem();
        _fileSeparator = new ToolStripSeparator();
        _quitMenuItem = new ToolStripMenuItem();
        _viewMenuItem = new ToolStripMenuItem();
        _kioskMenuItem = new ToolStripMenuItem();
        _windowFullScreenMenuItem = new ToolStripMenuItem();
        _viewSeparator = new ToolStripSeparator();
        _fontMenuItem = new ToolStripMenuItem();
        _keepAspectRatioMenuItem = new ToolStripMenuItem();
        _defineGridMenuItem = new ToolStripMenuItem();
        _fitScreenMenuItem = new ToolStripMenuItem();
        _toolsMenuItem = new ToolStripMenuItem();
        _optionsMenuItem = new ToolStripMenuItem();
        _kioskModeManager = new KioskModeManager(components);
        _boardTimer = new System.Windows.Forms.Timer(components);
        _clockTimer = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)_board).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_clock).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_kioskModeManager).BeginInit();
        _layout.SuspendLayout();
        _bottomBar.SuspendLayout();
        _menuStrip.SuspendLayout();
        SuspendLayout();
        //
        // _layout
        //
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_board, 0, 0);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 28);
        _layout.Name = "_layout";
        _layout.RowCount = 1;
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layout.Size = new Size(1100, 412);
        _layout.TabIndex = 0;
        //
        // _board
        //
        _board.Anchor = AnchorStyles.None;
        _board.Columns = 46;
        _board.FontSize = 18F;
        _board.Location = new Point(3, 3);
        _board.Margin = new Padding(3);
        _board.Name = "_board";
        _board.Rows = 9;
        _board.TabIndex = 0;
        //
        // _bottomBar
        //
        _bottomBar.AutoSize = true;
        _bottomBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _bottomBar.Controls.Add(_clock);
        _bottomBar.Controls.Add(_updateButton);
        _bottomBar.Controls.Add(_jamButton);
        _bottomBar.Controls.Add(_autoSizeCheckBox);
        _bottomBar.Controls.Add(_speedComboBox);
        _bottomBar.Controls.Add(_soundCheckBox);
        _bottomBar.Controls.Add(_tuneButton);
        _bottomBar.Dock = DockStyle.Bottom;
        _bottomBar.Location = new Point(0, 440);
        _bottomBar.Name = "_bottomBar";
        _bottomBar.Padding = new Padding(4);
        _bottomBar.Size = new Size(1100, 60);
        _bottomBar.TabIndex = 1;
        _bottomBar.WrapContents = false;
        //
        // _clock
        //
        _clock.Anchor = AnchorStyles.Left;
        _clock.CharacterSet = " 0123456789:";
        _clock.Columns = 5;
        _clock.FlapBackColor = Color.FromArgb(0x2A, 0x2A, 0x2A);
        _clock.FlapForeColor = Color.FromArgb(0xF5, 0xC6, 0x42);
        _clock.FontSize = 24F;
        _clock.Location = new Point(7, 7);
        _clock.Name = "_clock";
        _clock.Padding = new Padding(4);
        _clock.TabIndex = 0;
        //
        // _updateButton
        //
        _updateButton.Anchor = AnchorStyles.Left;
        _updateButton.AutoSize = true;
        _updateButton.Location = new Point(200, 20);
        _updateButton.Margin = new Padding(12, 3, 3, 3);
        _updateButton.Name = "_updateButton";
        _updateButton.Size = new Size(100, 30);
        _updateButton.TabIndex = 1;
        _updateButton.Text = "Next departures";
        _updateButton.UseVisualStyleBackColor = true;
        _updateButton.Click += UpdateButton_Click;
        //
        // _jamButton
        //
        _jamButton.Anchor = AnchorStyles.Left;
        _jamButton.AutoSize = true;
        _jamButton.Location = new Point(310, 20);
        _jamButton.Name = "_jamButton";
        _jamButton.Size = new Size(100, 30);
        _jamButton.TabIndex = 2;
        _jamButton.Text = "Jam something";
        _jamButton.UseVisualStyleBackColor = true;
        _jamButton.Click += JamButton_Click;
        //
        // _autoSizeCheckBox
        //
        _autoSizeCheckBox.Anchor = AnchorStyles.Left;
        _autoSizeCheckBox.AutoSize = true;
        _autoSizeCheckBox.Checked = true;
        _autoSizeCheckBox.CheckState = CheckState.Checked;
        _autoSizeCheckBox.Location = new Point(420, 22);
        _autoSizeCheckBox.Margin = new Padding(12, 3, 3, 3);
        _autoSizeCheckBox.Name = "_autoSizeCheckBox";
        _autoSizeCheckBox.Size = new Size(120, 24);
        _autoSizeCheckBox.TabIndex = 3;
        _autoSizeCheckBox.Text = "Board dictates size";
        _autoSizeCheckBox.UseVisualStyleBackColor = true;
        _autoSizeCheckBox.CheckedChanged += AutoSizeCheckBox_CheckedChanged;
        //
        // _speedComboBox
        //
        _speedComboBox.Anchor = AnchorStyles.Left;
        _speedComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _speedComboBox.Location = new Point(560, 20);
        _speedComboBox.Margin = new Padding(12, 3, 3, 3);
        _speedComboBox.Name = "_speedComboBox";
        _speedComboBox.Size = new Size(140, 28);
        _speedComboBox.TabIndex = 4;
        _speedComboBox.SelectedIndexChanged += SpeedComboBox_SelectedIndexChanged;
        //
        // _soundCheckBox
        //
        _soundCheckBox.Anchor = AnchorStyles.Left;
        _soundCheckBox.AutoSize = true;
        _soundCheckBox.Location = new Point(710, 22);
        _soundCheckBox.Margin = new Padding(12, 3, 3, 3);
        _soundCheckBox.Name = "_soundCheckBox";
        _soundCheckBox.Size = new Size(70, 24);
        _soundCheckBox.TabIndex = 5;
        _soundCheckBox.Text = "Sound";
        _soundCheckBox.UseVisualStyleBackColor = true;
        _soundCheckBox.CheckedChanged += SoundCheckBox_CheckedChanged;
        //
        // _tuneButton
        //
        _tuneButton.Anchor = AnchorStyles.Left;
        _tuneButton.AutoSize = true;
        _tuneButton.Enabled = false;
        _tuneButton.Location = new Point(790, 20);
        _tuneButton.Name = "_tuneButton";
        _tuneButton.Size = new Size(100, 30);
        _tuneButton.TabIndex = 6;
        _tuneButton.Text = "Play a tune";
        _tuneButton.UseVisualStyleBackColor = true;
        _tuneButton.Click += TuneButton_Click;
        //
        // _menuStrip
        //
        _menuStrip.ImageScalingSize = new Size(20, 20);
        _menuStrip.Items.AddRange(new ToolStripItem[] { _fileMenuItem, _viewMenuItem, _toolsMenuItem });
        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Size = new Size(1100, 28);
        _menuStrip.TabIndex = 1;
        //
        // _fileMenuItem
        //
        _fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _autoSaveSettingsMenuItem, _saveSettingsMenuItem, _fileSeparator, _quitMenuItem });
        _fileMenuItem.Name = "_fileMenuItem";
        _fileMenuItem.Size = new Size(46, 24);
        _fileMenuItem.Text = "&File";
        //
        // _autoSaveSettingsMenuItem
        //
        _autoSaveSettingsMenuItem.Checked = true;
        _autoSaveSettingsMenuItem.CheckOnClick = false;
        _autoSaveSettingsMenuItem.CheckState = CheckState.Checked;
        _autoSaveSettingsMenuItem.Name = "_autoSaveSettingsMenuItem";
        _autoSaveSettingsMenuItem.Size = new Size(224, 26);
        _autoSaveSettingsMenuItem.Text = "&Auto-Save Settings";
        _autoSaveSettingsMenuItem.Click += AutoSaveSettingsMenuItem_Click;
        //
        // _saveSettingsMenuItem
        //
        _saveSettingsMenuItem.Name = "_saveSettingsMenuItem";
        _saveSettingsMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        _saveSettingsMenuItem.Size = new Size(224, 26);
        _saveSettingsMenuItem.Text = "&Save Settings";
        _saveSettingsMenuItem.Click += SaveSettingsMenuItem_Click;
        //
        // _fileSeparator
        //
        _fileSeparator.Name = "_fileSeparator";
        _fileSeparator.Size = new Size(221, 6);
        //
        // _quitMenuItem
        //
        _quitMenuItem.Name = "_quitMenuItem";
        _quitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        _quitMenuItem.Size = new Size(224, 26);
        _quitMenuItem.Text = "&Quit";
        _quitMenuItem.Click += QuitMenuItem_Click;
        //
        // _viewMenuItem
        //
        _viewMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _kioskMenuItem, _windowFullScreenMenuItem, _viewSeparator, _fontMenuItem, _keepAspectRatioMenuItem, _defineGridMenuItem, _fitScreenMenuItem });
        _viewMenuItem.Name = "_viewMenuItem";
        _viewMenuItem.Size = new Size(55, 24);
        _viewMenuItem.Text = "&View";
        //
        // _kioskMenuItem
        //
        _kioskMenuItem.Name = "_kioskMenuItem";
        _kioskMenuItem.Size = new Size(286, 26);
        _kioskMenuItem.Text = "Full Screen (&Kiosk Mode)";
        _kioskMenuItem.Click += KioskMenuItem_Click;
        //
        // _windowFullScreenMenuItem
        //
        _windowFullScreenMenuItem.Name = "_windowFullScreenMenuItem";
        _windowFullScreenMenuItem.Size = new Size(286, 26);
        _windowFullScreenMenuItem.Text = "Full Screen (&Window)";
        _windowFullScreenMenuItem.Click += WindowFullScreenMenuItem_Click;
        //
        // _viewSeparator
        //
        _viewSeparator.Name = "_viewSeparator";
        _viewSeparator.Size = new Size(283, 6);
        //
        // _fontMenuItem
        //
        _fontMenuItem.Name = "_fontMenuItem";
        _fontMenuItem.Size = new Size(286, 26);
        _fontMenuItem.Text = "&Font Name and Size...";
        _fontMenuItem.Click += FontMenuItem_Click;
        //
        // _keepAspectRatioMenuItem
        //
        _keepAspectRatioMenuItem.Checked = true;
        _keepAspectRatioMenuItem.CheckState = CheckState.Checked;
        _keepAspectRatioMenuItem.Name = "_keepAspectRatioMenuItem";
        _keepAspectRatioMenuItem.Size = new Size(286, 26);
        _keepAspectRatioMenuItem.Text = "Keep &Aspect Ratio";
        _keepAspectRatioMenuItem.Click += KeepAspectRatioMenuItem_Click;
        //
        // _defineGridMenuItem
        //
        _defineGridMenuItem.Name = "_defineGridMenuItem";
        _defineGridMenuItem.Size = new Size(286, 26);
        _defineGridMenuItem.Text = "Define &Lines/Column Count...";
        _defineGridMenuItem.Click += DefineGridMenuItem_Click;
        //
        // _fitScreenMenuItem
        //
        _fitScreenMenuItem.Name = "_fitScreenMenuItem";
        _fitScreenMenuItem.Size = new Size(286, 26);
        _fitScreenMenuItem.Text = "Fit &Screen Size";
        _fitScreenMenuItem.Click += FitScreenMenuItem_Click;
        //
        // _toolsMenuItem
        //
        _toolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _optionsMenuItem });
        _toolsMenuItem.Name = "_toolsMenuItem";
        _toolsMenuItem.Size = new Size(58, 24);
        _toolsMenuItem.Text = "&Tools";
        //
        // _optionsMenuItem
        //
        _optionsMenuItem.Name = "_optionsMenuItem";
        _optionsMenuItem.Size = new Size(145, 26);
        _optionsMenuItem.Text = "&Options...";
        _optionsMenuItem.Click += OptionsMenuItem_Click;
        //
        // _kioskModeManager
        //
        // KioskModeManager owns all form fullscreen state and restores it when kiosk mode ends.
        _kioskModeManager.ContainerControl = this;
        _kioskModeManager.EscapeExitsFullScreen = true;
        _kioskModeManager.MousePointerAutoHideDelay = 5000;
        _kioskModeManager.ToggleFullScreenKeys = Keys.F11;
        _kioskModeManager.TopMostInFullScreen = true;
        _kioskModeManager.FullScreenChanged += KioskModeManager_FullScreenChanged;
        //
        // _boardTimer
        //
        _boardTimer.Interval = 30000;
        _boardTimer.Tick += BoardTimer_Tick;
        //
        // _clockTimer
        //
        _clockTimer.Interval = 1000;
        _clockTimer.Tick += ClockTimer_Tick;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(0x14, 0x14, 0x14);
        ClientSize = new Size(1100, 500);
        Controls.Add(_layout);
        Controls.Add(_bottomBar);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Departures";
        ((System.ComponentModel.ISupportInitialize)_board).EndInit();
        ((System.ComponentModel.ISupportInitialize)_clock).EndInit();
        ((System.ComponentModel.ISupportInitialize)_kioskModeManager).EndInit();
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        _bottomBar.ResumeLayout(false);
        _bottomBar.PerformLayout();
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _layout;
    private SplitFlapCharacterDisplay _board;
    private FlowLayoutPanel _bottomBar;
    private SplitFlapCharacterDisplay _clock;
    private Button _updateButton;
    private Button _jamButton;
    private CheckBox _autoSizeCheckBox;
    private ComboBox _speedComboBox;
    private CheckBox _soundCheckBox;
    private Button _tuneButton;
    private MenuStrip _menuStrip;
    private ToolStripMenuItem _fileMenuItem;
    private ToolStripMenuItem _autoSaveSettingsMenuItem;
    private ToolStripMenuItem _saveSettingsMenuItem;
    private ToolStripSeparator _fileSeparator;
    private ToolStripMenuItem _quitMenuItem;
    private ToolStripMenuItem _viewMenuItem;
    private ToolStripMenuItem _kioskMenuItem;
    private ToolStripMenuItem _windowFullScreenMenuItem;
    private ToolStripSeparator _viewSeparator;
    private ToolStripMenuItem _fontMenuItem;
    private ToolStripMenuItem _keepAspectRatioMenuItem;
    private ToolStripMenuItem _defineGridMenuItem;
    private ToolStripMenuItem _fitScreenMenuItem;
    private ToolStripMenuItem _toolsMenuItem;
    private ToolStripMenuItem _optionsMenuItem;
    private KioskModeManager _kioskModeManager;
    private System.Windows.Forms.Timer _boardTimer;
    private System.Windows.Forms.Timer _clockTimer;
}
