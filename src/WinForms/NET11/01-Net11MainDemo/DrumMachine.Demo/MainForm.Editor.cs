using DrumMachine.Demo.Controls;
using DrumMachine.Demo.Documents;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Sequencing;

namespace DrumMachine.Demo;

partial class MainForm
{
    private BarViewWindow ViewWindow => new(
        _session.Current.Score.BarCount, _settings.BarsPerView, Math.Max(0, _barSelector.SelectedIndex));

    private void ApplyEdit(LoopDocument document, string description, bool rebuildGrid = false)
    {
        if (_updatingControls)
        {
            return;
        }
        if (_session.Apply(document, description))
        {
            ApplyToPlayer();
            RefreshDocumentControls(rebuildGrid);
        }
    }

    private void Slider_GestureStarted(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            _session.BeginGesture(ReferenceEquals(sender, _tempo) ? "Change tempo"
                : ReferenceEquals(sender, _volume) ? "Change volume" : "Change metallic level");
        }
    }

    private void Slider_GestureCompleted(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            _session.CommitGesture();
            UpdateCommandState();
        }
    }

    private void CommitEdits()
    {
        _stepGrid.EndEdit();
        _tempo.CommitGesture();
        _volume.CommitGesture();
        _metallic.CommitGesture();
        _session.CommitGesture();
    }

    private void Tempo_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            ApplyEdit(_session.Current.WithTempo(_tempo.Value), "Change tempo");
        }
    }

    private void VolumeSelector_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            CommitEdits();
            RefreshDocumentControls(rebuildGrid: false);
        }
    }

    private void Volume_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls && _volumeSelector.SelectedIndex >= 0)
        {
            LoopDocument document = _volumeSelector.SelectedIndex == 0
                ? _session.Current.WithMasterVolume(_volume.Value)
                : _session.Current.WithInstrumentVolume(Cr78Kit.Instruments[_volumeSelector.SelectedIndex - 1], _volume.Value);
            ApplyEdit(document, "Change volume");
        }
    }

    private void Loop_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            CommitEdits();
            ApplyEdit(_session.Current.WithLoop(_loopButton.Checked), "Toggle looping");
        }
    }

    private void Metallic_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            CommitEdits();
            ApplyEdit(_session.Current.WithMetallic(_metallicButton.Checked, _session.Current.MetallicVolumePercent), "Toggle metallic layer");
        }
    }

    private void Metallic_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            ApplyEdit(_session.Current.WithMetallic(_session.Current.MetallicEnabled, _metallic.Value), "Change metallic level");
        }
    }

    private void Undo_Click(object? sender, EventArgs e)
    {
        CommitEdits();
        if (_session.Undo())
        {
            ApplyToPlayer();
            RefreshDocumentControls(rebuildGrid: true);
        }
    }

    private void Redo_Click(object? sender, EventArgs e)
    {
        CommitEdits();
        if (_session.Redo())
        {
            ApplyToPlayer();
            RefreshDocumentControls(rebuildGrid: true);
        }
    }

    private void OneBar_Click(object? sender, EventArgs e) => ChangeView(1);
    private void TwoBars_Click(object? sender, EventArgs e) => ChangeView(2);

    private void ChangeView(int barsPerView)
    {
        CommitEdits();
        int firstBar = ViewWindow.FirstBar;
        _settings = _settings with { BarsPerView = barsPerView };
        RefreshDocumentControls(rebuildGrid: true, selectedPage: firstBar / barsPerView);
    }

    private void BarSelector_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            CommitEdits();
            LoadScoreGrid();
        }
    }

    private void RefreshDocumentControls(bool rebuildGrid, int? selectedPage = null)
    {
        _updatingControls = true;
        try
        {
            LoopDocument document = _session.Current;
            _tempo.Value = document.TempoBpm;
            _tempoLabel.Text = $"Tempo: {document.TempoBpm} BPM";
            _loopButton.Checked = document.Loop;
            _metallicButton.Checked = document.MetallicEnabled;
            _metallic.Value = document.MetallicVolumePercent;
            _metallicButton.Text = $"Metallic layer: {document.MetallicVolumePercent}%";
            int volume = _volumeSelector.SelectedIndex <= 0 ? document.MasterVolumePercent
                : document.PercussionVolumes[Cr78Kit.Instruments[_volumeSelector.SelectedIndex - 1]];
            _volume.Value = volume;
            _volumeLabel.Text = $"Volume: {volume}%";
            _oneBarMenuItem.Checked = _settings.BarsPerView == 1;
            _twoBarsMenuItem.Checked = _settings.BarsPerView == 2;
            if (rebuildGrid)
            {
                int page = selectedPage ?? Math.Max(0, _barSelector.SelectedIndex);
                _barSelector.Items.Clear();
                int pages = (document.Score.BarCount + _settings.BarsPerView - 1) / _settings.BarsPerView;
                for (int i = 0; i < pages; i++)
                {
                    _barSelector.Items.Add(new BarViewWindow(document.Score.BarCount, _settings.BarsPerView, i));
                }
                _barSelector.SelectedIndex = Math.Min(page, pages - 1);
            }
        }
        finally
        {
            _updatingControls = false;
        }
        if (rebuildGrid)
        {
            LoadScoreGrid();
        }
        UpdateCommandState();
    }

    private void LoadScoreGrid()
    {
        _updatingControls = true;
        try
        {
            BarViewWindow view = ViewWindow;
            _stepGrid.Rows.Clear();
            for (int i = 0; i < 32; i++)
            {
                DataGridViewColumn column = _stepGrid.Columns[i + 2];
                column.Visible = i < view.BarsPerView * 16;
                column.HeaderText = $"{view.FirstBar + i / 16 + 1}:{i % 16 + 1}";
            }

            foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
            {
                object[] values = new object[34];
                values[0] = Cr78Kit.GetDisplayName(instrument);
                values[1] = "Play";
                for (int i = 0; i < 32; i++)
                {
                    values[i + 2] = i < view.BarsPerView * 16 && view.TryGetPosition(i, out int bar, out int step)
                        && _session.Current.Score.HasHit(bar, instrument, step);
                }
                DataGridViewRow row = _stepGrid.Rows[_stepGrid.Rows.Add(values)];
                row.Tag = instrument;
                for (int i = 0; i < view.BarsPerView * 16; i++)
                {
                    bool valid = view.TryGetPosition(i, out int bar, out int step);
                    row.Cells[i + 2].ReadOnly = !valid;
                    row.Cells[i + 2].ToolTipText = valid
                        ? $"{Cr78Kit.GetDisplayName(instrument)}, bar {bar + 1}, step {step + 1}"
                        : "This loop has no second bar here; View does not add bars.";
                }
            }
        }
        finally
        {
            _updatingControls = false;
        }
        HighlightStep(-1, force: true);
    }

    private void StepGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex == _auditionColumn.Index
            && _stepGrid.Rows[e.RowIndex].Tag is Cr78Instrument instrument)
        {
            WithPlayer(player => player.Audition(instrument));
        }
    }

    private void StepGrid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_stepGrid.IsCurrentCellDirty && _stepGrid.CurrentCell is DataGridViewCheckBoxCell)
        {
            _stepGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void StepGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_updatingControls || e.RowIndex < 0 || e.ColumnIndex < 2)
        {
            return;
        }
        int displayedStep = e.ColumnIndex - 2;
        BarViewWindow view = ViewWindow;
        if (displayedStep < view.BarsPerView * 16
            && view.TryGetPosition(displayedStep, out int bar, out int step)
            && _stepGrid.Rows[e.RowIndex].Tag is Cr78Instrument instrument)
        {
            _session.CommitGesture();
            bool enabled = _stepGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value is true;
            ApplyEdit(_session.Current.WithScore(_session.Current.Score.WithStep(bar, instrument, step, enabled)), "Edit score step");
        }
    }

    private void StepGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        AppLogger.Error("Editor", "A score cell could not be edited.", e.Exception);
        _statusLabel.Text = "That edit could not be applied.";
        e.ThrowException = false;
    }

    private void UpdateCommandState()
    {
        bool editing = !_busy && (!Automated || _scenarioCompleted);
        bool audio = AudioAvailable && editing;
        _newMenuItem.Enabled = _newButton.Enabled = editing;
        _openMenuItem.Enabled = _openButton.Enabled = editing;
        _recentMenu.Enabled = editing;
        _saveMenuItem.Enabled = _saveButton.Enabled = editing && (_session.FilePath is null || _session.IsDirty);
        _saveAsMenuItem.Enabled = editing;
        _undoMenuItem.Enabled = editing && _session.CanUndo;
        _redoMenuItem.Enabled = editing && _session.CanRedo;
        _undoMenuItem.Text = _session.CanUndo ? $"&Undo {_session.UndoDescription}" : "&Undo";
        _redoMenuItem.Text = _session.CanRedo ? $"&Redo {_session.RedoDescription}" : "&Redo";
        _toolsMenu.Enabled = _viewMenu.Enabled = editing;
        _stepGrid.Enabled = editing;
        _tempo.Enabled = _volume.Enabled = _volumeSelector.Enabled = _metallic.Enabled = editing;
        _loopButton.Enabled = _metallicButton.Enabled = _barSelector.Enabled = editing;
        _playButton.Enabled = audio && _player!.State != DrumTransportState.Playing;
        _pauseButton.Enabled = audio && _player!.State == DrumTransportState.Playing;
        _stopButton.Enabled = _auditionMetallicButton.Enabled = audio;
        string name = _session.FilePath is null ? "Untitled" : Path.GetFileName(_session.FilePath);
        Text = $"{name}{(_session.IsDirty ? " *" : "")} - Analog Rhythm Lab";
    }

    private void UiTimer_Tick(object? sender, EventArgs e)
    {
        if (_player is null)
        {
            return;
        }
        DrumPlaybackSnapshot position = _player.GetPlaybackSnapshot();
        HighlightStep(position.IsPlaying || position.IsPaused ? ViewWindow.GetDisplayedStep(position.Bar, position.Step) : -1);
        string pending = position.HasPendingChanges ? " - pending next bar" : "";
        string clock = position.IsPlaybackSynchronized ? "played" : "submitted";
        _positionLabel.Text = $"{position.State}: bar {position.Bar + 1}, step {position.Step + 1} ({clock}){pending}";
        UpdateCommandState();
    }

    private void HighlightStep(int displayedStep, bool force = false)
    {
        if (_stepGrid is null || _stepGrid.Columns.Count < 34 || _barSelector.SelectedIndex < 0)
        {
            return;
        }
        if (!force && _highlightedStep == displayedStep)
        {
            return;
        }

        BarViewWindow view = ViewWindow;
        for (int i = 0; i < 32; i++)
        {
            bool missing = i < view.BarsPerView * 16 && !view.TryGetPosition(i, out _, out _);
            DataGridViewColumn column = _stepGrid.Columns[i + 2];
            column.DefaultCellStyle.BackColor = i == displayedStep ? SystemColors.Highlight
                : missing ? SystemColors.Control : SystemColors.Window;
            column.DefaultCellStyle.ForeColor = i == displayedStep ? SystemColors.HighlightText
                : missing ? SystemColors.GrayText : SystemColors.WindowText;
        }
        _highlightedStep = displayedStep;
    }

    private void InitializeIcons()
    {
        try
        {
            _iconFactory = new SymbolIconFactory();
            _icons = new ToolbarIconSet(_iconFactory);
            if (_iconFactory.UsesFallback)
            {
                AppLogger.Information("Icons", $"Segoe Fluent Icons is unavailable; using {_iconFactory.FontFamilyName}.");
            }
            RebuildIcons();
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            AppLogger.Error("Icons", "Symbol fonts are unavailable; toolbar text remains usable.", ex);
        }
    }

    private void RebuildIcons()
    {
        if (_icons is null || _closing)
        {
            return;
        }
        (ToolStripItem Item, ToolbarSymbol Symbol)[] buttons =
        [
            (_newButton, ToolbarSymbol.New), (_openButton, ToolbarSymbol.Open), (_saveButton, ToolbarSymbol.Save),
            (_playButton, ToolbarSymbol.Play), (_pauseButton, ToolbarSymbol.Pause), (_stopButton, ToolbarSymbol.Stop),
            (_loopButton, ToolbarSymbol.Loop), (_metallicButton, ToolbarSymbol.Metallic),
            (_auditionMetallicButton, ToolbarSymbol.Audition)
        ];
        _toolStrip.SuspendLayout();
        try
        {
            foreach (var (item, symbol) in buttons)
            {
                _icons.Apply(item, symbol, (int)_settings.IconSize, DeviceDpi, SystemColors.ControlText);
                item.DisplayStyle = ReferenceEquals(item, _loopButton) || ReferenceEquals(item, _metallicButton)
                    ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image;
            }
            (ToolStripItem Item, ToolbarSymbol Symbol)[] menus =
            [
                (_newMenuItem, ToolbarSymbol.New), (_openMenuItem, ToolbarSymbol.Open), (_saveMenuItem, ToolbarSymbol.Save),
                (_saveAsMenuItem, ToolbarSymbol.Save), (_undoMenuItem, ToolbarSymbol.Undo), (_redoMenuItem, ToolbarSymbol.Redo),
                (_optionsMenuItem, ToolbarSymbol.Options), (_quitMenuItem, ToolbarSymbol.Quit)
            ];
            foreach (var (item, symbol) in menus)
            {
                _icons.Apply(item, symbol, 16, DeviceDpi, SystemColors.MenuText);
            }
        }
        finally
        {
            _toolStrip.ResumeLayout(true);
        }
    }
}
