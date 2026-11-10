// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VisualStylesModeDemo;

/// <summary>
///  A borderless, inheritable form with a Windows 11-style, client-painted frame and title bar.
/// </summary>
/// <remarks>
///  Dock content normally; DisplayRectangle excludes the chrome. Absolute control coordinates
///  remain client coordinates. Windows supplies resizing, moving, snapping, and the system menu.
/// </remarks>
public partial class Win11FormBaseClass : Form
{
    internal const int HtMinButton = 8;
    internal const int HtMaxButton = 9;
    internal const int HtClose = 20;
    internal static readonly int[] CaptionButtons = [HtMinButton, HtMaxButton, HtClose];

    private const int HtClient = 1;
    private const int HtCaption = 2;
    private const int HtSysMenu = 3;
    private const int WmSysCommand = 0x0112;
    private const int ScSize = 0xF000;
    private const int ScMove = 0xF010;
    private const int ScMinimize = 0xF020;
    private const int ScMaximize = 0xF030;
    private const int ScClose = 0xF060;
    private const int ScRestore = 0xF120;

    private Font _captionFont = SystemFonts.CaptionFont ?? SystemFonts.DefaultFont;
    private bool _active;
    private bool _resizable = true;
    private int _hotButton;
    private int _pressedButton;

    /// <summary>Creates a form with Windows 11-style custom chrome.</summary>
    public Win11FormBaseClass()
    {
        InitializeComponent();
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
        Disposed += Form_Disposed;
    }

    /// <summary>Gets the fixed border style used by this custom-chrome form.</summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DefaultValue(FormBorderStyle.None)]
    public new FormBorderStyle FormBorderStyle
    {
        get => base.FormBorderStyle;

        set
        {
            if (value != FormBorderStyle.None)
            {
                throw new ArgumentException("Custom chrome requires FormBorderStyle.None.", nameof(value));
            }

            base.FormBorderStyle = value;
        }
    }

    /// <summary>Gets or sets whether the window supports resizing and maximizing.</summary>
    [DefaultValue(true)]
    [Category("Window Style")]
    [Description("Allows native edge resizing and maximizing of the custom-chrome window.")]
    public bool Resizable
    {
        get => _resizable;
        set
        {
            if (_resizable == value)
            {
                return;
            }

            _resizable = value;
            if (IsHandleCreated)
            {
                RecreateHandle();
            }

            PerformLayout();
            Invalidate();
        }
    }

    /// <inheritdoc />
    public override Rectangle DisplayRectangle
    {
        get
        {
            Rectangle bounds = base.DisplayRectangle;
            int border = Resizable && WindowState == FormWindowState.Normal ? Scale(8) : BorderThickness;

            return new Rectangle(bounds.Left + border, bounds.Top + TitleBarHeight,
                Math.Max(0, bounds.Width - 2 * border),
                Math.Max(0, bounds.Height - TitleBarHeight - border));
        }
    }

    internal int TitleBarHeight => Math.Max(Scale(32), Scale(_captionFont.Height + 12));
    private int BorderThickness => WindowState == FormWindowState.Maximized ? 0 : 1;
    private bool Mirrored => RightToLeft == RightToLeft.Yes && RightToLeftLayout;
    private bool InDesigner => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    internal Rectangle IconBounds
    {
        get
        {
            int size = Scale(16);
            return Mirror(new Rectangle(Scale(8), (TitleBarHeight - size) / 2, size, size));
        }
    }

