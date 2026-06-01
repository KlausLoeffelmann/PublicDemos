using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

public interface ICatalogService
{
    AppEntry CreateDefaultApp();

    WingetPackage CreateDemoPackage();

    /// <summary>
    ///  Returns curated template entries for commonly installed developer apps. Each entry carries
    ///  its winget Id so installed apps can be matched against the catalog.
    /// </summary>
    IReadOnlyList<AppEntry> GetWellKnownApps();
}
