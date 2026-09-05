using SplitFlap.Audio.Analysis;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SplitFlap.Audio.WinForms;

/// <summary>
///  Paints finished spectrum snapshots; neither construction nor painting opens a device or runs an FFT.
/// </summary>
[ToolboxItem(true)]
public class AudioSpectrumControl : Control
{
    private static readonly int[] s_frequencies = [20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000];
    private static readonly string[] s_frequencyLabels = ["20", "50", "100", "200", "500", "1k", "2k", "5k", "10k", "20k Hz"];
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 33 };
    private AudioSpectrumSource? _source;
    private float[] _decibels = [];
    private PointF[] _points = [];
    private AxisTick[] _levelTicks = [];
    private AudioSpectrumFrame _frame;
    private Rectangle _plot;
    private Pen? _gridPen;
    private Pen? _spectrumPen;
    private Pen? _peakPen;
    private int _firstBin;
    private double _maximumFrequency = 20000;
    private double _logFrequencyRange = Math.Log(1000);
    private bool _hasFrame;
    private string _levelsText = string.Empty;
    private string _stateText = "No spectrum source.";

    /// <summary>
    ///  Creates an inert, Designer-safe control; a host supplies its runtime Source separately.
    /// </summary>
    public AudioSpectrumControl()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        BackColor = SystemColors.Window;
        ForeColor = SystemColors.WindowText;
        TabStop = false;
        _timer.Tick += RefreshSpectrum;
        RebuildGeometry();
    }

    /// <summary>
    ///  Gets or sets the caller-owned analyzer; replacing or disposing this control never disposes it.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AudioSpectrumSource? Source
    {
        get => _source;
        set
        {
            if (ReferenceEquals(_source, value))
            {
                return;
            }

            _source = value;
            _decibels = value is null ? [] : new float[value.BinCount];
            _frame = default;
            _hasFrame = false;
            _levelsText = string.Empty;
            _stateText = value is null ? "No spectrum source." : "Waiting for a complete output window...";
            RebuildGeometry();
            UpdateTimer();
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets the natural size used when the control is first placed in the Designer.
    /// </summary>
    protected override Size DefaultSize
        => new(640, 280);

    /// <summary>
    ///  Starts UI refresh only after a runtime handle exists.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RebuildGeometry();
        UpdateTimer();
    }

    /// <summary>
    ///  Stops UI refresh before the handle is destroyed.
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        _timer.Stop();
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    ///  Avoids refreshing an invisible surface without affecting the caller's analyzer.
    /// </summary>
    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        UpdateTimer();
    }

    /// <summary>
    ///  Recomputes logarithmic bin positions when the available surface changes.
    /// </summary>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        RebuildGeometry();
    }

    /// <summary>
    ///  Reserves label space using the inherited, already-DPI-aware font.
    /// </summary>
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RebuildGeometry();
        Invalidate();
    }

    /// <summary>
    ///  Recomputes geometry after host-supplied padding changes.
    /// </summary>
    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        RebuildGeometry();
        Invalidate();
    }

    /// <summary>
    ///  Recreates cached strokes and geometry after a per-monitor DPI transition.
    /// </summary>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        ReleasePens();
        RebuildGeometry();
        Invalidate();
    }

    /// <summary>
    ///  Recreates color-bearing GDI resources when the system theme changes.
    /// </summary>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        ReleasePens();
        Invalidate();
    }

    /// <summary>
    ///  Draws only the UI-owned copied snapshot and cached geometry.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics graphics = e.Graphics;
        graphics.Clear(BackColor);
        float scale = DeviceDpi / 96f;
        if (_plot.Width < 40 * scale || _plot.Height < 30 * scale)
        {
            TextRenderer.DrawText(graphics, _stateText, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            return;
        }

        _gridPen ??= new Pen(SystemColors.ControlDark, Math.Max(1, scale));
        _spectrumPen ??= new Pen(SystemColors.Highlight, Math.Max(1, 1.8f * scale));
        _peakPen ??= new Pen(SystemColors.HotTrack, Math.Max(1, scale)) { DashStyle = DashStyle.Dot };
        graphics.SmoothingMode = SmoothingMode.None;

        foreach (AxisTick tick in _levelTicks)
        {
            float y = LevelToY(tick.Level);
            graphics.DrawLine(_gridPen, _plot.Left, y, _plot.Right, y);
            Rectangle label = new(Padding.Left, (int)y - Font.Height / 2, _plot.Left - Padding.Left - 6, Font.Height);
            TextRenderer.DrawText(graphics, tick.Text, Font, label, ForeColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        float lastLabel = float.NegativeInfinity;
        int labelWidth = (int)(54 * scale);
        for (int i = 0; i < s_frequencies.Length; i++)
        {
            if (s_frequencies[i] > _maximumFrequency || _logFrequencyRange <= 0)
            {
                continue;
            }

            float x = FrequencyToX(s_frequencies[i]);
            graphics.DrawLine(_gridPen, x, _plot.Top, x, _plot.Bottom);
            if (x - lastLabel >= labelWidth)
            {
                Rectangle label = new((int)x - labelWidth / 2, _plot.Bottom + 2, labelWidth, Font.Height);
                TextRenderer.DrawText(graphics, s_frequencyLabels[i], Font, label, ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
                lastLabel = x;
            }
        }

        if (_hasFrame && _points.Length >= 2)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawLines(_spectrumPen, _points);
            if (_frame.PeakFrequency >= 20 && _frame.PeakFrequency <= _maximumFrequency)
            {
                float x = FrequencyToX(_frame.PeakFrequency);
                graphics.DrawLine(_peakPen, x, _plot.Top, x, _plot.Bottom);
            }
        }

        Rectangle levels = new(_plot.Left, _plot.Bottom + Font.Height + (int)(8 * scale), _plot.Width, Font.Height);
        Rectangle state = new(levels.Left, levels.Bottom, levels.Width, Font.Height);
        TextRenderer.DrawText(graphics, _levelsText, Font, levels, ForeColor,
            TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        if (_hasFrame && _frame.ClippedSamples != 0)
        {
            graphics.FillRectangle(SystemBrushes.Highlight, state);
            TextRenderer.DrawText(graphics, _stateText, Font, state, SystemColors.HighlightText,
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }
        else
        {
            TextRenderer.DrawText(graphics, _stateText, Font, state, ForeColor,
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }
    }

    /// <summary>
    ///  Releases this control's timer and GDI resources, but not its caller-owned Source.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Stop();
            _timer.Tick -= RefreshSpectrum;
            _timer.Dispose();
            ReleasePens();
            _source = null;
        }

        base.Dispose(disposing);
    }

    private void RefreshSpectrum(object? sender, EventArgs e)
    {
        if (_source is null)
        {
            return;
        }

        if (_source.TryCopySpectrum(_decibels, out AudioSpectrumFrame frame))
        {
            bool geometryChanged = !_hasFrame || frame.SampleRate != _frame.SampleRate || frame.FftSize != _frame.FftSize;
            _frame = frame;
            _hasFrame = true;
            if (geometryChanged)
            {
                RebuildGeometry();
            }

            UpdatePoints();
            _levelsText = $"Peak {_frame.PeakFrequency:0.#} Hz  ·  {_frame.PeakLevel:0.0} dBFS  ·  RMS {_frame.RmsLevel:0.0} dBFS";
            string timing = frame.IsPlaybackSynchronized ? "Played buffers" : "Submitted stream (no device clock)";
            _stateText = $"{timing}  ·  Clips {frame.ClippedSamples}  ·  Dropped blocks {frame.DroppedBlocks}";
        }
        else
        {
            _hasFrame = false;
            _levelsText = string.Empty;
            _stateText = _source.Completion.IsFaulted
                ? "Spectrum analyzer failed; observe Source.Completion for details."
                : _source.Completion.IsCompleted ? "Spectrum stopped." : "Waiting for a contiguous output window...";
        }

        if (_source.Completion.IsCompleted)
        {
            _timer.Stop();
        }

        Invalidate();
    }

    private void UpdateTimer()
    {
        if (_source?.Completion.IsCompleted == true)
        {
            _hasFrame = false;
            _levelsText = string.Empty;
            _stateText = _source.Completion.IsFaulted ? "Spectrum analyzer failed." : "Spectrum stopped.";
            Invalidate();
        }

        if (IsHandleCreated && Visible && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime &&
            _source is not null && !_source.Completion.IsCompleted)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }
    }

    private void RebuildGeometry()
    {
        float scale = DeviceDpi / 96f;
        int edge = (int)Math.Ceiling(8 * scale);
        int labelWidth = Math.Max((int)(40 * scale), Font.Height * 3);
        _plot = new(
            Padding.Left + labelWidth,
            Padding.Top + edge,
            Math.Max(0, ClientSize.Width - Padding.Horizontal - labelWidth - (int)(32 * scale)),
            Math.Max(0, ClientSize.Height - Padding.Vertical - edge * 3 - Font.Height * 3));

        float minimum = _source?.Options.MinimumDecibels ?? -90;
        _levelTicks = new AxisTick[(int)Math.Ceiling(-minimum / 20) + 1];
        for (int i = 0; i < _levelTicks.Length; i++)
        {
            float level = Math.Max(minimum, -20 * i);
            _levelTicks[i] = new(level, i == 0 ? "0 dBFS" : $"{level:0.#}");
        }

        int sampleRate = _hasFrame ? _frame.SampleRate : 48_000;
        int fftSize = _source?.Options.FftSize ?? 4096;
        _maximumFrequency = Math.Min(20000, sampleRate / 2d);
        _logFrequencyRange = Math.Log(_maximumFrequency / 20);
        _firstBin = Math.Max(1, (int)Math.Ceiling(20d * fftSize / sampleRate));
        int lastBin = Math.Min(fftSize / 2, (int)Math.Floor(_maximumFrequency * fftSize / sampleRate));
        int count = _source is null || _logFrequencyRange <= 0 ? 0 : Math.Max(0, lastBin - _firstBin + 1);
        if (_points.Length != count)
        {
            _points = new PointF[count];
        }

        // FFT bins are evenly spaced in Hz, not in screen pixels. Cache their logarithmic
        // positions on resize; the timer only updates heights from finished dB values.
        for (int i = 0; i < _points.Length; i++)
        {
            double frequency = (double)(i + _firstBin) * sampleRate / fftSize;
            _points[i].X = FrequencyToX(frequency);
        }

        UpdatePoints();
    }

    private void UpdatePoints()
    {
        if (!_hasFrame)
        {
            return;
        }

        for (int i = 0; i < _points.Length; i++)
        {
            _points[i].Y = LevelToY(_decibels[_firstBin + i]);
        }
    }

    private float FrequencyToX(double frequency)
        => _plot.Left + (float)(Math.Log(frequency / 20) / _logFrequencyRange) * _plot.Width;

    private float LevelToY(float decibels)
    {
        float minimum = _source?.Options.MinimumDecibels ?? -90;
        float level = Math.Clamp(decibels, minimum, 0);
        return _plot.Top + level / minimum * _plot.Height;
    }

    private void ReleasePens()
    {
        _gridPen?.Dispose();
        _spectrumPen?.Dispose();
        _peakPen?.Dispose();
        _gridPen = null;
        _spectrumPen = null;
        _peakPen = null;
    }

    private readonly record struct AxisTick(float Level, string Text);
}
