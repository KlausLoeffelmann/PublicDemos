// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Repro scenarios for TextBox / RichTextBox non-client (border) painting: hovering the mouse
///  over these controls must NOT erase or corrupt the rendered text.
/// </summary>
public partial class TextBoxScenariosView : UserControl, IScenarioView
{
    public TextBoxScenariosView()
    {
        InitializeComponent();
    }

    protected override void CreateHandle()
    {
        base.CreateHandle();
    }

    public string DisplayName => "TextBox / RichTextBox Scenarios";
}
