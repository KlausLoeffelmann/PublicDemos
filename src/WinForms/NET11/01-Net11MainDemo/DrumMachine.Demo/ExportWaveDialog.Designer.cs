namespace DrumMachine.Demo;

partial class ExportWaveDialog
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
        _scopeGroup = new GroupBox();
        _scopeLayout = new TableLayoutPanel();
        _currentBar = new RadioButton();
        _fullLoop = new RadioButton();
        _repeated = new RadioButton();
        _repetitionsCaption = new Label();
        _repetitions = new NumericUpDown();
        _includeTail = new CheckBox();
        _explanation = new Label();
        _duration = new Label();
        _buttons = new FlowLayoutPanel();
        _export = new Button();
        _cancel = new Button();
        SuspendLayout();
        _layout.SuspendLayout();
        _scopeGroup.SuspendLayout();
        _scopeLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_repetitions).BeginInit();
        _buttons.SuspendLayout();
        //
        // _layout
        //
        _layout.AutoSize = true;
        _layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_scopeGroup, 0, 0);
        _layout.Controls.Add(_includeTail, 0, 1);
        _layout.Controls.Add(_explanation, 0, 2);
        _layout.Controls.Add(_duration, 0, 3);
        _layout.Controls.Add(_buttons, 0, 4);
        _layout.Dock = DockStyle.Fill;
        _layout.Name = "_layout";
        _layout.Padding = new Padding(16);
        _layout.RowCount = 5;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(520, 360);
        _layout.TabIndex = 0;
        //
        // _scopeGroup
        //
        _scopeGroup.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        _scopeGroup.AutoSize = true;
        _scopeGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _scopeGroup.Controls.Add(_scopeLayout);
        _scopeGroup.Name = "_scopeGroup";
        _scopeGroup.Padding = new Padding(12);
        _scopeGroup.TabIndex = 0;
        _scopeGroup.TabStop = false;
        _scopeGroup.Text = "What to export";
        //
        // _scopeLayout
        //
        _scopeLayout.AutoSize = true;
        _scopeLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _scopeLayout.ColumnCount = 3;
        _scopeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _scopeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _scopeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _scopeLayout.Controls.Add(_currentBar, 0, 0);
        _scopeLayout.Controls.Add(_fullLoop, 0, 1);
        _scopeLayout.Controls.Add(_repeated, 0, 2);
        _scopeLayout.Controls.Add(_repetitions, 1, 2);
        _scopeLayout.Controls.Add(_repetitionsCaption, 2, 2);
        _scopeLayout.Dock = DockStyle.Fill;
        _scopeLayout.Name = "_scopeLayout";
        _scopeLayout.RowCount = 3;
        _scopeLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _scopeLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _scopeLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _scopeLayout.TabIndex = 0;
        _scopeLayout.SetColumnSpan(_currentBar, 3);
        _scopeLayout.SetColumnSpan(_fullLoop, 3);
        //
        // _currentBar
        //
        _currentBar.AccessibleName = "Current bar";
        _currentBar.Anchor = AnchorStyles.Left;
        _currentBar.AutoSize = true;
        _currentBar.Name = "_currentBar";
        _currentBar.TabIndex = 0;
        _currentBar.Text = "Current &bar";
        _currentBar.UseVisualStyleBackColor = true;
        _currentBar.CheckedChanged += Scope_CheckedChanged;
        //
        // _fullLoop
        //
        _fullLoop.AccessibleName = "Full loop";
        _fullLoop.Anchor = AnchorStyles.Left;
        _fullLoop.AutoSize = true;
        _fullLoop.Checked = true;
        _fullLoop.Name = "_fullLoop";
        _fullLoop.TabIndex = 1;
        _fullLoop.TabStop = true;
        _fullLoop.Text = "&Full loop";
        _fullLoop.UseVisualStyleBackColor = true;
        _fullLoop.CheckedChanged += Scope_CheckedChanged;
        //
        // _repeated
        //
        _repeated.AccessibleName = "Repeated full loop";
        _repeated.Anchor = AnchorStyles.Left;
        _repeated.AutoSize = true;
        _repeated.Margin = new Padding(3, 8, 3, 3);
        _repeated.Name = "_repeated";
        _repeated.TabIndex = 2;
        _repeated.Text = "&Repeated full loop";
        _repeated.UseVisualStyleBackColor = true;
        _repeated.CheckedChanged += Scope_CheckedChanged;
        //
        // _repetitions
        //
        _repetitions.AccessibleName = "Number of full-loop repetitions";
        _repetitions.Anchor = AnchorStyles.Left;
        _repetitions.Enabled = false;
        _repetitions.Location = new Point(160, 62);
        _repetitions.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        _repetitions.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _repetitions.Name = "_repetitions";
        _repetitions.Size = new Size(84, 23);
        _repetitions.TabIndex = 3;
        _repetitions.Value = new decimal(new int[] { 1, 0, 0, 0 });
        _repetitions.ValueChanged += Repetitions_ValueChanged;
        //
        // _repetitionsCaption
        //
        _repetitionsCaption.Anchor = AnchorStyles.Left;
        _repetitionsCaption.AutoSize = true;
        _repetitionsCaption.Enabled = false;
        _repetitionsCaption.Margin = new Padding(3, 8, 3, 3);
        _repetitionsCaption.Name = "_repetitionsCaption";
        _repetitionsCaption.TabIndex = 4;
        _repetitionsCaption.Text = "times (1 to 10000)";
        //
        // _includeTail
        //
        _includeTail.AccessibleDescription = "Append the natural decay after the musical end. Leave off to keep the file at the musical length.";
        _includeTail.AccessibleName = "Include final sound tail";
        _includeTail.Anchor = AnchorStyles.Left;
        _includeTail.AutoSize = true;
        _includeTail.Margin = new Padding(3, 12, 3, 3);
        _includeTail.Name = "_includeTail";
        _includeTail.TabIndex = 1;
        _includeTail.Text = "&Include final sound tail";
        _includeTail.UseVisualStyleBackColor = true;
        //
        // _explanation
        //
        _explanation.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        _explanation.AutoSize = true;
        _explanation.Margin = new Padding(3, 12, 3, 3);
        _explanation.MaximumSize = new Size(440, 0);
        _explanation.Name = "_explanation";
        _explanation.TabIndex = 2;
        _explanation.Text = "Writes unnormalized 16-bit stereo PCM using the current player calibration, tempo, mixer, metallic layer, and sounds. Both channels carry that mono mix. Repeated loops are continuous, so decaying sounds carry into the next repetition. No keepalive tone or extra noise is added. Playback is left running, and the loop document is not changed or saved. The estimate is musical length only. A final tail that would push the file past one hour is still rejected.";
        //
        // _duration
        //
        _duration.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        _duration.AutoSize = true;
        _duration.Margin = new Padding(3, 12, 3, 3);
        _duration.MaximumSize = new Size(440, 0);
        _duration.Name = "_duration";
        _duration.TabIndex = 3;
        _duration.Text = "Estimated musical duration:";
        //
        // _buttons
        //
        _buttons.Anchor = AnchorStyles.Right;
        _buttons.AutoSize = true;
        _buttons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttons.Controls.Add(_export);
        _buttons.Controls.Add(_cancel);
        _buttons.Margin = new Padding(3, 16, 3, 3);
        _buttons.Name = "_buttons";
        _buttons.TabIndex = 4;
        //
        // _export
        //
        _export.AccessibleName = "Export";
        _export.AutoSize = true;
        _export.MinimumSize = new Size(75, 28);
        _export.Name = "_export";
        _export.TabIndex = 0;
        _export.Text = "&Export";
        _export.UseVisualStyleBackColor = true;
        _export.Click += Export_Click;
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
        // ExportWaveDialog
        //
        AcceptButton = _export;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = _cancel;
        ClientSize = new Size(520, 360);
        Controls.Add(_layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimumSize = new Size(480, 280);
        MinimizeBox = false;
        Name = "ExportWaveDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Export WAV";
        _buttons.ResumeLayout(false);
        _buttons.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_repetitions).EndInit();
        _scopeLayout.ResumeLayout(false);
        _scopeLayout.PerformLayout();
        _scopeGroup.ResumeLayout(false);
        _scopeGroup.PerformLayout();
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private TableLayoutPanel _layout;
    private GroupBox _scopeGroup;
    private TableLayoutPanel _scopeLayout;
    private RadioButton _currentBar;
    private RadioButton _fullLoop;
    private RadioButton _repeated;
    private Label _repetitionsCaption;
    private NumericUpDown _repetitions;
    private CheckBox _includeTail;
    private Label _explanation;
    private Label _duration;
    private FlowLayoutPanel _buttons;
    private Button _export;
    private Button _cancel;
}
