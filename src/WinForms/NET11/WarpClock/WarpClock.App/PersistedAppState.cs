using System.Text.Json.Serialization;
using WarpClock.Engine;

namespace WarpClock.App;

/// <summary>
///  Versioned persisted UI state for the WarpClock host.
/// </summary>
public sealed class PersistedAppState
{
    public const int CurrentSchemaVersion = 6;

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    public PersistedWindowSettings Window { get; set; } = new();

    public PersistedClockSettings Clock { get; set; } = new();

    public PersistedThemeState Theme { get; set; } = new();

    public WarpClockOptions Options { get; set; } = new();

    public void Normalize()
    {
        int loadedSchemaVersion = SchemaVersion;
        SchemaVersion = CurrentSchemaVersion;
        Window ??= new PersistedWindowSettings();
        Clock ??= new PersistedClockSettings();
        Theme ??= new PersistedThemeState();
        Options ??= new WarpClockOptions();

        if (loadedSchemaVersion < 6)
        {
            Options.Hands = new HandOptions
            {
                HourMotion = Clock.HourMotion,
                MinuteMotion = Clock.MinuteMotion,
                SecondMotion = Clock.SecondMotion,
                GraceSeconds = Clock.GraceSeconds,
            };
        }

        Window.Normalize();
        Clock.Normalize();
        Theme.Normalize();
        Options.Normalize();
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

    public string? CurrentThemeSetPath { get; set; }

    public string? DefaultThemeSetPath { get; set; }

    public List<PersistedThemeCustomPropertyValue> CustomPropertyValues { get; set; } = [];

    [JsonPropertyName("CurrentThemeListPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegacyCurrentThemeListPath
    {
        get => null;
        set
        {
            if (string.IsNullOrWhiteSpace(CurrentThemeSetPath))
            {
                CurrentThemeSetPath = value;
            }
        }
    }

    [JsonPropertyName("DefaultThemeListPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegacyDefaultThemeListPath
    {
        get => null;
        set
        {
            if (string.IsNullOrWhiteSpace(DefaultThemeSetPath))
            {
                DefaultThemeSetPath = value;
            }
        }
    }

    public void Normalize()
    {
        ThemeReferenceUtility.Normalize(CurrentTheme);

        if (CurrentTheme is not null && string.IsNullOrWhiteSpace(CurrentTheme.ThemeKey))
        {
            CurrentTheme = null;
        }

        CurrentThemeSetPath = NormalizePath(CurrentThemeSetPath);
        DefaultThemeSetPath = NormalizePath(DefaultThemeSetPath);
        CustomPropertyValues ??= [];
        CustomPropertyValues = NormalizeCustomPropertyValues(CustomPropertyValues);
    }

    private static List<PersistedThemeCustomPropertyValue> NormalizeCustomPropertyValues(
        IEnumerable<PersistedThemeCustomPropertyValue> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        Dictionary<string, PersistedThemeCustomPropertyValue> normalized = new(StringComparer.OrdinalIgnoreCase);
        foreach (PersistedThemeCustomPropertyValue value in values)
        {
            if (value is null)
            {
                continue;
            }

            value.Normalize();
            if (string.IsNullOrWhiteSpace(value.ThemeKey) || string.IsNullOrWhiteSpace(value.PropertyName))
            {
                continue;
            }

            normalized[CreateCompositeKey(value.ThemeKey, value.PropertyName)] = value;
        }

        return normalized.Values
            .OrderBy(value => value.ThemeKey, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value.PropertyName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string CreateCompositeKey(string themeKey, string propertyName)
        => $"{themeKey}\u001f{propertyName}";

    private static string? NormalizePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        return path.Trim();
    }
}

public sealed class PersistedThemeCustomPropertyValue
{
    public string ThemeKey { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public void Normalize()
    {
        ThemeKey = ThemeCatalogInfo.NormalizeThemeKey(ThemeKey);
        PropertyName = PropertyName?.Trim() ?? string.Empty;
        Value ??= string.Empty;
    }
}
