namespace LargeFormSmokeTest.Models;

/// <summary>
///  Root object of the demo dataset. Maps 1:1 to the top-level shape described by
///  tax-demo-data.schema.json.
/// </summary>
public sealed class TaxDemoDataset
{
    /// <summary>Gets or sets the schema version string.</summary>
    public string SchemaVersion { get; set; } = string.Empty;

    /// <summary>Gets or sets the timestamp at which the dataset was generated.</summary>
    public DateTimeOffset? Generated { get; set; }

    /// <summary>Gets the tax payers contained in the dataset.</summary>
    public List<Person> Persons { get; set; } = [];
}
