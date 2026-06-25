namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AussergewBelastungenSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AussergewBelastungenSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAussergewBelastungen;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        SetNumeric(_inp0, (decimal)detail.MedicalCost);
        SetNumeric(_inp1, (decimal)detail.CareCostExtraordinary);
        SetNumeric(_inp2, (decimal)detail.DisabilityAllowance);
        SetNumeric(_inp3, (decimal)detail.CraftsmenServices);
        SetNumeric(_inp4, (decimal)detail.HouseholdServices);
        SetNumeric(_inp5, (decimal)detail.CareLevel);
        _inp6.Checked = detail.LumpSumDisability;
    }
}
