using DrumMachine.Demo.Controls;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;

namespace DrumMachine.Demo;

/// <summary>
///  The subdivision used by the bar-fill commands.
/// </summary>
internal enum NoteDivision
{
    /// <summary>
    ///  A hit on every sixteenth-note step.
    /// </summary>
    Sixteenth = 1,

    /// <summary>
    ///  A hit on every eighth note.
    /// </summary>
    Eighth = 2,

    /// <summary>
    ///  A hit on every quarter note.
    /// </summary>
    Quarter = 4,

    /// <summary>
    ///  A hit on every half note.
    /// </summary>
    Half = 8,

    /// <summary>
    ///  A hit on the first step of the bar.
    /// </summary>
    Whole = 16
}

partial class MainForm
{
    private const int FirstStepColumn = 3;
    private readonly HashSet<Cr78Instrument> _selectedTracks = [.. Cr78Kit.Instruments];
    private bool _editAllBars;

    private int CurrentBar
    {
        get
        {
            if (_barSelector.SelectedIndex < 0)
            {
                return 0;
            }

            BarViewWindow view = ViewWindow;
            if (_stepGrid.CurrentCell is { ColumnIndex: >= FirstStepColumn } cell)
            {
                int displayedStep = cell.ColumnIndex - FirstStepColumn;
                if (displayedStep < view.BarsPerView * PercussionScore.StepsPerBar
                    && view.TryGetPosition(displayedStep, out int bar, out _))
                {
                    return bar;
                }
            }

            return view.FirstBar;
        }
    }

    private void PopulateEditorChoices()
    {
        _scopeSelector.Items.Add("Current bar");
        _scopeSelector.Items.Add("All bars");
        _scopeSelector.SelectedIndex = 0;
        _noteValueSelector.Items.Add("16th notes");
        _noteValueSelector.Items.Add("8th notes");
        _noteValueSelector.Items.Add("Quarter notes");
        _noteValueSelector.Items.Add("Half notes");
        _noteValueSelector.Items.Add("Whole notes");
        _noteValueSelector.SelectedIndex = 0;
        RefreshPresetControls(0);
    }

    private void AddBar_Click(object? sender, EventArgs e)
    {
        int bar = CurrentBar;
        EditScore(
            score => score.InsertBar(bar + 1),
            "Add bar",
            $"Inserted an empty bar after bar {bar + 1}.",
            bar + 1);
    }

    private void DeleteBar_Click(object? sender, EventArgs e)
    {
        int bar = CurrentBar;
        int focus = Math.Max(0, Math.Min(bar, _session.Current.Score.BarCount - 2));
        EditScore(
            score => score.RemoveBar(bar),
            "Delete bar",
            $"Deleted bar {bar + 1}.",
            focus);
    }

    private void CopyBar_Click(object? sender, EventArgs e)
    {
        if (_selectedTracks.Count == 0)
        {
            _statusLabel.Text = "Select at least one track before copying a bar.";
            return;
        }

        int bar = CurrentBar;
        EditScore(
            score => score.DuplicateBar(bar, _selectedTracks),
            "Copy bar",
            $"Copied bar {bar + 1} for {_selectedTracks.Count} selected track(s).",
            bar + 1);
    }

    private void Fill_Click(object? sender, EventArgs e)
        => FillSelection(_noteValueSelector.SelectedIndex switch
        {
            1 => NoteDivision.Eighth,
            2 => NoteDivision.Quarter,
            3 => NoteDivision.Half,
            4 => NoteDivision.Whole,
            _ => NoteDivision.Sixteenth
        });

    private void FillMenu_Click(object? sender, EventArgs e)
    {
        NoteDivision division = sender switch
        {
            ToolStripMenuItem { Name: "_fillEighthMenuItem" } => NoteDivision.Eighth,
            ToolStripMenuItem { Name: "_fillQuarterMenuItem" } => NoteDivision.Quarter,
            ToolStripMenuItem { Name: "_fillHalfMenuItem" } => NoteDivision.Half,
            ToolStripMenuItem { Name: "_fillWholeMenuItem" } => NoteDivision.Whole,
            _ => NoteDivision.Sixteenth
        };
        _noteValueSelector.SelectedIndex = division switch
        {
            NoteDivision.Eighth => 1,
            NoteDivision.Quarter => 2,
            NoteDivision.Half => 3,
            NoteDivision.Whole => 4,
            _ => 0
        };
        FillSelection(division);
    }

