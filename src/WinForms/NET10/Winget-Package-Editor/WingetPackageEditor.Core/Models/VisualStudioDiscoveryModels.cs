namespace WingetPackageEditor.Core.Models;

/// <summary>
///  Identifies the update channel a Visual Studio installation was acquired from.
/// </summary>
public enum VisualStudioChannel
{
    Unknown,
    Release,
    Preview,
    Canary,
    Main
}

/// <summary>
///  Describes a single Visual Studio data/registry hive folder under
///  <c>%LocalAppData%\Microsoft\VisualStudio</c>.
/// </summary>
public sealed record VisualStudioHiveInfo(
    string Name,
    string Path,
    string SettingsFilePath,
    bool IsExperimental);

/// <summary>
///  Represents a raw Visual Studio hive folder discovered on disk before correlation.
/// </summary>
public sealed record VisualStudioHiveFolder(string Name, string Path);

/// <summary>
///  Describes a concrete Visual Studio installation as reported by <c>vswhere.exe</c>,
///  together with the local data hives correlated to it.
/// </summary>
public sealed record VisualStudioInstanceInfo(
    string InstanceId,
    string DisplayName,
    string Year,
    string Edition,
    VisualStudioChannel Channel,
    string ChannelId,
    string Version,
    string ShortVersion,
    DateTimeOffset? InstallDate,
    string InstallationPath,
    string ProductId,
    bool IsPrerelease,
    IReadOnlyList<VisualStudioHiveInfo> Hives)
{
    /// <summary>
    ///  Gets a human-readable channel label, falling back to the raw channel id segment
    ///  when the channel could not be classified.
    /// </summary>
    public string ChannelLabel
    {
        get
        {
            if (Channel != VisualStudioChannel.Unknown)
            {
                return Channel.ToString();
            }

            string segment = ChannelId.Split('.').LastOrDefault() ?? string.Empty;
            return segment.Length > 0 ? segment : "Unknown";
        }
    }

    /// <summary>
    ///  Gets the "Channel-Edition" combination label used as a grouping node.
    /// </summary>
    public string SkuComboLabel => $"{ChannelLabel}-{Edition}";
}
