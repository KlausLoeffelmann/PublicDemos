using WarpToolkit.WinForms.Specialized;

namespace BranchComposer.App.Services;

public sealed class GitConsoleService
{
    private readonly object _syncRoot = new();
    private readonly Queue<ConsoleEntry> _pendingEntries = new();
    private ConsoleControl? _console;

    public async Task AttachAsync(ConsoleControl console)
    {
        ArgumentNullException.ThrowIfNull(console);

        ConsoleEntry[] pendingEntries;
        lock (_syncRoot)
        {
            _console = console;
            pendingEntries = _pendingEntries.ToArray();
            _pendingEntries.Clear();
        }

        foreach (ConsoleEntry entry in pendingEntries)
        {
            await WriteToConsoleAsync(console, entry).ConfigureAwait(false);
        }
    }

    public void Detach(ConsoleControl console)
    {
        ArgumentNullException.ThrowIfNull(console);

        lock (_syncRoot)
        {
            if (ReferenceEquals(_console, console))
            {
                _console = null;
            }
        }
    }

    public async ValueTask WriteAsync(
        string text,
        ConsoleMessageKind kind = ConsoleMessageKind.Standard,
        bool includeTimestamp = true)
    {
        ArgumentNullException.ThrowIfNull(text);

        ConsoleEntry entry = new(text, kind, includeTimestamp);
        ConsoleControl? console;
        lock (_syncRoot)
        {
            console = _console;
            if (console is null)
            {
                _pendingEntries.Enqueue(entry);
                return;
            }
        }

        await WriteToConsoleAsync(console, entry).ConfigureAwait(false);
    }

    private static Task WriteToConsoleAsync(ConsoleControl console, ConsoleEntry entry)
        => console.WriteMessageAsync(entry.Text, entry.Kind, entry.IncludeTimestamp);

    private sealed record ConsoleEntry(string Text, ConsoleMessageKind Kind, bool IncludeTimestamp);
}
