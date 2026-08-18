using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WarpClock.App;

public partial class FormMain
{
    private const int FrameRateSampleWindowTicks = 10;
    private const int FrameRateSampleWindowSeconds = 2;
    private const int FrameRateSessionSampleCount = 60;

    private FrameRateCsvRecorder? _frameRateRecorder;
    private CancellationTokenSource? _debugRunCancellation;
    private string? _frameRateSessionThemeName;
    private int _frameRateSampleIndex;
    private int _frameRateWindowTickCount;
    private int _frameRateWindowValueCount;
    private int _frameRateSessionValueCount;
    private double _frameRateWindowFpsTotal;
    private double _frameRateSessionFpsTotal;
    private double _frameRateSessionMinFps = double.MaxValue;
    private double _frameRateSessionMaxFps = double.MinValue;
    private bool _recordFramerateEnabled;

    private void StartDiagnosticsIfRequested()
    {
        if (_recordFramerateEnabled && _frameRateRecorder is null && _current is not null)
        {
            RestartFrameRateRecordingForThemeChange(ThemeSelectionReason.DefaultStartup);
        }

        if (_startupOptions.DebugRunSeconds is int debugRunSeconds)
        {
            _debugRunCancellation?.Dispose();
            _debugRunCancellation = new CancellationTokenSource();
            _ = RunDebugCaptureAsync(debugRunSeconds, _debugRunCancellation.Token);
        }
    }

    private void StopDiagnostics()
    {
        _debugRunCancellation?.Cancel();
        _debugRunCancellation?.Dispose();
        _debugRunCancellation = null;
        StopFrameRateRecording("shutdown");
    }

    private void OnRecordFramerateClick(object? sender, EventArgs e)
    {
        _recordFramerateEnabled = !_recordFramerateEnabled;
        RefreshAllSettingChecks();
        MarkClockSettingsCustomized();
        PersistCurrentAppState();

        if (_recordFramerateEnabled)
        {
            RestartFrameRateRecordingForThemeChange(ThemeSelectionReason.Manual);
            _statusInfo.Text = "Frame-rate recording: On";
        }
        else
        {
            StopFrameRateRecording("disabled by user");
            _statusInfo.Text = "Frame-rate recording: Off";
        }
    }

    private void RestartFrameRateRecordingForThemeChange(ThemeSelectionReason reason)
    {
        if (!_recordFramerateEnabled || _current is null)
        {
            return;
        }

        StopFrameRateRecording(reason == ThemeSelectionReason.Scheduled ? "theme changed (scheduled)" : "theme changed");

        try
        {
            string frameRateSessionDirectory = _appPaths.CreateDiagnosticsRunDirectory("FrameRate");
            Directory.CreateDirectory(frameRateSessionDirectory);

            string csvPath = Path.Combine(frameRateSessionDirectory, "framerate.csv");
            _frameRateRecorder = new FrameRateCsvRecorder(csvPath);
            _frameRateSessionThemeName = _current.Catalog.GetConcreteDisplayName(_currentResolvedVariant, GetCurrentThemePeriod());
            ResetFrameRateAccumulators();

            _logger.LogInformation(
                "Recording frame-rate averages for theme {ThemeName} to {Path}.",
                _frameRateSessionThemeName,
                csvPath);

            _statusInfo.Text = $"Recording frame rate: {_frameRateSessionThemeName}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            _logger.LogWarning(ex, "Could not start frame-rate recording.");
            _statusInfo.Text = $"Frame-rate recording failed: {ex.Message}";
            _frameRateRecorder?.Dispose();
            _frameRateRecorder = null;
        }
    }

