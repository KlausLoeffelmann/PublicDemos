using System.Text.Json;
using DrumMachine.Demo.Documents;

namespace DrumMachine.Demo;

/// <summary>
///  Loads explicit startup defaults on invalid preferences and atomically persists successfully validated choices.
/// </summary>
internal static class AppSettingsStore
{
    private const int CurrentVersion = 2;

    /// <summary>
    ///  Bounds startup preference reads independently of larger musical documents.
    /// </summary>
    internal const int MaximumFileBytes = 64 * 1024;

    /// <summary>
    ///  Loads small startup preferences before UI creation, logging malformed or unreadable data and using defaults.
    /// </summary>
    public static AppSettings Load(string? path = null)
    {
        string source = path ?? AppPaths.SettingsFile;
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(source);
            using JsonDocument json = StrictJson.Parse(JsonFileStorage.Read(source, MaximumFileBytes));
            return ReadSettings(json.RootElement).ValidateAndNormalize();
        }
        catch (FileNotFoundException)
        {
            return new AppSettings();
        }
        catch (DirectoryNotFoundException)
        {
            return new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or
            InvalidDataException or JsonException or ArgumentException or NotSupportedException)
        {
            AppLogger.Warning("Settings", $"Could not load '{source}'. Explicit application defaults will be used.", ex);
            return new AppSettings();
        }
    }

    /// <summary>
    ///  Saves a validated settings snapshot and propagates failures so Options never claims an unsuccessful save.
    /// </summary>
    public static Task SaveAsync(
        AppSettings settings,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        AppSettings snapshot = settings.ValidateAndNormalize();
        return JsonFileStorage.WriteAsync(
            path ?? AppPaths.SettingsFile,
            writer => WriteSettings(writer, snapshot),
            MaximumFileBytes,
            createDirectory: true,
            cancellationToken);
    }

    private static AppSettings ReadSettings(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("version", out JsonElement versionElement))
        {
            throw new InvalidDataException("A versioned settings object was expected.");
        }

        int version = StrictJson.Integer(versionElement, "version", 1, CurrentVersion);
        if (version == 1)
        {
            StrictJson.RequireProperties(root,
                "version", "theme", "iconSize", "defaultFolder", "recentFiles", "barsPerView");
        }
        else
        {
            StrictJson.RequireProperties(root,
                "version", "theme", "iconSize", "fontSize", "defaultFolder", "recentFiles", "barsPerView");
        }

        AppTheme theme = StrictJson.String(root.GetProperty("theme"), "theme") switch
        {
            "Classic" => AppTheme.Classic,
            "Dark" => AppTheme.Dark,
            "System" => AppTheme.System,
            _ => throw new InvalidDataException("The application theme is unsupported.")
        };
        ToolbarIconSize iconSize = StrictJson.String(root.GetProperty("iconSize"), "iconSize") switch
        {
            "Small" => ToolbarIconSize.Small,
            "Medium" => ToolbarIconSize.Medium,
            "Large" => ToolbarIconSize.Large,
            _ => throw new InvalidDataException("The toolbar icon size is unsupported.")
        };
        AppFontSize fontSize = version == 1
            ? AppFontSize.Small
            : StrictJson.String(root.GetProperty("fontSize"), "fontSize") switch
            {
                "Small" => AppFontSize.Small,
                "Normal" => AppFontSize.Normal,
                "Large" => AppFontSize.Large,
                "Xxl" => AppFontSize.Xxl,
                _ => throw new InvalidDataException("The application font size is unsupported.")
            };
        string folder = StrictJson.String(root.GetProperty("defaultFolder"), "defaultFolder");
        int barsPerView = StrictJson.Integer(root.GetProperty("barsPerView"), "barsPerView", 1, 2);
        JsonElement entries = root.GetProperty("recentFiles");
        StrictJson.RequireArray(entries, "recentFiles");
        if (entries.GetArrayLength() > AppSettings.MaximumRecentFiles)
        {
            throw new InvalidDataException("The recent-file list exceeds five entries.");
        }

        List<string> recent = [];
        foreach (JsonElement entry in entries.EnumerateArray())
        {
            recent.Add(StrictJson.String(entry, "recentFiles"));
        }

        return new AppSettings
        {
            Theme = theme,
            IconSize = iconSize,
            FontSize = fontSize,
            DefaultFolder = folder,
            RecentFiles = recent,
            BarsPerView = barsPerView
        };
    }

    private static void WriteSettings(Utf8JsonWriter writer, AppSettings settings)
    {
        writer.WriteStartObject();
        writer.WriteNumber("version", CurrentVersion);
        writer.WriteString("theme", settings.Theme.ToString());
        writer.WriteString("iconSize", settings.IconSize.ToString());
        writer.WriteString("fontSize", settings.FontSize.ToString());
        writer.WriteString("defaultFolder", settings.DefaultFolder);
        writer.WriteStartArray("recentFiles");
        foreach (string path in settings.RecentFiles)
        {
            writer.WriteStringValue(path);
        }

        writer.WriteEndArray();
        writer.WriteNumber("barsPerView", settings.BarsPerView);
        writer.WriteEndObject();
    }
}
