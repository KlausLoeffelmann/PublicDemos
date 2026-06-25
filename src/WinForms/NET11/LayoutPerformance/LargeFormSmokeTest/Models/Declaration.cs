namespace LargeFormSmokeTest.Models;

/// <summary>
///  Payment / processing status of an <see cref="Declaration"/>. The names are kept in
///  German on purpose because they are domain data values (not localizable UI chrome).
/// </summary>
public enum DeclarationStatus
{
    /// <summary>Open — the assessed tax has not been paid yet.</summary>
    Offen,

    /// <summary>Settled — the assessed tax has been paid in full.</summary>
    Beglichen,

    /// <summary>Deferred — payment has been officially postponed.</summary>
    Gestundet
}

/// <summary>
///  One income-tax declaration (Einkommensteuererklärung) for a given tax year.
/// </summary>
public sealed class Declaration
{
    /// <summary>Gets or sets the tax year (Veranlagungsjahr).</summary>
    public int Year { get; set; }

    /// <summary>Gets or sets the assessment basis (Bemessungsgrundlage).</summary>
    public decimal AssessmentBasis { get; set; }

    /// <summary>Gets or sets the assessed / payable tax (festgesetzte Steuer).</summary>
    public decimal AssessedTax { get; set; }

    /// <summary>Gets or sets the due date (Fälligkeit).</summary>
    public DateOnly DueDate { get; set; }

    /// <summary>Gets or sets the still outstanding amount (ausstehender Betrag).</summary>
    public decimal OutstandingAmount { get; set; }

    /// <summary>Gets or sets the processing status.</summary>
    public DeclarationStatus Status { get; set; }

    /// <summary>
    ///  Gets or sets which kind(s) of return are owed for this year. Not part of the source JSON;
    ///  it is assigned deterministically when the dataset is loaded.
    /// </summary>
    public TaxObligation Obligation { get; set; }
}
