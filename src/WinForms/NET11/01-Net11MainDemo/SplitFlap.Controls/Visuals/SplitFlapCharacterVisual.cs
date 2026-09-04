namespace SplitFlap.Visuals;

/// <summary>
///  One mechanical split-flap character. Not a control: a pure drawing primitive with its own
///  state machine and back buffer, so it can be driven by a <see cref="SplitFlapAnimator"/> and
///  hosted by any control, a departure board, an alarm clock, or something we haven't thought of yet.
/// </summary>
/// <remarks>
///  <para>
///   The drum only rotates forward. A transition from 'A' to 'Z' walks every flap in between,
///   which is where the characteristic clatter of a whole board comes from.
///  </para>
///  <para>
///   Threading: property setters are expected on the owner's thread. <see cref="Advance"/> and
///   <see cref="RenderFrame"/> are called by the animator thread; <see cref="Draw"/> is called by
///   the UI thread and only blits the finished front buffer. Events are raised on the animator thread.
///  </para>
/// </remarks>
public sealed class SplitFlapCharacterVisual : IDisposable
{
    /// <summary>The default drum: blank first (that's the reset position), then letters, digits, punctuation.</summary>
    public const string DefaultCharacterSet = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-./:+&";

    /// <summary>White. Independent of dark mode, on purpose: the flap is a piece of plastic, not a theme.</summary>
    public static readonly Color DefaultForeColor = Color.White;

    /// <summary>Almost black (#333333). Also not a theme.</summary>
    public static readonly Color DefaultBackColor = Color.FromArgb(0x33, 0x33, 0x33);

    private const double JamHoldMilliseconds = 250;

    private enum VisualState { Idle, Flipping, JamHold, Recovering }

    [Flags]
    private enum RaisedEvents { None = 0, FlapFell = 1, Jammed = 2, Settled = 4 }

    private readonly Lock _sync = new();
    private readonly Lock _bufferSync = new();

    private string _characterSet = DefaultCharacterSet;
    private int _current;
    private int _target;
    private int _flipTo;
    private double _progress;
    private double _timer;
    private bool _resetting;
    private bool _forceJam;
    private VisualState _state;
    private bool _dirty = true;

    private Font? _font;
    private Font? _ownedDefaultFont;
    private Bitmap? _front;
    private Bitmap? _back;
    private bool _disposed;

    /// <summary>Raised on the animator thread each time a flap has fallen. Hook your clack sound here.</summary>
    public event EventHandler<FlapEventArgs>? FlapFell;

    /// <summary>Raised on the animator thread when the jam detection kicked in.</summary>
    public event EventHandler<FlapEventArgs>? Jammed;

    /// <summary>Raised on the animator thread when the visual reached its target character.</summary>
    public event EventHandler<FlapEventArgs>? Settled;

    /// <summary>
    ///  The ordered set of characters on the drum. Index 0 is the reset position and should be a blank.
    /// </summary>
    public string CharacterSet
    {
        get => _characterSet;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);

