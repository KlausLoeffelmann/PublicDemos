using System.Globalization;
using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IBackupService"/>
public sealed class BackupService(ILogger<BackupService> logger) : IBackupService
{
    private readonly ILogger<BackupService> _logger = logger;

    /// <inheritdoc />
    public Task<BackupResult> BackupAsync(
        IReadOnlyList<DiscoveredItem> items,
        BackupOptions options,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(options);

        return options.Mode switch
        {
            BackupMode.ZipArchive => Task.Run(() => ZipToArchive(items, options, progress, cancellationToken), cancellationToken),
            _ => Task.Run(() => CopyToFolder(items, options, progress, cancellationToken), cancellationToken),
        };
    }

    /// <summary>
    ///  Builds the timestamped folder/file name used for a fresh backup:
    ///  <c>WinBaas-{MachineName}-{yyyy-MM-dd--(UTC±HH)HH-mm-ss}</c>.
    /// </summary>
    public static string BuildStampName(DateTimeOffset stamp)
    {
        TimeSpan offset = stamp.Offset;
        string sign = offset.Ticks >= 0 ? "+" : "-";
        string zone = $"UTC{sign}{Math.Abs(offset.Hours):00}";

        return string.Format(
            CultureInfo.InvariantCulture,
            "WinBaas-{0}-{1:yyyy-MM-dd}--({2}){1:HH-mm-ss}",
            SafeName(Environment.MachineName),
            stamp,
            zone);
    }

    private BackupResult CopyToFolder(
        IReadOnlyList<DiscoveredItem> items,
        BackupOptions options,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        DateTimeOffset stamp = DateTimeOffset.Now;
        string finalDir = Path.Combine(options.Destination, BuildStampName(stamp));
        Directory.CreateDirectory(finalDir);

        // Create the report file *first* so its CreationTime is captured at
        // the moment the backup truly began. We fill its content last.
        Guid reportId = Guid.NewGuid();
        string reportPath = Path.Combine(finalDir, $"WinBaas-{reportId}.md");
        File.Create(reportPath).Dispose();
        DateTime reportCreatedAt = File.GetCreationTime(reportPath);

        var entries = new List<BackupEntry>(items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            DiscoveredItem item = items[i];
            try
            {
                if (item.IsFolder || string.IsNullOrEmpty(Path.GetExtension(item.FullPath)) || !File.Exists(item.FullPath))
                {
                    entries.Add(new BackupEntry(item, item.FullPath, Copied: false, Skipped: true, Reason: "not a regular file"));
                    continue;
                }

                string targetDir = Path.Combine(finalDir, SafeName(item.Source.Name));
                Directory.CreateDirectory(targetDir);
                string target = Path.Combine(targetDir, item.Name);
                File.Copy(item.FullPath, target, overwrite: true);
                entries.Add(new BackupEntry(item, target, Copied: true, Skipped: false, Reason: null));
                progress?.Report($"Copied {item.FullPath}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to copy {Path}.", item.FullPath);
                entries.Add(new BackupEntry(item, item.FullPath, Copied: false, Skipped: true, Reason: ex.Message));
                progress?.Report($"Skipped {item.FullPath}: {ex.Message}");
            }
        }

        string content = BuildReport(reportId, reportCreatedAt, BackupMode.CopyToFolder, finalDir, entries);
        File.WriteAllText(reportPath, content, Encoding.UTF8);
        File.SetCreationTime(reportPath, reportCreatedAt);

        progress?.Report($"Report written: {reportPath}");
        return new BackupResult(finalDir, reportPath, reportId, entries.Count(e => e.Copied));
    }

    private BackupResult ZipToArchive(
        IReadOnlyList<DiscoveredItem> items,
        BackupOptions options,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        DateTimeOffset stamp = DateTimeOffset.Now;
        string root = Path.GetDirectoryName(options.Destination) ?? options.Destination;
        if (string.IsNullOrEmpty(root))
        {
            root = options.Destination;
        }

        Directory.CreateDirectory(root);
        string archiveName = BuildStampName(stamp) + ".zip";
        string destination = Path.Combine(root, archiveName);
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }

        Guid reportId = Guid.NewGuid();
        string reportPath = Path.Combine(root, $"WinBaas-{reportId}.md");
        File.Create(reportPath).Dispose();
        DateTime reportCreatedAt = File.GetCreationTime(reportPath);

