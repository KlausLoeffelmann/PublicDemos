using System.Diagnostics;
using System.Text.Json;

namespace WarpClock.App;

public partial class FormMain
{
    private static readonly JsonSerializerOptions s_windowSettingsJsonOptions = new()
    {
        WriteIndented = true,
    };

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
    private bool _windowSettingsRestored;

    private void RestoreWindowSettings()
    {
        if (_windowSettingsRestored)
        {
            return;
        }

        _windowSettingsRestored = true;
        WindowSettings? settings = TryLoadWindowSettings(out string? error);

        Screen screen = Screen.PrimaryScreen ?? Screen.FromControl(this);
        Rectangle defaultBounds = GetDefaultWindowedBounds(screen.WorkingArea);
        Rectangle savedBounds = settings?.GetWindowedBounds() ?? Rectangle.Empty;
        _lastWindowedBounds = IsSaneWindowedBounds(savedBounds) ? savedBounds : defaultBounds;

        StartPosition = FormStartPosition.Manual;
        WindowState = FormWindowState.Normal;
        Bounds = _lastWindowedBounds;

        ApplyKioskSettings(settings);

        if (settings?.Mode == WindowPresentationMode.NoChrome)
        {
            EnterNoChromeMode();
        }
        else if (settings?.Mode == WindowPresentationMode.FullScreen)
        {
            _modeBeforeFullScreen = WindowPresentationMode.Windowed;
            _kioskModeManager.FullScreen = true;
        }

        if (error is not null)
        {
            _statusInfo.Text = error;
        }
    }

    private void ApplyKioskSettings(WindowSettings? settings)
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

    private void SaveWindowSettings()
    {
        CaptureWindowedBounds();

        WindowSettings settings = new()
        {
            X = _lastWindowedBounds.X,
            Y = _lastWindowedBounds.Y,
            Width = _lastWindowedBounds.Width,
            Height = _lastWindowedBounds.Height,
            Mode = _kioskModeManager.FullScreen
                ? WindowPresentationMode.FullScreen
                : _presentationMode,
            ToggleFullScreenKeys = _kioskModeManager.ToggleFullScreenKeys,
            AlwaysOn = _kioskModeManager.AlwaysOn,
            EscapeExitsFullScreen = _kioskModeManager.EscapeExitsFullScreen,
            MousePointerAutoHideDelay = _kioskModeManager.MousePointerAutoHideDelay,
            TopMostInFullScreen = _kioskModeManager.TopMostInFullScreen,
        };

        try
        {
            string path = GetWindowSettingsPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(settings, s_windowSettingsJsonOptions));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            Debug.WriteLine($"Could not save WarpClock window settings: {ex.Message}");
        }
    }

    private static WindowSettings? TryLoadWindowSettings(out string? error)
    {
        try
        {
            string path = GetWindowSettingsPath();
            error = null;

            return File.Exists(path)
                ? JsonSerializer.Deserialize<WindowSettings>(File.ReadAllText(path), s_windowSettingsJsonOptions)
                : null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            error = $"Window settings could not be restored: {ex.Message}";
            return null;
        }
    }

    private static string GetWindowSettingsPath() =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WarpClock",
            "window.json");

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

        Rectangle bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        if (IsSaneWindowedBounds(bounds))
        {
            _lastWindowedBounds = bounds;
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
            FormWindowState.Normal,
            _lastWindowedBounds,
            TopMost: false);

        this.RestoreWindowsChrome(state);
        _chromeState = null;
        _presentationMode = WindowPresentationMode.Windowed;
        _miHideWindowsChrome.Checked = false;
        _statusMode.Text = "Windowed";
    }

    private sealed class WindowSettings
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public WindowPresentationMode Mode { get; set; }

        public Keys ToggleFullScreenKeys { get; set; } = Keys.Control | Keys.Shift | Keys.Return;

        public bool AlwaysOn { get; set; }

        public bool EscapeExitsFullScreen { get; set; } = true;

        public int MousePointerAutoHideDelay { get; set; } = 5_000;

        public bool TopMostInFullScreen { get; set; } = true;

        public Rectangle GetWindowedBounds() => new(X, Y, Width, Height);
    }

    private enum WindowPresentationMode
    {
        Windowed,
        FullScreen,
        NoChrome,
    }
}
