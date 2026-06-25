namespace LargeFormSmokeTest.Localization;

/// <summary>
///  Central catalog of all localization keys used across the UI. Using constants instead of
///  magic strings gives compile-time safety and a single place to discover every chrome label.
/// </summary>
public static class StringKeys
{
    // ---- Application ----------------------------------------------------------------
    public const string AppTitle = "App.Title";

    // ---- Generic menus / commands ---------------------------------------------------
    public const string MenuView = "Menu.View";
    public const string MenuFile = "Menu.File";
    public const string MenuEdit = "Menu.Edit";
    public const string MenuLanguage = "Menu.Language";
    public const string MenuTheme = "Menu.Theme";
    public const string MenuGoTo = "Menu.GoTo";
    public const string LanguageEnglish = "Language.English";
    public const string LanguageGerman = "Language.German";
    public const string ThemeClassic = "Theme.Classic";
    public const string ThemeDark = "Theme.Dark";
    public const string CmdClose = "Cmd.Close";
    public const string CmdExport = "Cmd.Export";
    public const string CmdSave = "Cmd.Save";
    public const string CmdCancel = "Cmd.Cancel";

    // ---- MainForm -------------------------------------------------------------------
    public const string MainTitle = "Main.Title";
    public const string MainPayers = "Main.Payers";
    public const string MainDetails = "Main.DetailsAtAGlance";
    public const string MainDeclarations = "Main.Declarations";
    public const string CmdEditPerson = "Cmd.EditPerson";
    public const string CmdOpenDeclaration = "Cmd.OpenDeclaration";

    // ---- Payer grid columns ---------------------------------------------------------
    public const string ColTaxNumber = "Col.TaxNumber";
    public const string ColTitle = "Col.Title";
    public const string ColFirstName = "Col.FirstName";
    public const string ColLastName = "Col.LastName";
    public const string ColMaidenName = "Col.MaidenName";
    public const string ColBirthDate = "Col.BirthDate";
    public const string ColBirthPlace = "Col.BirthPlace";
    public const string ColCity = "Col.City";
    public const string ColMother = "Col.Mother";
    public const string ColFather = "Col.Father";

    // ---- Declaration grid columns ---------------------------------------------------
    public const string ColYear = "Col.Year";
    public const string ColAssessmentBasis = "Col.AssessmentBasis";
    public const string ColAssessedTax = "Col.AssessedTax";
    public const string ColDueDate = "Col.DueDate";
    public const string ColOutstanding = "Col.Outstanding";
    public const string ColStatus = "Col.Status";
    public const string ColObligation = "Col.Obligation";

    // ---- Detail field labels --------------------------------------------------------
    public const string FieldName = "Field.Name";
    public const string FieldBirth = "Field.Birth";
    public const string FieldAddress = "Field.Address";
    public const string FieldPreviousAddress = "Field.PreviousAddress";
    public const string FieldMaidenName = "Field.MaidenName";
    public const string FieldMother = "Field.Mother";
    public const string FieldFather = "Field.Father";
    public const string FieldContacts = "Field.Contacts";
    public const string FieldTaxNumber = "Field.TaxNumber";

    // ---- PersonForm -----------------------------------------------------------------
    public const string PersonTitle = "Person.Title";
    public const string PersonGroupPersonal = "Person.Group.Personal";
    public const string PersonGroupAddress = "Person.Group.Address";
    public const string PersonGroupParents = "Person.Group.Parents";
    public const string FieldTitle = "Field.Title";
    public const string FieldFirstName = "Field.FirstName";
    public const string FieldLastName = "Field.LastName";
    public const string FieldBirthDate = "Field.BirthDate";
    public const string FieldBirthPlace = "Field.BirthPlace";
    public const string FieldStreet = "Field.Street";
    public const string FieldHouseNumber = "Field.HouseNumber";
    public const string FieldPostalCode = "Field.PostalCode";
    public const string FieldCity = "Field.City";
    public const string FieldCountry = "Field.Country";

    // ---- DeclarationForm ------------------------------------------------------------
    public const string DeclTitle = "Decl.Title";
    public const string CmdEditTaxForm = "Cmd.EditTaxForm";
    public const string CmdSaveChanges = "Cmd.SaveChanges";
    public const string CmdSaveAndClose = "Cmd.SaveAndClose";
    public const string CmdCloseWithoutSaving = "Cmd.CloseWithoutSaving";
    public const string DeclReadOnlyBanner = "Decl.ReadOnlyBanner";
    public const string DeclEditableBanner = "Decl.EditableBanner";

    // ---- Section titles -------------------------------------------------------------
    public const string SecMantelbogen = "Sec.Mantelbogen";
    public const string SecAnlageN = "Sec.AnlageN";
    public const string SecAnlageKap = "Sec.AnlageKap";
    public const string SecAnlageV = "Sec.AnlageV";
    public const string SecAnlageG = "Sec.AnlageG";
    public const string SecAnlageS = "Sec.AnlageS";
    public const string SecVorsorge = "Sec.Vorsorge";
    public const string SecAnlageKind = "Sec.AnlageKind";
    public const string SecSonderausgaben = "Sec.Sonderausgaben";
    public const string SecAussergewBelastungen = "Sec.AussergewBelastungen";
    public const string SecAnlageR = "Sec.AnlageR";
    public const string SecAnlageSo = "Sec.AnlageSo";
    public const string SecAnlageUnterhalt = "Sec.AnlageUnterhalt";
    public const string SecEnergetische = "Sec.Energetische";
    public const string SecLohnsteuer = "Sec.Lohnsteuer";

    // ---- Form title banners ---------------------------------------------------------
    public const string TitleEinkommensteuer = "Title.Einkommensteuer";
    public const string TitleLohnsteuer = "Title.Lohnsteuer";
}
