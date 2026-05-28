using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WinForms;
using System.Diagnostics;
using WarpToolkit.Microsoft.Extensions.Logging;
using WarpToolkit.WinForms.AppServices.ServiceExtensions;

namespace Winget_Package_Editor;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        WinFormsApplicationBuilder builder = WinFormsApplication.CreateBuilder();

        // We want to use the UserSettings service, for a convenient
        // way to store user settings in a file.
        builder.Services.AddWinFormsUserSettingsService();

        // We want to use the Exception service, so we can handle
        // unhandled exceptions in a consistent way.
        builder.Services.AddWinFormsExceptionService();

        // One we setup this service, compatible component can use the
        // service to either get the AI-Provider key via this default local
        // key, or can pass a different EnvironmentVariable key once they got
        // the service, to get the actual key from the environment variable.
        builder.Services.AddLocalKeyRetrievalService();

        // We want to use the BlazorWebView service, so we can
        // so we can use the ChatView control, which is based
        // on the BlazorWebView control.
        // builder.Services.AddWindowsFormsBlazorWebView();

        Debug.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);

        // Configure logging
        builder.Logging.AddTimeStampedDebug();

        // Register the main form as a scoped service.
        // This is not only convenient, but also allows us to use dependency injection,
        // and particularly to provide the Form the ServiceProvider, which it itself can
        // distribute by calling the Form Extension method `AssignServices(serviceProvider)`.
        builder.Services.AddScoped<MainForm>();

        // Configure WinForms-specific options

        // Variant 1: loading configuration from an appsettings.json file.
        // builder.UseStartupForm<MainForm>()
        //    // We are using an appsettings.json file for configuration.
        //    .AllowWinFormsJsonAppSettings();

        // Variant 2: Setting up configuration through code.
        builder.UseStartupForm<MainForm>()
            .UseHighDpiMode(HighDpiMode.SystemAware)
            .UseColorMode(SystemColorMode.System)
            .UseTextRenderingV2()
            .UseVisualStyles();

        // Build and run the application
        WinFormsApplication app = builder.Build();

        app.Run();
    }
}
