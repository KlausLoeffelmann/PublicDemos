using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WarpClock.Abstractions;

namespace WarpClock.App;

public partial class FormMain
{
    private readonly TimeZoneRotationController _timeZoneRotation = new();
    private readonly Stopwatch _timeZoneClock = Stopwatch.StartNew();
    private System.Windows.Forms.Timer? _timeZoneTimer;
    private WarpClockOptions _options = new();
    private TimeSpan _lastTimeZoneTick;

    private void RestoreRuntimeOptions(WarpClockOptions? persisted)
    {
        _options = persisted?.Clone() ?? new WarpClockOptions();
        _options.Normalize();

        if (string.IsNullOrWhiteSpace(_options.Folders.ThemesFolder))
        {
            _options.Folders.ThemesFolder = _appPaths.PluginDirectory;
        }

        ReconfigurePluginFolder(_options.Folders.ThemesFolder);
        _pictureCatalog.Refresh(_options.Folders.PicturesFolder);
        _timeZoneRotation.Reset(_options.TimeZones);
        _lastTimeZoneTick = _timeZoneClock.Elapsed;
    }

    private WarpClockOptions CaptureRuntimeOptions()
    {
        WarpClockOptions captured = _options.Clone();
        captured.Hands = new HandOptions
        {
            HourMotion = _clock.HourMotion,
            MinuteMotion = _clock.MinuteMotion,
            SecondMotion = _clock.SecondMotion,
            GraceSeconds = _clock.GraceSeconds,
        };
        captured.Normalize();
        return captured;
    }

    private void SynchronizeHandOptionsFromClock()
        => _options.Hands = new HandOptions
        {
            HourMotion = _clock.HourMotion,
            MinuteMotion = _clock.MinuteMotion,
            SecondMotion = _clock.SecondMotion,
            GraceSeconds = _clock.GraceSeconds,
        };

    private void ApplyRuntimeOptions(bool reloadPluginsForFolderChange)
    {
        _options.Normalize();

        _clock.HourMotion = _options.Hands.HourMotion;
        _clock.MinuteMotion = _options.Hands.MinuteMotion;
        _clock.SecondMotion = _options.Hands.SecondMotion;
        _clock.GraceSeconds = _options.Hands.GraceSeconds;

        string requestedThemeFolder = string.IsNullOrWhiteSpace(_options.Folders.ThemesFolder)
            ? _appPaths.PluginDirectory
            : _options.Folders.ThemesFolder;
        bool themeFolderChanged = !PathEquals(requestedThemeFolder, _pluginLoader.PluginDirectory);
        if (themeFolderChanged)
        {
            if (reloadPluginsForFolderChange)
            {
                RemoveLoadedPluginThemes();
            }

            ReconfigurePluginFolder(requestedThemeFolder);
            EnablePluginWatcher();
            if (reloadPluginsForFolderChange)
            {
                _ = ReloadPluginsAsync();
            }
        }

        _pictureCatalog.Refresh(_options.Folders.PicturesFolder);
        _timeZoneRotation.Reset(_options.TimeZones);
        _lastTimeZoneTick = _timeZoneClock.Elapsed;
        ApplyDisplayedTimeZone();

        _timeZoneTimer?.Start();
        RefreshRuntimePresentation();
        MarkClockSettingsCustomized();
    }

    private void RemoveLoadedPluginThemes()
    {
        if (_current is not null
            && !string.Equals(_current.Catalog.Source, StockThemeSource, StringComparison.OrdinalIgnoreCase))
        {
            ThemeEntry fallback = _themes.First(entry =>
                string.Equals(entry.Catalog.Source, StockThemeSource, StringComparison.OrdinalIgnoreCase));
            SelectTheme(
                new ThemeSelection(fallback, explicitVariant: null),
                ThemeSelectionReason.PluginReload,
                applyThemeDefaults: false);
        }

        _themes.RemoveAll(entry =>
            !string.Equals(entry.Catalog.Source, StockThemeSource, StringComparison.OrdinalIgnoreCase));
        RebuildThemeMenuItems();
    }

    private void OnTimeZoneTimerTick(object? sender, EventArgs e)
    {
        TimeSpan now = _timeZoneClock.Elapsed;
        TimeSpan elapsed = now - _lastTimeZoneTick;
        _lastTimeZoneTick = now;

        if (_timeZoneRotation.Advance(elapsed))
        {
            ApplyDisplayedTimeZone();
        }

        RefreshRuntimePresentation();
    }

