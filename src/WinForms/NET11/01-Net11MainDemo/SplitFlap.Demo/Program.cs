namespace SplitFlap.Demo;

/// <summary>
///  Provides the process entry point and application-wide exception logging.
/// </summary>
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (!StartupOptions.TryParse(args, out StartupOptions options, out string? error))
        {
            Console.Error.WriteLine(error);
            Console.Error.WriteLine(StartupOptions.Usage);
            return 2;
        }

        AppLogger.Initialize();
        AppLogger.Information("Application", $"Starting with {options}.");

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
            AppLogger.Critical("Application", "Startup failed.", ex);
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
        AppLogger.Error("Application", "Unhandled UI-thread exception.", e.Exception);
        MessageBox.Show(
            $"An unexpected error occurred. Details were written to:{Environment.NewLine}{AppPaths.LogDirectory}",
            "Split-Flap Demo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            AppLogger.Critical("Application", "Unhandled non-UI exception.", exception);
        }
    }
}
