namespace SplitFlap.Demo;

partial class GridDimensionsDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        _layout = new TableLayoutPanel();
        _rowsLabel = new Label();
        _rowsNumericUpDown = new NumericUpDown();
        _columnsLabel = new Label();
        _columnsNumericUpDown = new NumericUpDown();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        _layout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_rowsNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_columnsNumericUpDown).BeginInit();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.AutoSize = true;
        _layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_rowsLabel, 0, 0);
        _layout.Controls.Add(_rowsNumericUpDown, 1, 0);
        _layout.Controls.Add(_columnsLabel, 0, 1);
        _layout.Controls.Add(_columnsNumericUpDown, 1, 1);
        _layout.Controls.Add(_buttonPanel, 0, 2);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(12);
        _layout.RowCount = 3;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.SetColumnSpan(_buttonPanel, 2);
        _layout.Size = new Size(340, 148);
        _layout.TabIndex = 0;
        // 
        // _rowsLabel
        // 
        _rowsLabel.Anchor = AnchorStyles.Left;
        _rowsLabel.AutoSize = true;
        _rowsLabel.Location = new Point(15, 20);
        _rowsLabel.Name = "_rowsLabel";
        _rowsLabel.Size = new Size(45, 20);
        _rowsLabel.TabIndex = 0;
        _rowsLabel.Text = "&Lines:";
        // 
        // _rowsNumericUpDown
        // 
        _rowsNumericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _rowsNumericUpDown.Location = new Point(92, 15);
        _rowsNumericUpDown.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
        _rowsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _rowsNumericUpDown.Name = "_rowsNumericUpDown";
        _rowsNumericUpDown.Size = new Size(233, 27);
        _rowsNumericUpDown.TabIndex = 1;
        _rowsNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // _columnsLabel
        // 
        _columnsLabel.Anchor = AnchorStyles.Left;
        _columnsLabel.AutoSize = true;
        _columnsLabel.Location = new Point(15, 53);
        _columnsLabel.Name = "_columnsLabel";
        _columnsLabel.Size = new Size(71, 20);
        _columnsLabel.TabIndex = 2;
        _columnsLabel.Text = "&Columns:";
        // 
        // _columnsNumericUpDown
        // 
        _columnsNumericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _columnsNumericUpDown.Location = new Point(92, 48);
        _columnsNumericUpDown.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
        _columnsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _columnsNumericUpDown.Name = "_columnsNumericUpDown";
        _columnsNumericUpDown.Size = new Size(233, 27);
        _columnsNumericUpDown.TabIndex = 3;
        _columnsNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // _buttonPanel
        // 
        _buttonPanel.Anchor = AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        _buttonPanel.Location = new Point(161, 81);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(164, 35);
        _buttonPanel.TabIndex = 4;
        // 
        // _okButton
        // 
        _okButton.AutoSize = true;
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Location = new Point(3, 3);
        _okButton.MinimumSize = new Size(75, 0);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(75, 29);
        _okButton.TabIndex = 0;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        // 
        // _cancelButton
        // 
        _cancelButton.AutoSize = true;
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(84, 3);
        _cancelButton.MinimumSize = new Size(75, 0);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(77, 29);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // GridDimensionsDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = _cancelButton;
        ClientSize = new Size(340, 148);
        Controls.Add(_layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "GridDimensionsDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Display Dimensions";
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_rowsNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)_columnsNumericUpDown).EndInit();
        _buttonPanel.ResumeLayout(false);
        _buttonPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _layout;
    private Label _rowsLabel;
    private NumericUpDown _rowsNumericUpDown;
    private Label _columnsLabel;
    private NumericUpDown _columnsNumericUpDown;
    private FlowLayoutPanel _buttonPanel;
    private Button _okButton;
    private Button _cancelButton;
}
