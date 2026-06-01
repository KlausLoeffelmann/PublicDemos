using System.Globalization;
using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Parses <c>vswhere.exe</c> output and maps it onto <see cref="VisualStudioInstanceInfo"/>
///  records, correlating Visual Studio data hives discovered on disk.
/// </summary>
public static class VisualStudioDiscoveryParser
{
    /// <summary>
    ///  Splits raw <c>vswhere</c> output into per-instance key/value blocks.
    /// </summary>
    /// <param name="output">The raw standard-output text emitted by <c>vswhere</c>.</param>
    /// <returns>One dictionary per discovered instance block.</returns>
    public static IReadOnlyList<IReadOnlyDictionary<string, string>> ParseBlocks(string? output)
    {
        List<IReadOnlyDictionary<string, string>> blocks = [];
        if (string.IsNullOrWhiteSpace(output))
        {
            return blocks;
        }

        Dictionary<string, string> current = new(StringComparer.OrdinalIgnoreCase);

        void FlushBlock()
        {
            if (current.ContainsKey("instanceId"))
            {
                blocks.Add(current);
            }

            current = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        foreach (string rawLine in output.Replace("\r\n", "\n").Split('\n'))
        {
            string line = rawLine.TrimEnd();
            if (line.Length == 0)
            {
                FlushBlock();
                continue;
            }

            int separator = line.IndexOf(':');
            if (separator <= 0)
            {
                // Header/banner lines (no "key: value" shape) are ignored.
                continue;
            }

            string key = line[..separator].Trim();
            string value = line[(separator + 1)..].Trim();
            if (key.Length == 0)
            {
                continue;
            }

            current[key] = value;
        }

        FlushBlock();
        return blocks;
    }

    /// <summary>
    ///  Maps a parsed <c>vswhere</c> block onto a <see cref="VisualStudioInstanceInfo"/>,
    ///  attaching any hive folders that belong to the instance.
    /// </summary>
    /// <param name="block">A parsed key/value block.</param>
    /// <param name="hiveFolders">All Visual Studio hive folders discovered on disk.</param>
    /// <returns>The mapped instance, or <see langword="null"/> when the block has no instance id.</returns>
    public static VisualStudioInstanceInfo? MapInstance(
        IReadOnlyDictionary<string, string> block,
        IReadOnlyList<VisualStudioHiveFolder> hiveFolders)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(hiveFolders);

        if (!block.TryGetValue("instanceId", out string? instanceId) || string.IsNullOrWhiteSpace(instanceId))
        {
            return null;
        }

        string version = GetValue(block, "installationVersion");
        string shortVersion = ToShortVersion(version);
        string productId = GetValue(block, "productId");
        string displayName = GetValue(block, "displayName");
        string edition = MapEdition(productId, displayName);
        string channelId = GetValue(block, "channelId");
        VisualStudioChannel channel = MapChannel(channelId);
        string year = MapYear(block, shortVersion);
        string installationPath = GetValue(block, "installationPath");
        bool isPrerelease = string.Equals(GetValue(block, "isPrerelease"), "1", StringComparison.Ordinal);
        DateTimeOffset? installDate = ParseDate(GetValue(block, "installDate"));

        IReadOnlyList<VisualStudioHiveInfo> hives = CorrelateHives(hiveFolders, shortVersion, instanceId);

        return new VisualStudioInstanceInfo(
            InstanceId: instanceId,
            DisplayName: string.IsNullOrWhiteSpace(displayName) ? $"Visual Studio {edition} {year}".Trim() : displayName,
            Year: year,
            Edition: edition,
            Channel: channel,
            ChannelId: channelId,
            Version: version,
            ShortVersion: shortVersion,
            InstallDate: installDate,
            InstallationPath: installationPath,
            ProductId: productId,
            IsPrerelease: isPrerelease,
            Hives: hives);
    }

