// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Stress scenario that drives hover, pressed, and checked transitions across 156 ButtonBase
///  controls on the same UI-thread timer tick.
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

    private readonly List<ButtonBase> _animatedControls = [];
    private readonly List<Button> _pushButtons = [];
    private readonly List<CheckBox> _checkBoxes = [];
    private readonly List<RadioButton> _radioButtons = [];
    private bool _activePhase;
    private long _phaseCount;

    public ParallelAnimationView()
    {
        InitializeComponent();
        BuildMatrix();
        UpdateStatus();
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

        for (int column = 0; column < ColumnCount; column++)
        {
            _animationTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116F));
        }

        for (int row = 0; row < RowCount; row++)
        {
            _animationTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        }

        for (int index = 0; index < descriptors.Count; index++)
        {
            ControlDescriptor descriptor = descriptors[index];
            ButtonBase control = CreateControl(descriptor, index);

            _animatedControls.Add(control);
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
        _activePhase = !_activePhase;
        _phaseCount++;

        MethodInfo hoverMethod = _activePhase ? s_onMouseEnterMethod : s_onMouseLeaveMethod;
        foreach (ButtonBase control in _animatedControls)
        {
            hoverMethod.Invoke(control, s_eventArguments);
        }

        foreach (Button button in _pushButtons)
        {
            if (_activePhase)
            {
                s_onMouseDownMethod.Invoke(button, s_mouseDownArguments);
            }
            else
            {
                s_resetFlagsAndPaintMethod.Invoke(button, parameters: null);
            }
        }

        foreach (CheckBox checkBox in _checkBoxes)
        {
            checkBox.Checked = !checkBox.Checked;
        }

        foreach (RadioButton radioButton in _radioButtons)
        {
            radioButton.Checked = !radioButton.Checked;
        }

        UpdateStatus();
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
            s_onMouseLeaveMethod.Invoke(control, s_eventArguments);
        }

        foreach (Button button in _pushButtons)
        {
            s_resetFlagsAndPaintMethod.Invoke(button, parameters: null);
        }

        _activePhase = false;
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        string lifecycle = _animationTimer.Enabled ? "running" : "stopped";
        string phase = _activePhase ? "hot / pressed" : "normal / released";

        _statusLabel.Text =
            $"{_animatedControls.Count} controls | {_pushButtons.Count} Buttons | "
            + $"{_checkBoxes.Count} CheckBoxes | {_radioButtons.Count} RadioButtons | "
            + $"{lifecycle}, phase {_phaseCount}: {phase}";
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
}
