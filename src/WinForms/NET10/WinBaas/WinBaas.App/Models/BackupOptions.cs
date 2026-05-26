namespace WinBaas.Models;

/// <summary>
///  How a backup operation writes its output.
/// </summary>
public enum BackupMode
{
    /// <summary>Copy selected items into a destination folder, preserving structure.</summary>
    CopyToFolder,

    /// <summary>Pack selected items into a single ZIP archive.</summary>
    ZipArchive,
}

/// <summary>
///  Options consumed by <see cref="WinBaas.Services.IBackupService"/>.
/// </summary>
public sealed class BackupOptions
{
    /// <summary>Selected output mode.</summary>
    public BackupMode Mode { get; init; } = BackupMode.CopyToFolder;

    /// <summary>Destination folder (CopyToFolder) or ZIP file path (ZipArchive).</summary>
    public required string Destination { get; init; }
}
