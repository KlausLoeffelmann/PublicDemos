// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Owns the "double-click to (de)select controls for the shared PropertyGrid" behavior for one
///  scenario view. Replaces the old <c>ScenarioSelectionHelper</c> / companion-CheckBox model.
/// </summary>
/// <remarks>
///  <para>
///   Each selectable demo control lives inside a <see cref="SelectablePanel"/>. This controller:
///   <list type="bullet">
///    <item>toggles a panel's <see cref="SelectablePanel.Selected"/> state on double-click;</item>
///    <item>supports Shift + double-click "from anchor to target" rectangle selection across the
///     cells of a shared <see cref="TableLayoutPanel"/>;</item>
///    <item>exposes Select All / Clear Selection and the currently selected hosted controls;</item>
///    <item>raises <see cref="SelectionChanged"/> (coalesced during bulk operations).</item>
///   </list>
///  </para>
///  <para>
///   Button-derived controls (<see cref="Button"/>, <see cref="CheckBox"/>, <see cref="RadioButton"/>)
///   deliberately suppress the <see cref="Control.DoubleClick"/> event, so we detect the double-click
///   ourselves from <see cref="Control.MouseDown"/> using <see cref="SystemInformation.DoubleClickTime"/>
///   and <see cref="SystemInformation.DoubleClickSize"/> - this works uniformly for every control type.
///  </para>
/// </remarks>
internal sealed class SelectionController
{
    private readonly List<SelectablePanel> _panels = [];
    private SelectablePanel? _anchor;
    private bool _suspendNotifications;

    /// <summary>
    ///  Raised whenever the set of selected panels changes. During a bulk operation (Shift-range,
    ///  Select All, Clear Selection) it is raised exactly once at the end rather than per panel.
    /// </summary>
    public event EventHandler? SelectionChanged;

    /// <summary>
    ///  Registers already-constructed <see cref="SelectablePanel"/>s (e.g. those placed directly in
    ///  a Designer-built grid) and wires their double-click handling.
    /// </summary>
    public void Register(params SelectablePanel[] panels)
    {
        foreach (SelectablePanel panel in panels)
        {
            Wire(panel);
        }
    }

    /// <summary>
    ///  Wraps each existing demo control in a new <see cref="SelectablePanel"/> in place (preserving
    ///  its <see cref="TableLayoutPanel"/> cell/span or, as a fallback, its bounds) and registers it.
    ///  Used to retrofit views that were previously built around companion CheckBoxes.
    /// </summary>
    public IReadOnlyList<SelectablePanel> WrapAndRegister(params Control[] targets)
    {
        List<SelectablePanel> created = new(targets.Length);
        foreach (Control target in targets)
        {
            SelectablePanel panel = WrapInPlace(target);
            Wire(panel);
            created.Add(panel);
        }

        return created;
    }

    /// <summary>Selects every registered panel (Edit &gt; Select All).</summary>
    public void SelectAll() => ApplyBulk(_panels, selected: true);

    /// <summary>Clears the selection of every registered panel (Edit &gt; Clear Selection).</summary>
    public void ClearSelection()
    {
        ApplyBulk(_panels, selected: false);
        _anchor = null;
    }

    /// <summary>Returns the hosted control of every currently selected panel.</summary>
    public IReadOnlyList<Control> GetSelectedControls() =>
        _panels
            .Where(panel => panel.Selected && panel.HostedControl is not null)
            .Select(panel => panel.HostedControl!)
            .ToArray();

    /// <summary>
    ///  Applies a new selection gap (in pixels) to every registered panel, so the whole view's
    ///  controls keep a consistent margin between their chrome and the selection frame (driven by
    ///  the View &gt; Selection Margin menu in <see cref="MainForm"/>).
    /// </summary>
    public void SetSelectionGap(int gap)
    {
        foreach (SelectablePanel panel in _panels)
        {
            panel.SelectionGap = gap;
        }
    }

    private void Wire(SelectablePanel panel)
    {
        _panels.Add(panel);
        panel.SelectedChanged += Panel_SelectedChanged;

        // Double-clicking the padding "frame" hits the panel; double-clicking the control hits the
        // hosted child. Both must route to the same activation, so detect on both.
        HookDoubleClick(panel, panel);
        if (panel.HostedControl is Control hosted)
        {
            HookDoubleClick(hosted, panel);
        }
    }

    /// <summary>
    ///  Attaches a manual double-click detector to <paramref name="source"/> that activates
    ///  <paramref name="panel"/>. Manual detection is required because Button/CheckBox/RadioButton
    ///  do not raise the framework <see cref="Control.DoubleClick"/> event.
    /// </summary>
    private void HookDoubleClick(Control source, SelectablePanel panel)
    {
        // Per-control closure state; each control tracks its own "previous click" independently.
        DateTime lastClickUtc = DateTime.MinValue;
        Point lastClickPoint = Point.Empty;

        source.MouseDown += (_, e) =>
        {
            DateTime now = DateTime.UtcNow;
            Size slop = SystemInformation.DoubleClickSize;

            bool isDoubleClick =
                (now - lastClickUtc).TotalMilliseconds <= SystemInformation.DoubleClickTime
                && Math.Abs(e.X - lastClickPoint.X) <= slop.Width
                && Math.Abs(e.Y - lastClickPoint.Y) <= slop.Height;

            if (isDoubleClick)
            {
                // Reset so a third rapid click doesn't immediately re-trigger.
                lastClickUtc = DateTime.MinValue;
                Activate(panel);
            }
            else
            {
                lastClickUtc = now;
                lastClickPoint = e.Location;
            }
        };
    }

