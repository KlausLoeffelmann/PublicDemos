using System.ComponentModel;
using WarpToolkit.WinForms.Containers;

namespace WarpClock.App;

partial class OptionsDialog
{
    private IContainer components;
    private TableLayoutPanel _layoutPanel;
    private Label _introLabel;
    private FluentTabControl _tabs;
    private FlowLayoutPanel _buttonPanel;
    private Button _okButton;
    private Button _cancelButton;

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
        components = new Container();
        _layoutPanel = new TableLayoutPanel();
        _introLabel = new Label();
        _tabs = new FluentTabControl();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        _layoutPanel.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 1;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_introLabel, 0, 0);
        _layoutPanel.Controls.Add(_tabs, 0, 1);
        _layoutPanel.Controls.Add(_buttonPanel, 0, 2);
        _layoutPanel.Dock = DockStyle.Fill;
        _layoutPanel.Location = new Point(0, 0);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.Padding = new Padding(12);
        _layoutPanel.RowCount = 3;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.Size = new Size(980, 760);
        _layoutPanel.TabIndex = 0;
        // 
        // _introLabel
        // 
        _introLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _introLabel.AutoSize = true;
        _introLabel.Location = new Point(15, 15);
        _introLabel.Margin = new Padding(3);
        _introLabel.Name = "_introLabel";
        _introLabel.Size = new Size(950, 30);
        _introLabel.TabIndex = 0;
        _introLabel.Text = "Review a cloned working copy of the WarpClock options. Choose OK to keep changes or Cancel to discard them.";
        // 
        // _tabs
        // 
        _tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tabs.Location = new Point(15, 51);
        _tabs.Margin = new Padding(3);
        _tabs.Name = "_tabs";
        _tabs.Size = new Size(950, 652);
        _tabs.TabIndex = 1;
        // 
        // _buttonPanel
        // 
        _buttonPanel.Anchor = AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        _buttonPanel.Location = new Point(794, 709);
        _buttonPanel.Margin = new Padding(3);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(171, 38);
        _buttonPanel.TabIndex = 2;
        // 
        // _okButton
        // 
        _okButton.Location = new Point(3, 3);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(75, 32);
        _okButton.TabIndex = 0;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OnOkClick;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(84, 3);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(84, 32);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // OptionsDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(980, 760);
        Controls.Add(_layoutPanel);
        MinimumSize = new Size(760, 580);
        Name = "OptionsDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "WarpClock Options";
        _layoutPanel.ResumeLayout(false);
        _layoutPanel.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
