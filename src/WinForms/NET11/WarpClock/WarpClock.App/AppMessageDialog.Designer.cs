namespace WarpClock.App;

partial class AppMessageDialog
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
        _headlineLabel = new Label();
        _messageTextBox = new TextBox();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _layout.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_headlineLabel, 0, 0);
        _layout.Controls.Add(_messageTextBox, 0, 1);
        _layout.Controls.Add(_buttonPanel, 0, 2);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(12, 12);
        _layout.Name = "_layout";
        _layout.RowCount = 3;
        _layout.RowStyles.Add(new RowStyle());
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layout.RowStyles.Add(new RowStyle());
        _layout.Size = new Size(560, 257);
        _layout.TabIndex = 0;
        // 
        // _headlineLabel
        // 
        _headlineLabel.AutoSize = true;
        _headlineLabel.Dock = DockStyle.Fill;
        _headlineLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _headlineLabel.Location = new Point(3, 0);
        _headlineLabel.Margin = new Padding(3, 0, 3, 12);
        _headlineLabel.Name = "_headlineLabel";
        _headlineLabel.Size = new Size(554, 20);
        _headlineLabel.TabIndex = 0;
        _headlineLabel.Text = "WarpClock";
        // 
        // _messageTextBox
        // 
        _messageTextBox.Dock = DockStyle.Fill;
        _messageTextBox.Location = new Point(3, 35);
        _messageTextBox.Margin = new Padding(3, 3, 3, 12);
        _messageTextBox.Multiline = true;
        _messageTextBox.Name = "_messageTextBox";
        _messageTextBox.ReadOnly = true;
        _messageTextBox.ScrollBars = ScrollBars.Vertical;
        _messageTextBox.Size = new Size(554, 177);
        _messageTextBox.TabIndex = 1;
        // 
        // _buttonPanel
        // 
        _buttonPanel.AutoSize = true;
        _buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Dock = DockStyle.Right;
        _buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        _buttonPanel.Location = new Point(479, 227);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(78, 27);
        _buttonPanel.TabIndex = 2;
        // 
        // _okButton
        // 
        _okButton.AutoSize = true;
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Location = new Point(3, 0);
        _okButton.Margin = new Padding(3, 0, 0, 0);
        _okButton.MinimumSize = new Size(75, 0);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(75, 27);
        _okButton.TabIndex = 0;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OkButton_Click;
        // 
        // AppMessageDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(584, 281);
        Controls.Add(_layout);
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(420, 240);
        Name = "AppMessageDialog";
        Padding = new Padding(12);
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "WarpClock";
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        _buttonPanel.PerformLayout();
        ResumeLayout(false);
    }

    private TableLayoutPanel _layout;
    private Label _headlineLabel;
    private TextBox _messageTextBox;
    private FlowLayoutPanel _buttonPanel;
    private Button _okButton;
}
