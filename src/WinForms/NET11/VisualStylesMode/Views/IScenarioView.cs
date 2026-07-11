// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Contract every scenario UserControl (shown in <see cref="MainForm"/>'s Panel1) implements
///  so the host form can drive a single, shared PropertyGrid (in Panel2) against whichever
///  controls the user has selected. Selection is done by double-clicking a control (or its
///  surrounding <see cref="SelectablePanel"/>); Shift + double-click selects a rectangular range.
/// </summary>
internal interface IScenarioView
{
    /// <summary>
    ///  Raised whenever the set of selected controls changes, so the host can
    ///  refresh <see cref="PropertyGrid.SelectedObjects"/> and the status bar.
    /// </summary>
    event EventHandler? SelectionChanged;

    /// <summary>
    ///  The display name used for the View menu item and the status bar.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///  Returns the controls currently selected for editing in the shared PropertyGrid.
    /// </summary>
    IReadOnlyList<Control> GetSelectedControls();

    /// <summary>
    ///  Selects every selectable control in this view (Edit &gt; Select All).
    /// </summary>
    void SelectAll();

    /// <summary>
    ///  Clears the selection of every control in this view (Edit &gt; Clear Selection).
    /// </summary>
    void ClearSelection();

    /// <summary>
    ///  Applies the selection margin (in pixels) - the gap between each selectable control's chrome
    ///  and its selection frame - to every selectable control in this view (View &gt; Selection Margin).
    /// </summary>
    void SetSelectionMargin(int gap);
}
