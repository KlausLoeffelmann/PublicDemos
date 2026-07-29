using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace TreeViewNodeFontScaling;

/// <summary>
///  A TreeView that supports per-node font styles, colors, spacing, full-row
///  highlighting, and automatic global item-height calculation.
/// </summary>
public class TreeViewEx : TreeView
{
    private const int TvmInsertItemA = 0x1100;
    private const int TvmDeleteItem = 0x1101;
    private const int TvmInsertItemW = 0x1132;
    private const int LogicalGlyphSize = 9;

    private Dictionary<FontKey, Font>? _derivedFonts;
    private TreeNode? _hotNode;
    private bool _recalculationPosted;
    private bool _recalculationRequested;
    private bool _recalculating;

    /// <summary>
    ///  Initializes a new instance of the <see cref="TreeViewEx"/> class.
    /// </summary>
    public TreeViewEx()
    {
        DrawMode = TreeViewDrawMode.OwnerDrawAll;
        DoubleBuffered = true;
        FullRowSelect = false;
        HideSelection = false;
        HotTracking = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    /// <summary>
    ///  Occurs after the calculated global <see cref="TreeView.ItemHeight"/> changes.
    /// </summary>
    [Category("Property Changed")]
    public event EventHandler? CalculatedItemHeightChanged;

    /// <summary>
    ///  Recalculates the global item height required by all current nodes.
    /// </summary>
    /// <remarks>
    ///  Call this method after changing <see cref="TreeNode.NodeFont"/> through a
    ///  base-typed or ordinary <see cref="TreeNode"/>, because that property does
    ///  not expose a managed change notification.
    /// </remarks>
    public void RecalculateItemHeight()
    {
        if (InvokeRequired)
        {
            throw new InvalidOperationException(
                "RecalculateItemHeight must be called on the TreeViewEx UI thread.");
        }

        if (_recalculating)
        {
            return;
        }

        _recalculating = true;
        _recalculationRequested = false;
        ClearDerivedFonts();

        try
        {
            int requiredHeight = CalculateRequiredHeight(Font, Padding.Empty, Padding.Empty);

            foreach (TreeNode node in EnumerateNodes(Nodes))
            {
                Font effectiveFont = GetEffectiveFont(node);
                Padding margin = GetMargin(node);
                Padding padding = GetPadding(node);
                requiredHeight = Math.Max(
                    requiredHeight,
                    CalculateRequiredHeight(effectiveFont, margin, padding));
            }

            if (ItemHeight != requiredHeight)
            {
                ItemHeight = requiredHeight;
                CalculatedItemHeightChanged?.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }
        finally
        {
            _recalculating = false;
        }
    }

    /// <inheritdoc/>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RecalculateItemHeight();
    }

    /// <inheritdoc/>
    protected override void OnFontChanged(EventArgs e)
    {
        ClearDerivedFonts();
        base.OnFontChanged(e);
        RequestItemHeightRecalculation();
    }

    /// <inheritdoc/>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        ClearDerivedFonts();
        base.OnDpiChangedAfterParent(e);
        RequestItemHeightRecalculation();
    }

