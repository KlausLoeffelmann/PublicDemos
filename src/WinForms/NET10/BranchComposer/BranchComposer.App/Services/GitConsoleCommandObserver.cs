using System.Globalization;
using WarpToolkit.WinForms.Github.Git;
using WarpToolkit.WinForms.Tooling;

namespace BranchComposer.App.Services;

public sealed class GitConsoleCommandObserver(GitConsoleService consoleService) : IGitCommandObserver
{
    public ValueTask CommandStartingAsync(
        GitCommandStartingEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        return consoleService.WriteAsync(
            $"git {string.Join(' ', eventArgs.Arguments)}",
            ConsoleMessageKind.Command);
    }

    public ValueTask StandardOutputReceivedAsync(
        GitCommandOutputEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        return string.IsNullOrWhiteSpace(eventArgs.Text)
            ? ValueTask.CompletedTask
            : consoleService.WriteAsync(eventArgs.Text, ConsoleMessageKind.Output, includeTimestamp: false);
    }

    public ValueTask StandardErrorReceivedAsync(
        GitCommandOutputEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        return string.IsNullOrWhiteSpace(eventArgs.Text)
            ? ValueTask.CompletedTask
            : consoleService.WriteAsync(eventArgs.Text, ConsoleMessageKind.Error, includeTimestamp: false);
    }

    public ValueTask CommandCompletedAsync(
        GitCommandCompletedEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        ConsoleMessageKind kind = eventArgs.ExitCode == 0
            ? ConsoleMessageKind.Success
            : ConsoleMessageKind.Warning;

        return consoleService.WriteAsync(
            $"git exited with code {eventArgs.ExitCode} after {eventArgs.Duration.TotalMilliseconds.ToString("N0", CultureInfo.CurrentCulture)} ms.",
            kind);
    }
}
