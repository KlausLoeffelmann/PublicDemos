using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

public interface ICatalogService
{
    AppEntry CreateDefaultApp();

    WingetPackage CreateDemoPackage();
}
