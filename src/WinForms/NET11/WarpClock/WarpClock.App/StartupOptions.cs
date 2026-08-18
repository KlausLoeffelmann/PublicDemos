using System.Globalization;

namespace WarpClock.App;

/// <summary>
///  Strongly typed command-line options for hosted startup and diagnostics.
/// </summary>
public sealed class StartupOptions
{
    public static StartupOptions Empty { get; } = new();

    public string? StartTheme { get; private init; }

    public bool? StartKioskMode { get; private init; }

    public bool? AlwaysOn { get; private init; }

    public bool? RecordFramerate { get; private init; }

    public int? DebugRunSeconds { get; private init; }

    public bool DontPersist { get; private init; }

    public static StartupOptions Parse(IReadOnlyList<string> args)
    {
        ArgumentNullException.ThrowIfNull(args);

        string? startTheme = null;
        bool? startKioskMode = null;
        bool? alwaysOn = null;
        bool? recordFramerate = null;
        int? debugRunSeconds = null;
        bool dontPersist = false;

        for (int i = 0; i < args.Count; i++)
        {
            if (!TrySplitOption(args[i], out string optionName, out string? inlineValue))
            {
                throw new ArgumentException($"Unexpected argument '{args[i]}'.");
            }

            switch (optionName.ToLowerInvariant())
            {
                case "starttheme":
                    startTheme = ReadRequiredValue(args, ref i, inlineValue, "StartTheme");
                    break;

                case "startkioskmode":
                    startKioskMode = ParseBoolean(
                        ReadRequiredValue(args, ref i, inlineValue, "StartKioskMode"));
                    break;

                case "alwayson":
                    alwaysOn = ParseBoolean(ReadOptionalValue(args, ref i, inlineValue, defaultIfMissing: "true"));
                    break;

                case "recordframerate":
                    recordFramerate = ParseBoolean(ReadOptionalValue(args, ref i, inlineValue, defaultIfMissing: "true"));
                    break;

                case "debugrun":
                    debugRunSeconds = ParseDebugRunSeconds(
                        ReadRequiredValue(args, ref i, inlineValue, "DebugRun"));
                    break;

                case "dontpersist":
                    dontPersist = ParseBoolean(ReadOptionalValue(args, ref i, inlineValue, defaultIfMissing: "true"));
                    break;

                case "?":
                case "h":
                case "help":
                    throw new ArgumentException(GetUsageText());

                default:
                    throw new ArgumentException(
                        $"Unknown option '{args[i]}'.{Environment.NewLine}{Environment.NewLine}{GetUsageText()}");
            }
        }

        return new StartupOptions
        {
            StartTheme = startTheme,
            StartKioskMode = startKioskMode,
            AlwaysOn = alwaysOn,
            RecordFramerate = recordFramerate,
            DebugRunSeconds = debugRunSeconds,
            DontPersist = dontPersist,
        };
    }

    public static string GetUsageText()
        => "Supported options:" + Environment.NewLine
         + "  --StartTheme <family-or-variant name>" + Environment.NewLine
        + "  --StartKioskMode <true|false>" + Environment.NewLine
         + "  --AlwaysOn [true|false]" + Environment.NewLine
         + "  --RecordFramerate [true|false]" + Environment.NewLine
         + "  --DebugRun <1-15>" + Environment.NewLine
         + "  --DontPersist [true|false]";

    private static bool TrySplitOption(string raw, out string optionName, out string? inlineValue)
    {
        optionName = string.Empty;
        inlineValue = null;

        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        string token;
        if (raw.StartsWith("--", StringComparison.Ordinal))
        {
            token = raw[2..];
        }
        else if (raw.StartsWith("/", StringComparison.Ordinal) || raw.StartsWith("-", StringComparison.Ordinal))
        {
            token = raw[1..];
        }
        else
        {
            return false;
        }

        if (token.Length == 0)
        {
            return false;
        }

        int separatorIndex = token.IndexOfAny(['=', ':']);
        if (separatorIndex >= 0)
        {
            optionName = token[..separatorIndex];
            inlineValue = token[(separatorIndex + 1)..];
            return optionName.Length > 0;
        }

        optionName = token;
        return true;
    }

    private static string ReadRequiredValue(
        IReadOnlyList<string> args,
        ref int index,
        string? inlineValue,
        string optionName)
    {
        string value = ReadOptionalValue(args, ref index, inlineValue, defaultIfMissing: string.Empty);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"Option '{optionName}' requires a value.{Environment.NewLine}{Environment.NewLine}{GetUsageText()}");
        }

        return value;
    }

    private static string ReadOptionalValue(
        IReadOnlyList<string> args,
        ref int index,
        string? inlineValue,
        string defaultIfMissing)
    {
        if (inlineValue is not null)
        {
            return inlineValue;
        }

        if (index + 1 >= args.Count || LooksLikeOption(args[index + 1]))
        {
            return defaultIfMissing;
        }

        index++;
        return args[index];
    }

    private static bool LooksLikeOption(string token)
        => !string.IsNullOrWhiteSpace(token)
            && (token.StartsWith("--", StringComparison.Ordinal)
                || token.StartsWith("/", StringComparison.Ordinal)
                || token.StartsWith("-", StringComparison.Ordinal));

    private static bool ParseBoolean(string raw)
        => raw.Trim().ToLowerInvariant() switch
        {
            "1" or "true" or "yes" or "on" => true,
            "0" or "false" or "no" or "off" => false,
            _ => throw new ArgumentException(
                $"Invalid boolean value '{raw}'. Expected true/false, yes/no, on/off, or 1/0."),
        };

    private static int ParseDebugRunSeconds(string raw)
    {
        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
            || value is < 1 or > 15)
        {
            throw new ArgumentException("DebugRun must be an integer from 1 to 15 seconds.");
        }

        return value;
    }
}
