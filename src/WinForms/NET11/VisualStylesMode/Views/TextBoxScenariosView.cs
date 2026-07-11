// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Repro scenarios for TextBox / RichTextBox non-client (border) painting: hovering the mouse
///  over these controls must NOT erase or corrupt the rendered text. Each control is wrapped in a
///  <see cref="SelectablePanel"/> so one or more can be double-clicked into the shared PropertyGrid
///  at once (the leading <see cref="Label"/> in each row just describes the scenario).
/// </summary>
public partial class TextBoxScenariosView : UserControl, IScenarioView
{
    private readonly SelectionController _selection = new();

    public TextBoxScenariosView()
    {
        InitializeComponent();

        _selection.SelectionChanged += (_, _) => SelectionChanged?.Invoke(this, EventArgs.Empty);

        // Wrap each demo control in a SelectablePanel in place; double-clicking any of them toggles
        // its membership in the shared PropertyGrid selection.
        _selection.WrapAndRegister(
            _textBoxDefault,
            _textBoxFixedSingle,
            _textBoxMultiline,
            _textBoxNoBorderReadOnly,
            _richTextBoxDefault,
            _richTextBoxFixedSingle,
            _richTextBoxNoWordWrap,
            _richTextBoxReadOnly);
    }

    public event EventHandler? SelectionChanged;

    public string DisplayName => "TextBox / RichTextBox Scenarios";

    public IReadOnlyList<Control> GetSelectedControls() => _selection.GetSelectedControls();

    public void SelectAll() => _selection.SelectAll();

    public void ClearSelection() => _selection.ClearSelection();

    public void SetSelectionMargin(int gap) => _selection.SetSelectionGap(gap);
}
