namespace WarpClock.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Follow the operating system's light/dark preference for the WinForms chrome
        // (menu strip, status strip, dialogs). The DirectX clock surface renders its own
        // theme-specific colors, so this only affects the surrounding application UI.
        Application.SetColorMode(SystemColorMode.System);

        Application.Run(new FormMain());
    }
}
