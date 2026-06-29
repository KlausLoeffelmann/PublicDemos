namespace LargeFormSmokeTest;

using LargeFormSmokeTest.Models;

/// <summary>
///  Parsed command-line options for the performance harness. The app is built purely for
///  layout/startup profiling, so the command line can launch a specific form (or a chain of
///  forms) directly — bypassing the overview — to make traces deterministic and repeatable.
/// </summary>
/// <remarks>
///  Supported syntax (case-insensitive):
///  <list type="bullet">
///   <item><description><c>main</c> — open the lightweight overview form (default).</description></item>
///   <item><description><c>declaration</c> / <c>decl</c> — open the heavy DeclarationForm directly.</description></item>
///   <item><description><c>--person N</c> — zero-based payer index.</description></item>
///   <item><description><c>--year N</c> — tax year, otherwise the payer's first declaration.</description></item>
///   <item><description><c>--count N</c> — open N heavy forms at once (form-chain stress).</description></item>
///   <item><description><c>--combined</c> — prefer a payer whose return needs both blocks.</description></item>
///  </list>
/// </remarks>
internal sealed class StartupOptions
{
    /// <summary>Which form to launch on startup (default: the overview MainForm).</summary>
    public StartupForm Form { get; private init; } = StartupForm.Main;

    /// <summary>Zero-based payer index to open.</summary>
    public int PersonIndex { get; private init; }

    /// <summary>Optional tax year; falls back to the payer's first declaration when null.</summary>
    public int? Year { get; private init; }

    /// <summary>How many declaration forms to open at once (chain stress test).</summary>
    public int Count { get; private init; } = 1;

    /// <summary>When true, prefers a payer/year whose obligation builds both title blocks.</summary>
    public bool PreferCombined { get; private init; }

    /// <summary>Parses the process arguments into a <see cref="StartupOptions"/> instance.</summary>
    public static StartupOptions Parse(string[] args)
    {
        StartupForm form = StartupForm.Main;
        int person = 0;
        int? year = null;
        int count = 1;
        bool combined = false;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLowerInvariant();

            switch (arg)
            {
                case "main":
                    form = StartupForm.Main;
                    break;
                case "declaration" or "decl":
                    form = StartupForm.Declaration;
                    break;
                case "--person" when TryNext(args, ref i, out int p):
                    person = p;
                    break;
                case "--year" when TryNext(args, ref i, out int y):
                    year = y;
                    break;
                case "--count" when TryNext(args, ref i, out int c):
                    count = c < 1 ? 1 : c;
                    break;
                case "--combined":
                    combined = true;
                    break;
            }
        }

        return new StartupOptions
        {
            Form = form,
            PersonIndex = person,
            Year = year,
            Count = count,
            PreferCombined = combined
        };
    }

    private static bool TryNext(string[] args, ref int i, out int value)
    {
        value = 0;

        return i + 1 < args.Length && int.TryParse(args[++i], out value);
    }
}

/// <summary>The form the harness opens on startup.</summary>
internal enum StartupForm
{
    /// <summary>The lightweight overview form.</summary>
    Main,

    /// <summary>The heavy form under test.</summary>
    Declaration
}
