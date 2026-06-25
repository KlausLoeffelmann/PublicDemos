namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;

/// <summary>
///  Designable Anlage section UserControl. Its GroupBox-dense layout lives in the Designer file;
///  shared localization / read-only / data plumbing comes from <see cref="SectionControl"/>.
/// </summary>
public partial class VorsorgeSection : SectionControl
{
    /// <summary>Initializes the section and registers its GroupBox for title localization.</summary>
    public VorsorgeSection()
    {
        InitializeComponent();
        SectionGroupBox = _groupBox;
    }

    /// <inheritdoc/>
    public override string TitleKey
        => StringKeys.SecVorsorge;
}
