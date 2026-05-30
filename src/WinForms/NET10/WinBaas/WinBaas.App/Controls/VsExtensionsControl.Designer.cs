namespace WinBaas.Controls;

partial class VsExtensionsControl
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _grid = new DataGridView();
        _colName = new DataGridViewTextBoxColumn();
        _colPublisher = new DataGridViewTextBoxColumn();
        _colVersion = new DataGridViewTextBoxColumn();
        _colPath = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
        SuspendLayout();
        // 
        // _grid
        // 
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = SystemColors.Window;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _grid.Columns.AddRange(new DataGridViewColumn[] { _colName, _colPublisher, _colVersion, _colPath });
        _grid.Dock = DockStyle.Fill;
        _grid.Font = new Font("Segoe UI", 11F);
        _grid.Location = new Point(0, 0);
        _grid.Name = "_grid";
        _grid.ReadOnly = true;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Size = new Size(916, 413);
        _grid.TabIndex = 0;
        // 
        // _colName
        // 
        _colName.FillWeight = 22F;
        _colName.HeaderText = "Name";
        _colName.Name = "_colName";
        _colName.ReadOnly = true;
        // 
        // _colPublisher
        // 
        _colPublisher.FillWeight = 18F;
        _colPublisher.HeaderText = "Publisher";
        _colPublisher.Name = "_colPublisher";
        _colPublisher.ReadOnly = true;
        // 
        // _colVersion
        // 
        _colVersion.FillWeight = 12F;
        _colVersion.HeaderText = "Version";
        _colVersion.Name = "_colVersion";
        _colVersion.ReadOnly = true;
        // 
        // _colPath
        // 
        _colPath.FillWeight = 48F;
        _colPath.HeaderText = "Path";
        _colPath.Name = "_colPath";
        _colPath.ReadOnly = true;
        // 
        // VsExtensionsControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_grid);
        Name = "VsExtensionsControl";
        Size = new Size(916, 413);
        ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView _grid;
    private DataGridViewTextBoxColumn _colName;
    private DataGridViewTextBoxColumn _colPublisher;
    private DataGridViewTextBoxColumn _colVersion;
    private DataGridViewTextBoxColumn _colPath;
}
