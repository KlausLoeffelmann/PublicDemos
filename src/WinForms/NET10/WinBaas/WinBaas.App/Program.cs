using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WinForms;
using WarpToolkit.WinForms.AppServices.ServiceExtensions;
using WinBaas.Services;

namespace WinBaas;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        WinFormsApplicationBuilder builder = WinFormsApplication.CreateBuilder(args);

        builder
            .UseColorMode(SystemColorMode.System)
            .UseHighDpiMode(HighDpiMode.PerMonitorV2)
            .UseVisualStyles()
            .UseTextRenderingV2();

        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddDebug();
        builder.Logging.Services.AddSingleton<ConsoleLoggerSink>();
        builder.Logging.Services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();

        builder.Services
            .AddWinFormsUserSettingsService()
            .AddWinFormsDialogService()
            .AddWinFormsExceptionService();

        builder.Services.AddSingleton<IFileTypeMap, FileTypeMap>();
        builder.Services.AddSingleton<IRegistryCatalog, RegistryCatalog>();
        builder.Services.AddSingleton<IRegistryDiscovery, RegistryDiscoveryService>();
        builder.Services.AddSingleton<IVisualStudioDiscovery, VisualStudioDiscovery>();
        builder.Services.AddSingleton<ICatalogService, CatalogService>();
        builder.Services.AddSingleton<IDiscoveryService, DiscoveryService>();
        builder.Services.AddSingleton<IBackupService, BackupService>();

        builder.UseStartupForm<FrmMain>();

        using WinFormsApplication app = builder.Build();
        app.Run();
    }
}
