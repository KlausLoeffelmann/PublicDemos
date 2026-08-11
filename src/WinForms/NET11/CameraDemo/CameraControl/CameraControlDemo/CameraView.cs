using System.ComponentModel;
using Windows.Media.Capture.Frames;

namespace CameraControlDemo;

/// <summary>
///  A WinForms control that presents live camera frames through DirectX.
/// </summary>
/// <remarks>
///  <para>
///   Frames arrive on a Media Foundation callback thread rather than through
///   <see cref="OnPaint"/>. A GPU-backed frame is handed to Direct2D as a DXGI surface
///   and drawn into a flip-model swap chain. DirectComposition places that swap chain
///   over this control's HWND, so scaling and presentation happen on the GPU without
///   a GDI bitmap or a UI-thread blit.
///  </para>
///  <para>
///   Cameras that provide only CPU pixels use a documented fallback: their BGRA data
///   is uploaded into one reusable Direct2D bitmap and then follows the same swap-chain
///   path. The renderer drops a frame if it is still busy, preserving real-time
///   behavior instead of accumulating delayed frames.
///  </para>
///  <para>
///   While no frame is available, the composition visual is hidden and ordinary
///   WinForms painting displays <see cref="StatusText"/>. Native graphics resources
///   are created lazily for the current HWND and released when that handle is destroyed.
///  </para>
/// </remarks>
public class CameraView : Control
{
    /// <summary>
    ///  Guards renderer replacement and frame-size state.
    /// </summary>
    private readonly Lock _stateLock = new();

    /// <summary>
    ///  Native renderer targeting the control's current handle.
    /// </summary>
    private DirectXCameraRenderer? _renderer;

    /// <summary>
    ///  Resolution of the most recently presented frame.
    /// </summary>
    private Size _frameSize;

    /// <summary>
    ///  Non-zero after a frame has been presented and the composition visual is shown.
    /// </summary>
    private int _hasFrame;

    /// <summary>
    ///  Backing field for <see cref="KeepAspectRatio"/>.
    /// </summary>
    private volatile bool _keepAspectRatio = true;

    /// <summary>
    ///  Thread-safe ARGB snapshot of <see cref="Control.BackColor"/> for rendering.
    /// </summary>
    private int _backgroundArgb = Color.Black.ToArgb();

    /// <summary>
    ///  Backing field for <see cref="StatusText"/>.
    /// </summary>
    private string? _statusText;

    /// <summary>
    ///  Initializes a new instance of the <see cref="CameraView"/> class.
    /// </summary>
    public CameraView()
    {
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.Opaque
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.Black;
        ForeColor = Color.Gainsboro;
    }

    /// <summary>
    ///  Gets or sets whether the frame is centered and scaled to fit while preserving
    ///  its aspect ratio. When disabled, the frame is drawn 1:1 from the top-left.
    /// </summary>
    [DefaultValue(true)]
    [Category("Appearance")]
    [Description("Letterboxes the frame preserving its aspect ratio instead of drawing it unscaled at 1:1.")]
    public bool KeepAspectRatio
    {
        get => _keepAspectRatio;
        set => _keepAspectRatio = value;
    }

