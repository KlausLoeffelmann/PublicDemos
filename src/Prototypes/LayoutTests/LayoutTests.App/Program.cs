using LayoutTests.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WinForms;
using WarpToolkit.WinForms.AppServices.ServiceExtensions;

namespace LayoutTests.App;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = WinFormsApplication.CreateBuilder(args);

        builder
            .UseColorMode(SystemColorMode.System)
            .UseHighDpiMode(HighDpiMode.PerMonitorV2)
            .UseVisualStyles()
            .UseTextRenderingV2()
            .UseStartupForm<MainForm>();

        builder.Services
            .AddWinFormsDialogService()
            .AddWinFormsExceptionService()
            .AddWinFormsUserSettingsService()
            .AddSingleton<ProbeSetStore>()
            .AddSingleton<UselessFacts>();

        builder.Build().Run();
    }
}
