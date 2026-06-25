namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class SonderausgabenSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public SonderausgabenSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecSonderausgaben;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        SetNumeric(_inp0, (decimal)detail.Donations);
        SetNumeric(_inp1, (decimal)detail.ChurchTaxPaid);
        SetNumeric(_inp2, (decimal)detail.AlimonyPaid);
        SetNumeric(_inp3, (decimal)detail.VocationalTraining);
        SetNumeric(_inp4, (decimal)detail.RetirementProvision);
        _inp5.Checked = detail.ChurchMember;
    }
}
