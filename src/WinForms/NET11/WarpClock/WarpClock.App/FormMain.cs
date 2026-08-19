using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WarpClock.Abstractions;
using WarpClock.Engine;

namespace WarpClock.App;

/// <summary>
///  The kiosk demo host: shows a <see cref="WarpClockControl"/> filling the window,
///  drives it with stock and drop-in plug-in themes, and demonstrates the .NET 11
///  <see cref="KioskModeManager"/> component for fullscreen / power / wake handling.
/// </summary>
public partial class FormMain : Form
{
    private readonly bool _designMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    private readonly List<ThemeEntry> _themes = [];
    private readonly List<ThemeMenuBinding> _themeMenuItems = [];
    private readonly ClockSettingsView _clockSettings;
    private readonly ThemePropertyGridAdapter _propertyGridSelection;
    private readonly ThemeCustomPropertyStore _themeCustomPropertyStore = new();
    private readonly SemaphoreSlim _loadGate = new(1, 1);

    private AppPaths _appPaths;
    private ThemePluginLoader _pluginLoader;
    private AppStateStore? _appStateStore;
    private ThemeSetStore? _themeSetStore;
    private AppExceptionRouter? _exceptionRouter;
    private StartupOptions _startupOptions = StartupOptions.Empty;
    private ILogger<FormMain> _logger = NullLogger<FormMain>.Instance;
    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private PictureFolderCatalog _pictureCatalog = new(NullLogger<PictureFolderCatalog>.Instance);
    private FileSystemWatcher? _pluginWatcher;
    private ThemeEntry? _current;
    private ThemeSelection? _currentSelection;
    private ThemeCustomPropertySession? _activeThemeCustomProperties;
    private ClockThemeVariantKind _currentResolvedVariant = ClockThemeVariantKind.Day;
    private Font? _stripFont;

    public FormMain()
    {
        _appPaths = new AppPaths();
        _pluginLoader = new ThemePluginLoader(_appPaths.PluginDirectory, NullLogger<ThemePluginLoader>.Instance);

        InitializeComponent();

        _clockSettings = new ClockSettingsView(this, _clock);
        _propertyGridSelection = new ThemePropertyGridAdapter(_clockSettings);
        _themeScheduleTimer = new System.Windows.Forms.Timer();
        _themeScheduleTimer.Tick += OnThemeScheduleTimerTick;
        _timeZoneTimer = new System.Windows.Forms.Timer
        {
            Interval = 1_000,
        };
        _timeZoneTimer.Tick += OnTimeZoneTimerTick;
        _propertyGrid.PropertyValueChanged += OnClockSettingsPropertyValueChanged;
        _splitContainer.SplitterMoved += OnSplitContainerSplitterMoved;
        RefreshPropertyGridSelection();

        if (_designMode)
        {
            return;
        }

        _clock.GraceSeconds = 5;
        ApplySystemTextScaleToStrips();
        RefreshAllSettingChecks();
        LoadStockThemes();

        if (_themes.Count > 0)
        {
            SelectTheme(new ThemeSelection(_themes[0], explicitVariant: null), ThemeSelectionReason.DefaultStartup, applyThemeDefaults: true);
        }

        _fpsTimer.Start();
    }

    public FormMain(
        AppPaths appPaths,
        ThemePluginLoader pluginLoader,
        AppStateStore appStateStore,
        ThemeSetStore themeSetStore,
        PictureFolderCatalog pictureCatalog,
        AppExceptionRouter exceptionRouter,
        StartupOptions startupOptions,
        ILogger<FormMain> logger,
        ILoggerFactory loggerFactory)
        : this()
    {
        ArgumentNullException.ThrowIfNull(appPaths);
        ArgumentNullException.ThrowIfNull(pluginLoader);
        ArgumentNullException.ThrowIfNull(appStateStore);
        ArgumentNullException.ThrowIfNull(themeSetStore);
        ArgumentNullException.ThrowIfNull(pictureCatalog);
        ArgumentNullException.ThrowIfNull(exceptionRouter);
        ArgumentNullException.ThrowIfNull(startupOptions);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _appPaths = appPaths;
        _pluginLoader = pluginLoader;
        _appStateStore = appStateStore;
        _themeSetStore = themeSetStore;
        _pictureCatalog = pictureCatalog;
        _exceptionRouter = exceptionRouter;
        _startupOptions = startupOptions;
        _logger = logger;
        _loggerFactory = loggerFactory;

        _exceptionRouter.Start(this, ReportRecoverableExceptionStatus);
    }

