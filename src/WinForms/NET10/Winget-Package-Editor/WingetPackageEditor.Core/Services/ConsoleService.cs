using System.Collections.ObjectModel;

namespace WingetPackageEditor.Core.Services;

public sealed class ConsoleService : IConsoleService
{
    public ObservableCollection<ConsoleMessage> Messages { get; } = [];

    public void Write(ConsoleMessageKind kind, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Messages.Add(new ConsoleMessage(DateTimeOffset.Now, kind, text));
    }
}
