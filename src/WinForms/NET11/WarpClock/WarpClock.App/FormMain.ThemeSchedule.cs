using Microsoft.Extensions.Logging;

namespace WarpClock.App;

public partial class FormMain
{
    private ThemeScheduleDocument? _themeSchedule;
    private System.Windows.Forms.Timer? _themeScheduleTimer;
    private string? _currentThemeSetPath;
    private string? _defaultThemeSetPath;

    private void LoadThemeSchedule()
    {
        IReadOnlyList<ThemeCatalogInfo> catalog = GetThemeCatalogSnapshot();
        _defaultThemeSetPath = NormalizeThemeSetPath(_loadedAppState?.Theme.DefaultThemeSetPath) ?? _appPaths.ThemeSetPath;
        _currentThemeSetPath = NormalizeThemeSetPath(_loadedAppState?.Theme.CurrentThemeSetPath) ?? _defaultThemeSetPath;

        if (_themeSetStore is null)
        {
            _themeSchedule = ThemeSetDefaults.CreateDefault(catalog);
            ScheduleNextThemeRotation();
            return;
        }

        MigrateLegacyDefaultThemeSetIfNeeded(catalog);
        EnsureDefaultThemeSetExists(catalog);

        ThemeScheduleDocument? document = TryLoadThemeSetForStartup(_currentThemeSetPath, catalog);
        if (document is null)
        {
            _currentThemeSetPath = _defaultThemeSetPath;
            document = _themeSetStore.EnsureDefaultAtPath(_defaultThemeSetPath!, catalog);
        }

        _themeSchedule = document;
        ScheduleNextThemeRotation();
    }

    private void MigrateLegacyDefaultThemeSetIfNeeded(IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        if (_themeSetStore is null)
        {
            return;
        }

        string legacyPath = NormalizeThemeSetPath(_appPaths.LegacyThemeListPath)
            ?? _appPaths.LegacyThemeListPath;
        string canonicalPath = NormalizeThemeSetPath(_appPaths.ThemeSetPath)
            ?? _appPaths.ThemeSetPath;

        bool legacyExists = File.Exists(legacyPath);
        bool canonicalExists = File.Exists(canonicalPath);
        bool currentUsesLegacy = string.Equals(_currentThemeSetPath, legacyPath, StringComparison.OrdinalIgnoreCase);
        bool defaultUsesLegacy = string.Equals(_defaultThemeSetPath, legacyPath, StringComparison.OrdinalIgnoreCase);

        if (!legacyExists)
        {
            if (defaultUsesLegacy)
            {
                _defaultThemeSetPath = canonicalPath;
            }

            if (currentUsesLegacy)
            {
                _currentThemeSetPath = canonicalPath;
            }

            return;
        }

        if (!canonicalExists)
        {
            try
            {
                _themeSetStore.MigrateLegacyDefaultFile(legacyPath, canonicalPath, catalog);
                _logger.LogInformation(
                    "Migrated the legacy WarpClock themelist at {LegacyPath} to the canonical themeset path {Path}.",
                    legacyPath,
                    canonicalPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "Could not migrate the legacy WarpClock themelist at {Path}.", legacyPath);

                if (string.Equals(_defaultThemeSetPath, _appPaths.ThemeSetPath, StringComparison.OrdinalIgnoreCase))
                {
                    _defaultThemeSetPath = legacyPath;
                }

                if (string.Equals(_currentThemeSetPath, _appPaths.ThemeSetPath, StringComparison.OrdinalIgnoreCase))
                {
                    _currentThemeSetPath = legacyPath;
                }

                return;
            }
        }

        if (defaultUsesLegacy || string.Equals(_defaultThemeSetPath, canonicalPath, StringComparison.OrdinalIgnoreCase))
        {
            _defaultThemeSetPath = canonicalPath;
        }

        if (currentUsesLegacy)
        {
            _currentThemeSetPath = canonicalPath;
        }
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

    private void OnCreateNewThemeSetClick(object? sender, EventArgs e)
    {
        ThemeScheduleDocument document = ThemeSetDefaults.CreateDefault(GetThemeCatalogSnapshot());
        document.Name = "WarpClock Themeset";
        EditAndSaveThemeSet(document, currentPath: null, requireSaveAs: true);
    }

    private void OnEditCurrentThemeSetClick(object? sender, EventArgs e)
    {
        ThemeScheduleDocument document = (_themeSchedule ?? ThemeSetDefaults.CreateDefault(GetThemeCatalogSnapshot())).Clone();
        EditAndSaveThemeSet(document, _currentThemeSetPath, requireSaveAs: false);
    }

    private void OnLoadThemeSetClick(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog = CreateThemeSetOpenDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string path = NormalizeThemeSetPath(dialog.FileName)
            ?? dialog.FileName;

        try
        {
            ThemeScheduleDocument document = _themeSetStore is null
                ? ThemeSetDefaults.CreateDefault(GetThemeCatalogSnapshot())
                : _themeSetStore.LoadFromPath(path, GetThemeCatalogSnapshot());

            ApplyAndPersistThemeSet(document, path, useAsDefaultPath: string.Equals(path, _defaultThemeSetPath, StringComparison.OrdinalIgnoreCase));
            _statusInfo.Text = $"Loaded themeset '{document.Name}'.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not load WarpClock themeset from {Path}.", path);
            _statusInfo.Text = $"Themeset load failed: {ex.Message}";
            AppMessageDialog.ShowMessage(
                this,
                "WarpClock - Themeset load failed",
                "WarpClock could not load the selected themeset.",
                ex.Message + Environment.NewLine + Environment.NewLine + path);
        }
    }

