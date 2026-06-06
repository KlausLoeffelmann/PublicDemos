namespace LayoutTests.App.Designer;

partial class AddContainerDialog
{
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
        components = new System.ComponentModel.Container();
        kindGroupBox = new GroupBox();
        ctorRadio = new RadioButton();
        lazyRadio = new RadioButton();
        nameLabel = new Label();
        nameTextBox = new TextBox();
        okButton = new Button();
        cancelButton = new Button();
        kindGroupBox.SuspendLayout();
        SuspendLayout();
        //
        // kindGroupBox
        //
        kindGroupBox.Controls.Add(ctorRadio);
        kindGroupBox.Controls.Add(lazyRadio);
        kindGroupBox.Location = new Point(12, 49);
        kindGroupBox.Name = "kindGroupBox";
        kindGroupBox.Size = new Size(320, 80);
        kindGroupBox.TabIndex = 2;
        kindGroupBox.TabStop = false;
        kindGroupBox.Text = "Container kind";
        //
        // ctorRadio
        //
        ctorRadio.AutoSize = true;
        ctorRadio.Location = new Point(12, 22);
        ctorRadio.Name = "ctorRadio";
        ctorRadio.Size = new Size(255, 19);
        ctorRadio.TabIndex = 0;
        ctorRadio.TabStop = true;
        ctorRadio.Text = "CTor — added in carrier constructor";
        //
        // lazyRadio
        //
        lazyRadio.AutoSize = true;
        lazyRadio.Location = new Point(12, 47);
        lazyRadio.Name = "lazyRadio";
        lazyRadio.Size = new Size(290, 19);
        lazyRadio.TabIndex = 1;
        lazyRadio.Text = "Lazy — added via BeginInvoke after Load";
        //
        // nameLabel
        //
        nameLabel.AutoSize = true;
        nameLabel.Location = new Point(12, 15);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(42, 15);
        nameLabel.TabIndex = 0;
        nameLabel.Text = "Name:";
        //
        // nameTextBox
        //
        nameTextBox.Location = new Point(72, 12);
        nameTextBox.Name = "nameTextBox";
        nameTextBox.Size = new Size(260, 23);
        nameTextBox.TabIndex = 1;
        //
        // okButton
        //
        okButton.DialogResult = DialogResult.OK;
        okButton.Location = new Point(176, 145);
        okButton.Name = "okButton";
        okButton.Size = new Size(75, 27);
        okButton.TabIndex = 3;
        okButton.Text = "OK";
        //
        // cancelButton
        //
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(257, 145);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 27);
        cancelButton.TabIndex = 4;
        cancelButton.Text = "Cancel";
        //
        // AddContainerDialog
        //
        AcceptButton = okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(344, 184);
        Controls.Add(cancelButton);
        Controls.Add(okButton);
        Controls.Add(nameTextBox);
        Controls.Add(nameLabel);
        Controls.Add(kindGroupBox);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AddContainerDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add Container";
        kindGroupBox.ResumeLayout(false);
        kindGroupBox.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.ComponentModel.IContainer components = null!;
    private GroupBox kindGroupBox;
    private RadioButton ctorRadio;
    private RadioButton lazyRadio;
    private Label nameLabel;
    private TextBox nameTextBox;
    private Button okButton;
    private Button cancelButton;
}
