using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WarpClock.App;

/// <summary>
///  Daily rolling file logger with fixed 14-day retention under AppData.
/// </summary>
public sealed class RollingFileLoggerProvider : ILoggerProvider
{
    private const int RetentionDays = 14;

    private readonly ConcurrentDictionary<string, RollingFileLogger> _loggers = new();
    private readonly object _gate = new();
    private readonly AppPaths _paths;
    private StreamWriter? _writer;
    private DateOnly _activeDate;

    public RollingFileLoggerProvider(AppPaths paths)
    {
        ArgumentNullException.ThrowIfNull(paths);
        _paths = paths;
        PruneOldLogs();
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new RollingFileLogger(this, name));

    public void Dispose()
    {
        lock (_gate)
        {
            _writer?.Dispose();
            _writer = null;
        }

        _loggers.Clear();
    }

    private void WriteLogLine(string categoryName, LogLevel level, EventId eventId, string message, Exception? exception)
    {
        string line = FormatLine(categoryName, level, eventId, message, exception);

        lock (_gate)
        {
            try
            {
                EnsureWriter();
                _writer?.WriteLine(line);
                _writer?.Flush();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _writer?.Dispose();
                _writer = null;
            }
        }
    }

    private void EnsureWriter()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        if (_writer is not null && _activeDate == today)
        {
            return;
        }

        _writer?.Dispose();
        _writer = null;

        Directory.CreateDirectory(_paths.LogDirectory);
        PruneOldLogs();

        string logPath = Path.Combine(_paths.LogDirectory, $"warpclock-{today:yyyyMMdd}.log");
        var stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        _writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
        {
            AutoFlush = true,
        };
        _activeDate = today;
    }

    private void PruneOldLogs()
    {
        try
        {
            if (!Directory.Exists(_paths.LogDirectory))
            {
                return;
            }

            DateTime cutoff = DateTime.Now.AddDays(-RetentionDays);
            foreach (string file in Directory.EnumerateFiles(_paths.LogDirectory, "warpclock-*.log"))
            {
                try
                {
                    if (File.GetLastWriteTime(file) < cutoff)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }
    }

    private static string FormatLine(
        string categoryName,
        LogLevel level,
        EventId eventId,
        string message,
        Exception? exception)
    {
        string normalizedMessage = string.IsNullOrWhiteSpace(message) ? string.Empty : message.Replace(Environment.NewLine, Environment.NewLine + "    ");
        string eventText = eventId.Id == 0 ? string.Empty : $" [{eventId.Id}]";
        string text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {LevelTag(level)} {categoryName}{eventText}: {normalizedMessage}";

        if (exception is not null)
        {
            text += Environment.NewLine + exception;
        }

        return text;
    }

    private static string LevelTag(LogLevel level) => level switch
    {
        LogLevel.Trace => "trce",
        LogLevel.Debug => "dbug",
        LogLevel.Information => "info",
        LogLevel.Warning => "warn",
        LogLevel.Error => "fail",
        LogLevel.Critical => "crit",
        _ => "none",
    };

    private sealed class RollingFileLogger(RollingFileLoggerProvider provider, string categoryName) : ILogger
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
            if (message.Length == 0 && exception is null)
            {
                return;
            }

            provider.WriteLogLine(categoryName, logLevel, eventId, message, exception);
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
