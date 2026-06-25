namespace LargeFormSmokeTest.Controls;

using LargeFormSmokeTest.Theming;

/// <summary>
///  A <see cref="DataGridView"/> that is double-buffered (flicker-free for the perf harness),
///  uses subtle alternating row colors and can switch between the Classic and Dark color
///  schemes provided by a <see cref="ThemeManager"/>. Reused across the app for every tabular
///  list (payers, declarations, …).
/// </summary>
public class ThemedDataGridView : DataGridView
{
    /// <summary>Initializes a new, double-buffered themed grid with sensible defaults.</summary>
    public ThemedDataGridView()
    {
        // Double buffering removes the flicker that a dense grid would otherwise show while
        // scrolling — important when this control is part of the form under test.
        DoubleBuffered = true;

        BorderStyle = BorderStyle.None;
        BackgroundColor = SystemColors.Window;
        // Let the header row grow to fit the (DPI-scaled) header font instead of clipping it,
        // and give it a comfortable minimum height. Data rows get a little extra height too.
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        ColumnHeadersHeight = 40;
        ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 6, 6, 6);
        ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        RowTemplate.Height = 28;
        RowHeadersVisible = false;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        ReadOnly = true;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        GridColor = Color.FromArgb(221, 221, 226);
    }

    /// <summary>
    ///  Applies the color scheme of the supplied <paramref name="theme"/> to header, body,
    ///  alternating rows, selection and grid lines.
    /// </summary>
    public void ApplyScheme(ThemeManager theme)
    {
        BackgroundColor = theme.GridBackColor;
        GridColor = theme.GridLineColor;

        DefaultCellStyle.BackColor = theme.GridBackColor;
        DefaultCellStyle.ForeColor = theme.GridForeColor;
        DefaultCellStyle.SelectionBackColor = theme.GridSelectionBackColor;
        DefaultCellStyle.SelectionForeColor = Color.White;

        AlternatingRowsDefaultCellStyle.BackColor = theme.GridAlternatingBackColor;
        AlternatingRowsDefaultCellStyle.ForeColor = theme.GridForeColor;
        AlternatingRowsDefaultCellStyle.SelectionBackColor = theme.GridSelectionBackColor;
        AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

        ColumnHeadersDefaultCellStyle.BackColor = theme.GridHeaderBackColor;
        ColumnHeadersDefaultCellStyle.ForeColor = theme.GridHeaderForeColor;
        ColumnHeadersDefaultCellStyle.SelectionBackColor = theme.GridHeaderBackColor;
        ColumnHeadersDefaultCellStyle.SelectionForeColor = theme.GridHeaderForeColor;

        Invalidate();
    }
}
