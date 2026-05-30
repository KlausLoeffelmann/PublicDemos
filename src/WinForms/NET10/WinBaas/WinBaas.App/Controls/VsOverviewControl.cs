using System.Globalization;
using WinBaas.Models;

namespace WinBaas.Controls;

/// <summary>
///  The flat overview grid for installed Visual Studio SKUs.
/// </summary>
public sealed partial class VsOverviewControl : UserControl
{
    public VsOverviewControl()
    {
        InitializeComponent();
    }

    /// <summary>Replaces the displayed Visual Studio SKUs.</summary>
    public void SetItems(IEnumerable<VsSku> skus)
    {
        ArgumentNullException.ThrowIfNull(skus);

        _grid.Rows.Clear();
        foreach (VsSku sku in skus)
        {
            _grid.Rows.Add(
                sku.NodeLabel,
                sku.InstallDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                sku.Version,
                sku.SettingsPath);
        }
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
}
