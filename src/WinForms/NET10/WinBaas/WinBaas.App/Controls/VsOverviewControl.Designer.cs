namespace WinBaas.Controls;

partial class VsOverviewControl
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
        _colSku = new DataGridViewTextBoxColumn();
        _colInstallDate = new DataGridViewTextBoxColumn();
        _colVersion = new DataGridViewTextBoxColumn();
        _colSettingsPath = new DataGridViewTextBoxColumn();
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
        _grid.Columns.AddRange(new DataGridViewColumn[] { _colSku, _colInstallDate, _colVersion, _colSettingsPath });
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
        // _colSku
        // 
        _colSku.FillWeight = 26F;
        _colSku.HeaderText = "SKU";
        _colSku.Name = "_colSku";
        _colSku.ReadOnly = true;
        // 
        // _colInstallDate
        // 
        _colInstallDate.FillWeight = 14F;
        _colInstallDate.HeaderText = "Install date";
        _colInstallDate.Name = "_colInstallDate";
        _colInstallDate.ReadOnly = true;
        // 
        // _colVersion
        // 
        _colVersion.FillWeight = 16F;
        _colVersion.HeaderText = "Version";
        _colVersion.Name = "_colVersion";
        _colVersion.ReadOnly = true;
        // 
        // _colSettingsPath
        // 
        _colSettingsPath.FillWeight = 44F;
        _colSettingsPath.HeaderText = "Settings-file path";
        _colSettingsPath.Name = "_colSettingsPath";
        _colSettingsPath.ReadOnly = true;
        // 
        // VsOverviewControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_grid);
        Name = "VsOverviewControl";
        Size = new Size(916, 413);
        ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView _grid;
    private DataGridViewTextBoxColumn _colSku;
    private DataGridViewTextBoxColumn _colInstallDate;
    private DataGridViewTextBoxColumn _colVersion;
    private DataGridViewTextBoxColumn _colSettingsPath;
}
