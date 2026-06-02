namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Scans the machine for installed applications and reports their winget package Ids.
/// </summary>
public interface IInstalledAppScanner
{
    /// <summary>
    ///  Returns the winget Ids of installed apps. Implementations stream tool output to the console
    ///  and never throw when winget is unavailable.
    /// </summary>
    IReadOnlyList<string> GetInstalledWingetIds();
}
