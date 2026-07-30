namespace CameraControlDemo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Application.SetColorMode(SystemColorMode.Dark);

#region New in .NET 11
            Application.SetDefaultFormRevealMode(FormRevealMode.Classic);
            Application.SetDefaultVisualStylesMode(VisualStylesMode.Classic);
#endregion

            Application.Run(new MainForm());
        }
    }
}