    internal Rectangle TitleBounds
    {
        get
        {
            int left = Scale(ShowIcon && ControlBox ? 32 : 12);

            int right = ControlBox ? GetCaptionButtonBounds(MinimizeBox || MaximizeBox ? HtMinButton : HtClose).Left
                : ClientSize.Width - Scale(12);

            if (Mirrored && ControlBox)
            {
                right = ClientSize.Width - GetCaptionButtonBounds(MinimizeBox || MaximizeBox ? HtMinButton : HtClose).Right;
            }

            return Mirror(new Rectangle(left, 0, Math.Max(0, right - left - Scale(8)), TitleBarHeight));
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (!InDesigner)
        {
            // Apply capabilities to the HWND, not CreateParams: WinForms uses CreateParams to
            // calculate ClientSize and would otherwise subtract a frame that we don't have.
            Marshal.SetLastPInvokeError(0);
            int previous = SetWindowLong(Handle, -16, NativeWindowStyle(GetWindowLong(Handle, -16)));

            if (previous == 0 && Marshal.GetLastPInvokeError() != 0)
            {
                throw new Win32Exception(Marshal.GetLastPInvokeError());
            }

            if (!SetWindowPos(Handle, 0, 0, 0, 0, 0, 0x0037)) // SWP_FRAMECHANGED, no move/size/activation/z-order.
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        ApplyDwmAttributes();
    }

    private int NativeWindowStyle(int style)
    {
        style &= ~(0x00C00000 | 0x00040000 | 0x00080000 | 0x00020000 | 0x00010000);
        style |= 0x00C00000; // WS_CAPTION preserves native system-menu and minimize/restore semantics.
        if (Resizable) style |= 0x00040000; // WS_THICKFRAME
        if (ControlBox) style |= 0x00080000; // WS_SYSMENU
        if (ControlBox && MinimizeBox) style |= 0x00020000;
        if (ControlBox && MaximizeBox && Resizable) style |= 0x00010000;

        return style;
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Win11ChromeRenderer.Draw(e.Graphics, this, _captionFont, _active, _hotButton, _pressedButton);
    }

    /// <inheritdoc />
    protected override void OnActivated(EventArgs e)
    {
        _active = true;
        Invalidate();
        base.OnActivated(e);
    }

    /// <inheritdoc />
    protected override void OnDeactivate(EventArgs e)
    {
        _active = false;
        ResetCaptionTracking();
        base.OnDeactivate(e);
    }

    /// <inheritdoc />
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        PerformLayout();
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        RefreshChrome();
    }

    /// <inheritdoc />
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Alt | Keys.Space) && ControlBox)
        {
            ShowSystemMenu(PointToScreen(new Point(Mirrored
                ? ClientSize.Width
                : 0, TitleBarHeight)));

            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <inheritdoc />
    protected override void WndProc(ref Message m)
    {
        if (InDesigner)
        {
            base.WndProc(ref m);
            return;
        }

        switch (m.Msg)
        {
            case 0x007C when (int)m.WParam == -16: // WM_STYLECHANGING, GWL_STYLE
                Marshal.WriteInt32(m.LParam, sizeof(int), NativeWindowStyle(Marshal.ReadInt32(m.LParam, sizeof(int))));
                break;

            case 0x007D: // WM_STYLECHANGED

            case 0x0080: // WM_SETICON (also used by ShowIcon)
                base.WndProc(ref m);
                PerformLayout();
                Invalidate();
                return;

            case 0x0317: // WM_PRINT: never let DefWindowProc print a second, native title bar.
                m.LParam = (nint)((long)m.LParam & ~2L); // Remove PRF_NONCLIENT.
                break;

            case 0x0083: // WM_NCCALCSIZE: all pixels belong to our client-painted chrome/content.
                if (m.WParam != 0 && IsZoomed(Handle))
                {
                    // Windows positions a maximized thick frame outside the work area. Keep
                    // that invisible resize border out of our visible client-painted caption.
                    NativeRect bounds = Marshal.PtrToStructure<NativeRect>(m.LParam);
                    MonitorInfo monitor = new() { Size = Marshal.SizeOf<MonitorInfo>() };
                    if (!GetMonitorInfo(MonitorFromRect(ref bounds, 2), ref monitor))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    bounds.Left = Math.Max(bounds.Left, monitor.Work.Left);
                    bounds.Top = Math.Max(bounds.Top, monitor.Work.Top);
                    bounds.Right = Math.Min(bounds.Right, monitor.Work.Right);
                    bounds.Bottom = Math.Min(bounds.Bottom, monitor.Work.Bottom);
                    Marshal.StructureToPtr(bounds, m.LParam, false);
                }

                m.Result = 0;
                return;
            case 0x0084: // WM_NCHITTEST
                m.Result = HitTest(PointToClient(ScreenPoint(m.LParam)));
                return;
            case 0x0085: // WM_NCPAINT
                m.Result = 0;
                return;
            case 0x0086: // WM_NCACTIVATE: preserve activation semantics without native frame painting.
                m.LParam = -1;
                base.WndProc(ref m);
                Invalidate();
                return;
            case 0x0024: // WM_GETMINMAXINFO
                base.WndProc(ref m);
                UpdateMinMaxInfo(m.LParam);
                return;
            case 0x00A0: // WM_NCMOUSEMOVE
                UpdateHotButton(PointToClient(ScreenPoint(m.LParam)));

                TrackMouseEventData tracking = new()
                {
                    Size = (uint)Marshal.SizeOf<TrackMouseEventData>(),
                    Flags = 0x12, // TME_LEAVE | TME_NONCLIENT
                    Window = Handle
                };

                if (!TrackMouseEvent(ref tracking))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                break;
            case 0x02A2: // WM_NCMOUSELEAVE
                SetHotButton(0);
                break;
            case 0x00A1: // WM_NCLBUTTONDOWN
            case 0x00A3: // WM_NCLBUTTONDBLCLK
                int hit = (int)m.WParam;
                if (CaptionButtons.Contains(hit))
                {
                    if (IsCaptionButtonEnabled(hit))
                    {
                        _pressedButton = hit;
                        SetHotButton(hit);
                        Capture = true;
                        Invalidate();
                    }

                    m.Result = 0;
                    return;
                }

                if (hit == HtSysMenu && ControlBox)
                {
                    if (m.Msg == 0x00A3)
                    {
                        ExecuteCaptionButton(HtClose);
                    }
                    else
                    {
                        Rectangle icon = IconBounds;
                        ShowSystemMenu(PointToScreen(new Point(Mirrored ? icon.Right : icon.Left, icon.Bottom)));
                    }

                    return;
                }

                if (hit == HtCaption && m.Msg == 0x00A3 && (!MaximizeBox || !Resizable))
                {
                    return;
                }

                break;
            case 0x0200 when _pressedButton != 0: // WM_MOUSEMOVE while captured
                UpdateHotButton(PointToClient(Cursor.Position));
                return;
            case 0x0202 when _pressedButton != 0: // WM_LBUTTONUP
                int pressed = _pressedButton;
                bool invoke = GetCaptionButtonBounds(pressed).Contains(PointToClient(Cursor.Position));
                ResetCaptionTracking();
                if (invoke)
                {
                    ExecuteCaptionButton(pressed);
                }

                return;
            case 0x0215: // WM_CAPTURECHANGED
                _pressedButton = 0;
                SetHotButton(0);
                break;
            case 0x00A5 when (int)m.WParam is HtCaption or HtSysMenu: // WM_NCRBUTTONUP
                if (ControlBox)
                {
                    ShowSystemMenu(ScreenPoint(m.LParam));
                }

                return;
            case 0x001A: // WM_SETTINGCHANGE
            case 0x031A: // WM_THEMECHANGED
                base.WndProc(ref m);
                RefreshChrome();
                return;
        }

        base.WndProc(ref m);
    }

    internal Rectangle GetCaptionButtonBounds(int hit)
    {
        if (!ControlBox || (hit != HtClose && !MinimizeBox && !MaximizeBox))
        {
            return Rectangle.Empty;
        }

        int index = hit switch { HtClose => 0, HtMaxButton => 1, HtMinButton => 2, _ => -1 };
        if (index < 0)
        {
            return Rectangle.Empty;
        }

        int width = Scale(46);
        int border = BorderThickness;

        return Mirror(new Rectangle(ClientSize.Width - border - (index + 1) * width, border,
            width, TitleBarHeight - border));
    }

    internal bool IsCaptionButtonEnabled(int hit) => Enabled && ControlBox && hit switch
    {
        HtMinButton => MinimizeBox,
        HtMaxButton => MaximizeBox && Resizable,
        HtClose => true,
        _ => false
    };

    private int HitTest(Point point)
    {
        if (!ClientRectangle.Contains(point))
        {
            return HtClient;
        }

        if (Resizable && WindowState == FormWindowState.Normal)
        {
            int grip = Scale(8);
            bool left = point.X < grip;
            bool right = point.X >= ClientSize.Width - grip;
            bool top = point.Y < grip;
            bool bottom = point.Y >= ClientSize.Height - grip;
            // Caption buttons reach the top edge, as they do on the native Windows 11 frame.
            bool button = CaptionButtons.Any(hit => GetCaptionButtonBounds(hit).Contains(point));
            if (top && left) return 13; // HTTOPLEFT
            if (top && right) return 14;
            if (bottom && left) return 16;
            if (bottom && right) return 17;
            if (left) return 10;
            if (right) return 11;
            if (top && !button) return 12;
            if (bottom) return 15;
        }

        foreach (int hit in CaptionButtons)
        {
            if (GetCaptionButtonBounds(hit).Contains(point))
            {
                // A disabled maximize target must not advertise Snap Layouts to the shell.
                return IsCaptionButtonEnabled(hit) ? hit : HtClient;
            }
        }

        if (ControlBox && ShowIcon && IconBounds.Contains(point)) return HtSysMenu;
        return point.Y < TitleBarHeight ? HtCaption : HtClient;
    }

    private void UpdateHotButton(Point point)
    {
        int hit = HitTest(point);
        SetHotButton(IsCaptionButtonEnabled(hit) ? hit : 0);
    }

    private void SetHotButton(int hit)
    {
        if (_hotButton != hit)
        {
            _hotButton = hit;
            Invalidate();
        }
    }

    private void ResetCaptionTracking()
    {
        _pressedButton = 0;
        Capture = false;
        _hotButton = 0;
        Invalidate();
    }

    private void ExecuteCaptionButton(int hit)
    {
        if (!IsCaptionButtonEnabled(hit))
        {
            return;
        }

        int command = hit switch
        {
            HtMinButton => ScMinimize,
            HtMaxButton => WindowState == FormWindowState.Maximized ? ScRestore : ScMaximize,
            _ => ScClose
        };
        PostSystemCommand(command);
    }

    private void ShowSystemMenu(Point location)
    {
        nint menu = GetSystemMenu(Handle, false);
        if (menu == 0)
        {
            throw new Win32Exception("The window has no system menu.");
        }

        bool normal = WindowState == FormWindowState.Normal;
        SetMenuEnabled(menu, ScRestore, !normal);
        SetMenuEnabled(menu, ScMove, normal);
        SetMenuEnabled(menu, ScSize, normal && Resizable);
        SetMenuEnabled(menu, ScMinimize, MinimizeBox && WindowState != FormWindowState.Minimized);
        SetMenuEnabled(menu, ScMaximize, MaximizeBox && Resizable && WindowState != FormWindowState.Maximized);

        int command = TrackPopupMenuEx(menu, 0x0100u | 0x0002u | (Mirrored ? 0x0008u : 0u),
            location.X, location.Y, Handle, 0); // TPM_RETURNCMD | TPM_RIGHTBUTTON

        if (command != 0)
        {
            PostSystemCommand(command);
        }
    }

    private static void SetMenuEnabled(nint menu, int command, bool enabled)
    {
        if (EnableMenuItem(menu, (uint)command, enabled ? 0u : 1u) == uint.MaxValue)
        {
            throw new Win32Exception($"Could not update system menu command 0x{command:X}.");
        }
    }

    private void PostSystemCommand(int command)
    {
        if (!PostMessage(Handle, WmSysCommand, command, 0))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    private void UpdateMinMaxInfo(nint address)
    {
        MinMaxInfo info = Marshal.PtrToStructure<MinMaxInfo>(address);
        MonitorInfo monitor = new() { Size = Marshal.SizeOf<MonitorInfo>() };
        if (!GetMonitorInfo(MonitorFromWindow(Handle, 2), ref monitor))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        info.MaxPosition = new Point(monitor.Work.Left - monitor.Monitor.Left, monitor.Work.Top - monitor.Monitor.Top);
        int width = monitor.Work.Right - monitor.Work.Left;
        int height = monitor.Work.Bottom - monitor.Work.Top;

        info.MaxSize = new Point(MaximumSize.Width > 0 ? Math.Min(width, MaximumSize.Width) : width,
            MaximumSize.Height > 0 ? Math.Min(height, MaximumSize.Height) : height);

        info.MinTrackSize.X = Math.Max(info.MinTrackSize.X, Scale(ControlBox ? MinimizeBox || MaximizeBox ? 200 : 108 : 64));
        info.MinTrackSize.Y = Math.Max(info.MinTrackSize.Y, TitleBarHeight + Scale(16));
        Marshal.StructureToPtr(info, address, false);
    }

    private void RefreshChrome()
    {
        _captionFont.Dispose();
        _captionFont = SystemFonts.CaptionFont ?? SystemFonts.DefaultFont;
        if (IsHandleCreated)
        {
            ApplyDwmAttributes();
        }

        PerformLayout();
        Invalidate();
    }

    private void ApplyDwmAttributes()
    {
        if (InDesigner || !OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            return;
        }

        int dark = Application.IsDarkModeEnabled && !SystemInformation.HighContrast ? 1 : 0;
        Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(Handle, 20, ref dark, sizeof(int)));
        int corners = 2; // DWMWCP_ROUND; DWM removes rounding automatically when maximized/snapped.
        Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(Handle, 33, ref corners, sizeof(int)));
        Margins margins = new() { Left = 1, Right = 1, Top = 1, Bottom = 1 };
        Marshal.ThrowExceptionForHR(DwmExtendFrameIntoClientArea(Handle, ref margins));
    }

    private int Scale(int value) => (int)Math.Round(value * DeviceDpi / 96d);
    private Rectangle Mirror(Rectangle bounds) => Mirrored
        ? new Rectangle(ClientSize.Width - bounds.Right, bounds.Top, bounds.Width, bounds.Height) : bounds;
    private static Point ScreenPoint(nint value) => new(unchecked((short)(long)value), unchecked((short)((long)value >> 16)));
    private void Form_Disposed(object? sender, EventArgs e) => _captionFont.Dispose();

    /// <inheritdoc />
    protected override AccessibleObject CreateAccessibilityInstance() => new ChromeAccessibleObject(this);

    private sealed class ChromeAccessibleObject(Win11FormBaseClass owner) : ControlAccessibleObject(owner)
    {
        private readonly Dictionary<int, CaptionAccessibleObject> _buttons = [];
        private int ContentChildCount => owner.Controls.Count;
        public override int GetChildCount() => ContentChildCount + VisibleButtons.Length;
        private int[] VisibleButtons => CaptionButtons.Where(hit => !owner.GetCaptionButtonBounds(hit).IsEmpty).ToArray();

        private CaptionAccessibleObject GetButton(int hit)
        {
            if (!_buttons.TryGetValue(hit, out CaptionAccessibleObject? button))
            {
                button = new CaptionAccessibleObject(owner, this, hit);
                _buttons.Add(hit, button);
            }

            return button;
        }

        public override AccessibleObject? GetChild(int index)
        {
            int count = ContentChildCount;
            return index >= 0 && index < count ? owner.Controls[index].AccessibilityObject
                : index - count < VisibleButtons.Length && index >= count
                ? GetButton(VisibleButtons[index - count]) : null;
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            foreach (int hit in VisibleButtons)
            {
                if (owner.RectangleToScreen(owner.GetCaptionButtonBounds(hit)).Contains(x, y))
                {
                    return GetButton(hit);
                }
            }

            return base.HitTest(x, y);
        }
    }

    private sealed class CaptionAccessibleObject(Win11FormBaseClass owner, AccessibleObject parent, int hit) : AccessibleObject
    {
        public override string? Name
        {
            get => base.Name ?? hit switch
            {
                HtMinButton => "Minimize",
                HtMaxButton => owner.WindowState == FormWindowState.Maximized ? "Restore" : "Maximize",
                _ => "Close"
            };
            set => base.Name = value;
        }

        public override string DefaultAction => "Press";
        public override AccessibleRole Role => AccessibleRole.PushButton;
        public override AccessibleObject Parent => parent;
        public override Rectangle Bounds => owner.RectangleToScreen(owner.GetCaptionButtonBounds(hit));
        public override AccessibleStates State => owner.IsCaptionButtonEnabled(hit) ? AccessibleStates.None : AccessibleStates.Unavailable;
        public override void DoDefaultAction() => owner.ExecuteCaptionButton(hit);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public Point Reserved;
        public Point MaxSize;
        public Point MaxPosition;
        public Point MinTrackSize;
        public Point MaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public NativeRect Monitor, Work;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TrackMouseEventData
    {
        public uint Size, Flags;
        public nint Window;
        public uint HoverTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Margins
    {
        public int Left, Right, Top, Bottom;
    }

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int value, int size);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmExtendFrameIntoClientArea(nint hwnd, ref Margins margins);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TrackMouseEvent(ref TrackMouseEventData data);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint MonitorFromWindow(nint hwnd, uint flags);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint MonitorFromRect(ref NativeRect bounds, uint flags);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsZoomed(nint hwnd);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", ExactSpelling = true)]
    private static extern int GetWindowLong(nint hwnd, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", ExactSpelling = true, SetLastError = true)]
    private static extern int SetWindowLong(nint hwnd, int index, int value);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(nint hwnd, nint insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(nint monitor, ref MonitorInfo info);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetSystemMenu(nint hwnd, [MarshalAs(UnmanagedType.Bool)] bool revert);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern uint EnableMenuItem(nint menu, uint item, uint enable);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern int TrackPopupMenuEx(nint menu, uint flags, int x, int y, nint hwnd, nint parameters);

    [DllImport("user32.dll", EntryPoint = "PostMessageW", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessage(nint hwnd, int message, nint wParam, nint lParam);
}