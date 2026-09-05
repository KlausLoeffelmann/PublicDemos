using DrumMachine.Demo.Documents;

namespace DrumMachine.Demo;

partial class MainForm
{
    private async void New_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(async () =>
        {
            using NewLoopDialog dialog = new();
            if (dialog.ShowDialog(this) != DialogResult.OK || !await ConfirmDiscardAsync())
            {
                return;
            }
            ReplaceDocument(LoopDocument.CreateEmpty(dialog.BarCount), path: null);
        });

    private async void Open_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(async () =>
        {
            _openDialog.InitialDirectory = GetDialogDirectory();
            _openDialog.FileName = "";
            if (_openDialog.ShowDialog(this) == DialogResult.OK)
            {
                await OpenPathAsync(_openDialog.FileName);
            }
        });

    private async void Save_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(async () => { await SaveCoreAsync(saveAs: false); });

    private async void SaveAs_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(async () => { await SaveCoreAsync(saveAs: true); });

    private void Quit_Click(object? sender, EventArgs e) => Close();

    private async void Recent_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: string path })
        {
            await RunDocumentCommandAsync(() => OpenPathAsync(path));
        }
    }

    private async Task RunDocumentCommandAsync(Func<Task> operation)
    {
        if (_busy || _closing)
        {
            return;
        }
        CommitEdits();
        _busy = true;
        UpdateCommandState();
        try
        {
            await operation();
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
        {
            // Closing an unattended scenario cancels its outstanding file operation.
        }
        catch (Exception ex)
        {
            ReportDocumentError(ex);
        }
        finally
        {
            _busy = false;
            if (!_closing)
            {
                UpdateCommandState();
            }
        }
    }

    private async Task OpenPathAsync(string path)
    {
        // Do not stop the currently playing document just to find out that a selected
        // file is corrupt or unavailable. Validate the complete replacement first.
        LoopDocument replacement;
        try
        {
            replacement = await LoopDocumentStore.LoadAsync(path, _lifetime.Token);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            _settings = _settings.WithRemovedRecentFile(path);
            RefreshRecentFiles();
            await PersistPreferencesAsync();
            throw;
        }

        bool hadUnsavedChanges = _session.IsDirty;
        if (!await ConfirmDiscardAsync())
        {
            return;
        }
        string fullPath = Path.GetFullPath(path);
        if (hadUnsavedChanges && !_session.IsDirty
            && string.Equals(fullPath, _session.FilePath, StringComparison.OrdinalIgnoreCase))
        {
            // Reopening the current path after choosing Save must load that newly written
            // snapshot, not the older bytes we validated before the unsaved-change prompt.
            replacement = await LoopDocumentStore.LoadAsync(fullPath, _lifetime.Token);
        }
        ReplaceDocument(replacement, fullPath);
        await RecordRecentAsync(fullPath);
        _statusLabel.Text = $"Opened {Path.GetFileName(fullPath)}.";
        AppLogger.Information("Document", $"Opened '{fullPath}'.");
    }

    private void ReplaceDocument(LoopDocument document, string? path)
    {
        _session.Replace(document, path);
        // The audio side receives one reset/configuration transaction, not independently
        // visible property changes that could play part of the previous score.
        ApplyToPlayer(reset: true);
        RefreshDocumentControls(rebuildGrid: true, selectedPage: 0);
    }

    private async Task<bool> ConfirmDiscardAsync()
    {
        CommitEdits();
        if (!_session.IsDirty)
        {
            return true;
        }

        DialogResult answer = MessageBox.Show(
            this, "Save changes to the current loop?", "Unsaved loop",
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        return answer switch
        {
            DialogResult.Yes => await SaveCoreAsync(saveAs: false),
            DialogResult.No => true,
            _ => false
        };
    }

    private async Task<bool> SaveCoreAsync(bool saveAs)
    {
        CommitEdits();
        string? path = _session.FilePath;
        if (saveAs || path is null)
        {
            _saveDialog.InitialDirectory = GetDialogDirectory();
            _saveDialog.FileName = path is null ? "Untitled.drumloop.json" : Path.GetFileName(path);
            if (_saveDialog.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }
            path = _saveDialog.FileName;
        }

        LoopDocument snapshot = _session.Current;
        try
        {
            await LoopDocumentStore.SaveAsync(snapshot, path, _lifetime.Token);
            _session.MarkSaved(snapshot, path);
            await RecordRecentAsync(path);
            _statusLabel.Text = $"Saved {Path.GetFileName(path)}.";
            AppLogger.Information("Document", $"Saved '{path}'.");
            return true;
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
        {
            return false;
        }
        catch (Exception ex)
        {
            ReportDocumentError(ex);
            return false;
        }
    }

    private string GetDialogDirectory()
    {
        if (_session.FilePath is { } path)
        {
            return Path.GetDirectoryName(path) ?? _settings.DefaultFolder;
        }
        return _settings.DefaultFolder;
    }

    private void ReportDocumentError(Exception exception)
    {
        AppLogger.Error("Document", "The document operation did not complete.", exception);
        if (_closing)
        {
            return;
        }
        _statusLabel.Text = $"Document unchanged: {exception.Message}";
        if (Automated)
        {
            Environment.ExitCode = 1;
            Close();
        }
        else
        {
            MessageBox.Show(
                this, exception.Message, "Loop file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task RecordRecentAsync(string path)
    {
        _settings = _settings.WithRecentFile(path);
        RefreshRecentFiles();
        await PersistPreferencesAsync();
    }

    private void RefreshRecentFiles()
    {
        // These are data-driven menu entries. No filesystem probing happens while a menu opens.
        foreach (ToolStripItem item in _recentMenu.DropDownItems.Cast<ToolStripItem>().ToArray())
        {
            _recentMenu.DropDownItems.Remove(item);
            if (!ReferenceEquals(item, _recentEmpty))
            {
                item.Dispose();
            }
        }

        if (_settings.RecentFiles.Count == 0)
        {
            _recentMenu.DropDownItems.Add(_recentEmpty);
            return;
        }

        for (int i = 0; i < _settings.RecentFiles.Count; i++)
        {
            string path = _settings.RecentFiles[i];
            string filename = Path.GetFileName(path);
            bool duplicate = _settings.RecentFiles.Count(
                other => string.Equals(Path.GetFileName(other), filename, StringComparison.OrdinalIgnoreCase)) > 1;
            string label = duplicate ? $"{filename} ({Path.GetDirectoryName(path)})" : filename;
            ToolStripMenuItem item = new($"&{i + 1} {label.Replace("&", "&&", StringComparison.Ordinal)}")
            {
                Name = $"recentFile{i + 1}",
                Tag = path,
                ToolTipText = path
            };
            item.Click += Recent_Click;
            _recentMenu.DropDownItems.Add(item);
        }
    }

    private async void Options_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(async () =>
        {
            using OptionsDialog dialog = new(_settings);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            AppSettings chosen = dialog.Result;
            // Persist before applying the choice: an unsuccessful Options save is not an
            // implicit acceptance or a modification to the musical document.
            if (!IgnoreSettings)
            {
                await AppSettingsStore.SaveAsync(chosen, null, _lifetime.Token);
            }
            _settings = chosen;
            RebuildIcons();
            _statusLabel.Text = chosen.Theme != _appliedTheme
                ? "Options saved. Restart required to apply the selected theme."
                : "Options saved. Folder and toolbar icon size applied.";
        });

    private async Task PersistPreferencesAsync()
    {
        if (IgnoreSettings || _closing)
        {
            return;
        }
        try
        {
            await AppSettingsStore.SaveAsync(_settings, null, _lifetime.Token);
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            // Preferences are separate from a successfully saved/opened document.
            // Report the loss of preference persistence without undoing that file operation.
            AppLogger.Error("Settings", "Could not save application preferences.", ex);
            _statusLabel.Text = $"Preferences could not be saved. Logs: {AppPaths.LogDirectory}";
            if (!Automated)
            {
                MessageBox.Show(this, ex.Message, "Application settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
