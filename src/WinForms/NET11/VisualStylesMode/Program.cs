// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo;

// This project is meant for temporary testing and experimenting and should be kept as simple as possible.

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetDefaultVisualStylesMode(VisualStylesMode.Net11);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.SetSystemTextSizeAwareness(SystemTextSizeAwareness.Notify);
        Application.SetColorMode(SystemColorMode.System);
        MainForm form = new();
        Application.Run(form);
    }
}
