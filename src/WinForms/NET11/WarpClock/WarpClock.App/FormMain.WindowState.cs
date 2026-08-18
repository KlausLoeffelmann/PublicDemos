namespace WarpClock.App;

public partial class FormMain
{
    private static readonly Keys[] s_supportedFullScreenKeys =
    [
        Keys.Control | Keys.Return,
        Keys.Control | Keys.Shift | Keys.Return,
        Keys.F11,
        Keys.F12,
    ];

    private static readonly int[] s_supportedMouseHideDelays = [0, 1_000, 2_000, 5_000, 10_000];

    private WindowPresentationMode _presentationMode;
    private WindowPresentationMode _modeBeforeFullScreen;
    private Rectangle _lastWindowedBounds;
    private FormChromeState? _chromeState;
    private bool _windowStateApplied;
    private FormWindowState _windowedWindowState = FormWindowState.Normal;
    private int _propertyPanelWidth = 320;

    private void ApplyWindowSettings(PersistedWindowSettings? settings)
    {
        if (_windowStateApplied)
        {
            return;
        }

        _windowStateApplied = true;

        Screen screen = Screen.PrimaryScreen ?? Screen.FromControl(this);
        Rectangle defaultBounds = GetDefaultWindowedBounds(screen.WorkingArea);
        Rectangle savedBounds = settings?.GetWindowedBounds() ?? Rectangle.Empty;

        _lastWindowedBounds = IsSaneWindowedBounds(savedBounds) ? savedBounds : defaultBounds;
        _windowedWindowState = settings?.WindowState == FormWindowState.Maximized
            ? FormWindowState.Maximized
            : FormWindowState.Normal;
        _propertyPanelWidth = settings?.PropertyPanelWidth > 200
            ? settings.PropertyPanelWidth
            : 320;

        StartPosition = FormStartPosition.Manual;
        WindowState = FormWindowState.Normal;
        Bounds = _lastWindowedBounds;

        ApplyKioskSettings(settings);

        WindowPresentationMode mode = settings?.PresentationMode ?? WindowPresentationMode.Windowed;
        _presentationMode = WindowPresentationMode.Windowed;

        if (mode == WindowPresentationMode.NoChrome)
        {
            EnterNoChromeMode();
            return;
        }

        if (mode == WindowPresentationMode.FullScreen)
        {
            _modeBeforeFullScreen = WindowPresentationMode.Windowed;
            _kioskModeManager.FullScreen = true;
            return;
        }

        if (_windowedWindowState == FormWindowState.Maximized)
        {
            WindowState = FormWindowState.Maximized;
        }

        RefreshKioskChecks();
    }

    private PersistedWindowSettings CaptureWindowSettings()
    {
        CaptureWindowedBounds();

        if (!_splitContainer.Panel2Collapsed && _splitContainer.Panel2.Width > 0)
        {
            _propertyPanelWidth = _splitContainer.Panel2.Width;
        }

        return new PersistedWindowSettings
        {
            X = _lastWindowedBounds.X,
            Y = _lastWindowedBounds.Y,
            Width = _lastWindowedBounds.Width,
            Height = _lastWindowedBounds.Height,
            WindowState = _windowedWindowState,
            PresentationMode = _kioskModeManager.FullScreen
                ? WindowPresentationMode.FullScreen
                : _presentationMode,
            ToggleFullScreenKeys = _kioskModeManager.ToggleFullScreenKeys,
            AlwaysOn = _kioskModeManager.AlwaysOn,
            RecordFramerate = _recordFramerateEnabled,
            EscapeExitsFullScreen = _kioskModeManager.EscapeExitsFullScreen,
            MousePointerAutoHideDelay = _kioskModeManager.MousePointerAutoHideDelay,
            TopMostInFullScreen = _kioskModeManager.TopMostInFullScreen,
            PropertyPanelWidth = _propertyPanelWidth,
        };
    }