    private void StopFrameRateRecording(string reason)
    {
        if (_frameRateRecorder is null)
        {
            return;
        }

        try
        {
            double averageFps = _frameRateSessionValueCount > 0
                ? _frameRateSessionFpsTotal / _frameRateSessionValueCount
                : 0d;

            double minFps = _frameRateSessionValueCount > 0 ? _frameRateSessionMinFps : 0d;
            double maxFps = _frameRateSessionValueCount > 0 ? _frameRateSessionMaxFps : 0d;

            _logger.LogInformation(
                "Frame-rate session finished. Theme={ThemeName} Samples={Samples} Average={AverageFps:0.###} Min={MinFps:0.###} Max={MaxFps:0.###} Reason={Reason} File={FilePath}",
                _frameRateSessionThemeName,
                _frameRateSampleIndex,
                averageFps,
                minFps,
                maxFps,
                reason,
                _frameRateRecorder.FilePath);
        }
        finally
        {
            _frameRateRecorder.Dispose();
            _frameRateRecorder = null;
            _frameRateSessionThemeName = null;
            ResetFrameRateAccumulators();
        }
    }

    private void ResetFrameRateAccumulators()
    {
        _frameRateSampleIndex = 0;
        _frameRateWindowTickCount = 0;
        _frameRateWindowValueCount = 0;
        _frameRateSessionValueCount = 0;
        _frameRateWindowFpsTotal = 0d;
        _frameRateSessionFpsTotal = 0d;
        _frameRateSessionMinFps = double.MaxValue;
        _frameRateSessionMaxFps = double.MinValue;
    }

