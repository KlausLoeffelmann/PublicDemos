namespace DrumMachine.Demo;

/// <summary>
///  Keeps rhythm-demo diagnostics separate from the split-flap application's files.
/// </summary>
internal static class AppPaths
{
    /// <summary>
    ///  Gets the new demo's per-user diagnostic directory.
    /// </summary>
    public static string LogDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DrumMachine.Demo",
        "Logs");
}
