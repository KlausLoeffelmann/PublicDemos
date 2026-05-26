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

    /// <summary>Human-readable name shown in the TreeView.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Long-form description shown as tooltip / details.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>The kind of source.</summary>
    public CatalogEntryKind Kind { get; init; }

    /// <summary>
    ///  The base path for folder/file entries; the variable name for environment
    ///  variables; the instance name for SQL Server entries.
    /// </summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>File extensions (with leading dot) to match for folder entries.</summary>
    public IReadOnlyList<string> Extensions { get; init; } = [];

    /// <summary>Specific file names to match in addition to extensions.</summary>
    public IReadOnlyList<string> KnownFileNames { get; init; } = [];

    /// <summary>Whether folder entries should descend into subfolders.</summary>
    public bool IncludeSubfolders { get; init; }

    /// <summary>True for entries added by the user; false for built-in seeds.</summary>
    public bool IsUserDefined { get; init; }
}
