using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Stores each package as <c>{Id}.json</c> under a packages folder, with deletions backed up to a
///  sibling backups folder as <c>WPE{yyMMddHHmmss}.bak</c>. By default the root lives under
///  <c>%AppData%\Winget-Package-Editor</c>.
/// </summary>
public sealed class JsonPackageStore : IPackageStore
{
    private readonly string _packagesDirectory;
    private readonly string _backupsDirectory;
    private readonly IConsoleService? _consoleService;

    public JsonPackageStore(IConsoleService? consoleService = null)
        : this(DefaultRootDirectory(), consoleService)
    {
    }

    public JsonPackageStore(string rootDirectory, IConsoleService? consoleService = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        _packagesDirectory = Path.Combine(rootDirectory, "Packages");
        _backupsDirectory = Path.Combine(rootDirectory, "Backups");
        _consoleService = consoleService;
    }

    public static string DefaultRootDirectory()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Winget-Package-Editor");

    public IReadOnlyList<WingetPackage> LoadAll()
    {
        List<WingetPackage> packages = [];
        if (!Directory.Exists(_packagesDirectory))
        {
            return packages;
        }

        foreach (string file in Directory.EnumerateFiles(_packagesDirectory, "*.json"))
        {
            try
            {
                if (PackageJsonSerializer.Deserialize(File.ReadAllText(file)) is { } package)
                {
                    packages.Add(package);
                }
            }
            catch (Exception ex)
            {
                _consoleService?.Write(ConsoleMessageKind.Warning, $"Skipped unreadable package '{file}': {ex.Message}");
            }
        }

        return packages;
    }

    public void Save(WingetPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);

        try
        {
            Directory.CreateDirectory(_packagesDirectory);
            File.WriteAllText(GetPackagePath(package), PackageJsonSerializer.Serialize(package));
        }
        catch (Exception ex)
        {
            _consoleService?.Write(ConsoleMessageKind.Error, $"Could not save package '{package.Name}': {ex.Message}");
        }
    }

    public void Delete(WingetPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);

        try
        {
            Directory.CreateDirectory(_backupsDirectory);
            string backupName = $"WPE{DateTime.Now:yyMMddHHmmss}.bak";
            File.WriteAllText(Path.Combine(_backupsDirectory, backupName), PackageJsonSerializer.Serialize(package));

            string path = GetPackagePath(package);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            _consoleService?.Write(ConsoleMessageKind.Error, $"Could not remove package '{package.Name}': {ex.Message}");
        }
    }

    private string GetPackagePath(WingetPackage package)
    {
        string id = string.IsNullOrWhiteSpace(package.Id) ? Guid.NewGuid().ToString("N") : package.Id;
        package.Id = id;
        return Path.Combine(_packagesDirectory, $"{id}.json");
    }
}
