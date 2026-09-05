namespace DrumMachine.Demo;

partial class NewLoopDialog
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

    private void InitializeComponent()
    {
        _layout = new TableLayoutPanel();
        _caption = new Label();
        _bars = new ComboBox();
        _buttons = new FlowLayoutPanel();
        _ok = new Button();
        _cancel = new Button();
        SuspendLayout();
        _layout.SuspendLayout();
        _buttons.SuspendLayout();
        //
        // _layout
        //
        _layout.AutoSize = true;
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_caption, 0, 0);
        _layout.Controls.Add(_bars, 1, 0);
        _layout.Controls.Add(_buttons, 1, 1);
        _layout.Dock = DockStyle.Fill;
        _layout.Name = "_layout";
        _layout.Padding = new Padding(16);
        _layout.RowCount = 2;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(340, 120);
        _layout.TabIndex = 0;
        //
        // _caption
        //
        _caption.Anchor = AnchorStyles.Left;
        _caption.AutoSize = true;
        _caption.Name = "_caption";
        _caption.TabIndex = 0;
        _caption.Text = "Number of &bars:";
        //
        // _bars
        //
        _bars.AccessibleName = "Number of bars in the new loop";
        _bars.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _bars.DropDownStyle = ComboBoxStyle.DropDownList;
        _bars.Items.AddRange(new object[] { "1 bar", "2 bars", "4 bars" });
        _bars.Name = "_bars";
        _bars.TabIndex = 1;
        //
        // _buttons
        //
        _buttons.Anchor = AnchorStyles.Right;
        _buttons.AutoSize = true;
        _buttons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttons.Controls.Add(_ok);
        _buttons.Controls.Add(_cancel);
        _buttons.Margin = new Padding(3, 16, 3, 3);
        _buttons.Name = "_buttons";
        _buttons.TabIndex = 2;
        //
        // _ok
        //
        _ok.AutoSize = true;
        _ok.DialogResult = DialogResult.OK;
        _ok.MinimumSize = new Size(75, 28);
        _ok.Name = "_ok";
        _ok.TabIndex = 0;
        _ok.Text = "Create";
        _ok.UseVisualStyleBackColor = true;
        //
        // _cancel
        //
        _cancel.AutoSize = true;
        _cancel.DialogResult = DialogResult.Cancel;
        _cancel.MinimumSize = new Size(75, 28);
        _cancel.Name = "_cancel";
        _cancel.TabIndex = 1;
        _cancel.Text = "Cancel";
        _cancel.UseVisualStyleBackColor = true;
        //
        // NewLoopDialog
        //
        AcceptButton = _ok;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = _cancel;
        ClientSize = new Size(340, 120);
        Controls.Add(_layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "NewLoopDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "New loop";
        _buttons.ResumeLayout(false);
        _buttons.PerformLayout();
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel _layout;
    private Label _caption;
    private ComboBox _bars;
    private FlowLayoutPanel _buttons;
    private Button _ok;
    private Button _cancel;
}
