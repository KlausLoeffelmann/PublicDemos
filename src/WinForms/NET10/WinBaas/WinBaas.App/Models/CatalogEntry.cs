namespace WinBaas.Models;

/// <summary>
///  Describes one entry in the WinBaas catalog of "easy to forget"
///  backup sources.
/// </summary>
/// <remarks>
///  <para>
///   Entries are either built-in (seeded by <see cref="WinBaas.Services.CatalogService"/>)
///   or user-defined (added via the Add Object dialog).
///  </para>
/// </remarks>
public sealed class CatalogEntry
{
    /// <summary>Stable identifier for the entry.</summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Category used as a parent node in the TreeView (e.g. "Developer Tools").</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>Human-readable name shown in the TreeView.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Long-form description shown as tooltip / details.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>The kind of source.</summary>
    public CatalogEntryKind Kind { get; init; }

    /// <summary>
    ///  One or more backup paths for the entry. May contain
    ///  <c>%APPDATA%</c>-style environment variables or
    ///  <c>&lt;version&gt;</c> / <c>&lt;lang&gt;</c> wildcard segments that
    ///  <see cref="WinBaas.Services.DiscoveryService"/> expands at scan time.
    /// </summary>
    public IReadOnlyList<string> Paths { get; init; } = [];

    /// <summary>File extensions (with leading dot) to match for folder entries.</summary>
    public IReadOnlyList<string> Extensions { get; init; } = [];

    /// <summary>Specific file names to match in addition to extensions.</summary>
    public IReadOnlyList<string> KnownFileNames { get; init; } = [];

    /// <summary>Whether folder entries should descend into subfolders.</summary>
    public bool IncludeSubfolders { get; init; }

    /// <summary>True for entries added by the user; false for built-in seeds.</summary>
    public bool IsUserDefined { get; init; }
}

