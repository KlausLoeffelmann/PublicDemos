using System.ComponentModel;
using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.Controls;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Engine;

/// <summary>
///  A hardware-accelerated analog clock that renders every part — face, markers,
///  hands, arbour, drums — as its own DirectComposition visual. The active
///  <see cref="IClockTheme"/> describes and draws the parts and may relocate anchors
///  or tweak parameters, but the engine alone owns time and hand pointing, so the
///  displayed time is always correct.
/// </summary>
public sealed class WarpClockControl : D2DPanel
{
    private readonly Lock _sync = new();
    private readonly ClockTimeModel _timeModel = new();
    private readonly HandPointingSolver _solver = new();
    private readonly MagneticNumeralSolver _magneticSolver = new();
    private readonly OledViewTransformController _oledViewTransform = new();
    private readonly DefaultClockLayout _defaultLayout = new();
    private readonly Dictionary<ClockElementId, ElementRuntime> _runtime = [];

    private IClockTheme? _theme;
    private bool _themeChangePending;
    private IClockLayout? _activeLayout;
    private IClockElementRenderer? _renderer;
    private IThemeAnimator? _animator;
    private IReadOnlyList<ClockElementDescriptor> _descriptors = [];
    private ThemeTickContext? _tickContext;
    private readonly ElementRenderContext _renderContext = new();
    private readonly ThemeInfoOverlay _themeInfoOverlay = new();

    private SizeF _surface = new(2, 2);
    private ClockGeometry _sceneGeometry = ClockGeometry.ForSurface(new SizeF(2, 2));
    private float _faceRotation;
    private int _graceSeconds = 5;
    private float _glideDurationSeconds = 0.5f;
    private ClockHandMotion _secondMotion = ClockHandMotion.Crawling;
    private ClockHandMotion _minuteMotion = ClockHandMotion.Crawling;
    private ClockHandMotion _hourMotion = ClockHandMotion.Crawling;
    private bool _magneticNumerals;
    private OledViewMode _oledView;
    private float _currentOledViewScale = 1f;
    private Point _currentOledViewOffset;
    private RenderThemeInfo _renderThemeInfo = RenderThemeInfo.FadeAlternateScreenSides;
    private ThemeInfoPlacement _themeInfoPlacement = ThemeInfoPlacement.LeftScreenSide;
    private bool _sceneBuilt;
    private double _framesPerSecond;

    /// <summary>Initializes a new <see cref="WarpClockControl"/>.</summary>
    public WarpClockControl()
    {
        DoubleBuffered = false;
        BackColor = Color.Black;
        RenderMode = RenderMode.D2DDedicatedRenderThread;
        PreserveLastFrame = true;
        VSyncEnabled = true;
        TargetFrameRate = 60d;

        RenderBackground += OnRenderBackground;
        Render += OnRenderForeground;
    }

    /// <summary>The most recent smoothed render frame rate (frames per second).</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double CurrentFramesPerSecond => Volatile.Read(ref _framesPerSecond);

    /// <summary>How (and whether) the "{theme} - {author}" info overlay is rendered.</summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RenderThemeInfo RenderThemeInfo
    {
        get
        {
            lock (_sync)
            {
                return _renderThemeInfo;
            }
        }
        set
        {
            lock (_sync)
            {
                _renderThemeInfo = value;
            }
        }
    }

    /// <summary>Where the info overlay sits for the fixed-position render modes.</summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ThemeInfoPlacement ThemeInfoPlacement
    {
        get
        {
            lock (_sync)
            {
                return _themeInfoPlacement;
            }
        }
        set
        {
            lock (_sync)
            {
                _themeInfoPlacement = value;
            }
        }
    }

    /// <summary>
    ///  Applies a scene-wide anti-burn-in view transform independently of the active theme.
    /// </summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public OledViewMode OledView
    {
        get
        {
            lock (_sync)
            {
                return _oledView;
            }
        }
        set
        {
            lock (_sync)
            {
                if (_oledView == value)
                {
                    return;
                }

                _oledView = value;
            }
        }
    }

    /// <summary>The scene-wide scale currently applied by <see cref="OledView"/>.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float CurrentOledViewScale
    {
        get
        {
            lock (_sync)
            {
                return _currentOledViewScale;
            }
        }
    }

