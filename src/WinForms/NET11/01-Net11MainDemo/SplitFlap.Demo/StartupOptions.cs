namespace SplitFlap.Demo;

/// <summary>
///  Identifies the deterministic smoke scenario to run after the form is shown.
/// </summary>
internal enum SmokeScenario
{
    /// <summary>
    ///  Runs the app normally without an automated scenario.
    /// </summary>
    None,

    /// <summary>
    ///  Exercises display updates, animation, jamming, and sizing.
    /// </summary>
    Display,

    /// <summary>
    ///  Initializes audio and plays a short deterministic melody.
    /// </summary>
    Sound,

    /// <summary>
    ///  Exercises both display and sound behavior.
    /// </summary>
    All
}

/// <summary>
///  Describes command-line behavior for repeatable interactive and automated runs.
/// </summary>
internal sealed record StartupOptions
{
    /// <summary>
    ///  Gets options for a normal interactive launch.
    /// </summary>
    public static StartupOptions Interactive { get; } = new();

    /// <summary>
    ///  Gets the smoke scenario selected by the command line.
    /// </summary>
    public SmokeScenario Scenario { get; private init; }

    /// <summary>
    ///  Gets the optional duration after which the form closes itself.
    /// </summary>
    public TimeSpan? RunFor { get; private init; }

    /// <summary>
    ///  Gets whether loading and saving user settings are disabled.
    /// </summary>
    public bool NoSettings { get; private init; }

    /// <summary>
    ///  Gets command-line usage text.
    /// </summary>
    public static string Usage =>
        "Usage: SplitFlap.Demo [--scenario display|sound|all] [--run-for <seconds>] [--no-settings]";

    /// <summary>
    ///  Parses command-line arguments without throwing for user input errors.
    /// </summary>
    public static bool TryParse(
        IReadOnlyList<string> args,
        out StartupOptions options,
        out string? error)
    {
        SmokeScenario scenario = SmokeScenario.None;
        TimeSpan? runFor = null;
        bool noSettings = false;

        for (int index = 0; index < args.Count; index++)
        {
            string argument = args[index];

            switch (argument.ToLowerInvariant())
            {
                case "--scenario":
                    if (!TryReadValue(args, ref index, out string? scenarioText)
                        || !Enum.TryParse(scenarioText, ignoreCase: true, out scenario)
                        || scenario is SmokeScenario.None)
                    {
                        options = Interactive;
                        error = "--scenario requires display, sound, or all.";
                        return false;
                    }

                    break;

                case "--run-for":
                    if (!TryReadValue(args, ref index, out string? secondsText)
                        || !double.TryParse(
                            secondsText,
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double seconds)
                        || !double.IsFinite(seconds)
                        || seconds <= 0
                        || seconds > 3600)
                    {
                        options = Interactive;
                        error = "--run-for requires a number of seconds greater than 0 and no more than 3600.";
                        return false;
                    }

                    runFor = TimeSpan.FromSeconds(seconds);
                    break;

                case "--no-settings":
                    noSettings = true;
                    break;

                default:
                    options = Interactive;
                    error = $"Unknown argument: {argument}";
                    return false;
            }
        }

        options = new StartupOptions
        {
            Scenario = scenario,
            RunFor = runFor,
            NoSettings = noSettings
        };
        error = null;
        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"scenario={Scenario}, runFor={RunFor?.ToString() ?? "interactive"}, noSettings={NoSettings}";

    private static bool TryReadValue(
        IReadOnlyList<string> args,
        ref int index,
        out string? value)
    {
        if (index + 1 >= args.Count || args[index + 1].StartsWith("--", StringComparison.Ordinal))
        {
            value = null;
            return false;
        }

        value = args[++index];
        return true;
    }
}
