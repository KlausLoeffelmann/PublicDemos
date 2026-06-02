using System.Globalization;
using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IBackupService"/>
public sealed class BackupService(ILogger<BackupService> logger) : IBackupService
{
    private static readonly string[] s_visualStudioCacheDirectories =
    [
        "ComponentModelCache",
        "Backup Files",
        "ImageLibrary",
        "MEFCache",
    ];

    private readonly ILogger<BackupService> _logger = logger;

    /// <inheritdoc />
    public Task<BackupResult> BackupAsync(
        BackupSelection selection,
        BackupOptions options,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentNullException.ThrowIfNull(options);

        return options.Mode switch
        {
            BackupMode.ZipArchive => Task.Run(() => ZipToArchive(selection, options, progress, cancellationToken), cancellationToken),
            _ => Task.Run(() => CopyToFolder(selection, options, progress, cancellationToken), cancellationToken),
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
        BackupSelection selection,
        BackupOptions options,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        DateTimeOffset stamp = DateTimeOffset.Now;
        string finalDir = Path.Combine(options.Destination, BuildStampName(stamp));
        Directory.CreateDirectory(finalDir);

        BackupMaterial material = PopulateFolderLayout(selection, finalDir, BackupMode.CopyToFolder, progress, ct);
        string content = BuildReport(material.ReportId, material.ReportCreatedAt, BackupMode.CopyToFolder, finalDir, material.Entries, material.SpecialSections);
        File.WriteAllText(material.ReportPath, content, Encoding.UTF8);
        File.SetCreationTime(material.ReportPath, material.ReportCreatedAt);

        progress?.Report($"Report written: {material.ReportPath}");
        return new BackupResult(finalDir, material.ReportPath, material.ReportId, material.Entries.Count(entry => entry.Copied));
    }

    private BackupResult ZipToArchive(
        BackupSelection selection,
        BackupOptions options,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        DateTimeOffset stamp = DateTimeOffset.Now;

        // The user always picks a destination folder (FolderBrowserDialog), so
        // treat Destination as the root directory the .zip is written into,
        // mirroring the copy-to-folder branch.
        string root = options.Destination;
        if (string.IsNullOrEmpty(root))
        {
            root = Directory.GetCurrentDirectory();
        }

        Directory.CreateDirectory(root);
        string stagingFolder = Path.Combine(root, BuildStampName(stamp));
        if (Directory.Exists(stagingFolder))
        {
            Directory.Delete(stagingFolder, recursive: true);
        }

        BackupMaterial material = PopulateFolderLayout(selection, stagingFolder, BackupMode.ZipArchive, progress, ct);
        string content = BuildReport(material.ReportId, material.ReportCreatedAt, BackupMode.ZipArchive, stagingFolder, material.Entries, material.SpecialSections);
        File.WriteAllText(material.ReportPath, content, Encoding.UTF8);
        File.SetCreationTime(material.ReportPath, material.ReportCreatedAt);

        string destination = Path.Combine(root, BuildStampName(stamp) + ".zip");
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }

        ZipFile.CreateFromDirectory(stagingFolder, destination, CompressionLevel.Optimal, includeBaseDirectory: false);

        string externalReportPath = Path.Combine(root, $"WinBaas-{material.ReportId}.md");
        File.Copy(material.ReportPath, externalReportPath, overwrite: true);
        File.SetCreationTime(externalReportPath, material.ReportCreatedAt);

        try
        {
            Directory.Delete(stagingFolder, recursive: true);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Could not remove temporary folder {Folder} after creating ZIP backup.", stagingFolder);
        }

        progress?.Report($"Report written: {externalReportPath}");
        return new BackupResult(destination, externalReportPath, material.ReportId, material.Entries.Count(entry => entry.Copied));
    }

