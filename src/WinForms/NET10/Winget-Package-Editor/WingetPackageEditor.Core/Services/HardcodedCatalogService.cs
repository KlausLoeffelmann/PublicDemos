using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

public sealed class HardcodedCatalogService : ICatalogService
{
    public AppEntry CreateDefaultApp()
    {
        return new GenericAppEntry
        {
            Id = "Microsoft.PowerShell",
            DisplayName = "PowerShell 7",
            Action = AppAction.Ensure,
            Source = AppSource.Winget,
            Scope = AppScope.Machine
        };
    }

    public WingetPackage CreateDemoPackage()
    {
        return new WingetPackage
        {
            Name = "Developer Workstation",
            Description = "V0 demo package for exercising MVVM bindings.",
            Author = Environment.UserName,
            Apps =
            [
                CreateDefaultApp(),
                new VSCodeEntry
                {
                    Id = "Microsoft.VisualStudioCode",
                    DisplayName = "Visual Studio Code",
                    Extensions =
                    [
                        "ms-dotnettools.csharp",
                        "github.copilot",
                        "github.vscode-github-actions"
                    ]
                },
                new VisualStudioEntry
                {
                    Id = "Microsoft.VisualStudio.2022.Professional",
                    DisplayName = "Visual Studio 2022 Professional",
                    Edition = VSEdition.Professional,
                    Channel = VSChannel.Release,
                    VSConfigInline = """
                        {
                          "version": "1.0",
                          "components": []
                        }
                        """,
                    Extensions =
                    [
                        new VsixReference { Identifier = "GitHub.copilotvs" },
                        new VsixReference { Identifier = "VisualStudioExptTeam.VSColorOutput64" }
                    ]
                }
            ]
        };
    }
}