    /// <summary>The pixel offset currently applied by <see cref="OledView"/>.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Point CurrentOledViewOffset
    {
        get
        {
            lock (_sync)
            {
                return _currentOledViewOffset;
            }
        }
    }

    // ── Public configuration ──

    /// <summary>The active theme. Set on the UI thread.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IClockTheme? Theme
    {
        get
        {
            lock (_sync)
            {
                return _theme;
            }
        }
        set
        {
            lock (_sync)
            {
                if (ReferenceEquals(_theme, value))
                {
                    return;
                }

                _theme = value;
                _themeChangePending = true;
            }
        }
    }

    /// <summary>The hand-to-target catch-up window in seconds (1..30). Used in free-floating layouts.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int GraceSeconds
    {
        get
        {
            lock (_sync)
            {
                return _graceSeconds;
            }
        }
        set
        {
            lock (_sync)
            {
                _graceSeconds = Math.Clamp(value, 1, 30);
            }
        }
    }

    /// <summary>Second-hand motion in a radial layout. <see cref="ClockHandMotion.Crawling"/> is disabled when the theme is free-floating.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockHandMotion SecondMotion
    {
        get
        {
            lock (_sync)
            {
                return _secondMotion;
            }
        }
        set
        {
            lock (_sync)
            {
                _secondMotion = value;
            }
        }
    }

    /// <summary>Minute-hand motion in a radial layout.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockHandMotion MinuteMotion
    {
        get
        {
            lock (_sync)
            {
                return _minuteMotion;
            }
        }
        set
        {
            lock (_sync)
            {
                _minuteMotion = value;
            }
        }
    }

    /// <summary>Hour-hand motion in a radial layout.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockHandMotion HourMotion
    {
        get
        {
            lock (_sync)
            {
                return _hourMotion;
            }
        }
        set
        {
            lock (_sync)
            {
                _hourMotion = value;
            }
        }
    }

    /// <summary>
    ///  The ease-in-out glide duration (seconds) used by <see cref="ClockHandMotion.Sweep"/>
    ///  and by the magnetic-numeral aiming. A second-hand glide of 0.5s reaches the next
    ///  mark half-way through the second and rests for the remainder. Clamped to 0.1..5s.
    /// </summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float GlideDurationSeconds
    {
        get
        {
            lock (_sync)
            {
                return _glideDurationSeconds;
            }
        }
        set
        {
            lock (_sync)
            {
                _glideDurationSeconds = Math.Clamp(value, 0.1f, 5f);
            }
        }
    }

    /// <summary>A demo time offset added to the real wall clock.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeSpan TimeOffset
    {
        get
        {
            lock (_sync)
            {
                return _timeModel.TimeOffset;
            }
        }
        set
        {
            lock (_sync)
            {
                _timeModel.TimeOffset = value;
            }
        }
    }

    /// <summary>A demo speed multiplier. 1.0 is real time.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double SpeedMultiplier
    {
        get
        {
            lock (_sync)
            {
                return _timeModel.SpeedMultiplier;
            }
        }
        set
        {
            lock (_sync)
            {
                _timeModel.SpeedMultiplier = value;
            }
        }
    }

    /// <summary>
    ///  When <see langword="true"/>, every hand "finds" the hour numerals wherever the
    ///  theme has placed them on the canvas and swings to the next one at its own rate
    ///  (second-by-second, minute-by-minute, hour-by-hour). Numerals marked
    ///  <see cref="ClockNumeralVisibility.Invisible"/> (or absent) are skipped — a hand
    ///  that would land on one stays where it is. Independent of the per-hand
    ///  <see cref="ClockHandMotion"/>; the glide uses <see cref="GlideDurationSeconds"/>.
    /// </summary>
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool MagneticNumerals
    {
        get
        {
            lock (_sync)
            {
                return _magneticNumerals;
            }
        }
        set
        {
            lock (_sync)
            {
                if (_magneticNumerals == value)
                {
                    return;
                }

                _magneticNumerals = value;

                // Drop stale per-hand state so the hands don't jump from an old solver's pose.
                _magneticSolver.Reset();
                _solver.Reset();
            }
        }
    }

    /// <summary>Whether the active theme uses a free-floating (non-radial) layout.</summary>
    [Browsable(false)]
    public bool FreeFloating
    {
        get
        {
            lock (_sync)
            {
                return _theme?.Capabilities.FreeFloating ?? false;
            }
        }
    }