    private void ApplyDisplayedTimeZone()
    {
        ConfiguredTimeZone configured = _timeZoneRotation.Current;
        TimeZoneInfo zone;
        try
        {
            zone = TimeZoneInfo.FindSystemTimeZoneById(configured.TimeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            _logger.LogWarning(
                ex,
                "Configured timezone {TimeZoneId} is unavailable; using the local timezone.",
                configured.TimeZoneId);
            zone = TimeZoneInfo.Local;
        }

        _clock.DisplayedTimeZone = zone;
    }

    private void RefreshRuntimePresentation()
    {
        ConfiguredTimeZone configured = _timeZoneRotation.Current;
        TimeZoneInfo zone = _clock.DisplayedTimeZone;
        DateTime displayedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        string alias = string.IsNullOrWhiteSpace(configured.DisplayName)
            ? zone.DisplayName
            : configured.DisplayName;
        string designation = CreateTimeZoneDesignation(zone.GetUtcOffset(displayedTime));
        bool alternate = !configured.IsDefault;
        bool showTimeZone = _options.TimeZones.Enabled
            && _options.TimeZones.ShowOnClockFace
            && (!_options.TimeZones.ShowOnlyWhenAlternate || alternate);
        string tickerText = ComposeTickerText(displayedTime, alias);

        _clock.AmbientContent = new ClockAmbientSnapshot
        {
            TickerText = tickerText,
            TimeZoneAlias = alias,
            TimeZoneDesignation = designation,
            PresentationState = alternate
                ? ClockAmbientPresentationState.Alternate
                : ClockAmbientPresentationState.Default,
            IndexedImages = _pictureCatalog.Paths
                .Select((path, index) => new ClockIndexedImageSnapshot
                {
                    Index = index,
                    Source = path,
                    Description = Path.GetFileName(path),
                })
                .ToArray(),
        };
        _clock.AuxiliaryVisibility = ClockAuxiliaryVisibility.Default with
        {
            ShowTimeZone = showTimeZone,
            ShowFractionSecond = _options.Display.ShowFractionSecondVisual,
            ShowOverlayMessage = _options.Display.ShowThemeTickerVisual,
        };
        _clock.TimeZoneHeadlineFallbackEnabled = _options.TimeZones.Enabled
            && _options.TimeZones.ShowHeadlineFallback
            && (!_options.TimeZones.ShowOnlyWhenAlternate || alternate);
        _clock.TimeZoneHeadlineText = $"Timezone: {alias}";
        _clock.TimeZoneHeadlineNightMode = _currentResolvedVariant
            is ClockThemeVariantKind.Night or ClockThemeVariantKind.OledNight;

        _tickerBand.Visible = _options.Display.TickerEnabled;
        if (_tickerBand.Visible)
        {
            _tickerBand.Height = _tickerBand.GetPreferredSize(ClientSize).Height;
            _tickerBand.TickerText = tickerText;
        }
    }

    private void RefreshTickerText()
    {
        if (!_options.Display.TickerEnabled)
        {
            return;
        }

        TimeZoneInfo zone = _clock.DisplayedTimeZone;
        DateTime displayedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
        ConfiguredTimeZone configured = _timeZoneRotation.Current;
        string alias = string.IsNullOrWhiteSpace(configured.DisplayName)
            ? zone.DisplayName
            : configured.DisplayName;
        _tickerBand.TickerText = ComposeTickerText(displayedTime, alias);
    }

    private string ComposeTickerText(DateTime displayedTime, string timeZoneAlias)
        => TickerContentComposer.Compose(
            _options.Display.TickerContentOrder,
            _options.Display.CustomTickerMessage,
            displayedTime,
            timeZoneAlias,
            _current?.Catalog.FamilyName);

    private void ReconfigurePluginFolder(string folder)
    {
        string normalized = Path.GetFullPath(folder);
        if (PathEquals(normalized, _pluginLoader.PluginDirectory))
        {
            return;
        }

        _pluginWatcher?.Dispose();
        _pluginWatcher = null;
        _pluginLoader = new ThemePluginLoader(
            normalized,
            _loggerFactory.CreateLogger<ThemePluginLoader>());
    }

    private static bool PathEquals(string left, string right)
        => string.Equals(
            Path.GetFullPath(left).TrimEnd(Path.DirectorySeparatorChar),
            Path.GetFullPath(right).TrimEnd(Path.DirectorySeparatorChar),
            StringComparison.OrdinalIgnoreCase);

    private static string CreateTimeZoneDesignation(TimeSpan offset)
    {
        if (offset == TimeSpan.Zero)
        {
            return "GMT";
        }

        char sign = offset < TimeSpan.Zero ? '-' : '+';
        offset = offset.Duration();
        return offset.Minutes == 0
            ? $"GMT{sign}{offset.Hours}"
            : $"GMT{sign}{offset.Hours}:{offset.Minutes:00}";
    }
}
