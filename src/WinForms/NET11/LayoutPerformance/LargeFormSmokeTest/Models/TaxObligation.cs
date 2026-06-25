namespace LargeFormSmokeTest.Models;

/// <summary>
///  Describes which kind(s) of tax return a person must file for a given year. German tax payers
///  may owe only assessed income tax, or — when they are both employed and run a side business —
///  both wage tax (Lohnsteuer) and income tax (Einkommensteuer).
/// </summary>
public enum TaxObligation
{
    /// <summary>Only an income-tax return (Einkommensteuererklärung) is required.</summary>
    Einkommensteuer,

    /// <summary>
    ///  Both a wage-tax and an income-tax return are required — e.g. someone who is 70% employed
    ///  and additionally has a self-employed side hustle.
    /// </summary>
    LohnsteuerUndEinkommensteuer
}
