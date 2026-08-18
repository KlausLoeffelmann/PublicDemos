using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace WarpClock.App;

/// <summary>
///  Loads, migrates, and saves the versioned persisted UI state.
/// </summary>
public sealed class AppStateStore
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
        },
    };

    private readonly AppPaths _paths;
    private readonly ILogger<AppStateStore> _logger;

    public AppStateStore(AppPaths paths, ILogger<AppStateStore> logger)
    {
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(logger);

        _paths = paths;
        _logger = logger;
    }

    public PersistedAppState? Load()
    {
        if (File.Exists(_paths.SettingsPath))
        {
            return LoadCurrentSettings();
        }

        if (File.Exists(_paths.LegacyWindowSettingsPath))
        {
            return MigrateLegacyWindowSettings();
        }

        return null;
    }

    public void Save(PersistedAppState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        state.Normalize();
        WriteJsonFile(_paths.SettingsPath, state);
    }

    private PersistedAppState? LoadCurrentSettings()
    {
        try
        {
            PersistedAppState state = ReadJsonFile<PersistedAppState>(_paths.SettingsPath);
            state.Normalize();
            return state;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            BackupUnreadableFile(_paths.SettingsPath);
            _logger.LogWarning(ex, "Could not restore WarpClock UI settings from {Path}.", _paths.SettingsPath);
            return null;
        }
    }

    private PersistedAppState? MigrateLegacyWindowSettings()
    {
        try
        {
            LegacyWindowSettings legacy = ReadJsonFile<LegacyWindowSettings>(_paths.LegacyWindowSettingsPath);
            PersistedAppState migrated = new()
            {
                Window = new PersistedWindowSettings
                {
                    X = legacy.X,
                    Y = legacy.Y,
                    Width = legacy.Width,
                    Height = legacy.Height,
                    PresentationMode = legacy.Mode,
                    ToggleFullScreenKeys = legacy.ToggleFullScreenKeys,
                    AlwaysOn = legacy.AlwaysOn,
                    EscapeExitsFullScreen = legacy.EscapeExitsFullScreen,
                    MousePointerAutoHideDelay = legacy.MousePointerAutoHideDelay,
                    TopMostInFullScreen = legacy.TopMostInFullScreen,
                },
            };

            migrated.Normalize();

            try
            {
                Save(migrated);
                _logger.LogInformation(
                    "Migrated legacy window settings from {LegacyPath} to {SettingsPath}.",
                    _paths.LegacyWindowSettingsPath,
                    _paths.SettingsPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                _logger.LogWarning(ex, "Migrated legacy WarpClock settings but could not persist the new settings file.");
            }

            return migrated;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            BackupUnreadableFile(_paths.LegacyWindowSettingsPath);
            _logger.LogWarning(ex, "Could not restore legacy WarpClock window settings from {Path}.", _paths.LegacyWindowSettingsPath);
            return null;
        }
    }

    private static T ReadJsonFile<T>(string path)
    {
        string json = File.ReadAllText(path);
        T? result = JsonSerializer.Deserialize<T>(json, s_jsonOptions);

        return result ?? throw new JsonException($"The file '{path}' contained null JSON content.");
    }

    private static void WriteJsonFile<T>(string path, T value)
    {
        string directory = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException($"Path '{path}' does not contain a directory.");

        Directory.CreateDirectory(directory);

        string tempPath = path + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(value, s_jsonOptions));
        File.Move(tempPath, path, overwrite: true);
    }

    private void BackupUnreadableFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return;
            }

            string backupPath = Path.Combine(
                Path.GetDirectoryName(path) ?? _paths.RootDirectory,
                $"{Path.GetFileNameWithoutExtension(path)}.corrupt-{DateTime.Now:yyyyMMdd-HHmmss}{Path.GetExtension(path)}");

            File.Move(path, backupPath, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(ex, "Could not preserve unreadable settings file {Path}.", path);
        }
    }

    private sealed class LegacyWindowSettings
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public WindowPresentationMode Mode { get; set; }

        public Keys ToggleFullScreenKeys { get; set; } = Keys.Control | Keys.Shift | Keys.Return;

        public bool AlwaysOn { get; set; }

        public bool EscapeExitsFullScreen { get; set; } = true;

        public int MousePointerAutoHideDelay { get; set; } = 5_000;

        public bool TopMostInFullScreen { get; set; } = true;
    }
}
