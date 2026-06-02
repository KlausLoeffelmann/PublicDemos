using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WarpToolkit.ComponentModel;
using WinBaas.Dialogs;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IDiscoveryService"/>
public sealed class DiscoveryService(
    IFileTypeMap fileTypeMap,
    IUserSettingsService settings,
    ILogger<DiscoveryService> logger) : IDiscoveryService
{
    private readonly IFileTypeMap _fileTypeMap = fileTypeMap;
    private readonly IUserSettingsService _settings = settings;
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
            CatalogEntryKind.SqlServer when _settings.Get(DlgOptions.KeySqlDiscovery, true)
                => Task.Run(() => DiscoverSqlServer(entry, cancellationToken), cancellationToken),
            _ => Task.FromResult<IReadOnlyList<DiscoveredItem>>([]),
        };
    }

    private IReadOnlyList<DiscoveredItem> DiscoverFolder(CatalogEntry entry, CancellationToken ct)
    {
        var results = new List<DiscoveredItem>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string rawPath in entry.Paths)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            foreach (string resolved in ExpandPath(rawPath))
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                if (File.Exists(resolved))
                {
                    AddFile(results, entry, resolved, seen);
                    continue;
                }

                if (!Directory.Exists(resolved))
                {
                    _logger.LogDebug("Skipping folder source {Path} (does not exist).", resolved);
                    continue;
                }

                EnumerateFolderInto(results, entry, resolved, seen, ct);
            }
        }

        return results;
    }

    private void EnumerateFolderInto(List<DiscoveredItem> results, CatalogEntry entry, string root, HashSet<string> seen, CancellationToken ct)
    {
        SearchOption opt = entry.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        // Match by extension globs (e.g. "*.json") and by explicit well-known
        // file names (e.g. "NuGet.Config", ".gitconfig"). When neither is
        // configured, fall back to every file.
        var patterns = new List<string>();
        patterns.AddRange(entry.Extensions.Select(e => "*" + e));
        patterns.AddRange(entry.KnownFileNames);
        if (patterns.Count == 0)
        {
            patterns.Add("*");
        }

        foreach (string pattern in patterns)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            try
            {
                foreach (string file in Directory.EnumerateFiles(root, pattern, opt))
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    AddFile(results, entry, file, seen);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not enumerate {Path} for pattern {Pattern}.", root, pattern);
            }
        }
    }

    private void AddFile(List<DiscoveredItem> results, CatalogEntry entry, string file, HashSet<string> seen)
    {
        try
        {
            var fi = new FileInfo(file);
            if (!seen.Add(fi.FullName))
            {
                return;
            }

            results.Add(new DiscoveredItem
            {
                Source = entry,
                Name = fi.Name,
                FullPath = fi.FullName,
                FileTypeLabel = _fileTypeMap.GetLabel(fi.Name, entry),
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

    /// <summary>
    ///  Expands a raw catalog path: substitutes env vars, the
    ///  <c>%DOCUMENTS%</c> token, and any <c>&lt;wildcard&gt;</c> segments
    ///  by enumerating the parent directory.
    /// </summary>
    private IEnumerable<string> ExpandPath(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            yield break;
        }

        string expanded = raw.Replace(
            "%DOCUMENTS%",
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            StringComparison.OrdinalIgnoreCase);
        expanded = Environment.ExpandEnvironmentVariables(expanded);

        if (!expanded.Contains('<'))
        {
            yield return expanded;
            yield break;
        }

        foreach (string match in ExpandWildcardSegments(expanded))
        {
            yield return match;
        }
    }

    private IEnumerable<string> ExpandWildcardSegments(string path)
    {
        string[] parts = path.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);

        bool isAbsolute = Path.IsPathRooted(path);
        var stack = new List<string> { isAbsolute ? Path.GetPathRoot(path)! : string.Empty };

        foreach (string part in parts)
        {
            if (part == Path.GetPathRoot(path)?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            {
                continue;
            }

            bool hasWildcard = part.Contains('<');
            var next = new List<string>(stack.Count);
            foreach (string current in stack)
            {
                if (!hasWildcard)
                {
                    next.Add(string.IsNullOrEmpty(current) ? part : Path.Combine(current, part));
                    continue;
                }

                string parent = string.IsNullOrEmpty(current) ? Directory.GetCurrentDirectory() : current;
                if (!Directory.Exists(parent))
                {
                    continue;
                }

                string mask = System.Text.RegularExpressions.Regex.Replace(part, "<[^>]+>", "*");
                try
                {
                    foreach (string sub in Directory.EnumerateDirectories(parent, mask))
                    {
                        next.Add(sub);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Wildcard expansion failed for {Mask} in {Parent}.", mask, parent);
                }
            }

            stack = next;
            if (stack.Count == 0)
            {
                break;
            }
        }

        return stack;
    }

    private IReadOnlyList<DiscoveredItem> DiscoverFile(CatalogEntry entry)
    {
        var results = new List<DiscoveredItem>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string raw in entry.Paths)
        {
            foreach (string resolved in ExpandPath(raw))
            {
                if (!File.Exists(resolved))
                {
                    continue;
                }

                AddFile(results, entry, resolved, seen);
            }
        }

        return results;
    }

    private IReadOnlyList<DiscoveredItem> DiscoverEnvironmentVariables(CatalogEntry entry)
    {
        var results = new List<DiscoveredItem>();
        string target = entry.Paths.Count > 0 ? entry.Paths[0] : "User";

        EnvironmentVariableTarget envTarget = target.Equals("Machine", StringComparison.OrdinalIgnoreCase)
            ? EnvironmentVariableTarget.Machine
            : EnvironmentVariableTarget.User;

        try
        {
            foreach (var kv in Environment.GetEnvironmentVariables(envTarget).Cast<System.Collections.DictionaryEntry>())
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
            _logger.LogWarning(ex, "Could not enumerate {Target} environment variables.", envTarget);
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
        string flavor = entry.Paths.Count > 0 ? entry.Paths[0] : "LocalDB";
        bool isLocalDb = flavor.Equals("LocalDB", StringComparison.OrdinalIgnoreCase);
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
