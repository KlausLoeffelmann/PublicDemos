// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo;

/// <summary>
///  Compile-time switches for the conference demo. Restart after changing these values.
/// </summary>
internal static class DemoConstants
{
    /// <summary>Initial application and active-view styling: Classic or Net11.</summary>
    internal const VisualStylesMode StartupVisualStylesMode = VisualStylesMode.Classic;

    /// <summary>The scenario selected when the host opens.</summary>
    internal const DemoView StartupView = DemoView.Customer;

    /// <summary>Use Classic to watch initial painting, or Deferred to reveal the completed form.</summary>
    internal const FormRevealMode StartupFormRevealMode = FormRevealMode.Deferred;

    /// <summary>Delay after each customer control's first paint; zero disables the slowdown.</summary>
    internal const int CustomerFirstPaintDelayMilliseconds = 60;

    /// <summary>Maximum startup interval during which customer paints may be slowed.</summary>
    internal const int CustomerRevealBudgetMilliseconds = 1800;
}

/// <summary>
///  Stable startup identifiers, independent of the View menu order and display captions.
/// </summary>
internal enum DemoView
{
    TextBoxes,
    Buttons,
    CheckBoxesAndRadioButtons,
    CashRegister,
    Customer,
    ParallelAnimations,
    RoundedRectangles,
}
