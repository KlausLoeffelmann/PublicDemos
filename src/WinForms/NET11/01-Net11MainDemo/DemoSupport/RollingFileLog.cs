using System.Diagnostics;
using System.Text;

namespace WinFormsDemo.Infrastructure;

/// <summary>
///  Shares daily AppData logging between demos without introducing a hosting framework.
/// </summary>
internal sealed class RollingFileLog : IDisposable
{
    private const int RetentionDays = 14;
    private readonly string _directory;
    private readonly string _prefix;
    private readonly Lock _sync = new();
    private StreamWriter? _writer;
    private DateOnly _writerDate;

    /// <summary>
    ///  Configures a logger for one application's folder and filename prefix.
    /// </summary>
    internal RollingFileLog(string directory, string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        _directory = directory;
        _prefix = prefix;
    }

    /// <summary>
    ///  Prepares the folder and removes expired logs.
    /// </summary>
    internal void Initialize()
    {
        lock (_sync)
        {
            try
            {
                Directory.CreateDirectory(_directory);
                DateTime cutoff = DateTime.Now.AddDays(-RetentionDays);
                foreach (string path in Directory.EnumerateFiles(_directory, $"{_prefix}-*.log"))
                {
                    try
                    {
                        if (File.GetLastWriteTime(path) < cutoff)
                        {
                            File.Delete(path);
                        }
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        Trace.TraceWarning($"Could not remove an expired demo log: {ex}");
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Trace.TraceError($"Could not initialize demo file logging: {ex}");
            }
        }
    }

    /// <summary>
    ///  Writes a timestamped diagnostic, retaining exception details.
    /// </summary>
    internal void Write(string level, string category, string message, Exception? exception = null)
    {
        string normalized = message.Replace(
            Environment.NewLine, Environment.NewLine + "    ", StringComparison.Ordinal);

        lock (_sync)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                if (_writer is null || _writerDate != today)
                {
                    CloseWriter();
                    Directory.CreateDirectory(_directory);
                    string path = Path.Combine(_directory, $"{_prefix}-{today:yyyyMMdd}.log");
                    FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    _writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                    _writerDate = today;
                }

                _writer.WriteLine($"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} {level,-8} {category}: {normalized}");
                if (exception is not null)
                {
                    _writer.WriteLine(exception);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // A failed logger cannot log to itself. Preserve the diagnostic in the process
                // trace and allow the next write to retry a transient file-access failure.
                Trace.TraceError($"{level} {category}: {normalized}{Environment.NewLine}{exception}{Environment.NewLine}File logging failed: {ex}");
                CloseWriter();
            }
        }
    }

    /// <summary>
    ///  Flushes and closes the current file.
    /// </summary>
    public void Dispose()
    {
        lock (_sync)
        {
            CloseWriter();
        }
    }

    private void CloseWriter()
    {
        StreamWriter? writer = _writer;
        _writer = null;
        try
        {
            writer?.Dispose();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Trace.TraceError($"Could not close the demo log: {ex}");
        }
    }
}
