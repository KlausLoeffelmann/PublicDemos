namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class LohnsteuerbescheinigungSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public LohnsteuerbescheinigungSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecLohnsteuer;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp1.Text = detail.EmployerLst;
        SelectRadio(_rad2, (int)detail.TaxClass);
        _inp3.Text = detail.ETin;
        SetNumeric(_inp4, (decimal)detail.GrossWageLst);
        SetNumeric(_inp5, (decimal)detail.WageTaxLst);
        SetNumeric(_inp6, (decimal)detail.SolidaritySurchargeLst);
        SetNumeric(_inp7, (decimal)detail.ChurchTaxLst);
        SetNumeric(_inp8, (decimal)detail.InsuranceDays);
    }
}
