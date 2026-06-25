namespace LargeFormSmokeTest.Models;

/// <summary>
///  A postal address (Anschrift) of a tax payer. Used both for the current and a
///  previous address.
/// </summary>
public sealed class Address
{
    /// <summary>Gets or sets the street name (Straße).</summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>Gets or sets the house number (Hausnummer).</summary>
    public string HouseNumber { get; set; } = string.Empty;

    /// <summary>Gets or sets the postal code (Postleitzahl).</summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the city (Ort).</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the country (Land).</summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    ///  Returns a single-line, human-readable representation of the address.
    /// </summary>
    public override string ToString()
        => $"{Street} {HouseNumber}, {PostalCode} {City}, {Country}";
}
