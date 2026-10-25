namespace SplitFlap.Demo;

partial class OptionsDialog
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
        _descriptionLabel = new Label();
        _updateIntervalTrackBar = new TrackBar();
        _scaleLayout = new TableLayoutPanel();
        _minimumLabel = new Label();
        _currentIntervalLabel = new Label();
        _maximumLabel = new Label();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        _layout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_updateIntervalTrackBar).BeginInit();
        _scaleLayout.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        //
        // _layout
        //
        _layout.AutoSize = true;
        _layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_descriptionLabel, 0, 0);
        _layout.Controls.Add(_updateIntervalTrackBar, 0, 1);
        _layout.Controls.Add(_scaleLayout, 0, 2);
        _layout.Controls.Add(_buttonPanel, 0, 3);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(16);
        _layout.RowCount = 4;
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.Size = new Size(520, 194);
        _layout.TabIndex = 0;
        //
        // _descriptionLabel
        //
        _descriptionLabel.AutoSize = true;
        _descriptionLabel.Location = new Point(19, 16);
        _descriptionLabel.Margin = new Padding(3, 0, 3, 8);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(293, 20);
        _descriptionLabel.TabIndex = 0;
        _descriptionLabel.Text = "&Update the timetable automatically every:";
        //
        // _updateIntervalTrackBar
        //
        _updateIntervalTrackBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _updateIntervalTrackBar.LargeChange = 6;
        _updateIntervalTrackBar.Location = new Point(19, 47);
        _updateIntervalTrackBar.Maximum = 30;
        _updateIntervalTrackBar.Minimum = 1;
        _updateIntervalTrackBar.Name = "_updateIntervalTrackBar";
        _updateIntervalTrackBar.Size = new Size(482, 56);
        _updateIntervalTrackBar.TabIndex = 1;
        _updateIntervalTrackBar.TickFrequency = 1;
        _updateIntervalTrackBar.Value = 3;
        _updateIntervalTrackBar.ValueChanged += UpdateIntervalTrackBar_ValueChanged;
        //
        // _scaleLayout
        //
        _scaleLayout.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _scaleLayout.AutoSize = true;
        _scaleLayout.ColumnCount = 3;
        _scaleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _scaleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _scaleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _scaleLayout.Controls.Add(_minimumLabel, 0, 0);
        _scaleLayout.Controls.Add(_currentIntervalLabel, 1, 0);
        _scaleLayout.Controls.Add(_maximumLabel, 2, 0);
        _scaleLayout.Location = new Point(19, 106);
        _scaleLayout.Name = "_scaleLayout";
        _scaleLayout.RowCount = 1;
        _scaleLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _scaleLayout.Size = new Size(482, 20);
        _scaleLayout.TabIndex = 2;
        //
        // _minimumLabel
        //
        _minimumLabel.AutoSize = true;
        _minimumLabel.Location = new Point(3, 0);
        _minimumLabel.Name = "_minimumLabel";
        _minimumLabel.Size = new Size(75, 20);
        _minimumLabel.TabIndex = 0;
        _minimumLabel.Text = "10 seconds";
        //
        // _currentIntervalLabel
        //
        _currentIntervalLabel.Anchor = AnchorStyles.Top;
        _currentIntervalLabel.AutoSize = true;
        _currentIntervalLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _currentIntervalLabel.Location = new Point(199, 0);
        _currentIntervalLabel.Name = "_currentIntervalLabel";
        _currentIntervalLabel.Size = new Size(85, 20);
        _currentIntervalLabel.TabIndex = 1;
        _currentIntervalLabel.Text = "30 seconds";
        //
        // _maximumLabel
        //
        _maximumLabel.AutoSize = true;
        _maximumLabel.Location = new Point(384, 0);
        _maximumLabel.Name = "_maximumLabel";
        _maximumLabel.Size = new Size(95, 20);
        _maximumLabel.TabIndex = 2;
        _maximumLabel.Text = "300 seconds";
        //
        // _buttonPanel
        //
        _buttonPanel.Anchor = AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.Location = new Point(337, 134);
        _buttonPanel.Margin = new Padding(3, 8, 3, 3);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(164, 35);
        _buttonPanel.TabIndex = 3;
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
        // OptionsDialog
        //
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = _cancelButton;
        ClientSize = new Size(520, 194);
        Controls.Add(_layout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OptionsDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Options";
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_updateIntervalTrackBar).EndInit();
        _scaleLayout.ResumeLayout(false);
        _scaleLayout.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        _buttonPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _layout;
    private Label _descriptionLabel;
    private TrackBar _updateIntervalTrackBar;
    private TableLayoutPanel _scaleLayout;
    private Label _minimumLabel;
    private Label _currentIntervalLabel;
    private Label _maximumLabel;
    private FlowLayoutPanel _buttonPanel;
    private Button _okButton;
    private Button _cancelButton;
}
