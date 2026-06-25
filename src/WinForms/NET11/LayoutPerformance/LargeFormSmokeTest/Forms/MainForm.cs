namespace LargeFormSmokeTest.Forms;

using LargeFormSmokeTest.Controls;
using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;
using LargeFormSmokeTest.Theming;

/// <summary>
///  Overview / warm-up form. It is intentionally cheap: it lists the tax payers and the
///  declarations of the selected payer, and launches the (heavy) <see cref="DeclarationForm"/>
///  that is the actual subject of the performance test. Showing it first warms the process,
///  JIT and loaded assemblies before the form under test is measured.
/// </summary>
public partial class MainForm : Form
{
    private readonly ILocalizer _localizer = AppServices.Localizer;
    private readonly ThemeManager _theme = AppServices.Theme;

    // Owned bold header font, recreated on each theme change (the previous one is disposed).
    private Font? _headerFont;

    // Payer-grid columns kept as fields so their header captions can be re-localized live.
    private readonly DataGridViewTextBoxColumn _colTaxNumber = new();
    private readonly DataGridViewTextBoxColumn _colTitle = new();
    private readonly DataGridViewTextBoxColumn _colFirstName = new();
    private readonly DataGridViewTextBoxColumn _colLastName = new();
    private readonly DataGridViewTextBoxColumn _colMaiden = new();
    private readonly DataGridViewTextBoxColumn _colBirthDate = new();
    private readonly DataGridViewTextBoxColumn _colBirthPlace = new();
    private readonly DataGridViewTextBoxColumn _colCity = new();
    private readonly DataGridViewTextBoxColumn _colMother = new();
    private readonly DataGridViewTextBoxColumn _colFather = new();

    // Declaration-grid columns (bottom-right), likewise kept for live re-localization.
    private readonly DataGridViewTextBoxColumn _dcolYear = new();
    private readonly DataGridViewTextBoxColumn _dcolBasis = new();
    private readonly DataGridViewTextBoxColumn _dcolTax = new();
    private readonly DataGridViewTextBoxColumn _dcolDue = new();
    private readonly DataGridViewTextBoxColumn _dcolOutstanding = new();
    private readonly DataGridViewTextBoxColumn _dcolStatus = new();
    private readonly DataGridViewTextBoxColumn _dcolObligation = new();

    /// <summary>Initializes the overview form and binds it to the repository data.</summary>
    public MainForm()
    {
        InitializeComponent();

        BuildPayerColumns();
        BuildDeclarationColumns();
        WireEvents();
        StyleCaptions();
        LoadPayers();

        ApplyLocalization();
        ApplyTheme();

        // Reflect the currently active language / theme as checked menu items.
        UpdateMenuChecks();

        // Establish the current row explicitly (Selected alone does not set CurrentRow), then
        // refresh the detail region so the form is populated before the user interacts.
        if (_payersGrid.Rows.Count > 0)
        {
            _payersGrid.CurrentCell = _payersGrid.Rows[0].Cells[0];
            _payersGrid.Rows[0].Selected = true;
            UpdateDetails();
        }
    }

    private Person? SelectedPerson
        => _payersGrid.CurrentRow?.Tag as Person;

    private Declaration? SelectedDeclaration
        => _declarationsGrid.CurrentRow?.Tag as Declaration;

    private void WireEvents()
    {
        _payersGrid.SelectionChanged += (_, _) => UpdateDetails();
        _declarationsGrid.CellDoubleClick += OnDeclarationDoubleClick;
        _declarationsGrid.CellFormatting += OnDeclarationCellFormatting;

        _btnEditPerson.Click += (_, _) => EditSelectedPerson();
        _btnOpenDeclaration.Click += (_, _) => OpenSelectedDeclaration();

        _menuEnglish.Click += (_, _) => _localizer.Language = AppLanguage.English;
        _menuGerman.Click += (_, _) => _localizer.Language = AppLanguage.German;
        _menuClassic.Click += (_, _) => _theme.Theme = AppTheme.Classic;
        _menuDark.Click += (_, _) => _theme.Theme = AppTheme.Dark;

        _localizer.LanguageChanged += (_, _) =>
        {
            ApplyLocalization();
            UpdateMenuChecks();
        };

        _theme.ThemeChanged += (_, _) =>
        {
            ApplyTheme();
            UpdateMenuChecks();
        };
    }

