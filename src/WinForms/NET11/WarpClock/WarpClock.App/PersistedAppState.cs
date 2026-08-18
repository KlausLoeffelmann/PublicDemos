using WarpClock.Engine;

namespace WarpClock.App;

/// <summary>
///  Versioned persisted UI state for the WarpClock host.
/// </summary>
public sealed class PersistedAppState
{
    public const int CurrentSchemaVersion = 3;

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    public PersistedWindowSettings Window { get; set; } = new();

    public PersistedClockSettings Clock { get; set; } = new();

    public PersistedThemeState Theme { get; set; } = new();

    public void Normalize()
    {
        SchemaVersion = CurrentSchemaVersion;
        Window ??= new PersistedWindowSettings();
        Clock ??= new PersistedClockSettings();
        Theme ??= new PersistedThemeState();

        Window.Normalize();
        Clock.Normalize();
        Theme.Normalize();
    }
}

public sealed class PersistedWindowSettings
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

    public WindowPresentationMode PresentationMode { get; set; } = WindowPresentationMode.Windowed;

    public Keys ToggleFullScreenKeys { get; set; } = Keys.Control | Keys.Shift | Keys.Return;

    public bool AlwaysOn { get; set; }

    public bool RecordFramerate { get; set; }

    public bool EscapeExitsFullScreen { get; set; } = true;

    public int MousePointerAutoHideDelay { get; set; } = 5_000;

    public bool TopMostInFullScreen { get; set; } = true;

    public int PropertyPanelWidth { get; set; } = 320;

    public void Normalize()
    {
        if (WindowState == FormWindowState.Minimized)
        {
            WindowState = FormWindowState.Normal;
        }

        PropertyPanelWidth = Math.Max(200, PropertyPanelWidth);
    }

    public Rectangle GetWindowedBounds() => new(X, Y, Width, Height);
}

public sealed class PersistedClockSettings
{
    public bool HasUserState { get; set; }

    public ClockHandMotion SecondMotion { get; set; } = ClockHandMotion.Crawling;

    public ClockHandMotion MinuteMotion { get; set; } = ClockHandMotion.Crawling;

    public ClockHandMotion HourMotion { get; set; } = ClockHandMotion.Crawling;

    public int GraceSeconds { get; set; } = 5;

    public float GlideDurationSeconds { get; set; } = 0.5f;

    public bool MagneticNumerals { get; set; }

    public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;

    public double SpeedMultiplier { get; set; } = 1d;

    public RenderThemeInfo RenderThemeInfo { get; set; } = RenderThemeInfo.FadeAlternateScreenSides;

    public ThemeInfoPlacement ThemeInfoPlacement { get; set; } = ThemeInfoPlacement.LeftScreenSide;

    public bool OledView { get; set; }

    public bool VSyncEnabled { get; set; } = true;

    public double TargetFrameRate { get; set; } = 60d;

    public void Normalize()
    {
        GraceSeconds = Math.Clamp(GraceSeconds, 1, 30);
        GlideDurationSeconds = Math.Clamp(GlideDurationSeconds, 0.1f, 5f);
        TargetFrameRate = TargetFrameRate > 0d ? TargetFrameRate : 60d;
    }
}

public sealed class PersistedThemeState
{
    public ThemeReference? CurrentTheme { get; set; }

    public string? CurrentThemeListPath { get; set; }

    public string? DefaultThemeListPath { get; set; }

    public void Normalize()
    {
        ThemeReferenceUtility.Normalize(CurrentTheme);

        if (CurrentTheme is not null && string.IsNullOrWhiteSpace(CurrentTheme.ThemeKey))
        {
            CurrentTheme = null;
        }

        CurrentThemeListPath = NormalizePath(CurrentThemeListPath);
        DefaultThemeListPath = NormalizePath(DefaultThemeListPath);
    }

    private static string? NormalizePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        return path.Trim();
    }
}
