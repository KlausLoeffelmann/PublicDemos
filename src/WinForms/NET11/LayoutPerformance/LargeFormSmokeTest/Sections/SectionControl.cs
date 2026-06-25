namespace LargeFormSmokeTest.Sections;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Concrete, designable base for all Anlage section UserControls. It provides the shared
///  localization, read-only and data-loading plumbing so each derived section only has to lay
///  out its GroupBox and fields in the Designer and declare its title key.
/// </summary>
/// <remarks>
///  Derived sections set <see cref="SectionGroupBox"/> after <c>InitializeComponent</c>. Child
///  labels whose <see cref="Control.Tag"/> is a string are treated as localization keys, which
///  keeps the Designer markup declarative and free of code-behind wiring.
/// </remarks>
public class SectionControl : UserControl, ISection
{
    /// <summary>Gets or sets the root GroupBox whose caption carries the section title.</summary>
    protected GroupBox? SectionGroupBox { get; set; }

    /// <inheritdoc/>
    public virtual string TitleKey
        => string.Empty;

    /// <inheritdoc/>
    public virtual void ApplyLocalization(ILocalizer localizer)
    {
        if (SectionGroupBox is not null)
        {
            SectionGroupBox.Text = localizer[TitleKey];
        }

        LocalizeChildren(this, localizer);
    }

    /// <inheritdoc/>
    public virtual void SetReadOnly(bool readOnly)
        => SetReadOnlyRecursive(this, readOnly);

    /// <inheritdoc/>
    public virtual void LoadData(Person person, Declaration declaration, DeclarationDetail detail)
    {
        // The base section carries no bindable fields of its own; generated sections override
        // this to populate their inputs from the deterministic <see cref="DeclarationDetail"/>.
    }

    /// <summary>Assigns a value to a NumericUpDown, clamped to its valid range.</summary>
    protected static void SetNumeric(NumericUpDown numeric, decimal value)
        => numeric.Value = value < numeric.Minimum
            ? numeric.Minimum
            : value > numeric.Maximum
                ? numeric.Maximum
                : value;

    /// <summary>Assigns a date to a DateTimePicker, clamped to its valid range.</summary>
    protected static void SetDate(DateTimePicker picker, DateOnly date)
    {
        DateTime value = date.ToDateTime(TimeOnly.MinValue);

        picker.Value = value < picker.MinDate
            ? picker.MinDate
            : value > picker.MaxDate
                ? picker.MaxDate
                : value;
    }

    /// <summary>Checks the radio button at <paramref name="index"/> within a radio group panel.</summary>
    protected static void SelectRadio(Control group, int index)
    {
        int i = 0;

        foreach (Control child in group.Controls)
        {
            if (child is RadioButton radio)
            {
                radio.Checked = i == index;
                i++;
            }
        }
    }

    /// <summary>
    ///  Walks the control tree and localizes any control whose <see cref="Control.Tag"/> is a
    ///  non-empty string (interpreted as a <see cref="StringKeys"/> value).
    /// </summary>
    private static void LocalizeChildren(Control parent, ILocalizer localizer)
    {
        foreach (Control child in parent.Controls)
        {
            if (child.Tag is string key && key.Length > 0)
            {
                child.Text = localizer[key];
            }

            if (child.HasChildren)
            {
                LocalizeChildren(child, localizer);
            }
        }
    }

    /// <summary>
    ///  Recursively switches input controls between read-only and editable. Text boxes use their
    ///  native read-only mode (keeps text crisp); other inputs are disabled so the form clearly
    ///  reads as locked until the clerk chooses "Edit tax form". Chrome controls are skipped.
    /// </summary>
    private static void SetReadOnlyRecursive(Control parent, bool readOnly)
    {
        foreach (Control child in parent.Controls)
        {
            switch (child)
            {
                case TextBoxBase textBox:
                    textBox.ReadOnly = readOnly;
                    break;

                case ComboBox or NumericUpDown or CheckBox or RadioButton or DateTimePicker:
                    child.Enabled = !readOnly;
                    break;
            }

            if (child.HasChildren)
            {
                SetReadOnlyRecursive(child, readOnly);
            }
        }
    }
}
