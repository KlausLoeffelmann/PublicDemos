using System.Globalization;
using WinBaas.Models;

namespace WinBaas.Controls;

/// <summary>
///  The default file/folder/database detail view shown for regular catalog entries.
/// </summary>
public sealed partial class FilesGridControl : UserControl
{
    private IReadOnlyList<DiscoveredItem> _items = [];
    private bool _syncing;

    public FilesGridControl()
    {
        InitializeComponent();
        _colSize.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        _grid.CellValueChanged += Grid_CellValueChanged;
        _grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
        _grid.SelectionChanged += Grid_SelectionChanged;
    }

    /// <summary>Raised after a checkbox edit changed one or more item selections.</summary>
    public event EventHandler? CheckedItemsChanged;

    /// <summary>Raised when the selected rows imply a new size summary.</summary>
    public event EventHandler<string>? SelectionSizeChanged;

    /// <summary>The currently displayed items.</summary>
    public IReadOnlyList<DiscoveredItem> Items => _items;

    /// <summary>Replaces the displayed items.</summary>
    public void SetItems(IEnumerable<DiscoveredItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items = items.ToList();
        _grid.Rows.Clear();

        foreach (DiscoveredItem item in _items)
        {
            int rowIndex = _grid.Rows.Add(
                item.IsChecked,
                item.Name,
                item.FileTypeLabel,
                item.FullPath,
                item.LastChanged?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? string.Empty,
                item.Created?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                item.SizeBytes is null ? "\u2026" : FormatSizeShort(item.SizeBytes.Value));
            _grid.Rows[rowIndex].Tag = item;
        }

        RaiseSelectionSummary();
    }

    /// <summary>Applies the same checked state to all displayed items.</summary>
    public void SetAllChecked(bool value)
    {
        _syncing = true;
        try
        {
            foreach (DiscoveredItem item in _items)
            {
                item.IsChecked = value;
            }

            foreach (DataGridViewRow row in _grid.Rows)
            {
                row.Cells[_colCheck.Index].Value = value;
            }
        }
        finally
        {
            _syncing = false;
        }

        CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Applies the current dark/light styling to the grid.</summary>
    public void ApplyColorMode(bool dark)
    {
        _grid.EnableHeadersVisualStyles = false;
        _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = dark
            ? Color.FromArgb(0x2D, 0x2D, 0x30)
            : SystemColors.Control;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = dark
            ? Color.Gainsboro
            : SystemColors.ControlText;
        _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = _grid.ColumnHeadersDefaultCellStyle.BackColor;
        _grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = _grid.ColumnHeadersDefaultCellStyle.ForeColor;
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font(_grid.Font, FontStyle.Regular);

        _grid.BackgroundColor = dark ? Color.FromArgb(0x1E, 0x1E, 0x1E) : SystemColors.Window;
        _grid.GridColor = dark ? Color.FromArgb(0x3F, 0x3F, 0x46) : SystemColors.ControlDark;
        _grid.DefaultCellStyle.BackColor = dark ? Color.FromArgb(0x25, 0x25, 0x26) : SystemColors.Window;
        _grid.DefaultCellStyle.ForeColor = dark ? Color.Gainsboro : SystemColors.WindowText;
        _grid.DefaultCellStyle.SelectionBackColor = dark
            ? Color.FromArgb(0x37, 0x37, 0x3D)
            : SystemColors.Highlight;
        _grid.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
    }

    private void Grid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_grid.IsCurrentCellDirty && _grid.CurrentCell is DataGridViewCheckBoxCell)
        {
            _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_syncing || e.RowIndex < 0 || e.ColumnIndex != _colCheck.Index)
        {
            return;
        }

        if (_grid.Rows[e.RowIndex].Tag is DiscoveredItem item)
        {
            item.IsChecked = (bool)(_grid.Rows[e.RowIndex].Cells[_colCheck.Index].Value ?? false);
            CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e) => RaiseSelectionSummary();

    private void RaiseSelectionSummary()
    {
        if (_grid.SelectedRows.Count == 0)
        {
            SelectionSizeChanged?.Invoke(this, string.Empty);
            return;
        }

        long total = _grid.SelectedRows
            .Cast<DataGridViewRow>()
            .Select(row => row.Tag as DiscoveredItem)
            .Where(item => item?.SizeBytes is not null)
            .Sum(item => item!.SizeBytes!.Value);

        SelectionSizeChanged?.Invoke(this, FormatSize(total));
    }

    private static string FormatSize(long bytes)
    {
        string iec = FormatSizeShort(bytes);
        string raw = bytes.ToString("###,###,###,###,###,##0", CultureInfo.InvariantCulture);
        return $"{iec} ({raw} bytes)";
    }

    private static string FormatSizeShort(long bytes)
    {
        string[] units = ["bytes", "KiB", "MiB", "GiB", "TiB", "PiB"];
        double value = bytes;
        int unit = 0;
        while (value >= 1024d && unit < units.Length - 1)
        {
            value /= 1024d;
            unit++;
        }

        return unit == 0
            ? $"{bytes:N0} {units[0]}"
            : string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1}", value, units[unit]);
    }
}
