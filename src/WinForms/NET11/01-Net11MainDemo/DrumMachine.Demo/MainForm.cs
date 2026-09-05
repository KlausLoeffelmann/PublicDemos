using System.ComponentModel;
using System.Diagnostics;
using SplitFlap.Audio.Analysis;
using SplitFlap.Audio.Core;
using SplitFlap.Audio.Music;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Playback;
using SplitFlap.Audio.Sequencing;
using SplitFlap.Audio.Synthesis;

namespace DrumMachine.Demo;

/// <summary>
///  Demonstrates an original editable rhythm, procedurally modeled percussion, and the actual output spectrum.
/// </summary>
internal partial class MainForm : Form
{
    private readonly StartupOptions _options;
    private readonly CancellationTokenSource _lifetime = new();
    private PercussionScore _score = DemoScores.OriginalBallad;
    private AudioEngine? _engine;
    private DrumMachinePlayer? _player;
    private AudioSpectrumSource? _spectrum;
    private float[] _spectrumReadback = [];
    private bool _updatingGrid;
    private bool _closing;
    private bool _scenarioCompleted;
    private int _highlightedStep = -1;
    private Exception? _reportedAudioFailure;

    /// <summary>
    ///  Creates the form without opening a device, including when used by the Designer.
    /// </summary>
    public MainForm() : this(StartupOptions.Interactive)
    {
    }

    /// <summary>
    ///  Creates an interactive or timed scenario window.
    /// </summary>
    internal MainForm(StartupOptions options)
    {
        _options = options;
        InitializeComponent();
        for (int bar = 0; bar < _score.BarCount; bar++)
        {
            _barSelector.Items.Add(bar + 1);
        }

        _barSelector.SelectedIndex = 0;
    }

    /// <inheritdoc/>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        CancellationToken cancellation = _lifetime.Token;
        if (_options.RunFor is { } duration)
        {
            _exitTimer.Interval = Math.Max(1, (int)Math.Ceiling(duration.TotalMilliseconds));
            _exitTimer.Start();
        }

        try
        {
            // Device discovery can take time. It needs no UI thread; only assigning the finished
            // source to a control does. Dispose a late result if the user closed during startup.
            AudioEngine engine = await Task.Run(() => AudioEngine.Create());
            if (cancellation.IsCancellationRequested)
            {
                engine.Dispose();
                return;
            }

            _engine = engine;
            _engine.Reverb = ReverbSettings.Off;
            _engine.MasterVolume = _volume.Value / 100f;
            _player = new DrumMachinePlayer(_engine, _score, new Tempo((int)_tempo.Value))
            {
                Loop = _loopCheckBox.Checked,
                MetallicLevel = _metallic.Value / 100f
            };
            _spectrum = new AudioSpectrumSource(_engine);
            _spectrumReadback = new float[_spectrum.BinCount];
            _spectrumControl.Source = _spectrum;
            SetAudioControlsEnabled(true);
            _uiTimer.Start();
            _statusLabel.Text = "Dry CR-78-style kit. Original score; edits and tempo changes apply at the next bar.";
            AppLogger.Information("Audio", $"Opened {_engine.SampleRate} Hz output with a playback-aligned spectrum.");
            _ = ObserveAudioAsync(_engine);
            _ = ObserveSpectrumAsync(_spectrum);

            if (_options.Scenario != DemoScenario.None)
            {
                await RunScenarioAsync(_options.Scenario, cancellation);
                cancellation.ThrowIfCancellationRequested();
                _scenarioCompleted = true;
                if (_player is not null)
                {
                    _player.Loop = _loopCheckBox.Checked;
                }
                SetAudioControlsEnabled(true);
                AppLogger.Information("Harness", $"{_options.Scenario} scenario completed.");
                _statusLabel.Text = $"{_options.Scenario} scenario completed.";
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // The close/deadline path records incomplete requested scenarios before cancellation.
        }
        catch (Exception ex)
        {
            ReportAudioFailure(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        if (e.Cancel || _closing)
        {
            return;
        }

        _closing = true;
        if (_options.Scenario != DemoScenario.None && !_scenarioCompleted)
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

        _lifetime.Dispose();
    }

    /// <inheritdoc/>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        if (_stepGrid is not null)
        {
            HighlightStep(-1, force: true);
        }
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
                if (!_spectrumControl.IsDisposed)
                {
                    _spectrumControl.Source = null;
                }
                _statusLabel.Text = $"Spectrum unavailable; healthy audio can continue. Logs: {AppPaths.LogDirectory}";
                if (_options.Scenario != DemoScenario.None)
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
        Environment.ExitCode = 1;
        AppLogger.Error("Audio", "Playback or initialization failed.", exception);
        if (_closing)
        {
            return;
        }

        SetAudioControlsEnabled(false);
        _uiTimer.Stop();
        DisposeAudio();
        _statusLabel.Text = $"Audio failed. Logs: {AppPaths.LogDirectory}";
        if (_options.Scenario != DemoScenario.None)
        {
            Close();
        }
        else
        {
            MessageBox.Show(this, exception.Message, "Rhythm Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisposeAudio()
    {
        _spectrumControl.Source = null;
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
                // Finish independent cleanup, but retain every failure instead of hiding it.
                failures.Add(ex);
            }
        }

        if (failures.Count > 0)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Shutdown", "One or more audio resources could not close.", new AggregateException(failures));
        }
    }

    private void SetAudioControlsEnabled(bool enabled)
    {
        bool interactive = enabled && (_options.Scenario == DemoScenario.None || _scenarioCompleted);
        _transport.Enabled = interactive;
        _scoreToolbar.Enabled = interactive;
        _playButton.Enabled = enabled;
        _stopButton.Enabled = enabled;
        _metallicButton.Enabled = enabled;
        _stepGrid.Enabled = interactive;
    }

    private void PlayButton_Click(object? sender, EventArgs e)
        => WithPlayer(player => player.Start());

    private void StopButton_Click(object? sender, EventArgs e)
        => WithPlayer(player => player.Stop());

    private void MetallicButton_Click(object? sender, EventArgs e)
        => WithPlayer(player => player.Audition(Cr78Instrument.MetallicBeat, 0.8f));

    private void LoopCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        if (_player is not null)
        {
            WithPlayer(player => player.Loop = _loopCheckBox.Checked);
        }
    }

