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

    /// <summary>
    ///  The folder (CopyToFolder) or file path (ZipArchive) the user picked.
    ///  The <see cref="WinBaas.Services.IBackupService"/> derives the actual
    ///  destination from this by inserting a
    ///  <c>WinBaas-{machine}-{timestamp}</c> segment so each backup gets its
    ///  own directory.
    /// </summary>
    public required string Destination { get; init; }
}

/// <summary>
///  Outcome of a backup run.
/// </summary>
/// <param name="FinalDestination">
///  The actual destination directory (CopyToFolder) or .zip file (ZipArchive)
///  produced by the backup, after the timestamp/machine subfolder has been
///  applied.
/// </param>
/// <param name="ReportPath">Full path to the <c>WinBaas-{Guid}.md</c> log.</param>
/// <param name="ReportId">The <see cref="Guid"/> embedded in the log filename.</param>
/// <param name="CopiedFileCount">Number of items successfully copied or zipped.</param>
public sealed record BackupResult(
    string FinalDestination,
    string ReportPath,
    Guid ReportId,
    int CopiedFileCount);

