namespace LargeFormSmokeTest.Forms;

using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Modal editor for a single tax payer's personal data. It edits the live <see cref="Person"/>
///  in place and only commits the field values when the user clicks Save; Cancel discards them.
///  Being modal guarantees only one instance is open at a time.
/// </summary>
public partial class PersonForm : Form
{
    private readonly ILocalizer _localizer = AppServices.Localizer;
    private readonly Person _person;

    /// <summary>Initializes the editor for the supplied <paramref name="person"/>.</summary>
    public PersonForm(Person person)
    {
        _person = person;

        InitializeComponent();

        _btnSave.Click += (_, _) => Save();

        ApplyLocalization();
        LoadFromPerson();
    }

    private void LoadFromPerson()
    {
        _txtTitle.Text = _person.Title;
        _txtFirstName.Text = _person.FirstName;
        _txtLastName.Text = _person.LastName;
        _txtMaiden.Text = _person.MaidenName ?? string.Empty;
        _dtBirthDate.Value = ClampToPicker(_person.BirthDate);
        _txtBirthPlace.Text = _person.BirthPlace;

        _txtStreet.Text = _person.CurrentAddress.Street;
        _txtHouseNumber.Text = _person.CurrentAddress.HouseNumber;
        _txtPostalCode.Text = _person.CurrentAddress.PostalCode;
        _txtCity.Text = _person.CurrentAddress.City;
        _txtCountry.Text = _person.CurrentAddress.Country;

        _txtMother.Text = _person.Mother.FullName;
        _txtFather.Text = _person.Father.FullName;
    }

    private void Save()
    {
        _person.Title = _txtTitle.Text;
        _person.FirstName = _txtFirstName.Text;
        _person.LastName = _txtLastName.Text;
        _person.MaidenName = string.IsNullOrWhiteSpace(_txtMaiden.Text) ? null : _txtMaiden.Text;
        _person.BirthDate = DateOnly.FromDateTime(_dtBirthDate.Value);
        _person.BirthPlace = _txtBirthPlace.Text;

        _person.CurrentAddress.Street = _txtStreet.Text;
        _person.CurrentAddress.HouseNumber = _txtHouseNumber.Text;
        _person.CurrentAddress.PostalCode = _txtPostalCode.Text;
        _person.CurrentAddress.City = _txtCity.Text;
        _person.CurrentAddress.Country = _txtCountry.Text;

        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>Clamps a birth date into the DateTimePicker's valid range to avoid exceptions.</summary>
    private DateTime ClampToPicker(DateOnly date)
    {
        DateTime value = date.ToDateTime(TimeOnly.MinValue);

        if (value < _dtBirthDate.MinDate)
        {
            return _dtBirthDate.MinDate;
        }

        if (value > _dtBirthDate.MaxDate)
        {
            return _dtBirthDate.MaxDate;
        }

        return value;
    }

    private void ApplyLocalization()
    {
        Text = _localizer[StringKeys.PersonTitle];

        _personalGroup.Text = _localizer[StringKeys.PersonGroupPersonal];
        _addressGroup.Text = _localizer[StringKeys.PersonGroupAddress];
        _parentsGroup.Text = _localizer[StringKeys.PersonGroupParents];

        _capTitle.Text = _localizer[StringKeys.FieldTitle];
        _capFirstName.Text = _localizer[StringKeys.FieldFirstName];
        _capLastName.Text = _localizer[StringKeys.FieldLastName];
        _capMaiden.Text = _localizer[StringKeys.FieldMaidenName];
        _capBirthDate.Text = _localizer[StringKeys.FieldBirthDate];
        _capBirthPlace.Text = _localizer[StringKeys.FieldBirthPlace];

        _capStreet.Text = _localizer[StringKeys.FieldStreet];
        _capHouseNumber.Text = _localizer[StringKeys.FieldHouseNumber];
        _capPostalCode.Text = _localizer[StringKeys.FieldPostalCode];
        _capCity.Text = _localizer[StringKeys.FieldCity];
        _capCountry.Text = _localizer[StringKeys.FieldCountry];

        _capMother.Text = _localizer[StringKeys.FieldMother];
        _capFather.Text = _localizer[StringKeys.FieldFather];

        _btnSave.Text = _localizer[StringKeys.CmdSave];
        _btnCancel.Text = _localizer[StringKeys.CmdCancel];
    }
}