    private void BuildPayerColumns()
    {
        ConfigureColumn(_colTaxNumber, nameof(Person.TaxNumber));
        ConfigureColumn(_colTitle, nameof(Person.Title));
        ConfigureColumn(_colFirstName, nameof(Person.FirstName));
        ConfigureColumn(_colLastName, nameof(Person.LastName));
        ConfigureColumn(_colMaiden, nameof(Person.MaidenName));
        ConfigureColumn(_colBirthDate, nameof(Person.BirthDate));
        ConfigureColumn(_colBirthPlace, nameof(Person.BirthPlace));
        ConfigureColumn(_colCity, nameof(Person.CurrentAddress));
        ConfigureColumn(_colMother, nameof(Person.Mother));
        ConfigureColumn(_colFather, nameof(Person.Father));

        _payersGrid.Columns.AddRange(
            _colTaxNumber, _colTitle, _colFirstName, _colLastName, _colMaiden,
            _colBirthDate, _colBirthPlace, _colCity, _colMother, _colFather);
    }

    private void BuildDeclarationColumns()
    {
        ConfigureColumn(_dcolYear, nameof(Declaration.Year));
        ConfigureColumn(_dcolBasis, nameof(Declaration.AssessmentBasis));
        ConfigureColumn(_dcolTax, nameof(Declaration.AssessedTax));
        ConfigureColumn(_dcolDue, nameof(Declaration.DueDate));
        ConfigureColumn(_dcolOutstanding, nameof(Declaration.OutstandingAmount));
        ConfigureColumn(_dcolStatus, nameof(Declaration.Status));
        ConfigureColumn(_dcolObligation, nameof(Declaration.Obligation));

        _dcolBasis.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        _dcolTax.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        _dcolOutstanding.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        // One bold font for the color-coded Status column, set once (avoids allocating a Font
        // per cell during the frequent CellFormatting events).
        _dcolStatus.DefaultCellStyle.Font = new Font(_declarationsGrid.Font, FontStyle.Bold);

        _declarationsGrid.Columns.AddRange(
            _dcolYear, _dcolBasis, _dcolTax, _dcolDue, _dcolOutstanding, _dcolStatus, _dcolObligation);
    }

    /// <summary>Makes the left-column detail captions bold (kept out of the Designer file).</summary>
    private void StyleCaptions()
    {
        Font bold = new(Font, FontStyle.Bold);

        _capName.Font = bold;
        _capBirth.Font = bold;
        _capAddress.Font = bold;
        _capPrevAddress.Font = bold;
        _capMaiden.Font = bold;
        _capMother.Font = bold;
        _capFather.Font = bold;
        _capContacts.Font = bold;
    }

    private static void ConfigureColumn(DataGridViewTextBoxColumn column, string name)
    {
        column.Name = name;
        column.SortMode = DataGridViewColumnSortMode.NotSortable;
    }

    private void LoadPayers()
    {
        _payersGrid.Rows.Clear();

        foreach (Person person in AppServices.Repository.Persons)
        {
            int index = _payersGrid.Rows.Add(
                person.TaxNumber,
                person.Title,
                person.FirstName,
                person.LastName,
                person.MaidenName ?? string.Empty,
                person.BirthDate.ToString("d"),
                person.BirthPlace,
                person.CurrentAddress.City,
                person.Mother.FullName,
                person.Father.FullName);

            _payersGrid.Rows[index].Tag = person;
        }
    }

