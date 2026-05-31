// =========================================================================
// PROGRAM
// =========================================================================
using System.Runtime.InteropServices;

namespace WinFormsPong;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new PongForm());
    }
}
