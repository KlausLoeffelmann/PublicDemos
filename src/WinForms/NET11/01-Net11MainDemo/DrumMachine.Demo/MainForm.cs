using System.ComponentModel;
using DrumMachine.Demo.Controls;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Analysis;
using WinForms.Audio.Core;
using WinForms.Audio.Music;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;
using WinForms.Audio.Synthesis;

namespace DrumMachine.Demo;

/// <summary>
///  Hosts the loop document editor without making the UI responsible for musical timing.
/// </summary>
internal partial class MainForm : Form
{
    private readonly StartupOptions _options;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly LoopDocumentSession _session;
    private readonly AppTheme _appliedTheme;
    private readonly AppFontSize _appliedFontSize;
    private AppSettings _settings;
    private AudioEngine? _engine;
    private DrumMachinePlayer? _player;
    private AudioSpectrumSource? _spectrum;
    private SymbolIconFactory? _iconFactory;
    private ToolbarIconSet? _icons;
    private float[] _spectrumReadback = [];
    private bool _updatingControls = true;
    private bool _busy;
    private bool _closing;
    private bool _closeApproved;
    private bool _confirmingClose;
    private bool _scenarioCompleted;
    private int _highlightedStep = -1;
    private Exception? _reportedAudioFailure;

    private bool Automated => _options.Scenario != DemoScenario.None;
    private bool IgnoreSettings => _options.NoSettings || Automated;
    private bool AudioAvailable => _player is not null && _engine is not null && !_engine.Completion.IsCompleted;

    /// <summary>
    ///  Creates a Designer-safe form without opening audio or reading user files.
    /// </summary>
    public MainForm() : this(StartupOptions.Interactive)
    {
    }

    /// <summary>
    ///  Creates an editor using preferences already read before UI initialization.
    /// </summary>
    internal MainForm(StartupOptions options, AppSettings? settings = null)
    {
        _options = options;
        _settings = settings ?? new AppSettings();
        _appliedTheme = _settings.Theme;
        _appliedFontSize = _settings.FontSize;
        _session = new LoopDocumentSession(new LoopDocument(DemoScores.OriginalBallad));
        InitializeComponent();
        _volumeSelector.Items.Add("Master");
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            _volumeSelector.Items.Add(Cr78Kit.GetDisplayName(instrument));
        }

