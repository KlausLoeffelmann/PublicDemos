using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;

namespace CameraControlDemo;

/// <summary>
///  A selectable camera, i.e. a <see cref="MediaFrameSourceGroup"/> that offers at
///  least one color source.
/// </summary>
/// <param name="Group">The underlying frame source group.</param>
public sealed record CameraDevice(MediaFrameSourceGroup Group)
{
    /// <summary>
    ///  Gets the human readable device name shown in the device list.
    /// </summary>
    public string DisplayName => Group.DisplayName;

    /// <summary>
    ///  Returns the display name so the type can be bound directly to a ComboBox.
    /// </summary>
    /// <returns>The device display name.</returns>
    public override string ToString() => DisplayName;
}

/// <summary>
///  A selectable capture format of a camera.
/// </summary>
/// <param name="Format">The underlying media frame format.</param>
public sealed record CameraFormat(MediaFrameFormat Format)
{
    /// <summary>
    ///  Gets the frame width in pixels.
    /// </summary>
    public int Width => (int)Format.VideoFormat.Width;

    /// <summary>
    ///  Gets the frame height in pixels.
    /// </summary>
    public int Height => (int)Format.VideoFormat.Height;

    /// <summary>
    ///  Gets the nominal frame rate in frames per second.
    /// </summary>
    public double FrameRate => Format.FrameRate.Denominator == 0
        ? 0
        : Format.FrameRate.Numerator / (double)Format.FrameRate.Denominator;

    /// <summary>
    ///  Returns a "1920 x 1080 @ 30 fps (NV12)" style description for the ComboBox.
    /// </summary>
    /// <returns>The format description.</returns>
    public override string ToString()
        => $"{Width} x {Height} @ {FrameRate:0.##} fps ({Format.Subtype})";
}

/// <summary>
///  Carries a capture failure to the UI.
/// </summary>
/// <param name="message">A message suitable for display to the user.</param>
/// <param name="exception">The underlying exception, when there is one.</param>
public sealed class CameraErrorEventArgs(string message, Exception? exception = null)
    : EventArgs
{
    /// <summary>
    ///  Gets a message suitable for display to the user.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    ///  Gets the underlying exception, if any.
    /// </summary>
    public Exception? Exception { get; } = exception;
}

/// <summary>
///  Drives a <see cref="MediaCapture"/> based preview and pushes every frame into a
///  <see cref="CameraView"/>.
/// </summary>
/// <remarks>
///  The frame reader asks Media Foundation for BGRA8 in realtime mode and prefers
///  GPU-backed memory. Each callback passes the live <see cref="VideoMediaFrame"/> to
///  the view, which can present its Direct3D surface without a CPU copy. When a device
///  supplies only a software bitmap, the view uploads that bitmap through its DirectX
///  fallback. Holding the frame reference until presentation returns keeps either
///  backing store valid.
/// </remarks>
/// <param name="view">The view that renders the frames.</param>
public sealed class CameraCapture(CameraView view) : IAsyncDisposable
{
    /// <summary>
    ///  The view that presents captured frames.
    /// </summary>
    private readonly CameraView _view = view
        ?? throw new ArgumentNullException(nameof(view));

    /// <summary>
    ///  Serializes <see cref="StartAsync"/> and <see cref="StopAsync"/> so switching
    ///  device or format can never overlap with a running teardown.
    /// </summary>
    private readonly SemaphoreSlim _gate = new(1, 1);

    /// <summary>
    ///  The active capture session, or <see langword="null"/> while stopped.
    /// </summary>
    private MediaCapture? _mediaCapture;

    /// <summary>
    ///  The active frame reader, or <see langword="null"/> while stopped.
    /// </summary>
    private MediaFrameReader? _frameReader;

    /// <summary>
    ///  Non-zero once a frame handler failure has been reported for the current
    ///  session, so the error is surfaced exactly once instead of per frame.
    /// </summary>
    private int _frameErrorReported;

    /// <summary>
    ///  Raised when capture fails, either during start-up or in the frame handler.
    /// </summary>
    public event EventHandler<CameraErrorEventArgs>? Error;

    /// <summary>
    ///  Gets the capture formats of the currently selected source, in a stable order.
    /// </summary>
    public IReadOnlyList<CameraFormat> SupportedFormats { get; private set; } = [];

    /// <summary>
    ///  Enumerates all frame source groups that expose a color video source.
    /// </summary>
    /// <returns>The selectable camera devices.</returns>
    public static async Task<IReadOnlyList<CameraDevice>> GetDevicesAsync()
    {
        IReadOnlyList<MediaFrameSourceGroup> groups =
            await MediaFrameSourceGroup.FindAllAsync();

        return [.. groups
            .Where(group => group.SourceInfos.Any(IsColorVideoSource))
            .Select(group => new CameraDevice(group))];
    }

