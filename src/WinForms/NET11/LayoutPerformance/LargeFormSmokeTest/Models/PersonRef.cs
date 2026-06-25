namespace LargeFormSmokeTest.Models;

/// <summary>
///  A lightweight reference to a related person (e.g. mother or father). Carries only
///  the few fields needed for display; it is intentionally not a full <see cref="Person"/>.
/// </summary>
public sealed class PersonRef
{
    /// <summary>Gets or sets the first name (Vorname).</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the last name (Nachname).</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the date of birth (Geburtsdatum).</summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>Gets or sets the place of birth (Geburtsort).</summary>
    public string BirthPlace { get; set; } = string.Empty;

    /// <summary>Gets the full name in "First Last" form.</summary>
    public string FullName
        => $"{FirstName} {LastName}";
}
