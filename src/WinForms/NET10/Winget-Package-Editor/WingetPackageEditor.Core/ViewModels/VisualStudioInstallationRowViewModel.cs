using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;

namespace WingetPackageEditor.Core.ViewModels;

/// <summary>
///  A single Visual Studio installation/hive row displayed in the overview grid. A main
///  installation produces one row; each experimental hive produces an additional row.
/// </summary>
public sealed class VisualStudioInstallationRowViewModel
{
    public VisualStudioInstallationRowViewModel(
        VisualStudioInstanceInfo instance,
        string hiveName,
        string dataPath,
        bool isExperimental)
    {
        ArgumentNullException.ThrowIfNull(instance);
        Instance = instance;
        HiveName = hiveName;
        DataPath = dataPath ?? string.Empty;
        IsExperimental = isExperimental;
    }

    public VisualStudioInstanceInfo Instance { get; }

    public string SkuName => Instance.DisplayName;

    public string Version => Instance.Version;

    public DateTimeOffset? InstallDate => Instance.InstallDate;

    public string InstallDateDisplay => InstallDate is { } date
        ? date.LocalDateTime.ToString("yyyy-MM-dd")
        : string.Empty;

    public string InstanceId => Instance.InstanceId;

    public string InstallationPath => Instance.InstallationPath;

    public string DataPath { get; }

    public string HiveName { get; }

    public bool IsExperimental { get; }

    public string InstallationPathDisplay => PathShortener.Shorten(InstallationPath);

    public string DataPathDisplay => string.IsNullOrEmpty(DataPath)
        ? "(not created)"
        : PathShortener.Shorten(DataPath);

    /// <summary>
    ///  Builds the rows for a single instance: one main row plus one per experimental hive.
    /// </summary>
    public static IEnumerable<VisualStudioInstallationRowViewModel> CreateRows(VisualStudioInstanceInfo instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        VisualStudioHiveInfo? main = instance.Hives.FirstOrDefault(hive => !hive.IsExperimental);
        string mainHiveName = main?.Name ?? $"{instance.ShortVersion}_{instance.InstanceId}";
        yield return new VisualStudioInstallationRowViewModel(
            instance,
            mainHiveName,
            main?.Path ?? string.Empty,
            isExperimental: false);

        foreach (VisualStudioHiveInfo hive in instance.Hives.Where(hive => hive.IsExperimental))
        {
            yield return new VisualStudioInstallationRowViewModel(
                instance,
                hive.Name,
                hive.Path,
                isExperimental: true);
        }
    }
}
