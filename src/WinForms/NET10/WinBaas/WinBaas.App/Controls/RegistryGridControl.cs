using WinBaas.Models;

namespace WinBaas.Controls;

/// <summary>
///  The curated registry-value detail view.
/// </summary>
public sealed partial class RegistryGridControl : UserControl
{
    private IReadOnlyList<RegistryDiscoveredItem> _items = [];
    private bool _syncing;

    public RegistryGridControl()
    {
        InitializeComponent();

        // Keep registry rows uniform and single-line. Some curated values (long
        // REG_SZ paths, timestamps, multi-string joins) are wide; with the grid's
        // default wrapping these can otherwise blow up a single row's height.
        _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

        _grid.CellValueChanged += Grid_CellValueChanged;
        _grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
        _grid.SelectionChanged += Grid_SelectionChanged;
        _grid.RowEnter += Grid_RowEnter;
    }

    /// <summary>Raised after the checked state of one or more registry items changed.</summary>
    public event EventHandler? CheckedItemsChanged;

    /// <summary>Raised when a row selection should update the status strip text.</summary>
    public event EventHandler<string>? StatusTextChanged;

    /// <summary>The currently displayed registry items.</summary>
    public IReadOnlyList<RegistryDiscoveredItem> Items => _items;

    /// <summary>Replaces the displayed registry items.</summary>
    public void SetItems(IEnumerable<RegistryDiscoveredItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items = items.ToList();
        _grid.Rows.Clear();

        foreach (RegistryDiscoveredItem item in _items)
        {
            int rowIndex = _grid.Rows.Add(
                item.IsChecked,
                item.Name,
                item.RegistryPath,
                AbbreviateDescription(item.ShortDescription));

            DataGridViewRow row = _grid.Rows[rowIndex];
            row.Tag = item;
            row.Cells[_colValue.Index].ToolTipText = item.ValueText;
            if (!item.CanSelect)
            {
                row.ReadOnly = true;
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
                row.DefaultCellStyle.SelectionForeColor = SystemColors.GrayText;
            }
        }

        UpdateStatusTextFromSelection();
    }

    /// <summary>Applies the same checked state to every selectable registry item.</summary>
    public void SetAllChecked(bool value)
    {
        _syncing = true;
        try
        {
            foreach (RegistryDiscoveredItem item in _items.Where(item => item.CanSelect))
            {
                item.IsChecked = value;
            }

            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.Tag is RegistryDiscoveredItem item && item.CanSelect)
                {
                    row.Cells[_colCheck.Index].Value = value;
                }
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

        if (_grid.Rows[e.RowIndex].Tag is RegistryDiscoveredItem item && item.CanSelect)
        {
            item.IsChecked = (bool)(_grid.Rows[e.RowIndex].Cells[_colCheck.Index].Value ?? false);
            CheckedItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e) => UpdateStatusTextFromSelection();

    private void Grid_RowEnter(object? sender, DataGridViewCellEventArgs e) => UpdateStatusTextFromSelection();

    private void UpdateStatusTextFromSelection()
    {
        string text = _grid.SelectedRows.Count == 0
            ? string.Empty
            : (_grid.SelectedRows[0].Tag as RegistryDiscoveredItem)?.FullDescription ?? string.Empty;
        StatusTextChanged?.Invoke(this, text);
    }

    private static string AbbreviateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return string.Empty;
        }

        int sentenceEnd = description.IndexOf('.');
        if (sentenceEnd >= 0 && sentenceEnd < 80)
        {
            return description[..(sentenceEnd + 1)];
        }

        return description.Length <= 80
            ? description
            : description[..80].TrimEnd() + "\u2026";
    }
}
