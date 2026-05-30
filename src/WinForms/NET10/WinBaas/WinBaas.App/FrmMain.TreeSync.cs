using WinBaas.Models;

namespace WinBaas;

/// <summary>
///  Tree/detail synchronization: the selected tree node swaps the right-hand
///  detail control; tree check states and detail-grid checkbox edits stay in sync.
/// </summary>
public sealed partial class FrmMain
{
    private void TreeSources_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        RefreshDetailFromSelectedNode();
        UpdateCommandStates();
    }

    private void TreeSources_AfterCheck(object? sender, TreeViewEventArgs e)
    {
        if (_syncing || e.Node is null)
        {
            return;
        }

        _syncing = true;
        try
        {
            bool target = e.Node.Checked;
            switch (e.Node.Tag)
            {
                case CategoryTag _:
                    foreach (TreeNode leaf in EnumerateCatalogLeafNodes(e.Node.Nodes))
                    {
                        leaf.Checked = target;
                        if (_nodeItems.TryGetValue(leaf, out List<DiscoveredItem>? items))
                        {
                            foreach (DiscoveredItem item in items)
                            {
                                item.IsChecked = target;
                            }
                        }
                    }

                    if (ReferenceEquals(e.Node, _treeSources.SelectedNode))
                    {
                        _filesGridControl.SetAllChecked(target);
                    }
                    break;

                case CatalogEntry _:
                    if (_nodeItems.TryGetValue(e.Node, out List<DiscoveredItem>? items))
                    {
                        foreach (DiscoveredItem item in items)
                        {
                            item.IsChecked = target;
                        }
                    }

                    if (ReferenceEquals(e.Node, _treeSources.SelectedNode))
                    {
                        _filesGridControl.SetAllChecked(target);
                    }

                    if (e.Node.Parent is { Tag: CategoryTag } parent)
                    {
                        parent.Checked = parent.Nodes.Cast<TreeNode>().All(node => node.Checked);
                    }
                    break;

                case RegistryGroupTag _:
                    foreach (RegistryDiscoveredItem item in _registryItems.Where(item => item.CanSelect))
                    {
                        item.IsChecked = target;
                    }

                    if (ReferenceEquals(e.Node, _treeSources.SelectedNode))
                    {
                        _registryGridControl.SetAllChecked(target);
                    }
                    break;

                case VsRootTag _:
                    foreach (TreeNode child in e.Node.Nodes.Cast<TreeNode>())
                    {
                        SetVisualStudioNodeChecked(child, target);
                    }
                    break;

                case VsSku sku:
                    sku.IsChecked = target;
                    foreach (TreeNode child in e.Node.Nodes.Cast<TreeNode>())
                    {
                        child.Checked = target;
                    }

                    if (e.Node.Parent is { Tag: VsRootTag } vsRoot)
                    {
                        vsRoot.Checked = vsRoot.Nodes.Cast<TreeNode>().All(node => node.Checked);
                    }
                    break;

                case VsHivesTag _:
                    if (e.Node.Parent is { Tag: VsSku skuNode })
                    {
                        skuNode.IsChecked = target;
                        e.Node.Parent.Checked = target;
                        foreach (TreeNode child in e.Node.Parent.Nodes.Cast<TreeNode>())
                        {
                            child.Checked = target;
                        }

                        if (e.Node.Parent.Parent is { Tag: VsRootTag } root)
                        {
                            root.Checked = root.Nodes.Cast<TreeNode>().All(node => node.Checked);
                        }
                    }

                    break;

                case VsExtensionsTag _:
                    if (e.Node.Parent is { Tag: VsSku parentSku })
                    {
                        parentSku.IsChecked = target;
                        e.Node.Parent.Checked = target;
                        foreach (TreeNode child in e.Node.Parent.Nodes.Cast<TreeNode>())
                        {
                            child.Checked = target;
                        }

                        if (e.Node.Parent.Parent is { Tag: VsRootTag } root)
                        {
                            root.Checked = root.Nodes.Cast<TreeNode>().All(node => node.Checked);
                        }
                    }

                    break;
            }
        }
        finally
        {
            _syncing = false;
        }

        UpdateCommandStates();
    }

    private void FilesGridControl_CheckedItemsChanged(object? sender, EventArgs e)
    {
        if (_syncing)
        {
            return;
        }

        _syncing = true;
        try
        {
            if (_treeSources.SelectedNode is { Tag: CatalogEntry } leaf)
            {
                RefreshCatalogLeafCheckState(leaf);
            }
            else if (_treeSources.SelectedNode is { Tag: CategoryTag } categoryNode)
            {
                foreach (TreeNode leaf in EnumerateCatalogLeafNodes(categoryNode.Nodes))
                {
                    RefreshCatalogLeafCheckState(leaf);
                }

                categoryNode.Checked = categoryNode.Nodes.Cast<TreeNode>().All(node => node.Checked);
            }
        }
        finally
        {
            _syncing = false;
        }

        UpdateCommandStates();
    }

    private void RegistryGridControl_CheckedItemsChanged(object? sender, EventArgs e)
    {
        if (_syncing)
        {
            return;
        }

        _syncing = true;
        try
        {
            if (_registryRootNode is not null)
            {
                _registryRootNode.Checked = _registryItems.Where(item => item.CanSelect).All(item => item.IsChecked);
            }
        }
        finally
        {
            _syncing = false;
        }

        UpdateCommandStates();
    }

    private void RefreshDetailFromSelectedNode()
    {
        _statusSize.Text = string.Empty;
        if (_treeSources.SelectedNode is not { } node)
        {
            _filesGridControl.SetItems([]);
            ShowDetail(_filesGridControl);
            return;
        }

        switch (node.Tag)
        {
            case CategoryTag _:
                _filesGridControl.SetItems(
                    EnumerateCatalogLeafNodes(node.Nodes)
                        .Where(leaf => _nodeItems.ContainsKey(leaf))
                        .SelectMany(leaf => _nodeItems[leaf])
                        .ToList());
                ShowDetail(_filesGridControl);
                _statusInfo.Text = node.Text;
                break;

            case CatalogEntry entry:
                _filesGridControl.SetItems(_nodeItems.TryGetValue(node, out List<DiscoveredItem>? items) ? items : []);
                ShowDetail(_filesGridControl);
                _statusInfo.Text = entry.Description;
                break;

            case RegistryGroupTag registryTag:
                _registryGridControl.SetItems(_registryItems);
                ShowDetail(_registryGridControl);
                _statusInfo.Text = registryTag.Entry.Description;
                break;

            case VsRootTag rootTag:
                _vsOverviewControl.SetItems(_visualStudioSkus);
                ShowDetail(_vsOverviewControl);
                _statusInfo.Text = rootTag.Entry.Description;
                break;

            case VsSku sku:
                _vsOverviewControl.SetItems([sku]);
                ShowDetail(_vsOverviewControl);
                _statusInfo.Text = string.IsNullOrWhiteSpace(sku.SettingsPath) ? sku.NodeLabel : sku.SettingsPath;
                break;

            case VsHivesTag hivesTag:
                _vsHivesControl.SetItems(hivesTag.Sku.Hives);
                ShowDetail(_vsHivesControl);
                _statusInfo.Text = $"{hivesTag.Sku.Hives.Count} hive(s).";
                break;

            case VsExtensionsTag extensionsTag:
                _vsExtensionsControl.SetItems(extensionsTag.Sku.Extensions);
                ShowDetail(_vsExtensionsControl);
                _statusInfo.Text = $"{extensionsTag.Sku.Extensions.Count} extension(s).";
                break;

            default:
                _filesGridControl.SetItems([]);
                ShowDetail(_filesGridControl);
                break;
        }
    }

    private void RefreshCatalogLeafCheckState(TreeNode leaf)
    {
        if (leaf.Tag is not CatalogEntry)
        {
            return;
        }

        bool allChecked = _nodeItems.TryGetValue(leaf, out List<DiscoveredItem>? items)
            && items.Count > 0
            && items.All(item => item.IsChecked);
        leaf.Checked = allChecked;
        if (leaf.Parent is { Tag: CategoryTag } parent)
        {
            parent.Checked = parent.Nodes.Cast<TreeNode>().All(node => node.Checked);
        }
    }

    private void SetVisualStudioNodeChecked(TreeNode node, bool value)
    {
        node.Checked = value;
        if (node.Tag is VsSku sku)
        {
            sku.IsChecked = value;
        }

        foreach (TreeNode child in node.Nodes.Cast<TreeNode>())
        {
            child.Checked = value;
        }
    }

    private static IEnumerable<TreeNode> EnumerateCatalogLeafNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Nodes.Count == 0 && node.Tag is CatalogEntry)
            {
                yield return node;
            }
            else
            {
                foreach (TreeNode descendant in EnumerateCatalogLeafNodes(node.Nodes))
                {
                    yield return descendant;
                }
            }
        }
    }
}
