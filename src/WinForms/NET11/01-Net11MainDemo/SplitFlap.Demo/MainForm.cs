namespace SplitFlap.Demo;

/// <summary>
///  Presents the interactive split-flap departure-board demonstration.
/// </summary>
public partial class MainForm : Form
{
    private readonly StartupOptions _startupOptions;
    private readonly CancellationTokenSource _lifetimeCancellation = new();
    private FlightBoard _flights;
    private BoardSound? _sound;
    private CancellationTokenSource? _tuneCancellation;
    private Exception? _reportedAudioFailure;
    private bool _applyingSettings;
    private bool _adjustingAspectRatio;

    /// <summary>
    ///  Initializes the form for an interactive launch.
    /// </summary>
    public MainForm()
        : this(StartupOptions.Interactive)
    {
    }

    /// <summary>
    ///  Initializes the form with parsed startup behavior.
    /// </summary>
    internal MainForm(StartupOptions startupOptions)
    {
        ArgumentNullException.ThrowIfNull(startupOptions);
        _startupOptions = startupOptions;

        InitializeComponent();

        _flights = new FlightBoard(_board.Columns);

        _speedComboBox.DataSource = Enum.GetValues<FlipAnimationSpeed>();
        _speedComboBox.SelectedItem = _board.FlipAnimationSpeed;
    }

    /// <inheritdoc/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!_startupOptions.NoSettings)
        {
            ApplySettings(AppSettingsStore.Load());
        }

