namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageSSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageSSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageS;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.Activity;
        SetNumeric(_inp1, (decimal)detail.SelfEmployedIncome);
        SetNumeric(_inp2, (decimal)detail.SelfEmployedExpenses);
        SetNumeric(_inp3, (decimal)detail.SelfEmployedProfit);
        SetNumeric(_inp4, (decimal)detail.Prepayments);
        SetNumeric(_inp5, (decimal)detail.ArtistsSocialFund);
        SetNumeric(_inp6, (decimal)detail.TravelCost);
        SetNumeric(_inp7, (decimal)detail.Entertainment);
        SetNumeric(_inp8, (decimal)detail.SelfEmployedDepreciation);
        _inp9.Checked = detail.VatLiable;
    }
}
