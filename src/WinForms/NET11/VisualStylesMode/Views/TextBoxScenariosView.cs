// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Repro scenarios for TextBox / RichTextBox non-client (border) painting: hovering the mouse
///  over these controls must NOT erase or corrupt the rendered text. Each row has a CheckBox so
///  one or more of the controls below can be pushed into the shared PropertyGrid at once.
/// </summary>
public partial class TextBoxScenariosView : UserControl, IScenarioView
{
    private readonly CheckBox[] _checkBoxes;

    public TextBoxScenariosView()
    {
        InitializeComponent();

        _checkBoxes =
        [
            _textBoxDefaultCheckBox,
            _textBoxFixedSingleCheckBox,
            _textBoxMultilineCheckBox,
            _textBoxNoBorderReadOnlyCheckBox,
            _richTextBoxDefaultCheckBox,
            _richTextBoxFixedSingleCheckBox,
            _richTextBoxNoWordWrapCheckBox,
            _richTextBoxReadOnlyCheckBox,
        ];

        ScenarioSelectionHelper.Bind(_textBoxDefaultCheckBox, _textBoxDefault, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_textBoxFixedSingleCheckBox, _textBoxFixedSingle, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_textBoxMultilineCheckBox, _textBoxMultiline, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_textBoxNoBorderReadOnlyCheckBox, _textBoxNoBorderReadOnly, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_richTextBoxDefaultCheckBox, _richTextBoxDefault, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_richTextBoxFixedSingleCheckBox, _richTextBoxFixedSingle, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_richTextBoxNoWordWrapCheckBox, _richTextBoxNoWordWrap, CheckBox_CheckedChanged);
        ScenarioSelectionHelper.Bind(_richTextBoxReadOnlyCheckBox, _richTextBoxReadOnly, CheckBox_CheckedChanged);
    }

    public event EventHandler? SelectionChanged;

    public string DisplayName => "TextBox / RichTextBox Scenarios";

    public IReadOnlyList<Control> GetSelectedControls() => ScenarioSelectionHelper.GetChecked(_checkBoxes);

    public void SelectAll()
    {
        foreach (CheckBox checkBox in _checkBoxes)
        {
            checkBox.Checked = true;
        }
    }

    public void ClearSelection()
    {
        foreach (CheckBox checkBox in _checkBoxes)
        {
            checkBox.Checked = false;
        }
    }

    private void CheckBox_CheckedChanged(object? sender, EventArgs e) =>
        SelectionChanged?.Invoke(this, EventArgs.Empty);
}
