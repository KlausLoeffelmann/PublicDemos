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
        Application.SetDefaultVisualStylesMode(VisualStylesMode.Latest);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        MainForm form = new();
        Application.Run(form);
    }
}
