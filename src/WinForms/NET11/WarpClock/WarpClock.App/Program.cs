using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WinForms;
using WarpToolkit.WinForms.AppServices.ServiceExtensions;

namespace WarpClock.App;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        AppPaths appPaths = new();
        StartupOptions startupOptions;

        try
        {
            startupOptions = StartupOptions.Parse(args);
        }
        catch (ArgumentException ex)
        {
            AppMessageDialog.ShowMessage(
                owner: null,
                title: "WarpClock - Command-line error",
                headline: "WarpClock could not parse the command line.",
                message: ex.Message);
            return;
        }

        try
        {
            WinFormsApplicationBuilder builder = WinFormsApplication.CreateBuilder(args);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            builder
                .UseColorMode(SystemColorMode.System)
                .UseHighDpiMode(HighDpiMode.SystemAware)
                .UseVisualStyles()
                .UseTextRenderingV2()
                .UseStartupForm<FormMain>();

            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Logging.Services.AddSingleton<ILoggerProvider, RollingFileLoggerProvider>();

            builder.Services
                .AddWinFormsUserSettingsService()
                .AddWinFormsExceptionService()
                .AddSingleton(appPaths)
                .AddSingleton(startupOptions)
                .AddSingleton<ThemePluginLoader>()
                .AddSingleton<AppStateStore>()
                .AddSingleton<ThemeListStore>()
                .AddSingleton<AppExceptionRouter>();

            using WinFormsApplication app = builder.Build();
            app.Run();
        }
        catch (Exception ex)
        {
            TryWriteStartupFailure(appPaths, ex);

            AppMessageDialog.ShowMessage(
                owner: null,
                title: "WarpClock - Startup failure",
                headline: "WarpClock could not start.",
                message: ex.Message + Environment.NewLine + Environment.NewLine
                    + $"Details were written to:{Environment.NewLine}{appPaths.LogDirectory}");
        }
    }

    private static void TryWriteStartupFailure(AppPaths appPaths, Exception exception)
    {
        try
        {
            Directory.CreateDirectory(appPaths.LogDirectory);
            string logPath = Path.Combine(
                appPaths.LogDirectory,
                $"warpclock-{DateTime.Now:yyyyMMdd}.log");

            string text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} crit Program: Startup failure{Environment.NewLine}{exception}{Environment.NewLine}";
            File.AppendAllText(logPath, text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }
        catch
        {
        }
    }
}
