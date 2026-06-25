namespace LargeFormSmokeTest;

using LargeFormSmokeTest.Data;
using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Settings;
using LargeFormSmokeTest.Theming;

/// <summary>
///  Tiny process-wide service holder. The demo deliberately avoids a full DI container; these
///  few cross-cutting services (data, localization, theming, persisted settings) are created
///  once in <see cref="Initialize"/> and consumed by every form.
/// </summary>
public static class AppServices
{
    /// <summary>Gets the persisted user settings (language + theme).</summary>
    public static AppSettings Settings { get; private set; } = new();

    /// <summary>Gets the UI-chrome localizer.</summary>
    public static ILocalizer Localizer { get; private set; } = new Localizer();

    /// <summary>Gets the app-wide theme coordinator.</summary>
    public static ThemeManager Theme { get; private set; } = new();

    /// <summary>Gets the in-memory data repository.</summary>
    public static TaxRepository Repository
        => TaxRepository.Instance;

    /// <summary>
    ///  Creates the services from persisted settings. Must run once at startup before any form
    ///  is shown so the chosen language and theme are honored from the first paint.
    /// </summary>
    public static void Initialize()
    {
        Settings = AppSettings.Load();
        Localizer = new Localizer(Settings.Language);
        Theme = new ThemeManager();

        // Persist the choices whenever the user flips language or theme at runtime.
        Localizer.LanguageChanged += static (_, _) =>
        {
            Settings.Language = Localizer.Language;
            Settings.Save();
        };

        Theme.ThemeChanged += static (_, _) =>
        {
            Settings.Theme = Theme.Theme;
            Settings.Save();
        };
    }
}
