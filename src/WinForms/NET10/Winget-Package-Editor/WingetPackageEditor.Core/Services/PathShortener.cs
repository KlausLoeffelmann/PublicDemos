namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Produces compact, middle-elided representations of long file-system paths for display.
/// </summary>
public static class PathShortener
{
    /// <summary>
    ///  Shortens a path by eliding its middle, e.g.
    ///  <c>C:\Program Files\Microsoft Visual Studio\18\Insiders</c> becomes
    ///  <c>C:\Pro ... io\18\Insiders</c>.
    /// </summary>
    /// <param name="path">The path to shorten.</param>
    /// <param name="maxLength">The maximum length of the returned string.</param>
    /// <returns>The shortened path, or the original when it already fits.</returns>
    public static string Shorten(string? path, int maxLength = 28)
    {
        if (string.IsNullOrEmpty(path) || path.Length <= maxLength)
        {
            return path ?? string.Empty;
        }

        const string ellipsis = " ... ";
        int budget = maxLength - ellipsis.Length;
        if (budget <= 2)
        {
            return path[..maxLength];
        }

        int headLength = Math.Max(3, budget / 3);
        int tailLength = budget - headLength;

        string head = path[..headLength];
        string tail = path[^tailLength..];
        return $"{head}{ellipsis}{tail}";
    }
}
