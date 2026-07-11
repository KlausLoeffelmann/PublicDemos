// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  A <see cref="Panel"/> that hosts a single demo control and can be visually "selected" for the
///  shared <see cref="PropertyGrid"/> in <see cref="MainForm"/>. This replaces the old
///  per-control companion CheckBox: instead of checking a box, the user double-clicks a control
///  (or its surrounding panel) to toggle its <see cref="Selected"/> state.
/// </summary>
/// <remarks>
///  <para>
///   When <see cref="Selected"/> is <see langword="true"/> the panel paints a subtle, slightly
///   brighter (in DarkMode) or slightly darker (in light/Classic mode) background and draws a thin
///   accent rectangle inset by <see cref="Control.Padding"/>. The hosted control is expected to be
///   docked/anchored so the <see cref="Control.Padding"/> gap reveals that selection frame around it.
///  </para>
///  <para>
///   The panel itself only knows how to *show* selection and raise <see cref="SelectedChanged"/>;
///   the double-click wiring, the anchor/shift-range logic, and the "which controls are selected"
///   bookkeeping all live in <see cref="SelectionController"/> so a whole grid of panels behaves
///   consistently.
///  </para>
/// </remarks>
internal sealed class SelectablePanel : Panel
{
    // Kept as fields (rather than recreated per paint) only where cheap; pens/brushes that depend on
    // the live theme colors are created and disposed inside OnPaint so they always track DarkMode.
    private bool _selected;

    public SelectablePanel()
    {
        // UserPaint + double buffering give us a flicker-free custom selection frame; ResizeRedraw
        // makes sure the inset rectangle is repainted when the cell (and therefore the panel) resizes.
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
            true);

        // A uniform gap so the selection frame is drawn *around* the hosted control instead of on top
        // of it. This is the default; it can be changed at runtime via SelectionGap (View > Selection
        // Margin). AutoSize lets the panel hug its single child in the surrounding TableLayout.
        Padding = new Padding(DefaultSelectionGap);
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        TabStop = false;
    }

    /// <summary>The gap (in pixels) used between the hosted control and the selection frame by default.</summary>
    public const int DefaultSelectionGap = 10;

    /// <summary>
    ///  Gets or sets the uniform gap (in pixels) between the hosted control's chrome and the selection
    ///  frame. Backed by <see cref="Control.Padding"/>: changing it repositions the hosted control to
    ///  the new inset, lets the <see cref="Control.AutoSize"/> panel regrow around it, and repaints the
    ///  frame. Driven by the View &gt; Selection Margin menu in <see cref="MainForm"/>.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SelectionGap
    {
        get => Padding.Left;
        set
        {
            if (Padding.Left == value)
            {
                return;
            }

            Padding = new Padding(value);

            // Keep the hosted child parked at the new top-left inset so the gap is uniform on every
            // side; the AutoSize (GrowAndShrink) panel then resizes itself to child + 2*gap.
            if (HostedControl is Control hosted)
            {
                hosted.Location = new Point(value, value);
            }

            Invalidate();
        }
    }

    /// <summary>
    ///  Raised whenever <see cref="Selected"/> changes (whether via user interaction routed through
    ///  <see cref="SelectionController"/> or programmatically via Select All / Clear Selection).
    /// </summary>
    public event EventHandler? SelectedChanged;

    /// <summary>
    ///  Gets or sets whether this panel is currently part of the selection feeding the shared
    ///  <see cref="PropertyGrid"/>. Setting it repaints the panel and raises <see cref="SelectedChanged"/>.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false)]
    public bool Selected
    {
        get => _selected;
        set
        {
            if (_selected == value)
            {
                return;
            }

            _selected = value;
            Invalidate();
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///  The demo control this panel wraps (its single child), or <see langword="null"/> if empty.
    ///  Used by <see cref="SelectionController"/> to build the PropertyGrid's selected-objects list.
    /// </summary>
    public Control? HostedControl => Controls.Count > 0 ? Controls[0] : null;

    protected override void OnPaint(PaintEventArgs e)
    {
        // Paint the (possibly tinted) background first so the hosted child and the frame sit on top.
        Graphics graphics = e.Graphics;
        Color background = Selected ? GetSelectedBackColor() : BackColor;
        using (SolidBrush backgroundBrush = new(background))
        {
            graphics.FillRectangle(backgroundBrush, ClientRectangle);
        }

        base.OnPaint(e);

        if (!Selected)
        {
            return;
        }

        // Draw the accent frame inset by Padding so it frames the hosted control with a small gap.
        // Deflating by 1 extra pixel keeps the 2px-wide pen fully inside the client area.
        Rectangle frame = Rectangle.FromLTRB(
            Padding.Left - 3,
            Padding.Top - 3,
            Width - Padding.Right + 2,
            Height - Padding.Bottom + 2);
        frame.Intersect(new Rectangle(1, 1, Width - 2, Height - 2));

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using Pen accentPen = new(SystemColors.Highlight, 2f);
        graphics.DrawRectangle(accentPen, frame);
    }

    /// <summary>
    ///  Computes the tinted selection background: brighter than the base color in DarkMode, darker
    ///  in light/Classic mode, so the selection reads clearly against either theme.
    /// </summary>
    private Color GetSelectedBackColor()
    {
        Color baseColor = BackColor;

        // Application.IsDarkModeEnabled reflects the effective app color mode (System/Dark/Light).
        return Application.IsDarkModeEnabled
            ? ControlPaint.Light(baseColor, 0.35f)
            : ControlPaint.Dark(baseColor, 0.06f);
    }
}