    private BackupMaterial PopulateFolderLayout(
        BackupSelection selection,
        string finalDir,
        BackupMode mode,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        Directory.CreateDirectory(finalDir);

        Guid reportId = Guid.NewGuid();
        string reportPath = Path.Combine(finalDir, $"WinBaas-{reportId}.md");
        File.Create(reportPath).Dispose();
        DateTime reportCreatedAt = File.GetCreationTime(reportPath);

        List<BackupEntry> entries = CopyRegularFiles(selection.FileItems, finalDir, progress, ct);
        var specialSections = new List<string>();

        if (selection.RegistryItems.Any(item => item.IsChecked && item.IsPresent))
        {
            string registryFolder = WriteRegistryBackup(finalDir, selection.RegistryItems, progress);
            specialSections.Add(
                string.Join(Environment.NewLine,
                    "## Registry Backup",
                    string.Empty,
                    $"- Folder: `{registryFolder}`",
                    $"- Selected values: {selection.RegistryItems.Count(item => item.IsChecked && item.IsPresent)}",
                    $"- Catalog values emitted as comments: {selection.RegistryItems.Count(item => !item.IsPresent)}",
                    string.Empty));
        }

        IReadOnlyList<VsSku> selectedSkus = selection.VisualStudioSkus.Where(sku => sku.IsChecked).ToList();
        if (selectedSkus.Count > 0)
        {
            string visualStudioFolder = BackupVisualStudioSkus(finalDir, selectedSkus, progress, ct);
            var sb = new StringBuilder();
            sb.AppendLine("## Visual Studio");
            sb.AppendLine();
            sb.Append("- Folder: `").Append(visualStudioFolder).AppendLine("`");
            sb.Append("- Selected SKUs: ").AppendLine(selectedSkus.Count.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine();
            foreach (VsSku sku in selectedSkus)
            {
                sb.Append("- ").Append(sku.NodeLabel)
                    .Append(" — ").Append(sku.Hives.Count.ToString(CultureInfo.InvariantCulture)).Append(" hive(s), ")
                    .Append(sku.Extensions.Count.ToString(CultureInfo.InvariantCulture)).AppendLine(" extension(s)");
            }
            sb.AppendLine();
            specialSections.Add(sb.ToString());
        }

        return new BackupMaterial(reportId, reportPath, reportCreatedAt, entries, specialSections);
    }

    private List<BackupEntry> CopyRegularFiles(
        IReadOnlyList<DiscoveredItem> items,
        string finalDir,
        IProgress<string>? progress,
        CancellationToken ct)
    {
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

        return entries;
    }

    private string WriteRegistryBackup(
        string finalDir,
        IReadOnlyList<RegistryDiscoveredItem> items,
        IProgress<string>? progress)
    {
        string registryFolder = Path.Combine(finalDir, "Registry Backup");
        Directory.CreateDirectory(registryFolder);

        string stamp = DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        string regPath = Path.Combine(registryFolder, $"WinBaas-Registry-{stamp}.reg");
        string psPath = Path.Combine(registryFolder, "restore.ps1");

        File.WriteAllText(regPath, BuildRegFile(items), Encoding.Unicode);
        File.WriteAllText(psPath, BuildPowerShellRestoreScript(items), Encoding.UTF8);
        progress?.Report($"Wrote registry backup to {registryFolder}");
        return registryFolder;
    }

    private string BackupVisualStudioSkus(
        string finalDir,
        IReadOnlyList<VsSku> skus,
        IProgress<string>? progress,
        CancellationToken ct)
    {
        string visualStudioRoot = Path.Combine(finalDir, "Visual Studio");
        Directory.CreateDirectory(visualStudioRoot);

        foreach (VsSku sku in skus)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            string skuRoot = Path.Combine(visualStudioRoot, SafeName(sku.NodeLabel));
            string hivesRoot = Path.Combine(skuRoot, "Hives");
            string extensionsRoot = Path.Combine(skuRoot, "Extensions");
            Directory.CreateDirectory(hivesRoot);
            Directory.CreateDirectory(extensionsRoot);

            foreach (VsHive hive in sku.Hives)
            {
                if (ct.IsCancellationRequested || !Directory.Exists(hive.FullPath))
                {
                    continue;
                }

                string target = Path.Combine(hivesRoot, SafeName(hive.Name));
                CopyDirectory(hive.FullPath, target, excludeVisualStudioCaches: true);
                progress?.Report($"Copied {hive.FullPath}");
            }

            foreach (VsExtension extension in sku.Extensions)
            {
                if (ct.IsCancellationRequested || !Directory.Exists(extension.InstallPath))
                {
                    continue;
                }

                string folderName = string.IsNullOrWhiteSpace(extension.FolderName)
                    ? SafeName(extension.Name)
                    : SafeName(extension.FolderName);
                string target = Path.Combine(extensionsRoot, folderName);
                CopyDirectory(extension.InstallPath, target, excludeVisualStudioCaches: false);
            }

            File.WriteAllText(Path.Combine(skuRoot, "summary.md"), BuildVisualStudioSummary(sku), Encoding.UTF8);
        }

        return visualStudioRoot;
    }

