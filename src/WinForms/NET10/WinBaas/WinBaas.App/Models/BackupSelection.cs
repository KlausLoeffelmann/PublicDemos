namespace WinBaas.Models;

/// <summary>
///  The full set of objects selected or available for a single backup run.
/// </summary>
public sealed class BackupSelection
{
    /// <summary>Regular discovered file/folder/database items selected for backup.</summary>
    public IReadOnlyList<DiscoveredItem> FileItems { get; init; } = [];

    /// <summary>
    ///  All curated registry items discovered for the current machine. Checked
    ///  items are written as active restore entries; not-present ones are
    ///  emitted as comments.
    /// </summary>
    public IReadOnlyList<RegistryDiscoveredItem> RegistryItems { get; init; } = [];

    /// <summary>All discovered Visual Studio SKUs for the current machine.</summary>
    public IReadOnlyList<VsSku> VisualStudioSkus { get; init; } = [];

    /// <summary>Gets the user-visible count of selected backup groups/items.</summary>
    public int SelectedCount
        => FileItems.Count
         + RegistryItems.Count(item => item.IsChecked && item.IsPresent)
         + VisualStudioSkus.Count(sku => sku.IsChecked);

    /// <summary>Gets whether the selection contains anything to back up.</summary>
    public bool IsEmpty => SelectedCount == 0;
}
