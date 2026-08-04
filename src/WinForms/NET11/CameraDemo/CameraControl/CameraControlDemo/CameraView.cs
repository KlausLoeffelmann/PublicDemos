using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CameraControlDemo;

/// <summary>
///  A lightweight, flicker-free surface that displays camera frames.
/// </summary>
/// <remarks>
///  <para>
///   This is deliberately a <see cref="Control"/> and not a <see cref="PictureBox"/>:
///   frames arrive on a capture thread at up to 60 Hz and are blitted into a single
///   reusable back buffer, so there is no per-frame allocation and no intermediate
///   image assignment.
///  </para>
/// </remarks>
public class CameraView : Control
{
    /// <summary>
    ///  Guards <see cref="_backBuffer"/> against concurrent access by the capture
    ///  thread (writing pixels) and the UI thread (painting).
    /// </summary>
    private readonly Lock _bufferLock = new();

    /// <summary>
    ///  The single reusable back buffer. Re-created only when the source resolution
    ///  changes.
    /// </summary>
    private Bitmap? _backBuffer;

    /// <summary>
    ///  Non-zero while an <see cref="Control.Invalidate()"/> has been requested but the
    ///  corresponding <see cref="OnPaint"/> has not completed yet. Used to drop frames
    ///  instead of queueing them.
    /// </summary>
    private int _repaintPending;

    /// <summary>
    ///  <see langword="true"/> once at least one frame has been written into the back
    ///  buffer.
    /// </summary>
    private bool _hasFrame;

    /// <summary>
    ///  Backing field for <see cref="KeepAspectRatio"/>.
    /// </summary>
    private bool _keepAspectRatio = true;

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
    ///  Gets or sets a value indicating whether the frame is letterboxed into the
    ///  client area preserving its aspect ratio (<see langword="true"/>, the default),
    ///  or drawn unscaled 1:1 at the top-left corner (<see langword="false"/>).
    /// </summary>
    [DefaultValue(true)]
    [Category("Appearance")]
    [Description("Letterboxes the frame preserving its aspect ratio instead of drawing it unscaled at 1:1.")]
    public bool KeepAspectRatio
    {
        get => _keepAspectRatio;
        set
        {
            if (_keepAspectRatio == value)
            {
                return;
            }

            _keepAspectRatio = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets a message that is painted centered in the client area whenever no
    ///  frame is available - used for "starting camera", "no camera found" and error
    ///  text, so the control never just stays black without a clue why.
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

            if (!_hasFrame)
            {
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Gets the resolution of the frames currently being displayed, or
    ///  <see cref="Size.Empty"/> when no frame has been received yet.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Size FrameSize
    {
        get
        {
            lock (_bufferLock)
            {
                return _backBuffer is null
                    ? Size.Empty
                    : _backBuffer.Size;
            }
        }
    }

    /// <summary>
    ///  Returns the native source resolution so layout can size the control optimally.
    /// </summary>
    /// <param name="proposedSize">The size proposed by the layout engine.</param>
    /// <returns>The source resolution, or the base implementation while unknown.</returns>
    public override Size GetPreferredSize(Size proposedSize)
    {
        Size frameSize = FrameSize;

        return frameSize.IsEmpty
            ? base.GetPreferredSize(proposedSize)
            : frameSize;
    }

    /// <summary>
    ///  Discards the current frame so the control falls back to painting
    ///  <see cref="StatusText"/>.
    /// </summary>
    public void ClearFrame()
    {
        lock (_bufferLock)
        {
            _hasFrame = false;
        }

        Invalidate();
    }

    /// <summary>
    ///  Copies a frame into the back buffer and schedules a repaint.
    /// </summary>
    /// <remarks>
    ///  Called from the capture thread. When a repaint is still pending the frame is
    ///  dropped rather than queued, which keeps the preview at real time even when the
    ///  UI thread cannot keep up.
    /// </remarks>
    /// <param name="width">Frame width in pixels.</param>
    /// <param name="height">Frame height in pixels.</param>
    /// <param name="copy">
    ///  Callback that performs the actual pixel copy into the locked back buffer.
    /// </param>
    /// <returns>
    ///  <see langword="true"/> when the frame was copied; <see langword="false"/> when
    ///  it was dropped because a repaint is still outstanding.
    /// </returns>
    public bool TryWriteFrame(int width, int height, Action<BitmapData> copy)
    {
        ArgumentNullException.ThrowIfNull(copy);

        if (width <= 0 || height <= 0)
        {
            return false;
        }

        // Drop-don't-queue: a repaint from the previous frame is still outstanding.
        if (Interlocked.CompareExchange(ref _repaintPending, 1, 0) != 0)
        {
            return false;
        }

        try
        {
            lock (_bufferLock)
            {
                Bitmap buffer = EnsureBackBuffer(width, height);

                BitmapData data = buffer.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppPArgb);

                try
                {
                    copy(data);
                }
                finally
                {
                    buffer.UnlockBits(data);
                }

                _hasFrame = true;
            }
        }
        catch
        {
            Interlocked.Exchange(ref _repaintPending, 0);
            throw;
        }

        try
        {
            _ = InvokeAsync(() => Invalidate());
        }
        catch (Exception) when (IsDisposed || Disposing || !IsHandleCreated)
        {
            Interlocked.Exchange(ref _repaintPending, 0);
        }

        return true;
    }

    /// <summary>
    ///  Paints the current frame, or the status text when there is none.
    /// </summary>
    /// <param name="e">The paint event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        try
        {
            lock (_bufferLock)
            {
                if (!_hasFrame || _backBuffer is null)
                {
                    PaintStatus(e.Graphics);
                    return;
                }

                if (_keepAspectRatio)
                {
                    PaintLetterboxed(e.Graphics, _backBuffer);
                }
                else
                {
                    PaintUnscaled(e.Graphics, _backBuffer);
                }
            }
        }
        finally
        {
            // Release the gate so the next frame is accepted.
            Interlocked.Exchange(ref _repaintPending, 0);
        }
    }

    /// <summary>
    ///  Suppressed on purpose: the control paints every pixel of its client area
    ///  itself, so erasing the background first would only cause flicker.
    /// </summary>
    /// <param name="pevent">The paint event data.</param>
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
    }

