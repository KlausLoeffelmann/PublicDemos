using System.Text.Json;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Percussion;

namespace DrumMachine.Demo;

/// <summary>
///  Loads explicit startup defaults on invalid preferences and atomically persists successfully validated choices.
/// </summary>
internal static class AppSettingsStore
{
    private const int CurrentVersion = 4;

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
        else if (version == 2)
        {
            StrictJson.RequireProperties(root,
                "version", "theme", "iconSize", "fontSize", "defaultFolder", "recentFiles", "barsPerView");
        }
        else if (version == 3)
        {
            StrictJson.RequireProperties(root,
                "version", "theme", "iconSize", "fontSize", "defaultFolder", "recentFiles", "barsPerView", "trackPresets");
        }
        else
        {
            StrictJson.RequireProperties(root,
                "version", "theme", "iconSize", "fontSize", "defaultFolder", "recordingsFolder", "soundPresetFolder",
                "lastWavImportFolder", "recentFiles", "barsPerView", "trackPresets", "windowPlacement",
                "spectrumSplitterDip", "stepGridColumns");
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
            BarsPerView = barsPerView,
            TrackPresets = version < 3 ? TrackSelectionPreset.CreateEmpty() : ReadPresets(root),
            RecordingsFolder = version < 4
                ? AppSettings.SpecialFolderOrDocuments(Environment.SpecialFolder.MyMusic)
                : StrictJson.String(root.GetProperty("recordingsFolder"), "recordingsFolder"),
            SoundPresetFolder = version < 4
                ? AppSettings.SpecialFolderOrDocuments(Environment.SpecialFolder.MyDocuments)
                : StrictJson.String(root.GetProperty("soundPresetFolder"), "soundPresetFolder"),
            LastWavImportFolder = version < 4
                ? AppSettings.SpecialFolderOrDocuments(Environment.SpecialFolder.MyMusic)
                : StrictJson.String(root.GetProperty("lastWavImportFolder"), "lastWavImportFolder"),
            Window = version < 4 ? WindowPlacement.Unset : ReadWindow(root.GetProperty("windowPlacement")),
            SpectrumSplitterDistanceDip = version < 4
                ? 0
                : StrictJson.Integer(root.GetProperty("spectrumSplitterDip"), "spectrumSplitterDip", 0, 4_000),
            StepGridColumns = version < 4 ? [] : ReadColumns(root.GetProperty("stepGridColumns"))
        };
    }

    private static WindowPlacement ReadWindow(JsonElement window)
    {
        StrictJson.RequireProperties(window, "x", "y", "width", "height", "maximized", "dpi");
        return new WindowPlacement
        {
            X = StrictJson.Integer(window.GetProperty("x"), "x", -32_000, 32_000),
            Y = StrictJson.Integer(window.GetProperty("y"), "y", -32_000, 32_000),
            Width = StrictJson.Integer(window.GetProperty("width"), "width", 0, 10_000),
            Height = StrictJson.Integer(window.GetProperty("height"), "height", 0, 10_000),
            IsMaximized = StrictJson.Boolean(window.GetProperty("maximized"), "maximized"),
            SavedDpi = StrictJson.Integer(window.GetProperty("dpi"), "dpi", 0, 480)
        };
    }

    private static IReadOnlyList<StepGridColumnPreference> ReadColumns(JsonElement entries)
    {
        StrictJson.RequireArray(entries, "stepGridColumns");
        if (entries.GetArrayLength() == 0)
        {
            return [];
        }

        if (entries.GetArrayLength() != StepGridColumnPreference.Names.Count)
        {
            throw new InvalidDataException("Saved score columns must include every grid column.");
        }

        List<StepGridColumnPreference> columns = [];
        foreach (JsonElement entry in entries.EnumerateArray())
        {
            StrictJson.RequireProperties(entry, "name", "displayIndex", "widthDip");
            columns.Add(new StepGridColumnPreference
            {
                Name = StrictJson.String(entry.GetProperty("name"), "name"),
                DisplayIndex = StrictJson.Integer(entry.GetProperty("displayIndex"), "displayIndex", 0, StepGridColumnPreference.Names.Count - 1),
                WidthDip = StrictJson.Integer(entry.GetProperty("widthDip"), "widthDip", 16, 2_000)
            });
        }

        return columns;
    }

    private static IReadOnlyList<TrackSelectionPreset> ReadPresets(JsonElement root)
    {
        JsonElement entries = root.GetProperty("trackPresets");
        StrictJson.RequireArray(entries, "trackPresets");
        if (entries.GetArrayLength() != TrackSelectionPreset.SlotCount)
        {
            throw new InvalidDataException("Track presets must contain exactly eight slots.");
        }

        List<TrackSelectionPreset> presets = [];
        foreach (JsonElement entry in entries.EnumerateArray())
        {
            StrictJson.RequireProperties(entry, "name", "instruments");
            string name = StrictJson.String(entry.GetProperty("name"), "name");
            JsonElement instruments = entry.GetProperty("instruments");
            StrictJson.RequireArray(instruments, "instruments");
            List<Cr78Instrument> parsed = [];
            foreach (JsonElement instrument in instruments.EnumerateArray())
            {
                string token = MigrateLegacyInstrument(StrictJson.String(instrument, "instruments"));
                if (!Enum.TryParse(token, out Cr78Instrument value) || value.ToString() != token ||
                    !Cr78Kit.Instruments.Contains(value))
                {
                    throw new InvalidDataException($"Unknown percussion instrument '{token}'.");
                }

                if (!parsed.Contains(value))
                {
                    parsed.Add(value);
                }
                else if (token == instrument.GetString())
                {
                    throw new InvalidDataException($"Duplicate percussion instrument '{token}'.");
                }
            }

            presets.Add(new TrackSelectionPreset { Name = name, Instruments = parsed });
        }

        return presets;
    }

    /// <summary>
    ///  Maps retired score tokens onto the fixed seventeen-track kit. Current tokens are unchanged.
    /// </summary>
    private static string MigrateLegacyInstrument(string token)
        => token switch
        {
            nameof(Cr78Instrument.Maracas) when !Cr78Kit.Instruments.Contains(Cr78Instrument.Maracas)
                => nameof(Cr78Instrument.Tambourine2),
            nameof(Cr78Instrument.Claves) when !Cr78Kit.Instruments.Contains(Cr78Instrument.Claves)
                => nameof(Cr78Instrument.Guiro2),
            _ => token
        };

    private static void WriteSettings(Utf8JsonWriter writer, AppSettings settings)
    {
        writer.WriteStartObject();
        writer.WriteNumber("version", CurrentVersion);
        writer.WriteString("theme", settings.Theme.ToString());
        writer.WriteString("iconSize", settings.IconSize.ToString());
        writer.WriteString("fontSize", settings.FontSize.ToString());
        writer.WriteString("defaultFolder", settings.DefaultFolder);
        writer.WriteString("recordingsFolder", settings.RecordingsFolder);
        writer.WriteString("soundPresetFolder", settings.SoundPresetFolder);
        writer.WriteString("lastWavImportFolder", settings.LastWavImportFolder);
        writer.WriteStartArray("recentFiles");
        foreach (string path in settings.RecentFiles)
        {
            writer.WriteStringValue(path);
        }

        writer.WriteEndArray();
        writer.WriteNumber("barsPerView", settings.BarsPerView);
        writer.WriteStartArray("trackPresets");
        foreach (TrackSelectionPreset preset in settings.TrackPresets)
        {
            writer.WriteStartObject();
            writer.WriteString("name", preset.Name);
            writer.WriteStartArray("instruments");
            foreach (Cr78Instrument instrument in preset.Instruments)
            {
                writer.WriteStringValue(instrument.ToString());
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WritePropertyName("windowPlacement");
        writer.WriteStartObject();
        writer.WriteNumber("x", settings.Window.X);
        writer.WriteNumber("y", settings.Window.Y);
        writer.WriteNumber("width", settings.Window.Width);
        writer.WriteNumber("height", settings.Window.Height);
        writer.WriteBoolean("maximized", settings.Window.IsMaximized);
        writer.WriteNumber("dpi", settings.Window.SavedDpi);
        writer.WriteEndObject();
        writer.WriteNumber("spectrumSplitterDip", settings.SpectrumSplitterDistanceDip);
        writer.WriteStartArray("stepGridColumns");
        foreach (StepGridColumnPreference column in settings.StepGridColumns)
        {
            writer.WriteStartObject();
            writer.WriteString("name", column.Name);
            writer.WriteNumber("displayIndex", column.DisplayIndex);
            writer.WriteNumber("widthDip", column.WidthDip);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
