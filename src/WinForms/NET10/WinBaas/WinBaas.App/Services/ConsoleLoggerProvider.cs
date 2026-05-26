using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace WinBaas.Services;

/// <summary>
///  <see cref="ILoggerProvider"/> that forwards log messages to a
///  <see cref="ConsoleLoggerSink"/> so they get colorized in the
///  WinBaas Console tab.
/// </summary>
public sealed class ConsoleLoggerProvider(ConsoleLoggerSink sink) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new();
    private readonly ConsoleLoggerSink _sink = sink;

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new ConsoleLogger(_sink, name));

    /// <inheritdoc />
    public void Dispose() => _loggers.Clear();

    private sealed class ConsoleLogger(ConsoleLoggerSink sink, string category) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = formatter(state, exception);
            if (exception is not null)
            {
                message = string.IsNullOrEmpty(message)
                    ? exception.ToString()
                    : $"{message}{Environment.NewLine}{exception}";
            }

            sink.Write(logLevel, $"[{DateTime.Now:HH:mm:ss}] {LevelTag(logLevel)} {category}: {message}");
        }

        private static string LevelTag(LogLevel level) => level switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => "    ",
        };

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