    /// <summary>
    ///  Releases the back buffer.
    /// </summary>
    /// <param name="disposing">
    ///  <see langword="true"/> to release managed resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (_bufferLock)
            {
                _backBuffer?.Dispose();
                _backBuffer = null;
                _hasFrame = false;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Returns the back buffer, re-creating it when the source resolution changed.
    /// </summary>
    /// <param name="width">Required width in pixels.</param>
    /// <param name="height">Required height in pixels.</param>
    /// <returns>A back buffer of exactly the requested size.</returns>
    private Bitmap EnsureBackBuffer(int width, int height)
    {
        if (_backBuffer is not null
            && _backBuffer.Width == width
            && _backBuffer.Height == height)
        {
            return _backBuffer;
        }

        _backBuffer?.Dispose();
        _backBuffer = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
        _hasFrame = false;

        return _backBuffer;
    }

    /// <summary>
    ///  Draws the frame 1:1 at the origin without any scaling or filtering.
    /// </summary>
    /// <param name="graphics">The target graphics.</param>
    /// <param name="frame">The frame to draw.</param>
    private void PaintUnscaled(Graphics graphics, Bitmap frame)
    {
        // These must be set *before* drawing, otherwise GDI+ still filters.
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        graphics.SmoothingMode = SmoothingMode.None;

        graphics.DrawImageUnscaled(frame, 0, 0);

        // Whatever the frame does not cover keeps the BackColor.
        graphics.CompositingMode = CompositingMode.SourceOver;
        FillAround(graphics, new Rectangle(0, 0, frame.Width, frame.Height));
    }

    /// <summary>
    ///  Draws the frame centered and letterboxed, preserving the source aspect ratio.
    /// </summary>
    /// <param name="graphics">The target graphics.</param>
    /// <param name="frame">The frame to draw.</param>
    private void PaintLetterboxed(Graphics graphics, Bitmap frame)
    {
        Rectangle target = GetLetterboxRectangle(frame.Width, frame.Height);

        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        graphics.SmoothingMode = SmoothingMode.None;

        graphics.InterpolationMode =
            target.Width == frame.Width && target.Height == frame.Height
                ? InterpolationMode.NearestNeighbor
                : InterpolationMode.HighQualityBilinear;

        FillAround(graphics, target);
        graphics.DrawImage(frame, target);
    }

    /// <summary>
    ///  Computes the centered destination rectangle that preserves the source aspect
    ///  ratio inside the client area.
    /// </summary>
    /// <param name="sourceWidth">Source width in pixels.</param>
    /// <param name="sourceHeight">Source height in pixels.</param>
    /// <returns>The destination rectangle.</returns>
    private Rectangle GetLetterboxRectangle(int sourceWidth, int sourceHeight)
    {
        Rectangle client = ClientRectangle;

        if (client.Width <= 0 || client.Height <= 0 || sourceWidth <= 0 || sourceHeight <= 0)
        {
            return Rectangle.Empty;
        }

        double scale = Math.Min(
            client.Width / (double)sourceWidth,
            client.Height / (double)sourceHeight);

        int width = Math.Max(1, (int)Math.Round(sourceWidth * scale));
        int height = Math.Max(1, (int)Math.Round(sourceHeight * scale));

        return new Rectangle(
            client.X + ((client.Width - width) / 2),
            client.Y + ((client.Height - height) / 2),
            width,
            height);
    }

    /// <summary>
    ///  Fills the part of the client area that the frame does not cover with
    ///  <see cref="Control.BackColor"/>.
    /// </summary>
    /// <param name="graphics">The target graphics.</param>
    /// <param name="covered">The rectangle covered by the frame.</param>
    private void FillAround(Graphics graphics, Rectangle covered)
    {
        Rectangle client = ClientRectangle;

        if (covered.Contains(client))
        {
            return;
        }

        using SolidBrush brush = new(BackColor);

        // Top, bottom, left, right bands around the covered area.
        if (covered.Top > client.Top)
        {
            graphics.FillRectangle(
                brush,
                client.X, client.Y, client.Width, covered.Top - client.Top);
        }

        if (covered.Bottom < client.Bottom)
        {
            graphics.FillRectangle(
                brush,
                client.X, covered.Bottom, client.Width, client.Bottom - covered.Bottom);
        }

        if (covered.Left > client.Left)
        {
            graphics.FillRectangle(
                brush,
                client.X, covered.Y, covered.Left - client.Left, covered.Height);
        }

        if (covered.Right < client.Right)
        {
            graphics.FillRectangle(
                brush,
                covered.Right, covered.Y, client.Right - covered.Right, covered.Height);
        }
    }

    /// <summary>
    ///  Paints the background and the current <see cref="StatusText"/>.
    /// </summary>
    /// <param name="graphics">The target graphics.</param>
    private void PaintStatus(Graphics graphics)
    {
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.Clear(BackColor);

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
