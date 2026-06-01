using System.Diagnostics;
using System.Text;
using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Discovers Visual Studio installations by running <c>vswhere.exe</c> and correlating the
///  reported instances with the local data/experimental hives on disk.
/// </summary>
public sealed class LocalVisualStudioDiscoveryService : IVisualStudioDiscoveryService
{
    private readonly IConsoleService _consoleService;
    private readonly string _visualStudioLocalAppDataPath;
    private readonly string? _vsWherePathOverride;
    private readonly Func<string?>? _vsWhereOutputProvider;

    public LocalVisualStudioDiscoveryService(IConsoleService consoleService)
        : this(
            consoleService,
            DefaultLocalAppDataPath(),
            DefaultVsWherePath(),
            vsWhereOutputProvider: null)
    {
    }

    /// <summary>
    ///  Initializes a new instance for testing, allowing the hive folder location and the
    ///  raw <c>vswhere</c> output to be supplied directly.
    /// </summary>
    public LocalVisualStudioDiscoveryService(
        IConsoleService consoleService,
        string visualStudioLocalAppDataPath,
        string? vsWherePathOverride,
        Func<string?>? vsWhereOutputProvider)
    {
        _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        ArgumentException.ThrowIfNullOrWhiteSpace(visualStudioLocalAppDataPath);
        _visualStudioLocalAppDataPath = visualStudioLocalAppDataPath;
        _vsWherePathOverride = vsWherePathOverride;
        _vsWhereOutputProvider = vsWhereOutputProvider;
    }

    public IReadOnlyList<VisualStudioInstanceInfo> DiscoverInstances()
    {
        string? output = _vsWhereOutputProvider is not null
            ? _vsWhereOutputProvider()
            : RunVsWhere();

        if (string.IsNullOrWhiteSpace(output))
        {
            _consoleService.Write(ConsoleMessageKind.Warning, "No Visual Studio installations were reported by vswhere.");
            return [];
        }

        IReadOnlyList<VisualStudioHiveFolder> hiveFolders = EnumerateHiveFolders();
        IReadOnlyList<IReadOnlyDictionary<string, string>> blocks = VisualStudioDiscoveryParser.ParseBlocks(output);

        List<VisualStudioInstanceInfo> instances = [];
        foreach (IReadOnlyDictionary<string, string> block in blocks)
        {
            VisualStudioInstanceInfo? instance = VisualStudioDiscoveryParser.MapInstance(block, hiveFolders);
            if (instance is not null)
            {
                instances.Add(instance);
            }
        }

        _consoleService.Write(
            instances.Count == 0 ? ConsoleMessageKind.Warning : ConsoleMessageKind.Info,
            instances.Count == 0
                ? "No Visual Studio installations were discovered."
                : $"Discovered {instances.Count} Visual Studio installation(s).");

        return instances
            .OrderByDescending(instance => instance.Year, StringComparer.OrdinalIgnoreCase)
            .ThenBy(instance => instance.SkuComboLabel, StringComparer.OrdinalIgnoreCase)
            .ThenBy(instance => instance.Version, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private string? RunVsWhere()
    {
        string vsWherePath = _vsWherePathOverride ?? DefaultVsWherePath();
        if (!File.Exists(vsWherePath))
        {
            _consoleService.Write(ConsoleMessageKind.Warning, $"vswhere.exe was not found at '{vsWherePath}'.");
            return null;
        }

        try
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = vsWherePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            startInfo.ArgumentList.Add("-all");
            startInfo.ArgumentList.Add("-prerelease");

            _consoleService.Write(ConsoleMessageKind.Command, $"\"{vsWherePath}\" -all -prerelease");

            using Process process = new() { StartInfo = startInfo };
            if (!process.Start())
            {
                _consoleService.Write(ConsoleMessageKind.Error, "Failed to start vswhere.exe.");
                return null;
            }

            StringBuilder output = new();
            string? line;
            while ((line = process.StandardOutput.ReadLine()) is not null)
            {
                output.AppendLine(line);

                // Blank lines separate vswhere instance blocks; they are preserved in the
                // captured output for the parser but skipped here because the console service
                // rejects empty/whitespace text.
                if (!string.IsNullOrWhiteSpace(line))
                {
                    _consoleService.Write(ConsoleMessageKind.Info, line);
                }
            }

            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
            {
                _consoleService.Write(ConsoleMessageKind.Error, error.Trim());
            }

            if (process.ExitCode != 0)
            {
                _consoleService.Write(ConsoleMessageKind.Warning, $"vswhere.exe exited with code {process.ExitCode}.");
            }

            return output.ToString();
        }
        catch (Exception ex) when (ex is IOException or System.ComponentModel.Win32Exception or UnauthorizedAccessException)
        {
            _consoleService.Write(ConsoleMessageKind.Error, $"Failed to run vswhere.exe: {ex.Message}");
            return null;
        }
    }

    private IReadOnlyList<VisualStudioHiveFolder> EnumerateHiveFolders()
    {
        if (!Directory.Exists(_visualStudioLocalAppDataPath))
        {
            return [];
        }

        try
        {
            return Directory
                .EnumerateDirectories(_visualStudioLocalAppDataPath)
                .Select(path => new VisualStudioHiveFolder(Path.GetFileName(path), path))
                .ToArray();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            _consoleService.Write(ConsoleMessageKind.Warning, $"Failed to enumerate Visual Studio hive folders: {ex.Message}");
            return [];
        }
    }

    private static string DefaultLocalAppDataPath()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft",
            "VisualStudio");

    private static string DefaultVsWherePath()
    {
        string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        return Path.Combine(programFilesX86, "Microsoft Visual Studio", "Installer", "vswhere.exe");
    }
}