    private void RecordCurrentFrameRateSample()
    {
        if (_frameRateRecorder is null || _current is null)
        {
            return;
        }

        try
        {
            double fps = _clock.CurrentFramesPerSecond;
            _frameRateWindowFpsTotal += fps;
            _frameRateSessionFpsTotal += fps;
            _frameRateWindowValueCount++;
            _frameRateSessionValueCount++;
            _frameRateWindowTickCount++;
            _frameRateSessionMinFps = Math.Min(_frameRateSessionMinFps, fps);
            _frameRateSessionMaxFps = Math.Max(_frameRateSessionMaxFps, fps);

            if (_frameRateWindowTickCount < FrameRateSampleWindowTicks)
            {
                return;
            }

            _frameRateSampleIndex++;
            double averageFps = _frameRateWindowValueCount > 0
                ? _frameRateWindowFpsTotal / _frameRateWindowValueCount
                : 0d;

            _frameRateRecorder.WriteSample(
                DateTime.Now,
                _frameRateSampleIndex,
                FrameRateSampleWindowSeconds,
                averageFps,
                _current.Catalog.ThemeKey,
                _current.Catalog.GetConcreteDisplayName(_currentResolvedVariant, GetCurrentThemePeriod()),
                _kioskModeManager.FullScreen ? WindowPresentationMode.FullScreen : _presentationMode,
                _clock.VSyncEnabled,
                _kioskModeManager.AlwaysOn,
                GetOledViewEnabled());

            _frameRateWindowTickCount = 0;
            _frameRateWindowValueCount = 0;
            _frameRateWindowFpsTotal = 0d;

            if (_frameRateSampleIndex >= FrameRateSessionSampleCount)
            {
                StopFrameRateRecording("completed 2-minute sample window");
                _statusInfo.Text = "Frame-rate recording complete.";
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ObjectDisposedException)
        {
            _logger.LogWarning(ex, "Stopping frame-rate recording after a write failure.");
            StopFrameRateRecording("write failure");
            _statusInfo.Text = $"Frame-rate recording stopped: {ex.Message}";
        }
    }

    private async Task RunDebugCaptureAsync(int debugRunSeconds, CancellationToken cancellationToken)
    {
        string debugRunDirectory = _appPaths.CreateDiagnosticsRunDirectory("DebugRun");
        List<DebugRunCapture> captures = [];
        DateTime startedAt = DateTime.Now;

        try
        {
            _statusInfo.Text = $"Debug run: capturing every second for {debugRunSeconds}s";

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            Activate();
            BringToFront();

            await Task.Delay(750, cancellationToken);

            for (int second = 0; second <= debugRunSeconds; second++)
            {
                if (second > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }

                string prefix = $"second-{second:00}";
                CaptureDiagnosticsArtifacts(debugRunDirectory, prefix);
                captures.Add(new DebugRunCapture
                {
                    SecondOffset = second,
                    WindowImage = prefix + "-window.png",
                    ClockImage = prefix + "-clock.png",
                });
            }

            DebugRunMetadata metadata = CreateDebugRunMetadata(startedAt, DateTime.Now, debugRunSeconds, captures);
            Directory.CreateDirectory(debugRunDirectory);
            File.WriteAllText(
                Path.Combine(debugRunDirectory, "metadata.json"),
                JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));

            _logger.LogInformation("Debug-run capture completed under {Directory}.", debugRunDirectory);
            _statusInfo.Text = $"Debug run complete: {debugRunDirectory}";

            if (!IsDisposed && IsHandleCreated)
            {
                BeginInvoke(new Action(Close));
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ExternalException)
        {
            _logger.LogError(ex, "Debug-run capture failed.");
            _statusInfo.Text = $"Debug run failed: {ex.Message}";
        }
    }

    private DebugRunMetadata CreateDebugRunMetadata(
        DateTime startedAt,
        DateTime endedAt,
        int debugRunSeconds,
        IReadOnlyList<DebugRunCapture> captures)
        => new()
        {
            StartedAtLocal = startedAt,
            EndedAtLocal = endedAt,
            DurationSeconds = debugRunSeconds,
            CaptureCount = captures.Count,
            ThemeKey = _current?.Catalog.ThemeKey ?? string.Empty,
            ThemeDisplayName = _current?.Catalog.GetConcreteDisplayName(_currentResolvedVariant, GetCurrentThemePeriod()) ?? string.Empty,
            ThemeListName = _themeSchedule?.Name ?? string.Empty,
            CurrentThemeListPath = _currentThemeListPath,
            DefaultThemeListPath = _defaultThemeListPath,
            PresentationMode = _kioskModeManager.FullScreen ? WindowPresentationMode.FullScreen.ToString() : _presentationMode.ToString(),
            AlwaysOn = _kioskModeManager.AlwaysOn,
            VSyncEnabled = _clock.VSyncEnabled,
            OledView = GetOledViewEnabled(),
            RecordFramerateEnabled = _recordFramerateEnabled,
            WindowBounds = Bounds,
            Captures = captures.ToList(),
        };

    private void CaptureDiagnosticsArtifacts(string directory, string prefix)
    {
        Directory.CreateDirectory(directory);
        CaptureWindowScreenshot(Path.Combine(directory, $"{prefix}-window.png"));
        CaptureClockScreenshot(Path.Combine(directory, $"{prefix}-clock.png"));
    }

    private void CaptureWindowScreenshot(string path)
    {
        if (Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        using Bitmap bitmap = new(Bounds.Width, Bounds.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(Bounds.Location, Point.Empty, Bounds.Size);
        bitmap.Save(path, ImageFormat.Png);
    }

    private void CaptureClockScreenshot(string path)
    {
        if (_clock.ClientSize.Width <= 0 || _clock.ClientSize.Height <= 0)
        {
            return;
        }

        Point location = _clock.PointToScreen(Point.Empty);
        using Bitmap bitmap = new(_clock.ClientSize.Width, _clock.ClientSize.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(location, Point.Empty, _clock.ClientSize);
        bitmap.Save(path, ImageFormat.Png);
    }

    private sealed class DebugRunMetadata
    {
        public DateTime StartedAtLocal { get; set; }

        public DateTime EndedAtLocal { get; set; }

        public int DurationSeconds { get; set; }

        public int CaptureCount { get; set; }

        public string ThemeKey { get; set; } = string.Empty;

        public string ThemeDisplayName { get; set; } = string.Empty;

        public string ThemeListName { get; set; } = string.Empty;

        public string? CurrentThemeListPath { get; set; }

        public string? DefaultThemeListPath { get; set; }

        public string PresentationMode { get; set; } = string.Empty;

        public bool AlwaysOn { get; set; }

        public bool VSyncEnabled { get; set; }

        public bool OledView { get; set; }

        public bool RecordFramerateEnabled { get; set; }

        public Rectangle WindowBounds { get; set; }

        public List<DebugRunCapture> Captures { get; set; } = [];
    }

    private sealed class DebugRunCapture
    {
        public int SecondOffset { get; set; }

        public string WindowImage { get; set; } = string.Empty;

        public string ClockImage { get; set; } = string.Empty;
    }
}
