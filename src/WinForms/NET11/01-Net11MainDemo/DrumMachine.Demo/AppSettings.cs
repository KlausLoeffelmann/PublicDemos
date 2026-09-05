namespace DrumMachine.Demo;

/// <summary>
///  Selects the application's color mode at startup rather than changing a running form's theme.
/// </summary>
internal enum AppTheme
{
    /// <summary>
    ///  Uses the classic Windows Forms color mode.
    /// </summary>
    Classic,

    /// <summary>
    ///  Requests the Windows Forms dark color mode.
    /// </summary>
    Dark,

    /// <summary>
    ///  Uses the operating system's color preference at launch.
    /// </summary>
    System
}

/// <summary>
///  Defines toolbar glyph sizes in logical pixels at the 96-DPI design baseline.
/// </summary>
internal enum ToolbarIconSize
{
    /// <summary>
    ///  Renders toolbar icons at thirty-two logical pixels.
    /// </summary>
    Small = 32,

    /// <summary>
    ///  Renders toolbar icons at forty-eight logical pixels.
    /// </summary>
    Medium = 48,

    /// <summary>
    ///  Renders toolbar icons at sixty-four logical pixels.
    /// </summary>
    Large = 64
}

/// <summary>
///  Keeps user-interface preferences and recent paths outside musical documents and their Undo history.
/// </summary>
internal sealed record AppSettings
{
    private const int MaximumPathCharacters = 4_096;
    private IReadOnlyList<string> _recentFiles = Array.Empty<string>();

    /// <summary>
    ///  The maximum number of successfully opened or saved loop files retained in the menu.
    /// </summary>
    public const int MaximumRecentFiles = 5;

    /// <summary>
    ///  Gets the color mode to apply before constructing UI on the next launch.
    /// </summary>
    public AppTheme Theme { get; init; } = AppTheme.System;

    /// <summary>
    ///  Gets the immediately applicable toolbar icon size, independently of the menu glyph size.
    /// </summary>
    public ToolbarIconSize IconSize { get; init; } = ToolbarIconSize.Small;

    /// <summary>
    ///  Gets the initial folder for Open and untitled Save As without changing a named document's path.
    /// </summary>
    public string DefaultFolder { get; init; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    ///  Gets a defensive snapshot of recent paths, ordered with the latest successful operation first.
    /// </summary>
    public IReadOnlyList<string> RecentFiles
    {
        get => _recentFiles;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _recentFiles = Array.AsReadOnly(value.ToArray());
        }
    }

    /// <summary>
    ///  Gets the one- or two-bar viewport size, which never changes a score's actual length.
    /// </summary>
    public int BarsPerView { get; init; } = 1;

    /// <summary>
    ///  Moves a normalized Windows path to the front, deduplicates case-insensitively, and retains at most five.
    /// </summary>
    public AppSettings WithRecentFile(string path)
    {
        string normalized = NormalizeFilePath(path);
        List<string> recent = [normalized];
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase) { normalized };
        foreach (string previous in RecentFiles)
        {
            string fullPath = NormalizeFilePath(previous);
            if (seen.Add(fullPath))
            {
                recent.Add(fullPath);
                if (recent.Count == MaximumRecentFiles)
                {
                    break;
                }
            }
        }

        return this with { RecentFiles = recent };
    }

    /// <summary>
    ///  Removes only the specified normalized recent path without probing unrelated files or folders.
    /// </summary>
    public AppSettings WithRemovedRecentFile(string path)
    {
        string normalized = NormalizeFilePath(path);
        return this with
        {
            RecentFiles = RecentFiles
                .Select(NormalizeFilePath)
                .Where(previous => !StringComparer.OrdinalIgnoreCase.Equals(previous, normalized))
                .ToArray()
        };
    }

    /// <summary>
    ///  Validates persisted choices and paths without requiring removable or network folders to be online.
    /// </summary>
    internal AppSettings ValidateAndNormalize()
    {
        if (!Enum.IsDefined(Theme) || !Enum.IsDefined(IconSize))
        {
            throw new ArgumentException("The theme or toolbar icon size is unsupported.");
        }

        if (BarsPerView is not (1 or 2))
        {
            throw new ArgumentOutOfRangeException(nameof(BarsPerView), "The view must show one or two bars.");
        }

        if (string.IsNullOrWhiteSpace(DefaultFolder) || !Path.IsPathFullyQualified(DefaultFolder))
        {
            throw new ArgumentException("The default folder must be a fully qualified Windows path.");
        }

        string folder = NormalizePath(DefaultFolder);
        if (RecentFiles.Count > MaximumRecentFiles)
        {
            throw new ArgumentException("At most five recent files may be persisted.");
        }

        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        List<string> recent = [];
        foreach (string path in RecentFiles)
        {
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
            {
                throw new ArgumentException("Recent files must use fully qualified Windows paths.");
            }

            string fullPath = NormalizeFilePath(path);
            if (!seen.Add(fullPath))
            {
                throw new ArgumentException("The recent-file list contains a duplicate Windows path.");
            }

            recent.Add(fullPath);
        }

        return this with { DefaultFolder = folder, RecentFiles = recent };
    }

    private static string NormalizeFilePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (Path.EndsInDirectorySeparator(path))
        {
            throw new ArgumentException("A recent-file path must include a filename.", nameof(path));
        }

        string fullPath = NormalizePath(path);
        if (Path.GetFileName(fullPath).Length == 0)
        {
            throw new ArgumentException("A recent-file path must include a filename.", nameof(path));
        }

        return fullPath;
    }

    private static string NormalizePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (path.Length > MaximumPathCharacters || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            throw new ArgumentException("The path is invalid or exceeds the supported length.", nameof(path));
        }

        string fullPath = Path.GetFullPath(path);
        if (fullPath.Length > MaximumPathCharacters)
        {
            throw new ArgumentException("The path exceeds the supported length.", nameof(path));
        }

        string root = Path.GetPathRoot(fullPath)!;
        char[] invalidNameCharacters = Path.GetInvalidFileNameChars();
        foreach (string part in fullPath[root.Length..].Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
        {
            if (part.IndexOfAny(invalidNameCharacters) >= 0 || part.EndsWith(' ') || part.EndsWith('.'))
            {
                throw new ArgumentException("A path component contains invalid or ambiguous Windows filename characters.", nameof(path));
            }
        }

        return Path.TrimEndingDirectorySeparator(fullPath);
    }
}
