using System.Collections.ObjectModel;
using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.ViewModels;

/// <summary>
///  A concrete Visual Studio installation node (leaf of the navigation tree).
/// </summary>
public sealed class VisualStudioInstanceViewModel
{
    public VisualStudioInstanceViewModel(VisualStudioInstanceInfo model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Rows = [.. VisualStudioInstallationRowViewModel.CreateRows(model)];
    }

    public VisualStudioInstanceInfo Model { get; }

    public IReadOnlyList<VisualStudioInstallationRowViewModel> Rows { get; }

    public string Id => Model.InstanceId;

    public string TreeText => string.IsNullOrWhiteSpace(Model.Version)
        ? Model.InstanceId
        : $"{Model.Version} ({Model.InstanceId})";
}

/// <summary>
///  A Channel-Edition grouping node (e.g. <c>Preview-Enterprise</c>).
/// </summary>
public sealed class VisualStudioSkuComboViewModel
{
    public VisualStudioSkuComboViewModel(string comboLabel, IReadOnlyList<VisualStudioInstanceInfo> instances)
    {
        ComboLabel = comboLabel ?? throw new ArgumentNullException(nameof(comboLabel));
        ArgumentNullException.ThrowIfNull(instances);

        foreach (VisualStudioInstanceInfo instance in instances)
        {
            Instances.Add(new VisualStudioInstanceViewModel(instance));
        }

        Rows = [.. Instances.SelectMany(instance => instance.Rows)];
    }

    public string ComboLabel { get; }

    public string TreeText => ComboLabel;

    public ObservableCollection<VisualStudioInstanceViewModel> Instances { get; } = [];

    public IReadOnlyList<VisualStudioInstallationRowViewModel> Rows { get; }
}

/// <summary>
///  A Visual Studio version grouping node (2019/2022/2026).
/// </summary>
public sealed class VisualStudioVersionViewModel
{
    public VisualStudioVersionViewModel(string year, IReadOnlyList<VisualStudioInstanceInfo> instances)
    {
        Year = year ?? throw new ArgumentNullException(nameof(year));
        ArgumentNullException.ThrowIfNull(instances);

        foreach (IGrouping<string, VisualStudioInstanceInfo> combo in instances
                     .GroupBy(instance => instance.SkuComboLabel, StringComparer.OrdinalIgnoreCase)
                     .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            SkuCombos.Add(new VisualStudioSkuComboViewModel(combo.Key, [.. combo]));
        }

        Rows = [.. SkuCombos.SelectMany(combo => combo.Rows)];
    }

    public string Year { get; }

    public string TreeText => Year;

    public ObservableCollection<VisualStudioSkuComboViewModel> SkuCombos { get; } = [];

    public IReadOnlyList<VisualStudioInstallationRowViewModel> Rows { get; }
}

/// <summary>
///  The root "Visual Studio" node nested under each winget package. Holds the discovered
///  installation tree (Version -&gt; SKU-combo -&gt; instance) and a flattened overview.
/// </summary>
public sealed class VisualStudioBranchViewModel
{
    public VisualStudioBranchViewModel(IReadOnlyList<VisualStudioInstanceInfo> instances)
    {
        ArgumentNullException.ThrowIfNull(instances);

        foreach (IGrouping<string, VisualStudioInstanceInfo> versionGroup in instances
                     .GroupBy(instance => instance.Year, StringComparer.OrdinalIgnoreCase)
                     .OrderByDescending(group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            Versions.Add(new VisualStudioVersionViewModel(versionGroup.Key, [.. versionGroup]));
        }

        Rows = [.. Versions.SelectMany(version => version.Rows)];
    }

    public string TreeText => "Visual Studio";

    public ObservableCollection<VisualStudioVersionViewModel> Versions { get; } = [];

    public IReadOnlyList<VisualStudioInstallationRowViewModel> Rows { get; }
}
