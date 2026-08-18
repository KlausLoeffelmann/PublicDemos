using Microsoft.Extensions.Logging;

namespace WarpClock.App;

public partial class FormMain
{
    private ThemeScheduleDocument? _themeSchedule;
    private System.Windows.Forms.Timer? _themeScheduleTimer;
    private string? _currentThemeListPath;
    private string? _defaultThemeListPath;

    private void LoadThemeSchedule()
    {
        IReadOnlyList<ThemeCatalogInfo> catalog = GetThemeCatalogSnapshot();
        _defaultThemeListPath = NormalizeThemeListPath(_loadedAppState?.Theme.DefaultThemeListPath) ?? _appPaths.ThemeListPath;
        _currentThemeListPath = NormalizeThemeListPath(_loadedAppState?.Theme.CurrentThemeListPath) ?? _defaultThemeListPath;

        if (_themeListStore is null)
        {
            _themeSchedule = ThemeListDefaults.CreateDefault(catalog);
            ScheduleNextThemeRotation();
            return;
        }

        EnsureDefaultThemeListExists(catalog);

        ThemeScheduleDocument? document = TryLoadThemeListForStartup(_currentThemeListPath, catalog);
        if (document is null)
        {
            _currentThemeListPath = _defaultThemeListPath;
            document = _themeListStore.EnsureDefaultAtPath(_defaultThemeListPath!, catalog);
        }

        _themeSchedule = document;
        ScheduleNextThemeRotation();
    }

    private ThemeSchedulePeriod GetCurrentThemePeriod()
    {
        ThemeScheduleDocument schedule = _themeSchedule ?? new ThemeScheduleDocument();
        schedule.Normalize();
        return ThemeSchedulePlanner.GetCurrentPeriod(
            DateTime.Now,
            schedule.DayStartsAt ?? ThemeScheduleDocument.DefaultDayStartsAt,
            schedule.NightStartsAt ?? ThemeScheduleDocument.DefaultNightStartsAt);
    }

    private ThemeSelection? TryGetScheduledThemeSelection()
    {
        if (_themeSchedule is null || _themes.Count == 0)
        {
            return null;
        }

        ThemeReference? reference = ThemeSchedulePlanner.SelectTheme(
            _themeSchedule,
            GetThemeCatalogSnapshot(),
            DateTime.Now);

        return TryResolveThemeReference(reference, out ThemeSelection? selection)
            ? selection
            : null;
    }

    private void ScheduleNextThemeRotation()
    {
        _themeScheduleTimer?.Stop();

        if (_themeScheduleTimer is null || _themeSchedule is null)
        {
            return;
        }

        DateTime? nextChange = ThemeSchedulePlanner.GetNextChangeTime(
            _themeSchedule,
            GetThemeCatalogSnapshot(),
            DateTime.Now);

        if (nextChange is null)
        {
            return;
        }

        double interval = Math.Ceiling((nextChange.Value - DateTime.Now).TotalMilliseconds);
        _themeScheduleTimer.Interval = (int)Math.Clamp(interval, 1_000d, int.MaxValue);
        _themeScheduleTimer.Start();
    }

    private void OnThemeScheduleTimerTick(object? sender, EventArgs e)
    {
        _themeScheduleTimer?.Stop();

        try
        {
            ThemeSelection? selection = TryGetScheduledThemeSelection();
            if (selection is not null)
            {
                SelectTheme(selection, ThemeSelectionReason.Scheduled, ShouldApplyThemeDefaultsOnThemeChange());
            }
            else
            {
                ApplyEffectiveThemeInfoMode();
            }
        }
        finally
        {
            ScheduleNextThemeRotation();
        }
    }

    private void OnCreateNewThemeListClick(object? sender, EventArgs e)
    {
        ThemeScheduleDocument document = ThemeListDefaults.CreateDefault(GetThemeCatalogSnapshot());
        document.Name = "WarpClock Theme List";
        EditAndSaveThemeList(document, currentPath: null, requireSaveAs: true);
    }

