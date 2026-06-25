namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense, HighDPI-resilient layout (AutoSize
///  rows + margins) lives in the Designer file; shared localization / read-only plumbing comes
///  from <see cref="SectionControl"/>. Inputs are bound from the deterministic detail in LoadData.
/// </summary>
public partial class MantelbogenSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public MantelbogenSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecMantelbogen;

    /// <inheritdoc/>
    public override void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        _inp1.Text = detail.TaxNumber;
        _inp2.Text = detail.TaxOffice;
        SetNumeric(_inp3, (decimal)detail.Year);
        _inp4.Text = detail.TaxId;
        SelectRadio(_rad5, (int)detail.MaritalStatus);
        SelectRadio(_rad6, (int)detail.Religion);
        _inp7.Text = detail.Iban;
        _inp8.Checked = detail.SubmittedElectronically;
        SetNumeric(_inp11, (decimal)detail.AssessmentBasis);
        SetNumeric(_inp12, (decimal)detail.AssessedTax);
        SetNumeric(_inp13, (decimal)detail.OutstandingAmount);
        SelectRadio(_rad14, (int)detail.Status);
        SelectRadio(_rad15, (int)detail.Obligation);
    }
}
