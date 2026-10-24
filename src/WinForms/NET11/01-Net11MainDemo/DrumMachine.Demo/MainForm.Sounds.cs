using DrumMachine.Demo.Documents;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;

namespace DrumMachine.Demo;

partial class MainForm
{
    private PercussionSoundPreset _soundPreset = PercussionSoundPreset.Stock;
    private PercussionSoundPreset _committedSoundPreset = PercussionSoundPreset.Stock;
    private IReadOnlyDictionary<Cr78Instrument, Sample>? _preparedSamples;
    private int _preparedRate;
    private int _soundGeneration;

    private async void ImportWav_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(ImportWavAsync);

    private async void ResetSound_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(ResetCurrentSoundAsync);

    private async void LoadSoundPreset_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(LoadSoundPresetAsync);

    private async void SaveSoundPreset_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(SaveSoundPresetAsync);

    private async Task ImportWavAsync()
    {
        if (!TryGetCurrentInstrument(out Cr78Instrument instrument))
        {
            _statusLabel.Text = "Select a percussion track before importing a WAV file.";
            return;
        }

        _wavImportDialog.InitialDirectory = FirstExistingDirectory(
            _settings.LastWavImportFolder, _settings.RecordingsFolder, _settings.DefaultFolder);
        _wavImportDialog.FileName = "";
        if (_wavImportDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string path = Path.GetFullPath(_wavImportDialog.FileName);
        _soundPreset = _soundPreset.WithWaveFile(instrument, path);
        _settings = _settings.WithLastWavImportFolder(Path.GetDirectoryName(path)!);
        await PersistPreferencesAsync();
        await ApplyOwnedSamplesAsync();
        _statusLabel.Text = $"Mapped {Cr78Kit.GetDisplayName(instrument)} to {Path.GetFileName(path)}.";
        RefreshSoundStatus();
    }

    private async Task ResetCurrentSoundAsync()
    {
        if (!TryGetCurrentInstrument(out Cr78Instrument instrument))
        {
            _statusLabel.Text = "Select a percussion track before resetting its sound.";
            return;
        }

        _soundPreset = _soundPreset.WithoutWaveFile(instrument);
        await ApplyOwnedSamplesAsync();
        _statusLabel.Text = $"{Cr78Kit.GetDisplayName(instrument)} uses the synthesized stock sound.";
        RefreshSoundStatus();
    }

    private async Task LoadSoundPresetAsync()
    {
        if (!await ConfirmSoundPresetAsync())
        {
            return;
        }

        _soundPresetOpenDialog.InitialDirectory = FirstExistingDirectory(
            _settings.SoundPresetFolder, _settings.DefaultFolder);
        _soundPresetOpenDialog.FileName = "";
        if (_soundPresetOpenDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        PercussionSoundPreset preset = await PercussionSoundPresetStore.LoadAsync(
            _soundPresetOpenDialog.FileName, _lifetime.Token);
        _soundPreset = preset;
        _committedSoundPreset = preset;
        await ApplyOwnedSamplesAsync();
        _statusLabel.Text = $"Loaded sound preset \"{preset.Name}\".";
        RefreshSoundStatus();
    }

    private async Task<bool> SaveSoundPresetAsync()
    {
        _soundPresetSaveDialog.InitialDirectory = FirstExistingDirectory(
            _settings.SoundPresetFolder, _settings.DefaultFolder);
        _soundPresetSaveDialog.FileName = SanitizeFileName(_soundPreset.Name) + PercussionSoundPresetStore.FileExtension;
        if (_soundPresetSaveDialog.ShowDialog(this) != DialogResult.OK)
        {
            return false;
        }

        string path = _soundPresetSaveDialog.FileName;
        string name = Path.GetFileNameWithoutExtension(path);
        if (name.EndsWith(".drumkit", StringComparison.OrdinalIgnoreCase))
        {
            name = Path.GetFileNameWithoutExtension(name);
        }

        PercussionSoundPreset preset = _soundPreset.WithName(name);
        await PercussionSoundPresetStore.SaveAsync(preset, path, _lifetime.Token);
        // The store copies WAV assets beside the manifest. Reload so later saves do not depend on the import sources.
        PercussionSoundPreset owned = await PercussionSoundPresetStore.LoadAsync(path, _lifetime.Token);
        _soundPreset = owned;
        _committedSoundPreset = owned;
        await ApplyOwnedSamplesAsync();
        _statusLabel.Text = $"Saved sound preset \"{owned.Name}\".";
        RefreshSoundStatus();
        return true;
    }

    /// <summary>
    ///  Asks to save a dirty sound palette without consulting the loop's dirty flag.
    /// </summary>
    private async Task<bool> ConfirmSoundPresetAsync()
    {
        if (Automated || !SoundPresetIsDirty)
        {
            return true;
        }

        DialogResult answer = MessageBox.Show(
            this,
            "Save changes to the current sound preset?",
            "Unsaved sound preset",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);
        return answer switch
        {
            DialogResult.Yes => await SaveSoundPresetAsync(),
            DialogResult.No => true,
            _ => false
        };
    }

    private bool SoundPresetIsDirty => !SameMapping(_soundPreset, _committedSoundPreset);

    private static bool SameMapping(PercussionSoundPreset left, PercussionSoundPreset right)
    {
        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
        {
            return false;
        }

        if (left.WaveFiles.Count != right.WaveFiles.Count)
        {
            return false;
        }

        foreach ((Cr78Instrument instrument, string path) in left.WaveFiles)
        {
            if (!right.WaveFiles.TryGetValue(instrument, out string? other)
                || !string.Equals(path, other, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Rebuilds the current track-to-sample map and pushes it to the player without touching the score.
    /// </summary>
    private async Task ApplySoundMappingsAsync()
    {
        if (_engine is null || _player is null)
        {
            return;
        }

        int generation = ++_soundGeneration;
        PercussionSoundPreset preset = _soundPreset;
        int rate = _engine.SampleRate;
        IReadOnlyDictionary<Cr78Instrument, Sample> samples = await preset.PrepareAsync(rate, _lifetime.Token);
        if (generation != _soundGeneration || _closing || _player is null || _engine is null || _engine.SampleRate != rate)
        {
            return;
        }

        _preparedSamples = samples;
        _preparedRate = rate;
        _player.SetSamples(samples);
    }

    /// <summary>
    ///  Publishes prepared samples, and drops any previous map if the owned WAV files cannot be decoded.
    /// </summary>
    private async Task ApplyOwnedSamplesAsync()
    {
        try
        {
            await ApplySoundMappingsAsync();
        }
        catch
        {
            _preparedSamples = null;
            _preparedRate = 0;
            if (_player is not null && AudioAvailable)
            {
                _player.SetSamples(new Dictionary<Cr78Instrument, Sample>());
            }

            throw;
        }
    }

    /// <summary>
    ///  Reapplies the already prepared map after a score or transport reset so imports survive document edits.
    /// </summary>
    private void ReapplyPreparedSamples()
    {
        if (_player is null || _engine is null || _preparedSamples is null || _engine.SampleRate != _preparedRate)
        {
            return;
        }

        _player.SetSamples(_preparedSamples);
    }

    private void RefreshSoundStatus()
    {
        if (_soundStatusLabel is null)
        {
            return;
        }

        if (!TryGetCurrentInstrument(out Cr78Instrument instrument))
        {
            _soundStatusLabel.Text = $"Sounds: {_soundPreset.Name}{(SoundPresetIsDirty ? " (unsaved)" : "")}";
            return;
        }

        string source = _soundPreset.WaveFiles.TryGetValue(instrument, out string? path)
            ? Path.GetFileName(path)
            : "synthesis";
        string dirty = SoundPresetIsDirty ? " (unsaved)" : "";
        _soundStatusLabel.Text = $"{Cr78Kit.GetDisplayName(instrument)}: {source}{dirty}";
    }

    private bool TryGetCurrentInstrument(out Cr78Instrument instrument)
    {
        if (_stepGrid.CurrentRow?.Tag is Cr78Instrument value && Cr78Kit.Instruments.Contains(value))
        {
            instrument = value;
            return true;
        }

        instrument = default;
        return false;
    }

    private void UpdateSoundCommandState(bool editing)
    {
        bool hasTrack = TryGetCurrentInstrument(out _);
        _importWavMenuItem.Enabled = _importWavButton.Enabled = editing && hasTrack;
        _resetSoundMenuItem.Enabled = _resetSoundButton.Enabled = editing && hasTrack;
        _loadSoundPresetMenuItem.Enabled = _loadSoundPresetButton.Enabled = editing;
        _saveSoundPresetMenuItem.Enabled = _saveSoundPresetButton.Enabled = editing;
    }

    private static string FirstExistingDirectory(params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (!string.IsNullOrWhiteSpace(candidate) && Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Equals("Stock", StringComparison.OrdinalIgnoreCase))
        {
            return "Sounds";
        }

        char[] invalid = Path.GetInvalidFileNameChars();
        string trimmed = name.Trim();
        return trimmed.IndexOfAny(invalid) >= 0
            ? "Sounds"
            : trimmed;
    }
}
