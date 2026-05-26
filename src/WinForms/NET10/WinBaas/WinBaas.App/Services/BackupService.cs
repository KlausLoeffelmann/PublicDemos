using System.IO.Compression;
using Microsoft.Extensions.Logging;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IBackupService"/>
public sealed class BackupService(ILogger<BackupService> logger) : IBackupService
{
    private readonly ILogger<BackupService> _logger = logger;

    /// <inheritdoc />
    public Task BackupAsync(
        IReadOnlyList<DiscoveredItem> items,
        BackupOptions options,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(options);

        return options.Mode switch
        {
            BackupMode.CopyToFolder => Task.Run(() => CopyToFolder(items, options, progress, cancellationToken), cancellationToken),
            BackupMode.ZipArchive => Task.Run(() => ZipToArchive(items, options, progress, cancellationToken), cancellationToken),
            _ => Task.CompletedTask,
        };
    }

    private void CopyToFolder(
        IReadOnlyList<DiscoveredItem> items,
        BackupOptions options,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        Directory.CreateDirectory(options.Destination);

        for (int i = 0; i < items.Count; i++)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            DiscoveredItem item = items[i];
            try
            {
                if (item.IsFolder || string.IsNullOrEmpty(Path.GetExtension(item.FullPath)))
                {
                    // skip non-file kinds; only file items copy cleanly here.
                    continue;
                }

                string targetDir = Path.Combine(options.Destination, item.Source.Name);
                Directory.CreateDirectory(targetDir);
                string target = Path.Combine(targetDir, item.Name);
                File.Copy(item.FullPath, target, overwrite: true);
                progress?.Report($"Copied {item.FullPath}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to copy {Path}.", item.FullPath);
                progress?.Report($"Skipped {item.FullPath}: {ex.Message}");
            }
        }
    }

    private void ZipToArchive(
        IReadOnlyList<DiscoveredItem> items,
        BackupOptions options,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        string destination = options.Destination;
        if (!destination.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            destination = Path.ChangeExtension(destination, ".zip");
        }

        if (File.Exists(destination))
        {
            File.Delete(destination);
        }

        using var archive = ZipFile.Open(destination, ZipArchiveMode.Create);
        foreach (DiscoveredItem item in items)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            try
            {
                if (item.IsFolder || !File.Exists(item.FullPath))
                {
                    continue;
                }

                string entryName = Path.Combine(item.Source.Name, item.Name).Replace('\\', '/');
                archive.CreateEntryFromFile(item.FullPath, entryName, CompressionLevel.Optimal);
                progress?.Report($"Zipped {item.FullPath}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to zip {Path}.", item.FullPath);
                progress?.Report($"Skipped {item.FullPath}: {ex.Message}");
            }
        }
    }
}
