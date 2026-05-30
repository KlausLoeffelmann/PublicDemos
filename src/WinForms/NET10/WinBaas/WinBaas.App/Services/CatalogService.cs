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
        yield return new CatalogEntry
        {
            Category = string.Empty,
            Kind = CatalogEntryKind.Registry,
            Name = "Registry",
            Description = "Curated Windows registry values that are frequently changed by hand.",
            ShortTag = "Registry",
            IsUserDefined = false,
        };

        yield return new CatalogEntry
        {
            Category = string.Empty,
            Kind = CatalogEntryKind.VisualStudio,
            Name = "Visual Studio",
            Description = "Installed Visual Studio SKUs together with their hives and extensions.",
            ShortTag = "Visual Studio",
            IsUserDefined = false,
        };

        foreach (CatalogEntry entry in BuildSystemSeed())
        {
            yield return entry;
        }

        foreach (CatalogEntry entry in BuildAiToolsSeed())
        {
            yield return entry;
        }

        foreach (CatalogEntry entry in BuildDeveloperSeed())
        {
            yield return entry;
        }

        foreach (CatalogEntry entry in BuildCreatorSeed())
        {
            yield return entry;
        }

        foreach (CatalogEntry entry in BuildMusicianSeed())
        {
            yield return entry;
        }
    }

    private const string CategorySystem = "System";
    private const string CategoryAiTools = "AI Tools";
    private const string CategoryDeveloper = "Developer Tools";
    private const string CategoryCreator = "Creator / Design / Photo";
    private const string CategoryMusician = "Musician / Audio";
    private const string CategoryUser = "User";

    private static readonly string[] s_officeDocExts =
        [".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".md", ".txt", ".rtf"];

    private static IEnumerable<CatalogEntry> BuildSystemSeed()
    {
        string downloads = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        yield return Folder(CategorySystem,
            "Downloads (office docs)",
            "Word, Excel, PDF, PowerPoint and Markdown files in the Downloads folder.",
            [downloads],
            s_officeDocExts,
            recursive: false);

        yield return Folder(CategorySystem,
            "Downloads (archives)",
            "ZIP/7z/RAR archives in the Downloads folder.",
            [downloads],
            [".zip", ".7z", ".rar"],
            recursive: false);

        yield return Folder(CategorySystem,
            "Desktop files",
            "Files (not shortcuts) sitting on the Desktop.",
            [desktop],
            [.. s_officeDocExts, ".png", ".jpg", ".jpeg", ".gif"],
            recursive: true);

        yield return Folder(CategorySystem,
            "Screenshots",
            "PNG/JPEG screenshots under Pictures.",
            [Path.Combine(pictures, "Screenshots")],
            [".png", ".jpg", ".jpeg"],
            recursive: true);

        yield return Folder(CategorySystem,
            "Camera roll / photos",
            "Photos and videos under the Pictures folder.",
            [pictures],
            [".jpg", ".jpeg", ".png", ".heic", ".heif", ".gif", ".mp4", ".mov"],
            recursive: true);

        yield return Folder(CategorySystem,
            "Videos & recordings",
            "Videos and audio recordings under the Videos folder.",
            [videos],
            [".mp4", ".mov", ".avi", ".webm", ".mkv", ".wav", ".m4a"],
            recursive: true);

        yield return Folder(CategorySystem,
            "Edge favorites",
            "Edge favorites exported as .url files.",
            [Path.Combine(appData, "Microsoft", "Edge", "User Data", "Default", "Favorites")],
            [".url"],
            recursive: true);

        yield return new CatalogEntry
        {
            Category = CategorySystem,
            Kind = CatalogEntryKind.EnvironmentVariable,
            Name = "User environment variables (non-standard)",
            Description = "User-scoped environment variables that do not look like PATH definitions; useful for Copilot keys and other secrets.",
            Paths = ["User"],
            IsUserDefined = false,
        };

        yield return new CatalogEntry
        {
            Category = CategorySystem,
            Kind = CatalogEntryKind.SqlServer,
            Name = "SQL Server (LocalDB)",
            Description = "All discoverable LocalDB instances and their attached databases.",
            Paths = ["LocalDB"],
            IsUserDefined = false,
        };

        yield return new CatalogEntry
        {
            Category = CategorySystem,
            Kind = CatalogEntryKind.SqlServer,
            Name = "SQL Server (Express)",
            Description = "Local SQL Express instances and their attached databases.",
            Paths = ["SQLEXPRESS"],
            IsUserDefined = false,
        };
    }

    private static IEnumerable<CatalogEntry> BuildAiToolsSeed()
    {
        yield return Folder(CategoryAiTools,
            "Cursor",
            "Cursor user settings, keybindings, snippets and installed extensions.",
            [
                @"%APPDATA%\Cursor\User",
                @"%USERPROFILE%\.cursor",
                @"%APPDATA%\Cursor\User\globalStorage",
            ],
            [".json", ".jsonc", ".code-snippets", ".md"],
            recursive: true,
            shortTag: "Cursor");

        yield return Folder(CategoryAiTools,
            "Claude Code (Anthropic CLI)",
            "Claude Code config, credentials, agents, slash commands and project instructions (CLAUDE.md) under %USERPROFILE%\\.claude.",
            [@"%USERPROFILE%\.claude"],
            [".json", ".jsonc", ".md", ".yaml", ".yml", ".toml"],
            recursive: true,
            shortTag: "Claude Code");

        yield return Folder(CategoryAiTools,
            "OpenAI Codex CLI",
            "Codex CLI config (config.toml), MCP server list and history under %USERPROFILE%\\.codex.",
            [@"%USERPROFILE%\.codex"],
            [".toml", ".json", ".jsonc", ".md", ".log"],
            recursive: true,
            shortTag: "Codex");

        yield return Folder(CategoryAiTools,
            "GitHub Copilot CLI",
            "Copilot CLI user-level config, session state, custom agents, skills and MCP server list under %USERPROFILE%\\.copilot.",
            [@"%USERPROFILE%\.copilot"],
            [".json", ".jsonc", ".md", ".yaml", ".yml", ".toml"],
            recursive: true,
            shortTag: "Copilot CLI");

        yield return Folder(CategoryAiTools,
            "Visual Studio Copilot",
            "Visual Studio Copilot extension state and signed-in auth: %APPDATA%\\GitHub Copilot and %LOCALAPPDATA%\\GitHub Copilot.",
            [
                @"%APPDATA%\GitHub Copilot",
                @"%LOCALAPPDATA%\GitHub Copilot",
            ],
            [".json", ".jsonc", ".md", ".log"],
            recursive: true,
            shortTag: "VS Copilot");

        yield return Folder(CategoryAiTools,
            "VS Code Copilot",
            "VS Code Copilot extension global storage (chat history, prompts, agent config).",
            [
                @"%APPDATA%\Code\User\globalStorage\github.copilot",
                @"%APPDATA%\Code\User\globalStorage\github.copilot-chat",
                @"%APPDATA%\Code\User\prompts",
            ],
            [".json", ".jsonc", ".md", ".log", ".db"],
            recursive: true,
            shortTag: "VS Code Copilot");
    }

    private static IEnumerable<CatalogEntry> BuildDeveloperSeed()
    {
        yield return Folder(CategoryDeveloper,
            "Visual Studio – ASP.NET / web",
            "IIS Express config (Documents\\IISExpress\\config) and web project artifacts. The user cert store has to be re-exported via certmgr.msc.",
            [@"%USERPROFILE%\Documents\IISExpress\config"],
            [
                ".csproj", ".vbproj", ".cshtml", ".razor", ".aspx", ".ascx", ".master",
                ".asmx", ".svc", ".css", ".scss", ".js", ".ts", ".json", ".config",
                ".pubxml", ".publishsettings",
            ],
            recursive: true,
            shortTag: "VS Web");

        yield return Folder(CategoryDeveloper,
            "VS Code",
            "User settings/keybindings/snippets and installed extensions.",
            [@"%APPDATA%\Code\User", @"%USERPROFILE%\.vscode\extensions"],
            [".code-workspace", ".code-profile", ".json"],
            recursive: true,
            shortTag: "VS Code");

        yield return Folder(CategoryDeveloper,
            "VB6 / VBA legacy",
            "VB6 has no central data folder beyond the IDE settings in HKCU. Vbaddin.ini in %WINDIR% lists registered add-ins.",
            [@"%WINDIR%\Vbaddin.ini"],
            [
                ".vbp", ".vbg", ".frm", ".frx", ".bas", ".cls",
                ".ctl", ".ctx", ".dsr", ".dsx", ".res", ".ocx", ".dll",
            ],
            recursive: false,
            shortTag: "VB6");

        yield return Folder(CategoryDeveloper,
            "JetBrains Rider",
            "Rider settings/config (Settings Repository export is cleaner than copying raw).",
            [@"%APPDATA%\JetBrains\Rider<wildcard>", @"%LOCALAPPDATA%\JetBrains\Rider<wildcard>"],
            [".sln", ".slnx", ".csproj", ".cs", ".vb", ".editorconfig", ".DotSettings", ".json"],
            recursive: true,
            shortTag: "Rider");

        yield return Folder(CategoryDeveloper,
            "JetBrains IntelliJ / others",
            "JetBrains config & caches/plugins. Project-local config sits in .idea\\ alongside the project.",
            [@"%APPDATA%\JetBrains\<wildcard>", @"%LOCALAPPDATA%\JetBrains\<wildcard>"],
            [".iml", ".java", ".kt", ".gradle", ".xml", ".properties"],
            recursive: true,
            shortTag: "JetBrains");

        yield return Folder(CategoryDeveloper,
            "Eclipse workspace",
            "Workspace dir holds preferences and project metadata under .metadata. Back up the whole workspace.",
            [@"%USERPROFILE%\workspace", @"%USERPROFILE%\eclipse-workspace"],
            [".project", ".classpath", ".java", ".target", ".launch", ".epf"],
            recursive: true,
            shortTag: "Eclipse");

        yield return Folder(CategoryDeveloper,
            "Android Studio",
            "Config, SDK, AVDs, keystores and the Gradle cache. Do not lose debug.keystore or release .jks.",
            [
                @"%APPDATA%\Google\AndroidStudio<wildcard>",
                @"%LOCALAPPDATA%\Android\Sdk",
                @"%USERPROFILE%\.android",
                @"%USERPROFILE%\.gradle",
            ],
            [".gradle", ".kts", ".kt", ".java", ".xml", ".jks", ".keystore", ".properties"],
            recursive: true,
            shortTag: "Android Studio");

        yield return Folder(CategoryDeveloper,
            "Git (global)",
            "Global git config, ignore, and SSH keys. Credential Manager entries are not in a file.",
            [
                @"%USERPROFILE%\.gitconfig",
                @"%USERPROFILE%\.gitignore_global",
                @"%USERPROFILE%\.ssh",
            ],
            [".gitconfig", ".gitignore", ".gitattributes", ".pub"],
            recursive: false,
            shortTag: "Git");

        yield return Folder(CategoryDeveloper,
            "WSL distros",
            "WSL state under %LOCALAPPDATA%\\Packages. The clean way is `wsl --export <distro> <file>.tar`.",
            [@"%LOCALAPPDATA%\Packages", @"%USERPROFILE%\.wslconfig"],
            [".tar", ".wslconfig"],
            recursive: false,
            shortTag: "WSL");

        yield return Folder(CategoryDeveloper,
            "Node / npm",
            "Registry config + tokens and globally installed packages.",
            [@"%USERPROFILE%\.npmrc", @"%APPDATA%\npm"],
            [".npmrc", ".json", ".nvmrc"],
            recursive: false,
            shortTag: "npm");

        yield return Folder(CategoryDeveloper,
            "Docker Desktop",
            "Docker Desktop settings. WSL2 backend VHDX is huge — usually rebuild rather than copy.",
            [@"%APPDATA%\Docker"],
            [".json", ".yaml", ".yml"],
            recursive: true,
            shortTag: "Docker");

        yield return Folder(CategoryDeveloper,
            "SQL Server Management Studio (SSMS)",
            "Templates / projects under Documents and registered-server / connection history under %APPDATA%.",
            [
                @"%USERPROFILE%\Documents\SQL Server Management Studio",
                @"%APPDATA%\Microsoft\SQL Server Management Studio",
            ],
            [".sql", ".ssmssln", ".ssmssqlproj", ".bak", ".regsrvr"],
            recursive: true,
            shortTag: "SSMS");
    }

    private static IEnumerable<CatalogEntry> BuildCreatorSeed()
    {
        yield return Folder(CategoryCreator,
            "Adobe – shared / Creative Cloud",
            "Cross-app presets, libraries cache, sync settings. CC Libraries are cloud-synced; local presets are not.",
            [@"%APPDATA%\Adobe", @"%LOCALAPPDATA%\Adobe"],
            [".xml", ".json"],
            recursive: true,
            shortTag: "Adobe CC");

        yield return Folder(CategoryCreator,
            "Adobe Photoshop",
            "Presets (brushes, gradients, styles, patterns, shapes), workspace and prefs.",
            [@"%APPDATA%\Adobe\Adobe Photoshop <wildcard>"],
            [
                ".psd", ".psb", ".pdd",
                ".abr", ".atn", ".asl", ".grd", ".pat", ".csh",
                ".aco", ".act", ".tpl", ".psp",
            ],
            recursive: true,
            shortTag: "Photoshop");

        yield return Folder(CategoryCreator,
            "Photoshop Actions",
            "Custom action sets. Export each set explicitly to .atn — the Actions Palette cache is not a substitute.",
            [@"%APPDATA%\Adobe\Adobe Photoshop <wildcard>"],
            [".atn"],
            recursive: true,
            shortTag: "Photoshop Actions");

        yield return Folder(CategoryCreator,
            "Adobe Lightroom Classic",
            "Catalog (.lrcat), develop presets (.xmp) and camera profiles.",
            [
                @"%USERPROFILE%\Pictures\Lightroom",
                @"%APPDATA%\Adobe\CameraRaw\Settings",
                @"%APPDATA%\Adobe\CameraRaw\Camera Profiles",
            ],
            [".lrcat", ".lrdata", ".lrtemplate", ".xmp", ".dng"],
            recursive: true,
            shortTag: "Lightroom");

        yield return Folder(CategoryCreator,
            "Adobe Camera Raw",
            "Shared with Lightroom: develop settings, camera and lens profiles.",
            [@"%APPDATA%\Adobe\CameraRaw"],
            [".xmp", ".dcp", ".lcp"],
            recursive: true,
            shortTag: "Camera Raw");

        yield return Folder(CategoryCreator,
            "Adobe Illustrator",
            "Presets, workspaces, swatches (custom workspaces are the easy thing to lose).",
            [@"%APPDATA%\Adobe\Adobe Illustrator <wildcard>"],
            [".ai", ".ait", ".eps", ".svg", ".ase", ".aia", ".grd"],
            recursive: true,
            shortTag: "Illustrator");

        yield return Folder(CategoryCreator,
            "Adobe InDesign",
            "Presets, workspaces, autocorrect, defaults, glyph sets, scripts.",
            [@"%APPDATA%\Adobe\InDesign\Version <wildcard>"],
            [".indd", ".indt", ".indb", ".idml", ".indl", ".jsx"],
            recursive: true,
            shortTag: "InDesign");

        yield return Folder(CategoryCreator,
            "Adobe Premiere Pro",
            "Workspaces, presets, autosave under Documents\\Adobe\\Premiere Pro.",
            [@"%USERPROFILE%\Documents\Adobe\Premiere Pro\<wildcard>"],
            [".prproj", ".prtl", ".epr", ".prfpset", ".aaf", ".xml"],
            recursive: true,
            shortTag: "Premiere Pro");

        yield return Folder(CategoryCreator,
            "Adobe After Effects",
            "Presets and workspaces under Documents\\Adobe\\After Effects plus app-data.",
            [
                @"%USERPROFILE%\Documents\Adobe\After Effects <wildcard>",
                @"%APPDATA%\Adobe\After Effects <wildcard>",
            ],
            [".aep", ".aepx", ".aet", ".ffx"],
            recursive: true,
            shortTag: "After Effects");

        yield return Folder(CategoryCreator,
            "Adobe Acrobat",
            "Custom stamps, security policies, preferences.",
            [@"%APPDATA%\Adobe\Acrobat\<wildcard>"],
            [".pdf", ".fdf", ".xfdf", ".acrobatsecuritysettings"],
            recursive: true,
            shortTag: "Acrobat");

        yield return Folder(CategoryCreator,
            "Affinity (Photo / Designer / Publisher) v2",
            "Per-app settings and assets. Use the built-in Export Settings for a clean bundle.",
            [@"%APPDATA%\Affinity", @"%LOCALAPPDATA%\Affinity"],
            [
                ".afphoto", ".afdesign", ".afpub",
                ".afassets", ".afbrushes", ".afstyles", ".afmacros", ".afpalette", ".aftemplate",
            ],
            recursive: true,
            shortTag: "Affinity");

        yield return Folder(CategoryCreator,
            "GIMP",
            "One folder: brushes, scripts, plug-ins, prefs.",
            [@"%APPDATA%\GIMP\<wildcard>"],
            [".xcf", ".gbr", ".vbr", ".gih", ".pat", ".gpl", ".scm"],
            recursive: true,
            shortTag: "GIMP");

        yield return Folder(CategoryCreator,
            "Inkscape",
            "Preferences, extensions, templates and palettes.",
            [@"%APPDATA%\inkscape"],
            [".svg", ".svgz"],
            recursive: true,
            shortTag: "Inkscape");

        yield return Folder(CategoryCreator,
            "Blender",
            "Config, startup file, addons (startup.blend and userpref.blend are the must-haves).",
            [@"%APPDATA%\Blender Foundation\Blender\<wildcard>"],
            [".blend", ".blend1", ".blendswap", ".py"],
            recursive: true,
            shortTag: "Blender");

        yield return Folder(CategoryCreator,
            "DaVinci Resolve",
            "Project DB (if local) plus support config. Easiest: Project Manager → .drp export.",
            [@"%APPDATA%\Blackmagic Design\DaVinci Resolve\Support"],
            [".drp", ".drt", ".drx", ".drb"],
            recursive: true,
            shortTag: "Resolve");

        yield return Folder(CategoryCreator,
            "Capture One",
            "Styles & presets (catalog/session folders hold the data itself).",
            [@"%LOCALAPPDATA%\CaptureOne", @"%APPDATA%\Capture One"],
            [".cocatalog", ".cosessiondb", ".costyle", ".copreset", ".coproof"],
            recursive: true,
            shortTag: "Capture One");

        yield return Folder(CategoryCreator,
            "Figma (desktop)",
            "Files are cloud-side. Local cache is mostly disposable.",
            [@"%LOCALAPPDATA%\Figma"],
            [".fig", ".figma"],
            recursive: true,
            shortTag: "Figma");

        yield return Folder(CategoryCreator,
            "OBS Studio",
            "Scenes, profiles, settings — everything under %APPDATA%\\obs-studio.",
            [@"%APPDATA%\obs-studio"],
            [".json", ".ini"],
            recursive: true,
            shortTag: "OBS");

        yield return Folder(CategoryCreator,
            "ScreenToGif",
            "Settings (Settings.xaml) and shared recordings. Portable builds keep it next to the .exe.",
            [@"%LOCALAPPDATA%\ScreenToGif"],
            [".gif", ".apng", ".webp", ".mp4", ".stg", ".psd", ".xaml"],
            recursive: true,
            shortTag: "ScreenToGif");
    }

    private static IEnumerable<CatalogEntry> BuildMusicianSeed()
    {
        yield return Folder(CategoryMusician,
            "Steinberg Cubase",
            "Preferences, key commands, templates, track/channel presets (Defaults.xml + Key Commands.xml + Presets folders).",
            [@"%APPDATA%\Steinberg\Cubase <wildcard>"],
            [".cpr", ".bak", ".npr", ".steinbergproject", ".track", ".vstpreset", ".xml"],
            recursive: true,
            shortTag: "Cubase");

        yield return Folder(CategoryMusician,
            "Steinberg Nuendo",
            "Same layout as Cubase: preferences, key commands, templates, track/channel presets.",
            [@"%APPDATA%\Steinberg\Nuendo <wildcard>"],
            [".npr", ".vstpreset", ".track", ".xml"],
            recursive: true,
            shortTag: "Nuendo");

        yield return Folder(CategoryMusician,
            "Ableton Live",
            "User Library (default home for presets, racks, samples) and preferences.",
            [
                @"%USERPROFILE%\Documents\Ableton\User Library",
                @"%APPDATA%\Ableton\Live <wildcard>\Preferences",
            ],
            [".als", ".alc", ".adv", ".adg", ".alp", ".amxd", ".asd"],
            recursive: true,
            shortTag: "Ableton");

        yield return Folder(CategoryMusician,
            "FL Studio",
            "User projects and presets, plus app data + registration.",
            [@"%USERPROFILE%\Documents\Image-Line", @"%APPDATA%\Image-Line"],
            [".flp", ".fst", ".fsc", ".zip", ".flm"],
            recursive: true,
            shortTag: "FL Studio");

        yield return Folder(CategoryMusician,
            "PreSonus Studio One",
            "Presets/songs/extensions under Documents\\Studio One plus app settings.",
            [
                @"%USERPROFILE%\Documents\Studio One",
                @"%APPDATA%\PreSonus\Studio One <wildcard>",
            ],
            [".song", ".project", ".instrument", ".fxchain", ".multipreset", ".preset"],
            recursive: true,
            shortTag: "Studio One");

        yield return Folder(CategoryMusician,
            "Reaper",
            "reaper.ini, KeyMaps, ColorThemes, FX chains, track templates — all in %APPDATA%\\REAPER.",
            [@"%APPDATA%\REAPER"],
            [".rpp", ".rpp-bak", ".RfxChain", ".RTrackTemplate", ".ReaperTheme", ".ReaperThemeZip"],
            recursive: true,
            shortTag: "Reaper");

        yield return Folder(CategoryMusician,
            "Bitwig Studio",
            "User library + local settings.",
            [
                @"%USERPROFILE%\Documents\Bitwig Studio",
                @"%LOCALAPPDATA%\Bitwig Studio",
            ],
            [".bwproject", ".bwpreset", ".bwclip", ".bwpackage", ".bwdevice"],
            recursive: true,
            shortTag: "Bitwig");

        yield return Folder(CategoryMusician,
            "Pro Tools",
            "Avid preferences, I/O setups, key commands (the easy losses).",
            [@"%APPDATA%\Avid\Pro Tools", @"%LOCALAPPDATA%\Avid"],
            [".ptx", ".ptf", ".pts", ".ptt", ".aaf", ".omf"],
            recursive: true,
            shortTag: "Pro Tools");

        yield return Folder(CategoryMusician,
            "Native Instruments",
            "User presets and settings (content locations are DB-tracked by Native Access).",
            [
                @"%USERPROFILE%\Documents\Native Instruments",
                @"%APPDATA%\Native Instruments",
            ],
            [".nki", ".nkm", ".nkx", ".nkr", ".nksn", ".nksf", ".ncw"],
            recursive: true,
            shortTag: "NI");

        yield return Folder(CategoryMusician,
            "Spectrasonics STEAM",
            "STEAM folder holds the whole library. Location is registry-tracked under HKCU\\Software\\Spectrasonics.",
            [@"%USERPROFILE%\Documents\STEAM"],
            [".prt_omn", ".mlt_omn", ".db"],
            recursive: true,
            shortTag: "Spectrasonics");

        yield return Folder(CategoryMusician,
            "MuseScore",
            "Scores plus templates, styles, soundfonts and plugins.",
            [
                @"%USERPROFILE%\Documents\MuseScore<wildcard>",
                @"%LOCALAPPDATA%\MuseScore",
            ],
            [".mscz", ".mscx", ".mss", ".mpal", ".sf2", ".sf3", ".mid", ".midi", ".musicxml", ".mxl"],
            recursive: true,
            shortTag: "MuseScore");

        yield return Folder(CategoryMusician,
            "Audacity",
            "Settings, custom chains/macros, plug-ins.",
            [@"%APPDATA%\audacity"],
            [".aup3", ".aup", ".ny"],
            recursive: true,
            shortTag: "Audacity");
    }

    private static CatalogEntry Folder(
        string category,
        string name,
        string description,
        string[] paths,
        string[] extensions,
        bool recursive,
        string? shortTag = null)
        => new()
        {
            Category = category,
            Kind = CatalogEntryKind.Folder,
            Name = name,
            ShortTag = shortTag ?? string.Empty,
            Description = description,
            Paths = paths,
            Extensions = extensions,
            IncludeSubfolders = recursive,
            IsUserDefined = false,
        };
}