    private void ApplyKioskSettings(PersistedWindowSettings? settings)
    {
        Keys toggleKeys = settings is not null && s_supportedFullScreenKeys.Contains(settings.ToggleFullScreenKeys)
            ? settings.ToggleFullScreenKeys
            : Keys.Control | Keys.Shift | Keys.Return;

        int mouseHideDelay = settings is not null && s_supportedMouseHideDelays.Contains(settings.MousePointerAutoHideDelay)
            ? settings.MousePointerAutoHideDelay
            : 5_000;

        _kioskModeManager.ToggleFullScreenKeys = toggleKeys;
        _kioskModeManager.AlwaysOn = settings?.AlwaysOn ?? false;
        _kioskModeManager.EscapeExitsFullScreen = settings?.EscapeExitsFullScreen ?? true;
        _kioskModeManager.MousePointerAutoHideDelay = mouseHideDelay;
        _kioskModeManager.TopMostInFullScreen = settings?.TopMostInFullScreen ?? true;
        RefreshKioskChecks();
    }

    private static Rectangle GetDefaultWindowedBounds(Rectangle workingArea)
    {
        int width = Math.Max(1, (int)Math.Round(workingArea.Width * 0.8));
        int height = Math.Max(1, (int)Math.Round(workingArea.Height * 0.8));
        int x = workingArea.Left + ((workingArea.Width - width) / 2);
        int y = workingArea.Top + ((workingArea.Height - height) / 2);
        return new Rectangle(x, y, width, height);
    }

    private static bool IsSaneWindowedBounds(Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return false;
        }

        foreach (Screen screen in Screen.AllScreens)
        {
            Rectangle workingArea = screen.WorkingArea;
            int minimumWidth = Math.Min(800, Math.Max(1, (int)Math.Round(workingArea.Width * 0.8)));
            int minimumHeight = Math.Min(600, Math.Max(1, (int)Math.Round(workingArea.Height * 0.8)));

            if (bounds.Width >= minimumWidth
                && bounds.Height >= minimumHeight
                && workingArea.Contains(bounds))
            {
                return true;
            }
        }

        return false;
    }

    private void CaptureWindowedBounds()
    {
        if (_presentationMode != WindowPresentationMode.Windowed || _kioskModeManager.FullScreen)
        {
            return;
        }

        if (WindowState != FormWindowState.Minimized)
        {
            _windowedWindowState = WindowState;
        }

        Rectangle bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        if (IsSaneWindowedBounds(bounds))
        {
            _lastWindowedBounds = bounds;
        }
    }

    private void ApplyPropertyPanelWidth()
    {
        if (_splitContainer.Width <= 0)
        {
            return;
        }

        int panelWidth = Math.Clamp(_propertyPanelWidth, 200, Math.Max(200, _splitContainer.Width - 200));
        int splitterDistance = Math.Max(100, _splitContainer.Width - panelWidth - _splitContainer.SplitterWidth);
        _splitContainer.SplitterDistance = splitterDistance;
    }

    private void OnSplitContainerSplitterMoved(object? sender, SplitterEventArgs e)
    {
        if (!_splitContainer.Panel2Collapsed && _splitContainer.Panel2.Width > 0)
        {
            _propertyPanelWidth = _splitContainer.Panel2.Width;
        }
    }

    private void EnterNoChromeMode()
    {
        if (_presentationMode == WindowPresentationMode.NoChrome)
        {
            return;
        }

        if (_kioskModeManager.FullScreen)
        {
            _kioskModeManager.FullScreen = false;
        }

        CaptureWindowedBounds();
        _chromeState = this.HideWindowsChrome(_kioskModeManager.TopMostInFullScreen);
        _presentationMode = WindowPresentationMode.NoChrome;
        _miHideWindowsChrome.Checked = true;
        _statusMode.Text = "No chrome";
    }

    private void ExitNoChromeMode()
    {
        if (_presentationMode != WindowPresentationMode.NoChrome)
        {
            return;
        }

        FormChromeState state = _chromeState ?? new FormChromeState(
            FormBorderStyle.Sizable,
            _windowedWindowState,
            _lastWindowedBounds,
            TopMost: false);

        this.RestoreWindowsChrome(state);
        _chromeState = null;
        _presentationMode = WindowPresentationMode.Windowed;
        _miHideWindowsChrome.Checked = false;
        _statusMode.Text = "Windowed";
    }
}