    /// <inheritdoc/>
    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        InvalidateSelectedNode();
    }

    /// <inheritdoc/>
    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        InvalidateSelectedNode();
    }

    /// <inheritdoc/>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        SetHotNode(GetNodeAt(e.Location));
    }

    /// <inheritdoc/>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        SetHotNode(null);
    }

    /// <inheritdoc/>
    protected override void OnDrawNode(DrawTreeNodeEventArgs e)
    {
        base.OnDrawNode(e);

        if (e.DrawDefault)
        {
            return;
        }

        if (e.Node is null)
        {
            return;
        }

        TreeNode node = e.Node;
        Padding margin = GetMargin(node);
        Padding padding = GetPadding(node);
        Font effectiveFont = GetEffectiveFont(node);
        Size textSize = MeasureText(e.Graphics, node.Text, effectiveFont);
        Rectangle rowBounds = new(0, e.Bounds.Y, ClientSize.Width, e.Bounds.Height);
        Rectangle labelBounds = node.Bounds.IsEmpty ? e.Bounds : node.Bounds;
        Rectangle contentBounds = GetContentBounds(labelBounds, textSize, margin, padding);
        Rectangle highlightBounds = node is TreeNodeEx { FullRowSelect: true }
            ? GetFullRowBounds(rowBounds, margin)
            : contentBounds;

        using (var backgroundBrush = new SolidBrush(BackColor))
        {
            e.Graphics.FillRectangle(backgroundBrush, rowBounds);
        }

        Color nodeBackColor = node.BackColor.IsEmpty ? BackColor : node.BackColor;
        Color renderedBackgroundColor = nodeBackColor;
        if (nodeBackColor != BackColor)
        {
            using var nodeBackgroundBrush = new SolidBrush(nodeBackColor);
            e.Graphics.FillRectangle(nodeBackgroundBrush, contentBounds);
        }

        bool selected = node == SelectedNode || (e.State & TreeNodeStates.Selected) != 0;
        bool selectionVisible = selected && (Focused || !HideSelection);
        bool hovered = ReferenceEquals(node, _hotNode) || (e.State & TreeNodeStates.Hot) != 0;
        Color textColor = node.ForeColor.IsEmpty ? ForeColor : node.ForeColor;

        if (selectionVisible)
        {
            Color selectionColor = GetSelectionColor(node);
            using var selectionBrush = new SolidBrush(selectionColor);
            e.Graphics.FillRectangle(selectionBrush, highlightBounds);
            textColor = Focused ? SystemColors.HighlightText : SystemColors.ControlText;
            renderedBackgroundColor = selectionColor;
        }
        else if (hovered)
        {
            Color hoverColor = GetHoverColor(node);
            using var hoverBrush = new SolidBrush(hoverColor);
            e.Graphics.FillRectangle(hoverBrush, highlightBounds);
            renderedBackgroundColor = hoverColor;
        }

        DrawHierarchy(e.Graphics, node, labelBounds, rowBounds, renderedBackgroundColor);

        Rectangle textBounds = GetTextBounds(labelBounds, margin, padding, textSize);
        TextRenderer.DrawText(
            e.Graphics,
            node.Text,
            effectiveFont,
            textBounds,
            Enabled ? textColor : SystemColors.GrayText,
            TextFormatFlags.EndEllipsis
                | TextFormatFlags.NoPadding
                | TextFormatFlags.NoPrefix
                | TextFormatFlags.PreserveGraphicsClipping
                | TextFormatFlags.SingleLine
                | TextFormatFlags.VerticalCenter);

        if (selectionVisible && Focused && ShowFocusCues)
        {
            ControlPaint.DrawFocusRectangle(
                e.Graphics,
                Rectangle.Inflate(highlightBounds, -1, -1),
                textColor,
                GetSelectionColor(node));
        }
    }

    /// <inheritdoc/>
    protected override void WndProc(ref Message m)
    {
        int message = m.Msg;
        base.WndProc(ref m);

        if (message is TvmInsertItemA or TvmInsertItemW or TvmDeleteItem)
        {
            ClearDerivedFonts();
            RequestItemHeightRecalculation();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearDerivedFonts();
        }

        base.Dispose(disposing);
    }

    internal void NotifyNodeAppearanceChanged(TreeNode node, bool affectsItemHeight)
    {
        if (affectsItemHeight)
        {
            ClearDerivedFonts();
            RequestItemHeightRecalculation();
        }

        InvalidateNodeRow(node);
    }

    private static IEnumerable<TreeNode> EnumerateNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            yield return node;

            foreach (TreeNode child in EnumerateNodes(node.Nodes))
            {
                yield return child;
            }
        }
    }

    private static Padding GetMargin(TreeNode node)
        => node is TreeNodeEx extended ? Normalize(extended.ItemMargin) : Padding.Empty;

    private static Padding GetPadding(TreeNode node)
        => node is TreeNodeEx extended ? Normalize(extended.ItemPadding) : Padding.Empty;

    private static Padding Normalize(Padding value)
        => new(
            Math.Max(0, value.Left),
            Math.Max(0, value.Top),
            Math.Max(0, value.Right),
            Math.Max(0, value.Bottom));

    private static Rectangle GetFullRowBounds(Rectangle rowBounds, Padding margin)
        => new(
            margin.Left,
            rowBounds.Top + margin.Top,
            Math.Max(0, rowBounds.Width - margin.Horizontal),
            Math.Max(0, rowBounds.Height - margin.Vertical));

    private static Color GetSelectionColor(TreeNode node)
        => node is TreeNodeEx { SelectionColor.IsEmpty: false } extended
            ? extended.SelectionColor
            : SystemColors.Highlight;

    private static Color GetHoverColor(TreeNode node)
    {
        if (node is TreeNodeEx { HoverColor.IsEmpty: false } extended)
        {
            return extended.HoverColor;
        }

        return Application.IsDarkModeEnabled
            ? SystemColors.ControlDark
            : SystemColors.ControlLight;
    }

    private Font GetEffectiveFont(TreeNode node)
    {
        Font baseFont = node.NodeFont ?? Font;
        FontStyle style = baseFont.Style;

        if (node is TreeNodeEx extended)
        {
            if (extended.Bold)
            {
                style |= FontStyle.Bold;
            }

            if (extended.Italic)
            {
                style |= FontStyle.Italic;
            }

            if (extended.Underlined)
            {
                style |= FontStyle.Underline;
            }

            if (extended.StrikeThrough)
            {
                style |= FontStyle.Strikeout;
            }
        }

        if (style == baseFont.Style)
        {
            return baseFont;
        }

        FontKey key = new(baseFont, style);
        _derivedFonts ??= [];

        if (!_derivedFonts.TryGetValue(key, out Font? derivedFont))
        {
            derivedFont = new Font(baseFont, style);
            _derivedFonts.Add(key, derivedFont);
        }

        return derivedFont;
    }

    private int CalculateRequiredHeight(Font font, Padding margin, Padding padding)
    {
        Size textSize = TextRenderer.MeasureText(
            "Ag",
            font,
            Size.Empty,
            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

        int glyphMinimum = ScaleLogical(LogicalGlyphSize) + ScaleLogical(4);
        int contentHeight = Math.Max(textSize.Height + padding.Vertical, glyphMinimum);
        return Math.Max(1, contentHeight + margin.Vertical);
    }

    private static Size MeasureText(Graphics graphics, string text, Font font)
        => TextRenderer.MeasureText(
            graphics,
            string.IsNullOrEmpty(text) ? " " : text,
            font,
            Size.Empty,
            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);

    private static Rectangle GetContentBounds(
        Rectangle nativeBounds,
        Size textSize,
        Padding margin,
        Padding padding)
        => new(
            nativeBounds.Left + margin.Left,
            nativeBounds.Top + margin.Top,
            Math.Max(0, textSize.Width + padding.Horizontal),
            Math.Max(0, nativeBounds.Height - margin.Vertical));

    private Rectangle GetTextBounds(
        Rectangle nativeBounds,
        Padding margin,
        Padding padding,
        Size textSize)
    {
        int x = nativeBounds.Left + margin.Left + padding.Left;
        int y = nativeBounds.Top + margin.Top + padding.Top;
        int paddedHeight = Math.Max(0, nativeBounds.Height - margin.Vertical - padding.Vertical);
        int centeredY = y + Math.Max(0, (paddedHeight - textSize.Height) / 2);
        int width = Math.Max(0, ClientSize.Width - x - margin.Right - padding.Right);

        return new Rectangle(x, centeredY, width, textSize.Height);
    }

    private void DrawHierarchy(
        Graphics graphics,
        TreeNode node,
        Rectangle nativeBounds,
        Rectangle rowBounds,
        Color backgroundColor)
    {
        int branchX = nativeBounds.Left - Math.Max(1, Indent / 2);
        int centerY = rowBounds.Top + (rowBounds.Height / 2);

        if (ShowLines)
        {
            Color lineColor = LineColor.IsEmpty ? SystemColors.GrayText : LineColor;
            using var linePen = new Pen(lineColor)
            {
                DashStyle = DashStyle.Dot,
            };

            DrawAncestorContinuationLines(graphics, linePen, node, branchX, rowBounds);

            bool drawCurrentBranch = node.Level > 0 || ShowRootLines;
            if (drawCurrentBranch)
            {
                if (node.Parent is not null || node.PrevNode is not null)
                {
                    graphics.DrawLine(linePen, branchX, rowBounds.Top, branchX, centerY);
                }

                if (node.NextNode is not null)
                {
                    graphics.DrawLine(linePen, branchX, centerY, branchX, rowBounds.Bottom);
                }

                graphics.DrawLine(
                    linePen,
                    branchX,
                    centerY,
                    Math.Max(branchX, nativeBounds.Left - ScaleLogical(2)),
                    centerY);
            }
        }

        bool drawGlyph = ShowPlusMinus
            && node.Nodes.Count > 0;

        if (drawGlyph)
        {
            DrawExpandCollapseGlyph(graphics, node, branchX, centerY, backgroundColor);
        }
    }

    private void DrawAncestorContinuationLines(
        Graphics graphics,
        Pen linePen,
        TreeNode node,
        int branchX,
        Rectangle rowBounds)
    {
        TreeNode? ancestor = node.Parent;
        int x = branchX - Indent;

        while (ancestor is not null)
        {
            if (ancestor.NextNode is not null && (ancestor.Level > 0 || ShowRootLines))
            {
                graphics.DrawLine(linePen, x, rowBounds.Top, x, rowBounds.Bottom);
            }

            ancestor = ancestor.Parent;
            x -= Indent;
        }
    }

    private void DrawExpandCollapseGlyph(
        Graphics graphics,
        TreeNode node,
        int centerX,
        int centerY,
        Color backgroundColor)
    {
        int glyphSize = ScaleLogical(LogicalGlyphSize);
        Rectangle glyphBounds = new(
            centerX - (glyphSize / 2),
            centerY - (glyphSize / 2),
            glyphSize,
            glyphSize);

        VisualStyleElement element = node.IsExpanded
            ? VisualStyleElement.TreeView.Glyph.Opened
            : VisualStyleElement.TreeView.Glyph.Closed;

        if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(element))
        {
            var renderer = new VisualStyleRenderer(element);
            renderer.DrawBackground(graphics, glyphBounds);
            return;
        }

        Color glyphColor = Enabled ? ForeColor : SystemColors.GrayText;
        using var glyphPen = new Pen(glyphColor);
        using var backgroundBrush = new SolidBrush(backgroundColor);
        graphics.FillRectangle(backgroundBrush, glyphBounds);
        graphics.DrawRectangle(
            glyphPen,
            glyphBounds.Left,
            glyphBounds.Top,
            glyphBounds.Width - 1,
            glyphBounds.Height - 1);
        graphics.DrawLine(
            glyphPen,
            glyphBounds.Left + ScaleLogical(2),
            centerY,
            glyphBounds.Right - ScaleLogical(3),
            centerY);

        if (!node.IsExpanded)
        {
            graphics.DrawLine(
                glyphPen,
                centerX,
                glyphBounds.Top + ScaleLogical(2),
                centerX,
                glyphBounds.Bottom - ScaleLogical(3));
        }
    }

    private int ScaleLogical(int value)
        => Math.Max(1, (int)Math.Round(value * DeviceDpi / 96d));

    private void SetHotNode(TreeNode? node)
    {
        if (ReferenceEquals(_hotNode, node))
        {
            return;
        }

        TreeNode? previous = _hotNode;
        _hotNode = node;
        InvalidateNodeRow(previous);
        InvalidateNodeRow(_hotNode);
    }

    private void InvalidateSelectedNode()
        => InvalidateNodeRow(SelectedNode);

    private void InvalidateNodeRow(TreeNode? node)
    {
        if (node is null || !IsHandleCreated)
        {
            return;
        }

        Rectangle bounds = node.Bounds;
        if (bounds.IsEmpty)
        {
            return;
        }

        Invalidate(new Rectangle(0, bounds.Top, ClientSize.Width, ItemHeight));
    }

    private void RequestItemHeightRecalculation()
    {
        _recalculationRequested = true;

        if (!IsHandleCreated || IsDisposed || Disposing || _recalculationPosted)
        {
            return;
        }

        _recalculationPosted = true;
        BeginInvoke(ProcessPendingRecalculation);
    }

    private void ProcessPendingRecalculation()
    {
        _recalculationPosted = false;

        if (_recalculationRequested && !IsDisposed && !Disposing)
        {
            RecalculateItemHeight();
        }
    }

    private void ClearDerivedFonts()
    {
        if (_derivedFonts is null)
        {
            return;
        }

        foreach (Font font in _derivedFonts.Values)
        {
            font.Dispose();
        }

        _derivedFonts.Clear();
    }

    private readonly record struct FontKey(
        string Name,
        float Size,
        FontStyle Style,
        GraphicsUnit Unit,
        byte GdiCharSet,
        bool GdiVerticalFont)
    {
        public FontKey(Font font, FontStyle style)
            : this(
                font.Name,
                font.Size,
                style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont)
        {
        }
    }
}
