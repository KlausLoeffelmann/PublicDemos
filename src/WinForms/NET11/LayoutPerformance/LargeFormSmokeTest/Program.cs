namespace LargeFormSmokeTest;

using LargeFormSmokeTest.Forms;
using LargeFormSmokeTest.Models;

/// <summary>
///  Application entry point. Sets up high-DPI, applies the persisted color theme via the new
///  color-mode API, warms the in-memory repository, and launches whatever the command line asks
///  for. The default is the lightweight overview (<see cref="MainForm"/>); pass <c>declaration</c>
///  (with optional <c>--person</c>/<c>--year</c>/<c>--count</c>/<c>--combined</c>) to open the heavy
///  <see cref="DeclarationForm"/> directly for performance analysis.
/// </summary>
internal static class Program
{
    /// <summary>The main entry point for the application.</summary>
    [STAThread]
    private static void Main(string[] args)
    {
        // The installed .NET 11 preview ships a DEBUG build of System.Windows.Forms whose internal
        // Debug.Assert calls terminate the process via Environment.FailFast. One of them is a
        // false-positive raised while DataGridView renders its column-header text
        // ("Must preserve Graphics transformation!"). Detaching the default trace listener turns
        // those framework-internal asserts into no-ops so this demo runs on the preview runtime.
        // (This affects only Debug.Assert behavior; it changes none of our own logic.)
        System.Diagnostics.Trace.Listeners.Clear();

        ApplicationConfiguration.Initialize();
        Application.SetColorMode(SystemColorMode.System);

        // Build the cross-cutting services from persisted settings and apply the saved theme
        // app-wide before the first form is created (avoids a light->dark flash).
        AppServices.Initialize();
        AppServices.Theme.Apply();

        // Touch the repository so JSON load / JIT happen before the form under test is opened.
        _ = AppServices.Repository.Persons;

        StartupOptions options = StartupOptions.Parse(args);

        Application.Run(CreateStartupForm(options));
    }

    /// <summary>Builds the first form according to the parsed command-line options.</summary>
    private static Form CreateStartupForm(StartupOptions options)
    {
        if (options.Form is StartupForm.Main)
        {
            return new MainForm();
        }

        (Person person, Declaration declaration) = Resolve(options);
        DeclarationForm form = new(person, declaration);

        // Chain stress test: when more than one form is requested, cascade the extras once the
        // primary form is up so the cost of several large forms can be measured at once.
        if (options.Count > 1)
        {
            form.Shown += (_, _) => OpenAdditional(options, form);
        }

        return form;
    }

    private static void OpenAdditional(StartupOptions options, Form owner)
    {
        for (int i = 1; i < options.Count; i++)
        {
            (Person person, Declaration declaration) = Resolve(options, i);
            DeclarationForm extra = new(person, declaration);
            extra.Show(owner);
        }
    }

    /// <summary>Resolves the payer + declaration to open, honoring the requested offset.</summary>
    private static (Person Person, Declaration Declaration) Resolve(StartupOptions options, int offset = 0)
    {
        IReadOnlyList<Person> persons = AppServices.Repository.Persons;
        int index = Math.Clamp(options.PersonIndex + offset, 0, persons.Count - 1);
        Person person = persons[index];

        Declaration declaration =
            (options.Year is { } year ? person.Declarations.FirstOrDefault(d => d.Year == year) : null)
            ?? (options.PreferCombined
                ? person.Declarations.FirstOrDefault(d => d.Obligation is TaxObligation.LohnsteuerUndEinkommensteuer)
                : null)
            ?? person.Declarations[0];

        return (person, declaration);
    }
}