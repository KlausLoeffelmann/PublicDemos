// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Optional scenario capability for views whose push buttons can demonstrate a shared
///  <see cref="FlatStyle"/>.
/// </summary>
internal interface IFlatStyleScenarioView
{
    /// <summary>Gets the currently applied push-button style.</summary>
    FlatStyle CurrentFlatStyle { get; }

    /// <summary>Applies one push-button style to the complete scenario.</summary>
    void ApplyFlatStyle(FlatStyle flatStyle);
}
