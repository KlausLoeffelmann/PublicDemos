namespace WinBaas.Controls;

partial class VsHivesControl
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
        _table = new TableLayoutPanel();
        _headerHive = new Label();
        _headerPath = new Label();
        _headerAction = new Label();
        _table.SuspendLayout();
        SuspendLayout();
        // 
        // _table
        // 
        _table.AutoScroll = true;
        _table.ColumnCount = 3;
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _table.Controls.Add(_headerHive, 0, 0);
        _table.Controls.Add(_headerPath, 1, 0);
        _table.Controls.Add(_headerAction, 2, 0);
        _table.Dock = DockStyle.Fill;
        _table.Location = new Point(0, 0);
        _table.Name = "_table";
        _table.RowCount = 1;
        _table.RowStyles.Add(new RowStyle());
        _table.Size = new Size(916, 413);
        _table.TabIndex = 0;
        // 
        // _headerHive
        // 
        _headerHive.Anchor = AnchorStyles.Left;
        _headerHive.AutoSize = true;
        _headerHive.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _headerHive.Location = new Point(3, 0);
        _headerHive.Name = "_headerHive";
        _headerHive.Size = new Size(42, 20);
        _headerHive.TabIndex = 0;
        _headerHive.Text = "Hive";
        // 
        // _headerPath
        // 
        _headerPath.Anchor = AnchorStyles.Left;
        _headerPath.AutoSize = true;
        _headerPath.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _headerPath.Location = new Point(51, 0);
        _headerPath.Name = "_headerPath";
        _headerPath.Size = new Size(40, 20);
        _headerPath.TabIndex = 1;
        _headerPath.Text = "Path";
        // 
        // _headerAction
        // 
        _headerAction.Anchor = AnchorStyles.Left;
        _headerAction.AutoSize = true;
        _headerAction.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _headerAction.Location = new Point(875, 0);
        _headerAction.Name = "_headerAction";
        _headerAction.Size = new Size(38, 20);
        _headerAction.TabIndex = 2;
        _headerAction.Text = "Copy";
        // 
        // VsHivesControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_table);
        Name = "VsHivesControl";
        Size = new Size(916, 413);
        _table.ResumeLayout(false);
        _table.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _table;
    private Label _headerHive;
    private Label _headerPath;
    private Label _headerAction;
}
