using System.Diagnostics;
using System.Text;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Discovers installed winget package Ids by running <c>winget list</c> and parsing its tabular
///  output. All output is streamed to the console; failures are non-fatal and yield an empty result.
/// </summary>
public sealed class WingetListScanner(IConsoleService consoleService) 
    : IInstalledAppScanner
{
    private readonly IConsoleService _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));

    public IReadOnlyList<string> GetInstalledWingetIds()
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "winget",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        foreach (string argument in new[] { "list", "--disable-interactivity" })
        {
            startInfo.ArgumentList.Add(argument);
        }

        StringBuilder output = new();

        try
        {
            _consoleService.Write(ConsoleMessageKind.Command, "Running 'winget list'...");
            using Process process = new() { StartInfo = startInfo };
            process.OutputDataReceived += (_, args) =>
            {
                if (args.Data is not null)
                {
                    output.AppendLine(args.Data);
                    _consoleService.Write(ConsoleMessageKind.Info, args.Data);
                }
            };
            process.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    _consoleService.Write(ConsoleMessageKind.Error, args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            _consoleService.Write(ConsoleMessageKind.Error, $"'winget list' failed: {ex.Message}");
            return [];
        }

        return ParseIds(output.ToString());
    }

    /// <summary>
    ///  Parses the Id column out of <c>winget list</c> output. The Id column spans from the start of
    ///  the "Id" header to the start of the following header.
    /// </summary>
    public static IReadOnlyList<string> ParseIds(string output)
    {
        if (string.IsNullOrWhiteSpace(output))
        {
            return [];
        }

        string[] lines = output.Replace("\r\n", "\n").Split('\n');

        int headerIndex = Array.FindIndex(lines, IsHeader);
        if (headerIndex < 0)
        {
            return [];
        }

        string header = lines[headerIndex];
        int idStart = header.IndexOf("Id", StringComparison.Ordinal);
        if (idStart < 0)
        {
            return [];
        }

        int idEnd = FindNextColumnStart(header, idStart);

        List<string> ids = [];
        for (int i = headerIndex + 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('-'))
            {
                continue;
            }

            if (idStart >= line.Length)
            {
                continue;
            }

            int length = Math.Min(idEnd, line.Length) - idStart;
            string id = line.Substring(idStart, Math.Max(0, length)).Trim();
            if (id.Length > 0)
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    private static bool IsHeader(string line)
        => line.Contains("Id", StringComparison.Ordinal)
            && line.Contains("Version", StringComparison.Ordinal)
            && line.Contains("Name", StringComparison.Ordinal);

    private static int FindNextColumnStart(string header, int fromColumn)
    {
        // Columns are separated by two-or-more spaces; the next column starts after the gap.
        int gap = header.IndexOf("  ", fromColumn, StringComparison.Ordinal);
        if (gap < 0)
        {
            return header.Length;
        }

        int next = gap;
        while (next < header.Length && header[next] == ' ')
        {
            next++;
        }

        return next < header.Length ? next : header.Length;
    }
}
