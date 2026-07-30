// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Stress scenario that drives hover, pressed, and checked transitions across 156 ButtonBase
///  controls. Rather than animating every control on every timer tick, a random subset of
///  controls independently starts and stops short animation "bursts", capped by a
///  user-adjustable concurrency limit (see the ToolStrip slider), which is a more realistic
///  stress pattern than flipping every control in lockstep.
/// </summary>
public partial class ParallelAnimationView : UserControl, IScenarioView
{
    private const int ColumnCount = 12;
    private const int RowCount = 13;
    private const int RandomSeed = 14694;

    private static readonly MethodInfo s_onMouseEnterMethod = GetRequiredMethod("OnMouseEnter", typeof(EventArgs));
    private static readonly MethodInfo s_onMouseLeaveMethod = GetRequiredMethod("OnMouseLeave", typeof(EventArgs));
    private static readonly MethodInfo s_onMouseDownMethod = GetRequiredMethod("OnMouseDown", typeof(MouseEventArgs));
    private static readonly MethodInfo s_resetFlagsAndPaintMethod = GetRequiredMethod("ResetFlagsandPaint");
    private static readonly object?[] s_eventArguments = [EventArgs.Empty];
    private static readonly object?[] s_mouseDownArguments =
    [
        new MouseEventArgs(MouseButtons.Left, clicks: 1, x: 1, y: 1, delta: 0),
    ];

    // Random burst duration bounds, expressed in timer ticks. A "burst" is one control's
    // independent run of hover/pressed/checked toggling before it goes idle again.
    private const int MinBurstTicks = 2;
    private const int MaxBurstTicks = 8;

    private readonly List<ButtonBase> _animatedControls = [];
    private readonly List<Button> _pushButtons = [];
    private readonly List<CheckBox> _checkBoxes = [];
    private readonly List<RadioButton> _radioButtons = [];
    private readonly Dictionary<ButtonBase, AnimationState> _animationStates = [];

    // Deliberately NOT seeded: the user asked for genuinely random start/stop timing so the
    // stress pattern differs on every run, unlike the deterministic matrix-shuffle seed above.
    private readonly Random _animationRandom = new();

    private long _phaseCount;
    private int _maxConcurrent = 24;

    public ParallelAnimationView()
    {
        InitializeComponent();
        BuildMatrix();
        UpdateStatus(activeCount: 0);
    }