    /// <summary>
    ///  Handles a confirmed double-click on <paramref name="target"/>: plain toggle, or - when Shift
    ///  is held and an anchor exists in the same grid - a from/to rectangle (de)selection.
    /// </summary>
    private void Activate(SelectablePanel target)
    {
        bool shiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

        if (shiftHeld && _anchor is not null && TryGetRange(_anchor, target, out List<SelectablePanel> range))
        {
            // The whole rectangle takes the target cell's *new* state, so the same gesture both
            // selects (when target was unselected) and de-selects (when it was selected) a block.
            bool newState = !target.Selected;
            ApplyBulk(range, newState);

            // Keep the anchor so the user can grow/shrink the block with further Shift double-clicks.
            return;
        }

        target.Selected = !target.Selected;
        _anchor = target;
    }

    /// <summary>
    ///  Collects every registered panel whose cell falls inside the rectangle spanned by
    ///  <paramref name="anchor"/> and <paramref name="target"/>, provided both share the same parent
    ///  <see cref="TableLayoutPanel"/>. Cross-grid ranges are not defined, so this returns false and
    ///  the caller falls back to a plain toggle.
    /// </summary>
    private bool TryGetRange(SelectablePanel anchor, SelectablePanel target, out List<SelectablePanel> range)
    {
        range = [];

        if (anchor.Parent is not TableLayoutPanel grid || !ReferenceEquals(target.Parent, grid))
        {
            return false;
        }

        TableLayoutPanelCellPosition anchorCell = grid.GetPositionFromControl(anchor);
        TableLayoutPanelCellPosition targetCell = grid.GetPositionFromControl(target);
        if (anchorCell.Row < 0 || targetCell.Row < 0)
        {
            return false;
        }

        int minRow = Math.Min(anchorCell.Row, targetCell.Row);
        int maxRow = Math.Max(anchorCell.Row, targetCell.Row);
        int minColumn = Math.Min(anchorCell.Column, targetCell.Column);
        int maxColumn = Math.Max(anchorCell.Column, targetCell.Column);

        foreach (SelectablePanel panel in _panels)
        {
            if (!ReferenceEquals(panel.Parent, grid))
            {
                continue;
            }

            TableLayoutPanelCellPosition cell = grid.GetPositionFromControl(panel);
            if (cell.Row >= minRow && cell.Row <= maxRow && cell.Column >= minColumn && cell.Column <= maxColumn)
            {
                range.Add(panel);
            }
        }

        return range.Count > 0;
    }

    private void ApplyBulk(IEnumerable<SelectablePanel> panels, bool selected)
    {
        _suspendNotifications = true;
        try
        {
            foreach (SelectablePanel panel in panels)
            {
                panel.Selected = selected;
            }
        }
        finally
        {
            _suspendNotifications = false;
        }

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Panel_SelectedChanged(object? sender, EventArgs e)
    {
        if (_suspendNotifications)
        {
            return;
        }

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Moves <paramref name="target"/> into a fresh <see cref="SelectablePanel"/> while keeping its
    ///  place in the layout. Handles the common <see cref="TableLayoutPanel"/> case (cell + spans) and
    ///  falls back to a bounds-based swap for any other parent.
    /// </summary>
    private static SelectablePanel WrapInPlace(Control target)
    {
        Control? parent = target.Parent;
        SelectablePanel panel = new()
        {
            Name = $"{target.Name}SelectablePanel",
            Anchor = target.Anchor,
            Margin = target.Margin,
        };

        if (parent is TableLayoutPanel grid)
        {
            TableLayoutPanelCellPosition cell = grid.GetPositionFromControl(target);
            int columnSpan = grid.GetColumnSpan(target);
            int rowSpan = grid.GetRowSpan(target);

            grid.SuspendLayout();
            panel.SuspendLayout();

            grid.Controls.Remove(target);
            PlaceHosted(panel, target);
            grid.Controls.Add(panel, cell.Column, cell.Row);
            if (columnSpan > 1)
            {
                grid.SetColumnSpan(panel, columnSpan);
            }

            if (rowSpan > 1)
            {
                grid.SetRowSpan(panel, rowSpan);
            }

            panel.ResumeLayout(false);
            panel.PerformLayout();
            grid.ResumeLayout(true);
        }
        else if (parent is not null)
        {
            int childIndex = parent.Controls.GetChildIndex(target);
            panel.Bounds = target.Bounds;

            parent.SuspendLayout();
            panel.SuspendLayout();

            parent.Controls.Remove(target);
            PlaceHosted(panel, target);
            parent.Controls.Add(panel);
            parent.Controls.SetChildIndex(panel, childIndex);

            panel.ResumeLayout(false);
            panel.PerformLayout();
            parent.ResumeLayout(true);
        }
        else
        {
            PlaceHosted(panel, target);
        }

        return panel;
    }

    /// <summary>
    ///  Places <paramref name="target"/> at the panel's top-left inside its <see cref="Control.Padding"/>
    ///  so the AutoSize panel frames the control with the selection gap on every side.
    /// </summary>
    private static void PlaceHosted(SelectablePanel panel, Control target)
    {
        target.Margin = Padding.Empty;
        target.Dock = DockStyle.None;
        target.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        panel.Controls.Add(target);
        target.Location = new Point(panel.Padding.Left, panel.Padding.Top);
    }
}
