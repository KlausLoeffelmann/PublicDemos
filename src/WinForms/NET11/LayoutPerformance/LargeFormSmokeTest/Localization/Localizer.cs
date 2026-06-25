namespace LargeFormSmokeTest.Localization;

/// <summary>
///  Keyed-dictionary implementation of <see cref="ILocalizer"/>. Keeps one translation table
///  per supported language in memory; switching language is an O(1) pointer swap followed by a
///  single <see cref="LanguageChanged"/> notification.
/// </summary>
public sealed class Localizer : ILocalizer
{
    private readonly Dictionary<AppLanguage, Dictionary<string, string>> _tables;
    private AppLanguage _language;

    /// <summary>Initializes a new localizer starting in the given <paramref name="language"/>.</summary>
    public Localizer(AppLanguage language = AppLanguage.English)
    {
        _language = language;
        _tables = new Dictionary<AppLanguage, Dictionary<string, string>>
        {
            [AppLanguage.English] = BuildEnglish(),
            [AppLanguage.German] = BuildGerman()
        };
    }

    /// <inheritdoc/>
    public event EventHandler? LanguageChanged;

    /// <inheritdoc/>
    public AppLanguage Language
    {
        get => _language;

        set
        {
            if (_language == value)
            {
                return;
            }

            _language = value;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc/>
    public string this[string key]
        => Get(key);

    /// <inheritdoc/>
    public string Get(string key)
        => _tables[_language].TryGetValue(key, out string? value)
            ? value
            : key;

    private static Dictionary<string, string> BuildEnglish()
        => new()
        {
            [StringKeys.AppTitle] = "Tax Demo — Large Form Smoke Test",
            [StringKeys.MenuView] = "&View",
            [StringKeys.MenuFile] = "&File",
            [StringKeys.MenuEdit] = "&Edit",
            [StringKeys.MenuLanguage] = "&Language",
            [StringKeys.MenuTheme] = "&Theme",
            [StringKeys.MenuGoTo] = "&Go to section",
            [StringKeys.LanguageEnglish] = "English",
            [StringKeys.LanguageGerman] = "German",
            [StringKeys.ThemeClassic] = "Classic",
            [StringKeys.ThemeDark] = "Dark",
            [StringKeys.CmdClose] = "Close",
            [StringKeys.CmdExport] = "Export…",
            [StringKeys.CmdSave] = "Save",
            [StringKeys.CmdCancel] = "Cancel",
            [StringKeys.MainTitle] = "Tax Payers — Overview",
            [StringKeys.MainPayers] = "Tax payers",
            [StringKeys.MainDetails] = "Details at a glance",
            [StringKeys.MainDeclarations] = "Income-tax declarations",
            [StringKeys.CmdEditPerson] = "Edit person",
            [StringKeys.CmdOpenDeclaration] = "Open declaration",
            [StringKeys.ColTaxNumber] = "Tax number",
            [StringKeys.ColTitle] = "Title",
            [StringKeys.ColFirstName] = "First name",
            [StringKeys.ColLastName] = "Last name",
            [StringKeys.ColMaidenName] = "Maiden name",
            [StringKeys.ColBirthDate] = "Date of birth",
            [StringKeys.ColBirthPlace] = "Place of birth",
            [StringKeys.ColCity] = "City",
            [StringKeys.ColMother] = "Mother",
            [StringKeys.ColFather] = "Father",
            [StringKeys.ColYear] = "Year",
            [StringKeys.ColAssessmentBasis] = "Assessment basis",
            [StringKeys.ColAssessedTax] = "Tax payable",
            [StringKeys.ColDueDate] = "Due date",
            [StringKeys.ColOutstanding] = "Outstanding",
            [StringKeys.ColStatus] = "Status",
            [StringKeys.FieldName] = "Name",
            [StringKeys.FieldBirth] = "Born",
            [StringKeys.FieldAddress] = "Address",
            [StringKeys.FieldPreviousAddress] = "Previous address",
            [StringKeys.FieldMaidenName] = "Maiden name",
            [StringKeys.FieldMother] = "Mother",
            [StringKeys.FieldFather] = "Father",
            [StringKeys.FieldContacts] = "Contacts",
            [StringKeys.FieldTaxNumber] = "Tax number",
            [StringKeys.PersonTitle] = "Edit person",
            [StringKeys.PersonGroupPersonal] = "Personal data",
            [StringKeys.PersonGroupAddress] = "Current address",
            [StringKeys.PersonGroupParents] = "Parents",
            [StringKeys.FieldTitle] = "Title",
            [StringKeys.FieldFirstName] = "First name",
            [StringKeys.FieldLastName] = "Last name",
            [StringKeys.FieldBirthDate] = "Date of birth",
            [StringKeys.FieldBirthPlace] = "Place of birth",
            [StringKeys.FieldStreet] = "Street",
            [StringKeys.FieldHouseNumber] = "No.",
            [StringKeys.FieldPostalCode] = "Postal code",
            [StringKeys.FieldCity] = "City",
            [StringKeys.FieldCountry] = "Country",
            [StringKeys.DeclTitle] = "Income-tax declaration",
            [StringKeys.CmdEditTaxForm] = "Edit tax form",
            [StringKeys.CmdSaveChanges] = "Save changes",
            [StringKeys.CmdSaveAndClose] = "Save and close",
            [StringKeys.CmdCloseWithoutSaving] = "Close without saving",
            [StringKeys.DeclReadOnlyBanner] = "Read-only — choose “Edit tax form” to make changes.",
            [StringKeys.DeclEditableBanner] = "Editing — remember to save your changes.",
            [StringKeys.SecMantelbogen] = "Main sheet / master data",
            [StringKeys.SecAnlageN] = "Annex N — Employment income",
            [StringKeys.SecAnlageKap] = "Annex KAP — Investment income",
            [StringKeys.SecAnlageV] = "Annex V — Rental & leasing",
            [StringKeys.SecAnlageG] = "Annex G — Business income",
            [StringKeys.SecAnlageS] = "Annex S — Self-employment",
            [StringKeys.SecVorsorge] = "Annex — Pension expenses",
            [StringKeys.SecAnlageKind] = "Annex Child",
            [StringKeys.SecSonderausgaben] = "Special & extraordinary expenses"
        };

    private static Dictionary<string, string> BuildGerman()
        => new()
        {
            [StringKeys.AppTitle] = "Steuer-Demo — Large Form Smoke Test",
            [StringKeys.MenuView] = "&Ansicht",
            [StringKeys.MenuFile] = "&Datei",
            [StringKeys.MenuEdit] = "&Bearbeiten",
            [StringKeys.MenuLanguage] = "&Sprache",
            [StringKeys.MenuTheme] = "&Design",
            [StringKeys.MenuGoTo] = "&Gehe zu Abschnitt",
            [StringKeys.LanguageEnglish] = "Englisch",
            [StringKeys.LanguageGerman] = "Deutsch",
            [StringKeys.ThemeClassic] = "Klassisch",
            [StringKeys.ThemeDark] = "Dunkel",
            [StringKeys.CmdClose] = "Schließen",
            [StringKeys.CmdExport] = "Exportieren…",
            [StringKeys.CmdSave] = "Speichern",
            [StringKeys.CmdCancel] = "Abbrechen",
            [StringKeys.MainTitle] = "Steuerpflichtige — Übersicht",
            [StringKeys.MainPayers] = "Steuerpflichtige",
            [StringKeys.MainDetails] = "Details auf einen Blick",
            [StringKeys.MainDeclarations] = "Einkommensteuererklärungen",
            [StringKeys.CmdEditPerson] = "Person bearbeiten",
            [StringKeys.CmdOpenDeclaration] = "Erklärung öffnen",
            [StringKeys.ColTaxNumber] = "Steuernummer",
            [StringKeys.ColTitle] = "Titel",
            [StringKeys.ColFirstName] = "Vorname",
            [StringKeys.ColLastName] = "Nachname",
            [StringKeys.ColMaidenName] = "Geburtsname",
            [StringKeys.ColBirthDate] = "Geburtsdatum",
            [StringKeys.ColBirthPlace] = "Geburtsort",
            [StringKeys.ColCity] = "Ort",
            [StringKeys.ColMother] = "Mutter",
            [StringKeys.ColFather] = "Vater",
            [StringKeys.ColYear] = "Jahr",
            [StringKeys.ColAssessmentBasis] = "Bemessungsgrundlage",
            [StringKeys.ColAssessedTax] = "Zu zahlende Steuer",
            [StringKeys.ColDueDate] = "Fälligkeit",
            [StringKeys.ColOutstanding] = "Ausstehender Betrag",
            [StringKeys.ColStatus] = "Status",
            [StringKeys.FieldName] = "Name",
            [StringKeys.FieldBirth] = "Geboren",
            [StringKeys.FieldAddress] = "Anschrift",
            [StringKeys.FieldPreviousAddress] = "Vorherige Anschrift",
            [StringKeys.FieldMaidenName] = "Geburtsname",
            [StringKeys.FieldMother] = "Mutter",
            [StringKeys.FieldFather] = "Vater",
            [StringKeys.FieldContacts] = "Kontakte",
            [StringKeys.FieldTaxNumber] = "Steuernummer",
            [StringKeys.PersonTitle] = "Person bearbeiten",
            [StringKeys.PersonGroupPersonal] = "Persönliche Daten",
            [StringKeys.PersonGroupAddress] = "Aktuelle Anschrift",
            [StringKeys.PersonGroupParents] = "Eltern",
            [StringKeys.FieldTitle] = "Titel",
            [StringKeys.FieldFirstName] = "Vorname",
            [StringKeys.FieldLastName] = "Nachname",
            [StringKeys.FieldBirthDate] = "Geburtsdatum",
            [StringKeys.FieldBirthPlace] = "Geburtsort",
            [StringKeys.FieldStreet] = "Straße",
            [StringKeys.FieldHouseNumber] = "Nr.",
            [StringKeys.FieldPostalCode] = "PLZ",
            [StringKeys.FieldCity] = "Ort",
            [StringKeys.FieldCountry] = "Land",
            [StringKeys.DeclTitle] = "Einkommensteuererklärung",
            [StringKeys.CmdEditTaxForm] = "Steuerformular bearbeiten",
            [StringKeys.CmdSaveChanges] = "Änderungen speichern",
            [StringKeys.CmdSaveAndClose] = "Speichern und schließen",
            [StringKeys.CmdCloseWithoutSaving] = "Schließen ohne Speichern",
            [StringKeys.DeclReadOnlyBanner] = "Schreibgeschützt — „Steuerformular bearbeiten“ wählen, um Änderungen vorzunehmen.",
            [StringKeys.DeclEditableBanner] = "Bearbeitung — bitte Änderungen speichern.",
            [StringKeys.SecMantelbogen] = "Mantelbogen / Stammdaten",
            [StringKeys.SecAnlageN] = "Anlage N — Nichtselbständige Arbeit",
            [StringKeys.SecAnlageKap] = "Anlage KAP — Kapitalerträge",
            [StringKeys.SecAnlageV] = "Anlage V — Vermietung & Verpachtung",
            [StringKeys.SecAnlageG] = "Anlage G — Gewerbebetrieb",
            [StringKeys.SecAnlageS] = "Anlage S — Selbständige Arbeit",
            [StringKeys.SecVorsorge] = "Anlage Vorsorgeaufwand",
            [StringKeys.SecAnlageKind] = "Anlage Kind",
            [StringKeys.SecSonderausgaben] = "Sonderausgaben / außergewöhnliche Belastungen"
        };
}