    private void CopyDirectory(string sourceDirectory, string targetDirectory, bool excludeVisualStudioCaches)
    {
        Directory.CreateDirectory(targetDirectory);

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string name = Path.GetFileName(directory);
            if (excludeVisualStudioCaches
                && s_visualStudioCacheDirectories.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            CopyDirectory(directory, Path.Combine(targetDirectory, name), excludeVisualStudioCaches);
        }

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string name = Path.GetFileName(file);
            if (excludeVisualStudioCaches
                && name.Equals("ShellPackages.pkgdef", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            File.Copy(file, Path.Combine(targetDirectory, name), overwrite: true);
        }
    }

    private static string BuildRegFile(IReadOnlyList<RegistryDiscoveredItem> items)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Windows Registry Editor Version 5.00");
        builder.AppendLine();

        foreach (IGrouping<string, RegistryDiscoveredItem> group in items
                     .Where(item => item.IsChecked && item.IsPresent)
                     .GroupBy(item => item.Descriptor.RegFileKeyPath, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append('[').Append(group.Key).AppendLine("]");
            foreach (RegistryDiscoveredItem item in group)
            {
                builder.Append(item.Descriptor.RegFileValueToken)
                    .Append('=')
                    .AppendLine(FormatRegValue(item.Descriptor.ValueKind, item.Value));
            }
            builder.AppendLine();
        }

        foreach (RegistryDiscoveredItem item in items.Where(item => !item.IsPresent))
        {
            builder.Append("; missing: ").Append(item.Descriptor.DisplayPath).AppendLine();
        }

        return builder.ToString();
    }

    private static string BuildPowerShellRestoreScript(IReadOnlyList<RegistryDiscoveredItem> items)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Add-Type -AssemblyName Microsoft.Win32.Registry");
        builder.AppendLine();
        builder.AppendLine("function Set-WinBaasRegistryValue {");
        builder.AppendLine("    param(");
        builder.AppendLine("        [string]$Path,");
        builder.AppendLine("        [string]$Name,");
        builder.AppendLine("        [Microsoft.Win32.RegistryValueKind]$Kind,");
        builder.AppendLine("        [object]$Value");
        builder.AppendLine("    )");
        builder.AppendLine("    New-Item -Path $Path -Force | Out-Null");
        builder.AppendLine("    if ([string]::IsNullOrEmpty($Name)) {");
        builder.AppendLine("        if ($Kind -eq [Microsoft.Win32.RegistryValueKind]::String) {");
        builder.AppendLine("            New-Item -Path $Path -Force -Value ([string]$Value) | Out-Null");
        builder.AppendLine("        }");
        builder.AppendLine("        else {");
        builder.AppendLine("            New-ItemProperty -Path $Path -Name '' -PropertyType $Kind -Value $Value -Force | Out-Null");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("    else {");
        builder.AppendLine("        Set-ItemProperty -Path $Path -Name $Name -Type $Kind -Value $Value -Force");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();

        foreach (RegistryDiscoveredItem item in items)
        {
            if (item.IsChecked && item.IsPresent)
            {
                builder.Append("Set-WinBaasRegistryValue -Path '")
                    .Append(item.Descriptor.ProviderPath.Replace("'", "''", StringComparison.Ordinal))
                    .Append("' -Name '")
                    .Append(item.Descriptor.ValueName.Replace("'", "''", StringComparison.Ordinal))
                    .Append("' -Kind ([Microsoft.Win32.RegistryValueKind]::")
                    .Append(item.Descriptor.ValueKind)
                    .Append(") -Value ")
                    .AppendLine(FormatPowerShellLiteral(item.Value));
            }
            else if (!item.IsPresent)
            {
                builder.Append("# missing: ").Append(item.Descriptor.DisplayPath).AppendLine();
            }
        }

        return builder.ToString();
    }

    private static string BuildVisualStudioSummary(VsSku sku)
    {
        var builder = new StringBuilder();
        builder.Append("# ").AppendLine(sku.NodeLabel);
        builder.AppendLine();
        builder.Append("- Display name: ").AppendLine(sku.DisplayName);
        builder.Append("- Version: ").AppendLine(string.IsNullOrWhiteSpace(sku.Version) ? "(unknown)" : sku.Version);
        builder.Append("- Install date: ").AppendLine(sku.InstallDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "(unknown)");
        builder.Append("- Settings path: `").Append(sku.SettingsPath).AppendLine("`");
        builder.AppendLine();

        builder.AppendLine("## Hives");
        builder.AppendLine();
        foreach (VsHive hive in sku.Hives)
        {
            builder.Append("- `").Append(hive.Name).Append("` — `").Append(hive.FullPath).AppendLine("`");
        }

        builder.AppendLine();
        builder.AppendLine("## Extensions");
        builder.AppendLine();
        foreach (VsExtension extension in sku.Extensions)
        {
            builder.Append("- `").Append(extension.Name).Append("`");
            if (!string.IsNullOrWhiteSpace(extension.Version))
            {
                builder.Append(" — ").Append(extension.Version);
            }

            builder.Append(" — `").Append(extension.InstallPath).AppendLine("`");
        }

        return builder.ToString();
    }

