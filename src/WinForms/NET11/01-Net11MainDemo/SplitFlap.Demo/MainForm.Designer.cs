namespace SplitFlap.Demo;

partial class MainForm
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
        _boardTimer = new System.Windows.Forms.Timer(components);
        _clockTimer = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)_board).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_clock).BeginInit();
        _layout.SuspendLayout();
        _bottomBar.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.AutoSize = true;
        _layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_board, 0, 0);
        _layout.Controls.Add(_bottomBar, 0, 1);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.RowCount = 2;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(1100, 500);
        _layout.TabIndex = 0;
        // 
        // _board
        // 
        _board.Anchor = AnchorStyles.Top | AnchorStyles.Left;
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
        _bottomBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _bottomBar.AutoSize = true;
        _bottomBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _bottomBar.Controls.Add(_clock);
        _bottomBar.Controls.Add(_updateButton);
        _bottomBar.Controls.Add(_jamButton);
        _bottomBar.Controls.Add(_autoSizeCheckBox);
        _bottomBar.Controls.Add(_speedComboBox);
        _bottomBar.Controls.Add(_soundCheckBox);
        _bottomBar.Controls.Add(_tuneButton);
        _bottomBar.Location = new Point(3, 400);
        _bottomBar.Name = "_bottomBar";
        _bottomBar.Padding = new Padding(4);
        _bottomBar.Size = new Size(1094, 60);
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
        // _boardTimer
        // 
        _boardTimer.Interval = 9000;
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
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        BackColor = Color.FromArgb(0x14, 0x14, 0x14);
        ClientSize = new Size(1100, 500);
        Controls.Add(_layout);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Departures";
        ((System.ComponentModel.ISupportInitialize)_board).EndInit();
        ((System.ComponentModel.ISupportInitialize)_clock).EndInit();
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        _bottomBar.ResumeLayout(false);
        _bottomBar.PerformLayout();
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
    private System.Windows.Forms.Timer _boardTimer;
    private System.Windows.Forms.Timer _clockTimer;
}
