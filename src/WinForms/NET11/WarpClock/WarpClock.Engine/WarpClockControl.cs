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
    private readonly HandRotationSolver _handRotation = new();
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
    private readonly TimeZoneHeadlineOverlay _timeZoneHeadlineOverlay = new();

    private SizeF _surface = new(2, 2);
    private ClockGeometry _sceneGeometry = ClockGeometry.ForSurface(new SizeF(2, 2));
    private TimeZoneInfo _displayedTimeZone = TimeZoneInfo.Local;
    private ClockAmbientSnapshot _ambientContent = ClockAmbientSnapshot.Empty;
    private ClockAuxiliaryVisibility _auxiliaryVisibility = ClockAuxiliaryVisibility.Default;
    private ClockTimeZoneSnapshot _lastAnimatorTimeZone;
    private bool _hasAnimatorTimeZone;
    private float _faceRotation;
    private int _graceSeconds = 5;
    private float _glideDurationSeconds = 0.5f;
    private ClockHandMotion _secondMotion = ClockHandMotion.Crawling;
    private ClockHandMotion _minuteMotion = ClockHandMotion.Crawling;
    private ClockHandMotion _hourMotion = ClockHandMotion.Crawling;
    private ClockHandTargetMode _secondTargetMode = ClockHandTargetMode.ThemeDefault;
    private ClockHandTargetMode _minuteTargetMode = ClockHandTargetMode.ThemeDefault;
    private ClockHandTargetMode _hourTargetMode = ClockHandTargetMode.ThemeDefault;
    private bool _magneticNumerals;
    private OledViewMode _oledView;
    private float _currentOledViewScale = 1f;
    private Point _currentOledViewOffset;
    private RenderThemeInfo _renderThemeInfo = RenderThemeInfo.FadeAlternateScreenSides;
    private ThemeInfoPlacement _themeInfoPlacement = ThemeInfoPlacement.LeftScreenSide;
    private bool _timeZoneHeadlineFallbackEnabled;
    private string _timeZoneHeadlineText = string.Empty;
    private bool _timeZoneHeadlineNightMode;
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

    /// <summary>Whether the engine should draw its fallback fading time-zone headline.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool TimeZoneHeadlineFallbackEnabled
    {
        get
        {
            lock (_sync)
            {
                return _timeZoneHeadlineFallbackEnabled;
            }
        }
        set
        {
            lock (_sync)
            {
                _timeZoneHeadlineFallbackEnabled = value;
            }
        }
    }

    /// <summary>The fallback time-zone headline text drawn by the engine when enabled.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string TimeZoneHeadlineText
    {
        get
        {
            lock (_sync)
            {
                return _timeZoneHeadlineText;
            }
        }
        set
        {
            lock (_sync)
            {
                _timeZoneHeadlineText = value ?? string.Empty;
            }
        }
    }

    /// <summary>Whether the fallback time-zone headline should use its subdued night palette.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool TimeZoneHeadlineNightMode
    {
        get
        {
            lock (_sync)
            {
                return _timeZoneHeadlineNightMode;
            }
        }
        set
        {
            lock (_sync)
            {
                _timeZoneHeadlineNightMode = value;
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

    /// <summary>Second-hand motion used when the hand is targeting the radial dial.</summary>
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

    /// <summary>Global second-hand target override, or the active theme's choice.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockHandTargetMode SecondTargetMode
    {
        get { lock (_sync) { return _secondTargetMode; } }
        set { lock (_sync) { _secondTargetMode = value; _handRotation.Reset(); } }
    }

    /// <summary>Global minute-hand target override, or the active theme's choice.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockHandTargetMode MinuteTargetMode
    {
        get { lock (_sync) { return _minuteTargetMode; } }
        set { lock (_sync) { _minuteTargetMode = value; _handRotation.Reset(); } }
    }

    /// <summary>Global hour-hand target override, or the active theme's choice.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockHandTargetMode HourTargetMode
    {
        get { lock (_sync) { return _hourTargetMode; } }
        set { lock (_sync) { _hourTargetMode = value; _handRotation.Reset(); } }
    }

    /// <summary>
    ///  The ease-in-out glide duration (seconds) used by <see cref="ClockHandMotion.Sweep"/>.
    ///  Clamped to 0.1..5s.
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

    /// <summary>The displayed time zone. The engine converts from UTC each frame so DST remains correct.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TimeZoneInfo DisplayedTimeZone
    {
        get
        {
            lock (_sync)
            {
                return _displayedTimeZone;
            }
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            lock (_sync)
            {
                _displayedTimeZone = value;
                _timeModel.DisplayedTimeZone = value;
            }
        }
    }

    /// <summary>Host-supplied ambient content exposed to renderers and animators.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockAmbientSnapshot AmbientContent
    {
        get
        {
            lock (_sync)
            {
                return CloneAmbientSnapshot(_ambientContent);
            }
        }
        set
        {
            lock (_sync)
            {
                _ambientContent = CloneAmbientSnapshot(value);
            }
        }
    }

    /// <summary>Visibility gates for optional auxiliary visuals.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ClockAuxiliaryVisibility AuxiliaryVisibility
    {
        get
        {
            lock (_sync)
            {
                return _auxiliaryVisibility;
            }
        }
        set
        {
            lock (_sync)
            {
                _auxiliaryVisibility = value;
            }
        }
    }

    /// <summary>
    ///  The global default: when <see langword="true"/>, every hand that did not request
    ///  <see cref="ClockHandTargetMode.Radial"/> uses the current live hour numeral as
    ///  its reference and adds the hand's authoritative clockwise progress through that
    ///  numeral's 30-degree interval.
    /// </summary>
    /// <remarks>
    ///  This is only a default. A hand that explicitly requests
    ///  <see cref="ClockHandTargetMode.MagneticNumerals"/> stays magnetic even while this
    ///  is <see langword="false"/>, so a theme built around magnetic aiming keeps working
    ///  in a host that never switched the mode on.
    /// </remarks>
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
                _handRotation.Reset();
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
            _timeZoneHeadlineOverlay.Dispose();
        }
    }

    // ── Theme activation / scene build ──

    private void ActivateTheme()
    {
        TeardownScene();
        _handRotation.Reset();
        _faceRotation = 0f;
        _themeChangePending = false;
        _hasAnimatorTimeZone = false;

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

        // Surface size must be known before Initialize so themes can place
        // viewport-relative content (for example padded screen-top captions).
        if (_tickContext is not null)
        {
            ClockGeometry geometry = ClockGeometry.ForSurface(_surface, OledSceneTransform.Identity);
            _tickContext.SurfaceSize = geometry.Surface;
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
        ClockTimeZoneSnapshot timeZone = _timeModel.CreateTimeZoneSnapshot(time.Now);
        ClockAmbientSnapshot ambient = CloneAmbientSnapshot(_ambientContent);
        OledSceneTransform sceneTransform = _oledViewTransform.Advance(frameDelta, _surface, _oledView);
        ClockGeometry geometry = ClockGeometry.ForSurface(_surface, sceneTransform);
        _sceneGeometry = geometry;
        _currentOledViewScale = sceneTransform.Scale;
        _currentOledViewOffset = sceneTransform.Offset;

        RunAnimator(time, timeZone, ambient, dt, geometry.Surface);

        foreach (ElementRuntime runtime in _runtime.Values)
        {
            UpdateElement(runtime, time, timeZone, ambient, geometry, dt);
        }
    }

    private void RunAnimator(
        ClockTimeSnapshot time,
        ClockTimeZoneSnapshot timeZone,
        ClockAmbientSnapshot ambient,
        float dt,
        SizeF surfaceSize)
    {
        if (_animator is null || _tickContext is null)
        {
            return;
        }

        // The animator runs once per rendered frame (not on a coarse 10 Hz cadence), so
        // parameter-driven motion such as a theme's wandering numerals is as smooth as the
        // engine-driven hands, which are also recomputed every frame.
        _tickContext.Time = time;
        _tickContext.TimeZone = timeZone;
        _tickContext.Ambient = ambient;
        _tickContext.FrameDelta = TimeSpan.FromSeconds(dt);
        _tickContext.SurfaceSize = surfaceSize;
        _tickContext.FaceRotationDegrees = _faceRotation;

        if (_hasAnimatorTimeZone && !_lastAnimatorTimeZone.Equals(timeZone))
        {
            _animator.OnTimeZoneChanged(_tickContext, _lastAnimatorTimeZone, timeZone);
        }

        _lastAnimatorTimeZone = timeZone;
        _hasAnimatorTimeZone = true;
        _animator.OnTick(_tickContext);
        _faceRotation = _tickContext.FaceRotationDegrees;
    }

    private void UpdateElement(
        ElementRuntime runtime,
        ClockTimeSnapshot time,
        ClockTimeZoneSnapshot timeZone,
        ClockAmbientSnapshot ambient,
        ClockGeometry geometry,
        float dt)
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
            && parameters.Visibility == ClockNumeralVisibility.Visible
            && IsAuxiliaryVisible(descriptor.Id);

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
            runtime.ContentTimeZone = timeZone;
            runtime.ContentAmbient = ambient;
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

        ThemeCapabilities capabilities = _theme!.Capabilities;

        return _handRotation.Solve(new HandRotationRequest
        {
            Hand = descriptor.Hand,
            Pivot = anchor,
            Time = time,
            RequestedTargetMode = TargetModeFor(descriptor.Hand, parameters.HandTargetMode),
            Motion = descriptor.Hand == ClockHandKind.SubSecond
                ? ClockHandMotion.Crawling
                : MotionFor(descriptor.Hand),
            ThemeSupportsFreeFloating = capabilities.FreeFloating,
            HandsFollowFaceRotation = capabilities.HandsFollowFaceRotation,
            MagneticNumeralsEnabled = _magneticNumerals,
            AnchorOf = id => ResolveAnchor(id, geometry),
            NumeralVisibilityOf = NumeralVisibilityAt,
            FaceRotationDegrees = _faceRotation,
            ExtraRotationDegrees = parameters.ExtraRotationDegrees,
            GraceSeconds = _graceSeconds,
            GlideDurationSeconds = _glideDurationSeconds,
            DeltaSeconds = dt,
        });
    }

    private ClockHandMotion MotionFor(ClockHandKind hand)
        => hand switch
        {
            ClockHandKind.Hour => HourMotion,
            ClockHandKind.Minute => MinuteMotion,
            _ => SecondMotion,
        };

    private ClockHandTargetMode TargetModeFor(
        ClockHandKind hand,
        ClockHandTargetMode themeMode)
    {
        ClockHandTargetMode overrideMode = hand switch
        {
            ClockHandKind.Hour => _hourTargetMode,
            ClockHandKind.Minute => _minuteTargetMode,
            ClockHandKind.Second => _secondTargetMode,
            _ => ClockHandTargetMode.ThemeDefault,
        };

        return overrideMode == ClockHandTargetMode.ThemeDefault ? themeMode : overrideMode;
    }

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
        => ClockElementAnchorResolver.Resolve(
            id,
            geometry,
            _activeLayout,
            _runtime.TryGetValue(id, out ElementRuntime? runtime)
                ? runtime.Parameters.AnchorOffset
                : PointF.Empty,
            _faceRotation);

    private bool IsAuxiliaryVisible(ClockElementId id)
        => IsAuxiliaryVisible(id.Kind, _auxiliaryVisibility);

    internal static bool IsAuxiliaryVisible(ClockElementKind kind, ClockAuxiliaryVisibility visibility)
        => kind switch
        {
            ClockElementKind.TimeZone => visibility.ShowTimeZone,
            ClockElementKind.Day => visibility.ShowDay,
            ClockElementKind.Weekday => visibility.ShowWeekday,
            ClockElementKind.FractionSecondDial or ClockElementKind.SubSecondHand => visibility.ShowFractionSecond,
            ClockElementKind.OverlayMessage => visibility.ShowOverlayMessage,
            ClockElementKind.IndexedImage => visibility.ShowIndexedImages,
            _ => true,
        };

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
        _renderContext.TimeZone = runtime.ContentTimeZone;
        _renderContext.Ambient = runtime.ContentAmbient;
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

            if (_theme is not null
                && TimeZoneHeadlineOverlay.ShouldRender(
                    _timeZoneHeadlineFallbackEnabled,
                    _timeZoneHeadlineText,
                    _descriptors))
            {
                _timeZoneHeadlineOverlay.Render(
                    e.Graphics,
                    Size.Round(_surface),
                    _timeZoneHeadlineText,
                    _timeZoneHeadlineNightMode);
            }

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

    private static ClockAmbientSnapshot CloneAmbientSnapshot(ClockAmbientSnapshot snapshot)
        => new()
        {
            OverlayMessage = snapshot.OverlayMessage,
            TickerText = snapshot.TickerText,
            TimeZoneAlias = snapshot.TimeZoneAlias,
            TimeZoneDesignation = snapshot.TimeZoneDesignation,
            PresentationState = snapshot.PresentationState,
            IndexedImages = snapshot.IndexedImages?.ToArray() ?? Array.Empty<ClockIndexedImageSnapshot>(),
        };
}
