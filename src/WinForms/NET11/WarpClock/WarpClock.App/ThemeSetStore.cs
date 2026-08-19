using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using WarpClock.Abstractions;

namespace WarpClock.App;

/// <summary>
///  Persists editable theme-set documents at arbitrary JSON paths.
/// </summary>
public sealed class ThemeSetStore
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
    private readonly ILogger<ThemeSetStore> _logger;

    public ThemeSetStore(AppPaths paths, ILogger<ThemeSetStore> logger)
    {
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(logger);

        _paths = paths;
        _logger = logger;
    }

    public ThemeScheduleDocument EnsureDefaultAtPath(string path, IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(catalog);

        if (!File.Exists(path))
        {
            ThemeScheduleDocument defaults = ThemeSetDefaults.CreateDefault(catalog);
            ApplyCatalogMetadata(defaults, catalog, path);
            SaveToPath(path, defaults);
            return defaults;
        }

        try
        {
            return LoadFromPath(path, catalog);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            BackupUnreadableFile(path);
            _logger.LogWarning(ex, "Could not restore the WarpClock theme set from {Path}; rebuilding defaults.", path);

            ThemeScheduleDocument defaults = ThemeSetDefaults.CreateDefault(catalog);
            ApplyCatalogMetadata(defaults, catalog, path);
            SaveToPath(path, defaults);
            return defaults;
        }
    }

    public ThemeScheduleDocument MigrateLegacyDefaultFile(string legacyPath, string path, IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(legacyPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(catalog);

        ThemeScheduleDocument document = LoadFromPath(legacyPath, catalog);
        SaveToPath(path, document);

        try
        {
            File.Delete(legacyPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(ex, "Could not remove the legacy WarpClock theme-list file {Path}.", legacyPath);
        }

        return LoadFromPath(path, catalog);
    }

    public ThemeScheduleDocument LoadFromPath(string path, IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(catalog);

        ThemeScheduleDocument document = ReadJsonFile<ThemeScheduleDocument>(path);
        ApplyCatalogMetadata(document, catalog, path);
        return document;
    }

    public void SaveToPath(string path, ThemeScheduleDocument document)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(document);

        document.Normalize();
        if (string.IsNullOrWhiteSpace(document.Name))
        {
            document.Name = GetDocumentNameFromPath(path);
        }

        WriteJsonFile(path, document);
    }

    private static void ApplyCatalogMetadata(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> catalog,
        string path)
    {
        document.Normalize();

        if (string.IsNullOrWhiteSpace(document.Name))
        {
            document.Name = GetDocumentNameFromPath(path);
        }

        Dictionary<string, ThemeCatalogInfo> byKey = catalog
            .GroupBy(item => ThemeCatalogInfo.NormalizeThemeKey(item.ThemeKey), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);

        foreach (ThemeScheduleEntry entry in document.Entries)
        {
            ThemeReferenceUtility.Normalize(entry.Theme);

            if (!string.IsNullOrWhiteSpace(entry.Theme.ThemeKey)
                && byKey.TryGetValue(entry.Theme.ThemeKey, out ThemeCatalogInfo? info))
            {
                if (entry.Theme.Variant is ClockThemeVariantKind explicitVariant
                    && !info.SupportsVariant(explicitVariant))
                {
                    entry.Theme.Variant = null;
                }

                entry.DisplayName = info.FamilyName;
                entry.Source = info.Source;
                entry.Theme.ThemeKey = info.ThemeKey;
                entry.EligibleDuringDay &= info.SupportsPeriod(ThemeSchedulePeriod.Day, entry.Theme.Variant);
                entry.EligibleDuringNight &= info.SupportsPeriod(ThemeSchedulePeriod.Night, entry.Theme.Variant);
            }
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

    private static string GetDocumentNameFromPath(string path)
    {
        string fileName = Path.GetFileName(path);
        return fileName.EndsWith(".themeset.json", StringComparison.OrdinalIgnoreCase)
            ? fileName[..^".themeset.json".Length]
            : Path.GetFileNameWithoutExtension(path);
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
            _logger.LogWarning(ex, "Could not preserve unreadable theme-set file {Path}.", path);
        }
    }
}
