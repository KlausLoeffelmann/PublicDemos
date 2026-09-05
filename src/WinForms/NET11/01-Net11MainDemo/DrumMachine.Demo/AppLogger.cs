using WinFormsDemo.Infrastructure;

namespace DrumMachine.Demo;

/// <summary>
///  Gives this demo its own identity while sharing rolling-file logging with the other demo.
/// </summary>
internal static class AppLogger
{
    private static readonly RollingFileLog s_log = new(AppPaths.LogDirectory, "drummachine");

    /// <summary>
    ///  Prepares file logging.
    /// </summary>
    public static void Initialize() => s_log.Initialize();

    /// <summary>
    ///  Writes an informational event.
    /// </summary>
    public static void Information(string category, string message)
        => s_log.Write("INFO", category, message);

    /// <summary>
    ///  Writes a recoverable preferences or file-cleanup problem with its diagnostic details.
    /// </summary>
    public static void Warning(string category, string message, Exception? exception = null)
        => s_log.Write("WARN", category, message, exception);

    /// <summary>
    ///  Writes a failure and its full exception details.
    /// </summary>
    public static void Error(string category, string message, Exception? exception = null)
        => s_log.Write("ERROR", category, message, exception);

    /// <summary>
    ///  Flushes and closes the diagnostic file.
    /// </summary>
    public static void Shutdown() => s_log.Dispose();
}
