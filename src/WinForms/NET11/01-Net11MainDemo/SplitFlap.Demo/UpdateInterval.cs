namespace SplitFlap.Demo;

/// <summary>
///  Defines the supported timetable refresh interval.
/// </summary>
internal static class UpdateInterval
{
    /// <summary>
    ///  Gets the minimum interval in seconds.
    /// </summary>
    public const int MinimumSeconds = 10;

    /// <summary>
    ///  Gets the maximum interval in seconds.
    /// </summary>
    public const int MaximumSeconds = 300;

    /// <summary>
    ///  Gets the distance between adjacent slider values in seconds.
    /// </summary>
    public const int StepSeconds = 10;

    /// <summary>
    ///  Clamps an interval and rounds it to the nearest supported ten-second step.
    /// </summary>
    public static int Normalize(int seconds)
        => Math.Clamp(
            ((seconds + (StepSeconds / 2)) / StepSeconds) * StepSeconds,
            MinimumSeconds,
            MaximumSeconds);
}
