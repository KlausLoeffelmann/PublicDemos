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
        components = new System.ComponentModel.Container();
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
        _menuStrip = new MenuStrip();
        _fileMenu = new ToolStripMenuItem();
        _openMenuItem = new ToolStripMenuItem();
        _recentMenu = new ToolStripMenuItem();
        _recentEmpty = new ToolStripMenuItem();
        _saveMenuItem = new ToolStripMenuItem();
        _saveAsMenuItem = new ToolStripMenuItem();
        _exportWavMenuItem = new ToolStripMenuItem();
        _fileSeparator = new ToolStripSeparator();
        _quitMenuItem = new ToolStripMenuItem();
        _editMenu = new ToolStripMenuItem();
        _newMenuItem = new ToolStripMenuItem();
        _editSeparator = new ToolStripSeparator();
        _undoMenuItem = new ToolStripMenuItem();
        _redoMenuItem = new ToolStripMenuItem();
        _editCommandsSeparator = new ToolStripSeparator();
        _addBarMenuItem = new ToolStripMenuItem();
        _deleteBarMenuItem = new ToolStripMenuItem();
        _copyBarMenuItem = new ToolStripMenuItem();
        _patternMenuSeparator = new ToolStripSeparator();
        _fillMenu = new ToolStripMenuItem();
        _fillSixteenthMenuItem = new ToolStripMenuItem();
        _fillEighthMenuItem = new ToolStripMenuItem();
        _fillQuarterMenuItem = new ToolStripMenuItem();
        _fillHalfMenuItem = new ToolStripMenuItem();
        _fillWholeMenuItem = new ToolStripMenuItem();
        _clearMenuItem = new ToolStripMenuItem();
        _selectionMenuSeparator = new ToolStripSeparator();
        _selectAllMenuItem = new ToolStripMenuItem();
        _selectNoneMenuItem = new ToolStripMenuItem();
        _scopeMenuSeparator = new ToolStripSeparator();
        _scopeMenu = new ToolStripMenuItem();
        _currentBarScopeMenuItem = new ToolStripMenuItem();
        _allBarsScopeMenuItem = new ToolStripMenuItem();
        _savePresetMenuItem = new ToolStripMenuItem();
        _recallPresetMenuItem = new ToolStripMenuItem();
        _viewMenu = new ToolStripMenuItem();
        _oneBarMenuItem = new ToolStripMenuItem();
        _twoBarsMenuItem = new ToolStripMenuItem();
        _toolsMenu = new ToolStripMenuItem();
        _importWavMenuItem = new ToolStripMenuItem();
        _resetSoundMenuItem = new ToolStripMenuItem();
        _soundMenuSeparator = new ToolStripSeparator();
        _loadSoundPresetMenuItem = new ToolStripMenuItem();
        _saveSoundPresetMenuItem = new ToolStripMenuItem();
        _toolsSeparator = new ToolStripSeparator();
        _optionsMenuItem = new ToolStripMenuItem();
        _toolStrip = new ToolStrip();
        _newButton = new ToolStripButton();
        _openButton = new ToolStripButton();
        _saveButton = new ToolStripButton();
        _documentSeparator = new ToolStripSeparator();
        _playButton = new ToolStripButton();
        _pauseButton = new ToolStripButton();
        _stopButton = new ToolStripButton();
        _loopButton = new ToolStripButton();
        _transportSeparator = new ToolStripSeparator();
        _tempoLabel = new ToolStripLabel();
        _tempo = new DrumMachine.Demo.Controls.ToolStripTrackBar();
        _tempoSeparator = new ToolStripSeparator();
        _volumeLabel = new ToolStripLabel();
        _volumeSelector = new ToolStripComboBox();
        _volume = new DrumMachine.Demo.Controls.ToolStripTrackBar();
        _volumeSeparator = new ToolStripSeparator();
        _metallicButton = new ToolStripButton();
        _metallic = new DrumMachine.Demo.Controls.ToolStripTrackBar();
        _metallicSeparator = new ToolStripSeparator();
        _barLabel = new ToolStripLabel();
        _barSelector = new ToolStripComboBox();
        _auditionMetallicButton = new ToolStripButton();
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _soundStatusLabel = new ToolStripStatusLabel();
        _positionLabel = new ToolStripStatusLabel();
        _layout = new TableLayoutPanel();
        _contentSplit = new SplitContainer();
        _spectrumControl = new WinForms.Audio.WinForms.AudioSpectrumControl();
        _stepGrid = new DrumMachine.Demo.Controls.BeatGridView();
        _trackColumn = new DataGridViewCheckBoxColumn();
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
        _step17 = new DataGridViewCheckBoxColumn();
        _step18 = new DataGridViewCheckBoxColumn();
        _step19 = new DataGridViewCheckBoxColumn();
        _step20 = new DataGridViewCheckBoxColumn();
        _step21 = new DataGridViewCheckBoxColumn();
        _step22 = new DataGridViewCheckBoxColumn();
        _step23 = new DataGridViewCheckBoxColumn();
        _step24 = new DataGridViewCheckBoxColumn();
        _step25 = new DataGridViewCheckBoxColumn();
        _step26 = new DataGridViewCheckBoxColumn();
        _step27 = new DataGridViewCheckBoxColumn();
        _step28 = new DataGridViewCheckBoxColumn();
        _step29 = new DataGridViewCheckBoxColumn();
        _step30 = new DataGridViewCheckBoxColumn();
        _step31 = new DataGridViewCheckBoxColumn();
        _step32 = new DataGridViewCheckBoxColumn();
        _editStrip = new ToolStrip();
        _addBarButton = new ToolStripButton();
        _deleteBarButton = new ToolStripButton();
        _copyBarButton = new ToolStripButton();
        _barCommandSeparator = new ToolStripSeparator();
        _targetBarLabel = new ToolStripLabel();
        _scopeLabel = new ToolStripLabel();
        _scopeSelector = new ToolStripComboBox();
        _fillSeparator = new ToolStripSeparator();
        _fillLabel = new ToolStripLabel();
        _noteValueSelector = new ToolStripComboBox();
        _fillButton = new ToolStripButton();
        _clearButton = new ToolStripButton();
        _patternSeparator = new ToolStripSeparator();
        _selectAllButton = new ToolStripButton();
        _selectNoneButton = new ToolStripButton();
        _selectionSeparatorStrip = new ToolStripSeparator();
        _presetLabel = new ToolStripLabel();
        _presetSelector = new ToolStripComboBox();
        _presetName = new ToolStripTextBox();
        _savePresetButton = new ToolStripButton();
        _recallPresetButton = new ToolStripButton();
        _soundSeparator = new ToolStripSeparator();
        _importWavButton = new ToolStripButton();
        _resetSoundButton = new ToolStripButton();
        _loadSoundPresetButton = new ToolStripButton();
        _saveSoundPresetButton = new ToolStripButton();
        _openDialog = new OpenFileDialog();
        _saveDialog = new SaveFileDialog();
        _wavImportDialog = new OpenFileDialog();
        _soundPresetOpenDialog = new OpenFileDialog();
        _soundPresetSaveDialog = new SaveFileDialog();
        _uiTimer = new System.Windows.Forms.Timer(components);
        _exitTimer = new System.Windows.Forms.Timer(components);
        _menuStrip.SuspendLayout();
        _toolStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_tempo).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_volume).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_metallic).BeginInit();
        _statusStrip.SuspendLayout();
        _layout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_contentSplit).BeginInit();
        _contentSplit.Panel1.SuspendLayout();
        _contentSplit.Panel2.SuspendLayout();
        _contentSplit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_stepGrid).BeginInit();
        _editStrip.SuspendLayout();
        SuspendLayout();
        // 
        // _menuStrip
        // 
        _menuStrip.ImageScalingSize = new Size(24, 24);
        _menuStrip.Items.AddRange(new ToolStripItem[] { _fileMenu, _editMenu, _viewMenu, _toolsMenu });
        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Padding = new Padding(14, 4, 0, 4);
        _menuStrip.Size = new Size(1770, 37);
        _menuStrip.TabIndex = 0;
        // 
        // _fileMenu
        // 
        _fileMenu.DropDownItems.AddRange(new ToolStripItem[] { _openMenuItem, _recentMenu, _saveMenuItem, _saveAsMenuItem, _exportWavMenuItem, _fileSeparator, _quitMenuItem });
        _fileMenu.Name = "_fileMenu";
        _fileMenu.Size = new Size(57, 29);
        _fileMenu.Text = "&File";
        // 
        // _openMenuItem
        // 
        _openMenuItem.Name = "_openMenuItem";
        _openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        _openMenuItem.Size = new Size(306, 34);
        _openMenuItem.Text = "&Open...";
        _openMenuItem.Click += Open_Click;
        // 
        // _recentMenu
        // 
        _recentMenu.DropDownItems.AddRange(new ToolStripItem[] { _recentEmpty });
        _recentMenu.Name = "_recentMenu";
        _recentMenu.Size = new Size(306, 34);
        _recentMenu.Text = "&Recent";
        // 
        // _recentEmpty
        // 
        _recentEmpty.Enabled = false;
        _recentEmpty.Name = "_recentEmpty";
        _recentEmpty.Size = new Size(248, 34);
        _recentEmpty.Text = "(No recent files)";
        // 
        // _saveMenuItem
        // 
        _saveMenuItem.Name = "_saveMenuItem";
        _saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        _saveMenuItem.Size = new Size(306, 34);
        _saveMenuItem.Text = "&Save";
        _saveMenuItem.Click += Save_Click;
        // 
        // _saveAsMenuItem
        // 
        _saveAsMenuItem.Name = "_saveAsMenuItem";
        _saveAsMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
        _saveAsMenuItem.Size = new Size(306, 34);
        _saveAsMenuItem.Text = "Save &As...";
        _saveAsMenuItem.Click += SaveAs_Click;
        //
        // _exportWavMenuItem
        //
        _exportWavMenuItem.Name = "_exportWavMenuItem";
        _exportWavMenuItem.Size = new Size(306, 34);
        _exportWavMenuItem.Text = "Export &WAV...";
        _exportWavMenuItem.ToolTipText = "Export the current bar, the full loop, or repeated loops as WAV. Playback and the document are not changed.";
        _exportWavMenuItem.Click += ExportWav_Click;
        //
        // _fileSeparator
        // 
        _fileSeparator.Name = "_fileSeparator";
        _fileSeparator.Size = new Size(303, 6);
        // 
        // _quitMenuItem
        // 
        _quitMenuItem.Name = "_quitMenuItem";
        _quitMenuItem.Size = new Size(306, 34);
        _quitMenuItem.Text = "&Quit";
        _quitMenuItem.Click += Quit_Click;
        // 
        // _editMenu
        // 
        _editMenu.DropDownItems.AddRange(new ToolStripItem[] { _newMenuItem, _editSeparator, _undoMenuItem, _redoMenuItem, _editCommandsSeparator, _addBarMenuItem, _deleteBarMenuItem, _copyBarMenuItem, _patternMenuSeparator, _fillMenu, _clearMenuItem, _selectionMenuSeparator, _selectAllMenuItem, _selectNoneMenuItem, _scopeMenuSeparator, _scopeMenu, _savePresetMenuItem, _recallPresetMenuItem });
        _editMenu.Name = "_editMenu";
        _editMenu.Size = new Size(60, 29);
        _editMenu.Text = "&Edit";
        // 
        // _newMenuItem
        // 
        _newMenuItem.Name = "_newMenuItem";
        _newMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        _newMenuItem.Size = new Size(361, 34);
        _newMenuItem.Text = "&New...";
        _newMenuItem.Click += New_Click;
        // 
        // _editSeparator
        // 
        _editSeparator.Name = "_editSeparator";
        _editSeparator.Size = new Size(358, 6);
        // 
        // _undoMenuItem
        // 
        _undoMenuItem.Name = "_undoMenuItem";
        _undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
        _undoMenuItem.Size = new Size(361, 34);
        _undoMenuItem.Text = "&Undo";
        _undoMenuItem.Click += Undo_Click;
        // 
        // _redoMenuItem
        // 
        _redoMenuItem.Name = "_redoMenuItem";
        _redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
        _redoMenuItem.Size = new Size(361, 34);
        _redoMenuItem.Text = "&Redo";
        _redoMenuItem.Click += Redo_Click;
        // 
        // _editCommandsSeparator
        // 
        _editCommandsSeparator.Name = "_editCommandsSeparator";
        _editCommandsSeparator.Size = new Size(358, 6);
        // 
        // _addBarMenuItem
        // 
        _addBarMenuItem.Name = "_addBarMenuItem";
        _addBarMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.B;
        _addBarMenuItem.Size = new Size(361, 34);
        _addBarMenuItem.Text = "&Add bar";
        _addBarMenuItem.ToolTipText = "Insert an empty bar after the target bar (Ctrl+Shift+B)";
        _addBarMenuItem.Click += AddBar_Click;
        // 
        // _deleteBarMenuItem
        // 
        _deleteBarMenuItem.Name = "_deleteBarMenuItem";
        _deleteBarMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.D;
        _deleteBarMenuItem.Size = new Size(361, 34);
        _deleteBarMenuItem.Text = "&Delete bar";
        _deleteBarMenuItem.ToolTipText = "Delete the target bar from every track (Ctrl+Shift+D)";
        _deleteBarMenuItem.Click += DeleteBar_Click;
        // 
        // _copyBarMenuItem
        // 
        _copyBarMenuItem.Name = "_copyBarMenuItem";
        _copyBarMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.C;
        _copyBarMenuItem.Size = new Size(361, 34);
        _copyBarMenuItem.Text = "&Copy bar";
        _copyBarMenuItem.ToolTipText = "Insert a copy of the target bar. Only selected tracks are copied (Ctrl+Shift+C)";
        _copyBarMenuItem.Click += CopyBar_Click;
        // 
        // _patternMenuSeparator
        // 
        _patternMenuSeparator.Name = "_patternMenuSeparator";
        _patternMenuSeparator.Size = new Size(358, 6);
        // 
        // _fillMenu
        // 
        _fillMenu.DropDownItems.AddRange(new ToolStripItem[] { _fillSixteenthMenuItem, _fillEighthMenuItem, _fillQuarterMenuItem, _fillHalfMenuItem, _fillWholeMenuItem });
        _fillMenu.Name = "_fillMenu";
        _fillMenu.Size = new Size(361, 34);
        _fillMenu.Text = "&Fill";
        // 
        // _fillSixteenthMenuItem
        // 
        _fillSixteenthMenuItem.Name = "_fillSixteenthMenuItem";
        _fillSixteenthMenuItem.Size = new Size(230, 34);
        _fillSixteenthMenuItem.Text = "&16th notes";
        _fillSixteenthMenuItem.Click += FillMenu_Click;
        // 
        // _fillEighthMenuItem
        // 
        _fillEighthMenuItem.Name = "_fillEighthMenuItem";
        _fillEighthMenuItem.Size = new Size(230, 34);
        _fillEighthMenuItem.Text = "&8th notes";
        _fillEighthMenuItem.Click += FillMenu_Click;
        // 
        // _fillQuarterMenuItem
        // 
        _fillQuarterMenuItem.Name = "_fillQuarterMenuItem";
        _fillQuarterMenuItem.Size = new Size(230, 34);
        _fillQuarterMenuItem.Text = "&Quarter notes";
        _fillQuarterMenuItem.Click += FillMenu_Click;
        // 
        // _fillHalfMenuItem
        // 
        _fillHalfMenuItem.Name = "_fillHalfMenuItem";
        _fillHalfMenuItem.Size = new Size(230, 34);
        _fillHalfMenuItem.Text = "&Half notes";
        _fillHalfMenuItem.Click += FillMenu_Click;
        // 
        // _fillWholeMenuItem
        // 
        _fillWholeMenuItem.Name = "_fillWholeMenuItem";
        _fillWholeMenuItem.Size = new Size(230, 34);
        _fillWholeMenuItem.Text = "&Whole notes";
        _fillWholeMenuItem.Click += FillMenu_Click;
        // 
        // _clearMenuItem
        // 
        _clearMenuItem.Name = "_clearMenuItem";
        _clearMenuItem.Size = new Size(361, 34);
        _clearMenuItem.Text = "C&lear";
        _clearMenuItem.ToolTipText = "Clear the current bar, or all bars, on the selected tracks";
        _clearMenuItem.Click += Clear_Click;
        // 
        // _selectionMenuSeparator
        // 
        _selectionMenuSeparator.Name = "_selectionMenuSeparator";
        _selectionMenuSeparator.Size = new Size(358, 6);
        // 
        // _selectAllMenuItem
        // 
        _selectAllMenuItem.Name = "_selectAllMenuItem";
        _selectAllMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.A;
        _selectAllMenuItem.Size = new Size(361, 34);
        _selectAllMenuItem.Text = "Select &all tracks";
        _selectAllMenuItem.Click += SelectAllTracks_Click;
        // 
        // _selectNoneMenuItem
        // 
        _selectNoneMenuItem.Name = "_selectNoneMenuItem";
        _selectNoneMenuItem.Size = new Size(361, 34);
        _selectNoneMenuItem.Text = "Select n&one";
        _selectNoneMenuItem.Click += SelectNoneTracks_Click;
        // 
        // _scopeMenuSeparator
        // 
        _scopeMenuSeparator.Name = "_scopeMenuSeparator";
        _scopeMenuSeparator.Size = new Size(358, 6);
        // 
        // _scopeMenu
        // 
        _scopeMenu.DropDownItems.AddRange(new ToolStripItem[] { _currentBarScopeMenuItem, _allBarsScopeMenuItem });
        _scopeMenu.Name = "_scopeMenu";
        _scopeMenu.Size = new Size(361, 34);
        _scopeMenu.Text = "Bar &scope";
        // 
        // _currentBarScopeMenuItem
        // 
        _currentBarScopeMenuItem.Checked = true;
        _currentBarScopeMenuItem.CheckState = CheckState.Checked;
        _currentBarScopeMenuItem.Name = "_currentBarScopeMenuItem";
        _currentBarScopeMenuItem.Size = new Size(211, 34);
        _currentBarScopeMenuItem.Text = "&Current bar";
        _currentBarScopeMenuItem.Click += CurrentBarScope_Click;
        // 
        // _allBarsScopeMenuItem
        // 
        _allBarsScopeMenuItem.Name = "_allBarsScopeMenuItem";
        _allBarsScopeMenuItem.Size = new Size(211, 34);
        _allBarsScopeMenuItem.Text = "&All bars";
        _allBarsScopeMenuItem.Click += AllBarsScope_Click;
        // 
        // _savePresetMenuItem
        // 
        _savePresetMenuItem.Name = "_savePresetMenuItem";
        _savePresetMenuItem.Size = new Size(361, 34);
        _savePresetMenuItem.Text = "&Save track preset";
        _savePresetMenuItem.ToolTipText = "Save the current track selection in the selected preset slot";
        _savePresetMenuItem.Click += SavePreset_Click;
        // 
        // _recallPresetMenuItem
        // 
        _recallPresetMenuItem.Name = "_recallPresetMenuItem";
        _recallPresetMenuItem.Size = new Size(361, 34);
        _recallPresetMenuItem.Text = "&Recall track preset";
        _recallPresetMenuItem.Click += RecallPreset_Click;
        // 
        // _viewMenu
        // 
        _viewMenu.DropDownItems.AddRange(new ToolStripItem[] { _oneBarMenuItem, _twoBarsMenuItem });
        _viewMenu.Name = "_viewMenu";
        _viewMenu.Size = new Size(69, 29);
        _viewMenu.Text = "&View";
        // 
        // _oneBarMenuItem
        // 
        _oneBarMenuItem.Checked = true;
        _oneBarMenuItem.CheckState = CheckState.Checked;
        _oneBarMenuItem.Name = "_oneBarMenuItem";
        _oneBarMenuItem.Size = new Size(165, 34);
        _oneBarMenuItem.Text = "&1 Bar";
        _oneBarMenuItem.Click += OneBar_Click;
        // 
        // _twoBarsMenuItem
        // 
        _twoBarsMenuItem.Name = "_twoBarsMenuItem";
        _twoBarsMenuItem.Size = new Size(165, 34);
        _twoBarsMenuItem.Text = "&2 Bars";
        _twoBarsMenuItem.Click += TwoBars_Click;
        // 
        // _toolsMenu
        // 
        _toolsMenu.DropDownItems.AddRange(new ToolStripItem[] { _importWavMenuItem, _resetSoundMenuItem, _soundMenuSeparator, _loadSoundPresetMenuItem, _saveSoundPresetMenuItem, _toolsSeparator, _optionsMenuItem });
        _toolsMenu.Name = "_toolsMenu";
        _toolsMenu.Size = new Size(71, 29);
        _toolsMenu.Text = "&Tools";
        // 
        // _importWavMenuItem
        // 
        _importWavMenuItem.Name = "_importWavMenuItem";
        _importWavMenuItem.Size = new Size(396, 34);
        _importWavMenuItem.Text = "&Import WAV for current track...";
        _importWavMenuItem.ToolTipText = "Replace the selected fixed track's synthesized sound with a WAV file. Tracks cannot be added or removed.";
        _importWavMenuItem.Click += ImportWav_Click;
        // 
        // _resetSoundMenuItem
        // 
        _resetSoundMenuItem.Name = "_resetSoundMenuItem";
        _resetSoundMenuItem.Size = new Size(396, 34);
        _resetSoundMenuItem.Text = "&Reset current track to synthesis";
        _resetSoundMenuItem.ToolTipText = "Drop the imported WAV for the selected track and use its algorithmic stock sound.";
        _resetSoundMenuItem.Click += ResetSound_Click;
        // 
        // _soundMenuSeparator
        // 
        _soundMenuSeparator.Name = "_soundMenuSeparator";
        _soundMenuSeparator.Size = new Size(393, 6);
        // 
        // _loadSoundPresetMenuItem
        // 
        _loadSoundPresetMenuItem.Name = "_loadSoundPresetMenuItem";
        _loadSoundPresetMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.L;
        _loadSoundPresetMenuItem.Size = new Size(396, 34);
        _loadSoundPresetMenuItem.Text = "&Load sound preset...";
        _loadSoundPresetMenuItem.ToolTipText = "Load a portable sound preset for the fixed tracks (Ctrl+Shift+L)";
        _loadSoundPresetMenuItem.Click += LoadSoundPreset_Click;
        // 
        // _saveSoundPresetMenuItem
        // 
        _saveSoundPresetMenuItem.Name = "_saveSoundPresetMenuItem";
        _saveSoundPresetMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.K;
        _saveSoundPresetMenuItem.Size = new Size(396, 34);
        _saveSoundPresetMenuItem.Text = "&Save sound preset...";
        _saveSoundPresetMenuItem.ToolTipText = "Save the current track-to-WAV mapping as a portable sound preset (Ctrl+Shift+K)";
        _saveSoundPresetMenuItem.Click += SaveSoundPreset_Click;
        // 
        // _toolsSeparator
        // 
        _toolsSeparator.Name = "_toolsSeparator";
        _toolsSeparator.Size = new Size(393, 6);
        // 
        // _optionsMenuItem
        // 
        _optionsMenuItem.Name = "_optionsMenuItem";
        _optionsMenuItem.Size = new Size(396, 34);
        _optionsMenuItem.Text = "&Options...";
        _optionsMenuItem.Click += Options_Click;
        // 
        // _toolStrip
        // 
        _toolStrip.GripStyle = ToolStripGripStyle.Hidden;
        _toolStrip.ImageScalingSize = new Size(32, 32);
        _toolStrip.Items.AddRange(new ToolStripItem[] { _newButton, _openButton, _saveButton, _documentSeparator, _playButton, _pauseButton, _stopButton, _loopButton, _transportSeparator, _tempoLabel, _tempo, _tempoSeparator, _volumeLabel, _volumeSelector, _volume, _volumeSeparator, _metallicButton, _metallic, _metallicSeparator, _barLabel, _barSelector, _auditionMetallicButton });
        _toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
        _toolStrip.Location = new Point(0, 37);
        _toolStrip.Name = "_toolStrip";
        _toolStrip.Padding = new Padding(0, 0, 3, 0);
        _toolStrip.Size = new Size(1770, 53);
        _toolStrip.TabIndex = 1;
        // 
        // _newButton
        // 
        _newButton.Name = "_newButton";
        _newButton.Size = new Size(54, 48);
        _newButton.Text = "New";
        _newButton.ToolTipText = "New loop (Ctrl+N)";
        _newButton.Click += New_Click;
        // 
        // _openButton
        // 
        _openButton.Name = "_openButton";
        _openButton.Size = new Size(62, 48);
        _openButton.Text = "Open";
        _openButton.ToolTipText = "Open loop (Ctrl+O)";
        _openButton.Click += Open_Click;
        // 
        // _saveButton
        // 
        _saveButton.Name = "_saveButton";
        _saveButton.Size = new Size(55, 48);
        _saveButton.Text = "Save";
        _saveButton.ToolTipText = "Save loop (Ctrl+S)";
        _saveButton.Click += Save_Click;
        // 
        // _documentSeparator
        // 
        _documentSeparator.Name = "_documentSeparator";
        _documentSeparator.Size = new Size(6, 53);
        // 
        // _playButton
        // 
        _playButton.Enabled = false;
        _playButton.Name = "_playButton";
        _playButton.Size = new Size(51, 48);
        _playButton.Text = "Play";
        _playButton.ToolTipText = "Play or resume";
        _playButton.Click += Play_Click;
        // 
        // _pauseButton
        // 
        _pauseButton.Enabled = false;
        _pauseButton.Name = "_pauseButton";
        _pauseButton.Size = new Size(65, 48);
        _pauseButton.Text = "Pause";
        _pauseButton.Click += Pause_Click;
        // 
        // _stopButton
        // 
        _stopButton.Enabled = false;
        _stopButton.Name = "_stopButton";
        _stopButton.Size = new Size(53, 48);
        _stopButton.Text = "Stop";
        _stopButton.ToolTipText = "Stop and reset to the beginning";
        _stopButton.Click += Stop_Click;
        // 
        // _loopButton
        // 
        _loopButton.Checked = true;
        _loopButton.CheckOnClick = true;
        _loopButton.CheckState = CheckState.Checked;
        _loopButton.Name = "_loopButton";
        _loopButton.Size = new Size(58, 48);
        _loopButton.Text = "Loop";
        _loopButton.CheckedChanged += Loop_CheckedChanged;
        // 
        // _transportSeparator
        // 
        _transportSeparator.Name = "_transportSeparator";
        _transportSeparator.Size = new Size(6, 53);
        // 
        // _tempoLabel
        // 
        _tempoLabel.Name = "_tempoLabel";
        _tempoLabel.Size = new Size(141, 48);
        _tempoLabel.Text = "Tempo: 92 BPM";
        // 
        // _tempo
        // 
        _tempo.AccessibleName = "Tempo in beats per minute";
        _tempo.AutoSize = false;
        _tempo.Maximum = 240;
        _tempo.Minimum = 40;
        _tempo.Name = "_tempo";
        _tempo.Overflow = ToolStripItemOverflow.Never;
        _tempo.Size = new Size(150, 48);
        _tempo.Value = 92;
        _tempo.ValueChanged += Tempo_ValueChanged;
        _tempo.GestureStarted += Slider_GestureStarted;
        _tempo.GestureCompleted += Slider_GestureCompleted;
        // 
        // _tempoSeparator
        // 
        _tempoSeparator.Name = "_tempoSeparator";
        _tempoSeparator.Size = new Size(6, 53);
        // 
        // _volumeLabel
        // 
        _volumeLabel.Name = "_volumeLabel";
        _volumeLabel.Size = new Size(121, 48);
        _volumeLabel.Text = "Volume: 65%";
        // 
        // _volumeSelector
        // 
        _volumeSelector.AccessibleName = "Volume target";
        _volumeSelector.AutoSize = false;
        _volumeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        _volumeSelector.Name = "_volumeSelector";
        _volumeSelector.Size = new Size(268, 33);
        _volumeSelector.SelectedIndexChanged += VolumeSelector_SelectedIndexChanged;
        // 
        // _volume
        // 
        _volume.AccessibleName = "Selected channel volume percent";
        _volume.AutoSize = false;
        _volume.Maximum = 100;
        _volume.Name = "_volume";
        _volume.Overflow = ToolStripItemOverflow.Never;
        _volume.Size = new Size(150, 48);
        _volume.Value = 65;
        _volume.ValueChanged += Volume_ValueChanged;
        _volume.GestureStarted += Slider_GestureStarted;
        _volume.GestureCompleted += Slider_GestureCompleted;
        // 
        // _volumeSeparator
        // 
        _volumeSeparator.Name = "_volumeSeparator";
        _volumeSeparator.Size = new Size(6, 53);
        // 
        // _metallicButton
        // 
        _metallicButton.CheckOnClick = true;
        _metallicButton.Name = "_metallicButton";
        _metallicButton.Size = new Size(164, 48);
        _metallicButton.Text = "Metallic layer: 0%";
        _metallicButton.ToolTipText = "Enable the shared cymbal/hi-hat metallic layer";
        _metallicButton.CheckedChanged += Metallic_CheckedChanged;
        // 
        // _metallic
        // 
        _metallic.AccessibleName = "Remembered metallic layer percent";
        _metallic.AutoSize = false;
        _metallic.Maximum = 100;
        _metallic.Name = "_metallic";
        _metallic.Overflow = ToolStripItemOverflow.Never;
        _metallic.Size = new Size(150, 48);
        _metallic.ValueChanged += Metallic_ValueChanged;
        _metallic.GestureStarted += Slider_GestureStarted;
        _metallic.GestureCompleted += Slider_GestureCompleted;
        // 
        // _metallicSeparator
        // 
        _metallicSeparator.Name = "_metallicSeparator";
        _metallicSeparator.Size = new Size(6, 53);
        // 
        // _barLabel
        // 
        _barLabel.Name = "_barLabel";
        _barLabel.Size = new Size(57, 48);
        _barLabel.Text = "View:";
        // 
        // _barSelector
        // 
        _barSelector.AccessibleName = "Score bar range to edit";
        _barSelector.AutoSize = false;
        _barSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        _barSelector.Name = "_barSelector";
        _barSelector.Size = new Size(140, 33);
        _barSelector.SelectedIndexChanged += BarSelector_SelectedIndexChanged;
        // 
        // _auditionMetallicButton
        // 
        _auditionMetallicButton.Enabled = false;
        _auditionMetallicButton.Name = "_auditionMetallicButton";
        _auditionMetallicButton.Size = new Size(88, 29);
        _auditionMetallicButton.Text = "Audition";
        _auditionMetallicButton.ToolTipText = "Audition metallic sound";
        _auditionMetallicButton.Click += AuditionMetallic_Click;
        // 
        // _statusStrip
        // 
        _statusStrip.ImageScalingSize = new Size(24, 24);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel, _soundStatusLabel, _positionLabel });
        _statusStrip.Location = new Point(0, 1198);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Padding = new Padding(2, 0, 33, 0);
        _statusStrip.Size = new Size(1770, 32);
        _statusStrip.TabIndex = 3;
        // 
        // _statusLabel
        // 
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(1503, 25);
        _statusLabel.Spring = true;
        _statusLabel.Text = "Opening audio...";
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _soundStatusLabel
        // 
        _soundStatusLabel.Name = "_soundStatusLabel";
        _soundStatusLabel.Size = new Size(151, 25);
        _soundStatusLabel.Text = "Sound: synthesis";
        _soundStatusLabel.ToolTipText = "Mapped sound for the current fixed percussion track";
        // 
        // _positionLabel
        // 
        _positionLabel.Name = "_positionLabel";
        _positionLabel.Size = new Size(81, 25);
        _positionLabel.Text = "Stopped";
        // 
        // _layout
        // 
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_contentSplit, 0, 0);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 130);
        _layout.Margin = new Padding(4, 4, 4, 4);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(18, 18, 18, 18);
        _layout.RowCount = 1;
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layout.Size = new Size(1770, 1068);
        _layout.TabIndex = 2;
        // 
        // _contentSplit
        // 
        _contentSplit.Dock = DockStyle.Fill;
        _contentSplit.Location = new Point(22, 22);
        _contentSplit.Margin = new Padding(4, 4, 4, 4);
        _contentSplit.Name = "_contentSplit";
        _contentSplit.Orientation = Orientation.Horizontal;
        // 
        // _contentSplit.Panel1
        // 
        _contentSplit.Panel1.Controls.Add(_spectrumControl);
        // 
        // _contentSplit.Panel2
        // 
        _contentSplit.Panel2.Controls.Add(_stepGrid);
        _contentSplit.Size = new Size(1726, 1024);
        _contentSplit.SplitterDistance = 346;
        _contentSplit.SplitterWidth = 12;
        _contentSplit.TabIndex = 0;
        // 
        // _spectrumControl
        // 
        _spectrumControl.AccessibleName = "Frequency spectrum of the played audio";
        _spectrumControl.BackColor = SystemColors.Window;
        _spectrumControl.Dock = DockStyle.Fill;
        _spectrumControl.ForeColor = SystemColors.WindowText;
        _spectrumControl.Location = new Point(0, 0);
        _spectrumControl.Margin = new Padding(4, 4, 4, 4);
        _spectrumControl.MinimumSize = new Size(450, 150);
        _spectrumControl.Name = "_spectrumControl";
        _spectrumControl.Size = new Size(1726, 346);
        _spectrumControl.TabIndex = 0;
        _spectrumControl.TabStop = false;
        // 
        // _stepGrid
        // 
        _stepGrid.AccessibleDescription = "Fixed percussion tracks. Use includes a track in fill, clear, and copy. Space toggles a step. Play auditions the row. Import WAV changes only the current track's sound.";
        _stepGrid.AccessibleName = "Percussion score steps";
        _stepGrid.AllowUserToAddRows = false;
        _stepGrid.AllowUserToDeleteRows = false;
        _stepGrid.AllowUserToOrderColumns = true;
        _stepGrid.AllowUserToResizeRows = false;
        _stepGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        _stepGrid.BackgroundColor = SystemColors.Window;
        _stepGrid.BorderStyle = BorderStyle.None;
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = SystemColors.Control;
        dataGridViewCellStyle1.Font = new Font("Segoe UI", 9.5F);
        dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Control;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
        _stepGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        _stepGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _stepGrid.Columns.AddRange(new DataGridViewColumn[] { _trackColumn, _instrumentColumn, _auditionColumn, _step01, _step02, _step03, _step04, _step05, _step06, _step07, _step08, _step09, _step10, _step11, _step12, _step13, _step14, _step15, _step16, _step17, _step18, _step19, _step20, _step21, _step22, _step23, _step24, _step25, _step26, _step27, _step28, _step29, _step30, _step31, _step32 });
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = SystemColors.Window;
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.5F);
        dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
        _stepGrid.DefaultCellStyle = dataGridViewCellStyle2;
        _stepGrid.Dock = DockStyle.Fill;
        _stepGrid.EnableHeadersVisualStyles = false;
        _stepGrid.GridColor = SystemColors.ControlDark;
        _stepGrid.Location = new Point(0, 0);
        _stepGrid.Margin = new Padding(4, 4, 4, 4);
        _stepGrid.MultiSelect = false;
        _stepGrid.Name = "_stepGrid";
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle3.BackColor = SystemColors.Control;
        dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.5F);
        dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
        _stepGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
        _stepGrid.RowHeadersVisible = false;
        _stepGrid.RowHeadersWidth = 62;
        _stepGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _stepGrid.Size = new Size(1726, 666);
        _stepGrid.TabIndex = 1;
        _stepGrid.CellContentClick += StepGrid_CellContentClick;
        _stepGrid.CellValueChanged += StepGrid_CellValueChanged;
        _stepGrid.ColumnHeaderMouseClick += StepGrid_ColumnHeaderMouseClick;
        _stepGrid.CurrentCellChanged += StepGrid_CurrentCellChanged;
        _stepGrid.CurrentCellDirtyStateChanged += StepGrid_CurrentCellDirtyStateChanged;
        _stepGrid.DataError += StepGrid_DataError;
        // 
        // _trackColumn
        // 
        _trackColumn.FalseValue = false;
        _trackColumn.Frozen = true;
        _trackColumn.HeaderText = "Use";
        _trackColumn.MinimumWidth = 44;
        _trackColumn.Name = "_trackColumn";
        _trackColumn.ToolTipText = "Include this track in fill, clear, and copy. Click the header to select or deselect every track.";
        _trackColumn.TrueValue = true;
        _trackColumn.Width = 52;
        // 
        // _instrumentColumn
        // 
        _instrumentColumn.Frozen = true;
        _instrumentColumn.HeaderText = "Instrument";
        _instrumentColumn.MinimumWidth = 160;
        _instrumentColumn.Name = "_instrumentColumn";
        _instrumentColumn.ReadOnly = true;
        _instrumentColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        _instrumentColumn.Width = 190;
        // 
        // _auditionColumn
        // 
        _auditionColumn.Frozen = true;
        _auditionColumn.HeaderText = "Audition";
        _auditionColumn.MinimumWidth = 60;
        _auditionColumn.Name = "_auditionColumn";
        _auditionColumn.Text = "Play";
        _auditionColumn.UseColumnTextForButtonValue = true;
        _auditionColumn.Width = 70;
        // 
        // _step01
        // 
        _step01.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step01.HeaderText = "1";
        _step01.MinimumWidth = 32;
        _step01.Name = "_step01";
        // 
        // _step02
        // 
        _step02.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step02.HeaderText = "2";
        _step02.MinimumWidth = 32;
        _step02.Name = "_step02";
        // 
        // _step03
        // 
        _step03.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step03.HeaderText = "3";
        _step03.MinimumWidth = 32;
        _step03.Name = "_step03";
        // 
        // _step04
        // 
        _step04.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step04.DividerWidth = 3;
        _step04.HeaderText = "4";
        _step04.MinimumWidth = 32;
        _step04.Name = "_step04";
        // 
        // _step05
        // 
        _step05.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step05.HeaderText = "5";
        _step05.MinimumWidth = 32;
        _step05.Name = "_step05";
        // 
        // _step06
        // 
        _step06.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step06.HeaderText = "6";
        _step06.MinimumWidth = 32;
        _step06.Name = "_step06";
        // 
        // _step07
        // 
        _step07.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step07.HeaderText = "7";
        _step07.MinimumWidth = 32;
        _step07.Name = "_step07";
        // 
        // _step08
        // 
        _step08.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step08.DividerWidth = 3;
        _step08.HeaderText = "8";
        _step08.MinimumWidth = 32;
        _step08.Name = "_step08";
        // 
        // _step09
        // 
        _step09.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step09.HeaderText = "9";
        _step09.MinimumWidth = 32;
        _step09.Name = "_step09";
        // 
        // _step10
        // 
        _step10.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step10.HeaderText = "10";
        _step10.MinimumWidth = 32;
        _step10.Name = "_step10";
        // 
        // _step11
        // 
        _step11.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step11.HeaderText = "11";
        _step11.MinimumWidth = 32;
        _step11.Name = "_step11";
        // 
        // _step12
        // 
        _step12.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step12.DividerWidth = 3;
        _step12.HeaderText = "12";
        _step12.MinimumWidth = 32;
        _step12.Name = "_step12";
        // 
        // _step13
        // 
        _step13.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step13.HeaderText = "13";
        _step13.MinimumWidth = 32;
        _step13.Name = "_step13";
        // 
        // _step14
        // 
        _step14.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step14.HeaderText = "14";
        _step14.MinimumWidth = 32;
        _step14.Name = "_step14";
        // 
        // _step15
        // 
        _step15.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step15.HeaderText = "15";
        _step15.MinimumWidth = 32;
        _step15.Name = "_step15";
        // 
        // _step16
        // 
        _step16.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step16.DividerWidth = 6;
        _step16.HeaderText = "16";
        _step16.MinimumWidth = 32;
        _step16.Name = "_step16";
        // 
        // _step17
        // 
        _step17.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step17.HeaderText = "1";
        _step17.MinimumWidth = 32;
        _step17.Name = "_step17";
        _step17.Visible = false;
        // 
        // _step18
        // 
        _step18.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step18.HeaderText = "2";
        _step18.MinimumWidth = 32;
        _step18.Name = "_step18";
        _step18.Visible = false;
        // 
        // _step19
        // 
        _step19.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step19.HeaderText = "3";
        _step19.MinimumWidth = 32;
        _step19.Name = "_step19";
        _step19.Visible = false;
        // 
        // _step20
        // 
        _step20.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step20.DividerWidth = 3;
        _step20.HeaderText = "4";
        _step20.MinimumWidth = 32;
        _step20.Name = "_step20";
        _step20.Visible = false;
        // 
        // _step21
        // 
        _step21.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step21.HeaderText = "5";
        _step21.MinimumWidth = 32;
        _step21.Name = "_step21";
        _step21.Visible = false;
        // 
        // _step22
        // 
        _step22.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step22.HeaderText = "6";
        _step22.MinimumWidth = 32;
        _step22.Name = "_step22";
        _step22.Visible = false;
        // 
        // _step23
        // 
        _step23.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step23.HeaderText = "7";
        _step23.MinimumWidth = 32;
        _step23.Name = "_step23";
        _step23.Visible = false;
        // 
        // _step24
        // 
        _step24.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step24.DividerWidth = 3;
        _step24.HeaderText = "8";
        _step24.MinimumWidth = 32;
        _step24.Name = "_step24";
        _step24.Visible = false;
        // 
        // _step25
        // 
        _step25.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step25.HeaderText = "9";
        _step25.MinimumWidth = 32;
        _step25.Name = "_step25";
        _step25.Visible = false;
        // 
        // _step26
        // 
        _step26.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step26.HeaderText = "10";
        _step26.MinimumWidth = 32;
        _step26.Name = "_step26";
        _step26.Visible = false;
        // 
        // _step27
        // 
        _step27.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step27.HeaderText = "11";
        _step27.MinimumWidth = 32;
        _step27.Name = "_step27";
        _step27.Visible = false;
        // 
        // _step28
        // 
        _step28.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step28.DividerWidth = 3;
        _step28.HeaderText = "12";
        _step28.MinimumWidth = 32;
        _step28.Name = "_step28";
        _step28.Visible = false;
        // 
        // _step29
        // 
        _step29.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step29.HeaderText = "13";
        _step29.MinimumWidth = 32;
        _step29.Name = "_step29";
        _step29.Visible = false;
        // 
        // _step30
        // 
        _step30.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step30.HeaderText = "14";
        _step30.MinimumWidth = 32;
        _step30.Name = "_step30";
        _step30.Visible = false;
        // 
        // _step31
        // 
        _step31.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step31.HeaderText = "15";
        _step31.MinimumWidth = 32;
        _step31.Name = "_step31";
        _step31.Visible = false;
        // 
        // _step32
        // 
        _step32.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        _step32.HeaderText = "16";
        _step32.MinimumWidth = 32;
        _step32.Name = "_step32";
        _step32.Visible = false;
        // 
        // _editStrip
        // 
        _editStrip.GripStyle = ToolStripGripStyle.Hidden;
        _editStrip.ImageScalingSize = new Size(24, 24);
        _editStrip.Items.AddRange(new ToolStripItem[] { _addBarButton, _deleteBarButton, _copyBarButton, _barCommandSeparator, _targetBarLabel, _scopeLabel, _scopeSelector, _fillSeparator, _fillLabel, _noteValueSelector, _fillButton, _clearButton, _patternSeparator, _selectAllButton, _selectNoneButton, _selectionSeparatorStrip, _presetLabel, _presetSelector, _presetName, _savePresetButton, _recallPresetButton, _soundSeparator, _importWavButton, _resetSoundButton, _loadSoundPresetButton, _saveSoundPresetButton });
        _editStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
        _editStrip.Location = new Point(0, 90);
        _editStrip.Name = "_editStrip";
        _editStrip.Padding = new Padding(6, 3, 6, 3);
        _editStrip.Size = new Size(1770, 40);
        _editStrip.TabIndex = 4;
        _editStrip.Text = "Bar editor";
        // 
        // _addBarButton
        // 
        _addBarButton.Name = "_addBarButton";
        _addBarButton.Size = new Size(83, 29);
        _addBarButton.Text = "Add bar";
        _addBarButton.ToolTipText = "Insert an empty bar after the target bar. Bars are shared by every track (Ctrl+Shift+B).";
        _addBarButton.Click += AddBar_Click;
        // 
        // _deleteBarButton
        // 
        _deleteBarButton.Name = "_deleteBarButton";
        _deleteBarButton.Size = new Size(103, 29);
        _deleteBarButton.Text = "Delete bar";
        _deleteBarButton.ToolTipText = "Delete the target bar from every track. The last remaining bar cannot be deleted (Ctrl+Shift+D).";
        _deleteBarButton.Click += DeleteBar_Click;
        // 
        // _copyBarButton
        // 
        _copyBarButton.Name = "_copyBarButton";
        _copyBarButton.Size = new Size(92, 29);
        _copyBarButton.Text = "Copy bar";
        _copyBarButton.ToolTipText = "Insert a copy of the target bar. Only selected tracks are copied (Ctrl+Shift+C).";
        _copyBarButton.Click += CopyBar_Click;
        // 
        // _barCommandSeparator
        // 
        _barCommandSeparator.Name = "_barCommandSeparator";
        _barCommandSeparator.Size = new Size(6, 34);
        // 
        // _targetBarLabel
        // 
        _targetBarLabel.Name = "_targetBarLabel";
        _targetBarLabel.Size = new Size(116, 29);
        _targetBarLabel.Text = "Target: Bar 1";
        // 
        // _scopeLabel
        // 
        _scopeLabel.Name = "_scopeLabel";
        _scopeLabel.Size = new Size(67, 29);
        _scopeLabel.Text = "Scope:";
        // 
        // _scopeSelector
        // 
        _scopeSelector.AccessibleName = "Bar edit scope";
        _scopeSelector.AutoSize = false;
        _scopeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        _scopeSelector.Name = "_scopeSelector";
        _scopeSelector.Size = new Size(178, 33);
        _scopeSelector.ToolTipText = "Fill and clear use the target bar, or every bar. The target bar is the selected step's bar, or the first visible bar.";
        _scopeSelector.SelectedIndexChanged += ScopeSelector_SelectedIndexChanged;
        // 
        // _fillSeparator
        // 
        _fillSeparator.Name = "_fillSeparator";
        _fillSeparator.Size = new Size(6, 34);
        // 
        // _fillLabel
        // 
        _fillLabel.Name = "_fillLabel";
        _fillLabel.Size = new Size(40, 29);
        _fillLabel.Text = "Fill:";
        // 
        // _noteValueSelector
        // 
        _noteValueSelector.AccessibleName = "Fill note value";
        _noteValueSelector.AutoSize = false;
        _noteValueSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        _noteValueSelector.Name = "_noteValueSelector";
        _noteValueSelector.Size = new Size(193, 33);
        _noteValueSelector.ToolTipText = "Note value used by Fill";
        // 
        // _fillButton
        // 
        _fillButton.Name = "_fillButton";
        _fillButton.Size = new Size(40, 29);
        _fillButton.Text = "Fill";
        _fillButton.ToolTipText = "Fill the scoped bars of the selected tracks. Other steps in those bars are cleared. Deselected tracks are unchanged.";
        _fillButton.Click += Fill_Click;
        // 
        // _clearButton
        // 
        _clearButton.Name = "_clearButton";
        _clearButton.Size = new Size(60, 29);
        _clearButton.Text = "Clear";
        _clearButton.ToolTipText = "Clear the scoped bars of the selected tracks. Deselected tracks are unchanged.";
        _clearButton.Click += Clear_Click;
        // 
        // _patternSeparator
        // 
        _patternSeparator.Name = "_patternSeparator";
        _patternSeparator.Size = new Size(6, 34);
        // 
        // _selectAllButton
        // 
        _selectAllButton.Name = "_selectAllButton";
        _selectAllButton.Size = new Size(92, 29);
        _selectAllButton.Text = "All tracks";
        _selectAllButton.ToolTipText = "Select every track for fill, clear, and copy (Ctrl+Shift+A). Click the Use header to toggle all.";
        _selectAllButton.Click += SelectAllTracks_Click;
        // 
        // _selectNoneButton
        // 
        _selectNoneButton.Name = "_selectNoneButton";
        _selectNoneButton.Size = new Size(95, 29);
        _selectNoneButton.Text = "No tracks";
        _selectNoneButton.ToolTipText = "Deselect every track so fill, clear, and copy do not change the score.";
        _selectNoneButton.Click += SelectNoneTracks_Click;
        // 
        // _selectionSeparatorStrip
        // 
        _selectionSeparatorStrip.Name = "_selectionSeparatorStrip";
        _selectionSeparatorStrip.Size = new Size(6, 34);
        // 
        // _presetLabel
        // 
        _presetLabel.Name = "_presetLabel";
        _presetLabel.Size = new Size(68, 29);
        _presetLabel.Text = "Preset:";
        // 
        // _presetSelector
        // 
        _presetSelector.AccessibleName = "Track selection preset";
        _presetSelector.AutoSize = false;
        _presetSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        _presetSelector.Name = "_presetSelector";
        _presetSelector.Size = new Size(253, 33);
        _presetSelector.ToolTipText = "One of eight named track-selection presets";
        _presetSelector.SelectedIndexChanged += PresetSelector_SelectedIndexChanged;
        // 
        // _presetName
        // 
        _presetName.AccessibleName = "Track preset name";
        _presetName.AutoSize = false;
        _presetName.MaxLength = 40;
        _presetName.Name = "_presetName";
        _presetName.Size = new Size(208, 33);
        _presetName.ToolTipText = "Name for the selected preset. Press Enter or Save to store the current track selection.";
        // 
        // _savePresetButton
        // 
        _savePresetButton.Name = "_savePresetButton";
        _savePresetButton.Size = new Size(55, 29);
        _savePresetButton.Text = "Save";
        _savePresetButton.ToolTipText = "Save the current track selection in the selected preset, using the name in the text box.";
        _savePresetButton.Click += SavePreset_Click;
        // 
        // _recallPresetButton
        // 
        _recallPresetButton.Name = "_recallPresetButton";
        _recallPresetButton.Size = new Size(65, 29);
        _recallPresetButton.Text = "Recall";
        _recallPresetButton.ToolTipText = "Apply the selected preset's track selection. This does not change the score.";
        _recallPresetButton.Click += RecallPreset_Click;
        // 
        // _soundSeparator
        // 
        _soundSeparator.Name = "_soundSeparator";
        _soundSeparator.Size = new Size(6, 23);
        // 
        // _importWavButton
        // 
        _importWavButton.Name = "_importWavButton";
        _importWavButton.Size = new Size(117, 29);
        _importWavButton.Text = "Import WAV";
        _importWavButton.ToolTipText = "Import a WAV file for the current fixed percussion track. This does not add a track.";
        _importWavButton.Click += ImportWav_Click;
        // 
        // _resetSoundButton
        // 
        _resetSoundButton.Name = "_resetSoundButton";
        _resetSoundButton.Size = new Size(93, 29);
        _resetSoundButton.Text = "Synthesis";
        _resetSoundButton.ToolTipText = "Reset the current track's mapped sound to synthesis.";
        _resetSoundButton.Click += ResetSound_Click;
        // 
        // _loadSoundPresetButton
        // 
        _loadSoundPresetButton.Name = "_loadSoundPresetButton";
        _loadSoundPresetButton.Size = new Size(122, 29);
        _loadSoundPresetButton.Text = "Load sounds";
        _loadSoundPresetButton.ToolTipText = "Load a portable sound preset (Ctrl+Shift+L).";
        _loadSoundPresetButton.Click += LoadSoundPreset_Click;
        // 
        // _saveSoundPresetButton
        // 
        _saveSoundPresetButton.Name = "_saveSoundPresetButton";
        _saveSoundPresetButton.Size = new Size(120, 29);
        _saveSoundPresetButton.Text = "Save sounds";
        _saveSoundPresetButton.ToolTipText = "Save the complete sound preset, including every mapped track (Ctrl+Shift+K).";
        _saveSoundPresetButton.Click += SaveSoundPreset_Click;
        // 
        // _openDialog
        // 
        _openDialog.DefaultExt = "drumloop.json";
        _openDialog.Filter = "Drum loops (*.drumloop.json)|*.drumloop.json|JSON files (*.json)|*.json";
        _openDialog.Title = "Open percussion loop";
        // 
        // _saveDialog
        // 
        _saveDialog.DefaultExt = "drumloop.json";
        _saveDialog.Filter = "Drum loops (*.drumloop.json)|*.drumloop.json";
        _saveDialog.Title = "Save percussion loop";
        // 
        // _wavImportDialog
        // 
        _wavImportDialog.DefaultExt = "wav";
        _wavImportDialog.Filter = "WAV files (*.wav)|*.wav";
        _wavImportDialog.Title = "Import percussion WAV";
        // 
        // _soundPresetOpenDialog
        // 
        _soundPresetOpenDialog.DefaultExt = "drumkit.json";
        _soundPresetOpenDialog.Filter = "Drum sound presets (*.drumkit.json)|*.drumkit.json";
        _soundPresetOpenDialog.Title = "Load sound preset";
        // 
        // _soundPresetSaveDialog
        // 
        _soundPresetSaveDialog.DefaultExt = "drumkit.json";
        _soundPresetSaveDialog.Filter = "Drum sound presets (*.drumkit.json)|*.drumkit.json";
        _soundPresetSaveDialog.Title = "Save sound preset";
        // 
        // _uiTimer
        // 
        _uiTimer.Interval = 33;
        _uiTimer.Tick += UiTimer_Tick;
        // 
        // _exitTimer
        // 
        _exitTimer.Interval = 1000;
        _exitTimer.Tick += ExitTimer_Tick;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1770, 1230);
        Controls.Add(_layout);
        Controls.Add(_statusStrip);
        Controls.Add(_editStrip);
        Controls.Add(_toolStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;
        Margin = new Padding(4, 4, 4, 4);
        MinimumSize = new Size(1189, 902);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Analog Rhythm Lab";
        Disposed += MainForm_Disposed;
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _toolStrip.ResumeLayout(false);
        _toolStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_tempo).EndInit();
        ((System.ComponentModel.ISupportInitialize)_volume).EndInit();
        ((System.ComponentModel.ISupportInitialize)_metallic).EndInit();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _layout.ResumeLayout(false);
        _contentSplit.Panel1.ResumeLayout(false);
        _contentSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_contentSplit).EndInit();
        _contentSplit.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_stepGrid).EndInit();
        _editStrip.ResumeLayout(false);
        _editStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip _menuStrip;
    private ToolStripMenuItem _fileMenu;
    private ToolStripMenuItem _openMenuItem;
    private ToolStripMenuItem _recentMenu;
    private ToolStripMenuItem _recentEmpty;
    private ToolStripMenuItem _saveMenuItem;
    private ToolStripMenuItem _saveAsMenuItem;
    private ToolStripMenuItem _exportWavMenuItem;
    private ToolStripSeparator _fileSeparator;
    private ToolStripMenuItem _quitMenuItem;
    private ToolStripMenuItem _editMenu;
    private ToolStripMenuItem _newMenuItem;
    private ToolStripSeparator _editSeparator;
    private ToolStripMenuItem _undoMenuItem;
    private ToolStripMenuItem _redoMenuItem;
    private ToolStripMenuItem _viewMenu;
    private ToolStripMenuItem _oneBarMenuItem;
    private ToolStripMenuItem _twoBarsMenuItem;
    private ToolStripMenuItem _toolsMenu;
    private ToolStripMenuItem _optionsMenuItem;
    private ToolStrip _toolStrip;
    private ToolStripButton _newButton;
    private ToolStripButton _openButton;
    private ToolStripButton _saveButton;
    private ToolStripButton _playButton;
    private ToolStripButton _pauseButton;
    private ToolStripButton _stopButton;
    private ToolStripButton _loopButton;
    private ToolStripLabel _tempoLabel;
    private Controls.ToolStripTrackBar _tempo;
    private ToolStripLabel _volumeLabel;
    private ToolStripComboBox _volumeSelector;
    private Controls.ToolStripTrackBar _volume;
    private ToolStripButton _metallicButton;
    private Controls.ToolStripTrackBar _metallic;
    private ToolStripLabel _barLabel;
    private ToolStripComboBox _barSelector;
    private ToolStripButton _auditionMetallicButton;
    private ToolStripSeparator _documentSeparator;
    private ToolStripSeparator _transportSeparator;
    private ToolStripSeparator _tempoSeparator;
    private ToolStripSeparator _volumeSeparator;
    private ToolStripSeparator _metallicSeparator;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusLabel;
    private ToolStripStatusLabel _soundStatusLabel;
    private ToolStripStatusLabel _positionLabel;
    private TableLayoutPanel _layout;
    private SplitContainer _contentSplit;
    private WinForms.Audio.WinForms.AudioSpectrumControl _spectrumControl;
    private Controls.BeatGridView _stepGrid;
    private DataGridViewCheckBoxColumn _trackColumn;
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
    private DataGridViewCheckBoxColumn _step17;
    private DataGridViewCheckBoxColumn _step18;
    private DataGridViewCheckBoxColumn _step19;
    private DataGridViewCheckBoxColumn _step20;
    private DataGridViewCheckBoxColumn _step21;
    private DataGridViewCheckBoxColumn _step22;
    private DataGridViewCheckBoxColumn _step23;
    private DataGridViewCheckBoxColumn _step24;
    private DataGridViewCheckBoxColumn _step25;
    private DataGridViewCheckBoxColumn _step26;
    private DataGridViewCheckBoxColumn _step27;
    private DataGridViewCheckBoxColumn _step28;
    private DataGridViewCheckBoxColumn _step29;
    private DataGridViewCheckBoxColumn _step30;
    private DataGridViewCheckBoxColumn _step31;
    private DataGridViewCheckBoxColumn _step32;
    private OpenFileDialog _openDialog;
    private SaveFileDialog _saveDialog;
    private OpenFileDialog _wavImportDialog;
    private OpenFileDialog _soundPresetOpenDialog;
    private SaveFileDialog _soundPresetSaveDialog;
    private System.Windows.Forms.Timer _uiTimer;
    private System.Windows.Forms.Timer _exitTimer;
    private ToolStrip _editStrip;
    private ToolStripButton _addBarButton;
    private ToolStripButton _deleteBarButton;
    private ToolStripButton _copyBarButton;
    private ToolStripSeparator _barCommandSeparator;
    private ToolStripLabel _targetBarLabel;
    private ToolStripLabel _scopeLabel;
    private ToolStripComboBox _scopeSelector;
    private ToolStripSeparator _fillSeparator;
    private ToolStripLabel _fillLabel;
    private ToolStripComboBox _noteValueSelector;
    private ToolStripButton _fillButton;
    private ToolStripButton _clearButton;
    private ToolStripSeparator _patternSeparator;
    private ToolStripButton _selectAllButton;
    private ToolStripButton _selectNoneButton;
    private ToolStripSeparator _selectionSeparatorStrip;
    private ToolStripLabel _presetLabel;
    private ToolStripComboBox _presetSelector;
    private ToolStripTextBox _presetName;
    private ToolStripButton _savePresetButton;
    private ToolStripButton _recallPresetButton;
    private ToolStripSeparator _soundSeparator;
    private ToolStripButton _importWavButton;
    private ToolStripButton _resetSoundButton;
    private ToolStripButton _loadSoundPresetButton;
    private ToolStripButton _saveSoundPresetButton;
    private ToolStripMenuItem _importWavMenuItem;
    private ToolStripMenuItem _resetSoundMenuItem;
    private ToolStripSeparator _soundMenuSeparator;
    private ToolStripMenuItem _loadSoundPresetMenuItem;
    private ToolStripMenuItem _saveSoundPresetMenuItem;
    private ToolStripSeparator _toolsSeparator;
    private ToolStripSeparator _editCommandsSeparator;
    private ToolStripMenuItem _addBarMenuItem;
    private ToolStripMenuItem _deleteBarMenuItem;
    private ToolStripMenuItem _copyBarMenuItem;
    private ToolStripSeparator _patternMenuSeparator;
    private ToolStripMenuItem _fillMenu;
    private ToolStripMenuItem _fillSixteenthMenuItem;
    private ToolStripMenuItem _fillEighthMenuItem;
    private ToolStripMenuItem _fillQuarterMenuItem;
    private ToolStripMenuItem _fillHalfMenuItem;
    private ToolStripMenuItem _fillWholeMenuItem;
    private ToolStripMenuItem _clearMenuItem;
    private ToolStripSeparator _selectionMenuSeparator;
    private ToolStripMenuItem _selectAllMenuItem;
    private ToolStripMenuItem _selectNoneMenuItem;
    private ToolStripSeparator _scopeMenuSeparator;
    private ToolStripMenuItem _scopeMenu;
    private ToolStripMenuItem _currentBarScopeMenuItem;
    private ToolStripMenuItem _allBarsScopeMenuItem;
    private ToolStripMenuItem _savePresetMenuItem;
    private ToolStripMenuItem _recallPresetMenuItem;
}
