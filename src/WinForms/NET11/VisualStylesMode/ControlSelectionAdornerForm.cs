// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace VisualStylesModeDemo;

/// <summary>
///  A non-activating, per-pixel layered window that intercepts Edit-mode input without changing the
///  adorned control tree.
/// </summary>
internal sealed class ControlSelectionAdornerForm : Form
{
    private const int WsExLayered = 0x00080000;
    private const int WsExNoActivate = 0x08000000;
    private const int WsExToolWindow = 0x00000080;
    private const int WmMouseActivate = 0x0021;
    private const int MaNoActivate = 3;
    private const byte AcSrcOver = 0;
    private const byte AcSrcAlpha = 1;
    private const int UlwAlpha = 0x00000002;

    private readonly List<Control> _selectedControls = [];
    private Control? _target;
    private Control? _viewport;
    private Form? _ownerForm;
    private bool _editMode;
    private bool _synchronizing;
    private Color _accentColor = SystemColors.Highlight;

    public ControlSelectionAdornerForm()
    {
        AutoScaleMode = AutoScaleMode.None;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
    }

    public event EventHandler? SelectionChanged;

    public IReadOnlyList<Control> SelectedControls
    {
        get
        {
            PruneSelection();
            return _selectedControls.ToArray();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color AccentColor
    {
        get => _accentColor;
        set
        {
            if (_accentColor == value)
            {
                return;
            }

            _accentColor = value;
            RenderLayer();
        }
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ExStyle |= WsExLayered | WsExNoActivate | WsExToolWindow;
            return createParams;
        }
    }

    public void Activate(Form owner, Control target, Control viewport)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(viewport);

        if (!ReferenceEquals(_target, target) || !ReferenceEquals(_viewport, viewport))
        {
            ClearSelection();
            UnsubscribeGeometryEvents();
            _target = target;
            _viewport = viewport;
            _ownerForm = owner;
            SubscribeGeometryEvents();
        }

        _editMode = true;
        if (!Visible)
        {
            Show(owner);
        }

        SynchronizeBoundsAndRender();
    }

    public void DeactivateAndClear()
    {
        _editMode = false;
        ClearSelection();
        Hide();
    }

    public void SelectAll()
    {
        if (!_editMode || _target is null || _viewport is null)
        {
            return;
        }

        Rectangle viewportBounds = _viewport.RectangleToScreen(_viewport.ClientRectangle);
        List<Control> candidates = [];
        using (Region viewportRegion = new(viewportBounds))
        {
            CollectHitTestableControls(_target, viewportRegion, candidates);
        }

        _selectedControls.Clear();
        _selectedControls.AddRange(candidates);
        OnSelectionChanged();
    }

    public void ClearSelection()
    {
        if (_selectedControls.Count == 0)
        {
            RenderLayer();
            return;
        }

        _selectedControls.Clear();
        OnSelectionChanged();
    }

    public void SynchronizeBoundsAndRender()
    {
        if (_synchronizing || !_editMode || _target is null || _viewport is null || _ownerForm is null)
        {
            return;
        }

        try
        {
            _synchronizing = true;

            if (!_ownerForm.Visible
                || _ownerForm.WindowState == FormWindowState.Minimized
                || !_target.Visible
                || !_viewport.Visible)
            {
                Hide();
                return;
            }

            Rectangle targetBounds = _target.RectangleToScreen(_target.ClientRectangle);
            Rectangle viewportBounds = _viewport.RectangleToScreen(_viewport.ClientRectangle);
            Rectangle visibleBounds = Rectangle.Intersect(targetBounds, viewportBounds);

            if (visibleBounds.Width <= 0 || visibleBounds.Height <= 0)
            {
                Hide();
                return;
            }

            if (!Visible)
            {
                Show(_ownerForm);
            }

            Bounds = visibleBounds;
            RenderLayer();
        }
        finally
        {
            _synchronizing = false;
        }
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);

        if (e.Button != MouseButtons.Left || _target is null)
        {
            return;
        }

        // Use the physical cursor position rather than translating the layered Form's mouse
        // coordinates. Per-monitor DPI virtualization can otherwise apply a second conversion to
        // synthetic/layered-window coordinates and hit an ancestor instead of the visible leaf.
        Point screenPoint = Cursor.Position;
        if (!_target.RectangleToScreen(_target.ClientRectangle).Contains(screenPoint))
        {
            return;
        }

        Control selectedControl = FindDeepestControl(_target, screenPoint);
        bool additive = (ModifierKeys & Keys.Control) == Keys.Control;