    /// <summary>
    ///  Gets or sets the message painted while no camera frame is visible.
    /// </summary>
    [DefaultValue(null)]
    [Category("Appearance")]
    [Description("Message painted centered in the client area while no frame is available.")]
    public string? StatusText
    {
        get => _statusText;
        set
        {
            if (_statusText == value)
            {
                return;
            }

            _statusText = value;

            if (Volatile.Read(ref _hasFrame) == 0)
            {
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Gets the resolution of the most recently presented frame, or
    ///  <see cref="Size.Empty"/> before the first frame.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Size FrameSize
    {
        get
        {
            lock (_stateLock)
            {
                return _frameSize;
            }
        }
    }

    /// <summary>
    ///  Returns the source resolution so layout can size the control optimally.
    /// </summary>
    /// <param name="proposedSize">The size proposed by the layout engine.</param>
    /// <returns>The source resolution, or the base size while it is unknown.</returns>
    public override Size GetPreferredSize(Size proposedSize)
    {
        Size frameSize = FrameSize;

        return frameSize.IsEmpty
            ? base.GetPreferredSize(proposedSize)
            : frameSize;
    }

    /// <summary>
    ///  Hides the current frame and returns to WinForms status painting.
    /// </summary>
    public void ClearFrame()
    {
        DirectXCameraRenderer? renderer;

        lock (_stateLock)
        {
            _frameSize = Size.Empty;
            renderer = _renderer;
        }

        Interlocked.Exchange(ref _hasFrame, 0);
        renderer?.Hide();
        Invalidate();
    }

    /// <summary>
    ///  Attempts to present a live video frame on the renderer's callback thread.
    /// </summary>
    /// <remarks>
    ///  The caller must keep the owning <see cref="MediaFrameReference"/> alive until
    ///  this method returns because a GPU surface is borrowed directly from that frame.
    /// </remarks>
    /// <param name="frame">The video frame supplied by the media frame reader.</param>
    /// <returns>
    ///  <see langword="true"/> when the frame was presented; otherwise
    ///  <see langword="false"/> when no renderer exists or a busy/device-lost frame was
    ///  intentionally dropped.
    /// </returns>
    internal bool TryPresentFrame(VideoMediaFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        DirectXCameraRenderer? renderer;

        lock (_stateLock)
        {
            renderer = _renderer;
        }

        if (renderer is null
            || !renderer.TryPresent(
                frame.Direct3DSurface,
                frame.SoftwareBitmap,
                _keepAspectRatio,
                Color.FromArgb(Volatile.Read(ref _backgroundArgb)),
                out Size frameSize))
        {
            return false;
        }

        lock (_stateLock)
        {
            _frameSize = frameSize;
        }

        Interlocked.Exchange(ref _hasFrame, 1);
        return true;
    }

    /// <summary>
    ///  Creates a renderer for the control's newly created HWND.
    /// </summary>
    /// <param name="e">Unused event data.</param>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        lock (_stateLock)
        {
            SynchronizationContext uiContext =
                SynchronizationContext.Current
                ?? throw new InvalidOperationException(
                    "CameraView must create its handle on a WinForms UI thread.");

            _renderer = new DirectXCameraRenderer(
                Handle,
                ClientSize,
                uiContext,
                Environment.CurrentManagedThreadId);
        }
    }

    /// <summary>
    ///  Releases graphics resources before the current HWND becomes invalid.
    /// </summary>
    /// <param name="e">Unused event data.</param>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        DirectXCameraRenderer? renderer;

        lock (_stateLock)
        {
            renderer = _renderer;
            _renderer = null;
            _frameSize = Size.Empty;
        }

        Interlocked.Exchange(ref _hasFrame, 0);
        renderer?.Dispose();

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    ///  Records a new swap-chain size after WinForms lays out the control.
    /// </summary>
    /// <param name="e">Unused event data.</param>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        lock (_stateLock)
        {
            _renderer?.Resize(ClientSize);
        }
    }

    /// <summary>
    ///  Snapshots the WinForms background color for the capture-thread renderer.
    /// </summary>
    /// <param name="e">Unused event data.</param>
    protected override void OnBackColorChanged(EventArgs e)
    {
        Volatile.Write(ref _backgroundArgb, BackColor.ToArgb());
        base.OnBackColorChanged(e);
    }

    /// <summary>
    ///  Paints the background and status message beneath the composition visual.
    /// </summary>
    /// <param name="e">The WinForms paint event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(BackColor);

        if (Volatile.Read(ref _hasFrame) == 0)
        {
            PaintStatus(e.Graphics);
        }
    }

    /// <summary>
    ///  Suppresses the separate erase pass because <see cref="OnPaint"/> covers the
    ///  complete client area.
    /// </summary>
    /// <param name="pevent">Unused paint event data.</param>
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
    }

    /// <summary>
    ///  Paints the current status message centered within the client area.
    /// </summary>
    /// <param name="graphics">The target WinForms graphics surface.</param>
    private void PaintStatus(Graphics graphics)
    {
        if (string.IsNullOrWhiteSpace(_statusText))
        {
            return;
        }

        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisWord
        };
        using SolidBrush brush = new(ForeColor);

        Rectangle bounds = Rectangle.Inflate(ClientRectangle, -16, -16);

        if (bounds.Width > 0 && bounds.Height > 0)
        {
            graphics.DrawString(_statusText, Font, brush, bounds, format);
        }
    }
}
