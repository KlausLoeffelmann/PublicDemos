using System.Text;

namespace SplitFlap.Demo;

/// <summary>
///  Writes process diagnostics to daily rolling files in the application's AppData folder.
/// </summary>
internal static class AppLogger
{
    private const int RetentionDays = 14;
    private static readonly Lock s_sync = new();
    private static StreamWriter? s_writer;
    private static DateOnly s_writerDate;

    /// <summary>
    ///  Creates the log directory and removes expired log files.
    /// </summary>
    public static void Initialize()
    {
        lock (s_sync)
        {
            try
            {
                Directory.CreateDirectory(AppPaths.LogDirectory);
                PruneOldLogs();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Diagnostics are best-effort. The demo must remain usable when its AppData
                // location is temporarily unavailable.
            }
        }
    }

    /// <summary>
    ///  Writes an informational diagnostic.
    /// </summary>
    public static void Information(string category, string message)
        => Write("INFO", category, message, null);

    /// <summary>
    ///  Writes a warning diagnostic.
    /// </summary>
    public static void Warning(string category, string message, Exception? exception = null)
        => Write("WARN", category, message, exception);

    /// <summary>
    ///  Writes an error diagnostic.
    /// </summary>
    public static void Error(string category, string message, Exception? exception = null)
        => Write("ERROR", category, message, exception);

    /// <summary>
    ///  Writes a critical diagnostic.
    /// </summary>
    public static void Critical(string category, string message, Exception? exception = null)
        => Write("CRITICAL", category, message, exception);

    /// <summary>
    ///  Flushes and closes the active log file.
    /// </summary>
    public static void Shutdown()
    {
        lock (s_sync)
        {
            s_writer?.Dispose();
            s_writer = null;
        }
    }

    private static void Write(string level, string category, string message, Exception? exception)
    {
        string normalized = message.Replace(
            Environment.NewLine,
            Environment.NewLine + "    ",
            StringComparison.Ordinal);

        lock (s_sync)
        {
            try
            {
                EnsureWriter();
                s_writer!.WriteLine($"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} {level,-8} {category}: {normalized}");

                if (exception is not null)
                {
                    s_writer.WriteLine(exception);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Logging cannot safely report a failure to itself. Release the file so a later
                // event can retry after a transient lock or storage error.
                s_writer?.Dispose();
                s_writer = null;
            }
        }
    }

    private static void EnsureWriter()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        if (s_writer is not null && s_writerDate == today)
        {
            return;
        }

        s_writer?.Dispose();
        Directory.CreateDirectory(AppPaths.LogDirectory);

        string path = Path.Combine(AppPaths.LogDirectory, $"splitflap-{today:yyyyMMdd}.log");
        FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        s_writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
        {
            AutoFlush = true
        };
        s_writerDate = today;
    }

    private static void PruneOldLogs()
    {
        DateTime cutoff = DateTime.Now.AddDays(-RetentionDays);

        foreach (string path in Directory.EnumerateFiles(AppPaths.LogDirectory, "splitflap-*.log"))
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
                // Retention is maintenance only; one locked file must not prevent startup.
            }
        }
    }
}
