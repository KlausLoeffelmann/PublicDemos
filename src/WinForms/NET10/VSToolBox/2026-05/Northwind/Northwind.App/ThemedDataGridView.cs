using Microsoft.Win32;

namespace Northwind.App;

/// <summary>
/// A double-buffered, dark-mode-aware DataGridView that automatically
/// applies an appropriate color palette based on the current system theme.
/// </summary>
internal class ThemedDataGridView : DataGridView
{
    public ThemedDataGridView()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        ApplyTheme();
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        if (Application.IsDarkModeEnabled)
        {
            ApplyDarkPalette();
        }
        else
        {
            ApplyLightPalette();
        }
    }

    private void ApplyDarkPalette()
    {
        EnableHeadersVisualStyles = false;

        var bgColor = Color.FromArgb(32, 32, 32);
        var cellBg = Color.FromArgb(45, 45, 48);
        var cellFg = Color.WhiteSmoke;
        var altRowBg = Color.FromArgb(55, 55, 58);
        var headerBg = Color.FromArgb(28, 28, 28);
        var headerFg = Color.Gainsboro;
        var gridLine = Color.FromArgb(70, 70, 70);
        var selBg = Color.FromArgb(0, 120, 215);
        var selFg = Color.White;

        BackgroundColor = bgColor;
        GridColor = gridLine;

        DefaultCellStyle.BackColor = cellBg;
        DefaultCellStyle.ForeColor = cellFg;
        DefaultCellStyle.SelectionBackColor = selBg;
        DefaultCellStyle.SelectionForeColor = selFg;

        RowsDefaultCellStyle.BackColor = cellBg;
        RowsDefaultCellStyle.ForeColor = cellFg;
        RowsDefaultCellStyle.SelectionBackColor = selBg;
        RowsDefaultCellStyle.SelectionForeColor = selFg;

        AlternatingRowsDefaultCellStyle.BackColor = altRowBg;
        AlternatingRowsDefaultCellStyle.ForeColor = cellFg;
        AlternatingRowsDefaultCellStyle.SelectionBackColor = selBg;
        AlternatingRowsDefaultCellStyle.SelectionForeColor = selFg;

        ColumnHeadersDefaultCellStyle.BackColor = headerBg;
        ColumnHeadersDefaultCellStyle.ForeColor = headerFg;
        ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBg;
        ColumnHeadersDefaultCellStyle.SelectionForeColor = headerFg;

        RowHeadersDefaultCellStyle.BackColor = headerBg;
        RowHeadersDefaultCellStyle.ForeColor = headerFg;
        RowHeadersDefaultCellStyle.SelectionBackColor = headerBg;
        RowHeadersDefaultCellStyle.SelectionForeColor = headerFg;
    }

    private void ApplyLightPalette()
    {
        // Reset to system defaults for light mode
        EnableHeadersVisualStyles = true;

        BackgroundColor = SystemColors.Window;
        GridColor = SystemColors.ControlDark;

        DefaultCellStyle = new DataGridViewCellStyle();
        RowsDefaultCellStyle = new DataGridViewCellStyle();
        AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle();
        ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle();
        RowHeadersDefaultCellStyle = new DataGridViewCellStyle();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        }

        base.Dispose(disposing);
    }
}
