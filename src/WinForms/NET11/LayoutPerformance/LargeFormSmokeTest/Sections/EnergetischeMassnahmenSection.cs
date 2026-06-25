namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class EnergetischeMassnahmenSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public EnergetischeMassnahmenSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecEnergetische;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.Measure;
        SetNumeric(_inp1, (decimal)detail.MeasureTotalCost);
        SetNumeric(_inp2, (decimal)detail.MeasureEligibleAmount);
        SetNumeric(_inp3, (decimal)detail.MeasureCompletionYear);
        _inp4.Checked = detail.CertifiedCompany;
    }
}
