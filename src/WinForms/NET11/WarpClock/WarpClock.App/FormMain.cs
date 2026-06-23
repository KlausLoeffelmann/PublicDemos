using WarpClock.Abstractions;
using WarpClock.Engine;
using WarpClock.Themes.Builtin;

namespace WarpClock.App;

/// <summary>
///  The kiosk demo host: shows a <see cref="WarpClockControl"/> filling the window,
///  drives it with built-in and drop-in plug-in themes, and demonstrates the .NET 11
///  <see cref="KioskModeManager"/> component for fullscreen / power / wake handling.
/// </summary>
public partial class FormMain : Form
{
    private readonly List<ThemeEntry> _themes = [];
    private readonly Dictionary<ThemeEntry, ToolStripMenuItem> _themeItems = [];
    private readonly ThemePluginLoader _pluginLoader;
    private FileSystemWatcher? _pluginWatcher;
    private ThemeEntry? _current;

    // Serializes every plug-in discovery pass. The initial load runs on a worker thread
    // (see OnLoad) while the FileSystemWatcher can fire a hot-reload at any time; both go
    // through this gate so they can never call the non-thread-safe ThemePluginLoader
    // concurrently (which would corrupt its internal state and double-load assemblies).
    private readonly SemaphoreSlim _loadGate = new(1, 1);

    public FormMain()
    {
        InitializeComponent();

        _pluginLoader = new ThemePluginLoader(Path.Combine(AppContext.BaseDirectory, "plugins"));
        _clock.GraceSeconds = 5;

        RefreshGraceChecks();
        RefreshSpeedChecks();

        // Built-in themes are in-memory and instantaneous, so loading them and selecting
        // the first one here keeps the clock populated the moment the window appears.
        LoadBuiltInThemes();

        if (_themes.Count > 0)
        {
            SelectTheme(_themes[0]);
        }

        // NOTE: plug-in discovery (disk enumeration + assembly loading + reflection) and
        // the FileSystemWatcher are deliberately NOT started here. Doing that work in the
        // constructor would block startup, cannot be awaited, and would run before this
        // form owns a window handle (so progress reporting via InvokeAsync would throw).
        // It is deferred to OnLoad instead — see below.
    }

