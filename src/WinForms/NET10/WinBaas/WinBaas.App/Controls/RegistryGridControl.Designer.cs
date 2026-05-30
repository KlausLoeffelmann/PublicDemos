namespace WinBaas.Controls;

partial class RegistryGridControl
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
        _colValue = new DataGridViewTextBoxColumn();
        _colPath = new DataGridViewTextBoxColumn();
        _colDescription = new DataGridViewTextBoxColumn();
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
        _grid.Columns.AddRange(new DataGridViewColumn[] { _colCheck, _colValue, _colPath, _colDescription });
        _grid.Dock = DockStyle.Fill;
        _grid.Font = new Font("Segoe UI", 11F);
        _grid.Location = new Point(0, 0);
        _grid.MultiSelect = false;
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
        // _colValue
        // 
        _colValue.FillWeight = 18F;
        _colValue.HeaderText = "Value";
        _colValue.Name = "_colValue";
        _colValue.ReadOnly = true;
        // 
        // _colPath
        // 
        _colPath.FillWeight = 34F;
        _colPath.HeaderText = "Registry path";
        _colPath.Name = "_colPath";
        _colPath.ReadOnly = true;
        // 
        // _colDescription
        // 
        _colDescription.FillWeight = 44F;
        _colDescription.HeaderText = "Description";
        _colDescription.Name = "_colDescription";
        _colDescription.ReadOnly = true;
        // 
        // RegistryGridControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_grid);
        Name = "RegistryGridControl";
        Size = new Size(916, 413);
        ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView _grid;
    private DataGridViewCheckBoxColumn _colCheck;
    private DataGridViewTextBoxColumn _colValue;
    private DataGridViewTextBoxColumn _colPath;
    private DataGridViewTextBoxColumn _colDescription;
}
