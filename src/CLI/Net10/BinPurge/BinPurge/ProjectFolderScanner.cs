namespace BinPurge;

/// <summary>
/// The two kinds of build-output folders BinPurge knows how to purge.
/// </summary>
internal enum FolderKind
{
    Bin,
    Obj,
}

/// <summary>
/// A bin/obj folder that qualifies for purging: it is named "bin" or "obj" and its
/// immediate parent folder contains a .csproj or .vbproj file.
/// </summary>
internal readonly record struct CandidateFolder(FolderKind Kind, string ProjectFolderPath, string FolderPath);

/// <summary>
/// Recursively walks a directory tree looking for bin/obj folders that belong to an
/// actual MSBuild project (i.e. their immediate parent contains a .csproj or .vbproj file).
/// </summary>
internal static class ProjectFolderScanner
{
    public static IEnumerable<CandidateFolder> Scan(string basePath, bool includeBin, bool includeObj)
    {
        // Manual stack-based walk (instead of SearchOption.AllDirectories) so that once we
        // identify a bin/obj folder as a purge candidate, we don't bother descending into it -
        // its entire contents will be removed together with it anyway.
        var pending = new Stack<string>();
        pending.Push(basePath);

        while (pending.Count > 0)
        {
            var current = pending.Pop();

            string[] subDirectories;

            try
            {
                subDirectories = Directory.GetDirectories(current);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                Console.WriteLine($"Warning: could not enumerate '{current}': {ex.Message}");
                continue;
            }

            foreach (var subDirectory in subDirectories)
            {
                var folderName = Path.GetFileName(subDirectory);

                var kind = folderName.Equals("bin", StringComparison.OrdinalIgnoreCase) ? FolderKind.Bin
                    : folderName.Equals("obj", StringComparison.OrdinalIgnoreCase) ? FolderKind.Obj
                    : (FolderKind?)null;

                var kindEnabled = kind switch
                {
                    FolderKind.Bin => includeBin,
                    FolderKind.Obj => includeObj,
                    _ => false,
                };

                if (kind is not null && kindEnabled && HasProjectFile(current))
                {
                    // Candidate found - report it, but don't push it onto the stack: no need to
                    // descend further since the whole folder will be deleted as a unit.
                    yield return new CandidateFolder(kind.Value, current, subDirectory);
                    continue;
                }

                // Not a matching bin/obj folder (or its kind wasn't requested) - keep walking down.
                pending.Push(subDirectory);
            }
        }
    }

    /// <summary>
    /// Checks whether the given folder directly contains a .csproj or .vbproj file.
    /// </summary>
    private static bool HasProjectFile(string folderPath)
    {
        try
        {
            return Directory.EnumerateFiles(folderPath, "*.csproj").Any()
                || Directory.EnumerateFiles(folderPath, "*.vbproj").Any();
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            Console.WriteLine($"Warning: could not check for project file in '{folderPath}': {ex.Message}");
            return false;
        }
    }
}
