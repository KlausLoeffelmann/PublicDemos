using System.Globalization;
using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IBackupService"/>
public sealed class BackupService(ILogger<BackupService> logger) : IBackupService
{
    /// <summary>File name used for the Markdown backup manifest.</summary>
    public const string ManifestFileName = "WinBaas-Manifest.md";

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

        var manifestEntries = new List<(DiscoveredItem Item, string TargetPath, bool Copied)>();
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
                    manifestEntries.Add((item, item.FullPath, false));
                    continue;
                }

                string targetDir = Path.Combine(options.Destination, SafeName(item.Source.Name));
                Directory.CreateDirectory(targetDir);
                string target = Path.Combine(targetDir, item.Name);
                File.Copy(item.FullPath, target, overwrite: true);
                manifestEntries.Add((item, target, true));
                progress?.Report($"Copied {item.FullPath}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to copy {Path}.", item.FullPath);
                manifestEntries.Add((item, item.FullPath, false));
                progress?.Report($"Skipped {item.FullPath}: {ex.Message}");
            }
        }

        string manifestPath = Path.Combine(options.Destination, ManifestFileName);
        File.WriteAllText(manifestPath, BuildManifest(options, manifestEntries), Encoding.UTF8);
        progress?.Report($"Manifest written: {manifestPath}");
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

        var manifestEntries = new List<(DiscoveredItem Item, string TargetPath, bool Copied)>();
        using (var archive = ZipFile.Open(destination, ZipArchiveMode.Create))
        {
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
                        manifestEntries.Add((item, item.FullPath, false));
                        continue;
                    }

                    string entryName = Path.Combine(SafeName(item.Source.Name), item.Name).Replace('\\', '/');
                    archive.CreateEntryFromFile(item.FullPath, entryName, CompressionLevel.Optimal);
                    manifestEntries.Add((item, entryName, true));
                    progress?.Report($"Zipped {item.FullPath}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to zip {Path}.", item.FullPath);
                    manifestEntries.Add((item, item.FullPath, false));
                    progress?.Report($"Skipped {item.FullPath}: {ex.Message}");
                }
            }

            ZipArchiveEntry manifestEntry = archive.CreateEntry(ManifestFileName, CompressionLevel.Optimal);
            using var manifestStream = manifestEntry.Open();
            using var writer = new StreamWriter(manifestStream, Encoding.UTF8);
            writer.Write(BuildManifest(options, manifestEntries));
        }

        // Also drop a manifest next to the archive for easy reading.
        string sideCar = Path.ChangeExtension(destination, ".manifest.md");
        File.WriteAllText(sideCar, BuildManifest(options, manifestEntries), Encoding.UTF8);
        progress?.Report($"Manifest written: {sideCar}");
    }

    /// <summary>
    ///  Builds the Markdown backup manifest from <paramref name="entries"/>.
    /// </summary>
    private static string BuildManifest(
        BackupOptions options,
        IReadOnlyList<(DiscoveredItem Item, string TargetPath, bool Copied)> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# WinBaas Backup Manifest");
        sb.AppendLine();
        sb.Append("- **Generated:** ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture));
        sb.Append("- **Mode:** ").AppendLine(options.Mode == BackupMode.ZipArchive ? "ZIP archive" : "Copy-to-folder");
        sb.Append("- **Destination:** `").Append(options.Destination).AppendLine("`");

        int total = entries.Count(e => e.Copied);
        long totalBytes = entries.Where(e => e.Copied).Sum(e => e.Item.SizeBytes ?? 0L);
        sb.Append("- **Items copied:** ").Append(total).Append(" / ").AppendLine(entries.Count.ToString(CultureInfo.InvariantCulture));
        sb.Append("- **Total size:** ").AppendLine(FormatSize(totalBytes));
        sb.AppendLine();

        var grouped = entries
            .GroupBy(e => e.Item.Source.Category, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var categoryGroup in grouped)
        {
            sb.Append("## ").AppendLine(string.IsNullOrEmpty(categoryGroup.Key) ? "(Uncategorized)" : categoryGroup.Key);
            sb.AppendLine();

            var bySource = categoryGroup
                .GroupBy(e => e.Item.Source, ReferenceEqualityComparer.Instance)
                .OrderBy(g => ((CatalogEntry)g.Key).Name, StringComparer.OrdinalIgnoreCase);

            foreach (var sourceGroup in bySource)
            {
                var source = (CatalogEntry)sourceGroup.Key;
                int copied = sourceGroup.Count(e => e.Copied);
                long bytes = sourceGroup.Where(e => e.Copied).Sum(e => e.Item.SizeBytes ?? 0L);
                sb.Append("### ").Append(source.Name)
                  .Append(" — ").Append(copied).Append(" file(s), ").AppendLine(FormatSize(bytes));
                if (!string.IsNullOrWhiteSpace(source.Description))
                {
                    sb.Append("> ").AppendLine(source.Description);
                }
                sb.AppendLine();

                foreach (var entry in sourceGroup.OrderBy(e => e.Item.Name, StringComparer.OrdinalIgnoreCase))
                {
                    string status = entry.Copied ? "" : " _(skipped)_";
                    string changed = entry.Item.LastChanged is null
                        ? string.Empty
                        : ", " + entry.Item.LastChanged.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    string size = entry.Item.SizeBytes is null ? string.Empty : ", " + FormatSize(entry.Item.SizeBytes.Value);
                    sb.Append("- **").Append(entry.Item.Name).Append("**")
                      .Append(" — `").Append(entry.Item.FullPath).Append("`")
                      .Append(" → `").Append(entry.TargetPath).Append("`")
                      .Append(" _(").Append(entry.Item.FileTypeLabel).Append(size).Append(changed).Append(")_")
                      .AppendLine(status);
                }

                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static string FormatSize(long bytes)
    {
        string[] units = ["bytes", "KiB", "MiB", "GiB", "TiB"];
        double value = bytes;
        int unit = 0;
        while (value >= 1024d && unit < units.Length - 1)
        {
            value /= 1024d;
            unit++;
        }

        return unit == 0
            ? $"{bytes:N0} bytes"
            : string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1}", value, units[unit]);
    }

    private static string SafeName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return name;
    }
}