    /// <summary>
    ///  Deferred, awaitable initialization. By the time <see cref="OnLoad"/> runs the
    ///  window handle exists, so heavy plug-in discovery can happen on a worker thread
    ///  and UI updates can be marshaled safely back through <see cref="Control.InvokeAsync"/>.
    /// </summary>
    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_designMode)
        {
            return;
        }

        try
        {
            RestorePersistedState();
            await LoadPluginsAsync(initial: true);
            LoadThemeSchedule();
            ApplyLoadedClockState();

            ThemeSelection startupSelection = ResolveStartupThemeSelection(out ThemeSelectionReason reason);
            SelectTheme(
                startupSelection,
                reason,
                applyThemeDefaults: !HasLoadedClockState && ShouldApplyThemeDefaultsOnThemeChange());
            StartDiagnosticsIfRequested();
            EnablePluginWatcher();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WarpClock startup failed.");
            _statusInfo.Text = $"Startup failed: {ex.Message}";
        }
    }

    protected override void OnSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs e)
    {
        base.OnSystemVisualSettingsChanged(e);

        if (e.Changed == SystemVisualSettingsCategories.TextScale)
        {
            ApplySystemTextScaleToStrips();
        }
    }

    private void ApplySystemTextScaleToStrips()
    {
        Font baseFont = Font;
        float scaledSize = baseFont.SizeInPoints
            * (float)Application.SystemVisualSettings.TextScaleFactor;
        Font newFont = new(baseFont.FontFamily, scaledSize, baseFont.Style, GraphicsUnit.Point);
        Font? oldFont = _stripFont;

        _stripFont = newFont;
        _menuStrip.Font = newFont;
        _statusStrip.Font = newFont;

        oldFont?.Dispose();
    }

    private void ReportRecoverableExceptionStatus(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _statusInfo.Text = message;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (!e.Cancel)
        {
            PersistCurrentAppState();
        }
    }

    protected override void OnResizeEnd(EventArgs e)
    {
        base.OnResizeEnd(e);
        CaptureWindowedBounds();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _pluginWatcher?.Dispose();
        _themeScheduleTimer?.Dispose();
        _timeZoneTimer?.Dispose();
        _loadGate.Dispose();
        StopDiagnostics();
        _exceptionRouter?.Stop();
        _stripFont?.Dispose();
        _stripFont = null;
        base.OnFormClosed(e);
    }

    private void LoadStockThemes()
    {
        foreach (Func<IClockTheme> factory in s_stockThemeFactories)
        {
            AddThemeEntry(factory(), StockThemeSource);
        }

        RebuildThemeMenuItems();
    }

    private void AddThemeEntry(IClockTheme familyTheme, string source)
    {
        string familyName = GetThemeFamilyName(familyTheme);
        ThemeCatalogInfo catalog = new()
        {
            ThemeKey = ThemeCatalogInfo.CreateThemeKey(source, familyName, familyTheme.GetType()),
            FamilyName = familyName,
            Source = source,
            SupportedVariants = familyTheme.SupportedVariants,
        };

        ThemeEntry? existing = _themes.FirstOrDefault(candidate =>
            ThemeCatalogInfo.ThemeKeysMatch(candidate.Catalog.ThemeKey, catalog.ThemeKey));

        if (existing is not null)
        {
            if (string.Equals(existing.Catalog.Source, StockThemeSource, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(source, StockThemeSource, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug(
                    "Ignoring plug-in theme family {ThemeKey} from {Source} because the stock theme already owns that logical id.",
                    catalog.ThemeKey,
                    source);
                return;
            }

            existing.Update(familyTheme, catalog);
            _logger.LogDebug("Updated theme family {ThemeKey}.", catalog.ThemeKey);
            return;
        }

        ThemeEntry entry = new(familyTheme, catalog);
        _themes.Add(entry);
    }

    private void RebuildThemeMenuItems()
    {
        while (_themeMenu.DropDownItems.Count > 0)
        {
            _themeMenu.DropDownItems.RemoveAt(0);
        }

        _themeMenuItems.Clear();

        foreach (ThemeEntry entry in _themes)
        {
            AddThemeMenuItems(entry);
        }

        if (_current is not null)
        {
            UpdateThemeChecks(_current, _currentResolvedVariant);
        }
    }

    private void AddThemeMenuItems(ThemeEntry entry)
    {
        if (entry.Catalog.SupportedVariants.Count > 1)
        {
            ToolStripMenuItem familyMenu = new(entry.Catalog.FamilyName);

            foreach (ClockThemeVariantKind variant in entry.Catalog.SupportedVariants)
            {
                ToolStripMenuItem variantItem = new(ClockThemeVariants.GetLabel(variant))
                {
                    Tag = new ThemeSelection(entry, variant),
                };

                variantItem.Click += OnThemeSelected;
                familyMenu.DropDownItems.Add(variantItem);
                _themeMenuItems.Add(new ThemeMenuBinding((ThemeSelection)variantItem.Tag, variantItem));
            }

            _themeMenu.DropDownItems.Add(familyMenu);
            return;
        }

        ToolStripMenuItem item = new(entry.Catalog.FamilyName)
        {
            Tag = new ThemeSelection(entry, explicitVariant: null),
        };
        item.Click += OnThemeSelected;
        _themeMenu.DropDownItems.Add(item);
        _themeMenuItems.Add(new ThemeMenuBinding((ThemeSelection)item.Tag, item));
    }

    private void OnThemeSelected(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: ThemeSelection selection })
        {
            SelectTheme(selection, ThemeSelectionReason.Manual, ShouldApplyThemeDefaultsOnThemeChange());
        }
    }

    private void SelectTheme(ThemeSelection selection, ThemeSelectionReason reason, bool applyThemeDefaults)
    {
        ThemeSchedulePeriod period = GetCurrentThemePeriod();
        bool preferOledVariants = GetOledViewEnabled();
        ClockThemeVariantKind resolvedVariant = selection.Entry.Catalog.ResolveVariant(selection.ExplicitVariant, period, preferOledVariants);
        IClockTheme concreteTheme = selection.Entry.ResolveTheme(selection.ExplicitVariant, period, preferOledVariants);
        ThemeCustomPropertySession customProperties = ThemeCustomPropertySession.Create(
            selection.Entry.Catalog.ThemeKey,
            concreteTheme,
            _themeCustomPropertyStore,
            _logger);
        bool themeChanged = !ReferenceEquals(_current, selection.Entry)
            || _currentResolvedVariant != resolvedVariant;

        _current = selection.Entry;
        _currentSelection = selection;
        _activeThemeCustomProperties = customProperties;
        _currentResolvedVariant = resolvedVariant;
        ApplyThemeToClock(customProperties.Theme);
        ApplyEffectiveThemeInfoMode();

        if (applyThemeDefaults)
        {
            _clock.MagneticNumerals = customProperties.Theme.Capabilities.MagneticByDefault;
        }

        _miMagnetic.Checked = _clock.MagneticNumerals;
        UpdateThemeChecks(selection.Entry, _currentResolvedVariant);

        RefreshPropertyGridSelection();

        _statusInfo.Text = $"Theme: {customProperties.Theme.Name} ({selection.Entry.Catalog.Source})"
            + (customProperties.Theme.Capabilities.FreeFloating ? " — free-floating" : string.Empty)
            + (_clock.MagneticNumerals ? " — magnetic" : string.Empty);

        _logger.LogInformation(
            "Selected theme {ThemeName} from {Source}. Reason={Reason}",
            customProperties.Theme.Name,
            selection.Entry.Catalog.Source,
            reason);

        if (themeChanged)
        {
            RestartFrameRateRecordingForThemeChange(reason);
        }

        RefreshRuntimePresentation();
    }

    private void UpdateThemeChecks(ThemeEntry selectedEntry, ClockThemeVariantKind resolvedVariant)
    {
        foreach (ThemeMenuBinding binding in _themeMenuItems)
        {
            bool sameEntry = ReferenceEquals(binding.Selection.Entry, selectedEntry);
            bool shouldCheck = sameEntry
                && (selectedEntry.Catalog.SupportedVariants.Count == 1
                    || binding.Selection.ExplicitVariant == resolvedVariant);

            binding.Item.Checked = shouldCheck;
        }
    }

    private static string GetThemeFamilyName(IClockTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        if (theme.SupportedVariants.Count > 1)
        {
            foreach (ClockThemeVariantKind variant in theme.SupportedVariants)
            {
                string suffix = " - " + ClockThemeVariants.GetLabel(variant);
                if (theme.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    return theme.Name[..^suffix.Length];
                }
            }
        }

        return theme.Name;
    }

    private IReadOnlyList<ThemeCatalogInfo> GetThemeCatalogSnapshot()
        => _themes.Select(entry => entry.Catalog).ToList();

    private bool TryResolveThemeReference(ThemeReference? reference, out ThemeSelection? selection)
    {
        selection = null;
        if (reference is null || string.IsNullOrWhiteSpace(reference.ThemeKey))
        {
            return false;
        }

        ThemeReferenceUtility.Normalize(reference);
        ThemeEntry? entry = _themes.FirstOrDefault(candidate =>
            ThemeCatalogInfo.ThemeKeysMatch(candidate.Catalog.ThemeKey, reference.ThemeKey));

        if (entry is null)
        {
            return false;
        }

        ClockThemeVariantKind? explicitVariant = reference.Variant;
        if (explicitVariant is ClockThemeVariantKind variant && !entry.Catalog.SupportsVariant(variant))
        {
            explicitVariant = null;
        }

        selection = new ThemeSelection(entry, explicitVariant);
        return true;
    }

    private bool TryResolveThemeToken(string token, out ThemeSelection? selection)
    {
        selection = null;

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        string trimmed = token.Trim();
        string normalizedToken = ThemeCatalogInfo.NormalizeThemeKey(trimmed);
        foreach (ThemeEntry entry in _themes)
        {
            if (ThemeCatalogInfo.ThemeKeysMatch(entry.Catalog.ThemeKey, normalizedToken)
                || string.Equals(entry.Catalog.FamilyName, trimmed, StringComparison.OrdinalIgnoreCase))
            {
                selection = new ThemeSelection(entry, explicitVariant: null);
                return true;
            }

            foreach (ClockThemeVariantKind variant in entry.Catalog.SupportedVariants)
            {
                string display = entry.Catalog.GetConcreteDisplayName(variant, ThemeSchedulePeriod.Day);
                if (string.Equals(display, trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    selection = new ThemeSelection(entry, variant);
                    return true;
                }
            }
        }

        return false;
    }

    // ── Plug-in loading / hot-loading ──

    private async Task LoadPluginsAsync(bool initial)
    {
        await _loadGate.WaitAsync();

        try
        {
            if (IsHandleCreated)
            {
                await InvokeAsync(() => _statusInfo.Text = "Scanning for plug-in themes…");
            }

            IReadOnlyList<DiscoveredTheme> found = await Task.Run(_pluginLoader.LoadNew);

            if (found.Count == 0)
            {
                if (!initial && IsHandleCreated)
                {
                    await InvokeAsync(() => _statusInfo.Text = "No new plug-in themes found.");
                }

                return;
            }

            if (IsHandleCreated)
            {
                await InvokeAsync(() =>
                {
                    bool changed = false;
                    foreach (DiscoveredTheme discovered in found)
                    {
                        AddThemeEntry(discovered.Theme, discovered.SourceFile);
                        changed = true;
                    }

                    if (changed)
                    {
                        RebuildThemeMenuItems();
                    }

                    if (_themeSchedule is not null)
                    {
                        ThemeSelection? scheduled = TryGetScheduledThemeSelection();
                        if (scheduled is not null)
                        {
                            SelectTheme(scheduled, ThemeSelectionReason.PluginReload, ShouldApplyThemeDefaultsOnThemeChange());
                        }

                        ScheduleNextThemeRotation();
                    }
                    else if (_currentSelection is not null)
                    {
                        SelectTheme(_currentSelection, ThemeSelectionReason.PluginReload, applyThemeDefaults: false);
                    }

                    int added = found.Count;
                    _statusInfo.Text = $"Loaded or reloaded {added} plug-in theme family{(added == 1 ? string.Empty : "s")}.";
                });
            }
        }
        finally
        {
            _loadGate.Release();
        }
    }

    private void OnReloadPluginsClick(object? sender, EventArgs e) => _ = ReloadPluginsAsync();

    private void EnablePluginWatcher()
    {
        try
        {
            _pluginWatcher?.Dispose();
            _pluginWatcher = null;
            Directory.CreateDirectory(_pluginLoader.PluginDirectory);

            _pluginWatcher = new FileSystemWatcher(_pluginLoader.PluginDirectory, "*.dll")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            };
            _pluginWatcher.Created += OnPluginFileChanged;
            _pluginWatcher.Changed += OnPluginFileChanged;
            _pluginWatcher.EnableRaisingEvents = true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(ex, "Could not enable the plug-in watcher for {Path}.", _pluginLoader.PluginDirectory);
        }
    }

    private void OnPluginFileChanged(object sender, FileSystemEventArgs e) => _ = ReloadPluginsAsync();

    private async Task ReloadPluginsAsync()
    {
        try
        {
            await LoadPluginsAsync(initial: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Plug-in reload failed.");

            if (IsHandleCreated)
            {
                try
                {
                    await InvokeAsync(() => _statusInfo.Text = $"Plug-in reload failed: {ex.Message}");
                }
                catch (Exception invokeEx)
                {
                    _logger.LogWarning(invokeEx, "Could not report the plug-in reload failure on the UI thread.");
                }
            }
        }
    }

    // ── Speed menu handlers ──

    private void OnSpeedClick(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: double speed })
        {
            if (speed == 1d)
            {
                _clock.ResetTimeAcceleration();
            }

            _clock.SpeedMultiplier = speed;
            MarkClockSettingsCustomized();
            _statusInfo.Text = $"Speed: {speed:0}x";
        }
    }

    private void RefreshSpeedChecks()
    {
        _miSpeed1.Checked = _clock.SpeedMultiplier == 1d;
        _miSpeed10.Checked = _clock.SpeedMultiplier == 10d;
        _miSpeed60.Checked = _clock.SpeedMultiplier == 60d;
        _miSpeed600.Checked = _clock.SpeedMultiplier == 600d;
    }

    private void OnOptionsClick(object? sender, EventArgs e)
    {
        SynchronizeHandOptionsFromClock();
        using OptionsDialog dialog = new(_options);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _options = dialog.EditedOptions.Clone();
        ApplyRuntimeOptions(reloadPluginsForFolderChange: true);
        PersistCurrentAppState();
        _statusInfo.Text = "Options updated.";
    }

    // ── View menu handlers ──

    private void OnKioskClick(object? sender, EventArgs e) => _kioskModeManager.ToggleFullScreen();

    private void RefreshKioskChecks()
    {
        _miKiosk.Checked = _kioskModeManager.FullScreen;
        _miAlwaysOn.Checked = _kioskModeManager.AlwaysOn;
        _miAllowEscape.Checked = _kioskModeManager.EscapeExitsFullScreen;
        _miTopMostInFullScreen.Checked = _kioskModeManager.TopMostInFullScreen;
        _miHideWindowsChrome.Checked = _presentationMode == WindowPresentationMode.NoChrome;

        Keys keys = _kioskModeManager.ToggleFullScreenKeys;
        _miToggleControlEnter.Checked = keys == (Keys.Control | Keys.Return);
        _miToggleControlShiftEnter.Checked = keys == (Keys.Control | Keys.Shift | Keys.Return);
        _miToggleF11.Checked = keys == Keys.F11;
        _miToggleF12.Checked = keys == Keys.F12;

        int delay = _kioskModeManager.MousePointerAutoHideDelay;
        _miMouseHideNever.Checked = delay == 0;
        _miMouseHide1000.Checked = delay == 1_000;
        _miMouseHide2000.Checked = delay == 2_000;
        _miMouseHide5000.Checked = delay == 5_000;
        _miMouseHide10000.Checked = delay == 10_000;
    }

    private void OnKioskMenuOpening(object? sender, EventArgs e) => RefreshKioskChecks();

    private void OnFullScreenToggleKeysClick(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: Keys keys })
        {
            _kioskModeManager.ToggleFullScreenKeys = keys;
            RefreshKioskChecks();
            _statusInfo.Text = $"Full-screen toggle keys: {keys}";
        }
    }

    private void OnAlwaysOnClick(object? sender, EventArgs e)
    {
        _kioskModeManager.AlwaysOn = !_kioskModeManager.AlwaysOn;
        _miAlwaysOn.Checked = _kioskModeManager.AlwaysOn;
        _statusInfo.Text = $"Always on: {(_kioskModeManager.AlwaysOn ? "On" : "Off")}";
    }

    private void OnAllowEscapeClick(object? sender, EventArgs e)
    {
        _kioskModeManager.EscapeExitsFullScreen = !_kioskModeManager.EscapeExitsFullScreen;
        _miAllowEscape.Checked = _kioskModeManager.EscapeExitsFullScreen;
        _statusInfo.Text = $"Escape exits kiosk mode: {(_kioskModeManager.EscapeExitsFullScreen ? "On" : "Off")}";
    }

    private void OnMousePointerHideDelayClick(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: int delay })
        {
            _kioskModeManager.MousePointerAutoHideDelay = delay;
            RefreshKioskChecks();
            _statusInfo.Text = delay == 0
                ? "Mouse pointer auto-hide: Off"
                : $"Mouse pointer auto-hide: {delay:N0} ms";
        }
    }

    private void OnTopMostInFullScreenClick(object? sender, EventArgs e)
    {
        _kioskModeManager.TopMostInFullScreen = !_kioskModeManager.TopMostInFullScreen;
        _miTopMostInFullScreen.Checked = _kioskModeManager.TopMostInFullScreen;

        if (_presentationMode == WindowPresentationMode.NoChrome)
        {
            TopMost = _kioskModeManager.TopMostInFullScreen;
        }

        _statusInfo.Text = $"Topmost in full screen: {(_kioskModeManager.TopMostInFullScreen ? "On" : "Off")}";
    }

    private void OnHideWindowsChromeClick(object? sender, EventArgs e)
    {
        if (_presentationMode == WindowPresentationMode.NoChrome)
        {
            ExitNoChromeMode();
        }
        else
        {
            EnterNoChromeMode();
        }
    }

    private void OnMagneticClick(object? sender, EventArgs e)
    {
        _clock.MagneticNumerals = !_clock.MagneticNumerals;
        MarkClockSettingsCustomized();
        _statusInfo.Text = $"Magnetic numerals: {(_clock.MagneticNumerals ? "On" : "Off")}";
    }

    private void OnVSyncClick(object? sender, EventArgs e)
    {
        _clock.VSyncEnabled = !_clock.VSyncEnabled;
        MarkClockSettingsCustomized();
        _statusInfo.Text = $"VSync: {(_clock.VSyncEnabled ? "On" : "Off")}";
    }

    private void OnFpsTimerTick(object? sender, EventArgs e)
    {
        _statusFps.Text = $"{_clock.CurrentFramesPerSecond:0} fps";
        RecordCurrentFrameRateSample();
        RefreshTickerText();
    }

    // ── Theme-info overlay menu ──

    private void OnThemeInfoModeClick(object? sender, EventArgs e)
    {
        _preferredThemeInfoMode = ThemeInfoModeFor(sender);
        ApplyEffectiveThemeInfoMode();
        MarkClockSettingsCustomized();
        _statusInfo.Text = $"Theme info preference: {_preferredThemeInfoMode}";
    }

    private RenderThemeInfo ThemeInfoModeFor(object? sender)
    {
        if (ReferenceEquals(sender, _miInfoNever)) return RenderThemeInfo.Never;
        if (ReferenceEquals(sender, _miInfoFixed)) return RenderThemeInfo.FixedPosition;
        if (ReferenceEquals(sender, _miInfoFadeFixed)) return RenderThemeInfo.FadeInAndOutAtFixedPosition;
        return RenderThemeInfo.FadeAlternateScreenSides;
    }

    private void OnThemeInfoOpening(object? sender, EventArgs e)
    {
        RefreshThemeInfoMenuState();
    }

    private void OnThemeInfoPlacementClick(object? sender, EventArgs e)
    {
        ThemeInfoPlacement placement = ThemeInfoPlacementFor(sender);
        _clock.ThemeInfoPlacement = placement;
        MarkClockSettingsCustomized();
        _statusInfo.Text = $"Theme info placement: {placement}";
    }

    private ThemeInfoPlacement ThemeInfoPlacementFor(object? sender)
    {
        if (ReferenceEquals(sender, _miPlaceRight)) return ThemeInfoPlacement.RightScreenSide;
        if (ReferenceEquals(sender, _miPlaceFace)) return ThemeInfoPlacement.OnClockFace;
        return ThemeInfoPlacement.LeftScreenSide;
    }

    private void OnThemeInfoPlacementOpening(object? sender, EventArgs e)
    {
        ThemeInfoPlacement placement = _clock.ThemeInfoPlacement;
        _miPlaceLeft.Checked = placement == ThemeInfoPlacement.LeftScreenSide;
        _miPlaceRight.Checked = placement == ThemeInfoPlacement.RightScreenSide;
        _miPlaceFace.Checked = placement == ThemeInfoPlacement.OnClockFace;
    }

    private void OnTogglePropertiesClick(object? sender, EventArgs e)
    {
        if (_splitContainer.Panel2Collapsed)
        {
            if (_propertyGrid.Parent is null)
            {
                _splitContainer.Panel2.Controls.Add(_propertyGrid);
            }

            RefreshPropertyGridSelection();
            _splitContainer.Panel2Collapsed = false;
            ApplyPropertyPanelWidth();
        }
        else
        {
            if (_splitContainer.Panel2.Width > 0)
            {
                _propertyPanelWidth = _splitContainer.Panel2.Width;
            }

            _splitContainer.Panel2Collapsed = true;
        }

        _miProperties.Checked = !_splitContainer.Panel2Collapsed;
    }

    private void OnExitClick(object? sender, EventArgs e) => Close();

    private void ApplyThemeToClock(IClockTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        if (ReferenceEquals(_clock.Theme, theme))
        {
            _clock.Theme = null;
        }

        _clock.Theme = theme;
    }

    private void RefreshPropertyGridSelection()
    {
        _propertyGridSelection.SetThemeSession(_activeThemeCustomProperties);
        TypeDescriptor.Refresh(_propertyGridSelection);

        if (ReferenceEquals(_propertyGrid.SelectedObject, _propertyGridSelection))
        {
            _propertyGrid.SelectedObject = null;
        }

        _propertyGrid.SelectedObject = _propertyGridSelection;
        _propertyGrid.Refresh();
    }

    // ── Kiosk component events ──

    private void OnKioskFullScreenChanged(object? sender, EventArgs e)
    {
        bool fullscreen = _kioskModeManager.FullScreen;

        if (fullscreen)
        {
            _modeBeforeFullScreen = _presentationMode;

            if (_presentationMode == WindowPresentationMode.Windowed)
            {
                if (WindowState != FormWindowState.Minimized)
                {
                    _windowedWindowState = WindowState;
                }

                Rectangle windowedBounds = RestoreBounds;
                if (IsSaneWindowedBounds(windowedBounds))
                {
                    _lastWindowedBounds = windowedBounds;
                }
            }

            _presentationMode = WindowPresentationMode.FullScreen;
        }
        else
        {
            _presentationMode = _modeBeforeFullScreen;

            if (_presentationMode == WindowPresentationMode.NoChrome)
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                Bounds = Screen.FromControl(this).WorkingArea;
                TopMost = _kioskModeManager.TopMostInFullScreen;
            }
            else
            {
                _presentationMode = WindowPresentationMode.Windowed;

                if (_windowedWindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Maximized;
                }
            }
        }

        _menuStrip.Visible = !fullscreen;
        _statusStrip.Visible = !fullscreen;
        _statusMode.Text = fullscreen
            ? "Kiosk"
            : _presentationMode == WindowPresentationMode.NoChrome
                ? "No chrome"
                : "Windowed";
        RefreshKioskChecks();
    }

    /// <summary>
    ///  Ensures Escape always restores the normal window from no-chrome mode; fullscreen
    ///  Escape handling remains controlled by <see cref="KioskModeManager"/>.
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape && _presentationMode == WindowPresentationMode.NoChrome)
        {
            ExitNoChromeMode();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }
}