            lock (_sync)
            {
                _characterSet = value;
                _current = 0;
                _target = 0;
                _state = VisualState.Idle;
                _resetting = false;
                _dirty = true;
            }
        }
    }

    /// <summary>The font used for the glyph. Not owned by the visual; <see langword="null"/> uses a generic monospace font.</summary>
    public Font? Font
    {
        get => _font;
        set
        {
            _font = value;
            MarkDirty();
        }
    }

    /// <summary>Glyph color.</summary>
    public Color ForeColor
    {
        get;
        set
        {
            field = value;
            MarkDirty();
        }
    } = DefaultForeColor;

    /// <summary>Flap color.</summary>
    public Color BackColor
    {
        get;
        set
        {
            field = value;
            MarkDirty();
        }
    } = DefaultBackColor;

    /// <summary>Space between the flap's edge and the glyph, in device pixels.</summary>
    public Padding Padding
    {
        get;
        set
        {
            field = value;
            MarkDirty();
        }
    } = new(6, 2, 6, 2);

    /// <summary>Size of the flap in device pixels. Set by the hosting control.</summary>
    public Size Size
    {
        get;
        set
        {
            field = value;
            MarkDirty();
        }
    }

    /// <summary>DPI the back buffer is rendered at, so point-sized fonts come out right.</summary>
    public int Dpi
    {
        get;
        set
        {
            field = Math.Max(72, value);
            MarkDirty();
        }
    } = 96;

    /// <summary>Time per single flap fall.</summary>
    public FlipAnimationSpeed FlipAnimationSpeed { get; set; } = FlipAnimationSpeed.Medium;

    /// <summary>
    ///  Chance that a falling flap jams, in hundredths of a percent (0..10). 3 means 0.03 % per flap;
    ///  a full 8x32 board update rolls that dice roughly 4,500 times, so you will see one now and then.
    /// </summary>
    public int FlapJamProbability
    {
        get;
        set => field = Math.Clamp(value, 0, 10);
    } = 3;

    /// <summary>How long the controller waits in the reset position (blank) before it re-seeks, in milliseconds.</summary>
    public int JamRecoveryTime
    {
        get;
        set => field = Math.Max(0, value);
    } = 500;

    /// <summary>The character currently on the front of the flap.</summary>
    public char CurrentCharacter
    {
        get
        {
            lock (_sync)
            {
                return _characterSet[_current];
            }
        }
    }

    /// <summary>
    ///  The character the drum should seek to. Characters not on the drum are tried upper-cased,
    ///  then fall back to the reset position.
    /// </summary>
    public char TargetCharacter
    {
        get
        {
            lock (_sync)
            {
                return _characterSet[_target];
            }
        }
        set
        {
            lock (_sync)
            {
                _target = IndexOf(value);
            }
        }
    }

    /// <summary><see langword="true"/> while the drum shows its target and nothing is moving.</summary>
    public bool IsSettled
    {
        get
        {
            lock (_sync)
            {
                return _state is VisualState.Idle && _current == _target;
            }
        }
    }

    /// <summary><see langword="true"/> while the jam detection has the drum.</summary>
    public bool IsJammed
    {
        get
        {
            lock (_sync)
            {
                return _resetting || _state is VisualState.JamHold;
            }
        }
    }

    /// <summary>
    ///  Snaps current and target to the reset position without animation.
    /// </summary>
    public void Reset()
    {
        lock (_sync)
        {
            _current = 0;
            _target = 0;
            _state = VisualState.Idle;
            _resetting = false;
            _dirty = true;
        }
    }

    /// <summary>
    ///  Forces the jam detection to trigger on the next flap fall. For demos, obviously.
    /// </summary>
    public void ForceJam()
    {
        lock (_sync)
        {
            _forceJam = true;
        }
    }

    /// <summary>
    ///  Requests a re-render on the next animator frame, e.g. after a color change.
    /// </summary>
    public void Invalidate()
        => MarkDirty();

    /// <summary>
    ///  Measures the glyph cell (the largest glyph plus padding) for a font at a given DPI.
    ///  Use this for preferred-size calculations so they match what <see cref="RenderFrame"/> draws.
    /// </summary>
    public static Size MeasureCell(Font font, Padding padding, int dpi)
    {
        ArgumentNullException.ThrowIfNull(font);

        using Bitmap probe = new(1, 1);
        probe.SetResolution(dpi, dpi);

        using Graphics g = Graphics.FromImage(probe);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        float width = 0;

        foreach (string sample in (ReadOnlySpan<string>)["W", "M", "0", "@"])
        {
            width = Math.Max(width, g.MeasureString(sample, font, PointF.Empty, StringFormat.GenericTypographic).Width);
        }

        int glyphWidth = (int)Math.Ceiling(width) + 2;
        int glyphHeight = (int)Math.Ceiling(font.GetHeight(g));

        return new Size(glyphWidth + padding.Horizontal, glyphHeight + padding.Vertical);
    }

    /// <summary>
    ///  Advances the state machine. Animator thread only.
    /// </summary>
    /// <param name="elapsed">Time since the previous frame.</param>
    /// <returns><see langword="true"/> if the visual needs re-rendering.</returns>
    public bool Advance(TimeSpan elapsed)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        RaisedEvents raised = RaisedEvents.None;
        char character;
        bool changed;

        lock (_sync)
        {
            double ms = elapsed.TotalMilliseconds;
            changed = _dirty;

            switch (_state)
            {
                case VisualState.Idle when _current != Goal:
                    BeginFlip();
                    changed = true;
                    break;

                case VisualState.Flipping:
                    _progress += ms / FlipAnimationSpeed.ToMillisecondsPerFlap();
                    changed = true;

                    if (_progress >= 1)
                    {
                        raised = CompleteFlip();
                    }

                    break;

                case VisualState.JamHold:
                    _timer -= ms;

                    if (_timer <= 0)
                    {
                        _resetting = true;
                        BeginFlip();
                        changed = true;
                    }

                    break;

                case VisualState.Recovering:
                    _timer -= ms;

                    if (_timer <= 0)
                    {
                        _resetting = false;
                        _state = VisualState.Idle;
                        changed = true;
                    }

                    break;
            }

            character = _characterSet[_current];
        }

        RaiseEvents(raised, character);

        return changed;
    }

    /// <summary>
    ///  Renders the current state into the back buffer and swaps it to the front. Animator thread only.
    /// </summary>
    public void RenderFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        RenderSnapshot snapshot;

        lock (_sync)
        {
            snapshot = new RenderSnapshot(
                Size, Dpi, Padding, ForeColor, BackColor, _font ?? GetDefaultFont(),
                _characterSet[_current],
                _state is VisualState.Flipping ? _characterSet[_flipTo] : _characterSet[_current],
                _state is VisualState.Flipping,
                Math.Clamp(_progress, 0, 1));

            _dirty = false;
        }

        if (snapshot.Size.Width <= 0 || snapshot.Size.Height <= 0)
        {
            return;
        }

        Bitmap back = EnsureBuffer(ref _back, snapshot.Size, snapshot.Dpi);

        using (Graphics g = Graphics.FromImage(back))
        {
            Paint(g, snapshot);
        }

        lock (_bufferSync)
        {
            (_front, _back) = (_back, _front);
        }
    }

    /// <summary>
    ///  Blits the front buffer. UI thread. Renders synchronously once if the animator has not caught up yet.
    /// </summary>
    public void Draw(Graphics g, Point location)
    {
        ArgumentNullException.ThrowIfNull(g);

        if (_disposed)
        {
            return;
        }

        if (_front is null)
        {
            RenderFrame();
        }

        lock (_bufferSync)
        {
            if (_front is not null)
            {
                g.DrawImageUnscaled(_front, location);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        lock (_bufferSync)
        {
            _front?.Dispose();
            _back?.Dispose();
            _front = null;
            _back = null;
        }

        _ownedDefaultFont?.Dispose();
        _ownedDefaultFont = null;
    }

    private int Goal
        => _resetting ? 0 : _target;

    private void MarkDirty()
    {
        lock (_sync)
        {
            _dirty = true;
        }
    }

    private int IndexOf(char c)
    {
        int index = _characterSet.IndexOf(c);

        if (index < 0)
        {
            index = _characterSet.IndexOf(char.ToUpperInvariant(c));
        }

        return Math.Max(0, index);
    }

    private void BeginFlip()
    {
        _flipTo = (_current + 1) % _characterSet.Length;
        _progress = 0;
        _state = VisualState.Flipping;
    }

    private RaisedEvents CompleteFlip()
    {
        _current = _flipTo;
        RaisedEvents raised = RaisedEvents.FlapFell;

        if (!_resetting && _current != Goal && RollForJam())
        {
            _state = VisualState.JamHold;
            _timer = JamHoldMilliseconds;

            return raised | RaisedEvents.Jammed;
        }

        if (_current == Goal)
        {
            if (_resetting)
            {
                _state = VisualState.Recovering;
                _timer = JamRecoveryTime;
            }
            else
            {
                _state = VisualState.Idle;
                raised |= RaisedEvents.Settled;
            }

            return raised;
        }

        BeginFlip();

        return raised;
    }

    private bool RollForJam()
    {
        if (_forceJam)
        {
            _forceJam = false;

            return true;
        }

        return FlapJamProbability > 0
            && Random.Shared.NextDouble() < FlapJamProbability / 10_000.0;
    }

    private void RaiseEvents(RaisedEvents raised, char character)
    {
        if (raised is RaisedEvents.None)
        {
            return;
        }

        FlapEventArgs args = new(this, character);

        if (raised.HasFlag(RaisedEvents.FlapFell))
        {
            FlapFell?.Invoke(this, args);
        }

        if (raised.HasFlag(RaisedEvents.Jammed))
        {
            Jammed?.Invoke(this, args);
        }

        if (raised.HasFlag(RaisedEvents.Settled))
        {
            Settled?.Invoke(this, args);
        }
    }

    private Font GetDefaultFont()
        => _ownedDefaultFont ??= new Font(FontFamily.GenericMonospace, 24f, FontStyle.Regular, GraphicsUnit.Point);

    private static Bitmap EnsureBuffer(ref Bitmap? buffer, Size size, int dpi)
    {
        if (buffer is null || buffer.Size != size || (int)buffer.HorizontalResolution != dpi)
        {
            buffer?.Dispose();
            buffer = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            buffer.SetResolution(dpi, dpi);
        }

        return buffer;
    }

    private static void Paint(Graphics g, RenderSnapshot s)
    {
        g.Clear(Color.Transparent);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        Rectangle face = new(Point.Empty, s.Size);
        Rectangle glyphArea = new(
            s.Padding.Left,
            s.Padding.Top,
            face.Width - s.Padding.Horizontal,
            face.Height - s.Padding.Vertical);

        int mid = face.Y + face.Height / 2;
        Rectangle top = new(face.X, face.Y, face.Width, mid - face.Y);
        Rectangle bottom = new(face.X, mid, face.Width, face.Bottom - mid);
        float dpiScale = s.Dpi / 96f;
        int radius = Math.Max(2, (int)(3 * dpiScale));

        using GraphicsPath facePath = RoundedRectangle(face, radius);
        using SolidBrush faceBrush = new(s.BackColor);
        using SolidBrush glyphBrush = new(s.ForeColor);
        using StringFormat format = new(StringFormat.GenericTypographic)
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip
        };

        // Static halves: the top always shows where we're going, the bottom where we are.
        DrawHalf(g, top, s.TopCharacter);
        DrawHalf(g, bottom, s.BottomCharacter);

        if (s.IsFlipping)
        {
            // One flap rotates 180° around the hinge. cos() gives the foreshortening for free.
            double angle = Math.PI * s.Progress;
            float scale = (float)Math.Abs(Math.Cos(angle));

            if (s.Progress < 0.5)
            {
                DrawMovingFlap(top, s.BottomCharacter, scale, isFalling: true);
            }
            else
            {
                DrawMovingFlap(bottom, s.TopCharacter, scale, isFalling: false);
            }
        }

        // Hinge line plus a whisper of highlight below it.
        using Pen hinge = new(Color.FromArgb(160, 0, 0, 0), Math.Max(1f, dpiScale));
        using Pen highlight = new(Color.FromArgb(28, 255, 255, 255), 1f);
        g.DrawLine(hinge, face.X, mid, face.Right, mid);
        g.DrawLine(highlight, face.X, mid + hinge.Width, face.Right, mid + hinge.Width);

        void DrawHalf(Graphics gr, Rectangle half, char c)
        {
            gr.SetClip(half);
            gr.FillPath(faceBrush, facePath);
            gr.DrawString(c.ToString(), s.Font, glyphBrush, glyphArea, format);
            gr.ResetClip();
        }

        void DrawMovingFlap(Rectangle half, char c, float scale, bool isFalling)
        {
            if (scale < 0.02f)
            {
                return;
            }

            GraphicsState state = g.Save();

            g.TranslateTransform(0, mid);
            g.ScaleTransform(1f, scale);
            g.TranslateTransform(0, -mid);
            g.SetClip(half);

            g.FillPath(faceBrush, facePath);
            g.DrawString(c.ToString(), s.Font, glyphBrush, glyphArea, format);

            // Fake lighting: the flap darkens as it turns away from the viewer.
            int shade = (int)(150 * (1 - scale));
            using SolidBrush shadeBrush = new(Color.FromArgb(shade, isFalling ? Color.Black : Color.White));
            g.FillRectangle(shadeBrush, half);

            g.Restore(state);
        }
    }

    private static GraphicsPath RoundedRectangle(Rectangle r, int radius)
    {
        int d = radius * 2;
        GraphicsPath path = new();

        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();

        return path;
    }

    private readonly record struct RenderSnapshot(
        Size Size,
        int Dpi,
        Padding Padding,
        Color ForeColor,
        Color BackColor,
        Font Font,
        char BottomCharacter,
        char TopCharacter,
        bool IsFlipping,
        double Progress);
}
