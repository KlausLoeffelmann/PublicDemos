namespace WinBaas.Controls;

partial class FilesGridControl
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
        _colCheck = new DataGridViewCheckBoxColumn();
        _colName = new DataGridViewTextBoxColumn();
        _colType = new DataGridViewTextBoxColumn();
        _colPath = new DataGridViewTextBoxColumn();
        _colChanged = new DataGridViewTextBoxColumn();
        _colCreated = new DataGridViewTextBoxColumn();
        _colSize = new DataGridViewTextBoxColumn();
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
        _grid.Columns.AddRange(new DataGridViewColumn[] { _colCheck, _colName, _colType, _colPath, _colChanged, _colCreated, _colSize });
        _grid.Dock = DockStyle.Fill;
        _grid.Font = new Font("Segoe UI", 11F);
        _grid.Location = new Point(0, 0);
        _grid.MultiSelect = true;
        _grid.Name = "_grid";
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Size = new Size(916, 413);
        _grid.TabIndex = 0;
        // 
        // _colCheck
        // 
        _colCheck.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        _colCheck.FillWeight = 4F;
        _colCheck.HeaderText = "";
        _colCheck.Name = "_colCheck";
        _colCheck.Resizable = DataGridViewTriState.False;
        _colCheck.Width = 32;
        // 
        // _colName
        // 
        _colName.FillWeight = 24F;
        _colName.HeaderText = "Filename";
        _colName.Name = "_colName";
        _colName.ReadOnly = true;
        // 
        // _colType
        // 
        _colType.FillWeight = 18F;
        _colType.HeaderText = "File type";
        _colType.Name = "_colType";
        _colType.ReadOnly = true;
        // 
        // _colPath
        // 
        _colPath.FillWeight = 30F;
        _colPath.HeaderText = "Path";
        _colPath.Name = "_colPath";
        _colPath.ReadOnly = true;
        // 
        // _colChanged
        // 
        _colChanged.FillWeight = 12F;
        _colChanged.HeaderText = "Changed";
        _colChanged.Name = "_colChanged";
        _colChanged.ReadOnly = true;
        // 
        // _colCreated
        // 
        _colCreated.FillWeight = 8F;
        _colCreated.HeaderText = "Created";
        _colCreated.Name = "_colCreated";
        _colCreated.ReadOnly = true;
        // 
        // _colSize
        // 
        _colSize.FillWeight = 8F;
        _colSize.HeaderText = "Size";
        _colSize.Name = "_colSize";
        _colSize.ReadOnly = true;
        // 
        // FilesGridControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_grid);
        Name = "FilesGridControl";
        Size = new Size(916, 413);
        ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView _grid;
    private DataGridViewCheckBoxColumn _colCheck;
    private DataGridViewTextBoxColumn _colName;
    private DataGridViewTextBoxColumn _colType;
    private DataGridViewTextBoxColumn _colPath;
    private DataGridViewTextBoxColumn _colChanged;
    private DataGridViewTextBoxColumn _colCreated;
    private DataGridViewTextBoxColumn _colSize;
}
