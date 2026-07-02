namespace BinPurge;

/// <summary>
/// Result of scanning a candidate bin/obj folder: its total size on disk plus the
/// most recently written file found anywhere within it (used for the --mindate check).
/// </summary>
internal readonly record struct FolderScanResult(
    long TotalBytes,
    string? NewestFilePath,
    DateTime NewestFileWriteTimeUtc);

/// <summary>
/// Walks a folder tree once to compute both its total size and the newest
/// (most recently written) file within it, so we don't have to enumerate the
/// (potentially large) bin/obj tree twice.
/// </summary>
internal static class FolderSizeCalculator
{
    public static FolderScanResult Scan(string folderPath)
    {
        long totalBytes = 0;
        string? newestFilePath = null;
        var newestWriteTimeUtc = DateTime.MinValue;

        // EnumerateFiles with AllDirectories walks the entire tree under folderPath,
        // including nested subfolders (e.g. bin\Debug\net10.0\...).
        IEnumerable<string> files;
        try
        {
            files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            Console.WriteLine($"Warning: could not enumerate '{folderPath}': {ex.Message}");
            return new FolderScanResult(0, null, DateTime.MinValue);
        }

        foreach (var file in files)
        {
            try
            {
                var info = new FileInfo(file);
                totalBytes += info.Length;

                var writeTimeUtc = info.LastWriteTimeUtc;
                if (writeTimeUtc > newestWriteTimeUtc)
                {
                    newestWriteTimeUtc = writeTimeUtc;
                    newestFilePath = file;
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or FileNotFoundException)
            {
                // Skip files we can't stat (e.g. deleted mid-scan, locked); doesn't block the rest.
                Console.WriteLine($"Warning: could not read file info for '{file}': {ex.Message}");
            }
        }

        return new FolderScanResult(totalBytes, newestFilePath, newestWriteTimeUtc);
    }
}
