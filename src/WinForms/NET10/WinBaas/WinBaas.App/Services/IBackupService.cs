using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Copies or archives a set of discovered items to a destination.
/// </summary>
public interface IBackupService
{
    /// <summary>
    ///  Performs the backup of <paramref name="selection"/> per <paramref name="options"/>.
    /// </summary>
    Task<BackupResult> BackupAsync(
        BackupSelection selection,
        BackupOptions options,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default);
}
