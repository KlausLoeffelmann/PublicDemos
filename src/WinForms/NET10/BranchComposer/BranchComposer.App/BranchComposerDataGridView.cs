namespace BranchComposer.App;

public abstract class BranchComposerDataGridView : DataGridView
{
    private const int MinimumMeasuredColumnWidth = 64;
    private const int ClientWidthPadding = 2;

    private bool _isApplyingColumnWidths;

    protected BranchComposerDataGridView()
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
        EnableHeadersVisualStyles = false;
        MultiSelect = false;
        RowHeadersVisible = false;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        ShowEditingIcon = false;
        ApplyPalette();
    }

    protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
    {
        base.OnColumnAdded(e);
        e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        ApplyColumnWidths();
    }

    protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
    {
        base.OnColumnRemoved(e);
        ApplyColumnWidths();
    }

    protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
    {
        base.OnRowsAdded(e);
        ApplyColumnWidths();
    }

    protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
    {
        base.OnRowsRemoved(e);
        ApplyColumnWidths();
    }

    protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
    {
        base.OnCellValueChanged(e);
        ApplyColumnWidths();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        ApplyColumnWidths();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        ApplyColumnWidths();
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        ApplyPalette();
    }

    protected void ApplyColumnWidths()
    {
        if (_isApplyingColumnWidths || Columns.Count == 0 || ClientSize.Width <= 0)
        {
            return;
        }

        DataGridViewColumn[] visibleColumns = Columns
            .Cast<DataGridViewColumn>()
            .Where(column => column.Visible)
            .OrderBy(column => column.DisplayIndex)
            .ToArray();

        if (visibleColumns.Length == 0)
        {
            return;
        }

        try
        {
            _isApplyingColumnWidths = true;

            int[] preferredWidths = visibleColumns
                .Select(GetPreferredWidth)
                .ToArray();

            int totalPreferredWidth = preferredWidths.Sum();
            int availableWidth = GetAvailableColumnWidth();
            if (totalPreferredWidth < availableWidth)
            {
                int extraWidth = availableWidth - totalPreferredWidth;
                int extraWidthPerColumn = extraWidth / visibleColumns.Length;
                int remainder = extraWidth % visibleColumns.Length;

                for (int index = 0; index < preferredWidths.Length; index++)
                {
                    preferredWidths[index] += extraWidthPerColumn;
                    if (index < remainder)
                    {
                        preferredWidths[index]++;
                    }
                }
            }

            for (int index = 0; index < visibleColumns.Length; index++)
            {
                DataGridViewColumn column = visibleColumns[index];
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = preferredWidths[index];
            }
        }
        finally
        {
            _isApplyingColumnWidths = false;
        }
    }

    private int GetPreferredWidth(DataGridViewColumn column)
    {
        int preferredWidth = column.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, fixedHeight: true);
        return Math.Max(Math.Max(preferredWidth, column.MinimumWidth), MinimumMeasuredColumnWidth);
    }

    private int GetAvailableColumnWidth()
    {
        int availableWidth = ClientSize.Width - ClientWidthPadding;

        if (RowHeadersVisible)
        {
            availableWidth -= RowHeadersWidth;
        }

        if (Rows.Count > 0 && IsHandleCreated && DisplayedRowCount(includePartialRow: false) < Rows.Count)
        {
            availableWidth -= SystemInformation.VerticalScrollBarWidth;
        }

        return Math.Max(0, availableWidth);
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