        var entries = new List<BackupEntry>(items.Count);
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
                        entries.Add(new BackupEntry(item, item.FullPath, Copied: false, Skipped: true, Reason: "not a regular file"));
                        continue;
                    }

                    string entryName = Path.Combine(SafeName(item.Source.Name), item.Name).Replace('\\', '/');
                    archive.CreateEntryFromFile(item.FullPath, entryName, CompressionLevel.Optimal);
                    entries.Add(new BackupEntry(item, entryName, Copied: true, Skipped: false, Reason: null));
                    progress?.Report($"Zipped {item.FullPath}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to zip {Path}.", item.FullPath);
                    entries.Add(new BackupEntry(item, item.FullPath, Copied: false, Skipped: true, Reason: ex.Message));
                    progress?.Report($"Skipped {item.FullPath}: {ex.Message}");
                }
            }

            string content = BuildReport(reportId, reportCreatedAt, BackupMode.ZipArchive, destination, entries);

            // Embed the report inside the archive too.
            ZipArchiveEntry embedded = archive.CreateEntry($"WinBaas-{reportId}.md", CompressionLevel.Optimal);
            using (var s = embedded.Open())
            using (var w = new StreamWriter(s, Encoding.UTF8))
            {
                w.Write(content);
            }

            // And alongside the .zip for easy viewing.
            File.WriteAllText(reportPath, content, Encoding.UTF8);
            File.SetCreationTime(reportPath, reportCreatedAt);
        }

        progress?.Report($"Report written: {reportPath}");
        return new BackupResult(destination, reportPath, reportId, entries.Count(e => e.Copied));
    }

    private static string BuildReport(
        Guid reportId,
        DateTime reportCreatedAt,
        BackupMode mode,
        string finalDestination,
        IReadOnlyList<BackupEntry> entries)
    {
        var copied = entries.Where(e => e.Copied).ToList();
        int totalFiles = copied.Count;
        var byCategory = copied
            .GroupBy(e => string.IsNullOrEmpty(e.Item.Source.Category) ? "(Uncategorized)" : e.Item.Source.Category, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();
        int disciplines = byCategory.Count;
        int folderCount = copied
            .Select(e => Path.GetDirectoryName(e.Item.FullPath) ?? string.Empty)
            .Where(d => !string.IsNullOrEmpty(d))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var sb = new StringBuilder();
        sb.Append("# WinBaas Backup Log - ")
          .Append(FormatTitleDate(reportCreatedAt))
          .AppendLine();
        sb.AppendLine();
        sb.Append("## Backup-ID: ").Append(reportId).AppendLine();
        sb.AppendLine();
        sb.Append("- **Mode:** ").AppendLine(mode == BackupMode.ZipArchive ? "ZIP archive" : "Copy-to-folder");
        sb.Append("- **Destination:** `").Append(finalDestination).AppendLine("`");
        sb.Append("- **Machine:** ").AppendLine(Environment.MachineName);
        sb.AppendLine();
        sb.Append("Found ").Append(totalFiles).Append(" files to back-up from ")
          .Append(disciplines).Append(" disciplines and ")
          .Append(folderCount).AppendLine(" folders.");
        sb.AppendLine();

        foreach (var category in byCategory)
        {
            var stats = ComputeStats(category);
            sb.Append("## ").AppendLine(category.Key);
            sb.AppendLine();
            sb.Append("**").Append(category.Key).Append(":** ")
              .Append(stats.FileCount).Append(" files in ")
              .Append(stats.FolderCount).AppendLine(" folders.");
            sb.AppendLine();

            if (stats.Newest is { } newest)
            {
                AppendStatBullet(sb, "Newest", $"from {newest.Item.LastChanged:yy-MM-dd HH:mm}", newest);
            }
            if (stats.Oldest is { } oldest)
            {
                AppendStatBullet(sb, "Oldest", $"from {oldest.Item.LastChanged:yy-MM-dd HH:mm}", oldest);
            }
            if (stats.Smallest is { } smallest)
            {
                AppendStatBullet(sb, "Smallest", $"with {FormatSize(smallest.Item.SizeBytes ?? 0)}", smallest);
            }
            if (stats.Biggest is { } biggest)
            {
                AppendStatBullet(sb, "Biggest", $"with {FormatSize(biggest.Item.SizeBytes ?? 0)}", biggest);
            }
            sb.AppendLine();

            AppendCategoryDetails(sb, category);
        }

        return sb.ToString();
    }

    private static (int FileCount, int FolderCount, BackupEntry? Newest, BackupEntry? Oldest, BackupEntry? Smallest, BackupEntry? Biggest)
        ComputeStats(IGrouping<string, BackupEntry> category)
    {
        var list = category.ToList();
        int fileCount = list.Count;
        int folderCount = list
            .Select(e => Path.GetDirectoryName(e.Item.FullPath) ?? string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        BackupEntry? newest = list
            .Where(e => e.Item.LastChanged.HasValue)
            .OrderByDescending(e => e.Item.LastChanged!.Value)
            .FirstOrDefault();
        BackupEntry? oldest = list
            .Where(e => e.Item.LastChanged.HasValue)
            .OrderBy(e => e.Item.LastChanged!.Value)
            .FirstOrDefault();
        BackupEntry? smallest = list
            .Where(e => e.Item.SizeBytes.HasValue)
            .OrderBy(e => e.Item.SizeBytes!.Value)
            .FirstOrDefault();
        BackupEntry? biggest = list
            .Where(e => e.Item.SizeBytes.HasValue)
            .OrderByDescending(e => e.Item.SizeBytes!.Value)
            .FirstOrDefault();

        return (fileCount, folderCount, newest, oldest, smallest, biggest);
    }

    private static void AppendStatBullet(StringBuilder sb, string label, string detail, BackupEntry entry)
    {
        string dir = Path.GetDirectoryName(entry.Item.FullPath) ?? string.Empty;
        sb.Append(" - **").Append(label).Append("** ")
          .Append(detail).Append(": `")
          .Append(entry.Item.Name).Append("`")
          .Append(" in `").Append(dir).Append('`')
          .AppendLine();
    }

    /// <summary>
    ///  Emits category-specific extras after the standard stats:
    ///  Photoshop / Adobe action set listing, VS Code extensions,
    ///  Visual Studio extensions.
    /// </summary>
    private static void AppendCategoryDetails(StringBuilder sb, IGrouping<string, BackupEntry> category)
    {
        var actions = category
            .Where(e => string.Equals(Path.GetExtension(e.Item.FullPath), ".atn", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (actions.Count > 0)
        {
            sb.AppendLine("### Adobe — installed Actions");
            sb.AppendLine();
            foreach (BackupEntry a in actions.OrderBy(a => a.Item.Name, StringComparer.OrdinalIgnoreCase))
            {
                sb.Append(" - `").Append(a.Item.Name).Append("` — `").Append(a.Item.FullPath).Append('`').AppendLine();
            }
            sb.AppendLine();
        }

        if (category.Any(e => Matches(e.Item.Source.ShortTag, "VS Code", "VS Code Copilot", "Cursor")))
        {
            AppendDirectoryListing(
                sb,
                "VS Code / Cursor — installed extensions",
                Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.vscode\extensions"));
            AppendDirectoryListing(
                sb,
                "Cursor — installed extensions",
                Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.cursor\extensions"));
        }

        if (category.Any(e => Matches(e.Item.Source.ShortTag, "Visual Studio", "VS Copilot")))
        {
            string root = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Microsoft\VisualStudio");
            if (Directory.Exists(root))
            {
                foreach (string instance in Directory.EnumerateDirectories(root, "17.*"))
                {
                    string extDir = Path.Combine(instance, "Extensions");
                    AppendDirectoryListing(sb, $"Visual Studio — installed extensions ({Path.GetFileName(instance)})", extDir);
                }
            }
        }
    }

    private static bool Matches(string tag, params string[] candidates)
        => candidates.Any(c => c.Equals(tag, StringComparison.OrdinalIgnoreCase));

    private static void AppendDirectoryListing(StringBuilder sb, string heading, string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        string[] dirs;
        try
        {
            dirs = Directory.GetDirectories(directory);
        }
        catch
        {
            return;
        }

        if (dirs.Length == 0)
        {
            return;
        }

        sb.Append("### ").AppendLine(heading);
        sb.AppendLine();
        sb.Append("_Source folder:_ `").Append(directory).AppendLine("`");
        sb.AppendLine();
        foreach (string d in dirs.OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
        {
            sb.Append(" - `").Append(Path.GetFileName(d)).Append('`').AppendLine();
        }
        sb.AppendLine();
    }

    /// <summary>
    ///  Builds the H1 title date: <c>{dddd}, {MMM} {d&lt;ordinal&gt;} {yyyy} {HH}:{mm}</c>.
    /// </summary>
    internal static string FormatTitleDate(DateTime when)
    {
        string day = when.Day.ToString(CultureInfo.InvariantCulture) + OrdinalSuffix(when.Day);
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0:dddd}, {0:MMM} {1} {0:yyyy} {0:HH}:{0:mm}",
            when,
            day);
    }

    /// <summary>
    ///  English ordinal suffix for a day-of-month: <c>1 → "st"</c>,
    ///  <c>2 → "nd"</c>, <c>3 → "rd"</c>, <c>4-20 → "th"</c>,
    ///  <c>21 → "st"</c>, etc. .NET has no built-in format specifier for this.
    /// </summary>
    internal static string OrdinalSuffix(int day)
    {
        int hundredth = day % 100;
        if (hundredth is >= 11 and <= 13)
        {
            return "th";
        }

        return (day % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
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

    private sealed record BackupEntry(
        DiscoveredItem Item,
        string TargetPath,
        bool Copied,
        bool Skipped,
        string? Reason);
}
