using System.ComponentModel;
using WarpToolkit.WinForms.Controls;

namespace WarpClock.App;

partial class FoldersOptionsView
{
    private IContainer components;
    private TableLayoutPanel _layoutPanel;
    private GroupBox _foldersGroupBox;
    private TableLayoutPanel _foldersLayoutPanel;
    private Label _themesFolderLabel;
    private FilePathPicker _themesFolderPicker;
    private Label _calendarFolderLabel;
    private FilePathPicker _calendarFolderPicker;
    private Label _shortMessagesFolderLabel;
    private FilePathPicker _shortMessagesFolderPicker;
    private Label _picturesFolderLabel;
    private FilePathPicker _picturesFolderPicker;

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
        _foldersGroupBox = new GroupBox();
        _foldersLayoutPanel = new TableLayoutPanel();
        _themesFolderLabel = new Label();
        _themesFolderPicker = new FilePathPicker();
        _calendarFolderLabel = new Label();
        _calendarFolderPicker = new FilePathPicker();
        _shortMessagesFolderLabel = new Label();
        _shortMessagesFolderPicker = new FilePathPicker();
        _picturesFolderLabel = new Label();
        _picturesFolderPicker = new FilePathPicker();
        _layoutPanel.SuspendLayout();
        _foldersGroupBox.SuspendLayout();
        _foldersLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 1;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_foldersGroupBox, 0, 0);
        _layoutPanel.Dock = DockStyle.Top;
        _layoutPanel.Location = new Point(12, 12);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.RowCount = 1;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.Size = new Size(736, 258);
        _layoutPanel.TabIndex = 0;
        // 
        // _foldersGroupBox
        // 
        _foldersGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _foldersGroupBox.Controls.Add(_foldersLayoutPanel);
        _foldersGroupBox.Location = new Point(3, 3);
        _foldersGroupBox.Name = "_foldersGroupBox";
        _foldersGroupBox.Padding = new Padding(12);
        _foldersGroupBox.Size = new Size(730, 252);
        _foldersGroupBox.TabIndex = 0;
        _foldersGroupBox.TabStop = false;
        _foldersGroupBox.Text = "Content folders";
        // 
        // _foldersLayoutPanel
        // 
        _foldersLayoutPanel.ColumnCount = 2;
        _foldersLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _foldersLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _foldersLayoutPanel.Controls.Add(_themesFolderLabel, 0, 0);
        _foldersLayoutPanel.Controls.Add(_themesFolderPicker, 1, 0);
        _foldersLayoutPanel.Controls.Add(_calendarFolderLabel, 0, 1);
        _foldersLayoutPanel.Controls.Add(_calendarFolderPicker, 1, 1);
        _foldersLayoutPanel.Controls.Add(_shortMessagesFolderLabel, 0, 2);
        _foldersLayoutPanel.Controls.Add(_shortMessagesFolderPicker, 1, 2);
        _foldersLayoutPanel.Controls.Add(_picturesFolderLabel, 0, 3);
        _foldersLayoutPanel.Controls.Add(_picturesFolderPicker, 1, 3);
        _foldersLayoutPanel.Dock = DockStyle.Fill;
        _foldersLayoutPanel.Location = new Point(12, 28);
        _foldersLayoutPanel.Name = "_foldersLayoutPanel";
        _foldersLayoutPanel.RowCount = 4;
        _foldersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _foldersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _foldersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _foldersLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _foldersLayoutPanel.Size = new Size(706, 212);
        _foldersLayoutPanel.TabIndex = 0;
        // 
        // _themesFolderLabel
        // 
        _themesFolderLabel.Anchor = AnchorStyles.Left;
        _themesFolderLabel.AutoSize = true;
        _themesFolderLabel.Location = new Point(3, 7);
        _themesFolderLabel.Name = "_themesFolderLabel";
        _themesFolderLabel.Size = new Size(85, 15);
        _themesFolderLabel.TabIndex = 0;
        _themesFolderLabel.Text = "Themes folder:";
        // 
        // _themesFolderPicker
        // 
        _themesFolderPicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _themesFolderPicker.ButtonText = "...";
        _themesFolderPicker.DialogTitle = "Choose themes folder";
        _themesFolderPicker.FileOrFolderPath = "";
        _themesFolderPicker.Location = new Point(153, 3);
        _themesFolderPicker.Name = "_themesFolderPicker";
        _themesFolderPicker.PickerMode = FilePathPickerMode.FolderBrowser;
        _themesFolderPicker.ReadOnly = false;
        _themesFolderPicker.ShowRevealButton = true;
        _themesFolderPicker.Size = new Size(550, 24);
        _themesFolderPicker.TabIndex = 1;
        // 
        // _calendarFolderLabel
        // 
        _calendarFolderLabel.Anchor = AnchorStyles.Left;
        _calendarFolderLabel.AutoSize = true;
        _calendarFolderLabel.Location = new Point(3, 40);
        _calendarFolderLabel.Name = "_calendarFolderLabel";
        _calendarFolderLabel.Size = new Size(90, 15);
        _calendarFolderLabel.TabIndex = 2;
        _calendarFolderLabel.Text = "Calendar folder:";
        // 
        // _calendarFolderPicker
        // 
        _calendarFolderPicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _calendarFolderPicker.ButtonText = "...";
        _calendarFolderPicker.DialogTitle = "Choose calendar folder";
        _calendarFolderPicker.FileOrFolderPath = "";
        _calendarFolderPicker.Location = new Point(153, 36);
        _calendarFolderPicker.Name = "_calendarFolderPicker";
        _calendarFolderPicker.PickerMode = FilePathPickerMode.FolderBrowser;
        _calendarFolderPicker.ReadOnly = false;
        _calendarFolderPicker.ShowRevealButton = true;
        _calendarFolderPicker.Size = new Size(550, 24);
        _calendarFolderPicker.TabIndex = 3;
        // 
        // _shortMessagesFolderLabel
        // 
        _shortMessagesFolderLabel.Anchor = AnchorStyles.Left;
        _shortMessagesFolderLabel.AutoSize = true;
        _shortMessagesFolderLabel.Location = new Point(3, 73);
        _shortMessagesFolderLabel.Name = "_shortMessagesFolderLabel";
        _shortMessagesFolderLabel.Size = new Size(120, 15);
        _shortMessagesFolderLabel.TabIndex = 4;
        _shortMessagesFolderLabel.Text = "Short messages folder:";
        // 
        // _shortMessagesFolderPicker
        // 
        _shortMessagesFolderPicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _shortMessagesFolderPicker.ButtonText = "...";
        _shortMessagesFolderPicker.DialogTitle = "Choose short messages folder";
        _shortMessagesFolderPicker.FileOrFolderPath = "";
        _shortMessagesFolderPicker.Location = new Point(153, 69);
        _shortMessagesFolderPicker.Name = "_shortMessagesFolderPicker";
        _shortMessagesFolderPicker.PickerMode = FilePathPickerMode.FolderBrowser;
        _shortMessagesFolderPicker.ReadOnly = false;
        _shortMessagesFolderPicker.ShowRevealButton = true;
        _shortMessagesFolderPicker.Size = new Size(550, 24);
        _shortMessagesFolderPicker.TabIndex = 5;
        // 
        // _picturesFolderLabel
        // 
        _picturesFolderLabel.Anchor = AnchorStyles.Left;
        _picturesFolderLabel.AutoSize = true;
        _picturesFolderLabel.Location = new Point(3, 106);
        _picturesFolderLabel.Name = "_picturesFolderLabel";
        _picturesFolderLabel.Size = new Size(82, 15);
        _picturesFolderLabel.TabIndex = 6;
        _picturesFolderLabel.Text = "Pictures folder:";
        // 
        // _picturesFolderPicker
        // 
        _picturesFolderPicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _picturesFolderPicker.ButtonText = "...";
        _picturesFolderPicker.DialogTitle = "Choose pictures folder";
        _picturesFolderPicker.FileOrFolderPath = "";
        _picturesFolderPicker.Location = new Point(153, 102);
        _picturesFolderPicker.Name = "_picturesFolderPicker";
        _picturesFolderPicker.PickerMode = FilePathPickerMode.FolderBrowser;
        _picturesFolderPicker.ReadOnly = false;
        _picturesFolderPicker.ShowRevealButton = true;
        _picturesFolderPicker.Size = new Size(550, 24);
        _picturesFolderPicker.TabIndex = 7;
        // 
        // FoldersOptionsView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_layoutPanel);
        Name = "FoldersOptionsView";
        Padding = new Padding(12);
        Size = new Size(760, 280);
        _layoutPanel.ResumeLayout(false);
        _foldersGroupBox.ResumeLayout(false);
        _foldersLayoutPanel.ResumeLayout(false);
        _foldersLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
}
