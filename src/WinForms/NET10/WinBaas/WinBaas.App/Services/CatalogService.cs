using System.Text.Json;
using Microsoft.Extensions.Logging;
using WarpToolkit.ComponentModel;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="ICatalogService"/>
public sealed class CatalogService : ICatalogService
{
    /// <summary>Settings key under which the user-defined catalog is persisted.</summary>
    public const string SettingsKey = "WinBaas.Catalog";

    private readonly IUserSettingsService _settings;
    private readonly ILogger<CatalogService> _logger;
    private readonly List<CatalogEntry> _userEntries;
    private readonly List<CatalogEntry> _builtInEntries;

    public CatalogService(IUserSettingsService settings, ILogger<CatalogService> logger)
    {
        _settings = settings;
        _logger = logger;
        _builtInEntries = BuildSeed().ToList();

        string serialized = _settings.Get(SettingsKey, string.Empty);
        _userEntries = TryDeserialize(serialized);
        _logger.LogInformation("Loaded {Builtin} built-in and {User} user-defined catalog entries.",
            _builtInEntries.Count, _userEntries.Count);
    }

    /// <inheritdoc />
    public IReadOnlyList<CatalogEntry> GetAll()
        => _builtInEntries.Concat(_userEntries).ToList();

    /// <inheritdoc />
    public void Add(CatalogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (!entry.IsUserDefined)
        {
            throw new InvalidOperationException("Only user-defined entries can be added via Add.");
        }

        _userEntries.Add(entry);
        Save();
    }

    /// <inheritdoc />
    public bool Remove(Guid id)
    {
        int removed = _userEntries.RemoveAll(e => e.Id == id);
        if (removed > 0)
        {
            Save();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public void RestoreDefaults()
    {
        _userEntries.Clear();
        _settings.Remove(SettingsKey);
        _settings.Flush();
        _logger.LogInformation("Catalog restored to built-in defaults.");
    }

    /// <inheritdoc />
    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(_userEntries);
            _settings.Set(SettingsKey, json);
            _settings.Flush();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist catalog.");
        }
    }

    private List<CatalogEntry> TryDeserialize(string serialized)
    {
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<CatalogEntry>>(serialized) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not parse stored catalog; ignoring user-defined entries.");
            return [];
        }
    }

    private static IEnumerable<CatalogEntry> BuildSeed()
    {
        string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string downloads = Path.Combine(user, "Downloads");
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string source = Path.Combine(user, "source", "repos");

        yield return Folder(
            "Downloads (office docs)",
            "Word, Excel, PDF, PowerPoint and Markdown files in the Downloads folder.",
            downloads,
            [".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".md", ".rtf", ".txt"],
            recursive: false);

        yield return Folder(
            "Downloads (archives)",
            "ZIP/7z/RAR archives in the Downloads folder.",
            downloads,
            [".zip", ".7z", ".rar"],
            recursive: false);

        yield return Folder(
            "Desktop files",
            "Files (not shortcuts) sitting on the Desktop.",
            desktop,
            [".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".md", ".txt", ".rtf",
             ".png", ".jpg", ".jpeg", ".gif"],
            recursive: true);

        yield return Folder(
            "Screenshots",
            "PNG/JPEG screenshots under Pictures.",
            Path.Combine(pictures, "Screenshots"),
            [".png", ".jpg", ".jpeg"],
            recursive: true);

        yield return Folder(
            "Camera roll / photos",
            "Photos and videos under the Pictures folder.",
            pictures,
            [".jpg", ".jpeg", ".png", ".heic", ".heif", ".gif", ".mp4", ".mov"],
            recursive: true);

        yield return Folder(
            "Videos & recordings",
            "Videos and audio recordings under the Videos folder.",
            videos,
            [".mp4", ".mov", ".avi", ".webm", ".mkv", ".wav", ".m4a"],
            recursive: true);

        yield return Folder(
            "Camtasia raw recordings",
            "Camtasia project and raw capture files.",
            documents,
            [".camproj", ".camrec", ".trec"],
            recursive: true);

        yield return Folder(
            "Voice / screen recordings",
            "Windows voice recorder or screen recordings.",
            Path.Combine(documents, "Sound Recordings"),
            [".m4a", ".wav", ".mp4"],
            recursive: true);

        yield return Folder(
            "Edge favorites",
            "Edge favorites exported as .url files.",
            Path.Combine(appData, "Microsoft", "Edge", "User Data", "Default", "Favorites"),
            [".url"],
            recursive: true);

        yield return Folder(
            "Oh My Posh themes",
            "Oh My Posh theme definitions and Lua helpers.",
            Path.Combine(localAppData, "Programs", "oh-my-posh", "themes"),
            [".omp.json", ".json", ".lua"],
            recursive: true);

        yield return Folder(
            "Roaming AppData (settings)",
            "Custom app configuration files in %AppData%.",
            appData,
            [".json", ".xml", ".config", ".ini", ".toml", ".yaml", ".yml"],
            recursive: true);

        yield return Folder(
            "Visual Studio projects (non-git)",
            "Visual Studio solutions and projects on disk; the discovery service filters out projects already inside a git repo.",
            source,
            [".sln", ".slnx", ".csproj", ".vbproj"],
            recursive: true);

        yield return new CatalogEntry
        {
            Kind = CatalogEntryKind.EnvironmentVariable,
            Name = "User environment variables (non-standard)",
            Description = "User-scoped environment variables that do not look like a PATH definition; useful for Copilot keys and other secrets.",
            Path = "User",
            IsUserDefined = false,
        };

        yield return new CatalogEntry
        {
            Kind = CatalogEntryKind.SqlServer,
            Name = "SQL Server (LocalDB)",
            Description = "All discoverable LocalDB instances and their attached databases.",
            Path = "LocalDB",
            IsUserDefined = false,
        };

        yield return new CatalogEntry
        {
            Kind = CatalogEntryKind.SqlServer,
            Name = "SQL Server (Express)",
            Description = "Local SQL Express instances and their attached databases.",
            Path = "SQLEXPRESS",
            IsUserDefined = false,
        };
    }

    private static CatalogEntry Folder(
        string name,
        string description,
        string path,
        string[] extensions,
        bool recursive)
        => new()
        {
            Kind = CatalogEntryKind.Folder,
            Name = name,
            Description = description,
            Path = path,
            Extensions = extensions,
            IncludeSubfolders = recursive,
            IsUserDefined = false,
        };
}
