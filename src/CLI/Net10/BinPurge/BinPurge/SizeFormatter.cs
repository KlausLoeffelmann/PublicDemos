namespace BinPurge;

/// <summary>
/// Helper for turning raw byte counts into human-readable, auto-scaled strings
/// (bytes/KB/MB/GB), used both for individual folder sizes and for the final totals.
/// </summary>
internal static class SizeFormatter
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB"];

    /// <summary>
    /// Formats a byte count as the largest unit for which the value is at least 1,
    /// with up to two decimal places (e.g. "512 B", "3.42 MB").
    /// </summary>
    public static string Format(long bytes)
    {
        double value = bytes;
        var unitIndex = 0;

        // Keep dividing by 1024 while we can still represent the value as >= 1 in the next unit.
        while (value >= 1024 && unitIndex < Units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        // Bytes are always a whole number; larger units get up to two decimals.
        return unitIndex == 0
            ? $"{(long)value} {Units[unitIndex]}"
            : $"{value:0.##} {Units[unitIndex]}";
    }
}
