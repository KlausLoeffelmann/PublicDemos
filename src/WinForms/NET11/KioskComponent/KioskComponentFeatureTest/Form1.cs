using System.Diagnostics;

namespace KioskComponentFeatureTest;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void KioskModeManager_Wakeup(object sender, KioskModeWakeupEventArgs e)
    {
        Debug.Print(e.)

    }
}
