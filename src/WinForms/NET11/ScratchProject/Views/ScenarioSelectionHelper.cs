// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject.Views;

/// <summary>
///  Small, Designer-friendly helpers shared by the scenario views. Kept intentionally tiny
///  (no control construction) so it never conflicts with Designer-generated InitializeComponent
///  code — it only wires up the "select this control for the shared PropertyGrid" behavior.
/// </summary>
internal static class ScenarioSelectionHelper
{
    /// <summary>
    ///  Associates <paramref name="checkBox"/> with <paramref name="target"/> (via <see cref="Control.Tag"/>)
    ///  and wires <paramref name="checkedChanged"/> so the owning view can recompute its selection.
    /// </summary>
    public static void Bind(CheckBox checkBox, Control target, EventHandler checkedChanged)
    {
        checkBox.Tag = target;
        checkBox.CheckedChanged += checkedChanged;
    }

    /// <summary>
    ///  Collects the controls referenced by the <see cref="Control.Tag"/> of every checked CheckBox
    ///  in <paramref name="checkBoxes"/>.
    /// </summary>
    public static IReadOnlyList<Control> GetChecked(IEnumerable<CheckBox> checkBoxes) =>
        checkBoxes
            .Where(checkBox => checkBox.Checked && checkBox.Tag is Control)
            .Select(checkBox => (Control)checkBox.Tag!)
            .ToArray();
}
