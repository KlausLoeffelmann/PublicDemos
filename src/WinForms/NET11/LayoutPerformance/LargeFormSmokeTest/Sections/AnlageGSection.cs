namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageGSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageGSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageG;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.CompanyName;
        _inp1.Text = detail.TradeType;
        SetNumeric(_inp2, (decimal)detail.BusinessProfit);
        SetNumeric(_inp3, (decimal)detail.Revenue);
        SetNumeric(_inp4, (decimal)detail.TradeTax);
        SetNumeric(_inp5, (decimal)detail.TradeTaxCredit);
        SetNumeric(_inp6, (decimal)detail.InvestmentDeduction);
        SetNumeric(_inp7, (decimal)detail.Employees);
        SetNumeric(_inp8, (decimal)detail.OperatingExpenses);
        SetNumeric(_inp9, (decimal)detail.ParticipationPercent);
        _inp10.Checked = detail.SmallBusiness;
    }
}
