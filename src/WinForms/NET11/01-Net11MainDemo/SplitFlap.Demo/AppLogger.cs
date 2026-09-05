using WinFormsDemo.Infrastructure;

namespace SplitFlap.Demo;

/// <summary>
///  Writes process diagnostics to daily rolling files in the application's AppData folder.
/// </summary>
internal static class AppLogger
{
    private static readonly RollingFileLog s_log = new(AppPaths.LogDirectory, "splitflap");

    /// <summary>
    ///  Creates the log directory and removes expired log files.
    /// </summary>
    public static void Initialize()
        => s_log.Initialize();

    /// <summary>
    ///  Writes an informational diagnostic.
    /// </summary>
    public static void Information(string category, string message)
        => s_log.Write("INFO", category, message);

    /// <summary>
    ///  Writes a warning diagnostic.
    /// </summary>
    public static void Warning(string category, string message, Exception? exception = null)
        => s_log.Write("WARN", category, message, exception);

    /// <summary>
    ///  Writes an error diagnostic.
    /// </summary>
    public static void Error(string category, string message, Exception? exception = null)
        => s_log.Write("ERROR", category, message, exception);

    /// <summary>
    ///  Writes a critical diagnostic.
    /// </summary>
    public static void Critical(string category, string message, Exception? exception = null)
        => s_log.Write("CRITICAL", category, message, exception);

    /// <summary>
    ///  Flushes and closes the active log file.
    /// </summary>
    public static void Shutdown()
        => s_log.Dispose();
}
