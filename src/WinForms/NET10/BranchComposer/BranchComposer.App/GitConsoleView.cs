using WarpToolkit.WinForms.Specialized;

namespace BranchComposer.App;

public partial class GitConsoleView : UserControl
{
    public GitConsoleView()
    {
        InitializeComponent();
    }

    public ConsoleControl Console => consoleControl;
}
