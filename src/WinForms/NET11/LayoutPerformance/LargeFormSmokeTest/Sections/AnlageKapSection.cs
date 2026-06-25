namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageKapSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageKapSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageKap;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.Bank;
        SetNumeric(_inp1, (decimal)detail.CapitalIncome);
        SetNumeric(_inp2, (decimal)detail.CapitalGainsTax);
        SetNumeric(_inp3, (decimal)detail.SolidaritySurchargeKap);
        SetNumeric(_inp4, (decimal)detail.SaverAllowance);
        SetNumeric(_inp5, (decimal)detail.ForeignCapitalIncome);
        SetNumeric(_inp6, (decimal)detail.WithholdingTaxCredit);
        SetNumeric(_inp7, (decimal)detail.LossCarryforward);
        _inp8.Checked = detail.FavourableCheck;
        _inp9.Checked = detail.ChurchTaxLiableKap;
    }
}
