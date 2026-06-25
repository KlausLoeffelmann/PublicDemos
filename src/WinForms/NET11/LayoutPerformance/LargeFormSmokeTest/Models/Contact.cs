namespace LargeFormSmokeTest.Models;

/// <summary>
///  The kind of a <see cref="Contact"/> entry. Mirrors the values allowed by the
///  JSON schema (tax-demo-data.schema.json).
/// </summary>
public enum ContactKind
{
    /// <summary>A land-line phone number.</summary>
    Phone,

    /// <summary>A mobile phone number.</summary>
    Mobile,

    /// <summary>An e-mail address.</summary>
    Email,

    /// <summary>A fax number.</summary>
    Fax,

    /// <summary>A LinkedIn profile.</summary>
    LinkedIn,

    /// <summary>An X (formerly Twitter) handle.</summary>
    X,

    /// <summary>A Mastodon handle.</summary>
    Mastodon,

    /// <summary>A web site URL.</summary>
    Website
}

/// <summary>
///  A single contact entry (telephone, e-mail, social media, …) of a tax payer.
/// </summary>
public sealed class Contact
{
    /// <summary>Gets or sets the kind of contact.</summary>
    public ContactKind Kind { get; set; }

    /// <summary>Gets or sets the raw value (number, address, handle, …).</summary>
    public string Value { get; set; } = string.Empty;
}
