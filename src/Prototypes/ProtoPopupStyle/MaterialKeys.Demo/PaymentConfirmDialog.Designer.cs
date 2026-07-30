namespace MaterialKeys.Demo;

partial class PaymentConfirmDialog
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
        _promptLabel = new System.Windows.Forms.Label();
        _buttonLayout = new System.Windows.Forms.FlowLayoutPanel();
        _cashKey = new MaterialKeys.MaterialKeyButton();
        _cardKey = new MaterialKeys.MaterialKeyButton();
        _voucherKey = new MaterialKeys.MaterialKeyButton();
        _cancelKey = new MaterialKeys.MaterialKeyButton();
        _rootLayout.SuspendLayout();
        _buttonLayout.SuspendLayout();
        SuspendLayout();
        // 
        // _rootLayout
        // 
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _rootLayout.Controls.Add(_promptLabel, 0, 0);
        _rootLayout.Controls.Add(_buttonLayout, 0, 1);
        _rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        _rootLayout.Location = new System.Drawing.Point(12, 12);
        _rootLayout.Name = "_rootLayout";
        _rootLayout.RowCount = 2;
        _rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        _rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        _rootLayout.Size = new System.Drawing.Size(436, 176);
        _rootLayout.TabIndex = 0;
        // 
        // _promptLabel
        // 
        _promptLabel.AutoSize = true;
        _promptLabel.Dock = System.Windows.Forms.DockStyle.Fill;
        _promptLabel.Location = new System.Drawing.Point(3, 0);
        _promptLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
        _promptLabel.Name = "_promptLabel";
        _promptLabel.Size = new System.Drawing.Size(430, 15);
        _promptLabel.TabIndex = 0;
        _promptLabel.Text = "Choose a payment method. Enter triggers the default (CASH) key; Tab moves the focus cue.";
        // 
        // _buttonLayout
        // 
        _buttonLayout.AutoSize = true;
        _buttonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        _buttonLayout.Location = new System.Drawing.Point(3, 26);
        _buttonLayout.Name = "_buttonLayout";
        _buttonLayout.Size = new System.Drawing.Size(430, 147);
        _buttonLayout.TabIndex = 1;
        _buttonLayout.WrapContents = true;
        // 
        // _cashKey
        // 
        _cashKey.DialogResult = System.Windows.Forms.DialogResult.OK;
        _cashKey.Margin = new System.Windows.Forms.Padding(6);
        _cashKey.Name = "_cashKey";
        _cashKey.Size = new System.Drawing.Size(96, 56);
        _cashKey.TabIndex = 0;
        _cashKey.Text = "CASH";
        // 
        // _cardKey
        // 
        _cardKey.DialogResult = System.Windows.Forms.DialogResult.OK;
        _cardKey.Margin = new System.Windows.Forms.Padding(6);
        _cardKey.Name = "_cardKey";
        _cardKey.Size = new System.Drawing.Size(96, 56);
        _cardKey.TabIndex = 1;
        _cardKey.Text = "CARD";
        // 
        // _voucherKey
        // 
        _voucherKey.DialogResult = System.Windows.Forms.DialogResult.OK;
        _voucherKey.Margin = new System.Windows.Forms.Padding(6);
        _voucherKey.Name = "_voucherKey";
        _voucherKey.Size = new System.Drawing.Size(96, 56);
        _voucherKey.TabIndex = 2;
        _voucherKey.Text = "VOUCHER";
        // 
        // _cancelKey
        // 
        _cancelKey.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        _cancelKey.Margin = new System.Windows.Forms.Padding(6);
        _cancelKey.Name = "_cancelKey";
        _cancelKey.Size = new System.Drawing.Size(96, 56);
        _cancelKey.TabIndex = 3;
        _cancelKey.Text = "CANCEL";
        // 
        // _buttonLayout.Controls
        // 
        _buttonLayout.Controls.Add(_cashKey);
        _buttonLayout.Controls.Add(_cardKey);
        _buttonLayout.Controls.Add(_voucherKey);
        _buttonLayout.Controls.Add(_cancelKey);
        // 
        // PaymentConfirmDialog
        // 
        AcceptButton = _cashKey;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        CancelButton = _cancelKey;
        ClientSize = new System.Drawing.Size(460, 200);
        Controls.Add(_rootLayout);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PaymentConfirmDialog";
        Padding = new System.Windows.Forms.Padding(12);
        ShowInTaskbar = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Confirm Payment";
        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _buttonLayout.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _rootLayout;
    private System.Windows.Forms.Label _promptLabel;
    private System.Windows.Forms.FlowLayoutPanel _buttonLayout;
    private MaterialKeys.MaterialKeyButton _cashKey;
    private MaterialKeys.MaterialKeyButton _cardKey;
    private MaterialKeys.MaterialKeyButton _voucherKey;
    private MaterialKeys.MaterialKeyButton _cancelKey;
}
