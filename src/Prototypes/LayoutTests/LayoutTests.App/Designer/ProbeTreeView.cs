using LayoutTests.App.Models;

namespace LayoutTests.App.Designer;

public sealed class ProbeTreeView : TreeView
{
    private const int FormIconIndex = 0;
    private const int CtorIconIndex = 1;
    private const int LazyIconIndex = 2;

    private ProbeSet? _set;

    public ProbeTreeView()
    {
        HideSelection = false;
        ShowRootLines = true;
        FullRowSelect = true;
        ImageList = BuildImageList();
        ImageIndex = FormIconIndex;
        SelectedImageIndex = FormIconIndex;
    }

    public ContainerDefinition? SelectedContainer => SelectedNode?.Tag as ContainerDefinition;

    public void Bind(ProbeSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        _set = set;
        var previousId = SelectedContainer?.Id;
        BeginUpdate();
        try
        {
            Nodes.Clear();
            var root = new TreeNode(FormatFormNode(set))
            {
                Tag = null,
                ImageIndex = FormIconIndex,
                SelectedImageIndex = FormIconIndex,
            };
            Nodes.Add(root);
            AppendChildren(root, set.Roots);
            ExpandAll();
        }
        finally
        {
            EndUpdate();
        }

        if (previousId.HasValue && TryFindNode(Nodes, previousId.Value, out var node))
        {
            SelectedNode = node;
        }
        else
        {
            SelectedNode = Nodes.Count > 0 ? Nodes[0] : null;
        }
    }

    public void SelectContainer(ContainerDefinition def)
    {
        ArgumentNullException.ThrowIfNull(def);

        if (TryFindNode(Nodes, def.Id, out var node))
        {
            SelectedNode = node;
            node.EnsureVisible();
        }
    }

    public void RefreshSelectedNodeText()
    {
        if (SelectedNode is null)
        {
            return;
        }

        if (SelectedNode.Tag is ContainerDefinition def)
        {
            SelectedNode.Text = FormatContainerNode(def);
        }
    }

    public void RefreshRootNodeText()
    {
        if (Nodes.Count == 0 || _set is null)
        {
            return;
        }

        Nodes[0].Text = FormatFormNode(_set);
    }

    private static void AppendChildren(TreeNode parent, List<ContainerDefinition> children)
    {
        foreach (var child in children)
        {
            int icon = child.Kind == ContainerKind.CTor ? CtorIconIndex : LazyIconIndex;
            var node = new TreeNode(FormatContainerNode(child))
            {
                Tag = child,
                ImageIndex = icon,
                SelectedImageIndex = icon,
            };
            parent.Nodes.Add(node);
            AppendChildren(node, child.Children);
        }
    }

    private static bool TryFindNode(TreeNodeCollection nodes, Guid id, out TreeNode found)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is ContainerDefinition def && def.Id == id)
            {
                found = node;
                return true;
            }

            if (TryFindNode(node.Nodes, id, out found))
            {
                return true;
            }
        }

        found = null!;
        return false;
    }

    private static string FormatFormNode(ProbeSet set) =>
        $"Probe Form: {set.Name}";

    private static string FormatContainerNode(ContainerDefinition def) =>
        $"{def.Name} [{def.Kind}, {(int)def.Parameters.ScalePercent}%, {def.Parameters.AutoScaleMode}]";

    private static ImageList BuildImageList()
    {
        var list = new ImageList
        {
            ImageSize = new Size(16, 16),
            ColorDepth = ColorDepth.Depth32Bit,
        };

        list.Images.Add(CreateIcon(Color.SteelBlue, "F"));
        list.Images.Add(CreateIcon(Color.MediumSeaGreen, "C"));
        list.Images.Add(CreateIcon(Color.Goldenrod, "L"));
        return list;
    }

    private static Bitmap CreateIcon(Color color, string letter)
    {
        var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        using var brush = new SolidBrush(color);
        g.FillRectangle(brush, 1, 1, 14, 14);
        g.DrawRectangle(Pens.Black, 1, 1, 13, 13);
        using var font = new Font("Segoe UI", 8F, FontStyle.Bold);
        var size = g.MeasureString(letter, font);
        g.DrawString(letter, font, Brushes.White, (16 - size.Width) / 2, (16 - size.Height) / 2);
        return bmp;
    }
}
