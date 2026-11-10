// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo;

/// <summary>
///  Starts the exploratory WinForms demo using the switches in <see cref="DemoConstants"/>.
/// </summary>
internal static class Program
{
    /// <summary>Configures application-wide defaults before creating any windows.</summary>
    [STAThread]
    public static void Main()
    {
        // Keep GDI text rendering consistent with the native WinForms controls.
        Application.SetCompatibleTextRenderingDefault(false);

        // Enable the native visual styles introduced with Windows XP.
        Application.EnableVisualStyles();

        // DPI awareness and color mode are startup decisions; changing them requires a restart.
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // Follow the Windows light/dark preference (.NET 9 and later).
        Application.SetColorMode(SystemColorMode.System);

        // The View menu can still change the active scenario's style at runtime.
        Application.SetDefaultVisualStylesMode(DemoConstants.StartupVisualStylesMode);

        // Deferred reveal hides intermediate startup paints, including the artificial slowdown.
        Application.SetDefaultFormRevealMode(DemoConstants.StartupFormRevealMode);

        MainForm form = new();
        Application.Run(form);
    }
}
