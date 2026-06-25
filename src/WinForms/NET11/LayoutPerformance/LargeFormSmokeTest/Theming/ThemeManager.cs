namespace LargeFormSmokeTest.Theming;

/// <summary>
///  Central theme coordinator. Wraps the new <see cref="Application.SetColorMode"/> dark-mode
///  API and exposes a small palette plus a <see cref="ThemeChanged"/> event so owner-drawn
///  surfaces (e.g. <c>ThemedDataGridView</c>) can re-apply their per-theme color scheme.
/// </summary>
public sealed class ThemeManager
{
    private AppTheme _theme;

    /// <summary>Initializes a new manager in the given <paramref name="theme"/>.</summary>
    public ThemeManager(AppTheme theme = AppTheme.Classic)
    {
        _theme = theme;
    }

    /// <summary>Raised after <see cref="Theme"/> changed.</summary>
    public event EventHandler? ThemeChanged;

    /// <summary>Gets a value indicating whether the dark theme is currently active.</summary>
    public bool IsDark
        => _theme is AppTheme.Dark;

    /// <summary>Gets or sets the active theme, applying it app-wide and notifying listeners.</summary>
    public AppTheme Theme
    {
        get => _theme;

        set
        {
            if (_theme == value)
            {
                return;
            }

            _theme = value;
            Apply();
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Applies the current theme app-wide via the new color-mode API. Safe to call during
    ///  startup (before <see cref="Application.Run(Form)"/>) and at runtime.
    /// </summary>
    public void Apply()
        => Application.SetColorMode(_theme is AppTheme.Dark ? SystemColorMode.Dark : SystemColorMode.Classic);

    /// <summary>Gets the grid background color for the active theme.</summary>
    public Color GridBackColor
        => _theme is AppTheme.Dark ? Color.FromArgb(32, 32, 32) : Color.White;

    /// <summary>Gets the alternating-row background color for the active theme.</summary>
    public Color GridAlternatingBackColor
        => _theme is AppTheme.Dark ? Color.FromArgb(45, 45, 48) : Color.FromArgb(245, 245, 248);

    /// <summary>Gets the grid foreground (text) color for the active theme.</summary>
    public Color GridForeColor
        => _theme is AppTheme.Dark ? Color.Gainsboro : Color.FromArgb(20, 20, 20);

    /// <summary>Gets the grid header background color for the active theme.</summary>
    public Color GridHeaderBackColor
        => _theme is AppTheme.Dark ? Color.FromArgb(56, 56, 60) : Color.FromArgb(230, 230, 235);

    /// <summary>Gets the grid header foreground color for the active theme.</summary>
    public Color GridHeaderForeColor
        => _theme is AppTheme.Dark ? Color.Gainsboro : Color.FromArgb(20, 20, 20);

    /// <summary>Gets the grid selection background color for the active theme.</summary>
    public Color GridSelectionBackColor
        => _theme is AppTheme.Dark ? Color.FromArgb(0, 90, 158) : Color.FromArgb(0, 120, 215);

    /// <summary>Gets the grid grid-line color for the active theme.</summary>
    public Color GridLineColor
        => _theme is AppTheme.Dark ? Color.FromArgb(64, 64, 68) : Color.FromArgb(221, 221, 226);

    /// <summary>Gets a glyph/icon foreground color that reads well on the active theme.</summary>
    public Color IconColor
        => _theme is AppTheme.Dark ? Color.Gainsboro : Color.FromArgb(40, 40, 40);

    /// <summary>
    ///  Returns a status accent color for an income-tax declaration status, tuned to remain
    ///  legible on both the classic and the dark theme.
    /// </summary>
    public Color StatusColor(Models.DeclarationStatus status)
        => (status, IsDark) switch
        {
            (Models.DeclarationStatus.Offen, false) => Color.FromArgb(176, 0, 32),
            (Models.DeclarationStatus.Offen, true) => Color.FromArgb(255, 120, 120),
            (Models.DeclarationStatus.Beglichen, false) => Color.FromArgb(0, 120, 40),
            (Models.DeclarationStatus.Beglichen, true) => Color.FromArgb(120, 220, 140),
            (Models.DeclarationStatus.Gestundet, false) => Color.FromArgb(170, 110, 0),
            (Models.DeclarationStatus.Gestundet, true) => Color.FromArgb(240, 200, 90),
            _ => GridForeColor
        };
}