    /// <summary>Resets accumulated fast-forward time (call when returning to 1× speed).</summary>
    public void ResetTimeAcceleration()
    {
        lock (_sync)
        {
            _timeModel.ResetAccumulatedOffset();
        }
    }

    // ── Lifecycle ──

    /// <inheritdoc/>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        CacheSurface();
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        CacheSurface();
    }

    private void CacheSurface()
    {
        lock (_sync)
        {
            _surface = new SizeF(Math.Max(2, ClientSize.Width), Math.Max(2, ClientSize.Height));
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _themeInfoOverlay.Dispose();
        }
    }

    // ── Theme activation / scene build ──

    private void ActivateTheme()
    {
        TeardownScene();
        _solver.Reset();
        _magneticSolver.Reset();
        _faceRotation = 0f;
        _themeChangePending = false;

        if (_theme is null)
        {
            _activeLayout = null;
            _renderer = null;
            _animator = null;
            _descriptors = [];
            _tickContext = null;
            _sceneGeometry = ClockGeometry.ForSurface(_surface);
            _currentOledViewScale = 1f;
            _currentOledViewOffset = Point.Empty;
            return;
        }

        _activeLayout = _theme.CreateLayout();
        _renderer = _theme.CreateRenderer();
        _animator = _theme.CreateAnimator();
        _descriptors = _theme.CreateElements();

        // Enforce the time-correctness invariant: a free-floating layout cannot crawl.
        if (_theme.Capabilities.FreeFloating)
        {
            if (SecondMotion == ClockHandMotion.Crawling) SecondMotion = ClockHandMotion.Sweep;
            if (MinuteMotion == ClockHandMotion.Crawling) MinuteMotion = ClockHandMotion.Sweep;
            if (HourMotion == ClockHandMotion.Crawling) HourMotion = ClockHandMotion.Sweep;
        }

        _tickContext = new ThemeTickContext(_descriptors, GetParametersFor);
    }

    private ClockElementParameters GetParametersFor(ClockElementId id)
        => _runtime.TryGetValue(id, out ElementRuntime? runtime)
            ? runtime.Parameters
            : new ClockElementParameters();

    private void BuildSceneIfReady()
    {
        if (_sceneBuilt || _theme is null)
        {
            return;
        }

        foreach (ClockElementDescriptor descriptor in _descriptors.OrderBy(d => d.ZOrder))
        {
            D2DVisual visual = Visuals.AddNew(new Rectangle(0, 0, 1, 1));
            var runtime = new ElementRuntime
            {
                Descriptor = descriptor,
                Visual = visual,
            };

            visual.PaintContent += (_, e) => RedrawElement(runtime, e.Graphics);
            _runtime[descriptor.Id] = runtime;
        }

        _animator?.Initialize(_tickContext!);
        _sceneBuilt = true;
    }

    private void TeardownScene()
    {
        foreach (ElementRuntime runtime in _runtime.Values)
        {
            if (runtime.Visual is not null)
            {
                Visuals.Remove(runtime.Visual);
            }
        }

        _runtime.Clear();
        _sceneBuilt = false;
    }

    // ── Frame loop ──

    private void RenderFrame(TimeSpan frameDelta)
    {
        if (_themeChangePending)
        {
            ActivateTheme();
        }

        if (_theme is null)
        {
            return;
        }

        BuildSceneIfReady();

        float dt = (float)frameDelta.TotalSeconds;

        // Smooth the instantaneous frame rate with an exponential moving average so the
        // status-bar readout doesn't jitter frame to frame.
        if (dt > 0f)
        {
            double instantaneous = 1.0 / dt;
            double smoothed = _framesPerSecond <= 0.0
                ? instantaneous
                : _framesPerSecond * 0.9 + instantaneous * 0.1;
            Volatile.Write(ref _framesPerSecond, smoothed);
        }

        ClockTimeSnapshot time = _timeModel.CreateSnapshot();
        OledSceneTransform sceneTransform = _oledViewTransform.Advance(frameDelta, _surface, _oledView);
        ClockGeometry geometry = ClockGeometry.ForSurface(_surface, sceneTransform);
        _sceneGeometry = geometry;
        _currentOledViewScale = sceneTransform.Scale;
        _currentOledViewOffset = sceneTransform.Offset;

        RunAnimator(time, dt, geometry.Surface);

        foreach (ElementRuntime runtime in _runtime.Values)
        {
            UpdateElement(runtime, time, geometry, dt);
        }
    }

    private void RunAnimator(ClockTimeSnapshot time, float dt, SizeF surfaceSize)
    {
        if (_animator is null || _tickContext is null)
        {
            return;
        }

        // The animator runs once per rendered frame (not on a coarse 10 Hz cadence), so
        // parameter-driven motion such as a theme's wandering numerals is as smooth as the
        // engine-driven hands, which are also recomputed every frame.
        _tickContext.Time = time;
        _tickContext.FrameDelta = TimeSpan.FromSeconds(dt);
        _tickContext.SurfaceSize = surfaceSize;
        _tickContext.FaceRotationDegrees = _faceRotation;
        _animator.OnTick(_tickContext);
        _faceRotation = _tickContext.FaceRotationDegrees;
    }

    private void UpdateElement(ElementRuntime runtime, ClockTimeSnapshot time, ClockGeometry geometry, float dt)
    {
        if (runtime.Visual is null)
        {
            return;
        }

        ClockElementDescriptor descriptor = runtime.Descriptor;
        ClockElementParameters parameters = runtime.Parameters;
        float scale = geometry.DesignScale;

        SizeF contentSize = new(
            MathF.Max(1f, descriptor.ContentSize.Width * scale),
            MathF.Max(1f, descriptor.ContentSize.Height * scale));
        PointF pivotPixels = new(descriptor.Pivot.X * scale, descriptor.Pivot.Y * scale);

        PointF anchor = ResolveAnchor(descriptor.Id, geometry);

        int width = (int)MathF.Ceiling(contentSize.Width);
        int height = (int)MathF.Ceiling(contentSize.Height);

        var bounds = new Rectangle(
            (int)MathF.Round(anchor.X - pivotPixels.X),
            (int)MathF.Round(anchor.Y - pivotPixels.Y),
            Math.Max(1, width),
            Math.Max(1, height));

        runtime.Visual.Bounds = bounds;

        // Transparent / Invisible numerals are placed (so magnetic aiming can still target
        // a Transparent one) but not drawn; only a Visible+Visible element shows its visual.
        runtime.Visual.Visible = parameters.Visible
            && parameters.Visibility == ClockNumeralVisibility.Visible;

        float selfRotation = ComputeSelfRotation(descriptor, parameters, time, anchor, geometry, dt);

        runtime.Visual.SetTransform(
            selfRotation,
            parameters.Scale,
            parameters.Scale,
            parameters.SkewDegrees,
            pivotPixels.X,
            pivotPixels.Y);

        bool sizeChanged = runtime.ContentPixelSize != new Size(bounds.Width, bounds.Height);
        bool needsRedraw = !runtime.ContentDrawn
            || sizeChanged
            || descriptor.RedrawPerFrame
            || parameters.RedrawRequested;

        if (needsRedraw)
        {
            runtime.ContentPixelSize = new Size(bounds.Width, bounds.Height);
            runtime.PivotPixels = pivotPixels;
            runtime.ContentScale = scale;
            runtime.ContentTime = time;
            runtime.Visual.InvalidateContent();
            parameters.RedrawRequested = false;
        }
    }

    private float ComputeSelfRotation(
        ClockElementDescriptor descriptor,
        ClockElementParameters parameters,
        ClockTimeSnapshot time,
        PointF anchor,
        ClockGeometry geometry,
        float dt)
    {
        if (!descriptor.IsHand)
        {
            // Non-hands rotate rigidly with the face plus any theme-driven spin.
            return _faceRotation + parameters.ExtraRotationDegrees;
        }

        // Magnetic mode overrides ordinary pointing: the hand aims at scattered hour
        // numerals (skipping Invisible ones) and glides/tracks their live positions.
        if (_magneticNumerals)
        {
            float magnetic = _magneticSolver.Solve(
                descriptor.Hand,
                anchor,
                time,
                _glideDurationSeconds,
                dt,
                NumeralVisibilityAt,
                index => ResolveAnchor(ClockElementId.HourMarker(index), geometry));

            float magneticWobble = Math.Clamp(parameters.ExtraRotationDegrees, -5f, 5f);
            return magnetic + magneticWobble;
        }

        bool freeFloating = _theme!.Capabilities.FreeFloating;
        float target;

        if (freeFloating)
        {
            target = HandPointingSolver.FreeFloatingTargetAngle(
                descriptor.Hand, anchor, time, id => ResolveAnchor(id, geometry));
        }
        else
        {
            target = HandPointingSolver.RadialTargetAngle(time, descriptor.Hand, MotionFor(descriptor.Hand), _glideDurationSeconds);
            if (_theme.Capabilities.HandsFollowFaceRotation)
            {
                target += _faceRotation;
            }
        }

        float displayed = _solver.Solve(descriptor.Hand, target, _graceSeconds, freeFloating, dt);

        // Hand wobble is clamped so a theme can never misrepresent the time.
        float wobble = Math.Clamp(parameters.ExtraRotationDegrees, -5f, 5f);
        return displayed + wobble;
    }

    private ClockHandMotion MotionFor(ClockHandKind hand)
        => hand switch
        {
            ClockHandKind.Hour => HourMotion,
            ClockHandKind.Minute => MinuteMotion,
            _ => SecondMotion,
        };

    /// <summary>
    ///  Returns the visibility of hour numeral <paramref name="index"/> (0..11), or
    ///  <see langword="null"/> when the active theme did not materialize that numeral.
    ///  Used by the magnetic solver to decide which numerals are valid targets.
    /// </summary>
    private ClockNumeralVisibility? NumeralVisibilityAt(int index)
        => _runtime.TryGetValue(ClockElementId.HourMarker(index), out ElementRuntime? runtime)
            ? runtime.Parameters.Visibility
            : null;

    private PointF ResolveAnchor(ClockElementId id, ClockGeometry geometry)
    {
        if (_activeLayout is not null && _activeLayout.TryGetAnchor(id, geometry.Surface, out PointF anchor))
        {
            anchor = new PointF(anchor.X + geometry.Origin.X, anchor.Y + geometry.Origin.Y);
        }
        else
        {
            anchor = DefaultClockLayout.ResolveAnchor(id, geometry);
        }

        if (_runtime.TryGetValue(id, out ElementRuntime? runtime))
        {
            PointF offset = runtime.Parameters.AnchorOffset;
            float scale = geometry.DesignScale;
            anchor = new PointF(anchor.X + offset.X * scale, anchor.Y + offset.Y * scale);
        }

        // Face rotation orbits non-hand elements about the dial center.
        if (_faceRotation != 0f && id.Kind is not (ClockElementKind.HourHand
            or ClockElementKind.MinuteHand 
            or ClockElementKind.SecondHand 
            or ClockElementKind.SubSecondHand))
        {
            anchor = ClockMath.RotateAbout(anchor, geometry.Center, _faceRotation);
        }

        return anchor;
    }

    private void RedrawElement(ElementRuntime runtime, ID2DGraphics graphics)
    {
        if (_renderer is null)
        {
            return;
        }

        _renderContext.Id = runtime.Descriptor.Id;
        _renderContext.ContentSize = runtime.ContentPixelSize;
        _renderContext.Pivot = runtime.PivotPixels;
        _renderContext.Parameters = runtime.Parameters;
        _renderContext.Time = runtime.ContentTime;
        _renderContext.Scale = runtime.ContentScale;
        _renderer.DrawElement(graphics, _renderContext);
        runtime.ContentDrawn = true;
    }

    private static void OnRenderBackground(object? sender, D2DRenderEventArgs e)
        => e.Graphics.Clear(Color.Black);

    /// <summary>
    ///  Foreground pass (composited on top of the element visuals): draws the theme-info
    ///  overlay. Because <see cref="PreserveLastFrame"/> keeps the last foreground content
    ///  when a handler draws nothing, we clear the surface to transparent every frame —
    ///  otherwise a faded-out (or disabled) overlay would stay frozen on screen. The clear
    ///  is transparent so the retained hand/numeral visuals continue to show through.
    /// </summary>
    private void OnRenderForeground(object? sender, D2DRenderEventArgs e)
    {
        lock (_sync)
        {
            RenderFrame(e.FrameDelta);
            e.Graphics.Clear(Color.Transparent);

            if (_oledView != OledViewMode.Off
                || _renderThemeInfo == RenderThemeInfo.Never
                || _theme is null)
            {
                return;
            }

            _themeInfoOverlay.Configure(_theme.Name, _theme.Author);
            _themeInfoOverlay.Render(
                e.Graphics,
                Size.Round(_surface),
                _renderThemeInfo,
                _themeInfoPlacement,
                _sceneGeometry.Bounds);
        }
    }
}
