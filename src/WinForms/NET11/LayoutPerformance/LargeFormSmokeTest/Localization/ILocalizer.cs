namespace LargeFormSmokeTest.Localization;

/// <summary>
///  Provides localized UI-chrome strings and raises an event whenever the active language
///  changes so that open forms can re-apply their captions live.
/// </summary>
public interface ILocalizer
{
    /// <summary>Gets or sets the active UI language.</summary>
    AppLanguage Language { get; set; }

    /// <summary>Raised after <see cref="Language"/> changed; handlers should re-localize.</summary>
    event EventHandler? LanguageChanged;

    /// <summary>
    ///  Returns the localized string for <paramref name="key"/>; falls back to the key itself
    ///  when no translation exists (makes missing keys obvious during development).
    /// </summary>
    string Get(string key);

    /// <summary>Convenience indexer equivalent to <see cref="Get(string)"/>.</summary>
    string this[string key] { get; }
}
