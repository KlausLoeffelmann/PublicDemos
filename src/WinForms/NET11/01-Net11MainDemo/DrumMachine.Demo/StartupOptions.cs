using System.Globalization;

namespace DrumMachine.Demo;

/// <summary>
///  Selects a bounded integration scenario instead of ordinary interactive use.
/// </summary>
internal enum DemoScenario
{
    /// <summary>
    ///  Runs the interactive editor.
    /// </summary>
    None,
    /// <summary>
    ///  Auditions the complete percussion palette.
    /// </summary>
    Kit,
    /// <summary>
    ///  Plays the original score once.
    /// </summary>
    Score,
    /// <summary>
    ///  Confirms that a known output tone reaches the spectrum analyzer.
    /// </summary>
    Spectrum,
    /// <summary>
    ///  Exercises document storage, history, views, and pause/reset without user dialogs.
    /// </summary>
    Document,
    /// <summary>
    ///  Runs all three scenarios.
    /// </summary>
    All
}

/// <summary>
///  Parses repeatable launches without changing the interactive score or writing settings.
/// </summary>
internal sealed record StartupOptions
{
    /// <summary>
    ///  Gets the default interactive launch.
    /// </summary>
    public static StartupOptions Interactive { get; } = new();

    /// <summary>
    ///  Gets the selected integration scenario.
    /// </summary>
    public DemoScenario Scenario { get; private init; }

    /// <summary>
    ///  Gets the optional automatic-close deadline.
    /// </summary>
    public TimeSpan? RunFor { get; private init; }

    /// <summary>
    ///  Gets whether the caller requested command-line help.
    /// </summary>
    public bool ShowHelp { get; private init; }

    /// <summary>
    ///  Gets whether the launch bypasses application preferences and recent-file persistence.
    /// </summary>
    public bool NoSettings { get; private init; }

    /// <summary>
    ///  Gets the supported command-line syntax.
    /// </summary>
    public static string Usage =>
        "Usage: DrumMachine.Demo [--scenario kit|score|spectrum|document|all] [--run-for <seconds>] [--no-settings] [--help]";

    /// <summary>
    ///  Parses arguments strictly, reporting invalid input rather than silently choosing defaults.
    /// </summary>
    public static bool TryParse(IReadOnlyList<string> args, out StartupOptions options, out string? error)
    {
        DemoScenario scenario = DemoScenario.None;
        TimeSpan? runFor = null;
        bool help = false;
        bool noSettings = false;

        for (int index = 0; index < args.Count; index++)
        {
            switch (args[index].ToLowerInvariant())
            {
                case "--help":
                    help = true;
                    break;
                case "--no-settings":
                    noSettings = true;
                    break;
                case "--scenario":
                    if (++index >= args.Count || !TryScenario(args[index], out scenario))
                    {
                        options = Interactive;
                        error = "--scenario requires kit, score, spectrum, document, or all.";
                        return false;
                    }

                    break;
                case "--run-for":
                    if (++index >= args.Count
                        || !double.TryParse(args[index], NumberStyles.Float, CultureInfo.InvariantCulture, out double seconds)
                        || !double.IsFinite(seconds) || seconds <= 0 || seconds > 3600)
                    {
                        options = Interactive;
                        error = "--run-for requires a number greater than zero and no more than 3600 seconds.";
                        return false;
                    }

                    runFor = TimeSpan.FromSeconds(seconds);
                    break;
                default:
                    options = Interactive;
                    error = $"Unknown argument: {args[index]}";
                    return false;
            }
        }

        options = new StartupOptions { Scenario = scenario, RunFor = runFor, ShowHelp = help, NoSettings = noSettings };
        error = null;
        return true;
    }

    private static bool TryScenario(string value, out DemoScenario scenario)
    {
        scenario = value.ToLowerInvariant() switch
        {
            "kit" => DemoScenario.Kit,
            "score" => DemoScenario.Score,
            "spectrum" => DemoScenario.Spectrum,
            "document" => DemoScenario.Document,
            "all" => DemoScenario.All,
            _ => DemoScenario.None
        };
        return scenario != DemoScenario.None;
    }
}