    private void Tempo_ValueChanged(object? sender, EventArgs e)
    {
        if (_player is not null)
        {
            WithPlayer(player => player.Tempo = new Tempo((int)_tempo.Value));
        }
    }

    private void Volume_ValueChanged(object? sender, EventArgs e)
    {
        _volumeLabel.Text = $"&Master {_volume.Value}%";
        if (_engine is not null)
        {
            _engine.MasterVolume = _volume.Value / 100f;
        }
    }

    private void Metallic_ValueChanged(object? sender, EventArgs e)
    {
        _metallicLabel.Text = $"Metallic la&yer {_metallic.Value}%";
        if (_player is not null)
        {
            WithPlayer(player => player.MetallicLevel = _metallic.Value / 100f);
        }
    }

    private void WithPlayer(Action<DrumMachinePlayer> action)
    {
        if (_player is null)
        {
            _statusLabel.Text = "Audio is not ready.";
            return;
        }

        try
        {
            action(_player);
        }
        catch (Exception ex)
        {
            ReportAudioFailure(ex);
        }
    }

    private void BarSelector_SelectedIndexChanged(object? sender, EventArgs e)
        => LoadScoreGrid();

    private void LoadScoreGrid()
    {
        if (_barSelector.SelectedIndex < 0)
        {
            return;
        }

        _updatingGrid = true;
        try
        {
            _stepGrid.Rows.Clear();
            foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
            {
                object[] values = new object[2 + PercussionScore.StepsPerBar];
                values[0] = Cr78Kit.GetDisplayName(instrument);
                values[1] = "Play";
                for (int step = 0; step < PercussionScore.StepsPerBar; step++)
                {
                    values[step + 2] = _score.HasHit(_barSelector.SelectedIndex, instrument, step);
                }

                int row = _stepGrid.Rows.Add(values);
                _stepGrid.Rows[row].Tag = instrument;
            }
        }
        finally
        {
            _updatingGrid = false;
        }

        HighlightStep(-1);
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        _score = DemoScores.OriginalBallad;
        if (_player is not null)
        {
            WithPlayer(player => player.SetScore(_score));
        }

        LoadScoreGrid();
    }

