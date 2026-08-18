using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace WarpClock.App;

public partial class FormMain
{
    private PersistedAppState? _loadedAppState;
    private bool _persistenceEnabled = true;
    private bool _clockSettingsCustomized;

    private bool HasLoadedClockState => _loadedAppState?.Clock.HasUserState == true;

    private void RestorePersistedState()
    {
        _persistenceEnabled = !_startupOptions.DontPersist && _appStateStore is not null;
        _loadedAppState = _persistenceEnabled
            ? _appStateStore!.Load()
            : null;

        ApplyWindowSettings(CreateEffectiveWindowSettings(_loadedAppState?.Window));
        _recordFramerateEnabled = _startupOptions.RecordFramerate
            ?? _loadedAppState?.Window.RecordFramerate
            ?? false;
    }

    private PersistedWindowSettings CreateEffectiveWindowSettings(PersistedWindowSettings? source)
    {
        PersistedWindowSettings settings = source is null
            ? new PersistedWindowSettings()
            : new PersistedWindowSettings
            {
                X = source.X,
                Y = source.Y,
                Width = source.Width,
                Height = source.Height,
                WindowState = source.WindowState,
                PresentationMode = source.PresentationMode,
                ToggleFullScreenKeys = source.ToggleFullScreenKeys,
                AlwaysOn = source.AlwaysOn,
                RecordFramerate = source.RecordFramerate,
                EscapeExitsFullScreen = source.EscapeExitsFullScreen,
                MousePointerAutoHideDelay = source.MousePointerAutoHideDelay,
                TopMostInFullScreen = source.TopMostInFullScreen,
                PropertyPanelWidth = source.PropertyPanelWidth,
            };

        if (_startupOptions.StartKioskMode is bool startKioskMode)
        {
            settings.PresentationMode = startKioskMode
                ? WindowPresentationMode.FullScreen
                : WindowPresentationMode.Windowed;
        }

        if (_startupOptions.AlwaysOn is bool alwaysOn)
        {
            settings.AlwaysOn = alwaysOn;
        }

        if (_startupOptions.RecordFramerate is bool recordFramerate)
        {
            settings.RecordFramerate = recordFramerate;
        }

        settings.Normalize();
        return settings;
    }

    private void ApplyLoadedClockState()
    {
        if (_loadedAppState is null || !HasLoadedClockState)
        {
            return;
        }

        PersistedClockSettings settings = _loadedAppState.Clock;
        _clock.SecondMotion = settings.SecondMotion;
        _clock.MinuteMotion = settings.MinuteMotion;
        _clock.HourMotion = settings.HourMotion;
        _clock.GraceSeconds = settings.GraceSeconds;
        _clock.GlideDurationSeconds = settings.GlideDurationSeconds;
        _clock.MagneticNumerals = settings.MagneticNumerals;
        _clock.TimeOffset = settings.TimeOffset;
        _clock.SpeedMultiplier = settings.SpeedMultiplier;
        _preferredThemeInfoMode = settings.RenderThemeInfo;
        _clock.ThemeInfoPlacement = settings.ThemeInfoPlacement;
        SetOledViewEnabled(settings.OledView);
        _clock.VSyncEnabled = settings.VSyncEnabled;
        _clock.TargetFrameRate = settings.TargetFrameRate;
        ApplyEffectiveThemeInfoMode();

        _clockSettingsCustomized = true;
        RefreshAllSettingChecks();
        _propertyGrid.Refresh();
    }

    private ThemeSelection ResolveStartupThemeSelection(out ThemeSelectionReason reason)
    {
        if (!string.IsNullOrWhiteSpace(_startupOptions.StartTheme))
        {
            if (TryResolveThemeToken(_startupOptions.StartTheme!, out ThemeSelection? commandLineSelection))
            {
                reason = ThemeSelectionReason.CommandLine;
                return commandLineSelection!;
            }

            _logger.LogWarning("The requested startup theme '{Theme}' was not found. Falling back.", _startupOptions.StartTheme);
        }

        ThemeSelection? scheduledSelection = TryGetScheduledThemeSelection();
        if (scheduledSelection is not null)
        {
            reason = ThemeSelectionReason.Scheduled;
            return scheduledSelection;
        }

        if (TryResolveThemeReference(_loadedAppState?.Theme.CurrentTheme, out ThemeSelection? persistedSelection))
        {
            reason = ThemeSelectionReason.PersistedState;
            return persistedSelection!;
        }

        reason = ThemeSelectionReason.DefaultStartup;
        return new ThemeSelection(_themes[0], explicitVariant: null);
    }

    private PersistedAppState CaptureCurrentAppState()
        => new()
        {
            Window = CaptureWindowSettings(),
            Clock = new PersistedClockSettings
            {
                HasUserState = true,
                SecondMotion = _clock.SecondMotion,
                MinuteMotion = _clock.MinuteMotion,
                HourMotion = _clock.HourMotion,
                GraceSeconds = _clock.GraceSeconds,
                GlideDurationSeconds = _clock.GlideDurationSeconds,
                MagneticNumerals = _clock.MagneticNumerals,
                TimeOffset = _clock.TimeOffset,
                SpeedMultiplier = _clock.SpeedMultiplier,
                RenderThemeInfo = _preferredThemeInfoMode,
                ThemeInfoPlacement = _clock.ThemeInfoPlacement,
                OledView = GetOledViewEnabled(),
                VSyncEnabled = _clock.VSyncEnabled,
                TargetFrameRate = _clock.TargetFrameRate,
            },
            Theme = new PersistedThemeState
            {
                CurrentTheme = ThemeReferenceUtility.Clone(_currentSelection?.ToReference()),
                CurrentThemeListPath = _currentThemeListPath,
                DefaultThemeListPath = _defaultThemeListPath,
            },
        };

    private void PersistCurrentAppState()
    {
        if (!_persistenceEnabled || _appStateStore is null)
        {
            return;
        }

        try
        {
            _appStateStore.Save(CaptureCurrentAppState());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "Could not persist WarpClock UI settings.");
        }
    }

    private void RefreshAllSettingChecks()
    {
        RefreshGraceChecks();
        RefreshSpeedChecks();
        RefreshKioskChecks();
        _miOledView.Checked = GetOledViewEnabled();
        _miOledView.Enabled = SupportsOledView();
        _miRecordFramerate.Checked = _recordFramerateEnabled;
        _miMagnetic.Checked = _clock.MagneticNumerals;
        _miVSync.Checked = _clock.VSyncEnabled;
        OnSecondMotionOpening(this, EventArgs.Empty);
        OnMinuteMotionOpening(this, EventArgs.Empty);
        OnHourMotionOpening(this, EventArgs.Empty);
        OnThemeInfoOpening(this, EventArgs.Empty);
        OnThemeInfoPlacementOpening(this, EventArgs.Empty);
    }

    private bool ShouldApplyThemeDefaultsOnThemeChange() => !_clockSettingsCustomized;

    private void MarkClockSettingsCustomized()
    {
        _clockSettingsCustomized = true;
        RefreshAllSettingChecks();
        _propertyGrid.Refresh();
    }

    private void OnClockSettingsPropertyValueChanged(object? sender, PropertyValueChangedEventArgs e)
    {
        MarkClockSettingsCustomized();
        _statusInfo.Text = $"Setting updated: {e.ChangedItem?.Label ?? "Property"}";
    }
}

internal enum ThemeSelectionReason
{
    DefaultStartup,
    PersistedState,
    CommandLine,
    Scheduled,
    Manual,
    PluginReload,
    OledViewToggle,
}
