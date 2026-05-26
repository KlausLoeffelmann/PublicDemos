using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IDiscoveryService"/>
public sealed class DiscoveryService(
    IFileTypeMap fileTypeMap,
    ILogger<DiscoveryService> logger) : IDiscoveryService
{
    private readonly IFileTypeMap _fileTypeMap = fileTypeMap;
    private readonly ILogger<DiscoveryService> _logger = logger;

    /// <inheritdoc />
    public Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(
        CatalogEntry entry,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return entry.Kind switch
        {
            CatalogEntryKind.Folder => Task.Run(() => DiscoverFolder(entry, cancellationToken), cancellationToken),
            CatalogEntryKind.File => Task.Run(() => DiscoverFile(entry), cancellationToken),
            CatalogEntryKind.EnvironmentVariable => Task.Run(() => DiscoverEnvironmentVariables(entry), cancellationToken),
            CatalogEntryKind.SqlServer => Task.Run(() => DiscoverSqlServer(entry, cancellationToken), cancellationToken),
            _ => Task.FromResult<IReadOnlyList<DiscoveredItem>>([]),
        };
    }

    private IReadOnlyList<DiscoveredItem> DiscoverFolder(CatalogEntry entry, CancellationToken ct)
    {
        var results = new List<DiscoveredItem>();
        if (string.IsNullOrWhiteSpace(entry.Path) || !Directory.Exists(entry.Path))
        {
            _logger.LogDebug("Skipping folder source {Path} (does not exist).", entry.Path);
            return results;
        }

        SearchOption opt = entry.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        foreach (string ext in entry.Extensions)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            try
            {
                IEnumerable<string> matches = Directory.EnumerateFiles(entry.Path, "*" + ext, opt);
                foreach (string file in matches)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        var fi = new FileInfo(file);
                        results.Add(new DiscoveredItem
                        {
                            Source = entry,
                            Name = fi.Name,
                            FullPath = fi.FullName,
                            FileTypeLabel = _fileTypeMap.GetLabel(fi.Name),
                            LastChanged = fi.LastWriteTime,
                            Created = fi.CreationTime,
                            SizeBytes = fi.Length,
                            IsFolder = false,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Skipping inaccessible file {File}.", file);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not enumerate {Path} for extension {Ext}.", entry.Path, ext);
            }
        }

        return results;
    }

    private IReadOnlyList<DiscoveredItem> DiscoverFile(CatalogEntry entry)
    {
        if (!File.Exists(entry.Path))
        {
            return [];
        }

        try
        {
            var fi = new FileInfo(entry.Path);
            return
            [
                new DiscoveredItem
                {
                    Source = entry,
                    Name = fi.Name,
                    FullPath = fi.FullName,
                    FileTypeLabel = _fileTypeMap.GetLabel(fi.Name),
                    LastChanged = fi.LastWriteTime,
                    Created = fi.CreationTime,
                    SizeBytes = fi.Length,
                    IsFolder = false,
                }
            ];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not stat single-file entry {Path}.", entry.Path);
            return [];
        }
    }

    private IReadOnlyList<DiscoveredItem> DiscoverEnvironmentVariables(CatalogEntry entry)
    {
        var results = new List<DiscoveredItem>();

        EnvironmentVariableTarget target = entry.Path.Equals("Machine", StringComparison.OrdinalIgnoreCase)
            ? EnvironmentVariableTarget.Machine
            : EnvironmentVariableTarget.User;

        try
        {
            foreach (var kv in Environment.GetEnvironmentVariables(target).Cast<System.Collections.DictionaryEntry>())
            {
                string name = kv.Key?.ToString() ?? string.Empty;
                string value = kv.Value?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(name) || LooksLikePath(value))
                {
                    continue;
                }

                results.Add(new DiscoveredItem
                {
                    Source = entry,
                    Name = name,
                    FullPath = name,
                    FileTypeLabel = "Environment variable",
                    SizeBytes = value.Length,
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not enumerate {Target} environment variables.", target);
        }

        return results;
    }

    private static bool LooksLikePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        // PATH-style: multiple ; separated entries, or a single absolute path.
        if (value.Contains(';') && value.Contains(Path.DirectorySeparatorChar))
        {
            return true;
        }

        if (value.Length >= 3 && value[1] == ':' && (value[2] == '\\' || value[2] == '/'))
        {
            return true;
        }

        return false;
    }

    private IReadOnlyList<DiscoveredItem> DiscoverSqlServer(CatalogEntry entry, CancellationToken ct)
    {
        var results = new List<DiscoveredItem>();
        bool isLocalDb = entry.Path.Equals("LocalDB", StringComparison.OrdinalIgnoreCase);
        string[] instances = isLocalDb ? GetLocalDbInstances() : ["(local)\\SQLEXPRESS"];

        foreach (string instance in instances)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            string connectionString = isLocalDb
                ? $"Server=(LocalDB)\\{instance};Integrated Security=true;Connect Timeout=2"
                : $"Server={instance};Integrated Security=true;Connect Timeout=2";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT name FROM sys.databases WHERE database_id > 4";
                using var reader = cmd.ExecuteReader();
                while (reader.Read() && !ct.IsCancellationRequested)
                {
                    string db = reader.GetString(0);
                    results.Add(new DiscoveredItem
                    {
                        Source = entry,
                        Name = db,
                        FullPath = $"{instance}\\{db}",
                        FileTypeLabel = isLocalDb ? "LocalDB Database" : "SQL Express Database",
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Could not enumerate databases for {Instance}.", instance);
            }
        }

        return results;
    }

    private string[] GetLocalDbInstances()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "sqllocaldb.exe",
                Arguments = "info",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            using var process = Process.Start(psi);
            if (process is null)
            {
                return [];
            }

            string stdout = process.StandardOutput.ReadToEnd();
            process.WaitForExit(3000);
            return stdout
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "sqllocaldb.exe is not available; LocalDB discovery skipped.");
            return [];
        }
    }
}
