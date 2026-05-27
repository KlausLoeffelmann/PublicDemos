namespace BranchComposer.App;

public sealed class BranchSelectionDataGridView : DataGridView
{
    public BranchSelectionDataGridView()
    {
        DoubleBuffered = true;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        BackgroundColor = SystemColors.Window;
        BorderStyle = BorderStyle.None;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        EditMode = DataGridViewEditMode.EditOnEnter;
        EnableHeadersVisualStyles = false;
        MultiSelect = false;
        RowHeadersVisible = false;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        ShowEditingIcon = false;
        ApplyPalette();
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        ApplyPalette();
    }

    protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
    {
        base.OnColumnAdded(e);
        e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
    }

    private void ApplyPalette()
    {
        if (Application.IsDarkModeEnabled)
        {
            BackgroundColor = Color.FromArgb(30, 30, 34);
            GridColor = Color.FromArgb(70, 74, 82);
            DefaultCellStyle.BackColor = Color.FromArgb(36, 39, 45);
            DefaultCellStyle.ForeColor = Color.FromArgb(238, 238, 238);
            DefaultCellStyle.SelectionBackColor = Color.FromArgb(72, 91, 128);
            DefaultCellStyle.SelectionForeColor = Color.White;
            AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(42, 45, 52);
            ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 54, 63);
            ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(58, 66, 80);
            ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }
        else
        {
            BackgroundColor = Color.FromArgb(248, 250, 252);
            GridColor = Color.FromArgb(221, 226, 233);
            DefaultCellStyle.BackColor = Color.White;
            DefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);
            DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 229, 246);
            DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 251);
            ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(236, 241, 247);
            ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);
            ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 231, 244);
            ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
        }

        DefaultCellStyle.Padding = new Padding(6, 3, 6, 3);
        ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
    }
}
