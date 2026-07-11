// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Visual matrix for <see cref="CheckBox"/> and <see cref="RadioButton"/> rendering. It crosses the
///  new .NET 11 per-control <see cref="Control.VisualStylesMode"/> (Net11 vs Classic) against every
///  <see cref="Appearance"/> mode (Normal, ToggleSwitch, and Button) - and, for the Button appearance,
///  against all four <see cref="FlatStyle"/> variants.
/// </summary>
/// <remarks>
///  <para>
///   The four rows are: CheckBox·Net11, CheckBox·Classic, RadioButton·Net11, RadioButton·Classic.
///   The six columns are: Normal, ToggleSwitch, Button+Standard, Button+Popup, Button+Flat,
///   Button+System. That yields 24 demo controls, each wrapped in a <see cref="SelectablePanel"/> so
///   they can be double-clicked into the shared <see cref="PropertyGrid"/> (see
///   <see cref="SelectionController"/>).
///  </para>
///  <para>
///   Because the matrix is so regular, the 24 cells (plus the row/column header labels) are built in
///   <see cref="BuildMatrix"/> via nested loops rather than 24 near-identical Designer blocks - this
///   keeps the intent obvious and avoids copy/paste drift. The Designer file owns only the grid
///   structure and the intro label.
///  </para>
/// </remarks>
public partial class CheckBoxRadioButtonVisualStylesView : UserControl, IScenarioView
{
    private readonly SelectionController _selection = new();

    public CheckBoxRadioButtonVisualStylesView()
    {
        InitializeComponent();

        // Bubble the controller's selection changes up as the view's own IScenarioView event.
        _selection.SelectionChanged += (_, _) => SelectionChanged?.Invoke(this, EventArgs.Empty);

        BuildMatrix();
    }

    public event EventHandler? SelectionChanged;

    public string DisplayName => "CheckBox / RadioButton Visual Styles";

    public IReadOnlyList<Control> GetSelectedControls() => _selection.GetSelectedControls();

    public void SelectAll() => _selection.SelectAll();

    public void ClearSelection() => _selection.ClearSelection();

    public void SetSelectionMargin(int gap) => _selection.SetSelectionGap(gap);

    /// <summary>
    ///  Populates the 7x5 matrix grid: row 0 / column 0 hold non-selectable header labels, and the
    ///  inner 6x4 cells each host one CheckBox/RadioButton (wrapped in a <see cref="SelectablePanel"/>).
    /// </summary>
    private void BuildMatrix()
    {
        // Rows differ by control kind and VisualStylesMode. Factory delegates keep the CheckBox vs
        // RadioButton choice in one spot; everything else (Appearance/FlatStyle) comes from the column.
        (Func<ButtonBase> Factory, VisualStylesMode Mode, string Header, string NamePrefix)[] rows =
        [
            (static () => new CheckBox(), VisualStylesMode.Net11, "CheckBox \u00b7 Net11", "checkNet11"),
            (static () => new CheckBox(), VisualStylesMode.Classic, "CheckBox \u00b7 Classic", "checkClassic"),
            (static () => new RadioButton(), VisualStylesMode.Net11, "RadioButton \u00b7 Net11", "radioNet11"),
            (static () => new RadioButton(), VisualStylesMode.Classic, "RadioButton \u00b7 Classic", "radioClassic"),
        ];

        // Columns differ by Appearance and (for the Button appearance) FlatStyle. The Normal and
        // ToggleSwitch columns ignore FlatStyle, so their FlatStyle value is irrelevant.
        (string Header, string Caption, Appearance Appearance, FlatStyle FlatStyle, string NameToken)[] columns =
        [
            ("Normal", "Normal", Appearance.Normal, FlatStyle.Standard, "Normal"),
            ("ToggleSwitch", "Toggle", Appearance.ToggleSwitch, FlatStyle.Standard, "Toggle"),
            ("Button \u00b7 Standard", "Standard", Appearance.Button, FlatStyle.Standard, "Standard"),
            ("Button \u00b7 Popup", "Popup", Appearance.Button, FlatStyle.Popup, "Popup"),
            ("Button \u00b7 Flat", "Flat", Appearance.Button, FlatStyle.Flat, "Flat"),
            ("Button \u00b7 System", "System", Appearance.Button, FlatStyle.System, "System"),
        ];

        _matrixTableLayoutPanel.SuspendLayout();

        // Corner + column headers (row 0) and row headers (column 0).
        _matrixTableLayoutPanel.Controls.Add(CreateHeaderLabel("Control / Style", bold: true), 0, 0);
        for (int column = 0; column < columns.Length; column++)
        {
            _matrixTableLayoutPanel.Controls.Add(CreateHeaderLabel(columns[column].Header, bold: true), column + 1, 0);
        }

        List<SelectablePanel> panels = new(rows.Length * columns.Length);
        for (int row = 0; row < rows.Length; row++)
        {
            _matrixTableLayoutPanel.Controls.Add(CreateHeaderLabel(rows[row].Header, bold: false), 0, row + 1);

            for (int column = 0; column < columns.Length; column++)
            {
                ButtonBase control = rows[row].Factory();
                control.Name = $"_{rows[row].NamePrefix}{columns[column].NameToken}";
                control.AutoSize = true;
                control.Text = columns[column].Caption;
                control.FlatStyle = columns[column].FlatStyle;
                control.VisualStylesMode = rows[row].Mode;

                // Appearance and Checked live on the concrete types, not on ButtonBase.
                switch (control)
                {
                    case CheckBox checkBox:
                        checkBox.Appearance = columns[column].Appearance;
                        checkBox.Checked = true;
                        break;
                    case RadioButton radioButton:
                        radioButton.Appearance = columns[column].Appearance;
                        radioButton.Checked = true;
                        break;
                }

                panels.Add(AddCell(control, column + 1, row + 1));
            }
        }

        // Register all 24 panels at once so Shift-range selection can see the whole grid.
        _selection.Register([.. panels]);

        _matrixTableLayoutPanel.ResumeLayout(true);
    }

    /// <summary>
    ///  Wraps <paramref name="control"/> in a <see cref="SelectablePanel"/> and drops it into the given
    ///  matrix cell. The control sits at the panel's padding offset so the selection frame surrounds it.
    /// </summary>
    private SelectablePanel AddCell(Control control, int columnIndex, int rowIndex)
    {
        SelectablePanel panel = new()
        {
            Name = $"{control.Name}Panel",
            Margin = new Padding(4),
        };

        control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        panel.Controls.Add(control);
        control.Location = new Point(panel.Padding.Left, panel.Padding.Top);

        _matrixTableLayoutPanel.Controls.Add(panel, columnIndex, rowIndex);
        return panel;
    }

    private static Label CreateHeaderLabel(string text, bool bold) => new()
    {
        AutoSize = true,
        Anchor = AnchorStyles.Left,
        Margin = new Padding(6, 3, 12, 3),
        Text = text,
        Font = bold ? new Font("Segoe UI", 11F, FontStyle.Bold) : new Font("Segoe UI", 11F, FontStyle.Regular),
    };
}
