namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageVSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageVSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageV;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.PropertyAddress;
        SetNumeric(_inp1, (decimal)detail.RentalIncome);
        SetNumeric(_inp2, (decimal)detail.ServiceCharges);
        SetNumeric(_inp3, (decimal)detail.Depreciation);
        SetNumeric(_inp4, (decimal)detail.DebtInterest);
        SetNumeric(_inp5, (decimal)detail.MaintenanceCost);
        SetNumeric(_inp6, (decimal)detail.PropertyTax);
        SetNumeric(_inp7, (decimal)detail.Insurance);
        SetNumeric(_inp8, (decimal)detail.AdminCost);
        SetNumeric(_inp9, (decimal)detail.VacancyMonths);
        SetNumeric(_inp10, (decimal)detail.ConstructionYear);
        _inp11.Checked = detail.FullyLet;
    }
}