    private void Clear_Click(object? sender, EventArgs e)
    {
        if (_selectedTracks.Count == 0)
        {
            _statusLabel.Text = "Select at least one track before clearing.";
            return;
        }

        string scope = ScopeDescription();
        EditScore(
            score => score.ClearBars(ScopeBars(score), _selectedTracks),
            _editAllBars ? "Clear all bars" : "Clear bar",
            $"Cleared {scope} on {_selectedTracks.Count} selected track(s).");
    }

    private void SelectAllTracks_Click(object? sender, EventArgs e)
        => SetTrackSelection(Cr78Kit.Instruments, "Selected all tracks.");

    private void SelectNoneTracks_Click(object? sender, EventArgs e)
        => SetTrackSelection([], "Deselected all tracks. Fill, clear, and copy will skip every track.");

    private void ScopeSelector_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        _editAllBars = _scopeSelector.SelectedIndex == 1;
        UpdateCommandState();
    }

    private void CurrentBarScope_Click(object? sender, EventArgs e) => SetScope(allBars: false);

    private void AllBarsScope_Click(object? sender, EventArgs e) => SetScope(allBars: true);

    private void PresetSelector_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingControls || _presetSelector.SelectedIndex < 0)
        {
            return;
        }

        _presetName.Text = _settings.TrackPresets[_presetSelector.SelectedIndex].Name;
        UpdateCommandState();
    }

    private async void SavePreset_Click(object? sender, EventArgs e)
    {
        if (_updatingControls || _busy || _presetSelector.SelectedIndex < 0)
        {
            return;
        }

        int slot = _presetSelector.SelectedIndex;
        try
        {
            string name = TrackSelectionPreset.NormalizeName(_presetName.Text);
            _settings = _settings.WithTrackPreset(slot, name, _selectedTracks.OrderBy(instrument => instrument));
            RefreshPresetControls(slot);
            _statusLabel.Text = $"Saved {_settings.TrackPresets[slot].DisplayLabel(slot)}.";
            await PersistPreferencesAsync();
        }
        catch (ArgumentException ex)
        {
            _statusLabel.Text = ex.Message;
        }
        catch (Exception ex)
        {
            AppLogger.Error("Editor", "The track preset could not be saved.", ex);
            _statusLabel.Text = "The track preset could not be saved.";
        }
    }

    private void RecallPreset_Click(object? sender, EventArgs e)
    {
        if (_presetSelector.SelectedIndex < 0)
        {
            return;
        }

        TrackSelectionPreset preset = _settings.TrackPresets[_presetSelector.SelectedIndex];
        SetTrackSelection(preset.Instruments, $"Recalled {preset.DisplayLabel(_presetSelector.SelectedIndex)}.");
    }

    private void PresetName_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            SavePreset_Click(sender, e);
        }
    }

    private void StepGrid_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (_busy || e.ColumnIndex != _trackColumn.Index)
        {
            return;
        }

        if (_selectedTracks.Count == Cr78Kit.Instruments.Count)
        {
            SelectNoneTracks_Click(sender, e);
        }
        else
        {
            SelectAllTracks_Click(sender, e);
        }
    }

    private void StepGrid_CurrentCellChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls && !_closing)
        {
            UpdateCommandState();
            RefreshSoundStatus();
        }
    }

    private void FillSelection(NoteDivision division)
    {
        if (_selectedTracks.Count == 0)
        {
            _statusLabel.Text = "Select at least one track before filling.";
            return;
        }

        string scope = ScopeDescription();
        EditScore(
            score => score.FillBars(ScopeBars(score), _selectedTracks, (int)division),
            _editAllBars ? "Fill all bars" : "Fill bar",
            $"Filled {scope} on {_selectedTracks.Count} selected track(s) with {Describe(division)}.");
    }

    private void EditScore(Func<PercussionScore, PercussionScore> edit, string description, string success, int? focusBar = null)
    {
        if (_updatingControls || _busy)
        {
            return;
        }

        CommitEdits();
        try
        {
            PercussionScore edited = edit(_session.Current.Score);
            if (edited.Hits.Count > LoopDocument.MaximumHits)
            {
                _statusLabel.Text = "That edit exceeds the 65,536-hit document limit.";
                return;
            }

            int? page = focusBar is int bar ? bar / _settings.BarsPerView : null;
            if (ApplyEdit(_session.Current.WithScore(edited), description, rebuildGrid: true, page))
            {
                _statusLabel.Text = success;
            }
            else
            {
                _statusLabel.Text = "That edit did not change the score.";
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            AppLogger.Error("Editor", $"{description} could not be applied.", ex);
            _statusLabel.Text = ex.Message;
        }
    }

    private int[] ScopeBars(PercussionScore score)
        => _editAllBars ? Enumerable.Range(0, score.BarCount).ToArray() : [CurrentBar];

    private string ScopeDescription()
        => _editAllBars ? "all bars" : $"bar {CurrentBar + 1}";

    private void SetScope(bool allBars)
    {
        _editAllBars = allBars;
        bool updating = _updatingControls;
        _updatingControls = true;
        try
        {
            _scopeSelector.SelectedIndex = allBars ? 1 : 0;
        }
        finally
        {
            _updatingControls = updating;
        }

        UpdateCommandState();
    }

    private void SetTrackSelection(IEnumerable<Cr78Instrument> instruments, string status)
    {
        _selectedTracks.Clear();
        foreach (Cr78Instrument instrument in instruments)
        {
            if (Cr78Kit.Instruments.Contains(instrument))
            {
                _selectedTracks.Add(instrument);
            }
        }

        bool updating = _updatingControls;
        _updatingControls = true;
        try
        {
            RefreshTrackStyles();
        }
        finally
        {
            _updatingControls = updating;
        }

        _statusLabel.Text = status;
        UpdateCommandState();
    }

    private void UpdateTrackSelectionFromRow(int rowIndex)
    {
        if (_stepGrid.Rows[rowIndex].Tag is not Cr78Instrument instrument)
        {
            return;
        }

        bool selected = IsChecked(_stepGrid.Rows[rowIndex].Cells[_trackColumn.Index].Value);
        if (selected)
        {
            _selectedTracks.Add(instrument);
        }
        else
        {
            _selectedTracks.Remove(instrument);
        }

        ApplyTrackStyle(_stepGrid.Rows[rowIndex], selected);
        UpdateCommandState();
    }

    private void RefreshTrackStyles()
    {
        if (_stepGrid is null)
        {
            return;
        }

        foreach (DataGridViewRow row in _stepGrid.Rows)
        {
            if (row.Tag is Cr78Instrument instrument)
            {
                bool selected = _selectedTracks.Contains(instrument);
                if (row.Cells[_trackColumn.Index].Value is not bool current || current != selected)
                {
                    row.Cells[_trackColumn.Index].Value = selected;
                }

                ApplyTrackStyle(row, selected);
            }
        }
    }

    private void ApplyTrackStyle(DataGridViewRow row, bool selected)
    {
        DataGridViewCell name = row.Cells[_instrumentColumn.Index];
        if (row.Tag is Cr78Instrument instrument)
        {
            BeatGridView.ApplyTrackNameStyle(name, instrument, selected);
        }

        name.ToolTipText = selected
            ? "This track is included in fill, clear, and copy."
            : "This track is excluded from fill, clear, and copy. Step edits still apply.";
    }

    private void RefreshPresetControls(int selectedSlot)
    {
        bool updating = _updatingControls;
        _updatingControls = true;
        try
        {
            _presetSelector.Items.Clear();
            for (int slot = 0; slot < TrackSelectionPreset.SlotCount; slot++)
            {
                _presetSelector.Items.Add(_settings.TrackPresets[slot].DisplayLabel(slot));
            }

            _presetSelector.SelectedIndex = Math.Clamp(selectedSlot, 0, TrackSelectionPreset.SlotCount - 1);
            _presetName.Text = _settings.TrackPresets[_presetSelector.SelectedIndex].Name;
        }
        finally
        {
            _updatingControls = updating;
        }
    }

    private static bool IsChecked(object? value)
        => value is true or CheckState.Checked;

    private static string Describe(NoteDivision division)
        => division switch
        {
            NoteDivision.Eighth => "8th notes",
            NoteDivision.Quarter => "quarter notes",
            NoteDivision.Half => "half notes",
            NoteDivision.Whole => "whole notes",
            _ => "16th notes"
        };
}
