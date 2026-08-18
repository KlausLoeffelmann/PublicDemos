namespace WarpClock.App;

/// <summary>
///  Centralizes the on-disk locations used by the hosted app.
/// </summary>
public sealed class AppPaths
{
    private const string AppFolderName = "WarpClock";

    public AppPaths()
    {
        string root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppFolderName);

        RootDirectory = root;
        SettingsPath = Path.Combine(root, "settings.json");
        ThemeListPath = Path.Combine(root, "themelist.json");
        LegacyWindowSettingsPath = Path.Combine(root, "window.json");
        LogDirectory = Path.Combine(root, "Logs");
        DiagnosticsDirectory = Path.Combine(root, "Diagnostics");
        PluginDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
    }

    /// <summary>The roaming root folder for WarpClock app data.</summary>
    public string RootDirectory { get; }

    /// <summary>The versioned persisted UI-state file.</summary>
    public string SettingsPath { get; }

    /// <summary>The persisted theme-list/scheduler file.</summary>
    public string ThemeListPath { get; }

    /// <summary>The legacy pre-migration window settings file.</summary>
    public string LegacyWindowSettingsPath { get; }

    /// <summary>The rolling application log directory.</summary>
    public string LogDirectory { get; }

    /// <summary>The diagnostics output directory.</summary>
    public string DiagnosticsDirectory { get; }

    /// <summary>The runtime plug-in probing directory.</summary>
    public string PluginDirectory { get; }

    /// <summary>
    ///  Creates a timestamped diagnostics run directory under <see cref="DiagnosticsDirectory"/>.
    /// </summary>
    public string CreateDiagnosticsRunDirectory(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        return Path.Combine(
            DiagnosticsDirectory,
            $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}");
    }
}
