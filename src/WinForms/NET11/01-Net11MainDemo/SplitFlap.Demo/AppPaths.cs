namespace SplitFlap.Demo;

/// <summary>
///  Provides the per-user paths used by the application.
/// </summary>
internal static class AppPaths
{
    private const string ProductFolderName = "SplitFlap.Demo";

    /// <summary>
    ///  Gets the root folder under the current user's local application data.
    /// </summary>
    public static string DataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ProductFolderName);

    /// <summary>
    ///  Gets the folder containing rolling diagnostic logs.
    /// </summary>
    public static string LogDirectory { get; } = Path.Combine(DataDirectory, "Logs");

    /// <summary>
    ///  Gets the JSON settings file path.
    /// </summary>
    public static string SettingsFile { get; } = Path.Combine(DataDirectory, "settings.json");
}
