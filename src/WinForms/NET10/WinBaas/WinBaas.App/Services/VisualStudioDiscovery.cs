using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IVisualStudioDiscovery"/>
public sealed class VisualStudioDiscovery(ILogger<VisualStudioDiscovery> logger) : IVisualStudioDiscovery
{
    private readonly ILogger<VisualStudioDiscovery> _logger = logger;

    /// <inheritdoc />
    public Task<IReadOnlyList<VsSku>> DiscoverAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => DiscoverCore(cancellationToken), cancellationToken);

    private IReadOnlyList<VsSku> DiscoverCore(CancellationToken cancellationToken)
    {
        string visualStudioRoot = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Microsoft\VisualStudio");
        var hiveGroups = DiscoverHiveGroups(visualStudioRoot);
        List<VsWhereInstance> vsWhereInstances = DiscoverViaVsWhere();

        var skus = new List<VsSku>();
        var matchedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (VsWhereInstance instance in vsWhereInstances)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            HiveGroup? match = FindBestHiveGroup(instance, hiveGroups, matchedGroups);
            if (match is not null)
            {
                matchedGroups.Add(match.BaseHiveName);
            }

            skus.Add(CreateSku(instance, match));
        }

        foreach (HiveGroup group in hiveGroups.Where(group => !matchedGroups.Contains(group.BaseHiveName)))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            skus.Add(CreateFallbackSku(group));
        }

        return skus
            .OrderBy(sku => sku.Year, StringComparer.OrdinalIgnoreCase)
            .ThenBy(sku => sku.Edition, StringComparer.OrdinalIgnoreCase)
            .ThenBy(sku => sku.Ring, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private List<VsWhereInstance> DiscoverViaVsWhere()
    {
        string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        string vsWherePath = Path.Combine(programFilesX86, "Microsoft Visual Studio", "Installer", "vswhere.exe");
        if (!File.Exists(vsWherePath))
        {
            return [];
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = vsWherePath,
                Arguments = "-prerelease -all -format json",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process? process = Process.Start(startInfo);
            if (process is null)
            {
                return [];
            }

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(5000);
            if (string.IsNullOrWhiteSpace(output))
            {
                return [];
            }

            using JsonDocument document = JsonDocument.Parse(output);
            var instances = new List<VsWhereInstance>();
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                string displayName = GetString(element, "displayName");
                string version = GetString(element, "installationVersion");
                string productLineVersion = GetString(element, "productLineVersion");
                string channelId = GetString(element, "channelId");
                string productId = GetString(element, "productId");
                string installationPath = GetString(element, "installationPath");
                DateTime? installDate = DateTime.TryParse(
                    GetString(element, "installDate"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out DateTime parsed)
                    ? parsed
                    : null;

                instances.Add(new VsWhereInstance(
                    DisplayName: displayName,
                    Version: version,
                    Year: NormalizeYear(productLineVersion, version),
                    Edition: NormalizeEdition(displayName, productId),
                    Ring: NormalizeRing(channelId, displayName),
                    ChannelId: channelId,
                    InstallationPath: installationPath,
                    InstallDate: installDate));
            }

            return instances;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Visual Studio discovery via vswhere failed.");
            return [];
        }
    }

    private List<HiveGroup> DiscoverHiveGroups(string visualStudioRoot)
    {
        if (!Directory.Exists(visualStudioRoot))
        {
            return [];
        }

        var groups = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (string directory in Directory.EnumerateDirectories(visualStudioRoot))
            {
                string name = Path.GetFileName(directory);
                if (!IsVisualStudioHiveName(name))
                {
                    continue;
                }

                string baseName = name.EndsWith("Exp", StringComparison.OrdinalIgnoreCase)
                    ? name[..^3]
                    : name;

                if (!groups.TryGetValue(baseName, out List<string>? list))
                {
                    list = [];
                    groups.Add(baseName, list);
                }

                list.Add(directory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Could not enumerate Visual Studio hive folders below {Root}.", visualStudioRoot);
        }

        return groups
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => CreateHiveGroup(pair.Key, pair.Value))
            .ToList();
    }

    private HiveGroup CreateHiveGroup(string baseHiveName, IReadOnlyList<string> directories)
    {
        string? baseHivePath = directories
            .FirstOrDefault(path => !Path.GetFileName(path).EndsWith("Exp", StringComparison.OrdinalIgnoreCase))
            ?? directories.FirstOrDefault();

        string channelId = string.Empty;
        string edition = "(unknown edition)";
        string ring = "(unknown ring)";
        if (!string.IsNullOrEmpty(baseHivePath))
        {
            TryReadHiveMetadata(baseHivePath, ref channelId, ref edition, ref ring);
        }

        var hives = directories
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path => new VsHive
            {
                Name = Path.GetFileName(path),
                FullPath = path,
            })
            .ToList();

        string settingsPath = string.Empty;
        if (!string.IsNullOrEmpty(baseHivePath))
        {
            string currentSettings = Path.Combine(baseHivePath, "CurrentSettings.vssettings");
            settingsPath = File.Exists(currentSettings) ? currentSettings : baseHivePath;
        }

        IReadOnlyList<VsExtension> extensions = DiscoverExtensions(hives);
        string major = baseHiveName.Split(['.', '_'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

        return new HiveGroup(
            BaseHiveName: baseHiveName,
            BaseHivePath: baseHivePath ?? string.Empty,
            Year: NormalizeYear(string.Empty, major),
            Edition: edition,
            Ring: ring,
            ChannelId: channelId,
            SettingsPath: settingsPath,
            Hives: hives,
            Extensions: extensions);
    }

    private void TryReadHiveMetadata(string hivePath, ref string channelId, ref string edition, ref string ring)
    {
        string settingsPath = Path.Combine(hivePath, "Settings", "ApplicationPrivateSettings.xml");
        if (!File.Exists(settingsPath))
        {
            return;
        }

        try
        {
            XDocument document = XDocument.Load(settingsPath);
            string content = string.Concat(document.DescendantNodes().OfType<XText>().Select(text => text.Value));
            if (string.IsNullOrWhiteSpace(channelId))
            {
                channelId = document.Descendants()
                    .Attributes("Name")
                    .Where(attribute => attribute.Value.Contains("ChannelId", StringComparison.OrdinalIgnoreCase))
                    .Select(attribute => attribute.Parent?.Value)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                    ?? ExtractKnownValue(content, "ChannelId");
            }

            edition = NormalizeEdition(content, content);
            ring = NormalizeRing(channelId, content);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Could not parse {SettingsPath}.", settingsPath);
        }
    }

    private IReadOnlyList<VsExtension> DiscoverExtensions(IReadOnlyList<VsHive> hives)
    {
        var extensions = new Dictionary<string, VsExtension>(StringComparer.OrdinalIgnoreCase);
        foreach (VsHive hive in hives)
        {
            string extensionRoot = Path.Combine(hive.FullPath, "Extensions");
            if (!Directory.Exists(extensionRoot))
            {
                continue;
            }

            string[] directories;
            try
            {
                directories = Directory.GetDirectories(extensionRoot);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Could not enumerate Visual Studio extensions below {Path}.", extensionRoot);
                continue;
            }

            foreach (string directory in directories)
            {
                if (extensions.ContainsKey(directory))
                {
                    continue;
                }

                extensions.Add(directory, TryReadExtension(directory));
            }
        }

        return extensions.Values
            .OrderBy(extension => extension.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private VsExtension TryReadExtension(string directory)
    {
        string name = Path.GetFileName(directory);
        string publisher = string.Empty;
        string version = string.Empty;

        try
        {
            string? manifest = Directory.EnumerateFiles(directory, "*.vsixmanifest", SearchOption.AllDirectories).FirstOrDefault();
            if (!string.IsNullOrEmpty(manifest))
            {
                XDocument document = XDocument.Load(manifest);
                XElement? identity = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "Identity");
                XElement? displayName = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "DisplayName");
                if (!string.IsNullOrWhiteSpace(displayName?.Value))
                {
                    name = displayName.Value.Trim();
                }

                publisher = identity?.Attribute("Publisher")?.Value ?? string.Empty;
                version = identity?.Attribute("Version")?.Value ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Could not read Visual Studio extension manifest below {Path}.", directory);
        }

        return new VsExtension
        {
            Name = name,
            Publisher = publisher,
            Version = version,
            InstallPath = directory,
        };
    }

    private static HiveGroup? FindBestHiveGroup(
        VsWhereInstance instance,
        IReadOnlyList<HiveGroup> groups,
        IReadOnlySet<string> matchedGroups)
        => groups
            .Where(group => !matchedGroups.Contains(group.BaseHiveName))
            .Where(group => string.Equals(group.Year, instance.Year, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(group => string.Equals(group.Ring, instance.Ring, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(group => string.Equals(group.Edition, instance.Edition, StringComparison.OrdinalIgnoreCase))
            .ThenBy(group => group.BaseHiveName, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

    private static VsSku CreateSku(VsWhereInstance instance, HiveGroup? group)
        => new()
        {
            DisplayName = instance.DisplayName,
            Year = instance.Year,
            Edition = instance.Edition,
            Ring = instance.Ring,
            Version = instance.Version,
            InstallDate = instance.InstallDate,
            InstallationPath = instance.InstallationPath,
            SettingsPath = group?.SettingsPath ?? instance.InstallationPath,
            HiveRootPath = group?.BaseHivePath ?? string.Empty,
            Hives = group?.Hives ?? [],
            Extensions = group?.Extensions ?? [],
        };

    private static VsSku CreateFallbackSku(HiveGroup group)
        => new()
        {
            DisplayName = $"Visual Studio {group.Year} {group.Edition}".Trim(),
            Year = group.Year,
            Edition = group.Edition,
            Ring = group.Ring,
            SettingsPath = group.SettingsPath,
            HiveRootPath = group.BaseHivePath,
            Hives = group.Hives,
            Extensions = group.Extensions,
        };

    private static bool IsVisualStudioHiveName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return name.StartsWith("17.0", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("18.0", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetString(JsonElement element, string name)
        => element.TryGetProperty(name, out JsonElement value) ? value.GetString() ?? string.Empty : string.Empty;

    private static string NormalizeYear(string productLineVersion, string version)
    {
        if (!string.IsNullOrWhiteSpace(productLineVersion))
        {
            return productLineVersion.Trim();
        }

        string major = version.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? version;
        return major switch
        {
            "17" => "2022",
            "18" => "2026",
            _ => major,
        };
    }

    private static string NormalizeEdition(string displayName, string fallback)
    {
        string haystack = $"{displayName} {fallback}";
        if (haystack.Contains("Enterprise", StringComparison.OrdinalIgnoreCase))
        {
            return "Enterprise";
        }

        if (haystack.Contains("Professional", StringComparison.OrdinalIgnoreCase))
        {
            return "Professional";
        }

        if (haystack.Contains("Community", StringComparison.OrdinalIgnoreCase))
        {
            return "Community";
        }

        return "(unknown edition)";
    }

    private static string NormalizeRing(string channelId, string fallback)
    {
        string haystack = $"{channelId} {fallback}";
        if (haystack.Contains("IntPreview", StringComparison.OrdinalIgnoreCase)
            || haystack.Contains("Internal Preview", StringComparison.OrdinalIgnoreCase))
        {
            return "Int.Preview";
        }

        if (haystack.Contains("Preview", StringComparison.OrdinalIgnoreCase))
        {
            return "Preview";
        }

        if (haystack.Contains("Main", StringComparison.OrdinalIgnoreCase))
        {
            return "Main";
        }

        if (haystack.Contains("Release", StringComparison.OrdinalIgnoreCase))
        {
            return "Release";
        }

        return "(unknown ring)";
    }

    private static string ExtractKnownValue(string content, string label)
    {
        int index = content.IndexOf(label, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return string.Empty;
        }

        string remainder = content[index..];
        string[] tokens = remainder.Split(['"', '\'', '<', '>', '=', ' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries);
        return tokens.Skip(1).FirstOrDefault() ?? string.Empty;
    }

    private sealed record VsWhereInstance(
        string DisplayName,
        string Version,
        string Year,
        string Edition,
        string Ring,
        string ChannelId,
        string InstallationPath,
        DateTime? InstallDate);

    private sealed record HiveGroup(
        string BaseHiveName,
        string BaseHivePath,
        string Year,
        string Edition,
        string Ring,
        string ChannelId,
        string SettingsPath,
        IReadOnlyList<VsHive> Hives,
        IReadOnlyList<VsExtension> Extensions);
}
