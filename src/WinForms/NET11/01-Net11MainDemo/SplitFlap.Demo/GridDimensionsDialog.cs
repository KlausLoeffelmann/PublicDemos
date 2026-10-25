namespace SplitFlap.Demo;

/// <summary>
///  Collects validated split-flap row and column counts.
/// </summary>
internal partial class GridDimensionsDialog : Form
{
    /// <summary>
    ///  Initializes the dialog with the current grid dimensions.
    /// </summary>
    public GridDimensionsDialog(int rows, int columns)
    {
        InitializeComponent();
        _rowsNumericUpDown.Value = Math.Clamp(rows, 1, 64);
        _columnsNumericUpDown.Value = Math.Clamp(columns, 1, 256);
    }

    /// <summary>
    ///  Gets the selected row count.
    /// </summary>
    public int Rows
        => Decimal.ToInt32(_rowsNumericUpDown.Value);

    /// <summary>
    ///  Gets the selected column count.
    /// </summary>
    public int Columns
        => Decimal.ToInt32(_columnsNumericUpDown.Value);
}