    /// <summary>Refreshes the bottom "details at a glance" region for the selected payer.</summary>
    private void UpdateDetails()
    {
        Person? person = SelectedPerson;

        if (person is null)
        {
            return;
        }

        _lblTaxNumber.Text = person.TaxNumber;
        _lblName.Text = person.FullName;

        _valName.Text = person.FullName;
        _valBirth.Text = $"{person.BirthDate:d} — {person.BirthPlace}";
        _valAddress.Text = person.CurrentAddress.ToString();
        _valPrevAddress.Text = person.PreviousAddress?.ToString() ?? "—";
        _valMaiden.Text = person.MaidenName ?? "—";
        _valMother.Text = $"{person.Mother.FullName} ({person.Mother.BirthDate:d})";
        _valFather.Text = $"{person.Father.FullName} ({person.Father.BirthDate:d})";
        _valContacts.Text = string.Join("  •  ", person.Contacts.Select(c => $"{c.Kind}: {c.Value}"));

        LoadDeclarations(person);
    }

    private void LoadDeclarations(Person person)
    {
        _declarationsGrid.Rows.Clear();

        foreach (Declaration declaration in person.Declarations)
        {
            int index = _declarationsGrid.Rows.Add(
                declaration.Year,
                declaration.AssessmentBasis.ToString("N2"),
                declaration.AssessedTax.ToString("N2"),
                declaration.DueDate.ToString("d"),
                declaration.OutstandingAmount.ToString("N2"),
                declaration.Status.ToString(),
                ObligationShort(declaration.Obligation));

            _declarationsGrid.Rows[index].Tag = declaration;
        }
    }

    /// <summary>Renders the obligation enum as a compact grid label.</summary>
    private static string ObligationShort(TaxObligation obligation)
        => obligation is TaxObligation.LohnsteuerUndEinkommensteuer ? "LSt + ESt" : "ESt";