    private static string BuildReport(
        Guid reportId,
        DateTime reportCreatedAt,
        BackupMode mode,
        string finalDestination,
        IReadOnlyList<BackupEntry> entries,
        IReadOnlyList<string> specialSections)
    {
        var copied = entries.Where(entry => entry.Copied).ToList();
        int totalFiles = copied.Count;
        var byCategory = copied
            .GroupBy(entry => string.IsNullOrEmpty(entry.Item.Source.Category) ? "(Uncategorized)" : entry.Item.Source.Category, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();
        int disciplines = byCategory.Count;
        int folderCount = copied
            .Select(entry => Path.GetDirectoryName(entry.Item.FullPath) ?? string.Empty)
            .Where(directory => !string.IsNullOrEmpty(directory))
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
        sb.Append("Found ").Append(totalFiles).Append(" copied file(s) from ")
          .Append(disciplines).Append(" disciplines and ")
          .Append(folderCount).AppendLine(" folders.");
        sb.AppendLine();

        foreach (string section in specialSections)
        {
            sb.Append(section);
        }

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
        }

        return sb.ToString();
    }

    private static (int FileCount, int FolderCount, BackupEntry? Newest, BackupEntry? Oldest, BackupEntry? Smallest, BackupEntry? Biggest)
        ComputeStats(IGrouping<string, BackupEntry> category)
    {
        var list = category.ToList();
        int fileCount = list.Count;
        int folderCount = list
            .Select(entry => Path.GetDirectoryName(entry.Item.FullPath) ?? string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        BackupEntry? newest = list
            .Where(entry => entry.Item.LastChanged.HasValue)
            .OrderByDescending(entry => entry.Item.LastChanged!.Value)
            .FirstOrDefault();
        BackupEntry? oldest = list
            .Where(entry => entry.Item.LastChanged.HasValue)
            .OrderBy(entry => entry.Item.LastChanged!.Value)
            .FirstOrDefault();
        BackupEntry? smallest = list
            .Where(entry => entry.Item.SizeBytes.HasValue)
            .OrderBy(entry => entry.Item.SizeBytes!.Value)
            .FirstOrDefault();
        BackupEntry? biggest = list
            .Where(entry => entry.Item.SizeBytes.HasValue)
            .OrderByDescending(entry => entry.Item.SizeBytes!.Value)
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

    private static string FormatPowerShellLiteral(object? value) => value switch
    {
        null => "$null",
        string text => $"'{text.Replace("'", "''", StringComparison.Ordinal)}'",
        string[] values => "@(" + string.Join(", ", values.Select(FormatPowerShellLiteral)) + ")",
        byte[] bytes => "@(" + string.Join(", ", bytes.Select(b => b.ToString(CultureInfo.InvariantCulture))) + ")",
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "$null",
    };

    private static string FormatRegValue(RegistryValueKind kind, object? value)
    {
        return kind switch
        {
            RegistryValueKind.DWord => $"dword:{Convert.ToUInt32(value ?? 0, CultureInfo.InvariantCulture):x8}",
            RegistryValueKind.QWord => "hex(b):" + JoinHex(BitConverter.GetBytes(Convert.ToUInt64(value ?? 0, CultureInfo.InvariantCulture))),
            RegistryValueKind.MultiString => "hex(7):" + JoinHex(Encoding.Unicode.GetBytes(string.Join("\0", (string[]?)value ?? []) + "\0\0")),
            RegistryValueKind.ExpandString => "hex(2):" + JoinHex(Encoding.Unicode.GetBytes(Convert.ToString(value, CultureInfo.InvariantCulture) + "\0")),
            RegistryValueKind.Binary => "hex:" + JoinHex((byte[]?)value ?? []),
            _ => $"\"{(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty).Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"",
        };
    }

    private static string JoinHex(IEnumerable<byte> bytes)
        => string.Join(",", bytes.Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));

    private sealed record BackupEntry(
        DiscoveredItem Item,
        string TargetPath,
        bool Copied,
        bool Skipped,
        string? Reason);

    private sealed record BackupMaterial(
        Guid ReportId,
        string ReportPath,
        DateTime ReportCreatedAt,
        IReadOnlyList<BackupEntry> Entries,
        IReadOnlyList<string> SpecialSections);
}
