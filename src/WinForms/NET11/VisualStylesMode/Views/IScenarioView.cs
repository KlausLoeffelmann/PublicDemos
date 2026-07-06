// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Contract every scenario UserControl (shown in <see cref="MainForm"/>'s Panel1) implements
///  so the host form can drive a single, shared PropertyGrid (in Panel2) against whichever
///  controls the user has checked via the CheckBoxes sprinkled throughout the scenario UI.
/// </summary>
internal interface IScenarioView
{
    /// <summary>
    ///  Raised whenever the set of checked (selected) controls changes, so the host can
    ///  refresh <see cref="PropertyGrid.SelectedObjects"/> and the status bar.
    /// </summary>
    event EventHandler? SelectionChanged;

    /// <summary>
    ///  The display name used for the View menu item and the status bar.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///  Returns the controls currently checked for editing in the shared PropertyGrid.
    /// </summary>
    IReadOnlyList<Control> GetSelectedControls();

    /// <summary>
    ///  Checks every selectable CheckBox in this view (Edit > Select All).
    /// </summary>
    void SelectAll();

    /// <summary>
    ///  Unchecks every selectable CheckBox in this view (Edit > Reset Selection).
    /// </summary>
    void ClearSelection();
}
