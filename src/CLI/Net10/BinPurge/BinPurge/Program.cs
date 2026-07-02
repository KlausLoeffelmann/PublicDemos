using BinPurge;

if (!CliOptions.TryParse(args, out var options, out var error))
{
    if (error is not null)
    {
        Console.Error.WriteLine(error);
        Console.WriteLine();
        CliOptions.PrintUsage();
        return 1;
    }

    // error == null means --help was requested and usage was already printed.
    return 0;
}

return Run(options!);

static int Run(CliOptions options)
{
    long binCount = 0, objCount = 0;
    long binBytes = 0, objBytes = 0;

    foreach (var candidate in ProjectFolderScanner.Scan(options.BasePath, options.PurgeBin, options.PurgeObj))
    {
        var scanResult = FolderSizeCalculator.Scan(candidate.FolderPath);
        var kindLabel = candidate.Kind == FolderKind.Bin ? "bin" : "obj";

        // --mindate safety check: if anything inside the folder was written after --mindate,
        // treat the project as "still active" and skip deleting its output.
        if (options.MinDateUtc is { } minDateUtc && scanResult.NewestFilePath is not null
            && scanResult.NewestFileWriteTimeUtc > minDateUtc)
        {
            var newestFileLocalTime = scanResult.NewestFileWriteTimeUtc.ToLocalTime();
            var newestFileFolder = Path.GetDirectoryName(scanResult.NewestFilePath) ?? candidate.FolderPath;
            var newestFileName = Path.GetFileName(scanResult.NewestFilePath);

            Console.WriteLine(
                $"Excluded {kindLabel} in path {candidate.ProjectFolderPath} since " +
                $"{newestFileName}'s date in folder {newestFileFolder} was {newestFileLocalTime:yyyy-MM-dd HH:mm:ss}.");
            continue;
        }

        Console.WriteLine($"{kindLabel}: {SizeFormatter.Format(scanResult.TotalBytes)}  —  {candidate.ProjectFolderPath}");

        if (!options.DryRun)
        {
            try
            {
                Directory.Delete(candidate.FolderPath, recursive: true);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                Console.WriteLine($"Warning: could not delete '{candidate.FolderPath}': {ex.Message}");
                continue;
            }
        }

        if (candidate.Kind == FolderKind.Bin)
        {
            binCount++;
            binBytes += scanResult.TotalBytes;
        }
        else
        {
            objCount++;
            objBytes += scanResult.TotalBytes;
        }
    }

    var verb = options.DryRun ? "Would delete" : "Deleted";

    if (options.PurgeBin)
    {
        Console.WriteLine($"{verb} {binCount} bin folders with a total of {SizeFormatter.Format(binBytes)}.");
    }

    if (options.PurgeObj)
    {
        Console.WriteLine($"{verb} {objCount} obj folders with a total of {SizeFormatter.Format(objBytes)}.");
    }

    return 0;
}