    /// <summary>Color-codes the Status cell by declaration status, theme-aware.</summary>
    private void OnDeclarationCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _declarationsGrid.Columns[e.ColumnIndex] != _dcolStatus)
        {
            return;
        }

        if (_declarationsGrid.Rows[e.RowIndex].Tag is Declaration declaration)
        {
            e.CellStyle!.ForeColor = _theme.StatusColor(declaration.Status);
        }
    }

    private void OnDeclarationDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            OpenSelectedDeclaration();
        }
    }

    private void EditSelectedPerson()
    {
        Person? person = SelectedPerson;

        if (person is null)
        {
            return;
        }

        // PersonForm is modal (only one instance at a time). On save it commits into the
        // repository's Person object, so we refresh the row afterwards.
        using PersonForm editor = new(person);

        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            RefreshSelectedPayerRow(person);
            UpdateDetails();
        }
    }

    private void OpenSelectedDeclaration()
    {
        Person? person = SelectedPerson;
        Declaration? declaration = SelectedDeclaration ?? person?.Declarations.FirstOrDefault();

        if (person is null || declaration is null)
        {
            return;
        }

        // DeclarationForm is modeless / free-floating; several may be open at once.
        DeclarationForm form = new(person, declaration);
        form.Show(this);
    }

    private void RefreshSelectedPayerRow(Person person)
    {
        if (_payersGrid.CurrentRow is not { } row)
        {
            return;
        }

        row.Cells[_colTaxNumber.Index].Value = person.TaxNumber;
        row.Cells[_colTitle.Index].Value = person.Title;
        row.Cells[_colFirstName.Index].Value = person.FirstName;
        row.Cells[_colLastName.Index].Value = person.LastName;
        row.Cells[_colMaiden.Index].Value = person.MaidenName ?? string.Empty;
        row.Cells[_colBirthDate.Index].Value = person.BirthDate.ToString("d");
        row.Cells[_colBirthPlace.Index].Value = person.BirthPlace;
        row.Cells[_colCity.Index].Value = person.CurrentAddress.City;
    }

    /// <summary>Re-applies all localized UI chrome (window title, menus, columns, captions).</summary>
    private void ApplyLocalization()
    {
        Text = _localizer[StringKeys.MainTitle];

        _viewMenu.Text = _localizer[StringKeys.MenuView];
        _languageMenu.Text = _localizer[StringKeys.MenuLanguage];
        _menuEnglish.Text = _localizer[StringKeys.LanguageEnglish];
        _menuGerman.Text = _localizer[StringKeys.LanguageGerman];
        _themeMenu.Text = _localizer[StringKeys.MenuTheme];
        _menuClassic.Text = _localizer[StringKeys.ThemeClassic];
        _menuDark.Text = _localizer[StringKeys.ThemeDark];

        _btnEditPerson.Text = _localizer[StringKeys.CmdEditPerson];
        _btnEditPerson.ToolTipText = _localizer[StringKeys.CmdEditPerson];
        _btnOpenDeclaration.Text = _localizer[StringKeys.CmdOpenDeclaration];
        _btnOpenDeclaration.ToolTipText = _localizer[StringKeys.CmdOpenDeclaration];

        _detailGroup.Text = _localizer[StringKeys.MainDetails];
        _declarationsGroup.Text = _localizer[StringKeys.MainDeclarations];

        _capName.Text = _localizer[StringKeys.FieldName];
        _capBirth.Text = _localizer[StringKeys.FieldBirth];
        _capAddress.Text = _localizer[StringKeys.FieldAddress];
        _capPrevAddress.Text = _localizer[StringKeys.FieldPreviousAddress];
        _capMaiden.Text = _localizer[StringKeys.FieldMaidenName];
        _capMother.Text = _localizer[StringKeys.FieldMother];
        _capFather.Text = _localizer[StringKeys.FieldFather];
        _capContacts.Text = _localizer[StringKeys.FieldContacts];

        _colTaxNumber.HeaderText = _localizer[StringKeys.ColTaxNumber];
        _colTitle.HeaderText = _localizer[StringKeys.ColTitle];
        _colFirstName.HeaderText = _localizer[StringKeys.ColFirstName];
        _colLastName.HeaderText = _localizer[StringKeys.ColLastName];
        _colMaiden.HeaderText = _localizer[StringKeys.ColMaidenName];
        _colBirthDate.HeaderText = _localizer[StringKeys.ColBirthDate];
        _colBirthPlace.HeaderText = _localizer[StringKeys.ColBirthPlace];
        _colCity.HeaderText = _localizer[StringKeys.ColCity];
        _colMother.HeaderText = _localizer[StringKeys.ColMother];
        _colFather.HeaderText = _localizer[StringKeys.ColFather];

        _dcolYear.HeaderText = _localizer[StringKeys.ColYear];
        _dcolBasis.HeaderText = _localizer[StringKeys.ColAssessmentBasis];
        _dcolTax.HeaderText = _localizer[StringKeys.ColAssessedTax];
        _dcolDue.HeaderText = _localizer[StringKeys.ColDueDate];
        _dcolOutstanding.HeaderText = _localizer[StringKeys.ColOutstanding];
        _dcolStatus.HeaderText = _localizer[StringKeys.ColStatus];
        _dcolObligation.HeaderText = _localizer[StringKeys.ColObligation];
    }

    /// <summary>Applies the active theme to both grids and the bold header labels.</summary>
    private void ApplyTheme()
    {
        _payersGrid.ApplyScheme(_theme);
        _declarationsGrid.ApplyScheme(_theme);

        Font headerFont = new(Font.FontFamily, Font.Size + 2f, FontStyle.Bold);
        _headerFont?.Dispose();
        _headerFont = headerFont;
        _lblTaxNumber.Font = headerFont;
        _lblName.Font = headerFont;

        // Refresh the toolbar glyphs in a color that reads on the active theme, disposing the
        // previous bitmaps to avoid GDI leaks across theme switches.
        _btnEditPerson.Image?.Dispose();
        _btnOpenDeclaration.Image?.Dispose();
        _btnEditPerson.Image = IconFactory.GetIcon(FluentGlyph.Contact, 36, _theme.IconColor);
        _btnOpenDeclaration.Image = IconFactory.GetIcon(FluentGlyph.OpenFile, 36, _theme.IconColor);

        _declarationsGrid.Invalidate();
    }

    private void UpdateMenuChecks()
    {
        _menuEnglish.Checked = _localizer.Language is AppLanguage.English;
        _menuGerman.Checked = _localizer.Language is AppLanguage.German;
        _menuClassic.Checked = _theme.Theme is AppTheme.Classic;
        _menuDark.Checked = _theme.Theme is AppTheme.Dark;
    }
}
