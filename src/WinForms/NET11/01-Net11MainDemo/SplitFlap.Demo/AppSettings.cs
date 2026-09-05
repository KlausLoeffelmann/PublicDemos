using System.Text.Json;
using System.Text.Json.Serialization;

namespace SplitFlap.Demo;

/// <summary>
///  Contains the user-adjustable state persisted between application runs.
/// </summary>
internal sealed class AppSettings
{
    /// <summary>
    ///  Gets or sets whether closing the application automatically saves settings.
    /// </summary>
    public bool AutoSave { get; set; } = true;

    /// <summary>
    ///  Gets or sets the normal, restorable window bounds.
    /// </summary>
    public int WindowX { get; set; }

    /// <summary>
    ///  Gets or sets the normal window's top coordinate.
    /// </summary>
    public int WindowY { get; set; }

    /// <summary>
    ///  Gets or sets the normal window width.
    /// </summary>
    public int WindowWidth { get; set; }

    /// <summary>
    ///  Gets or sets the normal window height.
    /// </summary>
    public int WindowHeight { get; set; }

    /// <summary>
    ///  Gets or sets the normal window state; minimized is never persisted.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

    /// <summary>
    ///  Gets or sets the board font family name.
    /// </summary>
    public string FontName { get; set; } = MonospaceFonts.FallbackFamilyName;

    /// <summary>
    ///  Gets or sets the board font size in points.
    /// </summary>
    public float FontSize { get; set; } = 18f;

    /// <summary>
    ///  Gets or sets the board row count.
    /// </summary>
    public int Rows { get; set; } = 9;

    /// <summary>
    ///  Gets or sets the board column count.
    /// </summary>
    public int Columns { get; set; } = 46;

    /// <summary>
    ///  Gets or sets whether window resizing preserves board proportions.
    /// </summary>
    public bool KeepAspectRatio { get; set; } = true;

    /// <summary>
    ///  Gets or sets whether the board controls the form's preferred size.
    /// </summary>
    public bool BoardDictatesSize { get; set; } = true;

    /// <summary>
    ///  Gets or sets the flap animation speed.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FlipAnimationSpeed AnimationSpeed { get; set; } = FlipAnimationSpeed.Medium;

    /// <summary>
    ///  Gets or sets whether sound is enabled.
    /// </summary>
    public bool SoundEnabled { get; set; }

    /// <summary>
    ///  Gets or sets the timetable update interval in seconds.
    /// </summary>
    public int UpdateIntervalSeconds { get; set; } = 30;
}

/// <summary>
///  Loads and atomically saves <see cref="AppSettings"/> as JSON.
/// </summary>
internal static class AppSettingsStore
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    ///  Loads settings, returning defaults when no file exists or the file is invalid.
    /// </summary>
    public static AppSettings Load()
        => Load(AppPaths.SettingsFile);

    /// <summary>
    ///  Loads settings from a specified path, primarily for deterministic tests.
    /// </summary>
    internal static AppSettings Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            if (!File.Exists(path))
            {
                return new AppSettings();
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppSettings>(json, s_options) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            AppLogger.Warning("Settings", $"Could not load '{path}'. Defaults will be used.", ex);
            return new AppSettings();
        }
    }

    /// <summary>
    ///  Saves settings through a temporary file so interruption cannot truncate the last good copy.
    /// </summary>
    public static void Save(AppSettings settings)
        => Save(settings, AppPaths.SettingsFile);

    /// <summary>
    ///  Saves settings to a specified path, primarily for deterministic tests.
    /// </summary>
    internal static void Save(AppSettings settings, string path)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string directory = Path.GetDirectoryName(path)
            ?? throw new ArgumentException("The settings path must have a directory.", nameof(path));
        Directory.CreateDirectory(directory);
        string temporaryPath = path + ".tmp";

        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(settings, s_options));
            File.Move(temporaryPath, path, overwrite: true);
            AppLogger.Information("Settings", $"Saved '{path}'.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            AppLogger.Error("Settings", $"Could not save '{path}'.", ex);
            throw;
        }
        finally
        {
            try
            {
                File.Delete(temporaryPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AppLogger.Warning("Settings", $"Could not remove temporary file '{temporaryPath}'.", ex);
            }
        }
    }
}
