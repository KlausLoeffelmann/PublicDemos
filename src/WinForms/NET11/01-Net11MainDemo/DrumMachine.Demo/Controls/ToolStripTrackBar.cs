using System.ComponentModel;
using System.Windows.Forms.Design;

namespace DrumMachine.Demo.Controls;

/// <summary>
///  Hosts a native WinForms slider with serializable scalar properties and edit-gesture boundaries.
/// </summary>
/// <remarks>
///  Use the inherited Size and Width properties for layout. WinForms scales the hosted
///  control and its minimum thumb clearance normally; this host never scales its Font.
///  Keep a flow-layout ToolStrip and Overflow.Never so the slider remains keyboard accessible.
/// </remarks>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(ValueChanged))]
[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
public class ToolStripTrackBar : ToolStripControlHost, ISupportInitialize
{
    private GestureKind _gesture;
    private Keys _gestureKey;
    private bool _initializing;
    private int _initialValue;
    private int? _pendingValue;
    private bool _disposing;

    /// <summary>
    ///  Creates a keyboard-accessible horizontal stock slider, nominally 100 by 32 logical pixels.
    /// </summary>
    public ToolStripTrackBar()
        : base(new HostedTrackBar())
    {
        Overflow = ToolStripItemOverflow.Never;
    }

    /// <summary>
    ///  Gets or sets the lower bound of the slider.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(0)]
    public int Minimum
    {
        get => TrackBar.Minimum;
        set
        {
            int previousValue = TrackBar.Value;
            TrackBar.Minimum = value;
            NotifyRangeChange(previousValue);
        }
    }

    /// <summary>
    ///  Gets or sets the upper bound of the slider.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(10)]
    public int Maximum
    {
        get => TrackBar.Maximum;
        set
        {
            int previousValue = TrackBar.Value;
            TrackBar.Maximum = value;
            NotifyRangeChange(previousValue);
        }
    }

    /// <summary>
    ///  Gets or sets the current value without starting a user gesture for programmatic changes.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(0)]
    [Bindable(true)]
    public int Value
    {
        get => _pendingValue ?? TrackBar.Value;
        set
        {
            if (_initializing)
            {
                _pendingValue = value;
            }
            else
            {
                TrackBar.Value = value;
            }
        }
    }

    /// <summary>
    ///  Gets or sets the change made by an arrow-key step.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(1)]
    public int SmallChange
    {
        get => TrackBar.SmallChange;
        set => TrackBar.SmallChange = value;
    }

    /// <summary>
    ///  Gets or sets the change made by a page-key or track click.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(5)]
    public int LargeChange
    {
        get => TrackBar.LargeChange;
        set => TrackBar.LargeChange = value;
    }

    /// <summary>
    ///  Gets or sets the native slider's tick interval, although toolbar ticks are hidden.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(1)]
    public int TickFrequency
    {
        get => TrackBar.TickFrequency;
        set => TrackBar.TickFrequency = value;
    }

    /// <summary>
    ///  Occurs for value previews and programmatic value changes.
    /// </summary>
    [Category("Action")]
    public event EventHandler? ValueChanged;

    /// <summary>
    ///  Occurs once before a mouse, keyboard-repeat, or mouse-wheel edit starts.
    /// </summary>
    [Category("Action")]
    public event EventHandler? GestureStarted;

    /// <summary>
    ///  Occurs once when a gesture ends or is explicitly committed.
    /// </summary>
    [Category("Action")]
    public event EventHandler? GestureCompleted;

    /// <summary>
    ///  Finishes any outstanding edit before a command, save, target change, or Undo.
    /// </summary>
    public void CommitGesture()
    {
        if (_gesture == GestureKind.None)
        {
            return;
        }

        _gesture = GestureKind.None;
        _gestureKey = Keys.None;
        if (!_disposing)
        {
            GestureCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Preserves the inherited Width and Height when the strip asks for an automatic flow-layout size.
    /// </summary>
    public override Size GetPreferredSize(Size constrainingSize)
    {
        if (Control is not Control control)
        {
            return base.GetPreferredSize(constrainingSize);
        }

        // ToolStripItem.Width bypasses ToolStripControlHost.Size and does not update the
        // child's specified bounds. The stock host would otherwise revert that width.
        return new Size(Width, Math.Max(Height, control.MinimumSize.Height + Padding.Vertical));
    }

    /// <summary>
    ///  Defers the initial value until the Designer has supplied both range bounds.
    /// </summary>
    void ISupportInitialize.BeginInit()
    {
        if (!_initializing)
        {
            _initialValue = TrackBar.Value;
            _initializing = true;
            TrackBar.BeginInit();
        }
    }

    /// <summary>
    ///  Applies and constrains the initial value using the final serialized range.
    /// </summary>
    void ISupportInitialize.EndInit()
    {
        if (!_initializing)
        {
            return;
        }

        try
        {
            if (_pendingValue is int value)
            {
                TrackBar.Value = value;
            }

            TrackBar.EndInit();
        }
        finally
        {
            _pendingValue = null;
            _initializing = false;
        }

        if (_initialValue != TrackBar.Value)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Subscribes to the hosted control's value, input, and gesture-termination events.
    /// </summary>
    protected override void OnSubscribeControlEvents(Control? control)
    {
        base.OnSubscribeControlEvents(control);
        if (control is HostedTrackBar trackBar)
        {
            trackBar.ValueChanged += TrackBar_ValueChanged;
            trackBar.PointerGestureStarting += TrackBar_PointerGestureStarting;
            trackBar.MouseDown += TrackBar_MouseDown;
            trackBar.MouseUp += TrackBar_MouseUp;
            trackBar.MouseCaptureChanged += TrackBar_MouseCaptureChanged;
            trackBar.KeyDown += TrackBar_KeyDown;
            trackBar.KeyUp += TrackBar_KeyUp;
            trackBar.WheelGestureStarting += TrackBar_WheelGestureStarting;
            trackBar.WheelGestureCompleted += TrackBar_WheelGestureCompleted;
            trackBar.Leave += TrackBar_GestureEnded;
            trackBar.LostFocus += TrackBar_GestureEnded;
            trackBar.EnabledChanged += TrackBar_AvailabilityChanged;
            trackBar.VisibleChanged += TrackBar_AvailabilityChanged;
        }
    }

    /// <summary>
    ///  Unsubscribes every event added by this host before the base disposes its native control.
    /// </summary>
    protected override void OnUnsubscribeControlEvents(Control? control)
    {
        if (control is HostedTrackBar trackBar)
        {
            trackBar.ValueChanged -= TrackBar_ValueChanged;
            trackBar.PointerGestureStarting -= TrackBar_PointerGestureStarting;
            trackBar.MouseDown -= TrackBar_MouseDown;
            trackBar.MouseUp -= TrackBar_MouseUp;
            trackBar.MouseCaptureChanged -= TrackBar_MouseCaptureChanged;
            trackBar.KeyDown -= TrackBar_KeyDown;
            trackBar.KeyUp -= TrackBar_KeyUp;
            trackBar.WheelGestureStarting -= TrackBar_WheelGestureStarting;
            trackBar.WheelGestureCompleted -= TrackBar_WheelGestureCompleted;
            trackBar.Leave -= TrackBar_GestureEnded;
            trackBar.LostFocus -= TrackBar_GestureEnded;
            trackBar.EnabledChanged -= TrackBar_AvailabilityChanged;
            trackBar.VisibleChanged -= TrackBar_AvailabilityChanged;
        }

        base.OnUnsubscribeControlEvents(control);
    }

    /// <summary>
    ///  Finishes an edit when moving the host to another strip or removing it.
    /// </summary>
    protected override void OnOwnerChanged(EventArgs e)
    {
        CommitGesture();
        base.OnOwnerChanged(e);
    }

    /// <summary>
    ///  Stops gesture notifications and lets the standard host dispose its owned control.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposing = true;
            _gesture = GestureKind.None;
            _pendingValue = null;
            ValueChanged = null;
            GestureStarted = null;
            GestureCompleted = null;
        }

        base.Dispose(disposing);
    }

    private HostedTrackBar TrackBar
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposing || IsDisposed, this);
            return (HostedTrackBar)Control;
        }
    }

    private void StartGesture(GestureKind kind, Keys key = Keys.None)
    {
        if (_disposing || _initializing || (_gesture == kind && _gestureKey == key))
        {
            return;
        }

        CommitGesture();
        _gesture = kind;
        _gestureKey = key;
        GestureStarted?.Invoke(this, EventArgs.Empty);
    }

    private void TrackBar_ValueChanged(object? sender, EventArgs e)
    {
        if (!_initializing && !_disposing)
        {
            ValueChanged?.Invoke(this, e);
        }
    }

    private void NotifyRangeChange(int previousValue)
    {
        // The native TrackBar silently clamps Value when a range bound changes.
        if (TrackBar.Value != previousValue)
        {
            TrackBar_ValueChanged(TrackBar, EventArgs.Empty);
        }
    }

    private void TrackBar_PointerGestureStarting(object? sender, EventArgs e)
        => StartGesture(GestureKind.Mouse);

    private void TrackBar_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            StartGesture(GestureKind.Mouse);
        }
    }

    private void TrackBar_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && _gesture == GestureKind.Mouse)
        {
            CommitGesture();
        }
    }

    private void TrackBar_MouseCaptureChanged(object? sender, EventArgs e)
    {
        if (_gesture == GestureKind.Mouse && sender is TrackBar { Capture: false })
        {
            CommitGesture();
        }
    }

    private void TrackBar_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!e.Handled && !e.SuppressKeyPress && (e.Modifiers & Keys.Alt) == 0
            && IsAdjustmentKey(e.KeyCode))
        {
            StartGesture(GestureKind.Keyboard, e.KeyCode);
        }
    }

    private void TrackBar_KeyUp(object? sender, KeyEventArgs e)
    {
        if (_gesture == GestureKind.Keyboard && _gestureKey == e.KeyCode)
        {
            CommitGesture();
        }
    }

    private void TrackBar_WheelGestureStarting(object? sender, EventArgs e)
        => StartGesture(GestureKind.Wheel);

    private void TrackBar_WheelGestureCompleted(object? sender, EventArgs e)
    {
        if (_gesture == GestureKind.Wheel)
        {
            CommitGesture();
        }
    }

    private void TrackBar_GestureEnded(object? sender, EventArgs e) => CommitGesture();

    private void TrackBar_AvailabilityChanged(object? sender, EventArgs e)
    {
        if (sender is Control control && (!control.Enabled || !control.Visible))
        {
            CommitGesture();
        }
    }

    private static bool IsAdjustmentKey(Keys key)
        => key is Keys.Left or Keys.Right or Keys.Up or Keys.Down
            or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End;

    private enum GestureKind
    {
        None,
        Mouse,
        Keyboard,
        Wheel
    }

    private sealed class HostedTrackBar : TrackBar
    {
        /// <summary>
        ///  Configures the internal stock control without serializing its implementation details on a form.
        /// </summary>
        internal HostedTrackBar()
        {
            AutoSize = false;
            TickStyle = TickStyle.None;
            TabStop = true;
            Size = new Size(100, 32);
            MinimumSize = new Size(0, 32);
        }

        /// <summary>
        ///  Occurs before the native trackbar can change its value on a track click.
        /// </summary>
        internal event EventHandler? PointerGestureStarting;

        /// <summary>
        ///  Occurs before the stock mouse-wheel handler changes the value.
        /// </summary>
        internal event EventHandler? WheelGestureStarting;

        /// <summary>
        ///  Occurs after the stock mouse-wheel handler has finished all value notifications.
        /// </summary>
        internal event EventHandler? WheelGestureCompleted;

        /// <summary>
        ///  Starts pointer edits before native processing, which precedes the ordinary MouseDown event.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            const int WmLeftButtonDown = 0x0201;
            const int WmLeftButtonDoubleClick = 0x0203;
            if (Enabled && (m.Msg == WmLeftButtonDown || m.Msg == WmLeftButtonDoubleClick))
            {
                PointerGestureStarting?.Invoke(this, EventArgs.Empty);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        ///  Brackets a mouse-wheel message without replacing native scrolling behavior.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta == 0 || e is HandledMouseEventArgs { Handled: true }
                || (ModifierKeys & (Keys.Shift | Keys.Alt)) != 0 || MouseButtons != MouseButtons.None)
            {
                base.OnMouseWheel(e);
                return;
            }

            WheelGestureStarting?.Invoke(this, EventArgs.Empty);
            try
            {
                base.OnMouseWheel(e);
            }
            finally
            {
                WheelGestureCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
