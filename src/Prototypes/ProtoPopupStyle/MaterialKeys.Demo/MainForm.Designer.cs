namespace MaterialKeys.Demo;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _rootLayout = new System.Windows.Forms.TableLayoutPanel();
        _headerLabel = new System.Windows.Forms.Label();
        _keysPanel = new System.Windows.Forms.TableLayoutPanel();
        _leftPanel = new System.Windows.Forms.TableLayoutPanel();
        _numberPad = new System.Windows.Forms.TableLayoutPanel();
        _key7 = new MaterialKeys.MaterialKeyButton();
        _key8 = new MaterialKeys.MaterialKeyButton();
        _key9 = new MaterialKeys.MaterialKeyButton();
        _key4 = new MaterialKeys.MaterialKeyButton();
        _key5 = new MaterialKeys.MaterialKeyButton();
        _key6 = new MaterialKeys.MaterialKeyButton();
        _key1 = new MaterialKeys.MaterialKeyButton();
        _key2 = new MaterialKeys.MaterialKeyButton();
        _key3 = new MaterialKeys.MaterialKeyButton();
        _key0 = new MaterialKeys.MaterialKeyButton();
        _key00 = new MaterialKeys.MaterialKeyButton();
        _keyDot = new MaterialKeys.MaterialKeyButton();
        _totalKey = new MaterialKeys.MaterialKeyButton();
        _sidePanel = new System.Windows.Forms.TableLayoutPanel();
        _clearKey = new MaterialKeys.MaterialKeyButton();
        _voidKey = new MaterialKeys.MaterialKeyButton();
        _corrKey = new MaterialKeys.MaterialKeyButton();
        _dept1Key = new MaterialKeys.MaterialKeyButton();
        _dept2Key = new MaterialKeys.MaterialKeyButton();
        _dept3Key = new MaterialKeys.MaterialKeyButton();
        _dept4Key = new MaterialKeys.MaterialKeyButton();
        _bottomBar = new System.Windows.Forms.TableLayoutPanel();
        _statusLabel = new System.Windows.Forms.Label();
        _openDialogKey = new MaterialKeys.MaterialKeyButton();
        _rootLayout.SuspendLayout();
        _keysPanel.SuspendLayout();
        _leftPanel.SuspendLayout();
        _numberPad.SuspendLayout();
        _sidePanel.SuspendLayout();
        _bottomBar.SuspendLayout();
        SuspendLayout();
        // 
        // _rootLayout
        // 
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _rootLayout.Controls.Add(_headerLabel, 0, 0);
        _rootLayout.Controls.Add(_keysPanel, 0, 1);
        _rootLayout.Controls.Add(_bottomBar, 0, 2);
        _rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        _rootLayout.Location = new System.Drawing.Point(14, 14);
        _rootLayout.Name = "_rootLayout";
        _rootLayout.RowCount = 3;
        _rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _rootLayout.TabIndex = 0;
        // 
        // _headerLabel
        // 
        _headerLabel.AutoSize = true;
        _headerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
        _headerLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
        _headerLabel.Name = "_headerLabel";
        _headerLabel.TabIndex = 0;
        _headerLabel.Text = "Cash Register — MaterialKeyButton array";
        // 
        // _keysPanel
        // 
        _keysPanel.AutoSize = true;
        _keysPanel.ColumnCount = 2;
        _keysPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _keysPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _keysPanel.Controls.Add(_leftPanel, 0, 0);
        _keysPanel.Controls.Add(_sidePanel, 1, 0);
        _keysPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        _keysPanel.Name = "_keysPanel";
        _keysPanel.RowCount = 1;
        _keysPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _keysPanel.TabIndex = 1;
        // 
        // _leftPanel
        // 
        _leftPanel.AutoSize = true;
        _leftPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        _leftPanel.ColumnCount = 1;
        _leftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _leftPanel.Controls.Add(_numberPad, 0, 0);
        _leftPanel.Controls.Add(_totalKey, 0, 1);
        _leftPanel.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        _leftPanel.Name = "_leftPanel";
        _leftPanel.RowCount = 2;
        _leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _leftPanel.TabIndex = 0;
        // 
        // _numberPad
        // 
        _numberPad.AutoSize = true;
        _numberPad.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        _numberPad.ColumnCount = 3;
        _numberPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.Controls.Add(_key7, 0, 0);
        _numberPad.Controls.Add(_key8, 1, 0);
        _numberPad.Controls.Add(_key9, 2, 0);
        _numberPad.Controls.Add(_key4, 0, 1);
        _numberPad.Controls.Add(_key5, 1, 1);
        _numberPad.Controls.Add(_key6, 2, 1);
        _numberPad.Controls.Add(_key1, 0, 2);
        _numberPad.Controls.Add(_key2, 1, 2);
        _numberPad.Controls.Add(_key3, 2, 2);
        _numberPad.Controls.Add(_key0, 0, 3);
        _numberPad.Controls.Add(_key00, 1, 3);
        _numberPad.Controls.Add(_keyDot, 2, 3);
        _numberPad.Margin = new System.Windows.Forms.Padding(0);
        _numberPad.Name = "_numberPad";
        _numberPad.RowCount = 4;
        _numberPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _numberPad.TabIndex = 0;
        // 
        // _key7
        // 
        _key7.Margin = new System.Windows.Forms.Padding(4);
        _key7.Name = "_key7";
        _key7.Size = new System.Drawing.Size(64, 56);
        _key7.TabIndex = 0;
        _key7.Text = "7";
        // 
        // _key8
        // 
        _key8.Margin = new System.Windows.Forms.Padding(4);
        _key8.Name = "_key8";
        _key8.Size = new System.Drawing.Size(64, 56);
        _key8.TabIndex = 1;
        _key8.Text = "8";
        // 
        // _key9
        // 
        _key9.Margin = new System.Windows.Forms.Padding(4);
        _key9.Name = "_key9";
        _key9.Size = new System.Drawing.Size(64, 56);
        _key9.TabIndex = 2;
        _key9.Text = "9";
        // 
        // _key4
        // 
        _key4.Margin = new System.Windows.Forms.Padding(4);
        _key4.Name = "_key4";
        _key4.Size = new System.Drawing.Size(64, 56);
        _key4.TabIndex = 3;
        _key4.Text = "4";
        // 
        // _key5
        // 
        _key5.Margin = new System.Windows.Forms.Padding(4);
        _key5.Name = "_key5";
        _key5.Size = new System.Drawing.Size(64, 56);
        _key5.TabIndex = 4;
        _key5.Text = "5";
        // 
        // _key6
        // 
        _key6.Margin = new System.Windows.Forms.Padding(4);
        _key6.Name = "_key6";
        _key6.Size = new System.Drawing.Size(64, 56);
        _key6.TabIndex = 5;
        _key6.Text = "6";
        // 
        // _key1
        // 
        _key1.Margin = new System.Windows.Forms.Padding(4);
        _key1.Name = "_key1";
        _key1.Size = new System.Drawing.Size(64, 56);
        _key1.TabIndex = 6;
        _key1.Text = "1";
        // 
        // _key2
        // 
        _key2.Margin = new System.Windows.Forms.Padding(4);
        _key2.Name = "_key2";
        _key2.Size = new System.Drawing.Size(64, 56);
        _key2.TabIndex = 7;
        _key2.Text = "2";
        // 
        // _key3
        // 
        _key3.Margin = new System.Windows.Forms.Padding(4);
        _key3.Name = "_key3";
        _key3.Size = new System.Drawing.Size(64, 56);
        _key3.TabIndex = 8;
        _key3.Text = "3";
        // 
        // _key0
        // 
        _key0.Margin = new System.Windows.Forms.Padding(4);
        _key0.Name = "_key0";
        _key0.Size = new System.Drawing.Size(64, 56);
        _key0.TabIndex = 9;
        _key0.Text = "0";
        // 
        // _key00
        // 
        _key00.Margin = new System.Windows.Forms.Padding(4);
        _key00.Name = "_key00";
        _key00.Size = new System.Drawing.Size(64, 56);
        _key00.TabIndex = 10;
        _key00.Text = "00";
        // 
        // _keyDot
        // 
        _keyDot.Margin = new System.Windows.Forms.Padding(4);
        _keyDot.Name = "_keyDot";
        _keyDot.Size = new System.Drawing.Size(64, 56);
        _keyDot.TabIndex = 11;
        _keyDot.Text = ".";
        // 
        // _totalKey
        // 
        _totalKey.Dock = System.Windows.Forms.DockStyle.Fill;
        _totalKey.Margin = new System.Windows.Forms.Padding(4);
        _totalKey.Name = "_totalKey";
        _totalKey.Size = new System.Drawing.Size(192, 60);
        _totalKey.TabIndex = 12;
        _totalKey.Text = "TOTAL";
        // 
        // _sidePanel
        // 
        _sidePanel.AutoSize = true;
        _sidePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        _sidePanel.ColumnCount = 1;
        _sidePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.Controls.Add(_clearKey, 0, 0);
        _sidePanel.Controls.Add(_voidKey, 0, 1);
        _sidePanel.Controls.Add(_corrKey, 0, 2);
        _sidePanel.Controls.Add(_dept1Key, 0, 3);
        _sidePanel.Controls.Add(_dept2Key, 0, 4);
        _sidePanel.Controls.Add(_dept3Key, 0, 5);
        _sidePanel.Controls.Add(_dept4Key, 0, 6);
        _sidePanel.Margin = new System.Windows.Forms.Padding(0);
        _sidePanel.Name = "_sidePanel";
        _sidePanel.RowCount = 7;
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _sidePanel.TabIndex = 1;
        // 
        // _clearKey
        // 
        _clearKey.Dock = System.Windows.Forms.DockStyle.Fill;
        _clearKey.Margin = new System.Windows.Forms.Padding(4);
        _clearKey.Name = "_clearKey";
        _clearKey.Size = new System.Drawing.Size(132, 44);
        _clearKey.TabIndex = 13;
        _clearKey.Text = "CLEAR";
        // 
        // _voidKey
        // 
        _voidKey.Dock = System.Windows.Forms.DockStyle.Fill;
        _voidKey.Margin = new System.Windows.Forms.Padding(4);
        _voidKey.Name = "_voidKey";
        _voidKey.Size = new System.Drawing.Size(132, 44);
        _voidKey.TabIndex = 14;
        _voidKey.Text = "VOID";
        // 
        // _corrKey
        // 
        _corrKey.Dock = System.Windows.Forms.DockStyle.Fill;
        _corrKey.Margin = new System.Windows.Forms.Padding(4);
        _corrKey.Name = "_corrKey";
        _corrKey.Size = new System.Drawing.Size(132, 44);
        _corrKey.TabIndex = 15;
        _corrKey.Text = "CORR";
        // 
        // _dept1Key
        // 
        _dept1Key.Dock = System.Windows.Forms.DockStyle.Fill;
        _dept1Key.Margin = new System.Windows.Forms.Padding(4);
        _dept1Key.Name = "_dept1Key";
        _dept1Key.Size = new System.Drawing.Size(132, 44);
        _dept1Key.TabIndex = 16;
        _dept1Key.Text = "DEPT 1";
        // 
        // _dept2Key
        // 
        _dept2Key.Dock = System.Windows.Forms.DockStyle.Fill;
        _dept2Key.Margin = new System.Windows.Forms.Padding(4);
        _dept2Key.Name = "_dept2Key";
        _dept2Key.Size = new System.Drawing.Size(132, 44);
        _dept2Key.TabIndex = 17;
        _dept2Key.Text = "DEPT 2";
        // 
        // _dept3Key
        // 
        _dept3Key.Dock = System.Windows.Forms.DockStyle.Fill;
        _dept3Key.Margin = new System.Windows.Forms.Padding(4);
        _dept3Key.Name = "_dept3Key";
        _dept3Key.Size = new System.Drawing.Size(132, 44);
        _dept3Key.TabIndex = 18;
        _dept3Key.Text = "DEPT 3";
        // 
        // _dept4Key
        // 
        _dept4Key.Dock = System.Windows.Forms.DockStyle.Fill;
        _dept4Key.Margin = new System.Windows.Forms.Padding(4);
        _dept4Key.Name = "_dept4Key";
        _dept4Key.Size = new System.Drawing.Size(132, 44);
        _dept4Key.TabIndex = 19;
        _dept4Key.Text = "DEPT 4";
        // 
        // _bottomBar
        // 
        _bottomBar.AutoSize = true;
        _bottomBar.ColumnCount = 2;
        _bottomBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _bottomBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        _bottomBar.Controls.Add(_statusLabel, 0, 0);
        _bottomBar.Controls.Add(_openDialogKey, 1, 0);
        _bottomBar.Dock = System.Windows.Forms.DockStyle.Fill;
        _bottomBar.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
        _bottomBar.Name = "_bottomBar";
        _bottomBar.RowCount = 1;
        _bottomBar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _bottomBar.TabIndex = 2;
        // 
        // _statusLabel
        // 
        _statusLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
        _statusLabel.AutoSize = true;
        _statusLabel.Name = "_statusLabel";
        _statusLabel.TabIndex = 0;
        _statusLabel.Text = "Press a key…";
        // 
        // _openDialogKey
        // 
        _openDialogKey.Anchor = System.Windows.Forms.AnchorStyles.Right;
        _openDialogKey.AutoSize = true;
        _openDialogKey.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
        _openDialogKey.Name = "_openDialogKey";
        _openDialogKey.TabIndex = 20;
        _openDialogKey.Text = "TEST MODAL DIALOG…";
        // 
        // MainForm
        // 
        AcceptButton = _totalKey;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(640, 560);
        Controls.Add(_rootLayout);
        MinimumSize = new System.Drawing.Size(560, 540);
        Name = "MainForm";
        Padding = new System.Windows.Forms.Padding(14);
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "MaterialKeyButton — Cash Register";
        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _keysPanel.ResumeLayout(false);
        _keysPanel.PerformLayout();
        _leftPanel.ResumeLayout(false);
        _leftPanel.PerformLayout();
        _numberPad.ResumeLayout(false);
        _sidePanel.ResumeLayout(false);
        _bottomBar.ResumeLayout(false);
        _bottomBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _rootLayout;
    private System.Windows.Forms.Label _headerLabel;
    private System.Windows.Forms.TableLayoutPanel _keysPanel;
    private System.Windows.Forms.TableLayoutPanel _leftPanel;
    private System.Windows.Forms.TableLayoutPanel _numberPad;
    private MaterialKeys.MaterialKeyButton _key7;
    private MaterialKeys.MaterialKeyButton _key8;
    private MaterialKeys.MaterialKeyButton _key9;
    private MaterialKeys.MaterialKeyButton _key4;
    private MaterialKeys.MaterialKeyButton _key5;
    private MaterialKeys.MaterialKeyButton _key6;
    private MaterialKeys.MaterialKeyButton _key1;
    private MaterialKeys.MaterialKeyButton _key2;
    private MaterialKeys.MaterialKeyButton _key3;
    private MaterialKeys.MaterialKeyButton _key0;
    private MaterialKeys.MaterialKeyButton _key00;
    private MaterialKeys.MaterialKeyButton _keyDot;
    private MaterialKeys.MaterialKeyButton _totalKey;
    private System.Windows.Forms.TableLayoutPanel _sidePanel;
    private MaterialKeys.MaterialKeyButton _clearKey;
    private MaterialKeys.MaterialKeyButton _voidKey;
    private MaterialKeys.MaterialKeyButton _corrKey;
    private MaterialKeys.MaterialKeyButton _dept1Key;
    private MaterialKeys.MaterialKeyButton _dept2Key;
    private MaterialKeys.MaterialKeyButton _dept3Key;
    private MaterialKeys.MaterialKeyButton _dept4Key;
    private System.Windows.Forms.TableLayoutPanel _bottomBar;
    private System.Windows.Forms.Label _statusLabel;
    private MaterialKeys.MaterialKeyButton _openDialogKey;
}
