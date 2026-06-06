namespace LayoutTests.App.Carrier;

partial class CarrierForm
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
        SuspendLayout();
        //
        // CarrierForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 700);
        MinimumSize = new Size(640, 480);
        Name = "CarrierForm";
        StartPosition = FormStartPosition.WindowsDefaultLocation;
        Text = "Carrier";
        ResumeLayout(false);
    }

    private System.ComponentModel.IContainer components = null!;
}
