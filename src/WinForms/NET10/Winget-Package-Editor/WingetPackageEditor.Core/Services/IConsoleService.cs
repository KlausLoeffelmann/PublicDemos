using System.Collections.ObjectModel;

namespace WingetPackageEditor.Core.Services;

public interface IConsoleService
{
    ObservableCollection<ConsoleMessage> Messages { get; }

    void Write(ConsoleMessageKind kind, string text);
}
