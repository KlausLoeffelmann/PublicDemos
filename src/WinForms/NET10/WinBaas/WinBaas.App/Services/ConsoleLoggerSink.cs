using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using WarpToolkit.WinForms.Tooling;

namespace WinBaas.Services;

/// <summary>
///  Receives <see cref="ILogger"/> messages and forwards them to the
///  attached <see cref="ConsoleControl"/>.
/// </summary>
/// <remarks>
///  <para>
///   Messages emitted before the UI is up are buffered in memory and
///   replayed once <see cref="Attach"/> is called from
///   <see cref="FrmMain"/>.
///  </para>
/// </remarks>
public sealed class ConsoleLoggerSink
{
    private readonly object _gate = new();
    private readonly ConcurrentQueue<(LogLevel Level, string Message)> _buffer = new();
    private ConsoleControl? _console;

    /// <summary>
    ///  Attaches the console control to the sink and flushes any buffered output.
    /// </summary>
    public void Attach(ConsoleControl console)
    {
        ArgumentNullException.ThrowIfNull(console);

        lock (_gate)
        {
            _console = console;

            while (_buffer.TryDequeue(out var entry))
            {
                _ = console.WriteLineAsync(entry.Message, MapChannel(entry.Level));
            }
        }
    }

    /// <summary>
    ///  Writes a log line through the console control, if attached; otherwise buffers it.
    /// </summary>
    public void Write(LogLevel level, string message)
    {
        var console = _console;
        if (console is null)
        {
            _buffer.Enqueue((level, message));
            return;
        }

        _ = console.WriteLineAsync(message, MapChannel(level));
    }

    private static ConsoleChannel MapChannel(LogLevel level) => level switch
    {
        LogLevel.Trace => ConsoleChannel.Trace,
        LogLevel.Debug => ConsoleChannel.Debug,
        LogLevel.Information => ConsoleChannel.Information,
        LogLevel.Warning => ConsoleChannel.Warning,
        LogLevel.Error => ConsoleChannel.Error,
        LogLevel.Critical => ConsoleChannel.Error,
        _ => ConsoleChannel.Default,
    };
}
