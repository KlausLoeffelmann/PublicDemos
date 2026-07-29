namespace TreeViewNodeFontScaling;

public partial class MainForm : Form
{
    private readonly List<Font> _demoFonts = [];

    public MainForm()
    {
        InitializeComponent();
        PopulateTree();
        _treeView.RecalculateItemHeight();
        UpdateItemHeightStatus();
    }

    private void MainForm_Disposed(object? sender, EventArgs e)
    {
        foreach (Font font in _demoFonts)
        {
            font.Dispose();
        }

        _demoFonts.Clear();
    }

    private void PopulateTree()
    {
        _treeView.BeginUpdate();

        try
        {
            TreeNodeEx root = new("TreeViewEx rendering matrix")
            {
                Bold = true,
                FullRowSelect = true,
                ItemPadding = new Padding(6, 3, 6, 3),
                SelectionColor = Color.SteelBlue,
                HoverColor = Color.LightSteelBlue,
            };

            TreeNodeEx styles = new("Font style flags")
            {
                Bold = true,
                ItemMargin = new Padding(0, 2, 0, 2),
            };
            styles.Nodes.Add(new TreeNodeEx("Regular baseline"));
            styles.Nodes.Add(new TreeNodeEx("Bold") { Bold = true });
            styles.Nodes.Add(new TreeNodeEx("Italic") { Italic = true });
            styles.Nodes.Add(new TreeNodeEx("Underlined") { Underlined = true });
            styles.Nodes.Add(new TreeNodeEx("StrikeThrough") { StrikeThrough = true });
            styles.Nodes.Add(
                new TreeNodeEx("Bold + Italic + Underlined + StrikeThrough")
                {
                    Bold = true,
                    Italic = true,
                    Underlined = true,
                    StrikeThrough = true,
                });

            TreeNodeEx colors = new("Selection and hover colors") { Italic = true };
            colors.Nodes.Add(
                new TreeNodeEx("System selection and hover fallbacks")
                {
                    FullRowSelect = true,
                    ItemPadding = new Padding(5, 2, 5, 2),
                });
            colors.Nodes.Add(
                new TreeNodeEx("Custom selection: MediumSeaGreen")
                {
                    SelectionColor = Color.MediumSeaGreen,
                    ItemPadding = new Padding(5, 2, 5, 2),
                });
            colors.Nodes.Add(
                new TreeNodeEx("Custom hover: Moccasin")
                {
                    HoverColor = Color.Moccasin,
                    ItemPadding = new Padding(5, 2, 5, 2),
                });
            colors.Nodes.Add(
                new TreeNodeEx("Custom full-row selection and hover")
                {
                    FullRowSelect = true,
                    SelectionColor = Color.MediumPurple,
                    HoverColor = Color.Plum,
                    ItemMargin = new Padding(8, 2, 12, 2),
                    ItemPadding = new Padding(6, 3, 6, 3),
                });

            TreeNodeEx spacing = new("Margin, padding, and per-node full-row selection")
            {
                Underlined = true,
            };
            spacing.Nodes.Add(
                new TreeNodeEx("Content-only highlight with padding")
                {
                    HoverColor = Color.PaleTurquoise,
                    ItemPadding = new Padding(16, 6, 16, 6),
                });
            spacing.Nodes.Add(
                new TreeNodeEx("Full-row highlight with outer margin")
                {
                    FullRowSelect = true,
                    HoverColor = Color.PaleGoldenrod,
                    SelectionColor = Color.DarkGoldenrod,
                    ItemMargin = new Padding(12, 5, 20, 5),
                    ItemPadding = new Padding(8, 4, 8, 4),
                });
            spacing.Nodes.Add(
                new TreeNodeEx("Asymmetric spacing")
                {
                    ItemMargin = new Padding(4, 1, 18, 7),
                    ItemPadding = new Padding(20, 2, 4, 8),
                    Italic = true,
                });

            TreeNodeEx fonts = new("Mixed font families and sizes") { Bold = true };
            fonts.Nodes.Add(
                new TreeNodeEx("Segoe UI, 9 pt")
                {
                    NodeFont = CreateDemoFont("Segoe UI", 9F),
                });
            fonts.Nodes.Add(
                new TreeNodeEx("Consolas, 11 pt, italic")
                {
                    NodeFont = CreateDemoFont("Consolas", 11F),
                    Italic = true,
                    ItemPadding = new Padding(3),
                });
            fonts.Nodes.Add(
                new TreeNodeEx("Times New Roman, 14 pt, underlined")
                {
                    NodeFont = CreateDemoFont("Times New Roman", 14F),
                    Underlined = true,
                    ItemPadding = new Padding(4),
                });
            fonts.Nodes.Add(
                new TreeNodeEx("Segoe UI, 20 pt, bold and full-row")
                {
                    NodeFont = CreateDemoFont("Segoe UI", 20F),
                    Bold = true,
                    FullRowSelect = true,
                    SelectionColor = Color.Teal,
                    HoverColor = Color.LightSeaGreen,
                    ItemMargin = new Padding(4),
                    ItemPadding = new Padding(6),
                });

            TreeNodeEx hierarchy = new("Connector-line coverage") { StrikeThrough = true };
            TreeNodeEx firstBranch = new("First branch");
            firstBranch.Nodes.Add(new TreeNodeEx("First child"));
            TreeNodeEx middleChild = new("Middle child with descendants") { Bold = true };
            middleChild.Nodes.Add(new TreeNodeEx("Grandchild A"));
            TreeNodeEx grandchildB = new("Grandchild B");
            grandchildB.Nodes.Add(new TreeNodeEx("Great-grandchild"));
            middleChild.Nodes.Add(grandchildB);
            firstBranch.Nodes.Add(middleChild);
            firstBranch.Nodes.Add(new TreeNodeEx("Last child"));
            hierarchy.Nodes.Add(firstBranch);
            hierarchy.Nodes.Add(new TreeNodeEx("Middle sibling"));
            TreeNodeEx collapsedBranch = new("Collapsed branch - expand to inspect lines")
            {
                Italic = true,
            };
            collapsedBranch.Nodes.Add(new TreeNodeEx("Hidden child 1"));
            collapsedBranch.Nodes.Add(new TreeNodeEx("Hidden child 2"));
            hierarchy.Nodes.Add(collapsedBranch);
            hierarchy.Nodes.Add(new TreeNodeEx("Last sibling"));

            TreeNode compatibility = new("Ordinary TreeNode compatibility")
            {
                NodeFont = CreateDemoFont("Segoe UI", 10F, FontStyle.Italic),
            };
            compatibility.Nodes.Add(new TreeNode("Ordinary child node"));

            root.Nodes.Add(styles);
            root.Nodes.Add(colors);
            root.Nodes.Add(spacing);
            root.Nodes.Add(fonts);
            root.Nodes.Add(hierarchy);
            root.Nodes.Add(compatibility);
            _treeView.Nodes.Add(root);

            root.Expand();
            styles.Expand();
            colors.Expand();
            spacing.Expand();
            fonts.Expand();
            hierarchy.Expand();
            firstBranch.Expand();
            middleChild.Expand();
            grandchildB.Expand();
            compatibility.Expand();
            collapsedBranch.Collapse();

            _treeView.SelectedNode = colors.Nodes[3];
            _treeView.SelectedNode.EnsureVisible();
        }
        finally
        {
            _treeView.EndUpdate();
        }
    }

    private Font CreateDemoFont(
        string familyName,
        float emSize,
        FontStyle style = FontStyle.Regular)
    {
        Font font = new(familyName, emSize, style);
        _demoFonts.Add(font);
        return font;
    }

    private void TreeView_CalculatedItemHeightChanged(object? sender, EventArgs e)
        => UpdateItemHeightStatus();

    private void UpdateItemHeightStatus()
        => _statusLabel.Text =
            $"Calculated global ItemHeight: {_treeView.ItemHeight}px  |  "
            + "Hover, select, and expand nodes to exercise the renderer.";
}
