namespace LargeFormSmokeTest;

using LargeFormSmokeTest.Forms;

/// <summary>
///  Application entry point. Sets up high-DPI, applies the persisted color theme via the new
///  color-mode API, warms the in-memory repository, and runs the (cheap) overview form.
/// </summary>
internal static class Program
{
    /// <summary>The main entry point for the application.</summary>
    [STAThread]
    private static void Main()
    {
        // The installed .NET 11 preview ships a DEBUG build of System.Windows.Forms whose internal
        // Debug.Assert calls terminate the process via Environment.FailFast. One of them is a
        // false-positive raised while DataGridView renders its column-header text
        // ("Must preserve Graphics transformation!"). Detaching the default trace listener turns
        // those framework-internal asserts into no-ops so this demo runs on the preview runtime.
        // (This affects only Debug.Assert behavior; it changes none of our own logic.)
        System.Diagnostics.Trace.Listeners.Clear();

        ApplicationConfiguration.Initialize();

        // Build the cross-cutting services from persisted settings and apply the saved theme
        // app-wide before the first form is created (avoids a light->dark flash).
        AppServices.Initialize();
        AppServices.Theme.Apply();

        // Touch the repository so JSON load / JIT happen before the form under test is opened.
        _ = AppServices.Repository.Persons;

        Application.Run(new MainForm());
    }
}