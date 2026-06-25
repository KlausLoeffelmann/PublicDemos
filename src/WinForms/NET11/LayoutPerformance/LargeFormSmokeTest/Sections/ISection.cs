namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Contract implemented by every Anlage section UserControl hosted by the
///  <c>DeclarationForm</c>. It lets the host localize, toggle read-only state, populate data and
///  build the "Go to section" bookmarks generically, without knowing concrete section types.
/// </summary>
public interface ISection
{
    /// <summary>Gets the localization key of the section's display title.</summary>
    string TitleKey { get; }

    /// <summary>Re-applies localized captions to the section and its child controls.</summary>
    void ApplyLocalization(ILocalizer localizer);

    /// <summary>Switches all input controls between read-only and editable.</summary>
    void SetReadOnly(bool readOnly);

    /// <summary>Populates the section's controls from the given person and declaration.</summary>
    void LoadData(Person person, Declaration declaration);
}
