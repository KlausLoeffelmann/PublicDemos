using BranchComposer.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WinForms;
using WarpToolkit.WinForms.AppServices.ServiceExtensions;
using WarpToolkit.WinForms.Github.Git;
using WarpToolkit.WinForms.Github.Services;

namespace BranchComposer.App;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = WinFormsApplication.CreateBuilder(args);

        builder
            .UseColorMode(SystemColorMode.System)
            .UseHighDpiMode(HighDpiMode.SystemAware)
            .UseVisualStyles()
            .UseTextRenderingV2()
            .UseStartupForm<MainForm>();

        builder.Services
            .AddWinFormsDialogService()
            .AddWinFormsExceptionService()
            .AddWinFormsUserSettingsService()
            .AddSingleton<GitConsoleService>()
            .AddSingleton<IGitCommandObserver, GitConsoleCommandObserver>()
            .AddGitServices()
            .AddSingleton<AppStateStore>();

        builder.Build().Run();
    }
}
