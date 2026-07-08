namespace MaterialKeys;

/// <summary>
///  A button that looks and feels like the concave key of a mechanical table calculator or
///  cash register — raised rim, shallow bowl, and a physical press animation.
/// </summary>
/// <remarks>
///  <para>
///   The control owns input handling, state tracking, invalidation, layout and animation
///   timing. All drawing is delegated to <see cref="MaterialKeyButtonRenderer"/>, driven by a
///   <see cref="MaterialKeyButtonRenderContext"/> the control assembles per paint.
///  </para>
///  <para>
///   Default colors are theme-aware: a darker blue with white caption in classic mode, a light
///   blue with black caption in dark mode (resolved via <see cref="Application.IsDarkModeEnabled"/>).
///   Assigning <see cref="Control.BackColor"/>, <see cref="Control.ForeColor"/> or
///   <see cref="BorderColor"/> overrides the theme default; untouched defaults follow system
///   color-mode changes at runtime.
///  </para>
///  <para>
///   Hover, press and release are animated on a shared <c>HighPrecisionTimer</c>; the UI thread
///   is never blocked and the renderer only receives progress snapshots.
///  </para>
/// </remarks>
public class MaterialKeyButton : Control, IButtonControl
{
    private readonly MaterialKeyButtonRenderer _renderer = MaterialKeyButtonRenderer.Default;
    private readonly AnimationChannel _hover = new();
    private readonly AnimationChannel _press = new();

    private TimerRegistration? _timerRegistration;
    private bool _flashReturnPending;

    private bool _mouseOver;
    private bool _mouseDown;
    private bool _mouseInsideWhileDown;
    private bool _spaceDown;
    private bool _isDefault;

    private bool _backColorIsThemeDefault = true;
    private bool _foreColorIsThemeDefault = true;
    private Color _borderColor;
    private bool _borderColorIsThemeDefault = true;

    private int _borderWidth = 1;
    private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
    private MaterialKeyButtonTextEffect _textEffect = MaterialKeyButtonTextEffect.Raised;
    private DialogResult _dialogResult = DialogResult.None;

    /// <summary>
    ///  Initializes a new instance of the <see cref="MaterialKeyButton"/> class.
    /// </summary>
    public MaterialKeyButton()
    {
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.StandardClick
                | ControlStyles.Selectable,
            true);

        // A fast double press on a mechanical key is two strokes, not a double-click.
        SetStyle(ControlStyles.StandardDoubleClick, false);

        TabStop = true;
        AccessibleRole = AccessibleRole.PushButton;

