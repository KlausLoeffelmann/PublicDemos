using System.Globalization;
using WinBaas.Models;

namespace WinBaas;

/// <summary>
///  Tree/grid synchronization: clicks on a tree node refresh the grid; tree
///  check/uncheck propagates to grid rows; grid checkbox edits propagate
///  back to the tree (tri-state when partially checked). Parent (category)
///  nodes toggle every leaf entry below them.
/// </summary>
public sealed partial class FrmMain
{
    private void TreeSources_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        RefreshGridFromSelectedNode();
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

            if (e.Node.Tag is CategoryTag)
            {
                foreach (TreeNode leaf in EnumerateLeafNodes(e.Node.Nodes))
                {
                    leaf.Checked = target;
                    if (_nodeItems.TryGetValue(leaf, out var leafItems))
                    {
                        foreach (var item in leafItems)
                        {
                            item.IsChecked = target;
                        }
                    }
                }
            }
            else if (e.Node.Tag is CatalogEntry)
            {
                if (_nodeItems.TryGetValue(e.Node, out var items))
                {
                    foreach (var item in items)
                    {
                        item.IsChecked = target;
                    }
                }

                if (e.Node.Parent is { } parent && parent.Tag is CategoryTag)
                {
                    parent.Checked = parent.Nodes.Cast<TreeNode>().All(n => n.Checked);
                }
            }

            if (ReferenceEquals(e.Node, _treeSources.SelectedNode))
            {
                ApplyCheckedStateToGrid(target);
            }
        }
        finally
        {
            _syncing = false;
        }

        UpdateCommandStates();
    }

    private void Grid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_grid.IsCurrentCellDirty && _grid.CurrentCell is DataGridViewCheckBoxCell)
        {
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_syncing || e.RowIndex < 0 || e.ColumnIndex != _colCheck.Index)
        {
            return;
        }

        if (_grid.Rows[e.RowIndex].Tag is DiscoveredItem item)
        {
            item.IsChecked = (bool)(_grid.Rows[e.RowIndex].Cells[_colCheck.Index].Value ?? false);
        }

        if (_treeSources.SelectedNode is { Tag: CatalogEntry } leaf
            && _nodeItems.TryGetValue(leaf, out var items))
        {
            bool allChecked = items.Count > 0 && items.All(i => i.IsChecked);
            _syncing = true;
            try
            {
                leaf.Checked = allChecked;
                if (leaf.Parent is { } parent && parent.Tag is CategoryTag)
                {
                    parent.Checked = parent.Nodes.Cast<TreeNode>().All(n => n.Checked);
                }
            }
            finally
            {
                _syncing = false;
            }
        }

        UpdateCommandStates();
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            _statusSize.Text = string.Empty;
            return;
        }

        long total = _grid.SelectedRows
            .Cast<DataGridViewRow>()
            .Select(r => r.Tag as DiscoveredItem)
            .Where(i => i?.SizeBytes is not null)
            .Sum(i => i!.SizeBytes!.Value);

        _statusSize.Text = FormatSize(total);
    }

    private void RefreshGridFromSelectedNode()
    {
        _grid.Rows.Clear();
        if (_treeSources.SelectedNode is not { } node)
        {
            return;
        }

        IEnumerable<DiscoveredItem> items;
        if (node.Tag is CategoryTag)
        {
            items = EnumerateLeafNodes(node.Nodes)
                .Where(l => _nodeItems.ContainsKey(l))
                .SelectMany(l => _nodeItems[l]);
        }
        else if (_nodeItems.TryGetValue(node, out var leafItems))
        {
            items = leafItems;
        }
        else
        {
            return;
        }

        foreach (DiscoveredItem item in items)
        {
            int rowIndex = _grid.Rows.Add(
                item.IsChecked,
                item.Name,
                item.FileTypeLabel,
                item.LastChanged?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? string.Empty,
                item.Created?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                item.SizeBytes is null ? "\u2026" : FormatSizeShort(item.SizeBytes.Value));
            _grid.Rows[rowIndex].Tag = item;
        }
    }

    private void ApplyCheckedStateToGrid(bool target)
    {
        foreach (DataGridViewRow row in _grid.Rows)
        {
            row.Cells[_colCheck.Index].Value = target;
        }
    }

    private static IEnumerable<TreeNode> EnumerateLeafNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Nodes.Count == 0 && node.Tag is CatalogEntry)
            {
                yield return node;
            }
            else
            {
                foreach (TreeNode descendant in EnumerateLeafNodes(node.Nodes))
                {
                    yield return descendant;
                }
            }
        }
    }

    /// <summary>
    ///  Format <paramref name="bytes"/> as an IEC-style size with the matching
    ///  byte count in parentheses, e.g. <c>"1.23 MiB (1,290,000 bytes)"</c>.
    /// </summary>
    private static string FormatSize(long bytes)
    {
        string iec = FormatSizeShort(bytes);
        string raw = bytes.ToString("###,###,###,###,###,##0", CultureInfo.InvariantCulture);
        return $"{iec} ({raw} bytes)";
    }

    private static string FormatSizeShort(long bytes)
    {
        string[] units = ["bytes", "KiB", "MiB", "GiB", "TiB", "PiB"];
        double value = bytes;
        int unit = 0;
        while (value >= 1024d && unit < units.Length - 1)
        {
            value /= 1024d;
            unit++;
        }

        return unit == 0
            ? $"{bytes:N0} {units[0]}"
            : string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1}", value, units[unit]);
    }
}

