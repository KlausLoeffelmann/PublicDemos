using WarpToolkit.WinForms.Tooling;

namespace BranchComposer.App;

public partial class GitConsoleView : UserControl
{
    public GitConsoleView()
    {
        InitializeComponent();
    }

    public ConsoleControl Console => consoleControl;
}
