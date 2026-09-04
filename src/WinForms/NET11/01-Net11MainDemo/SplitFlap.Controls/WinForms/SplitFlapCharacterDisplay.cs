namespace SplitFlap.WinForms;

/// <summary>
///  A split-flap departure board: <see cref="Rows"/> x <see cref="Columns"/> mechanical characters,
///  driven by a shared <see cref="SplitFlapAnimator"/>. The UI thread never animates or renders a
///  flap; it only blits finished back buffers.
/// </summary>
/// <remarks>
///  <para>
///   With <see cref="AutoSize"/> on (the default) the control dictates its size from font, padding
///   and margins, and reports it honestly through <see cref="GetPreferredSize"/>, so an AutoSize
///   cell in a <see cref="TableLayoutPanel"/> gets a non-negotiable size, like a single-line TextBox.
///  </para>
///  <para>
///   With <see cref="AutoSize"/> off the board zooms its font to fill the client area, clamped by
///   <see cref="MinZoom"/> and <see cref="MaxZoom"/>. <see cref="KeepAspectRatio"/> decides whether
///   surplus space is letterboxed or the flaps stretch to fill it.
///  </para>
///  <para>
///   <see cref="CharacterPadding"/> and <see cref="CharacterMargin"/> are logical (96 dpi) values
///   and are scaled for the monitor the control is on.
///  </para>
/// </remarks>
[ToolboxItem(true)]
[DefaultProperty(nameof(Text))]
[DefaultEvent(nameof(TextChanged))]
[Description("A retro split-flap display board.")]
public class SplitFlapCharacterDisplay : Control, ISupportInitialize
{
    private const string DefaultFontName = MonospaceFonts.FallbackFamilyName;
    private const float DefaultFontSize = 20f;
    private const double DefaultLineHeight = 1.05;
    private const double DefaultMinZoom = 0.25;
    private const double DefaultMaxZoom = 4.0;
    private static readonly Color s_defaultBoardColor = Color.FromArgb(0x1E, 0x1E, 0x1E);

    private readonly List<(Font Font, long RetiredAt)> _retiredFonts = [];
    private SplitFlapCharacterVisual[] _visuals = [];
    private Rectangle[] _cells = [];
    private SplitFlapAnimator? _animator;
    private Font? _baseFont;
    private Font? _effectiveFont;
    private Size _glyphCellAtZoomOne;
    private bool _initializing;
    private bool _registered;
    private int _invalidatePending;

    /// <summary>
    ///  Initializes a new board with one row of twenty characters.
    /// </summary>
    public SplitFlapCharacterDisplay()
    {
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
            true);

        SetStyle(ControlStyles.Selectable, false);

        base.BackColor = s_defaultBoardColor;
        base.AutoSize = true;

