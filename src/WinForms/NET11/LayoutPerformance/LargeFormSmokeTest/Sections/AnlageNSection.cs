namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageNSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageNSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageN;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.Employer;
        SetNumeric(_inp1, (decimal)detail.GrossWage);
        SetNumeric(_inp2, (decimal)detail.WageTaxWithheld);
        SetNumeric(_inp3, (decimal)detail.SolidaritySurcharge);
        SetNumeric(_inp4, (decimal)detail.ChurchTax);
        SetNumeric(_inp6, (decimal)detail.CommutingKm);
        SetNumeric(_inp7, (decimal)detail.WorkDays);
        SetNumeric(_inp8, (decimal)detail.WorkEquipment);
        SetNumeric(_inp9, (decimal)detail.ProfessionalAssociationFees);
        SetNumeric(_inp10, (decimal)detail.FurtherEducation);
        _inp11.Checked = detail.DoubleHousehold;
        _inp12.Checked = detail.HomeOffice;
    }
}
