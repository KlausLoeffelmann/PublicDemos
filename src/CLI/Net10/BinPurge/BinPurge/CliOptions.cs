using System.Globalization;

namespace BinPurge;

/// <summary>
/// Parsed, validated command-line options for BinPurge.
/// </summary>
internal sealed class CliOptions
{
    public bool PurgeBin { get; private init; }
    public bool PurgeObj { get; private init; }
    public required string BasePath { get; init; }
    public DateTime? MinDateUtc { get; private init; }
    public bool DryRun { get; private init; }

    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Attempts to parse the raw command-line arguments into a validated <see cref="CliOptions"/>.
    /// Returns false (and prints usage/error info) if the arguments are missing, unknown, or invalid.
    /// </summary>
    public static bool TryParse(string[] args, out CliOptions? options, out string? error)
    {
        options = null;
        error = null;

        var purgeBin = false;
        var purgeObj = false;
        var dryRun = false;
        string? basePathArg = null;
        string? minDateArg = null;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--bin":
                    purgeBin = true;
                    break;

                case "--obj":
                    purgeObj = true;
                    break;

                case "--dry-run":
                    dryRun = true;
                    break;

                case "--basepath":
                    if (!TryTakeValue(args, ref i, "--basepath", out basePathArg, out error))
                    {
                        return false;
                    }
                    break;

                case "--mindate":
                    if (!TryTakeValue(args, ref i, "--mindate", out minDateArg, out error))
                    {
                        return false;
                    }
                    break;

                case "-h":
                case "--help":
                    error = null;
                    PrintUsage();
                    return false;

                default:
                    error = $"Unknown argument: '{args[i]}'.";
                    return false;
            }
        }

        if (!purgeBin && !purgeObj)
        {
            error = "At least one of --bin or --obj must be specified.";
            return false;
        }

        // Resolve base path: default to current directory, otherwise resolve relative/absolute paths.
        var basePath = string.IsNullOrWhiteSpace(basePathArg)
            ? Environment.CurrentDirectory
            : Path.GetFullPath(basePathArg, Environment.CurrentDirectory);

        if (!Directory.Exists(basePath))
        {
            error = $"Base path '{basePath}' does not exist.";
            return false;
        }

        DateTime? minDateUtc = null;
        if (minDateArg is not null)
        {
            if (!DateTime.TryParseExact(
                    minDateArg,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedMinDate))
            {
                error = $"Invalid --mindate value '{minDateArg}'. Expected format: {DateFormat}.";
                return false;
            }

            minDateUtc = DateTime.SpecifyKind(parsedMinDate, DateTimeKind.Local).ToUniversalTime();
        }

        options = new CliOptions
        {
            PurgeBin = purgeBin,
            PurgeObj = purgeObj,
            BasePath = basePath,
            MinDateUtc = minDateUtc,
            DryRun = dryRun,
        };

        return true;
    }

    private static bool TryTakeValue(
        string[] args, ref int i, string argName, out string? value, out string? error)
    {
        if (i + 1 >= args.Length)
        {
            value = null;
            error = $"Missing value for '{argName}'.";
            return false;
        }

        i++;
        value = args[i];
        error = null;
        return true;
    }

    public static void PrintUsage()
    {
        Console.WriteLine("""
            BinPurge - recursively deletes bin/obj folders belonging to .csproj/.vbproj projects.

            Usage:
              BinPurge (--bin | --obj) [--basepath <path>] [--mindate yyyy-MM-dd] [--dry-run]

            Options:
              --bin                 Purge 'bin' folders.
              --obj                 Purge 'obj' folders.
              --basepath <path>     Root folder to search (default: current directory).
              --mindate yyyy-MM-dd  Skip (exclude) any bin/obj folder that contains a file
                                    newer than this date.
              --dry-run             Report what would be deleted without deleting anything.

            At least one of --bin or --obj is required.
            """);
    }
}