    private void OnEditCurrentThemeListClick(object? sender, EventArgs e)
    {
        ThemeScheduleDocument document = (_themeSchedule ?? ThemeListDefaults.CreateDefault(GetThemeCatalogSnapshot())).Clone();
        EditAndSaveThemeList(document, _currentThemeListPath, requireSaveAs: false);
    }

    private void OnLoadThemeListClick(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog = CreateThemeListOpenDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string path = NormalizeThemeListPath(dialog.FileName)
            ?? dialog.FileName;

        try
        {
            ThemeScheduleDocument document = _themeListStore is null
                ? ThemeListDefaults.CreateDefault(GetThemeCatalogSnapshot())
                : _themeListStore.LoadFromPath(path, GetThemeCatalogSnapshot());

            ApplyAndPersistThemeList(document, path, useAsDefaultPath: string.Equals(path, _defaultThemeListPath, StringComparison.OrdinalIgnoreCase));
            _statusInfo.Text = $"Loaded themelist '{document.Name}'.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not load WarpClock themelist from {Path}.", path);
            _statusInfo.Text = $"Themelist load failed: {ex.Message}";
            AppMessageDialog.ShowMessage(
                this,
                "WarpClock - Themelist load failed",
                "WarpClock could not load the selected themelist.",
                ex.Message + Environment.NewLine + Environment.NewLine + path);
        }
    }

    private void OnSaveThemeListClick(object? sender, EventArgs e)
    {
        if (_themeSchedule is null)
        {
            _statusInfo.Text = "There is no themelist to save.";
            return;
        }

        if (TrySaveThemeListDocument(_themeSchedule.Clone(), _currentThemeListPath, IsCurrentThemeListDefault(), requireSaveAs: true, out string? savedPath))
        {
            _currentThemeListPath = savedPath;
            PersistCurrentAppState();
            _statusInfo.Text = $"Saved themelist '{_themeSchedule.Name}'.";
        }
    }

    private void EditAndSaveThemeList(ThemeScheduleDocument document, string? currentPath, bool requireSaveAs)
    {
        using ThemeListEditorDialog dialog = new(document, GetThemeCatalogSnapshot(), currentPath, _defaultThemeListPath);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!TrySaveThemeListDocument(dialog.EditedDocument.Clone(), currentPath, dialog.UseAsDefaultOnStartup, requireSaveAs, out string? savedPath))
        {
            return;
        }

        ApplyAndPersistThemeList(dialog.EditedDocument, savedPath, dialog.UseAsDefaultOnStartup);
        _statusInfo.Text = $"Saved themelist '{dialog.EditedDocument.Name}'.";
    }

    private void ApplyAndPersistThemeList(ThemeScheduleDocument document, string? currentPath, bool useAsDefaultPath)
    {
        _themeSchedule = document.Clone();
        _currentThemeListPath = currentPath;

        if (useAsDefaultPath)
        {
            _defaultThemeListPath = currentPath;
        }
        else if (!string.IsNullOrWhiteSpace(currentPath)
            && string.Equals(currentPath, _defaultThemeListPath, StringComparison.OrdinalIgnoreCase))
        {
            _defaultThemeListPath = _appPaths.ThemeListPath;
            EnsureDefaultThemeListExists(GetThemeCatalogSnapshot());
        }

        ScheduleNextThemeRotation();

        ThemeSelection? scheduled = TryGetScheduledThemeSelection();
        if (scheduled is not null)
        {
            SelectTheme(scheduled, ThemeSelectionReason.Scheduled, ShouldApplyThemeDefaultsOnThemeChange());
        }
        else
        {
            ApplyEffectiveThemeInfoMode();
        }

        PersistCurrentAppState();
        _propertyGrid.Refresh();
    }

    private bool TrySaveThemeListDocument(
        ThemeScheduleDocument document,
        string? currentPath,
        bool useAsDefaultPath,
        bool requireSaveAs,
        out string? savedPath)
    {
        savedPath = NormalizeThemeListPath(currentPath);
        if (requireSaveAs || string.IsNullOrWhiteSpace(savedPath))
        {
            using SaveFileDialog dialog = CreateThemeListSaveDialog(document, savedPath);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                _statusInfo.Text = "Themelist save canceled.";
                return false;
            }

            savedPath = NormalizeThemeListPath(dialog.FileName) ?? dialog.FileName;
        }

        try
        {
            if (_themeListStore is null)
            {
                document.Normalize();
            }
            else
            {
                _themeListStore.SaveToPath(savedPath!, document);
            }

            if (useAsDefaultPath)
            {
                _defaultThemeListPath = savedPath;
            }

            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not save WarpClock themelist to {Path}.", savedPath);
            _statusInfo.Text = $"Themelist save failed: {ex.Message}";
            AppMessageDialog.ShowMessage(
                this,
                "WarpClock - Themelist save failed",
                "WarpClock could not save the current themelist.",
                ex.Message + Environment.NewLine + Environment.NewLine + savedPath);
            return false;
        }
    }

    private ThemeScheduleDocument? TryLoadThemeListForStartup(string? path, IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        if (_themeListStore is null || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            return _themeListStore.LoadFromPath(path, catalog);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not restore themelist from {Path}; falling back to the default themelist.", path);
            return null;
        }
    }

    private void EnsureDefaultThemeListExists(IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        if (_themeListStore is null)
        {
            return;
        }

        _defaultThemeListPath ??= _appPaths.ThemeListPath;

        try
        {
            _themeListStore.EnsureDefaultAtPath(_defaultThemeListPath, catalog);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not ensure the default themelist at {Path}.", _defaultThemeListPath);
        }
    }

    private bool IsCurrentThemeListDefault()
        => !string.IsNullOrWhiteSpace(_currentThemeListPath)
            && string.Equals(_currentThemeListPath, _defaultThemeListPath, StringComparison.OrdinalIgnoreCase);

    private OpenFileDialog CreateThemeListOpenDialog()
    {
        OpenFileDialog dialog = new()
        {
            CheckFileExists = true,
            Filter = "WarpClock themelist (*.json)|*.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = GetThemeListInitialDirectory(_currentThemeListPath ?? _defaultThemeListPath),
            Title = "Load WarpClock themelist",
        };

        return dialog;
    }

    private SaveFileDialog CreateThemeListSaveDialog(ThemeScheduleDocument document, string? currentPath)
    {
        SaveFileDialog dialog = new()
        {
            AddExtension = true,
            DefaultExt = "json",
            Filter = "WarpClock themelist (*.json)|*.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = GetThemeListInitialDirectory(currentPath ?? _defaultThemeListPath),
            OverwritePrompt = true,
            Title = "Save WarpClock themelist",
        };

        if (!string.IsNullOrWhiteSpace(currentPath))
        {
            dialog.FileName = Path.GetFileName(currentPath);
        }
        else
        {
            dialog.FileName = MakeSafeFileName(document.Name) + ".json";
        }

        return dialog;
    }

    private string GetThemeListInitialDirectory(string? candidatePath)
    {
        string? path = NormalizeThemeListPath(candidatePath);
        if (!string.IsNullOrWhiteSpace(path))
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        Directory.CreateDirectory(_appPaths.RootDirectory);
        return _appPaths.RootDirectory;
    }

    private static string MakeSafeFileName(string name)
    {
        string trimmed = string.IsNullOrWhiteSpace(name)
            ? "WarpClock Theme List"
            : name.Trim();

        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            trimmed = trimmed.Replace(invalid, '-');
        }

        return trimmed;
    }

    private static string? NormalizeThemeListPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            return Path.GetFullPath(path.Trim());
        }
        catch (Exception) when (path is not null)
        {
            return path.Trim();
        }
    }
}