    /// <summary>
    ///  Selects the hive folders that belong to the instance identified by
    ///  <paramref name="shortVersion"/> and <paramref name="instanceId"/>.
    /// </summary>
    public static IReadOnlyList<VisualStudioHiveInfo> CorrelateHives(
        IReadOnlyList<VisualStudioHiveFolder> hiveFolders,
        string shortVersion,
        string instanceId)
    {
        ArgumentNullException.ThrowIfNull(hiveFolders);

        string prefix = $"{shortVersion}_{instanceId}";
        List<VisualStudioHiveInfo> hives = [];

        foreach (VisualStudioHiveFolder folder in hiveFolders)
        {
            if (!folder.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string remainder = folder.Name[prefix.Length..];
            bool isExperimental = remainder.Equals("Exp", StringComparison.OrdinalIgnoreCase);

            // Accept the main hive (no suffix) and the experimental hive.
            if (remainder.Length != 0 && !isExperimental)
            {
                continue;
            }

            string settingsFilePath = System.IO.Path.Combine(folder.Path, "Settings", "CurrentSettings.vssettings");
            hives.Add(new VisualStudioHiveInfo(folder.Name, folder.Path, settingsFilePath, isExperimental));
        }

        return hives;
    }

    /// <summary>
    ///  Classifies a raw <c>channelId</c> into a <see cref="VisualStudioChannel"/>.
    /// </summary>
    public static VisualStudioChannel MapChannel(string? channelId)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            return VisualStudioChannel.Unknown;
        }

        string lowered = channelId.ToLowerInvariant();
        if (lowered.Contains("canary", StringComparison.Ordinal))
        {
            return VisualStudioChannel.Canary;
        }

        if (lowered.Contains("main", StringComparison.Ordinal))
        {
            return VisualStudioChannel.Main;
        }

        if (lowered.Contains("preview", StringComparison.Ordinal))
        {
            return VisualStudioChannel.Preview;
        }

        if (lowered.Contains("release", StringComparison.Ordinal))
        {
            return VisualStudioChannel.Release;
        }

        return VisualStudioChannel.Unknown;
    }

    /// <summary>
    ///  Derives the release year (2019/2022/2026) from the product line version.
    /// </summary>
    public static string MapYear(IReadOnlyDictionary<string, string> block, string shortVersion)
    {
        ArgumentNullException.ThrowIfNull(block);

        string lineVersion = GetValue(block, "catalog_productLineVersion");
        if (string.IsNullOrWhiteSpace(lineVersion))
        {
            lineVersion = shortVersion.Split('.').FirstOrDefault() ?? string.Empty;
        }

        return lineVersion switch
        {
            "16" => "2019",
            "17" => "2022",
            "18" => "2026",
            _ => string.IsNullOrWhiteSpace(lineVersion) ? "Unknown" : lineVersion
        };
    }

    /// <summary>
    ///  Derives the edition (Community/Professional/Enterprise/...) from the product id.
    /// </summary>
    public static string MapEdition(string? productId, string? displayName)
    {
        string? edition = productId?.Split('.').LastOrDefault();
        if (!string.IsNullOrWhiteSpace(edition))
        {
            return edition;
        }

        foreach (string candidate in new[] { "Enterprise", "Professional", "Community", "BuildTools" })
        {
            if (!string.IsNullOrEmpty(displayName)
                && displayName.Contains(candidate, StringComparison.OrdinalIgnoreCase))
            {
                return candidate;
            }
        }

        return "Unknown";
    }

    /// <summary>
    ///  Converts a full installation version (e.g. <c>18.7.11822.327</c>) into the short
    ///  <c>major.0</c> form used by Visual Studio hive folder names (e.g. <c>18.0</c>).
    /// </summary>
    public static string ToShortVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return string.Empty;
        }

        string major = version.Split('.').FirstOrDefault() ?? string.Empty;
        return major.Length == 0 ? string.Empty : $"{major}.0";
    }

    private static DateTimeOffset? ParseDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTimeOffset parsed)
            || DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed;
        }

        return null;
    }

    private static string GetValue(IReadOnlyDictionary<string, string> block, string key)
        => block.TryGetValue(key, out string? value) ? value : string.Empty;
}
