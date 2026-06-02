namespace WingetPackageEditor.Core.Services;

public sealed record ConsoleMessage(
    DateTimeOffset Timestamp,
    ConsoleMessageKind Kind,
    string Text);

public enum ConsoleMessageKind
{
    Info,
    Warning,
    Error,
    Command,
    Debug
}
