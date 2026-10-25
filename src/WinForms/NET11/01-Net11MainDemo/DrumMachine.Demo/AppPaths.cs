namespace DrumMachine.Demo;

/// <summary>
///  Keeps rhythm-demo preferences and diagnostics separate from the split-flap application's files.
/// </summary>
internal static class AppPaths
{
    /// <summary>
    ///  Gets the rhythm demo's per-user preferences file, independently of saved loop documents.
    /// </summary>
    public static string SettingsFile { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DrumMachine.Demo",
        "settings.json");

    /// <summary>
    ///  Gets the new demo's per-user diagnostic directory.
    /// </summary>
    public static string LogDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DrumMachine.Demo",
        "Logs");
}
