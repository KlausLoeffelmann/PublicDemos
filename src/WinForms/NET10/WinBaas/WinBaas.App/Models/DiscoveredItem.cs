namespace WinBaas.Models;

/// <summary>
///  A single discovered item (file, folder, env-var, or database) that is
///  a candidate for backup.
/// </summary>
public sealed class DiscoveredItem
{
    /// <summary>The owning catalog entry.</summary>
    public required CatalogEntry Source { get; init; }

    /// <summary>Display name (file/folder name, env-var name, or database name).</summary>
    public required string Name { get; init; }

    /// <summary>Full path or qualified identifier of the discovered item.</summary>
    public required string FullPath { get; init; }

    /// <summary>User-friendly file type label (e.g. "PDF Document").</summary>
    public string FileTypeLabel { get; init; } = string.Empty;

    /// <summary>Last-changed timestamp, or <see langword="null"/> if unknown.</summary>
    public DateTime? LastChanged { get; init; }

    /// <summary>Created timestamp, or <see langword="null"/> if unknown.</summary>
    public DateTime? Created { get; init; }

    /// <summary>
    ///  Size in bytes. <see langword="null"/> when the size has not yet been computed
    ///  (e.g. folder size that is computed asynchronously).
    /// </summary>
    public long? SizeBytes { get; set; }

    /// <summary>True if this item represents a folder rather than a single file.</summary>
    public bool IsFolder { get; init; }

    /// <summary>True if the catalog entry yields only folders (no individual files).</summary>
    public bool IsFolderOnlyResult { get; init; }

    /// <summary>True if the user has marked this item to back up.</summary>
    public bool IsChecked { get; set; }
}