    /// <summary>
    ///  Deferred, awaitable initialization. By the time <see cref="OnLoad"/> runs the
    ///  window handle exists, so we can run the heavy plug-in discovery on a worker
    ///  thread and safely marshal status/UI updates back via <c>InvokeAsync</c>.
    /// </summary>
    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        try
        {
            // Heavy work off the constructor / UI thread …
            await LoadPluginsAsync(initial: true);

            // … and only AFTER the initial load completes do we start watching the folder.
            // Enabling the watcher earlier would let a file drop during startup trigger a
            // reload that races the worker-thread initial load.
            EnablePluginWatcher();
        }
        catch (Exception ex)
        {
            // OnLoad is an `async void` override: just like an async void event handler it
            // MUST catch, otherwise an exception here would crash the application.
            _statusInfo.Text = $"Plug-in load failed: {ex.Message}";
        }
    }

    /// <summary>
    ///  Disposes the runtime resources that were created in regular code (not by the
    ///  Designer), so they live outside the Designer's <c>Dispose(bool)</c>.
    /// </summary>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _pluginWatcher?.Dispose();
        _loadGate.Dispose();
        base.OnFormClosed(e);
    }

    private void LoadBuiltInThemes()
    {
        foreach (IClockTheme theme in BuiltInThemes.All())
        {
            AddThemeEntry(new ThemeEntry(theme, theme.Name, "built-in"));
        }
    }

    /// <summary>
    ///  Runs one plug-in discovery pass. Discovery (folder scan + assembly load +
    ///  reflection) happens on a worker thread; the resulting UI mutation (menu items,
    ///  status text) is marshaled back to the UI thread. All passes are serialized via
    ///  <see cref="_loadGate"/> so the worker-thread initial load and a watcher-triggered
    ///  hot-reload can never enter the non-thread-safe loader at the same time.
    /// </summary>
    /// <param name="initial">
    ///  <see langword="true"/> for the one-time startup load (stays quiet when nothing is
    ///  found); <see langword="false"/> for an explicit/hot reload (reports "none found").
    /// </param>
    private async Task LoadPluginsAsync(bool initial)
    {
        await _loadGate.WaitAsync();

        try
        {
            // Status reporting is best-effort: only marshal when a handle exists, because
            // InvokeAsync throws if the control has no window handle (e.g. during teardown).
            if (IsHandleCreated)
            {
                await InvokeAsync(() => _statusInfo.Text = "Scanning for plug-in themes…");
            }

            // The expensive part runs on the thread pool and touches no controls.
            IReadOnlyList<DiscoveredTheme> found =
                await Task.Run(() => _pluginLoader.LoadNew());

            if (found.Count == 0)
            {
                if (!initial && IsHandleCreated)
                {
                    await InvokeAsync(() => _statusInfo.Text = "No new plug-in themes found.");
                }

                return;
            }

            // UI-bound collections (the theme menu and backing lists) may only be touched
            // on the UI thread, so the additions are marshaled back here.
            if (IsHandleCreated)
            {
                await InvokeAsync(() =>
                {
                    foreach (DiscoveredTheme discovered in found)
                    {
                        AddThemeEntry(new ThemeEntry(discovered.Theme, discovered.DisplayName, discovered.SourceFile));
                    }

                    int added = found.Count;
                    _statusInfo.Text = $"Loaded {added} new plug-in theme{(added == 1 ? string.Empty : "s")}.";
                });
            }
        }
        finally
        {
            _loadGate.Release();
        }
    }

    private void AddThemeEntry(ThemeEntry entry)
    {
        var item = new ToolStripMenuItem(entry.Display) { Tag = entry };
        item.Click += OnThemeSelected;
        _themes.Add(entry);
        _themeItems[entry] = item;
        _themeMenu.DropDownItems.Add(item);
    }

    private void OnThemeSelected(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: ThemeEntry entry })
        {
            SelectTheme(entry);
        }
    }

    private void SelectTheme(ThemeEntry entry)
    {
        _current = entry;
        _clock.Theme = entry.Theme;

        foreach ((ThemeEntry candidate, ToolStripMenuItem item) in _themeItems)
        {
            item.Checked = ReferenceEquals(candidate, entry);
        }

        _statusInfo.Text = $"Theme: {entry.Display} ({entry.Source})" +
            (entry.Theme.Capabilities.FreeFloating ? " — free-floating" : string.Empty);
    }

    // ── Motion menu handlers ──

    private void OnSecondMotionClick(object? sender, EventArgs e)
        => SetSecondMotion(MotionFor(sender));

    private void OnMinuteMotionClick(object? sender, EventArgs e)
        => SetMinuteMotion(MotionFor(sender));

    private void OnHourMotionClick(object? sender, EventArgs e)
        => SetHourMotion(MotionFor(sender));

    private void SetSecondMotion(ClockHandMotion motion)
    {
        _clock.SecondMotion = motion;
        _statusInfo.Text = $"Second hand: {motion}";
    }

    private void SetMinuteMotion(ClockHandMotion motion)
    {
        _clock.MinuteMotion = motion;
        _statusInfo.Text = $"Minute hand: {motion}";
    }

    private void SetHourMotion(ClockHandMotion motion)
    {
        _clock.HourMotion = motion;
        _statusInfo.Text = $"Hour hand: {motion}";
    }

    private ClockHandMotion MotionFor(object? sender)
    {
        if (ReferenceEquals(sender, _miSecondCrawling)
            || ReferenceEquals(sender, _miMinuteCrawling)
            || ReferenceEquals(sender, _miHourCrawling))
        {
            return ClockHandMotion.Crawling;
        }

        if (ReferenceEquals(sender, _miSecondTick)
            || ReferenceEquals(sender, _miMinuteTick)
            || ReferenceEquals(sender, _miHourTick))
        {
            return ClockHandMotion.Tick;
        }

        return ClockHandMotion.Sweep;
    }

    private void OnSecondMotionOpening(object? sender, EventArgs e)
        => RefreshMotionChecks(_clock.SecondMotion, _miSecondCrawling, _miSecondSweep, _miSecondTick);

    private void OnMinuteMotionOpening(object? sender, EventArgs e)
        => RefreshMotionChecks(_clock.MinuteMotion, _miMinuteCrawling, _miMinuteSweep, _miMinuteTick);

    private void OnHourMotionOpening(object? sender, EventArgs e)
        => RefreshMotionChecks(_clock.HourMotion, _miHourCrawling, _miHourSweep, _miHourTick);

    private void RefreshMotionChecks(
        ClockHandMotion current,
        ToolStripMenuItem crawling,
        ToolStripMenuItem sweep,
        ToolStripMenuItem tick)
    {
        // A free-floating theme cannot crawl; reflect that in the menu.
        bool freeFloating = _current?.Theme.Capabilities.FreeFloating == true;
        crawling.Enabled = !freeFloating;
        crawling.Checked = current == ClockHandMotion.Crawling;
        sweep.Checked = current == ClockHandMotion.Sweep;
        tick.Checked = current == ClockHandMotion.Tick;
    }

    // ── Grace menu handlers ──

    private void OnGraceClick(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: int seconds })
        {
            _clock.GraceSeconds = seconds;
            RefreshGraceChecks();
            _statusInfo.Text = $"Grace catch-up: {seconds}s";
        }
    }

    private void RefreshGraceChecks()
    {
        _miGrace1.Checked = _clock.GraceSeconds == 1;
        _miGrace5.Checked = _clock.GraceSeconds == 5;
        _miGrace10.Checked = _clock.GraceSeconds == 10;
        _miGrace20.Checked = _clock.GraceSeconds == 20;
        _miGrace30.Checked = _clock.GraceSeconds == 30;
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
            RefreshSpeedChecks();
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

    // ── View menu handlers ──

    private void OnKioskClick(object? sender, EventArgs e) => _kioskModeManager.ToggleFullScreen();

    private void OnHideTaskbarClick(object? sender, EventArgs e)
    {
        _kioskModeManager.HideTaskbar = !_kioskModeManager.HideTaskbar;
        _miHideTaskbar.Checked = _kioskModeManager.HideTaskbar;
        _statusInfo.Text = $"Hide taskbar: {(_kioskModeManager.HideTaskbar ? "On" : "Off")}";
    }

    private void OnPreventSleepClick(object? sender, EventArgs e)
    {
        _kioskModeManager.SuppressPowerSaving = !_kioskModeManager.SuppressPowerSaving;
        _miPreventSleep.Checked = _kioskModeManager.SuppressPowerSaving;
        _statusInfo.Text = $"Prevent sleep: {(_kioskModeManager.SuppressPowerSaving ? "On" : "Off")}";
    }

    private void OnTogglePropertiesClick(object? sender, EventArgs e)
    {
        if (_splitContainer.Panel2Collapsed && _propertyGrid.Parent is null)
        {
            _splitContainer.Panel2.Controls.Add(_propertyGrid);
            _propertyGrid.SelectedObject = _clock;
        }

        _splitContainer.Panel2Collapsed = !_splitContainer.Panel2Collapsed;
        _miProperties.Checked = !_splitContainer.Panel2Collapsed;
    }

    private void OnExitClick(object? sender, EventArgs e) => Close();

    // ── Plug-in menu handlers + hot-loading ──

    private void OnReloadPluginsClick(object? sender, EventArgs e) => _ = ReloadPluginsAsync();

    private void OnOpenPluginsFolderClick(object? sender, EventArgs e) => OpenPluginFolder();

    private void EnablePluginWatcher()
    {
        try
        {
            Directory.CreateDirectory(_pluginLoader.PluginDirectory);

            _pluginWatcher = new FileSystemWatcher(_pluginLoader.PluginDirectory, "*.dll")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            };
            _pluginWatcher.Created += OnPluginFileChanged;
            _pluginWatcher.Changed += OnPluginFileChanged;

            // Enabled only now — after the initial load — so a DLL dropped during startup
            // cannot trigger a reload that overlaps the worker-thread initial load.
            _pluginWatcher.EnableRaisingEvents = true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Hot-loading is best-effort; the menu's Reload command still works.
        }
    }

    private void OnPluginFileChanged(object sender, FileSystemEventArgs e)
    {
        // This fires on a thread-pool thread. We do NOT marshal here: LoadPluginsAsync
        // does its own UI marshaling and is serialized via _loadGate, so it is safe to
        // kick off from any thread and cannot race the initial load.
        _ = ReloadPluginsAsync();
    }

    private async Task ReloadPluginsAsync()
    {
        try
        {
            await LoadPluginsAsync(initial: false);
        }
        catch (Exception ex)
        {
            // Fire-and-forget entry point: swallow nothing silently, but never let an
            // exception escape an unobserved Task.
            if (IsHandleCreated)
            {
                try
                {
                    await InvokeAsync(() => _statusInfo.Text = $"Plug-in reload failed: {ex.Message}");
                }
                catch
                {
                    // The handle may have gone away during shutdown; nothing to report to.
                }
            }
        }
    }

    private void OpenPluginFolder()
    {
        Directory.CreateDirectory(_pluginLoader.PluginDirectory);

        using var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _pluginLoader.PluginDirectory,
                UseShellExecute = true,
            },
        };

        process.Start();
    }

    // ── Kiosk component events ──

    private void OnKioskFullScreenChanged(object? sender, EventArgs e)
    {
        bool fullscreen = _kioskModeManager.FullScreen;
        _menuStrip.Visible = !fullscreen;
        _statusStrip.Visible = !fullscreen;
        _statusMode.Text = fullscreen ? "Kiosk" : "Windowed";
    }

    private void OnKioskWakeup(object? sender, KioskModeWakeupEventArgs e)
        => _statusInfo.Text = $"Wakeup: {e.Source}";
}
