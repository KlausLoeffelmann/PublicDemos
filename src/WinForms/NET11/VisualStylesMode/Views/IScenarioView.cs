// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Contract every scenario UserControl shown in <see cref="MainForm"/>'s Panel1 implements so the
///  host can build and update the View menu without changing the scenario's runtime layout.
/// </summary>
internal interface IScenarioView
{
    /// <summary>
    ///  The display name used for the View menu item and the status bar.
    /// </summary>
    string DisplayName { get; }
    void SuspendLayout();
    void ResumeLayout();
}
