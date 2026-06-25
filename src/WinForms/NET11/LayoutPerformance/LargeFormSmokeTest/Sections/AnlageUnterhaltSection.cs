namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class AnlageUnterhaltSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public AnlageUnterhaltSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecAnlageUnterhalt;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp0.Text = detail.RecipientName;
        SetNumeric(_inp1, (decimal)detail.MaintenanceAmount);
        SetNumeric(_inp2, (decimal)detail.RecipientIncome);
        SetNumeric(_inp3, (decimal)detail.MonthsSupported);
        _inp4.Checked = detail.HouseholdAbroad;
    }
}
