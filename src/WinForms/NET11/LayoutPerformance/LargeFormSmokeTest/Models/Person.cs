namespace LargeFormSmokeTest.Models;

/// <summary>
///  A tax payer (Steuerpflichtiger) including personal data, addresses, contacts,
///  parents and the list of income-tax declarations.
/// </summary>
public sealed class Person
{
    /// <summary>Gets or sets the stable identity of the person.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the tax number (Steuernummer).</summary>
    public string TaxNumber { get; set; } = string.Empty;

    /// <summary>Gets or sets an academic / honorific title (Titel), may be empty.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name (Vorname).</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the last name (Nachname).</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the maiden / birth name (Geburtsname), may be <see langword="null"/>.</summary>
    public string? MaidenName { get; set; }

    /// <summary>Gets or sets the date of birth (Geburtsdatum).</summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>Gets or sets the place of birth (Geburtsort).</summary>
    public string BirthPlace { get; set; } = string.Empty;

    /// <summary>Gets or sets the current address (aktuelle Anschrift).</summary>
    public Address CurrentAddress { get; set; } = new();

    /// <summary>Gets or sets the previous address (vorherige Anschrift), may be <see langword="null"/>.</summary>
    public Address? PreviousAddress { get; set; }

    /// <summary>Gets the contact entries (telephone, e-mail, social media, …).</summary>
    public List<Contact> Contacts { get; set; } = [];

    /// <summary>Gets or sets the reference to the mother (Mutter).</summary>
    public PersonRef Mother { get; set; } = new();

    /// <summary>Gets or sets the reference to the father (Vater).</summary>
    public PersonRef Father { get; set; } = new();

    /// <summary>Gets the income-tax declarations of this person, one per tax year.</summary>
    public List<Declaration> Declarations { get; set; } = [];

    /// <summary>Gets the optionally title-prefixed full name in "Title First Last" form.</summary>
    public string FullName
        => string.IsNullOrWhiteSpace(Title)
            ? $"{FirstName} {LastName}"
            : $"{Title} {FirstName} {LastName}";

    /// <summary>
    ///  Creates a deep copy of this person so that an editor form (e.g. PersonForm) can
    ///  mutate a working copy and only commit changes back on Save.
    /// </summary>
    public Person Clone()
    {
        Person clone = new()
        {
            Id = Id,
            TaxNumber = TaxNumber,
            Title = Title,
            FirstName = FirstName,
            LastName = LastName,
            MaidenName = MaidenName,
            BirthDate = BirthDate,
            BirthPlace = BirthPlace,
            CurrentAddress = CloneAddress(CurrentAddress),
            PreviousAddress = PreviousAddress is null ? null : CloneAddress(PreviousAddress),
            Mother = CloneRef(Mother),
            Father = CloneRef(Father)
        };

        foreach (Contact contact in Contacts)
        {
            clone.Contacts.Add(new Contact { Kind = contact.Kind, Value = contact.Value });
        }

        foreach (Declaration declaration in Declarations)
        {
            clone.Declarations.Add(new Declaration
            {
                Year = declaration.Year,
                AssessmentBasis = declaration.AssessmentBasis,
                AssessedTax = declaration.AssessedTax,
                DueDate = declaration.DueDate,
                OutstandingAmount = declaration.OutstandingAmount,
                Status = declaration.Status
            });
        }

        return clone;
    }

    private static Address CloneAddress(Address source)
        => new()
        {
            Street = source.Street,
            HouseNumber = source.HouseNumber,
            PostalCode = source.PostalCode,
            City = source.City,
            Country = source.Country
        };

    private static PersonRef CloneRef(PersonRef source)
        => new()
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            BirthDate = source.BirthDate,
            BirthPlace = source.BirthPlace
        };
}
