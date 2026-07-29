using System.ComponentModel;

namespace TreeViewNodeFontScaling;

/// <summary>
///  Extends <see cref="TreeNode"/> with per-node appearance and spacing options.
/// </summary>
public class TreeNodeEx : TreeNode
{
    private bool _bold;
    private bool _italic;
    private bool _underlined;
    private bool _strikeThrough;
    private Color _selectionColor;
    private Color _hoverColor;
    private bool _fullRowSelect;
    private Padding _itemMargin;
    private Padding _itemPadding;

    /// <summary>
    ///  Initializes an empty <see cref="TreeNodeEx"/>.
    /// </summary>
    public TreeNodeEx()
    {
    }

    /// <summary>
    ///  Initializes a <see cref="TreeNodeEx"/> with the specified text.
    /// </summary>
    /// <param name="text">The text displayed by the node.</param>
    public TreeNodeEx(string text)
        : base(text)
    {
    }

    /// <summary>
    ///  Initializes a <see cref="TreeNodeEx"/> with text and child nodes.
    /// </summary>
    /// <param name="text">The text displayed by the node.</param>
    /// <param name="children">The child nodes to add.</param>
    public TreeNodeEx(string text, TreeNode[] children)
        : base(text, children)
    {
    }

    /// <summary>
    ///  Gets or sets the node font and notifies an owning <see cref="TreeViewEx"/>
    ///  that its global item height may need recalculation.
    /// </summary>
    [DefaultValue(null)]
    public new Font? NodeFont
    {
        get => base.NodeFont;
        set
        {
            if (ReferenceEquals(base.NodeFont, value))
            {
                return;
            }

            base.NodeFont = value;
            NotifyOwner(affectsItemHeight: true);
        }
    }

    /// <summary>
    ///  Gets or sets whether the effective node font is bold.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Bold
    {
        get => _bold;
        set => SetField(ref _bold, value, affectsItemHeight: true);
    }

    /// <summary>
    ///  Gets or sets whether the effective node font is italic.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Italic
    {
        get => _italic;
        set => SetField(ref _italic, value, affectsItemHeight: true);
    }

    /// <summary>
    ///  Gets or sets whether the effective node font is underlined.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Underlined
    {
        get => _underlined;
        set => SetField(ref _underlined, value, affectsItemHeight: true);
    }

    /// <summary>
    ///  Gets or sets whether the effective node font uses strikeout.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool StrikeThrough
    {
        get => _strikeThrough;
        set => SetField(ref _strikeThrough, value, affectsItemHeight: true);
    }

    /// <summary>
    ///  Gets or sets the background color used when the node is selected.
    ///  <see cref="Color.Empty"/> uses the system selection color.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(typeof(Color), "")]
    public Color SelectionColor
    {
        get => _selectionColor;
        set => SetField(ref _selectionColor, value, affectsItemHeight: false);
    }

    /// <summary>
    ///  Gets or sets the background color used when the pointer is over the node.
    ///  <see cref="Color.Empty"/> uses a theme-aware system hover color.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(typeof(Color), "")]
    public Color HoverColor
    {
        get => _hoverColor;
        set => SetField(ref _hoverColor, value, affectsItemHeight: false);
    }

    /// <summary>
    ///  Gets or sets whether selection and hover backgrounds span the entire row.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool FullRowSelect
    {
        get => _fullRowSelect;
        set => SetField(ref _fullRowSelect, value, affectsItemHeight: false);
    }

    /// <summary>
    ///  Gets or sets the space outside the node's painted row content.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(typeof(Padding), "0, 0, 0, 0")]
    public Padding ItemMargin
    {
        get => _itemMargin;
        set => SetField(ref _itemMargin, value, affectsItemHeight: true);
    }

    /// <summary>
    ///  Gets or sets the space inside the node's painted row content.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(typeof(Padding), "0, 0, 0, 0")]
    public Padding ItemPadding
    {
        get => _itemPadding;
        set => SetField(ref _itemPadding, value, affectsItemHeight: true);
    }

    private void SetField(ref bool field, bool value, bool affectsItemHeight)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        NotifyOwner(affectsItemHeight);
    }

    private void SetField(ref Color field, Color value, bool affectsItemHeight)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        NotifyOwner(affectsItemHeight);
    }

    private void SetField(ref Padding field, Padding value, bool affectsItemHeight)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        NotifyOwner(affectsItemHeight);
    }

    private void NotifyOwner(bool affectsItemHeight)
    {
        if (TreeView is TreeViewEx treeView)
        {
            treeView.NotifyNodeAppearanceChanged(this, affectsItemHeight);
        }
    }
}
