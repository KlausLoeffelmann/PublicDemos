using System.ComponentModel;
using System.Diagnostics;

namespace WarpClock.App;

/// <summary>
///  A smooth, app-owned ticker band that scrolls one composed line from right to left.
/// </summary>
public sealed class TickerBandControl : Control
{
    private readonly System.Windows.Forms.Timer _animationTimer;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private string _tickerText = string.Empty;
    private float _scrollOffset;
    private long _lastTimestamp;

    public TickerBandControl()
    {
        DoubleBuffered = true;
        SetStyle(
            ControlStyles.UserPaint
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.FromArgb(14, 20, 30);
        ForeColor = Color.FromArgb(190, 208, 224);
        Padding = new Padding(12, 4, 12, 4);

        _animationTimer = new System.Windows.Forms.Timer
        {
            Interval = 16,
        };
        _animationTimer.Tick += OnAnimationTick;
    }

    /// <summary>The complete line displayed by the ticker.</summary>
    [DefaultValue("")]
    public string TickerText
    {
        get => _tickerText;
        set
        {
            string normalized = value ?? string.Empty;
            if (string.Equals(_tickerText, normalized, StringComparison.Ordinal))
            {
                return;
            }

            _tickerText = normalized;
            ResetScrollPosition();
            Invalidate();
        }
    }

    /// <summary>The horizontal travel speed in device-independent pixels per second.</summary>
    [DefaultValue(72f)]
    public float ScrollSpeed { get; set; } = 72f;

    /// <inheritdoc/>
    public override Size GetPreferredSize(Size proposedSize)
    {
        int textHeight = TextRenderer.MeasureText(
            "Ag",
            Font,
            Size.Empty,
            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Height;

        return new Size(
            Math.Max(1, proposedSize.Width),
            Math.Max(1, textHeight + Padding.Vertical + 2));
    }

    /// <inheritdoc/>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RestartAnimation();
    }

    /// <inheritdoc/>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        _animationTimer.Stop();
        base.OnHandleDestroyed(e);
    }

    /// <inheritdoc/>
    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);

        if (Visible && IsHandleCreated)
        {
            RestartAnimation();
        }
        else
        {
            _animationTimer.Stop();
        }
    }

    /// <inheritdoc/>
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        ResetScrollPosition();
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        ResetScrollPosition();
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(BackColor);

        if (string.IsNullOrEmpty(_tickerText))
        {
            return;
        }

        TextFormatFlags flags =
            TextFormatFlags.NoPadding
            | TextFormatFlags.NoPrefix
            | TextFormatFlags.SingleLine
            | TextFormatFlags.VerticalCenter;
        Size textSize = TextRenderer.MeasureText(e.Graphics, _tickerText, Font, Size.Empty, flags);
        int y = Padding.Top;
        int height = Math.Max(1, ClientSize.Height - Padding.Vertical);
        int spacing = Math.Max(48, ClientSize.Width / 5);
        int firstX = (int)MathF.Round(_scrollOffset);

        TextRenderer.DrawText(
            e.Graphics,
            _tickerText,
            Font,
            new Rectangle(firstX, y, textSize.Width, height),
            ForeColor,
            flags);

        int repeatedX = firstX + textSize.Width + spacing;
        if (repeatedX < ClientSize.Width)
        {
            TextRenderer.DrawText(
                e.Graphics,
                _tickerText,
                Font,
                new Rectangle(repeatedX, y, textSize.Width, height),
                ForeColor,
                flags);
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= OnAnimationTick;
            _animationTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        long now = _clock.ElapsedTicks;
        double elapsedSeconds = _lastTimestamp == 0
            ? 0d
            : (now - _lastTimestamp) / (double)Stopwatch.Frequency;
        _lastTimestamp = now;

        if (elapsedSeconds <= 0d || string.IsNullOrEmpty(_tickerText))
        {
            return;
        }

        float dpiScale = DeviceDpi / 96f;
        _scrollOffset -= (float)(elapsedSeconds * Math.Max(1f, ScrollSpeed) * dpiScale);

        Size textSize = TextRenderer.MeasureText(
            _tickerText,
            Font,
            Size.Empty,
            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);
        int spacing = Math.Max(48, ClientSize.Width / 5);
        if (_scrollOffset <= -textSize.Width - spacing)
        {
            _scrollOffset += textSize.Width + spacing;
        }

        Invalidate();
    }

    private void RestartAnimation()
    {
        _lastTimestamp = _clock.ElapsedTicks;
        ResetScrollPosition();
        _animationTimer.Start();
    }

    private void ResetScrollPosition()
    {
        _scrollOffset = Math.Max(Padding.Left, ClientSize.Width);
        _lastTimestamp = _clock.ElapsedTicks;
        Invalidate();
    }
}
