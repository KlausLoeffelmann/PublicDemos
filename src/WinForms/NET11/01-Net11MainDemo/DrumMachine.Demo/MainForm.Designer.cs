namespace DrumMachine.Demo;

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
        _layout = new TableLayoutPanel();
        _transport = new FlowLayoutPanel();
        _playButton = new Button();
        _stopButton = new Button();
        _loopCheckBox = new CheckBox();
        _tempoLabel = new Label();
        _tempo = new NumericUpDown();
        _volumeLabel = new Label();
        _volume = new TrackBar();
        _metallicLabel = new Label();
        _metallic = new TrackBar();
        _metallicButton = new Button();
        _spectrumControl = new SplitFlap.Audio.WinForms.AudioSpectrumControl();
        _scoreToolbar = new FlowLayoutPanel();
        _barLabel = new Label();
        _barSelector = new ComboBox();
        _resetButton = new Button();
        _positionLabel = new Label();
        _stepGrid = new DataGridView();
        _instrumentColumn = new DataGridViewTextBoxColumn();
        _auditionColumn = new DataGridViewButtonColumn();
        _step01 = new DataGridViewCheckBoxColumn();
        _step02 = new DataGridViewCheckBoxColumn();
        _step03 = new DataGridViewCheckBoxColumn();
        _step04 = new DataGridViewCheckBoxColumn();
        _step05 = new DataGridViewCheckBoxColumn();
        _step06 = new DataGridViewCheckBoxColumn();
        _step07 = new DataGridViewCheckBoxColumn();
        _step08 = new DataGridViewCheckBoxColumn();
        _step09 = new DataGridViewCheckBoxColumn();
        _step10 = new DataGridViewCheckBoxColumn();
        _step11 = new DataGridViewCheckBoxColumn();
        _step12 = new DataGridViewCheckBoxColumn();
        _step13 = new DataGridViewCheckBoxColumn();
        _step14 = new DataGridViewCheckBoxColumn();
        _step15 = new DataGridViewCheckBoxColumn();
        _step16 = new DataGridViewCheckBoxColumn();
        _statusLabel = new Label();
        components = new System.ComponentModel.Container();
        _uiTimer = new System.Windows.Forms.Timer(components);
        _exitTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        _layout.SuspendLayout();
        _transport.SuspendLayout();
        _scoreToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_tempo).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_volume).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_metallic).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_stepGrid).BeginInit();
        //
        // _layout
        //
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_transport, 0, 0);
        _layout.Controls.Add(_spectrumControl, 0, 1);
        _layout.Controls.Add(_scoreToolbar, 0, 2);
        _layout.Controls.Add(_stepGrid, 0, 3);
        _layout.Controls.Add(_statusLabel, 0, 4);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(12);
        _layout.RowCount = 5;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(1100, 820);
        _layout.TabIndex = 0;
        //
        // _transport
        //
        _transport.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _transport.AutoSize = true;
        _transport.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _transport.Controls.Add(_playButton);
        _transport.Controls.Add(_stopButton);
        _transport.Controls.Add(_loopCheckBox);
        _transport.Controls.Add(_tempoLabel);
        _transport.Controls.Add(_tempo);
        _transport.Controls.Add(_volumeLabel);
        _transport.Controls.Add(_volume);
        _transport.Controls.Add(_metallicLabel);
        _transport.Controls.Add(_metallic);
        _transport.Controls.Add(_metallicButton);
        _transport.Location = new Point(15, 15);
        _transport.Name = "_transport";
        _transport.Size = new Size(1070, 51);
        _transport.TabIndex = 0;
        //
        // _playButton
        //
        _playButton.AutoSize = true;
        _playButton.Enabled = false;
        _playButton.Location = new Point(3, 3);
        _playButton.MinimumSize = new Size(75, 28);
        _playButton.Name = "_playButton";
        _playButton.Size = new Size(75, 28);
        _playButton.TabIndex = 0;
        _playButton.Text = "&Play";
        _playButton.UseVisualStyleBackColor = true;
        _playButton.Click += PlayButton_Click;
        //
        // _stopButton
        //
        _stopButton.AutoSize = true;
        _stopButton.Enabled = false;
        _stopButton.Location = new Point(84, 3);
        _stopButton.MinimumSize = new Size(75, 28);
        _stopButton.Name = "_stopButton";
        _stopButton.Size = new Size(75, 28);
        _stopButton.TabIndex = 1;
        _stopButton.Text = "&Stop";
        _stopButton.UseVisualStyleBackColor = true;
        _stopButton.Click += StopButton_Click;
        //
        // _loopCheckBox
        //
        _loopCheckBox.AutoSize = true;
        _loopCheckBox.Checked = true;
        _loopCheckBox.CheckState = CheckState.Checked;
        _loopCheckBox.Location = new Point(165, 8);
        _loopCheckBox.Margin = new Padding(3, 8, 12, 3);
        _loopCheckBox.Name = "_loopCheckBox";
        _loopCheckBox.Size = new Size(55, 19);
        _loopCheckBox.TabIndex = 2;
        _loopCheckBox.Text = "&Loop";
        _loopCheckBox.UseVisualStyleBackColor = true;
        _loopCheckBox.CheckedChanged += LoopCheckBox_CheckedChanged;
        //
        // _tempoLabel
        //
        _tempoLabel.AutoSize = true;
        _tempoLabel.Location = new Point(235, 8);
        _tempoLabel.Margin = new Padding(3, 8, 3, 3);
        _tempoLabel.Name = "_tempoLabel";
        _tempoLabel.Size = new Size(75, 15);
        _tempoLabel.TabIndex = 3;
        _tempoLabel.Text = "&Tempo (BPM)";
        //
        // _tempo
        //
        _tempo.AccessibleName = "Tempo in beats per minute";
        _tempo.Location = new Point(316, 6);
        _tempo.Margin = new Padding(3, 6, 12, 3);
        _tempo.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
        _tempo.Minimum = new decimal(new int[] { 40, 0, 0, 0 });
        _tempo.Name = "_tempo";
        _tempo.Size = new Size(64, 23);
        _tempo.TabIndex = 4;
        _tempo.Value = new decimal(new int[] { 92, 0, 0, 0 });
        _tempo.ValueChanged += Tempo_ValueChanged;
        //
        // _volumeLabel
        //
        _volumeLabel.AutoSize = true;
        _volumeLabel.Location = new Point(395, 8);
        _volumeLabel.Margin = new Padding(3, 8, 3, 3);
        _volumeLabel.Name = "_volumeLabel";
        _volumeLabel.Size = new Size(69, 15);
        _volumeLabel.TabIndex = 5;
        _volumeLabel.Text = "&Master 65%";
        //
        // _volume
        //
        _volume.AccessibleName = "Master volume";
        _volume.LargeChange = 10;
        _volume.Location = new Point(470, 3);
        _volume.Maximum = 100;
        _volume.Name = "_volume";
        _volume.Size = new Size(125, 45);
        _volume.SmallChange = 5;
        _volume.TabIndex = 6;
        _volume.TickFrequency = 10;
        _volume.Value = 65;
        _volume.ValueChanged += Volume_ValueChanged;
        //
        // _metallicLabel
        //
        _metallicLabel.AutoSize = true;
        _metallicLabel.Location = new Point(601, 8);
        _metallicLabel.Margin = new Padding(3, 8, 3, 3);
        _metallicLabel.Name = "_metallicLabel";
        _metallicLabel.Size = new Size(93, 15);
        _metallicLabel.TabIndex = 7;
        _metallicLabel.Text = "Metallic la&yer 0%";
        //
        // _metallic
        //
        _metallic.AccessibleName = "Hi-hat and cymbal metallic layer level";
        _metallic.LargeChange = 10;
        _metallic.Location = new Point(700, 3);
        _metallic.Maximum = 100;
        _metallic.Name = "_metallic";
        _metallic.Size = new Size(125, 45);
        _metallic.SmallChange = 5;
        _metallic.TabIndex = 8;
        _metallic.TickFrequency = 10;
        _metallic.ValueChanged += Metallic_ValueChanged;
        //
        // _metallicButton
        //
        _metallicButton.AutoSize = true;
        _metallicButton.Enabled = false;
        _metallicButton.Location = new Point(831, 3);
        _metallicButton.MinimumSize = new Size(100, 28);
        _metallicButton.Name = "_metallicButton";
        _metallicButton.Size = new Size(100, 28);
        _metallicButton.TabIndex = 9;
        _metallicButton.Text = "Audition metallic";
        _metallicButton.UseVisualStyleBackColor = true;
        _metallicButton.Click += MetallicButton_Click;
        //
        // _spectrumControl
        //
        _spectrumControl.AccessibleName = "Frequency spectrum of the played audio";
        _spectrumControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _spectrumControl.Location = new Point(15, 81);
        _spectrumControl.Margin = new Padding(3, 12, 3, 12);
        _spectrumControl.MinimumSize = new Size(300, 150);
        _spectrumControl.Name = "_spectrumControl";
        _spectrumControl.Size = new Size(1070, 270);
        _spectrumControl.TabIndex = 1;
        _spectrumControl.TabStop = false;
        //
        // _scoreToolbar
        //
        _scoreToolbar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _scoreToolbar.AutoSize = true;
        _scoreToolbar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _scoreToolbar.Controls.Add(_barLabel);
        _scoreToolbar.Controls.Add(_barSelector);
        _scoreToolbar.Controls.Add(_resetButton);
        _scoreToolbar.Controls.Add(_positionLabel);
        _scoreToolbar.Location = new Point(15, 366);
        _scoreToolbar.Name = "_scoreToolbar";
        _scoreToolbar.Size = new Size(1070, 35);
        _scoreToolbar.TabIndex = 2;
        //
        // _barLabel
        //
        _barLabel.AutoSize = true;
        _barLabel.Location = new Point(3, 8);
        _barLabel.Margin = new Padding(3, 8, 3, 3);
        _barLabel.Name = "_barLabel";
        _barLabel.Size = new Size(57, 15);
        _barLabel.TabIndex = 0;
        _barLabel.Text = "View &bar";
        //
        // _barSelector
        //
        _barSelector.AccessibleName = "Score bar to edit";
        _barSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        _barSelector.Location = new Point(66, 5);
        _barSelector.Margin = new Padding(3, 5, 12, 3);
        _barSelector.Name = "_barSelector";
        _barSelector.Size = new Size(60, 23);
        _barSelector.TabIndex = 1;
        _barSelector.SelectedIndexChanged += BarSelector_SelectedIndexChanged;
        //
        // _resetButton
        //
        _resetButton.AutoSize = true;
        _resetButton.Location = new Point(141, 3);
        _resetButton.MinimumSize = new Size(95, 28);
        _resetButton.Name = "_resetButton";
        _resetButton.Size = new Size(95, 28);
        _resetButton.TabIndex = 2;
        _resetButton.Text = "&Reset pattern";
        _resetButton.UseVisualStyleBackColor = true;
        _resetButton.Click += ResetButton_Click;
        //
        // _positionLabel
        //
        _positionLabel.AutoSize = true;
        _positionLabel.Location = new Point(248, 8);
        _positionLabel.Margin = new Padding(12, 8, 3, 3);
        _positionLabel.Name = "_positionLabel";
        _positionLabel.Size = new Size(50, 15);
        _positionLabel.TabIndex = 3;
        _positionLabel.Text = "Stopped";
        //
        // _stepGrid
        //
        _stepGrid.AccessibleName = "Percussion score steps";
        _stepGrid.AccessibleDescription = "Select a bar, toggle steps with Space, or use a row's Play button to audition its instrument.";
        _stepGrid.AllowUserToAddRows = false;
        _stepGrid.AllowUserToDeleteRows = false;
        _stepGrid.AllowUserToOrderColumns = false;
        _stepGrid.AllowUserToResizeRows = false;
        _stepGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _stepGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        _stepGrid.BackgroundColor = SystemColors.Window;
        _stepGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _stepGrid.Columns.AddRange(new DataGridViewColumn[] { _instrumentColumn, _auditionColumn, _step01, _step02, _step03, _step04, _step05, _step06, _step07, _step08, _step09, _step10, _step11, _step12, _step13, _step14, _step15, _step16 });
        _stepGrid.Enabled = false;
        _stepGrid.Location = new Point(15, 407);
        _stepGrid.MultiSelect = false;
        _stepGrid.Name = "_stepGrid";
        _stepGrid.RowHeadersVisible = false;
        _stepGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _stepGrid.Size = new Size(1070, 360);
        _stepGrid.TabIndex = 3;
        _stepGrid.CellContentClick += StepGrid_CellContentClick;
        _stepGrid.CellValueChanged += StepGrid_CellValueChanged;
        _stepGrid.CurrentCellDirtyStateChanged += StepGrid_CurrentCellDirtyStateChanged;
        _stepGrid.DataError += StepGrid_DataError;
        //
        // Score columns
        //
        _instrumentColumn.Frozen = true;
        _instrumentColumn.HeaderText = "Instrument";
        _instrumentColumn.MinimumWidth = 130;
        _instrumentColumn.Name = "_instrumentColumn";
        _instrumentColumn.ReadOnly = true;
        _instrumentColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        _instrumentColumn.Width = 150;
        _auditionColumn.Frozen = true;
        _auditionColumn.HeaderText = "Audition";
        _auditionColumn.MinimumWidth = 60;
        _auditionColumn.Name = "_auditionColumn";
        _auditionColumn.Text = "Play";
        _auditionColumn.UseColumnTextForButtonValue = true;
        _auditionColumn.Width = 70;
        _step01.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step01.HeaderText = "1";
        _step01.MinimumWidth = 32;
        _step01.Name = "_step01";
        _step02.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step02.HeaderText = "2";
        _step02.MinimumWidth = 32;
        _step02.Name = "_step02";
        _step03.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step03.HeaderText = "3";
        _step03.MinimumWidth = 32;
        _step03.Name = "_step03";
        _step04.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step04.DividerWidth = 3;
        _step04.HeaderText = "4";
        _step04.MinimumWidth = 32;
        _step04.Name = "_step04";
        _step05.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step05.HeaderText = "5";
        _step05.MinimumWidth = 32;
        _step05.Name = "_step05";
        _step06.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step06.HeaderText = "6";
        _step06.MinimumWidth = 32;
        _step06.Name = "_step06";
        _step07.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step07.HeaderText = "7";
        _step07.MinimumWidth = 32;
        _step07.Name = "_step07";
        _step08.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step08.DividerWidth = 3;
        _step08.HeaderText = "8";
        _step08.MinimumWidth = 32;
        _step08.Name = "_step08";
        _step09.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step09.HeaderText = "9";
        _step09.MinimumWidth = 32;
        _step09.Name = "_step09";
        _step10.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step10.HeaderText = "10";
        _step10.MinimumWidth = 32;
        _step10.Name = "_step10";
        _step11.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step11.HeaderText = "11";
        _step11.MinimumWidth = 32;
        _step11.Name = "_step11";
        _step12.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step12.DividerWidth = 3;
        _step12.HeaderText = "12";
        _step12.MinimumWidth = 32;
        _step12.Name = "_step12";
        _step13.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step13.HeaderText = "13";
        _step13.MinimumWidth = 32;
        _step13.Name = "_step13";
        _step14.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step14.HeaderText = "14";
        _step14.MinimumWidth = 32;
        _step14.Name = "_step14";
        _step15.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step15.HeaderText = "15";
        _step15.MinimumWidth = 32;
        _step15.Name = "_step15";
        _step16.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step16.HeaderText = "16";
        _step16.MinimumWidth = 32;
        _step16.Name = "_step16";
        //
        // _statusLabel
        //
        _statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _statusLabel.AutoSize = true;
        _statusLabel.Location = new Point(15, 773);
        _statusLabel.Margin = new Padding(3, 6, 3, 3);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(1070, 30);
        _statusLabel.TabIndex = 4;
        _statusLabel.Text = "Opening audio. Original two-bar groove; step edits take effect at the next bar.";
        //
        // Timers
        //
        _uiTimer.Interval = 33;
        _uiTimer.Tick += UiTimer_Tick;
        _exitTimer.Interval = 1000;
        _exitTimer.Tick += ExitTimer_Tick;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1100, 820);
        Controls.Add(_layout);
        MinimumSize = new Size(800, 620);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Analog Rhythm Lab - CR-78-style synthesis and spectrum - .NET 11";
        Disposed += MainForm_Disposed;
        ((System.ComponentModel.ISupportInitialize)_tempo).EndInit();
        ((System.ComponentModel.ISupportInitialize)_volume).EndInit();
        ((System.ComponentModel.ISupportInitialize)_metallic).EndInit();
        ((System.ComponentModel.ISupportInitialize)_stepGrid).EndInit();
        _transport.ResumeLayout(false);
        _transport.PerformLayout();
        _scoreToolbar.ResumeLayout(false);
        _scoreToolbar.PerformLayout();
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _layout;
    private FlowLayoutPanel _transport;
    private Button _playButton;
    private Button _stopButton;
    private CheckBox _loopCheckBox;
    private Label _tempoLabel;
    private NumericUpDown _tempo;
    private Label _volumeLabel;
    private TrackBar _volume;
    private Label _metallicLabel;
    private TrackBar _metallic;
    private Button _metallicButton;
    private SplitFlap.Audio.WinForms.AudioSpectrumControl _spectrumControl;
    private FlowLayoutPanel _scoreToolbar;
    private Label _barLabel;
    private ComboBox _barSelector;
    private Button _resetButton;
    private Label _positionLabel;
    private DataGridView _stepGrid;
    private DataGridViewTextBoxColumn _instrumentColumn;
    private DataGridViewButtonColumn _auditionColumn;
    private DataGridViewCheckBoxColumn _step01;
    private DataGridViewCheckBoxColumn _step02;
    private DataGridViewCheckBoxColumn _step03;
    private DataGridViewCheckBoxColumn _step04;
    private DataGridViewCheckBoxColumn _step05;
    private DataGridViewCheckBoxColumn _step06;
    private DataGridViewCheckBoxColumn _step07;
    private DataGridViewCheckBoxColumn _step08;
    private DataGridViewCheckBoxColumn _step09;
    private DataGridViewCheckBoxColumn _step10;
    private DataGridViewCheckBoxColumn _step11;
    private DataGridViewCheckBoxColumn _step12;
    private DataGridViewCheckBoxColumn _step13;
    private DataGridViewCheckBoxColumn _step14;
    private DataGridViewCheckBoxColumn _step15;
    private DataGridViewCheckBoxColumn _step16;
    private Label _statusLabel;
    private System.Windows.Forms.Timer _uiTimer;
    private System.Windows.Forms.Timer _exitTimer;
}