        base.BackColor = DefaultKeyBackColor;
        base.ForeColor = DefaultKeyForeColor;
        _borderColor = DefaultKeyBorderColor;
    }

    #region Internal static all-purpose settings (shared, for tuning the best defaults)

    /// <summary>
    ///  Gets or sets the process-wide corner radius magnitude.
    /// </summary>
    internal static MaterialKeyMetric CornerRadius
    {
        get => MaterialKeyButtonRendererOptions.Shared.CornerRadius;
        set => MaterialKeyButtonRendererOptions.Shared.CornerRadius = value;
    }

    /// <summary>
    ///  Gets or sets the process-wide concavity depth magnitude.
    /// </summary>
    internal static MaterialKeyMetric ConcavityDepth
    {
        get => MaterialKeyButtonRendererOptions.Shared.ConcavityDepth;
        set => MaterialKeyButtonRendererOptions.Shared.ConcavityDepth = value;
    }

    /// <summary>
    ///  Gets or sets the process-wide highlight strength magnitude.
    /// </summary>
    internal static MaterialKeyMetric HighlightStrength
    {
        get => MaterialKeyButtonRendererOptions.Shared.HighlightStrength;
        set => MaterialKeyButtonRendererOptions.Shared.HighlightStrength = value;
    }

    /// <summary>
    ///  Gets or sets the process-wide shadow strength magnitude.
    /// </summary>
    internal static MaterialKeyMetric ShadowStrength
    {
        get => MaterialKeyButtonRendererOptions.Shared.ShadowStrength;
        set => MaterialKeyButtonRendererOptions.Shared.ShadowStrength = value;
    }

    /// <summary>
    ///  Gets or sets the process-wide base animation duration.
    /// </summary>
    internal static TimeSpan AnimationDuration
    {
        get => MaterialKeyButtonRendererOptions.Shared.AnimationDuration;
        set => MaterialKeyButtonRendererOptions.Shared.AnimationDuration = value;
    }

    #endregion

    #region Theme defaults

    // Explicit ARGB palettes per color mode: SystemColors names are remapped (roughly
    // complementary) in dark mode, so relying on them for a branded default is not viable.

    private static Color DefaultKeyBackColor
        => Application.IsDarkModeEnabled
            ? Color.FromArgb(unchecked((int)0xFF9CC7F0))   // light blue, black caption
            : Color.FromArgb(unchecked((int)0xFF1F5AA8));  // darker blue, white caption

    private static Color DefaultKeyForeColor
        => Application.IsDarkModeEnabled
            ? Color.FromArgb(unchecked((int)0xFF101418))
            : Color.White;

    private static Color DefaultKeyBorderColor
        => Application.IsDarkModeEnabled
            ? Color.FromArgb(unchecked((int)0xFF4A7CB8))
            : Color.FromArgb(unchecked((int)0xFF143C70));

    #endregion

    #region Public properties

    /// <inheritdoc/>
    /// <remarks>
    ///  <para>
    ///   Re-exposed so it is browsable; a key with <see cref="AutoSize"/> enabled always
    ///   assumes its natural, clip-proof size.
    ///  </para>
    /// </remarks>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DefaultValue(false)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set
        {
            base.AutoSize = value;
            AdjustSize();
        }
    }

    /// <summary>
    ///  Gets or sets the border width of the key body in pixels. <c>0</c> disables the border.
    /// </summary>
    [Category("Appearance")]
    [Description("The border width of the key body in pixels. 0 disables the border.")]
    [DefaultValue(1)]
    public int BorderWidth
    {
        get => _borderWidth;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (_borderWidth == value)
            {
                return;
            }

            _borderWidth = value;
            AdjustSize();
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the border color of the key body.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The default is theme-dependent (classic vs. dark mode) and therefore serialized via
    ///   the <c>ShouldSerialize</c>/<c>Reset</c> pattern rather than <c>[DefaultValue]</c>.
    ///  </para>
    /// </remarks>
    [Category("Appearance")]
    [Description("The border color of the key body.")]
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            if (_borderColor == value)
            {
                return;
            }

            _borderColor = value;
            _borderColorIsThemeDefault = false;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the alignment of the caption on the keytop.
    /// </summary>
    [Category("Appearance")]
    [Description("The alignment of the caption on the keytop.")]
    [DefaultValue(ContentAlignment.MiddleCenter)]
    public ContentAlignment TextAlign
    {
        get => _textAlign;
        set
        {
            if (_textAlign == value)
            {
                return;
            }

            _textAlign = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets how the caption is physically integrated into the key surface.
    /// </summary>
    [Category("Appearance")]
    [Description("Whether the caption appears raised above or engraved into the key surface.")]
    [DefaultValue(MaterialKeyButtonTextEffect.Raised)]
    public MaterialKeyButtonTextEffect TextEffect
    {
        get => _textEffect;
        set
        {
            if (_textEffect == value)
            {
                return;
            }

            _textEffect = value;
            Invalidate();
        }
    }

    /// <inheritdoc cref="IButtonControl.DialogResult"/>
    [Category("Behavior")]
    [Description("The dialog result produced in a modal form by clicking the key.")]
    [DefaultValue(DialogResult.None)]
    public DialogResult DialogResult
    {
        get => _dialogResult;
        set => _dialogResult = value;
    }

    #endregion

    #region Theme-default serialization plumbing

    private bool ShouldSerializeBackColor()
        => !_backColorIsThemeDefault;

    /// <summary>
    ///  Resets <see cref="Control.BackColor"/> to the theme-dependent default.
    /// </summary>
    public override void ResetBackColor()
    {
        base.BackColor = DefaultKeyBackColor;
        _backColorIsThemeDefault = true;
        Invalidate();
    }

    private bool ShouldSerializeForeColor()
        => !_foreColorIsThemeDefault;

    /// <summary>
    ///  Resets <see cref="Control.ForeColor"/> to the theme-dependent default.
    /// </summary>
    public override void ResetForeColor()
    {
        base.ForeColor = DefaultKeyForeColor;
        _foreColorIsThemeDefault = true;
        Invalidate();
    }

    private bool ShouldSerializeBorderColor()
        => !_borderColorIsThemeDefault;

    /// <summary>
    ///  Resets <see cref="BorderColor"/> to the theme-dependent default.
    /// </summary>
    public void ResetBorderColor()
    {
        _borderColor = DefaultKeyBorderColor;
        _borderColorIsThemeDefault = true;
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnBackColorChanged(EventArgs e)
    {
        // Any assignment that lands on a non-default value marks the color as user-owned.
        if (BackColor != DefaultKeyBackColor)
        {
            _backColorIsThemeDefault = false;
        }

        base.OnBackColorChanged(e);
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnForeColorChanged(EventArgs e)
    {
        if (ForeColor != DefaultKeyForeColor)
        {
            _foreColorIsThemeDefault = false;
        }

        base.OnForeColorChanged(e);
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);

        // Untouched theme defaults follow a runtime light/dark switch.
        if (_backColorIsThemeDefault)
        {
            base.BackColor = DefaultKeyBackColor;
        }

        if (_foreColorIsThemeDefault)
        {
            base.ForeColor = DefaultKeyForeColor;
        }

        if (_borderColorIsThemeDefault)
        {
            _borderColor = DefaultKeyBorderColor;
        }

        Invalidate();
    }

    #endregion

    #region IButtonControl

    /// <inheritdoc/>
    public void NotifyDefault(bool value)
    {
        if (_isDefault == value)
        {
            return;
        }

        _isDefault = value;
        Invalidate();
    }

    /// <inheritdoc/>
    /// <remarks>
    ///  <para>
    ///   Plays a short press-flash animation so a programmatic or <kbd>Enter</kbd>-triggered
    ///   click still reads as a physical key stroke.
    ///  </para>
    /// </remarks>
    public void PerformClick()
    {
        if (!CanSelect || !Enabled)
        {
            return;
        }

        StartPressFlash();
        OnClick(EventArgs.Empty);
    }

    #endregion

    #region Layout & sizing

    /// <inheritdoc/>
    protected override Size DefaultSize
        => new(96, 44);

    /// <inheritdoc/>
    public override Size GetPreferredSize(Size proposedSize)
        => _renderer.GetPreferredSize(CreateRenderContext(ClientRectangle), proposedSize);

    /// <inheritdoc/>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (AutoSize && (specified & BoundsSpecified.Size) != 0)
        {
            Size preferred = GetPreferredSize(Size.Empty);
            width = preferred.Width;
            height = preferred.Height;
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    private void AdjustSize()
    {
        if (AutoSize)
        {
            Size = GetPreferredSize(Size.Empty);
        }
    }

    /// <inheritdoc/>
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        AdjustSize();
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        AdjustSize();
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        AdjustSize();
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        AdjustSize();
        Invalidate();
    }

    #endregion

    #region Painting

    /// <inheritdoc/>
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        // The key is drawn as a rounded shape with an ambient shadow; the area around it
        // shows the parent's surface, not the key's face color.
        Color background = Parent?.BackColor
            ?? (Application.IsDarkModeEnabled
                ? Color.FromArgb(unchecked((int)0xFF202020))
                : SystemColors.Control);

        using SolidBrush brush = new(background);
        pevent.Graphics.FillRectangle(brush, ClientRectangle);
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        _renderer.Render(e.Graphics, CreateRenderContext(ClientRectangle));
        base.OnPaint(e);
    }

    private MaterialKeyButtonRenderContext CreateRenderContext(Rectangle bounds)
        => new()
        {
            Bounds = bounds,
            Text = Text,
            Font = Font,
            BackColor = BackColor,
            ForeColor = ForeColor,
            BorderColor = BorderColor,
            BorderWidth = BorderWidth,
            Enabled = Enabled,
            Focused = Focused && ShowFocusCues,
            IsDefault = _isDefault,
            RenderState = GetRenderState(),
            AnimationState = new MaterialKeyButtonAnimationState(_hover.Current, _press.Current),
            TextAlign = TextAlign,
            RightToLeft = RightToLeft,
            Padding = Padding,
            DeviceDpi = DeviceDpi,
            TextEffect = TextEffect,
            ShowKeyboardCues = ShowKeyboardCues
        };

    private MaterialKeyButtonRenderState GetRenderState()
    {
        if (!Enabled)
        {
            return MaterialKeyButtonRenderState.Disabled;
        }

        MaterialKeyButtonRenderState state = MaterialKeyButtonRenderState.Normal;

        if (_mouseOver)
        {
            state |= MaterialKeyButtonRenderState.Hover;
        }

        if ((_mouseDown && _mouseInsideWhileDown) || _spaceDown || _flashReturnPending)
        {
            state |= MaterialKeyButtonRenderState.Pressed;
        }

        if (Focused)
        {
            state |= MaterialKeyButtonRenderState.Focused;
        }

        if (_isDefault)
        {
            state |= MaterialKeyButtonRenderState.Default;
        }

        return state;
    }

    #endregion

    #region Mouse & touch input

    // Touch input arrives through the promoted WM_LBUTTON* messages, i.e. through the same
    // mouse events handled here — press/release behave identically for finger and mouse.

    /// <inheritdoc/>
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _mouseOver = true;
        AnimateHover(1f);
    }

    /// <inheritdoc/>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _mouseOver = false;
        AnimateHover(0f);
    }

    /// <inheritdoc/>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button != MouseButtons.Left || !Enabled)
        {
            return;
        }

        Focus();
        _mouseDown = true;
        _mouseInsideWhileDown = true;
        AnimatePress(1f);
    }

    /// <inheritdoc/>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!_mouseDown)
        {
            return;
        }

        // The control has mouse capture: when the pointer drags off the key while held down,
        // the key pops back up; dragging back in presses it again. Click itself is raised by
        // the base class only when the release happens inside the bounds.
        bool inside = ClientRectangle.Contains(e.Location);

        if (inside != _mouseInsideWhileDown)
        {
            _mouseInsideWhileDown = inside;
            AnimatePress(inside ? 1f : 0f);
        }
    }

    /// <inheritdoc/>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        _mouseDown = false;
        _mouseInsideWhileDown = false;
        AnimatePress(0f);
    }

    /// <inheritdoc/>
    protected override void OnClick(EventArgs e)
    {
        if (DialogResult != DialogResult.None && FindForm() is { Modal: true } form)
        {
            form.DialogResult = DialogResult;
        }

        base.OnClick(e);
    }

    #endregion

    #region Keyboard input

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (!Enabled)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Space when !e.Alt:
                _spaceDown = true;
                AnimatePress(1f);
                e.Handled = true;
                break;

            case Keys.Enter:
                PerformClick();
                e.Handled = true;
                break;
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (e.KeyCode == Keys.Space && _spaceDown)
        {
            _spaceDown = false;
            AnimatePress(0f);

            if (Enabled)
            {
                OnClick(EventArgs.Empty);
            }

            e.Handled = true;
        }
    }

    /// <inheritdoc/>
    protected override bool ProcessMnemonic(char charCode)
    {
        if (Enabled && Visible && IsMnemonic(charCode, Text))
        {
            PerformClick();

            return true;
        }

        return base.ProcessMnemonic(charCode);
    }

    #endregion

    #region Focus & enabled state

    /// <inheritdoc/>
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);

        if (_spaceDown)
        {
            // A pending space-press is abandoned when focus moves away — matches Button.
            _spaceDown = false;
            AnimatePress(0f);
        }

        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);

        _mouseDown = false;
        _mouseInsideWhileDown = false;
        _spaceDown = false;
        _flashReturnPending = false;
        _hover.Snap(0f);
        _press.Snap(0f);
        StopAnimationTimer();
        Invalidate();
    }

    #endregion

    #region Animation

    private static double BaseDurationMs
        => Math.Max(1d, MaterialKeyButtonRendererOptions.Shared.AnimationDuration.TotalMilliseconds);

    private void AnimateHover(float target)
    {
        // Ease-out both ways; leaving takes a touch longer, like a key settling back.
        double duration = target > _hover.Current ? BaseDurationMs : BaseDurationMs * 1.4d;
        _hover.AnimateTo(target, duration, Easing.EaseOutCubic);
        EnsureAnimationTimer();
    }

    private void AnimatePress(float target)
    {
        // Press is snappier than hover; release slightly softer than press.
        double duration = target > _press.Current ? BaseDurationMs * 0.5d : BaseDurationMs * 0.75d;
        _press.AnimateTo(target, duration, Easing.EaseInOutQuad);
        EnsureAnimationTimer();
    }

    private void StartPressFlash()
    {
        _flashReturnPending = true;
        _press.AnimateTo(1f, BaseDurationMs * 0.4d, Easing.EaseInOutQuad);
        EnsureAnimationTimer();
    }

    private void EnsureAnimationTimer()
    {
        if (_timerRegistration is not null)
        {
            return;
        }

        if (!IsHandleCreated || IsDisposed)
        {
            // No message loop to marshal to — snap to the targets and repaint once.
            _hover.Snap(_hover.Target);
            _press.Snap(_press.Target);
            _flashReturnPending = false;
            Invalidate();

            return;
        }

        _timerRegistration = HighPrecisionTimer.Register(OnAnimationTickAsync);
    }

    private void StopAnimationTimer()
    {
        _timerRegistration?.Dispose();
        _timerRegistration = null;
    }

    private ValueTask OnAnimationTickAsync(HighPrecisionTimerTick tick, CancellationToken cancellationToken)
    {
        // Marshaled to the UI thread by the timer via the captured SynchronizationContext.
        if (IsDisposed || cancellationToken.IsCancellationRequested)
        {
            StopAnimationTimer();

            return ValueTask.CompletedTask;
        }

        double deltaMs = tick.ElapsedSinceLastTick.TotalMilliseconds;
        bool hoverActive = _hover.Tick(deltaMs);
        bool pressActive = _press.Tick(deltaMs);

        if (_flashReturnPending && !pressActive && _press.Current >= 1f)
        {
            // Flash reached the bottom of the stroke — release the key.
            _flashReturnPending = false;
            _press.AnimateTo(0f, BaseDurationMs * 0.75d, Easing.EaseOutCubic);
            pressActive = true;
        }

        Invalidate();

        if (!hoverActive && !pressActive)
        {
            StopAnimationTimer();
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        StopAnimationTimer();
        base.OnHandleDestroyed(e);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAnimationTimer();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  One animated scalar (hover or press) with retargetable, eased interpolation.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Retargeting mid-flight restarts the easing from the <em>current</em> value, so fast
    ///   hover/press/release sequences never jump.
    ///  </para>
    /// </remarks>
    private sealed class AnimationChannel
    {
        private float _startValue;
        private double _elapsedMs;
        private double _durationMs = 1d;
        private Func<float, float> _easing = Easing.EaseOutCubic;

        public float Current { get; private set; }

        public float Target { get; private set; }

        public void AnimateTo(float target, double durationMs, Func<float, float> easing)
        {
            if (MathF.Abs(target - Target) < 0.0001f && MathF.Abs(target - Current) < 0.0001f)
            {
                return;
            }

            _startValue = Current;
            Target = target;
            _elapsedMs = 0d;
            _durationMs = Math.Max(1d, durationMs);
            _easing = easing;
        }

        public void Snap(float value)
        {
            Current = value;
            Target = value;
            _elapsedMs = 0d;
        }

        /// <summary>
        ///  Advances the channel; returns whether it is still animating.
        /// </summary>
        public bool Tick(double deltaMs)
        {
            if (Current == Target)
            {
                return false;
            }

            _elapsedMs += Math.Max(0d, deltaMs);
            float t = (float)Math.Clamp(_elapsedMs / _durationMs, 0d, 1d);

            Current = t >= 1f
                ? Target
                : _startValue + ((Target - _startValue) * _easing(t));

            return Current != Target;
        }
    }

    #endregion
}