        RebuildVisuals();
    }

    /// <summary>The animator driving this board. Defaults to <see cref="SplitFlapAnimator.Default"/>.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SplitFlapAnimator Animator
    {
        get => _animator ?? SplitFlapAnimator.Default;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (ReferenceEquals(Animator, value))
            {
                return;
            }

            UnregisterVisuals();
            _animator = value;
            RegisterVisuals();
        }
    }

    /// <summary>The visuals, row-major. Exposed for sound hookups and for hosting them elsewhere.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<SplitFlapCharacterVisual> Visuals
        => _visuals;

    /// <summary>Factor applied to the glyph height to get the flap height. 1.0 is a tight fit.</summary>
    [Category("Appearance")]
    [Description("Factor applied to the glyph height to get the flap height. 1.0 is a tight fit.")]
    [DefaultValue(DefaultLineHeight)]
    public double LineHeight
    {
        get;
        set
        {
            value = Math.Clamp(value, 0.5, 4.0);

            if (field == value)
            {
                return;
            }

            field = value;
            OnMetricsChanged();
        }
    } = DefaultLineHeight;

    /// <summary>Name of a fixed-pitch font family. Falls back to Consolas if not installed.</summary>
    [Category("Appearance")]
    [Description("Name of a fixed-pitch font family.")]
    [DefaultValue(DefaultFontName)]
    [TypeConverter(typeof(MonospaceFontNameConverter))]
    public string FontName
    {
        get;
        set
        {
            value = string.IsNullOrWhiteSpace(value) ? DefaultFontName : value;

            if (field == value)
            {
                return;
            }

            field = value;
            OnFontMetricsChanged();
        }
    } = DefaultFontName;

    /// <summary>Font size in points. Scales with the monitor DPI like any point-sized font.</summary>
    [Category("Appearance")]
    [Description("Font size in points.")]
    [DefaultValue(DefaultFontSize)]
    public float FontSize
    {
        get;
        set
        {
            value = Math.Clamp(value, 4f, 400f);

            if (field == value)
            {
                return;
            }

            field = value;
            OnFontMetricsChanged();
        }
    } = DefaultFontSize;

    /// <summary>Space between a flap's edge and its glyph, in logical pixels.</summary>
    [Category("Layout")]
    [Description("Space between a flap's edge and its glyph, in logical pixels.")]
    public Padding CharacterPadding
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnMetricsChanged();
        }
    } = new(6, 2, 6, 2);

    /// <summary>Space around each flap, in logical pixels.</summary>
    [Category("Layout")]
    [Description("Space around each flap, in logical pixels.")]
    public Padding CharacterMargin
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnMetricsChanged();
        }
    } = new(2);

    /// <summary>Number of text rows.</summary>
    [Category("Layout")]
    [Description("Number of text rows.")]
    [DefaultValue(1)]
    public int Rows
    {
        get;
        set
        {
            value = Math.Clamp(value, 1, 64);

            if (field == value)
            {
                return;
            }

            field = value;
            OnGridChanged();
        }
    } = 1;

    /// <summary>Number of characters per row.</summary>
    [Category("Layout")]
    [Description("Number of characters per row.")]
    [DefaultValue(20)]
    public int Columns
    {
        get;
        set
        {
            value = Math.Clamp(value, 1, 256);

            if (field == value)
            {
                return;
            }

            field = value;
            OnGridChanged();
        }
    } = 20;

    /// <summary>Smallest font zoom when <see cref="AutoSize"/> is off.</summary>
    [Category("Layout")]
    [Description("Smallest font zoom factor when AutoSize is off.")]
    [DefaultValue(DefaultMinZoom)]
    public double MinZoom
    {
        get;
        set
        {
            value = Math.Clamp(value, 0.05, 10);

            if (field == value)
            {
                return;
            }

            field = value;
            OnMetricsChanged();
        }
    } = DefaultMinZoom;

    /// <summary>Largest font zoom when <see cref="AutoSize"/> is off.</summary>
    [Category("Layout")]
    [Description("Largest font zoom factor when AutoSize is off.")]
    [DefaultValue(DefaultMaxZoom)]
    public double MaxZoom
    {
        get;
        set
        {
            value = Math.Clamp(value, 0.05, 10);

            if (field == value)
            {
                return;
            }

            field = value;
            OnMetricsChanged();
        }
    } = DefaultMaxZoom;

    /// <summary>
    ///  When <see cref="AutoSize"/> is off: <see langword="true"/> letterboxes the board, <see langword="false"/>
    ///  stretches the flaps to fill the client area.
    /// </summary>
    [Category("Layout")]
    [Description("When AutoSize is off: keep the board's proportions or stretch the flaps to fill.")]
    [DefaultValue(true)]
    public bool KeepAspectRatio
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnMetricsChanged();
        }
    } = true;

    /// <summary>Time per single flap fall.</summary>
    [Category("Behavior")]
    [Description("Time per single flap fall.")]
    [DefaultValue(FlipAnimationSpeed.Medium)]
    public FlipAnimationSpeed FlipAnimationSpeed
    {
        get;
        set
        {
            field = value;
            ApplyToVisuals(v => v.FlipAnimationSpeed = value);
        }
    } = FlipAnimationSpeed.Medium;

    /// <summary>Chance that a falling flap jams, in hundredths of a percent (0..10).</summary>
    [Category("Behavior")]
    [Description("Chance that a falling flap jams, in hundredths of a percent (0..10).")]
    [DefaultValue(3)]
    public int FlapJamProbability
    {
        get;
        set
        {
            value = Math.Clamp(value, 0, 10);
            field = value;
            ApplyToVisuals(v => v.FlapJamProbability = value);
        }
    } = 3;

    /// <summary>Time in milliseconds a jammed character rests in the blank position before re-seeking.</summary>
    [Category("Behavior")]
    [Description("Time in milliseconds a jammed character rests in the blank position before re-seeking.")]
    [DefaultValue(500)]
    public int JamRecoveryTime
    {
        get;
        set
        {
            value = Math.Max(0, value);
            field = value;
            ApplyToVisuals(v => v.JamRecoveryTime = value);
        }
    } = 500;

    /// <summary>Ordered characters on the drum. Index 0 is the blank reset position.</summary>
    [Category("Behavior")]
    [Description("Ordered characters on the drum. Index 0 is the blank reset position.")]
    [DefaultValue(SplitFlapCharacterVisual.DefaultCharacterSet)]
    public string CharacterSet
    {
        get;
        set
        {
            value = string.IsNullOrEmpty(value) ? SplitFlapCharacterVisual.DefaultCharacterSet : value;

            if (field == value)
            {
                return;
            }

            field = value;
            ApplyToVisuals(v => v.CharacterSet = value);
            ApplyText();
        }
    } = SplitFlapCharacterVisual.DefaultCharacterSet;

    /// <summary>Glyph color. White by default, regardless of dark mode.</summary>
    [Category("Appearance")]
    [Description("Glyph color. Deliberately independent of dark mode.")]
    public Color FlapForeColor
    {
        get;
        set
        {
            field = value;
            ApplyToVisuals(v => v.ForeColor = value);
            RequestInvalidate();
        }
    } = SplitFlapCharacterVisual.DefaultForeColor;

    /// <summary>Flap color. Almost black by default, regardless of dark mode.</summary>
    [Category("Appearance")]
    [Description("Flap color. Deliberately independent of dark mode.")]
    public Color FlapBackColor
    {
        get;
        set
        {
            field = value;
            ApplyToVisuals(v => v.BackColor = value);
            RequestInvalidate();
        }
    } = SplitFlapCharacterVisual.DefaultBackColor;

    /// <summary>Board background: the housing around the flaps.</summary>
    [DefaultValue(typeof(Color), "30, 30, 30")]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    /// <summary>
    ///  The text shown on the board. Rows are separated by line breaks; missing characters are blanks.
    /// </summary>
    [Category("Appearance")]
    [Description("The text shown on the board. Rows are separated by line breaks.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    /// <summary>Whether the board dictates its own size from font and metrics. On by default.</summary>
    [Category("Layout")]
    [Description("Whether the board dictates its own size from font and metrics.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DefaultValue(true)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    /// <summary>Not used; see <see cref="FontName"/> and <see cref="FontSize"/>.</summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    /// <summary>Not used; see <see cref="FlapForeColor"/>.</summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    /// <inheritdoc/>
    protected override Padding DefaultPadding
        => new(8);

    /// <summary>
    ///  Snaps every character to blank without animation.
    /// </summary>
    public void Clear()
    {
        foreach (SplitFlapCharacterVisual visual in _visuals)
        {
            visual.Reset();
        }

        base.Text = string.Empty;
        RequestInvalidate();
    }

    /// <summary>
    ///  Makes the next falling flap on every character jam. Demo material.
    /// </summary>
    public void ForceJam()
    {
        foreach (SplitFlapCharacterVisual visual in _visuals)
        {
            visual.ForceJam();
        }
    }

    /// <summary>
    ///  Makes the next falling flap of one character jam.
    /// </summary>
    public void ForceJam(int row, int column)
        => GetVisual(row, column).ForceJam();

    /// <summary>
    ///  Returns the visual at a grid position.
    /// </summary>
    public SplitFlapCharacterVisual GetVisual(int row, int column)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(row, Rows);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(column, Columns);

        return _visuals[row * Columns + column];
    }

    /// <summary>
    ///  Returns a task that completes once every character shows its target.
    /// </summary>
    public async Task WaitForSettledAsync(CancellationToken cancellationToken = default)
    {
        while (!_visuals.All(v => v.IsSettled))
        {
            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public void BeginInit()
        => _initializing = true;

    /// <inheritdoc/>
    public void EndInit()
    {
        _initializing = false;
        RetireFont(ref _baseFont);
        _glyphCellAtZoomOne = Size.Empty;
        RebuildVisuals();
    }

    /// <inheritdoc/>
    public override Size GetPreferredSize(Size proposedSize)
    {
        EnsureBaseFont();

        Padding cp = ScaleLogical(CharacterPadding);
        Padding cm = ScaleLogical(CharacterMargin);
        Size cell = CellSizeFor(_glyphCellAtZoomOne, cp);

        return new Size(
            Padding.Horizontal + Columns * (cell.Width + cm.Horizontal),
            Padding.Vertical + Rows * (cell.Height + cm.Vertical));
    }

    /// <inheritdoc/>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (AutoSize && !_initializing)
        {
            Size preferred = GetPreferredSize(Size.Empty);
            width = preferred.Width;
            height = preferred.Height;
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <inheritdoc/>
    protected override void OnAutoSizeChanged(EventArgs e)
    {
        base.OnAutoSizeChanged(e);
        OnMetricsChanged();
    }

    /// <inheritdoc/>
    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        OnMetricsChanged();
    }

    /// <inheritdoc/>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (!AutoSize)
        {
            UpdateLayoutMetrics();
        }
    }

    /// <inheritdoc/>
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        ApplyText();
    }

    /// <inheritdoc/>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        OnFontMetricsChanged();
    }

    /// <inheritdoc/>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Volatile.Write(ref _invalidatePending, 0);
        UpdateLayoutMetrics();
        RegisterVisuals();
    }

    /// <inheritdoc/>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        UnregisterVisuals();
        base.OnHandleDestroyed(e);
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(BackColor);

        for (int i = 0; i < _visuals.Length; i++)
        {
            Rectangle cell = _cells[i];

            if (cell.IntersectsWith(e.ClipRectangle))
            {
                _visuals[i].Draw(g, cell.Location);
            }
        }

        base.OnPaint(e);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnregisterVisuals();

            foreach (SplitFlapCharacterVisual visual in _visuals)
            {
                visual.Dispose();
            }

            _visuals = [];
            _effectiveFont?.Dispose();
            _baseFont?.Dispose();
            DisposeRetiredFonts(all: true);
        }

        base.Dispose(disposing);
    }

    private void OnGridChanged()
    {
        if (_initializing)
        {
            return;
        }

        RebuildVisuals();
    }

    private void OnFontMetricsChanged()
    {
        if (_initializing)
        {
            return;
        }

        RetireFont(ref _baseFont);
        _glyphCellAtZoomOne = Size.Empty;
        OnMetricsChanged();
    }

    private void OnMetricsChanged()
    {
        if (_initializing)
        {
            return;
        }

        if (AutoSize)
        {
            // Setting Size funnels through SetBoundsCore, which enforces the preferred size,
            // and tells the parent's layout engine that we changed.
            Size = GetPreferredSize(Size.Empty);
        }

        UpdateLayoutMetrics();
    }

    private void RebuildVisuals()
    {
        UnregisterVisuals();

        foreach (SplitFlapCharacterVisual old in _visuals)
        {
            old.Dispose();
        }

        int count = Rows * Columns;
        SplitFlapCharacterVisual[] visuals = new SplitFlapCharacterVisual[count];

        for (int i = 0; i < count; i++)
        {
            visuals[i] = new SplitFlapCharacterVisual
            {
                CharacterSet = CharacterSet,
                ForeColor = FlapForeColor,
                BackColor = FlapBackColor,
                FlipAnimationSpeed = FlipAnimationSpeed,
                FlapJamProbability = FlapJamProbability,
                JamRecoveryTime = JamRecoveryTime
            };
        }

        _visuals = visuals;
        _cells = new Rectangle[count];

        OnMetricsChanged();
        ApplyText();
        RegisterVisuals();
    }

    private void EnsureBaseFont()
    {
        if (_baseFont is not null && !_glyphCellAtZoomOne.IsEmpty)
        {
            return;
        }

        _baseFont ??= new Font(MonospaceFonts.ResolveFamilyName(FontName), FontSize, FontStyle.Regular, GraphicsUnit.Point);
        _glyphCellAtZoomOne = SplitFlapCharacterVisual.MeasureCell(_baseFont, Padding.Empty, DeviceDpi);
    }

    private Size CellSizeFor(Size glyph, Padding cp)
        => new(
            glyph.Width + cp.Horizontal,
            (int)Math.Round(glyph.Height * LineHeight) + cp.Vertical);

    private void UpdateLayoutMetrics()
    {
        if (_initializing || _visuals.Length == 0)
        {
            return;
        }

        EnsureBaseFont();

        Padding cp = ScaleLogical(CharacterPadding);
        Padding cm = ScaleLogical(CharacterMargin);
        Rectangle client = ClientRectangle;
        client = new Rectangle(
            client.X + Padding.Left,
            client.Y + Padding.Top,
            Math.Max(0, client.Width - Padding.Horizontal),
            Math.Max(0, client.Height - Padding.Vertical));

        double zoom = 1;
        Font font = _baseFont!;
        Size glyph = _glyphCellAtZoomOne;

        if (!AutoSize && !_glyphCellAtZoomOne.IsEmpty)
        {
            int availableWidth = client.Width - Columns * (cm.Horizontal + cp.Horizontal);
            int availableHeight = client.Height - Rows * (cm.Vertical + cp.Vertical);
            double zoomX = availableWidth / (double)(Columns * _glyphCellAtZoomOne.Width);
            double zoomY = availableHeight / (Rows * _glyphCellAtZoomOne.Height * LineHeight);

            zoom = Math.Clamp(Math.Min(zoomX, zoomY), MinZoom, MaxZoom);

            if (Math.Abs(zoom - 1) > 0.001)
            {
                RetireFont(ref _effectiveFont);
                _effectiveFont = new Font(_baseFont!.FontFamily, (float)(FontSize * zoom), FontStyle.Regular, GraphicsUnit.Point);
                font = _effectiveFont;
                glyph = SplitFlapCharacterVisual.MeasureCell(font, Padding.Empty, DeviceDpi);
            }
        }

        Size cell = CellSizeFor(glyph, cp);

        if (!AutoSize && !KeepAspectRatio)
        {
            cell = new Size(
                Math.Max(cell.Width, client.Width / Columns - cm.Horizontal),
                Math.Max(cell.Height, client.Height / Rows - cm.Vertical));
        }

        Size pitch = new(cell.Width + cm.Horizontal, cell.Height + cm.Vertical);
        Size grid = new(Columns * pitch.Width, Rows * pitch.Height);
        Point origin = new(
            client.X + Math.Max(0, (client.Width - grid.Width) / 2),
            client.Y + Math.Max(0, (client.Height - grid.Height) / 2));

        int dpi = DeviceDpi;

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                int index = row * Columns + column;
                SplitFlapCharacterVisual visual = _visuals[index];

                _cells[index] = new Rectangle(
                    origin.X + column * pitch.Width + cm.Left,
                    origin.Y + row * pitch.Height + cm.Top,
                    cell.Width,
                    cell.Height);

                visual.Size = cell;
                visual.Padding = cp;
                visual.Dpi = dpi;
                visual.Font = font;
            }
        }

        DisposeRetiredFonts(all: false);
        RequestInvalidate();
    }

    private void ApplyText()
    {
        if (_initializing || _visuals.Length == 0)
        {
            return;
        }

        string[] lines = (Text ?? string.Empty).Split(["\r\n", "\n"], StringSplitOptions.None);

        for (int row = 0; row < Rows; row++)
        {
            string line = row < lines.Length ? lines[row] : string.Empty;

            for (int column = 0; column < Columns; column++)
            {
                _visuals[row * Columns + column].TargetCharacter = column < line.Length ? line[column] : ' ';
            }
        }
    }

    private void ApplyToVisuals(Action<SplitFlapCharacterVisual> action)
    {
        foreach (SplitFlapCharacterVisual visual in _visuals)
        {
            action(visual);
        }
    }

    private void RegisterVisuals()
    {
        if (_registered || !IsHandleCreated || _visuals.Length == 0)
        {
            return;
        }

        _registered = true;
        Animator.FrameRendered += OnFrameRendered;
        _ = Animator.RegisterAsync(_visuals);
    }

    private void UnregisterVisuals()
    {
        if (!_registered)
        {
            return;
        }

        _registered = false;
        Animator.FrameRendered -= OnFrameRendered;
        Animator.Unregister(_visuals);
    }

    private void OnFrameRendered(object? sender, EventArgs e)
        => RequestInvalidate();

    /// <summary>
    ///  Coalesces invalidation requests from any thread into a single posted Invalidate.
    /// </summary>
    private void RequestInvalidate()
    {
        if (!IsHandleCreated || Interlocked.Exchange(ref _invalidatePending, 1) == 1)
        {
            return;
        }

        try
        {
            _ = InvokeAsync(() =>
            {
                Volatile.Write(ref _invalidatePending, 0);
                Invalidate();
            });
        }
        catch (InvalidOperationException)
        {
            // Handle went away between the check and the post. Nothing to paint anymore.
            Volatile.Write(ref _invalidatePending, 0);
        }
    }

    private Padding ScaleLogical(Padding logical)
    {
        double factor = DeviceDpi / 96.0;

        return new Padding(
            (int)Math.Round(logical.Left * factor),
            (int)Math.Round(logical.Top * factor),
            (int)Math.Round(logical.Right * factor),
            (int)Math.Round(logical.Bottom * factor));
    }

    /// <summary>
    ///  Fonts can't be disposed the moment they're replaced: the animator thread may still be
    ///  drawing with them. They're parked and disposed once they're a second old.
    /// </summary>
    private void RetireFont(ref Font? font)
    {
        if (font is not null)
        {
            _retiredFonts.Add((font, Environment.TickCount64));
            font = null;
        }
    }

    private void DisposeRetiredFonts(bool all)
    {
        long cutoff = Environment.TickCount64 - 1000;

        for (int i = _retiredFonts.Count - 1; i >= 0; i--)
        {
            if (all || _retiredFonts[i].RetiredAt < cutoff)
            {
                _retiredFonts[i].Font.Dispose();
                _retiredFonts.RemoveAt(i);
            }
        }
    }

    private bool ShouldSerializeCharacterPadding()
        => CharacterPadding != new Padding(6, 2, 6, 2);

    private void ResetCharacterPadding()
        => CharacterPadding = new Padding(6, 2, 6, 2);

    private bool ShouldSerializeCharacterMargin()
        => CharacterMargin != new Padding(2);

    private void ResetCharacterMargin()
        => CharacterMargin = new Padding(2);

    private bool ShouldSerializeFlapForeColor()
        => FlapForeColor != SplitFlapCharacterVisual.DefaultForeColor;

    private void ResetFlapForeColor()
        => FlapForeColor = SplitFlapCharacterVisual.DefaultForeColor;

    private bool ShouldSerializeFlapBackColor()
        => FlapBackColor != SplitFlapCharacterVisual.DefaultBackColor;

    private void ResetFlapBackColor()
        => FlapBackColor = SplitFlapCharacterVisual.DefaultBackColor;
}
