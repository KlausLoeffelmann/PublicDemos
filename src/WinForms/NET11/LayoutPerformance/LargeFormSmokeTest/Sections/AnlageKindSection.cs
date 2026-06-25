namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageKindSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageKindSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageKind;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.ChildName;
        SetDate(_inp1, detail.ChildBirthDate);
        _inp2.Text = detail.ChildTaxId;
        SetNumeric(_inp3, (decimal)detail.ChildBenefit);
        SetNumeric(_inp4, (decimal)detail.ChildAllowance);
        SetNumeric(_inp5, (decimal)detail.CareCost);
        SetNumeric(_inp6, (decimal)detail.SchoolFees);
        SelectRadio(_rad7, (int)detail.EducationStatus);
        _inp8.Checked = detail.AwayAccommodation;
        SetNumeric(_inp9, (decimal)detail.DisabilityDegree);
    }
}
