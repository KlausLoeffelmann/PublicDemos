using WarpToolkit.WinForms.Specialized;
namespace BranchComposer.App;

partial class GitConsoleView
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
        consoleControl = new ConsoleControl();
        SuspendLayout();
        //
        // consoleControl
        //
        consoleControl.BackColor = Color.FromArgb(30, 30, 30);
        consoleControl.BorderStyle = BorderStyle.None;
        consoleControl.Dock = DockStyle.Fill;
        consoleControl.Font = new Font("Consolas", 9F);
        consoleControl.ForeColor = Color.Gainsboro;
        consoleControl.Location = new Point(0, 0);
        consoleControl.Name = "consoleControl";
        consoleControl.ReadOnly = true;
        consoleControl.Size = new Size(780, 220);
        consoleControl.TabIndex = 0;
        consoleControl.Text = "";
        //
        // GitConsoleView
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(consoleControl);
        Name = "GitConsoleView";
        Size = new Size(780, 220);
        ResumeLayout(false);
    }

    private System.ComponentModel.IContainer components = null!;
    private ConsoleControl consoleControl;
}