    private void StepGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex == _auditionColumn.Index
            && _stepGrid.Rows[e.RowIndex].Tag is Cr78Instrument instrument)
        {
            WithPlayer(player => player.Audition(instrument));
        }
    }

    private void StepGrid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_stepGrid.IsCurrentCellDirty && _stepGrid.CurrentCell is DataGridViewCheckBoxCell)
        {
            _stepGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void StepGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_updatingGrid || e.RowIndex < 0 || e.ColumnIndex < _step01.Index
            || _barSelector.SelectedIndex < 0)
        {
            return;
        }

        if (_stepGrid.Rows[e.RowIndex].Tag is Cr78Instrument instrument)
        {
            bool enabled = _stepGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value is true;
            _score = _score.WithStep(_barSelector.SelectedIndex, instrument, e.ColumnIndex - _step01.Index, enabled);
            WithPlayer(player => player.SetScore(_score));
        }
    }

    private void StepGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        AppLogger.Error("Editor", "A score cell could not be edited.", e.Exception);
        _statusLabel.Text = "That score edit could not be applied.";
        e.ThrowException = false;
    }

    private void UiTimer_Tick(object? sender, EventArgs e)
    {
        if (_player is null)
        {
            return;
        }

        DrumPlaybackSnapshot position = _player.GetPlaybackSnapshot();
        int visibleStep = position.IsPlaying && position.Bar == _barSelector.SelectedIndex ? position.Step : -1;
        HighlightStep(visibleStep);
        string clock = position.IsPlaybackSynchronized ? "played audio" : "submitted audio";
        string pending = position.HasPendingChanges ? " - edits pending next bar" : "";
        _positionLabel.Text = position.IsPlaying
            ? $"Bar {position.Bar + 1}, step {position.Step + 1} ({clock}){pending}"
            : $"Stopped{pending}";
    }

    private void HighlightStep(int step, bool force = false)
    {
        if (_step01 is null || _step01.Index < 0
            || _stepGrid.Columns.Count < _step01.Index + PercussionScore.StepsPerBar)
        {
            return;
        }

        if (!force && _highlightedStep == step)
        {
            return;
        }

        for (int i = 0; i < PercussionScore.StepsPerBar; i++)
        {
            DataGridViewColumn column = _stepGrid.Columns[_step01.Index + i];
            column.DefaultCellStyle.BackColor = i == step ? SystemColors.Highlight : SystemColors.Window;
            column.DefaultCellStyle.ForeColor = i == step ? SystemColors.HighlightText : SystemColors.WindowText;
        }

        _highlightedStep = step;
    }

    private void ExitTimer_Tick(object? sender, EventArgs e)
    {
        _exitTimer.Stop();
        if (_options.Scenario != DemoScenario.None && !_scenarioCompleted)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Harness", "The automatic-close deadline expired before the requested scenario completed.");
        }

        Close();
    }

    private async Task RunScenarioAsync(DemoScenario scenario, CancellationToken cancellation)
    {
        AudioEngine engine = _engine ?? throw new InvalidOperationException("The audio engine is unavailable.");
        DrumMachinePlayer player = _player ?? throw new InvalidOperationException("The drum player is unavailable.");
        AppLogger.Information("Harness", $"Running {scenario}.");

        if (scenario is DemoScenario.Kit or DemoScenario.All)
        {
            foreach (Cr78Instrument instrument in Cr78Kit.Instruments.Append(Cr78Instrument.MetallicBeat))
            {
                _statusLabel.Text = $"Audition: {Cr78Kit.GetDisplayName(instrument)}";
                await engine.Play(Cr78Kit.CreateVoice(engine.SampleRate, instrument, 0.75f))
                    .WaitAsync(TimeSpan.FromSeconds(5), cancellation);
                await Task.Delay(50, cancellation);
            }
        }

        if (scenario is DemoScenario.Score or DemoScenario.All)
        {
            player.Loop = false;
            player.Start();
            await WaitUntilAsync(() => player.IsPlaying, TimeSpan.FromSeconds(5), cancellation);
            await WaitUntilAsync(() => !player.IsPlaying, TimeSpan.FromSeconds(20), cancellation);
        }

        if (scenario is DemoScenario.Spectrum or DemoScenario.All)
        {
            const double frequency = 1_000;
            VoiceChannel channel = engine.CreateChannel(VoicePatch.Default with { Volume = 0.25f });
            Task tone = channel.PlaySoundAsync(frequency, TimeSpan.FromMilliseconds(900), cancellation);
            await WaitUntilAsync(
                () => SpectrumContains(frequency),
                TimeSpan.FromSeconds(3),
                cancellation);
            await tone.WaitAsync(TimeSpan.FromSeconds(3), cancellation);
        }
    }

    private bool SpectrumContains(double frequency)
    {
        if (_spectrum is null || !_spectrum.TryCopySpectrum(_spectrumReadback, out AudioSpectrumFrame frame))
        {
            return false;
        }

        double binWidth = frame.SampleRate / (double)frame.FftSize;
        return frame.IsPlaybackSynchronized && frame.PeakLevel > 0.01f
            && Math.Abs(frame.PeakFrequency - frequency) <= binWidth * 2;
    }

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout, CancellationToken cancellation)
    {
        Stopwatch clock = Stopwatch.StartNew();
        while (!condition())
        {
            cancellation.ThrowIfCancellationRequested();
            if (clock.Elapsed >= timeout)
            {
                throw new TimeoutException("The requested audio scenario did not reach its expected state.");
            }

            await Task.Delay(20, cancellation);
        }
    }
}
