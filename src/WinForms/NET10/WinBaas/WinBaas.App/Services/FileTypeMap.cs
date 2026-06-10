using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IFileTypeMap"/>
public sealed class FileTypeMap : IFileTypeMap
{
    private static readonly Dictionary<string, string> s_byExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        [".sln"] = "VS Solution",
        [".slnx"] = "VS V17+ Solution",
        [".cs"] = "C# Code File",
        [".vb"] = "VB Code File",
        [".csproj"] = "C# Project",
        [".vbproj"] = "VB Project",
        [".md"] = "Markdown File",
        [".txt"] = "Text File",
        [".rtf"] = "Rich Text",
        [".jpeg"] = "JPEG Image",
        [".jpg"] = "JPEG Image",
        [".png"] = "PNG Image",
        [".gif"] = "GIF Image",
        [".bmp"] = "BMP Image",
        [".heic"] = "HEIF Image",
        [".heif"] = "HEIF Image",
        [".mp4"] = "Video File",
        [".mov"] = "Video File",
        [".avi"] = "Video File",
        [".webm"] = "Video File",
        [".mkv"] = "Video File",
        [".wav"] = "Audio File",
        [".m4a"] = "Audio File",
        [".mp3"] = "Audio File",
        [".json"] = "Configuration File",
        [".xml"] = "Configuration File",
        [".config"] = "Configuration File",
        [".ini"] = "Configuration File",
        [".toml"] = "Configuration File",
        [".yaml"] = "Configuration File",
        [".yml"] = "Configuration File",
        [".url"] = "Edge Favorite",
        [".camproj"] = "Camtasia Recording",
        [".camrec"] = "Camtasia Recording",
        [".trec"] = "Camtasia Recording",
        [".lua"] = "Lua Script",
        [".ps1"] = "PowerShell Script",
        [".cmd"] = "Batch Script",
        [".bat"] = "Batch Script",
        [".sql"] = "SQL Script",
        [".bak"] = "SQL Backup",
        [".mdf"] = "SQL Data File",
        [".ldf"] = "SQL Log File",
        [".pdf"] = "PDF Document",
        [".doc"] = "Word Document",
        [".docx"] = "Word Document",
        [".xls"] = "Excel Workbook",
        [".xlsx"] = "Excel Workbook",
        [".ppt"] = "PowerPoint Presentation",
        [".pptx"] = "PowerPoint Presentation",
        [".zip"] = "ZIP Archive",
        [".7z"] = "7-Zip Archive",
        [".rar"] = "RAR Archive",
    };

    /// <summary>
    ///  Known config / data filenames (basename, case-insensitive) → friendly
    ///  label. Looked up <em>before</em> the extension map so that
    ///  e.g. <c>settings.json</c> wins over generic <c>"Configuration File"</c>.
    /// </summary>
    private static readonly Dictionary<string, string> s_byFileName = new(StringComparer.OrdinalIgnoreCase)
    {
        // VS Code / Cursor user files
        ["settings.json"] = "User Settings",
        ["keybindings.json"] = "Keybindings",
        ["tasks.json"] = "Tasks",
        ["launch.json"] = "Launch Config",
        ["argv.json"] = "Runtime Args",
        ["snippets.json"] = "Snippets",

        // .NET / NuGet
        ["NuGet.Config"] = "NuGet Config",
        ["nuget.config"] = "NuGet Config",
        ["global.json"] = ".NET SDK Selector",
        ["Directory.Packages.props"] = "Central Package Versions",
        ["Directory.Build.props"] = "Build Defaults",
        ["Directory.Build.targets"] = "Build Targets",
        ["appsettings.json"] = ".NET App Settings",
        [".editorconfig"] = "EditorConfig",

        // Git / dev environment
        [".gitconfig"] = "Git Global Config",
        [".gitignore"] = "Git Ignore",
        [".gitignore_global"] = "Git Global Ignore",
        [".gitattributes"] = "Git Attributes",

        // Node
        ["package.json"] = "npm Manifest",
        ["package-lock.json"] = "npm Lockfile",
        [".npmrc"] = "npm Config",
        [".nvmrc"] = "Node Version",
        ["tsconfig.json"] = "TypeScript Config",

        // Docker
        ["Dockerfile"] = "Dockerfile",
        ["compose.yaml"] = "Compose File",
        ["compose.yml"] = "Compose File",
        ["docker-compose.yml"] = "Compose File",
        ["docker-compose.yaml"] = "Compose File",

        // WSL
        [".wslconfig"] = "WSL Config",

        // Photoshop / Adobe specific data
        ["Actions Palette.psp"] = "Actions Palette State",

        // OBS / DAW project files (well-known names)
        ["reaper.ini"] = "Reaper Config",
        ["Defaults.xml"] = "DAW Defaults",
        ["Key Commands.xml"] = "Key Commands",

        // Copilot / AI tool config
        ["copilot-instructions.md"] = "Copilot Instructions",
        ["AGENTS.md"] = "Copilot Agents Manifest",
        ["SKILL.md"] = "Copilot Skill",
        ["mcp.json"] = "MCP Server List",
        ["claude.json"] = "Claude Code Config",
        ["CLAUDE.md"] = "Claude Project Instructions",
        ["config.toml"] = "Codex CLI Config",
    };

    /// <inheritdoc />
    public string GetLabel(string path, CatalogEntry? source = null)
    {
        string fileName = System.IO.Path.GetFileName(path);
        string ext = System.IO.Path.GetExtension(path);

        string? specific = null;

        // Folder names like ".github/skills/<x>/SKILL.md" — keep specific match.
        if (!string.IsNullOrEmpty(fileName) && s_byFileName.TryGetValue(fileName, out var named))
        {
            specific = named;
        }
        else if (!string.IsNullOrEmpty(ext) && s_byExtension.TryGetValue(ext, out var extLabel))
        {
            specific = extLabel;
        }
        else if (!string.IsNullOrEmpty(ext))
        {
            specific = ext.TrimStart('.').ToUpperInvariant() + " File";
        }

        string tag = ResolveTag(source);
        if (string.IsNullOrEmpty(tag))
        {
            return specific ?? string.Empty;
        }

        return specific is null
            ? tag
            : $"{tag} \u00B7 {specific}";
    }

    private static string ResolveTag(CatalogEntry? source)
    {
        if (source is null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(source.ShortTag))
        {
            return source.ShortTag;
        }

        // Strip parenthetical suffixes so "Downloads (office docs)" -> "Downloads".
        string name = source.Name;
        int paren = name.IndexOf('(');
        return (paren > 0 ? name[..paren] : name).Trim();
    }
}

