namespace DrumMachine.Demo;

/// <summary>
///  Starts the standalone rhythm demo and routes failures to its dedicated AppData log.
/// </summary>
internal static class Program
{
    private static bool s_automated;

    [STAThread]
    private static int Main(string[] args)
    {
        if (!StartupOptions.TryParse(args, out StartupOptions options, out string? error))
        {
            Console.Error.WriteLine(error);
            Console.Error.WriteLine(StartupOptions.Usage);
            return 2;
        }

        if (options.ShowHelp)
        {
            Console.WriteLine(StartupOptions.Usage);
            return 0;
        }

        s_automated = options.Scenario != DemoScenario.None;
        AppLogger.Initialize();
        AppLogger.Information("Application", $"Starting scenario={options.Scenario}, runFor={options.RunFor}.");
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        ApplicationConfiguration.Initialize();
        Application.SetColorMode(SystemColorMode.System);

        try
        {
            Application.Run(new MainForm(options));
            return Environment.ExitCode;
        }
        catch (Exception ex)
        {
            Environment.ExitCode = 1;
            AppLogger.Error("Application", "Startup failed.", ex);
            return 1;
        }
        finally
        {
            Application.ThreadException -= OnThreadException;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            AppLogger.Information("Application", $"Stopped with exit code {Environment.ExitCode}.");
            AppLogger.Shutdown();
        }
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        Environment.ExitCode = 1;
        AppLogger.Error("Application", "Unhandled UI exception.", e.Exception);
        if (s_automated)
        {
            Application.Exit();
        }
        else
        {
            MessageBox.Show(
                $"An unexpected error occurred.{Environment.NewLine}{e.Exception.Message}{Environment.NewLine}Logs: {AppPaths.LogDirectory}",
                "Rhythm Demo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        Environment.ExitCode = 1;
        if (e.ExceptionObject is Exception exception)
        {
            AppLogger.Error("Application", "Unhandled worker exception.", exception);
        }
    }
}
