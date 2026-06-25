namespace LargeFormSmokeTest.Settings;

using System.Text.Json;
using System.Text.Json.Serialization;
using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Theming;

/// <summary>
///  Persisted user choices (UI language and color theme). Stored as a small JSON file under
///  <c>%AppData%\LargeFormSmokeTest\settings.json</c> so the harness reopens in the last state.
/// </summary>
public sealed class AppSettings
{
    private static readonly string s_settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LargeFormSmokeTest",
        "settings.json");

    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>Gets or sets the persisted UI language.</summary>
    public AppLanguage Language { get; set; } = AppLanguage.English;

    /// <summary>Gets or sets the persisted color theme.</summary>
    public AppTheme Theme { get; set; } = AppTheme.Classic;

    /// <summary>
    ///  Loads the settings from disk, returning defaults when the file is absent or unreadable.
    /// </summary>
    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(s_settingsPath))
            {
                string json = File.ReadAllText(s_settingsPath);

                return JsonSerializer.Deserialize<AppSettings>(json, s_options) ?? new AppSettings();
            }
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            // A corrupt or locked settings file must never prevent the app from starting.
        }

        return new AppSettings();
    }

    /// <summary>
    ///  Persists the current settings to disk, swallowing IO errors (the harness must keep
    ///  running even when the settings directory is not writable).
    /// </summary>
    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(s_settingsPath)!);
            File.WriteAllText(s_settingsPath, JsonSerializer.Serialize(this, s_options));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Best-effort persistence only.
        }
    }
}