    private void OnSaveThemeSetClick(object? sender, EventArgs e)
    {
        if (_themeSchedule is null)
        {
            _statusInfo.Text = "There is no themeset to save.";
            return;
        }

        if (TrySaveThemeSetDocument(_themeSchedule.Clone(), _currentThemeSetPath, IsCurrentThemeSetDefault(), requireSaveAs: true, out string? savedPath))
        {
            _currentThemeSetPath = savedPath;
            PersistCurrentAppState();
            _statusInfo.Text = $"Saved themeset '{_themeSchedule.Name}'.";
        }
    }

    private void EditAndSaveThemeSet(ThemeScheduleDocument document, string? currentPath, bool requireSaveAs)
    {
        using ThemeSetEditorDialog dialog = new(document, GetThemeCatalogSnapshot(), currentPath, _defaultThemeSetPath);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!TrySaveThemeSetDocument(dialog.EditedDocument.Clone(), currentPath, dialog.UseAsDefaultOnStartup, requireSaveAs, out string? savedPath))
        {
            return;
        }

        ApplyAndPersistThemeSet(dialog.EditedDocument, savedPath, dialog.UseAsDefaultOnStartup);
        _statusInfo.Text = $"Saved themeset '{dialog.EditedDocument.Name}'.";
    }

    private void ApplyAndPersistThemeSet(ThemeScheduleDocument document, string? currentPath, bool useAsDefaultPath)
    {
        _themeSchedule = document.Clone();
        _currentThemeSetPath = currentPath;

        if (useAsDefaultPath)
        {
            _defaultThemeSetPath = currentPath;
        }
        else if (!string.IsNullOrWhiteSpace(currentPath)
            && string.Equals(currentPath, _defaultThemeSetPath, StringComparison.OrdinalIgnoreCase))
        {
            _defaultThemeSetPath = _appPaths.ThemeSetPath;
            EnsureDefaultThemeSetExists(GetThemeCatalogSnapshot());
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

    private bool TrySaveThemeSetDocument(
        ThemeScheduleDocument document,
        string? currentPath,
        bool useAsDefaultPath,
        bool requireSaveAs,
        out string? savedPath)
    {
        savedPath = NormalizeThemeSetPath(currentPath);
        if (requireSaveAs
            || string.IsNullOrWhiteSpace(savedPath)
            || !HasThemeSetFileExtension(savedPath))
        {
            using SaveFileDialog dialog = CreateThemeSetSaveDialog(document, savedPath);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                _statusInfo.Text = "Themeset save canceled.";
                return false;
            }

            savedPath = NormalizeThemeSetPath(dialog.FileName) ?? dialog.FileName;
        }

        try
        {
            if (_themeSetStore is null)
            {
                document.Normalize();
            }
            else
            {
                _themeSetStore.SaveToPath(savedPath!, document);
            }

            if (useAsDefaultPath)
            {
                _defaultThemeSetPath = savedPath;
            }

            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not save WarpClock themeset to {Path}.", savedPath);
            _statusInfo.Text = $"Themeset save failed: {ex.Message}";
            AppMessageDialog.ShowMessage(
                this,
                "WarpClock - Themeset save failed",
                "WarpClock could not save the current themeset.",
                ex.Message + Environment.NewLine + Environment.NewLine + savedPath);
            return false;
        }
    }

    private ThemeScheduleDocument? TryLoadThemeSetForStartup(string? path, IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        if (_themeSetStore is null || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        try
        {
            return _themeSetStore.LoadFromPath(path, catalog);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not restore themeset from {Path}; falling back to the default themeset.", path);
            return null;
        }
    }

    private void EnsureDefaultThemeSetExists(IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        if (_themeSetStore is null)
        {
            return;
        }

        _defaultThemeSetPath ??= _appPaths.ThemeSetPath;

        try
        {
            _themeSetStore.EnsureDefaultAtPath(_defaultThemeSetPath, catalog);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not ensure the default themeset at {Path}.", _defaultThemeSetPath);
        }
    }

    private bool IsCurrentThemeSetDefault()
        => !string.IsNullOrWhiteSpace(_currentThemeSetPath)
            && string.Equals(_currentThemeSetPath, _defaultThemeSetPath, StringComparison.OrdinalIgnoreCase);

    private OpenFileDialog CreateThemeSetOpenDialog()
    {
        OpenFileDialog dialog = new()
        {
            CheckFileExists = true,
            Filter = "WarpClock themeset (*.themeset.json)|*.themeset.json|Legacy WarpClock theme list (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = GetThemeSetInitialDirectory(_currentThemeSetPath ?? _defaultThemeSetPath),
            Title = "Load WarpClock themeset",
        };

        return dialog;
    }

    private SaveFileDialog CreateThemeSetSaveDialog(ThemeScheduleDocument document, string? currentPath)
    {
        SaveFileDialog dialog = new()
        {
            AddExtension = true,
            DefaultExt = "themeset.json",
            Filter = "WarpClock themeset (*.themeset.json)|*.themeset.json|Legacy JSON (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = GetThemeSetInitialDirectory(currentPath ?? _defaultThemeSetPath),
            OverwritePrompt = true,
            Title = "Save WarpClock themeset",
        };

        if (!string.IsNullOrWhiteSpace(currentPath))
        {
            dialog.FileName = GetPreferredThemeSetFileName(currentPath);
        }
        else
        {
            dialog.FileName = MakeSafeFileName(document.Name) + ".themeset.json";
        }

        return dialog;
    }

    private string GetThemeSetInitialDirectory(string? candidatePath)
    {
        string? path = NormalizeThemeSetPath(candidatePath);
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
            ? "WarpClock Themeset"
            : name.Trim();

        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            trimmed = trimmed.Replace(invalid, '-');
        }

        return trimmed;
    }

    private static string GetPreferredThemeSetFileName(string currentPath)
    {
        string fileName = Path.GetFileName(currentPath);
        return HasThemeSetFileExtension(fileName)
            ? fileName
            : Path.GetFileNameWithoutExtension(currentPath) + ".themeset.json";
    }

    private static bool HasThemeSetFileExtension(string path)
        => path.EndsWith(".themeset.json", StringComparison.OrdinalIgnoreCase);

    private static string? NormalizeThemeSetPath(string? path)
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
