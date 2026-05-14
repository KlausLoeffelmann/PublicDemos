using System.Diagnostics;
using Northwind.DataLayer;

namespace Northwind.App
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

            Application.Run(new FrmMain());
        }
    }
}