        // Runs after the form is shown and the queue has drained, so the room sees the board
        // come up from blank instead of getting it pre-settled.
        _ = InvokeAsync(StartAsync, _lifetimeCancellation.Token);
    }

    /// <inheritdoc/>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // KioskModeManager owns and restores all form presentation state. Leave kiosk through
        // the component before capturing ordinary window settings.
        if (_kioskModeManager.FullScreen)
        {
            _kioskModeManager.FullScreen = false;
        }

        if (!_startupOptions.NoSettings && _autoSaveSettingsMenuItem.Checked)
        {
            TrySaveSettings(showFailure: false);
        }

        _lifetimeCancellation.Cancel();
        _boardTimer.Stop();
        _clockTimer.Stop();
        _tuneCancellation?.Cancel();
        _sound?.Dispose();
        _sound = null;
        base.OnFormClosing(e);
    }

    /// <inheritdoc/>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _lifetimeCancellation.Dispose();
        _tuneCancellation?.Dispose();
        base.OnFormClosed(e);
    }

    /// <inheritdoc/>
    protected override void OnResizeEnd(EventArgs e)
    {
        base.OnResizeEnd(e);
        ApplyWindowAspectRatio();
    }

    /// <inheritdoc/>
    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);

        if (_layout is not null
            && _board is not null
            && !_board.AutoSize
            && _layout.ClientSize is { Width: > 0, Height: > 0 } hostSize)
        {
            _board.Size = DisplayLayoutCalculator.ScaleToFit(
                _board.GetPreferredSize(Size.Empty),
                hostSize);
        }
    }

    private async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_startupOptions.RunFor is { } runFor)
            {
                _ = CloseAfterAsync(runFor, cancellationToken);
            }

            await Task.Delay(600, cancellationToken);

            UpdateClock();
            _board.Text = _flights.Next(_board.Rows);

            _clockTimer.Start();
            _boardTimer.Start();

            if (_startupOptions.Scenario is not SmokeScenario.None)
            {
                await RunScenarioAsync(_startupOptions.Scenario, cancellationToken);
            }

        }
        catch (OperationCanceledException)
        {
            // A timed or user-requested close cancels all pending startup/scenario work.
        }
        catch (Exception ex)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Harness", "The automated scenario failed.", ex);
            Close();
        }
    }

    private async Task CloseAfterAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken);
            Close();
        }
        catch (OperationCanceledException)
        {
            // The form was closed before the harness timer elapsed.
        }
    }

    private void UpdateClock()
        => _clock.Text = DateTime.Now.ToString("HH:mm");

    private void BoardTimer_Tick(object? sender, EventArgs e)
        => _board.Text = _flights.Next(_board.Rows);

    private void ClockTimer_Tick(object? sender, EventArgs e)
        => UpdateClock();

    private void UpdateButton_Click(object? sender, EventArgs e)
    {
        _boardTimer.Stop();
        _board.Text = _flights.Next(_board.Rows);
        _boardTimer.Start();
    }

    private void JamButton_Click(object? sender, EventArgs e)
    {
        // Jam a handful of characters on the next update so the reset dance is visible.
        for (int i = 0; i < 4; i++)
        {
            _board.ForceJam(
                _board.Rows == 1 ? 0 : Random.Shared.Next(1, _board.Rows),
                Random.Shared.Next(_board.Columns));
        }

        UpdateButton_Click(sender, e);
    }

    private void AutoSizeCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        bool dictates = _autoSizeCheckBox.Checked;

        // The one-cell host centers an unanchored board. It never constrains the form, which lets
        // KioskModeManager own fullscreen bounds without AutoSize fighting those bounds.
        _board.AutoSize = dictates;

        if (!dictates && _layout.ClientSize is { Width: > 0, Height: > 0 } hostSize)
        {
            _board.Size = DisplayLayoutCalculator.ScaleToFit(
                _board.GetPreferredSize(Size.Empty),
                hostSize);
        }

        if (!_applyingSettings)
        {
            AppLogger.Information("Display", $"Board dictates size: {dictates}.");
        }
    }

    private void SoundCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        if (_applyingSettings)
        {
            return;
        }

        SetSoundEnabled(_soundCheckBox.Checked, showFailure: true);
    }

    private bool SetSoundEnabled(bool enabled, bool showFailure)
    {
        try
        {
            if (enabled && _sound is null)
            {
                _sound = new BoardSound(_board.Animator);
                _sound.CreateMelodyChannel(VoicePatch.Lead);
                _reportedAudioFailure = null;
                AppLogger.Information("Audio", $"Initialized {nameof(WaveOutSink)} at {_sound.SampleRate} Hz.");
                _ = ObserveSoundAsync(_sound);
            }
            else if (!enabled)
            {
                _tuneCancellation?.Cancel();
                _sound?.Dispose();
                _sound = null;
                AppLogger.Information("Audio", "Sound disabled.");
            }

            _tuneButton.Enabled = _sound is not null;
            _soundCheckBox.Checked = _sound is not null;
            return _sound is not null;
        }
        catch (Exception ex)
        {
            BoardSound? failedSound = _sound;
            _sound = null;
            _soundCheckBox.Checked = false;
            _tuneButton.Enabled = false;
            Exception failure = ex;
            try
            {
                failedSound?.Dispose();
            }
            catch (Exception cleanupError)
            {
                failure = new AggregateException(
                    "Changing the sound state and releasing the audio device failed.", ex, cleanupError);
            }

            AppLogger.Error("Audio", "Could not change the sound state.", failure);

            if (showFailure)
            {
                MessageBox.Show(
                    this,
                    $"{failure.Message}{Environment.NewLine}{Environment.NewLine}Details: {AppPaths.LogDirectory}",
                    "Sound",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return false;
        }
    }

    private async Task ObserveSoundAsync(BoardSound sound)
    {
        try
        {
            // Started from the UI thread: the continuation returns here, not to the audio pump.
            // One observed lifetime task covers clacks and buzzes that have no per-voice task.
            await sound.Completion;
        }
        catch (Exception ex)
        {
            ReportSoundFailure(sound, ex);
        }
    }

    private void ReportSoundFailure(BoardSound sound, Exception exception)
    {
        if (ReferenceEquals(_reportedAudioFailure, exception))
        {
            return;
        }

        _reportedAudioFailure = exception;
        AppLogger.Error("Audio", "Playback failed.", exception);

        // An old engine may report failure after the user has already disabled/replaced it.
        // Keep its diagnostic, but do not turn off a newer, healthy engine.
        if (!ReferenceEquals(_sound, sound))
        {
            return;
        }

        SetSoundEnabled(enabled: false, showFailure: false);
        if (_lifetimeCancellation.IsCancellationRequested || IsDisposed || Disposing)
        {
            return;
        }

        if (_startupOptions.Scenario is not SmokeScenario.None)
        {
            Environment.ExitCode = 1;
            Close();
            return;
        }

        MessageBox.Show(
            this,
            $"{exception.Message}{Environment.NewLine}{Environment.NewLine}Details: {AppPaths.LogDirectory}",
            "Sound",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private async void TuneButton_Click(object? sender, EventArgs e)
    {
        BoardSound? sound = _sound;
        if (sound is null)
        {
            return;
        }

        using CancellationTokenSource cancellation = new();
        _tuneCancellation?.Cancel();
        _tuneCancellation = cancellation;

        try
        {
            _tuneButton.Enabled = false;

            // Beethoven. Public domain, and every German in the room will hum along whether they want to or not.
            const string melody =
                "E4-4 E4-4 F4-4 G4-4 G4-4 F4-4 E4-4 D4-4 C4-4 C4-4 D4-4 E4-4 E4-4. D4-8 D4-2 " +
                "E4-4 E4-4 F4-4 G4-4 G4-4 F4-4 E4-4 D4-4 C4-4 C4-4 D4-4 E4-4 D4-4. C4-8 C4-2";

            await sound.Melody.PlayNotesAsync(melody, Tempo.Allegro, cancellation.Token);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // Disabling sound or closing the form intentionally interrupts this tune.
        }
        catch (Exception ex)
        {
            ReportSoundFailure(sound, ex);
        }
        finally
        {
            if (ReferenceEquals(_tuneCancellation, cancellation))
            {
                _tuneCancellation = null;
                _tuneButton.Enabled = _sound is not null;
            }
        }
    }

    private void SpeedComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_speedComboBox.SelectedItem is FlipAnimationSpeed speed)
        {
            _board.FlipAnimationSpeed = speed;
            _clock.FlipAnimationSpeed = speed;
        }
    }

    private async Task RunScenarioAsync(SmokeScenario scenario, CancellationToken cancellationToken)
    {
        AppLogger.Information("Harness", $"Running {scenario} scenario.");

        if (scenario is SmokeScenario.Display or SmokeScenario.All)
        {
            _board.Text = _flights.Next(_board.Rows);
            _board.ForceJam(Math.Min(1, _board.Rows - 1), 0);
            await Task.Delay(750, cancellationToken);
        }

        if (scenario is SmokeScenario.Sound or SmokeScenario.All)
        {
            if (!SetSoundEnabled(enabled: true, showFailure: false))
            {
                throw new InvalidOperationException("The sound scenario could not initialize the audio device.");
            }

            using CancellationTokenSource soundTimeout =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            soundTimeout.CancelAfter(TimeSpan.FromSeconds(5));
            await _sound!.Melody.PlayNotesAsync("C4-8 E4-8 G4-8 C5-4", Tempo.Allegro, soundTimeout.Token);
        }

        AppLogger.Information("Harness", $"{scenario} scenario completed.");
    }

    private void AutoSaveSettingsMenuItem_Click(object? sender, EventArgs e)
        => _autoSaveSettingsMenuItem.Checked = !_autoSaveSettingsMenuItem.Checked;

    private void SaveSettingsMenuItem_Click(object? sender, EventArgs e)
        => TrySaveSettings(showFailure: true);

    private void QuitMenuItem_Click(object? sender, EventArgs e)
        => Close();

    private void KioskMenuItem_Click(object? sender, EventArgs e)
    {
        _kioskModeManager.FullScreen = !_kioskModeManager.FullScreen;
        AppLogger.Information("Presentation", $"Kiosk mode: {_kioskModeManager.FullScreen}.");
    }

    private void OptionsMenuItem_Click(object? sender, EventArgs e)
    {
        using OptionsDialog dialog = new(_boardTimer.Interval / 1000);

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            SetUpdateInterval(dialog.UpdateIntervalSeconds);
        }
    }

    private void WindowFullScreenMenuItem_Click(object? sender, EventArgs e)
    {
        if (_kioskModeManager.FullScreen)
        {
            _kioskModeManager.FullScreen = false;
        }

        WindowState = WindowState == FormWindowState.Maximized
            ? FormWindowState.Normal
            : FormWindowState.Maximized;
        RefreshMenuChecks();
    }

    private void KioskModeManager_FullScreenChanged(object? sender, EventArgs e)
        => RefreshMenuChecks();

    private void FontMenuItem_Click(object? sender, EventArgs e)
    {
        using Font initialFont = new(
            MonospaceFonts.ResolveFamilyName(_board.FontName),
            _board.FontSize,
            FontStyle.Regular,
            GraphicsUnit.Point);
        using FontDialog dialog = new()
        {
            Font = initialFont,
            FontMustExist = true,
            ShowEffects = false,
            MinSize = 4,
            MaxSize = 400
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _board.FontName = dialog.Font.FontFamily.Name;
            _board.FontSize = dialog.Font.SizeInPoints;
            ApplyWindowAspectRatio();
        }
    }

    private void KeepAspectRatioMenuItem_Click(object? sender, EventArgs e)
    {
        _keepAspectRatioMenuItem.Checked = !_keepAspectRatioMenuItem.Checked;
        _board.KeepAspectRatio = _keepAspectRatioMenuItem.Checked;
        ApplyWindowAspectRatio();
    }

    private void DefineGridMenuItem_Click(object? sender, EventArgs e)
    {
        using GridDimensionsDialog dialog = new(_board.Rows, _board.Columns);

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _board.Rows = dialog.Rows;
            _board.Columns = dialog.Columns;
            _flights = new FlightBoard(_board.Columns);
            _board.Text = _flights.Next(_board.Rows);
            ApplyWindowAspectRatio();
        }
    }

    private void FitScreenMenuItem_Click(object? sender, EventArgs e)
    {
        if (_kioskModeManager.FullScreen)
        {
            _kioskModeManager.FullScreen = false;
        }

        WindowState = FormWindowState.Normal;
        Screen screen = Screen.FromControl(this);
        Size target = new(
            (int)(screen.WorkingArea.Width * 0.9),
            (int)((screen.WorkingArea.Height - _bottomBar.Height - _menuStrip.Height) * 0.9));

        bool previousAutoSize = _board.AutoSize;
        _board.AutoSize = true;
        Size preferred = _board.GetPreferredSize(Size.Empty);
        DisplayLayoutCalculator.DisplayFit fit = DisplayLayoutCalculator.CalculateGridFit(
            _board.Rows,
            _board.Columns,
            _board.FontSize,
            preferred,
            target);
        _board.Rows = fit.Rows;
        _board.Columns = fit.Columns;
        _board.FontSize = fit.FontSize;
        _flights = new FlightBoard(_board.Columns);
        _board.Text = _flights.Next(_board.Rows);
        _board.AutoSize = previousAutoSize;

        ClientSize = new Size(
            Math.Min(screen.WorkingArea.Width, _board.GetPreferredSize(Size.Empty).Width),
            Math.Min(
                screen.WorkingArea.Height,
                _board.GetPreferredSize(Size.Empty).Height + _bottomBar.Height + _menuStrip.Height));
        CenterToScreen();
    }

    private void ApplyWindowAspectRatio()
    {
        if (_adjustingAspectRatio
            || !_keepAspectRatioMenuItem.Checked
            || WindowState != FormWindowState.Normal
            || _kioskModeManager.FullScreen)
        {
            return;
        }

        Size preferred = _board.GetPreferredSize(Size.Empty);

        if (preferred.Width <= 0 || preferred.Height <= 0)
        {
            return;
        }

        _adjustingAspectRatio = true;

        try
        {
            int nonBoardHeight = _bottomBar.Height + _menuStrip.Height + Padding.Vertical;
            int targetHeight = (int)Math.Round(ClientSize.Width * preferred.Height / (double)preferred.Width)
                + nonBoardHeight;
            ClientSize = new Size(ClientSize.Width, Math.Max(nonBoardHeight + 1, targetHeight));
        }
        finally
        {
            _adjustingAspectRatio = false;
        }
    }

    private void ApplySettings(AppSettings settings)
    {
        _applyingSettings = true;

        try
        {
            _autoSaveSettingsMenuItem.Checked = settings.AutoSave;
            _board.FontName = settings.FontName;
            _board.FontSize = settings.FontSize;
            _board.Rows = settings.Rows;
            _board.Columns = settings.Columns;
            _board.KeepAspectRatio = settings.KeepAspectRatio;
            _keepAspectRatioMenuItem.Checked = settings.KeepAspectRatio;
            _autoSizeCheckBox.Checked = settings.BoardDictatesSize;
            _speedComboBox.SelectedItem = settings.AnimationSpeed;
            SetUpdateInterval(settings.UpdateIntervalSeconds);
            _flights = new FlightBoard(_board.Columns);

            Rectangle savedBounds = new(
                settings.WindowX,
                settings.WindowY,
                settings.WindowWidth,
                settings.WindowHeight);

            if (IsVisibleOnAnyScreen(savedBounds))
            {
                StartPosition = FormStartPosition.Manual;
                Bounds = savedBounds;
                WindowState = settings.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Maximized
                    : FormWindowState.Normal;
            }

        }
        finally
        {
            _applyingSettings = false;
        }

        SetSoundEnabled(settings.SoundEnabled, showFailure: false);
        RefreshMenuChecks();
    }

    private AppSettings CaptureSettings()
    {
        Rectangle bounds = RestoreBounds;

        return new AppSettings
        {
            AutoSave = _autoSaveSettingsMenuItem.Checked,
            WindowX = bounds.X,
            WindowY = bounds.Y,
            WindowWidth = bounds.Width,
            WindowHeight = bounds.Height,
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Maximized
                : FormWindowState.Normal,
            FontName = _board.FontName,
            FontSize = _board.FontSize,
            Rows = _board.Rows,
            Columns = _board.Columns,
            KeepAspectRatio = _keepAspectRatioMenuItem.Checked,
            BoardDictatesSize = _autoSizeCheckBox.Checked,
            AnimationSpeed = _board.FlipAnimationSpeed,
            SoundEnabled = _sound is not null,
            UpdateIntervalSeconds = _boardTimer.Interval / 1000
        };
    }

    private void TrySaveSettings(bool showFailure)
    {
        try
        {
            AppSettingsStore.Save(CaptureSettings());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            if (showFailure)
            {
                MessageBox.Show(
                    this,
                    $"{ex.Message}{Environment.NewLine}{Environment.NewLine}Details: {AppPaths.LogDirectory}",
                    "Save Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    private void RefreshMenuChecks()
    {
        _kioskMenuItem.Checked = _kioskModeManager.FullScreen;
        _windowFullScreenMenuItem.Checked =
            !_kioskModeManager.FullScreen && WindowState == FormWindowState.Maximized;
    }

    private void SetUpdateInterval(int seconds)
    {
        int normalizedSeconds = UpdateInterval.Normalize(seconds);
        _boardTimer.Interval = normalizedSeconds * 1000;
        AppLogger.Information("Options", $"Timetable update interval: {normalizedSeconds} seconds.");
    }

    private static bool IsVisibleOnAnyScreen(Rectangle bounds)
        => bounds.Width >= 320
            && bounds.Height >= 240
            && Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(bounds));
}