        if (additive)
        {
            if (!_selectedControls.Remove(selectedControl))
            {
                _selectedControls.Add(selectedControl);
            }
        }
        else if (_selectedControls.Count == 1 && ReferenceEquals(_selectedControls[0], selectedControl))
        {
            _selectedControls.Clear();
        }
        else
        {
            _selectedControls.Clear();
            _selectedControls.Add(selectedControl);
        }

        OnSelectionChanged();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WmMouseActivate)
        {
            m.Result = MaNoActivate;
            return;
        }

        base.WndProc(ref m);
    }

    private static Control FindDeepestControl(Control control, Point screenPoint)
    {
        foreach (Control child in control.Controls)
        {
            if (!child.Visible || !GetControlScreenBounds(child).Contains(screenPoint))
            {
                continue;
            }

            return FindDeepestControl(child, screenPoint);
        }

        return control;
    }

    private static void CollectHitTestableControls(Control control, Region availableRegion, List<Control> candidates)
    {
        if (!control.Visible || control.IsDisposed)
        {
            return;
        }

        using Region visibleRegion = availableRegion.Clone();
        visibleRegion.Intersect(GetControlScreenBounds(control));
        if (!HasVisibleArea(visibleRegion))
        {
            return;
        }

        using Region childClip = visibleRegion.Clone();
        childClip.Intersect(control.RectangleToScreen(control.ClientRectangle));
        using Region exposedRegion = visibleRegion.Clone();
        using Region higherSiblingRegion = new();
        higherSiblingRegion.MakeEmpty();

        for (int index = 0; index < control.Controls.Count; index++)
        {
            Control child = control.Controls[index];
            if (!child.Visible || child.IsDisposed)
            {
                continue;
            }

            Rectangle childBounds = GetControlScreenBounds(child);
            using Region childRegion = new(childBounds);
            childRegion.Intersect(childClip);
            if (!HasVisibleArea(childRegion))
            {
                continue;
            }

            exposedRegion.Exclude(childRegion);

            // Controls index 0 is the front-most sibling. Remove everything already occupied by a
            // higher sibling before deciding whether this child can be the deepest hit.
            childRegion.Exclude(higherSiblingRegion);
            if (HasVisibleArea(childRegion))
            {
                CollectHitTestableControls(child, childRegion, candidates);
            }

            higherSiblingRegion.Union(childBounds);
        }

        if (HasVisibleArea(exposedRegion))
        {
            candidates.Add(control);
        }
    }

    private static bool HasVisibleArea(Region region)
    {
        using Matrix identity = new();
        return region.GetRegionScans(identity).Any(static area => area.Width >= 1F && area.Height >= 1F);
    }

    private void OnSelectionChanged()
    {
        RenderLayer();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void PruneSelection()
    {
        int removed = _selectedControls.RemoveAll(
            control => control.IsDisposed || !control.Visible || (_target is not null && !IsDescendantOrSelf(_target, control)));

        if (removed > 0)
        {
            RenderLayer();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private static bool IsDescendantOrSelf(Control root, Control candidate)
    {
        for (Control? current = candidate; current is not null; current = current.Parent)
        {
            if (ReferenceEquals(current, root))
            {
                return true;
            }
        }

        return false;
    }

    private void RenderLayer()
    {
        if (!IsHandleCreated || !Visible || Width <= 0 || Height <= 0)
        {
            return;
        }

        bool selectionPruned = PruneSelectionWithoutRendering();

        using Bitmap bitmap = new(Width, Height, PixelFormat.Format32bppPArgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.Clear(Color.FromArgb(1, 0, 0, 0));
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            float scale = DeviceDpi / 96F;
            float penWidth = Math.Max(2F, 2F * scale);
            using Pen pen = new(_accentColor, penWidth);
            using SolidBrush fill = new(Color.FromArgb(32, _accentColor));

            foreach (Control control in _selectedControls)
            {
                Rectangle screenBounds = GetControlScreenBounds(control);
                screenBounds.Intersect(Bounds);
                if (screenBounds.Width <= 0 || screenBounds.Height <= 0)
                {
                    continue;
                }

                screenBounds.Offset(-Left, -Top);
                graphics.FillRectangle(fill, screenBounds);

                float inset = penWidth / 2F;
                RectangleF frame = new(
                    screenBounds.X + inset,
                    screenBounds.Y + inset,
                    Math.Max(0F, screenBounds.Width - penWidth),
                    Math.Max(0F, screenBounds.Height - penWidth));
                graphics.DrawRectangle(pen, frame);
            }
        }

        UpdateLayeredBitmap(bitmap);

        if (selectionPruned)
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool PruneSelectionWithoutRendering() =>
        _selectedControls.RemoveAll(
            control => control.IsDisposed || !control.Visible || (_target is not null && !IsDescendantOrSelf(_target, control))) > 0;

    private static Rectangle GetControlScreenBounds(Control control)
    {
        if (control.Parent is Control parent)
        {
            return new Rectangle(parent.PointToScreen(control.Location), control.Size);
        }

        return control.RectangleToScreen(control.ClientRectangle);
    }

    private void UpdateLayeredBitmap(Bitmap bitmap)
    {
        nint screenDc = GetDC(0);
        nint memoryDc = CreateCompatibleDC(screenDc);
        nint bitmapHandle = 0;
        nint previousBitmap = 0;

        try
        {
            bitmapHandle = bitmap.GetHbitmap(Color.FromArgb(0));
            previousBitmap = SelectObject(memoryDc, bitmapHandle);

            NativePoint source = new(0, 0);
            NativePoint destination = new(Left, Top);
            NativeSize size = new(Width, Height);
            BlendFunction blend = new()
            {
                BlendOp = AcSrcOver,
                SourceConstantAlpha = 255,
                AlphaFormat = AcSrcAlpha,
            };

            if (!UpdateLayeredWindow(
                    Handle,
                    screenDc,
                    ref destination,
                    ref size,
                    memoryDc,
                    ref source,
                    0,
                    ref blend,
                    UlwAlpha))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            if (previousBitmap != 0)
            {
                SelectObject(memoryDc, previousBitmap);
            }

            if (bitmapHandle != 0)
            {
                DeleteObject(bitmapHandle);
            }

            DeleteDC(memoryDc);
            ReleaseDC(0, screenDc);
        }
    }

    private void SubscribeGeometryEvents()
    {
        if (_target is null || _viewport is null || _ownerForm is null)
        {
            return;
        }

        _target.Layout += GeometryChanged;
        _target.LocationChanged += GeometryChanged;
        _target.SizeChanged += GeometryChanged;
        _target.VisibleChanged += GeometryChanged;
        _viewport.Layout += GeometryChanged;
        _viewport.LocationChanged += GeometryChanged;
        _viewport.SizeChanged += GeometryChanged;
        _viewport.VisibleChanged += GeometryChanged;
        _ownerForm.LocationChanged += GeometryChanged;
        _ownerForm.SizeChanged += GeometryChanged;
        _ownerForm.VisibleChanged += GeometryChanged;

        if (_viewport is ScrollableControl scrollable)
        {
            scrollable.Scroll += Viewport_Scroll;
        }

        if (_target is ScrollableControl targetScrollable && !ReferenceEquals(targetScrollable, _viewport))
        {
            targetScrollable.Scroll += Viewport_Scroll;
        }
    }

    private void UnsubscribeGeometryEvents()
    {
        if (_target is not null)
        {
            _target.Layout -= GeometryChanged;
            _target.LocationChanged -= GeometryChanged;
            _target.SizeChanged -= GeometryChanged;
            _target.VisibleChanged -= GeometryChanged;

            if (_target is ScrollableControl targetScrollable && !ReferenceEquals(targetScrollable, _viewport))
            {
                targetScrollable.Scroll -= Viewport_Scroll;
            }
        }

        if (_viewport is not null)
        {
            _viewport.Layout -= GeometryChanged;
            _viewport.LocationChanged -= GeometryChanged;
            _viewport.SizeChanged -= GeometryChanged;
            _viewport.VisibleChanged -= GeometryChanged;

            if (_viewport is ScrollableControl scrollable)
            {
                scrollable.Scroll -= Viewport_Scroll;
            }
        }

        if (_ownerForm is not null)
        {
            _ownerForm.LocationChanged -= GeometryChanged;
            _ownerForm.SizeChanged -= GeometryChanged;
            _ownerForm.VisibleChanged -= GeometryChanged;
        }
    }

    private void GeometryChanged(object? sender, EventArgs e) => SynchronizeBoundsAndRender();

    private void Viewport_Scroll(object? sender, ScrollEventArgs e) => SynchronizeBoundsAndRender();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnsubscribeGeometryEvents();
        }

        base.Dispose(disposing);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeSize(int width, int height)
    {
        public int Width = width;
        public int Height = height;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UpdateLayeredWindow(
        nint window,
        nint destinationDc,
        ref NativePoint destination,
        ref NativeSize size,
        nint sourceDc,
        ref NativePoint source,
        int colorKey,
        ref BlendFunction blend,
        int flags);

    [DllImport("user32.dll")]
    private static extern nint GetDC(nint window);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint window, nint dc);

    [DllImport("gdi32.dll")]
    private static extern nint CreateCompatibleDC(nint dc);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteDC(nint dc);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint dc, nint objectHandle);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint objectHandle);
}