    /// <summary>
    ///  Starts previewing the given device, optionally with an explicit capture format.
    /// </summary>
    /// <param name="device">The device to preview.</param>
    /// <param name="format">
    ///  The capture format to apply, or <see langword="null"/> to keep the source
    ///  default and simply publish the available formats.
    /// </param>
    /// <returns>
    ///  <see langword="true"/> when the preview was started successfully.
    /// </returns>
    public async Task<bool> StartAsync(CameraDevice device, CameraFormat? format)
    {
        ArgumentNullException.ThrowIfNull(device);

        await _gate.WaitAsync();

        try
        {
            // Always tear the previous session down completely first - that is what
            // makes switching device or format race free.
            await StopCoreAsync();

            Interlocked.Exchange(ref _frameErrorReported, 0);

            MediaCapture mediaCapture = new();

            MediaCaptureInitializationSettings settings = new()
            {
                SourceGroup = device.Group,
                MemoryPreference = MediaCaptureMemoryPreference.Auto,
                StreamingCaptureMode = StreamingCaptureMode.Video,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl
            };

            try
            {
                await mediaCapture.InitializeAsync(settings);
            }
            catch (UnauthorizedAccessException ex)
            {
                mediaCapture.Dispose();

                OnError(
                    "Access to the camera was denied. Enable it under "
                        + "Settings > Privacy & security > Camera and make sure "
                        + "\"Let desktop apps access your camera\" is switched on.",
                    ex);

                return false;
            }
            catch (Exception ex)
            {
                mediaCapture.Dispose();
                OnError($"The camera could not be initialized: {ex.Message}", ex);

                return false;
            }

            _mediaCapture = mediaCapture;

            MediaFrameSource? source = mediaCapture.FrameSources.Values
                .FirstOrDefault(candidate => IsColorVideoSource(candidate.Info));

            if (source is null)
            {
                await StopCoreAsync();
                OnError("The selected device does not expose a color video source.");

                return false;
            }

            SupportedFormats = [.. source.SupportedFormats
                .Where(candidate => candidate.VideoFormat is not null)
                .Select(candidate => new CameraFormat(candidate))
                .OrderByDescending(candidate => candidate.Width * candidate.Height)
                .ThenByDescending(candidate => candidate.FrameRate)];

            if (format is not null)
            {
                // Must happen before the reader exists, otherwise the reader keeps
                // delivering the old geometry.
                await source.SetFormatAsync(format.Format);
            }

            // Bgra8 lets Media Foundation do the conversion - never unpack NV12 here.
            MediaFrameReader reader = await mediaCapture.CreateFrameReaderAsync(
                source,
                MediaEncodingSubtypes.Bgra8);

            reader.AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime;
            reader.FrameArrived += OnFrameArrived;

            _frameReader = reader;

            MediaFrameReaderStartStatus status = await reader.StartAsync();

            if (status != MediaFrameReaderStartStatus.Success)
            {
                await StopCoreAsync();
                OnError($"The camera preview could not be started ({status}).");

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            await StopCoreAsync();
            OnError($"The camera preview could not be started: {ex.Message}", ex);

            return false;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    ///  Stops the preview and releases the reader and the capture device.
    /// </summary>
    /// <returns>A task that completes once everything is torn down.</returns>
    public async Task StopAsync()
    {
        await _gate.WaitAsync();

        try
        {
            await StopCoreAsync();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    ///  Stops the preview and releases all resources.
    /// </summary>
    /// <returns>A task that completes once everything is torn down.</returns>
    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _gate.Dispose();
    }

    /// <summary>
    ///  Determines whether a source info describes a color video stream.
    /// </summary>
    /// <param name="info">The source info to test.</param>
    /// <returns><see langword="true"/> for color preview/record streams.</returns>
    private static bool IsColorVideoSource(MediaFrameSourceInfo info)
        => info.SourceKind == MediaFrameSourceKind.Color
            && info.MediaStreamType is MediaStreamType.VideoPreview
                or MediaStreamType.VideoRecord;

    /// <summary>
    ///  Performs the actual teardown. The caller must hold <see cref="_gate"/>.
    /// </summary>
    /// <returns>A task that completes once everything is torn down.</returns>
    private async Task StopCoreAsync()
    {
        MediaFrameReader? reader = _frameReader;
        _frameReader = null;

        if (reader is not null)
        {
            reader.FrameArrived -= OnFrameArrived;

            try
            {
                await reader.StopAsync();
            }
            catch (Exception)
            {
                // The reader may already be gone - nothing useful to report here.
            }

            reader.Dispose();
        }

        MediaCapture? mediaCapture = _mediaCapture;
        _mediaCapture = null;
        mediaCapture?.Dispose();

        SupportedFormats = [];
    }

    /// <summary>
    ///  Copies the arrived frame into the view's back buffer. Runs on a capture thread.
    /// </summary>
    /// <param name="sender">The frame reader.</param>
    /// <param name="args">Unused event data.</param>
    private void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
    {
        try
        {
            using MediaFrameReference? frame = sender.TryAcquireLatestFrame();

            VideoMediaFrame? videoFrame = frame?.VideoMediaFrame;

            if (videoFrame is null)
            {
                return;
            }

            _view.TryPresentFrame(videoFrame);
        }
        catch (Exception ex)
        {
            // Report the first failure per session; without this the control would
            // just stay black with no indication of why.
            if (Interlocked.Exchange(ref _frameErrorReported, 1) == 0)
            {
                OnError($"The camera frame could not be processed: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    ///  Raises the <see cref="Error"/> event.
    /// </summary>
    /// <param name="message">A message suitable for display to the user.</param>
    /// <param name="exception">The underlying exception, when there is one.</param>
    private void OnError(string message, Exception? exception = null)
        => Error?.Invoke(this, new CameraErrorEventArgs(message, exception));
}