        _volumeSelector.SelectedIndex = 0;
        _updatingControls = false;
        RefreshDocumentControls(rebuildGrid: true);
        RefreshRecentFiles();
    }

    /// <inheritdoc/>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        InitializeIcons();
        CancellationToken cancellation = _lifetime.Token;
        if (_options.RunFor is { } duration)
        {
            _exitTimer.Interval = Math.Max(1, (int)Math.Ceiling(duration.TotalMilliseconds));
            _exitTimer.Start();
        }

        try
        {
            AudioEngine engine = await Task.Run(() => AudioEngine.Create());
            if (cancellation.IsCancellationRequested)
            {
                engine.Dispose();
                return;
            }

            _engine = engine;
            _engine.Reverb = ReverbSettings.Off;
            // The player's master fader is ramped along with its percussion channels.
            // Leave the engine gain neutral so it is not applied a second time.
            _engine.MasterVolume = 1f;
            _player = new DrumMachinePlayer(engine, _session.Current.Score, new Tempo(_session.Current.TempoBpm));
            ApplyToPlayer(reset: true);
            if (_player is null)
            {
                return;
            }

            _spectrum = new AudioSpectrumSource(engine);
            _spectrumReadback = new float[_spectrum.BinCount];
            _spectrumControl.Source = _spectrum;
            _uiTimer.Start();
            UpdateCommandState();
            _statusLabel.Text = "Original groove. Score and tempo edits take effect at a bar boundary.";
            AppLogger.Information("Audio", $"Opened {engine.SampleRate} Hz output with a playback-aligned spectrum.");
            _ = ObserveAudioAsync(engine);
            _ = ObserveSpectrumAsync(_spectrum);

            if (Automated)
            {
                await RunScenarioAsync(_options.Scenario, cancellation);
                cancellation.ThrowIfCancellationRequested();
                _scenarioCompleted = true;
                ApplyToPlayer(reset: true);
                AppLogger.Information("Harness", $"{_options.Scenario} scenario completed.");
                _statusLabel.Text = $"{_options.Scenario} scenario completed.";
                UpdateCommandState();
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // The deadline/close path records an incomplete scenario before cancelling it.
        }
        catch (Exception ex)
        {
            ReportAudioFailure(ex);
        }
    }

    /// <inheritdoc/>
    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        if (e.Cancel || _closing)
        {
            return;
        }

        if (!_closeApproved && !Automated)
        {
            e.Cancel = true;
            if (_busy || _confirmingClose)
            {
                _statusLabel.Text = "Finish the current document operation before closing.";
                return;
            }

            _confirmingClose = true;
            await RunDocumentCommandAsync(async () =>
            {
                if (await ConfirmDiscardAsync())
                {
                    await PersistPreferencesAsync();
                    _closeApproved = true;
                }
            });
            _confirmingClose = false;
            if (_closeApproved)
            {
                Close();
            }
            return;
        }

        _closing = true;
        if (Automated && !_scenarioCompleted)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Harness", "The requested scenario was interrupted before completion.");
        }

        _lifetime.Cancel();
        _uiTimer.Stop();
        _exitTimer.Stop();
        DisposeAudio();
    }

    private void MainForm_Disposed(object? sender, EventArgs e)
    {
        if (!_closing)
        {
            _closing = true;
            _lifetime.Cancel();
            DisposeAudio();
        }

        _icons?.Dispose();
        _iconFactory?.Dispose();
        _recentEmpty.Dispose();
        _openDialog.Dispose();
        _saveDialog.Dispose();
        _lifetime.Dispose();
    }

    /// <inheritdoc/>
    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        RebuildIcons();
    }

    /// <inheritdoc/>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        if (_stepGrid is not null && !_updatingControls)
        {
            HighlightStep(-1, force: true);
        }
        RebuildIcons();
    }

    private async Task ObserveAudioAsync(AudioEngine engine)
    {
        try
        {
            await engine.Completion;
        }
        catch (Exception ex)
        {
            ReportAudioFailure(ex);
        }
    }

    private async Task ObserveSpectrumAsync(AudioSpectrumSource spectrum)
    {
        try
        {
            await spectrum.Completion;
        }
        catch (Exception ex)
        {
            AppLogger.Error("Spectrum", "The analyzer stopped.", ex);
            if (!_closing && ReferenceEquals(_spectrum, spectrum))
            {
                _spectrumControl.Source = null;
                _statusLabel.Text = $"Spectrum unavailable; audio can continue. Logs: {AppPaths.LogDirectory}";
                if (Automated)
                {
                    Environment.ExitCode = 1;
                    Close();
                }
            }
        }
    }

    private void ReportAudioFailure(Exception exception)
    {
        if (ReferenceEquals(_reportedAudioFailure, exception))
        {
            return;
        }

        _reportedAudioFailure = exception;
        AppLogger.Error("Audio", "Playback or initialization failed.", exception);
        if (_closing)
        {
            return;
        }

        _uiTimer.Stop();
        DisposeAudio();
        UpdateCommandState();
        _statusLabel.Text = $"Audio unavailable; document editing is still available. Logs: {AppPaths.LogDirectory}";
        if (Automated)
        {
            Environment.ExitCode = 1;
            Close();
        }
        else
        {
            MessageBox.Show(this, exception.Message, "Audio", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisposeAudio()
    {
        if (!_spectrumControl.IsDisposed)
        {
            _spectrumControl.Source = null;
        }

        IDisposable?[] resources = [_spectrum, _player, _engine];
        _spectrum = null;
        _player = null;
        _engine = null;
        List<Exception> failures = [];
        foreach (IDisposable? resource in resources)
        {
            try
            {
                resource?.Dispose();
            }
            catch (Exception ex)
            {
                failures.Add(ex);
            }
        }

        if (failures.Count > 0)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Shutdown", "Audio resources could not all close.", new AggregateException(failures));
        }
    }

    private void ApplyToPlayer(bool reset = false)
    {
        if (!AudioAvailable)
        {
            return;
        }

        LoopDocument document = _session.Current;
        float[] levels = Cr78Kit.Instruments.Select(instrument => document.PercussionVolumes[instrument] / 100f).ToArray();
        WithPlayer(player => player.ApplyConfiguration(
            document.Score, new Tempo(document.TempoBpm), document.MasterVolumePercent / 100f,
            levels, document.Loop, document.MetallicEnabled, document.MetallicVolumePercent / 100f, reset));
    }

    private void WithPlayer(Action<DrumMachinePlayer> action)
    {
        if (!AudioAvailable)
        {
            _statusLabel.Text = "Audio is not available.";
            return;
        }

        try
        {
            action(_player!);
            UpdateCommandState();
        }
        catch (Exception ex)
        {
            ReportAudioFailure(ex);
        }
    }

    private void Play_Click(object? sender, EventArgs e) => WithPlayer(player => player.Start());
    private void Pause_Click(object? sender, EventArgs e) => WithPlayer(player => player.Pause());
    private void Stop_Click(object? sender, EventArgs e) => WithPlayer(player => player.Stop());
    private void AuditionMetallic_Click(object? sender, EventArgs e)
        => WithPlayer(player => player.Audition(Cr78Instrument.MetallicBeat, 0.8f));

    private void ExitTimer_Tick(object? sender, EventArgs e)
    {
        _exitTimer.Stop();
        if (Automated && !_scenarioCompleted)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Harness", "The deadline expired before the requested scenario completed.");
        }
        Close();
    }
}