    public string DisplayName => "Parallel Button Animations";

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateAnimationTimer();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        StopAnimation();
        base.OnHandleDestroyed(e);
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        UpdateAnimationTimer();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        UpdateAnimationTimer();
    }

    private static MethodInfo GetRequiredMethod(string name, params Type[] parameterTypes)
        => typeof(ButtonBase).GetMethod(
            name,
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: parameterTypes,
            modifiers: null)
        ?? throw new MissingMethodException(typeof(ButtonBase).FullName, name);

    private void BuildMatrix()
    {
        FlatStyle[] flatStyles =
        [
            FlatStyle.Standard,
            FlatStyle.Popup,
            FlatStyle.Flat,
            FlatStyle.System,
        ];

        Appearance[] appearances =
        [
            Appearance.Normal,
            Appearance.Button,
            Appearance.ToggleSwitch,
        ];

        List<ControlDescriptor> descriptors = new(ColumnCount * RowCount);
        for (int index = 0; index < ColumnCount * RowCount; index++)
        {
            descriptors.Add(new ControlDescriptor(
                Kind: (ControlKind)(index % 3),
                FlatStyle: flatStyles[(index / 3) % flatStyles.Length],
                Appearance: appearances[(index / 12) % appearances.Length],
                InitiallyChecked: (index & 1) == 0));
        }

        Random random = new(RandomSeed);
        for (int index = descriptors.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            (descriptors[index], descriptors[swapIndex]) = (descriptors[swapIndex], descriptors[index]);
        }

        _animationTableLayoutPanel.SuspendLayout();

        // Percent-based (rather than fixed-pixel) column/row styles let every cell share the
        // available real estate equally, so the matrix grows or shrinks with the view instead
        // of relying on scrollbars.
        for (int column = 0; column < ColumnCount; column++)
        {
            _animationTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / ColumnCount));
        }

        for (int row = 0; row < RowCount; row++)
        {
            _animationTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / RowCount));
        }

        for (int index = 0; index < descriptors.Count; index++)
        {
            ControlDescriptor descriptor = descriptors[index];
            ButtonBase control = CreateControl(descriptor, index);

            _animatedControls.Add(control);
            _animationStates[control] = new AnimationState();
            _animationTableLayoutPanel.Controls.Add(control, index % ColumnCount, index / ColumnCount);
        }

        _animationTableLayoutPanel.ResumeLayout(true);
    }

    private ButtonBase CreateControl(ControlDescriptor descriptor, int index)
    {
        ButtonBase control;

        switch (descriptor.Kind)
        {
            case ControlKind.Button:
                Button button = new();
                _pushButtons.Add(button);
                control = button;
                break;

            case ControlKind.CheckBox:
                CheckBox checkBox = new()
                {
                    Appearance = descriptor.Appearance,
                    Checked = descriptor.InitiallyChecked,
                };

                _checkBoxes.Add(checkBox);
                control = checkBox;
                break;

            case ControlKind.RadioButton:
                RadioButton radioButton = new()
                {
                    Appearance = descriptor.Appearance,
                    AutoCheck = false,
                    Checked = descriptor.InitiallyChecked,
                };

                _radioButtons.Add(radioButton);
                control = radioButton;
                break;

            default:
                throw new InvalidOperationException($"Unsupported control kind: {descriptor.Kind}.");
        }

        control.AccessibleName = $"{descriptor.Kind} {index + 1}, {descriptor.FlatStyle}";
        control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        control.FlatStyle = descriptor.FlatStyle;
        control.Margin = new Padding(4);
        control.Name = $"_{descriptor.Kind.ToString().ToLowerInvariant()}{index + 1:D3}";
        control.TabIndex = index;
        control.Text = $"{GetKindCaption(descriptor.Kind)} {GetFlatStyleCaption(descriptor.FlatStyle)}";
        control.UseVisualStyleBackColor = true;

        return control;
    }

    private static string GetKindCaption(ControlKind kind) => kind switch
    {
        ControlKind.Button => "Button",
        ControlKind.CheckBox => "Check",
        ControlKind.RadioButton => "Radio",
        _ => throw new InvalidOperationException($"Unsupported control kind: {kind}."),
    };

    private static string GetFlatStyleCaption(FlatStyle flatStyle) => flatStyle switch
    {
        FlatStyle.Standard => "Std",
        FlatStyle.Popup => "Popup",
        FlatStyle.Flat => "Flat",
        FlatStyle.System => "System",
        _ => throw new InvalidOperationException($"Unsupported FlatStyle: {flatStyle}."),
    };

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        _phaseCount++;
        int activeCount = 0;

        // Advance every currently-animating control by one tick; once its burst runs out,
        // reset it to its idle/normal visual state and free it up for a future burst.
        foreach (ButtonBase control in _animatedControls)
        {
            AnimationState state = _animationStates[control];
            if (!state.IsAnimating)
            {
                continue;
            }

            state.CurrentPhaseHot = !state.CurrentPhaseHot;
            SetControlPhase(control, state.CurrentPhaseHot);
            state.RemainingTicks--;

            if (state.RemainingTicks <= 0)
            {
                state.IsAnimating = false;
                SetControlPhase(control, hot: false);
            }
            else
            {
                activeCount++;
            }
        }

        // Randomly start new bursts on idle controls, but never exceed the concurrency cap.
        int freeSlots = _maxConcurrent - activeCount;
        if (freeSlots > 0)
        {
            List<ButtonBase> idleControls = new(_animatedControls.Count);
            foreach (ButtonBase control in _animatedControls)
            {
                if (!_animationStates[control].IsAnimating)
                {
                    idleControls.Add(control);
                }
            }

            // Shuffle so a different random subset of idle controls gets a chance to start
            // each tick, then only take as many as there are free concurrency slots.
            for (int index = idleControls.Count - 1; index > 0; index--)
            {
                int swapIndex = _animationRandom.Next(index + 1);
                (idleControls[index], idleControls[swapIndex]) = (idleControls[swapIndex], idleControls[index]);
            }

            int startCount = Math.Min(freeSlots, idleControls.Count);
            for (int index = 0; index < startCount; index++)
            {
                ButtonBase control = idleControls[index];
                AnimationState state = _animationStates[control];
                state.IsAnimating = true;
                state.CurrentPhaseHot = true;
                state.RemainingTicks = _animationRandom.Next(MinBurstTicks, MaxBurstTicks + 1);
                SetControlPhase(control, hot: true);
                activeCount++;
            }
        }

        UpdateStatus(activeCount);
    }

    /// <summary>
    ///  Applies the hover/pressed (or checked) visual state for a single control, reusing the
    ///  reflection-based ButtonBase hooks so no mouse/keyboard input is simulated on the OS level.
    /// </summary>
    private static void SetControlPhase(ButtonBase control, bool hot)
    {
        MethodInfo hoverMethod = hot ? s_onMouseEnterMethod : s_onMouseLeaveMethod;
        hoverMethod.Invoke(control, s_eventArguments);

        switch (control)
        {
            case Button button when hot:
                s_onMouseDownMethod.Invoke(button, s_mouseDownArguments);
                break;

            case Button button:
                s_resetFlagsAndPaintMethod.Invoke(button, parameters: null);
                break;

            case CheckBox checkBox:
                checkBox.Checked = hot;
                break;

            case RadioButton radioButton:
                radioButton.Checked = hot;
                break;
        }
    }

    private void MaxConcurrentTrackBar_ValueChanged(object sender, EventArgs e)
    {
        _maxConcurrent = _maxConcurrentTrackBar.Value;
        _maxConcurrentValueLabel.Text = $"{_maxConcurrent} / {ColumnCount * RowCount}";
    }

    private void UpdateAnimationTimer()
    {
        bool shouldRun = IsHandleCreated && Parent is not null && Visible && !DesignMode;
        if (shouldRun)
        {
            _animationTimer.Start();
        }
        else
        {
            StopAnimation();
        }
    }

    private void StopAnimation()
    {
        _animationTimer.Stop();

        foreach (ButtonBase control in _animatedControls)
        {
            AnimationState state = _animationStates[control];
            state.IsAnimating = false;
            state.CurrentPhaseHot = false;
            SetControlPhase(control, hot: false);
        }

        UpdateStatus(activeCount: 0);
    }

    private void UpdateStatus(int activeCount)
    {
        string lifecycle = _animationTimer.Enabled ? "running" : "stopped";

        _statusLabel.Text =
            $"{_animatedControls.Count} controls | {_pushButtons.Count} Buttons | "
            + $"{_checkBoxes.Count} CheckBoxes | {_radioButtons.Count} RadioButtons | "
            + $"{lifecycle}, tick {_phaseCount}: {activeCount} / {_maxConcurrent} animating";
    }

    private enum ControlKind
    {
        Button,
        CheckBox,
        RadioButton,
    }

    private readonly record struct ControlDescriptor(
        ControlKind Kind,
        FlatStyle FlatStyle,
        Appearance Appearance,
        bool InitiallyChecked);

    /// <summary>
    ///  Tracks whether a single control is currently mid-burst, which visual phase it's in, and
    ///  how many ticks remain before the burst ends and the control goes idle again.
    /// </summary>
    private sealed class AnimationState
    {
        public bool IsAnimating;
        public bool CurrentPhaseHot;
        public int RemainingTicks;
    }
}
