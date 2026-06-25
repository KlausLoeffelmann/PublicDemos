namespace LargeFormSmokeTest.Forms;

using LargeFormSmokeTest.Controls;
using LargeFormSmokeTest.Data;
using LargeFormSmokeTest.Localization;
using LargeFormSmokeTest.Models;
using LargeFormSmokeTest.Sections;
using LargeFormSmokeTest.Theming;

/// <summary>
///  The form under test: one tall, scrollable form densely packed with GroupBox section
///  UserControls (one per Anlage). It is modeless / free-floating, so several years or payers
///  can be open at once. The View menu offers bookmarks that scroll a section into view, and the
///  form opens read-only until the clerk chooses "Edit tax form".
/// </summary>
public partial class DeclarationForm : Form
{
    private readonly ILocalizer _localizer = AppServices.Localizer;
    private readonly ThemeManager _theme = AppServices.Theme;
    private readonly Person _person;
    private readonly Declaration _declaration;
    private readonly DeclarationDetail _detail;

    // All hosted section UserControls, in display order. Built once in the constructor.
    private readonly List<SectionControl> _sections = [];

    // The two big title banners (Einkommensteuer always, Lohnsteuer only when combined).
    private readonly SectionLabel _titleEinkommensteuer = new();
    private SectionLabel? _titleLohnsteuer;

    private int _hostRow;
    private bool _isEditing;

    /// <summary>Initializes the declaration form for a given payer and tax year.</summary>
    public DeclarationForm(Person person, Declaration declaration)
    {
        _person = person;
        _declaration = declaration;

        // The detailed Anlage values are produced deterministically and used as the single source
        // of truth for every section's bound fields.
        _detail = DeclarationDetailFactory.Create(person, declaration);

        InitializeComponent();

        BuildSections();
        BuildBookmarks();
        WireEvents();

        ApplyLocalization();
        ApplyTheme();

        // The form always opens locked; editing is an explicit, deliberate action.
        SetEditing(false);
    }

    /// <summary>
    ///  Instantiates the Anlage sections and lays them out in the 2-column scrolling host. Title
    ///  banners and tall sections span both columns; the remaining Anlagen are paired side by side
    ///  to make the layout genuinely two-dimensional (and more layout-intensive to measure).
    /// </summary>
    private void BuildSections()
    {
        // Create the sections once, in display order. The list drives bookmarks, read-only state,
        // localization and data binding.
        MantelbogenSection mantelbogen = new();
        AnlageNSection anlageN = new();
        AnlageKapSection anlageKap = new();
        AnlageVSection anlageV = new();
        AnlageGSection anlageG = new();
        AnlageSSection anlageS = new();
        VorsorgeSection vorsorge = new();
        AnlageKindSection anlageKind = new();
        SonderausgabenSection sonderausgaben = new();
        AussergewBelastungenSection aussergew = new();
        AnlageRSection anlageR = new();
        AnlageSoSection anlageSo = new();
        AnlageUnterhaltSection unterhalt = new();
        EnergetischeMassnahmenSection energetische = new();
        LohnsteuerbescheinigungSection lohnsteuer = new();

        _sections.AddRange([
            mantelbogen, anlageN, anlageKap, anlageV, anlageG, anlageS, vorsorge, anlageKind,
            sonderausgaben, aussergew, anlageR, anlageSo, unterhalt, energetische, lohnsteuer
        ]);

        bool combined = _declaration.Obligation is TaxObligation.LohnsteuerUndEinkommensteuer;

        _host.SuspendLayout();

        // ---- Einkommensteuer block ----
        AddFullWidth(_titleEinkommensteuer);
        AddFullWidth(mantelbogen);
        AddPair(anlageN, anlageKap);
        AddPair(anlageV, anlageG);
        AddPair(anlageS, vorsorge);
        AddPair(anlageKind, sonderausgaben);
        AddPair(aussergew, anlageR);
        AddPair(anlageSo, unterhalt);
        AddPair(energetische, null);

        // ---- Lohnsteuer block (second title only when both returns are owed) ----
        if (combined)
        {
            _titleLohnsteuer = new SectionLabel();
            AddFullWidth(_titleLohnsteuer);
        }

        AddFullWidth(lohnsteuer);

        // Populate every section from the deterministic detail.
        foreach (SectionControl section in _sections)
        {
            section.LoadData(_person, _declaration, _detail);
        }

        _host.ResumeLayout(true);
    }

    /// <summary>Adds a control spanning both host columns and advances to the next row.</summary>
    private void AddFullWidth(Control control)
    {
        BeginRow();
        control.Dock = DockStyle.Top;
        control.Margin = new Padding(6);
        _host.Controls.Add(control, 0, _hostRow);
        _host.SetColumnSpan(control, 2);
        _hostRow++;
    }

    /// <summary>Adds a left/right pair into one host row (right may be <see langword="null"/>).</summary>
    private void AddPair(Control left, Control? right)
    {
        BeginRow();

        left.Dock = DockStyle.Top;
        left.Margin = new Padding(6);
        _host.Controls.Add(left, 0, _hostRow);

        if (right is not null)
        {
            right.Dock = DockStyle.Top;
            right.Margin = new Padding(6);
            _host.Controls.Add(right, 1, _hostRow);
        }

        _hostRow++;
    }

    /// <summary>Ensures an AutoSize row style exists for the row about to be filled.</summary>
    private void BeginRow()
    {
        while (_host.RowStyles.Count <= _hostRow)
        {
            _host.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }
    }

    /// <summary>Creates one "Go to section" menu entry per section that scrolls it into view.</summary>
    private void BuildBookmarks()
    {
        foreach (SectionControl section in _sections)
        {
            ToolStripMenuItem item = new() { Tag = section };
            item.Click += (_, _) => _host.ScrollControlIntoView(section);
            _viewMenu.DropDownItems.Add(item);
        }
    }

    private void WireEvents()
    {
        _menuEditTaxForm.Click += (_, _) => SetEditing(true);
        _btnEdit.Click += (_, _) => SetEditing(true);

        _menuSaveChanges.Click += (_, _) => SaveChanges();
        _btnSave.Click += (_, _) => SaveChanges();

        _menuSaveAndClose.Click += (_, _) => SaveAndClose();
        _menuCloseWithoutSaving.Click += (_, _) => CloseWithoutSaving();

        _menuExport.Click += (_, _) => Export();
        _btnExport.Click += (_, _) => Export();

        _menuFileClose.Click += (_, _) => Close();
        _btnClose.Click += (_, _) => Close();

        FormClosing += OnFormClosing;

        // Use named handlers so a closed (disposed) modeless form can unsubscribe from the
        // app-wide language/theme events and not run ApplyLocalization/ApplyTheme on dead controls.
        _localizer.LanguageChanged += OnLanguageChanged;
        _theme.ThemeChanged += OnThemeChanged;
        FormClosed += (_, _) =>
        {
            _localizer.LanguageChanged -= OnLanguageChanged;
            _theme.ThemeChanged -= OnThemeChanged;
        };

        // Dispose the final toolbar glyph bitmaps when the form goes away (they are owned by us,
        // not by the ToolStrip items).
        Disposed += (_, _) =>
        {
            _btnEdit.Image?.Dispose();
            _btnSave.Image?.Dispose();
            _btnExport.Image?.Dispose();
            _btnClose.Image?.Dispose();
        };
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
        => ApplyLocalization();

    private void OnThemeChanged(object? sender, EventArgs e)
        => ApplyTheme();

    /// <summary>Switches the whole form between read-only and editable, updating chrome state.</summary>
    private void SetEditing(bool editing)
    {
        _isEditing = editing;

        foreach (SectionControl section in _sections)
        {
            section.SetReadOnly(!editing);
        }

        _menuEditTaxForm.Enabled = !editing;
        _btnEdit.Enabled = !editing;
        _menuSaveChanges.Enabled = editing;
        _btnSave.Enabled = editing;
        _menuSaveAndClose.Enabled = editing;
        _menuCloseWithoutSaving.Enabled = editing;

        _banner.Text = editing
            ? _localizer[StringKeys.DeclEditableBanner]
            : _localizer[StringKeys.DeclReadOnlyBanner];

        UpdateBannerColors();
    }

    private void SaveChanges()
    {
        // The demo sections carry synthetic field values, so "saving" simply acknowledges the
        // edit and returns the form to its locked state.
        SetEditing(false);
    }

    private void SaveAndClose()
    {
        SetEditing(false);
        Close();
    }

    private void CloseWithoutSaving()
    {
        _isEditing = false;
        Close();
    }

    private void Export()
        => MessageBox.Show(
            this,
            $"{_person.FullName} — {_declaration.Year}",
            _localizer[StringKeys.CmdExport],
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_isEditing)
        {
            return;
        }

        DialogResult choice = MessageBox.Show(
            this,
            _localizer[StringKeys.DeclEditableBanner],
            _localizer[StringKeys.DeclTitle],
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (choice == DialogResult.No)
        {
            e.Cancel = true;
        }
    }

    /// <summary>
    ///  Re-applies all localized chrome: window title (with obligation), menu, banners, the two
    ///  title bands and every section's caption + bookmark.
    /// </summary>
    private void ApplyLocalization()
    {
        string obligation = _declaration.Obligation is TaxObligation.LohnsteuerUndEinkommensteuer
            ? $"{_localizer[StringKeys.TitleEinkommensteuer]} + {_localizer[StringKeys.TitleLohnsteuer]}"
            : _localizer[StringKeys.TitleEinkommensteuer];

        Text = $"{obligation} — {_person.FullName} ({_declaration.Year})";

        _titleEinkommensteuer.Text = _localizer[StringKeys.TitleEinkommensteuer];

        if (_titleLohnsteuer is not null)
        {
            _titleLohnsteuer.Text = _localizer[StringKeys.TitleLohnsteuer];
        }

        _fileMenu.Text = _localizer[StringKeys.MenuFile];
        _editMenu.Text = _localizer[StringKeys.MenuEdit];
        _viewMenu.Text = _localizer[StringKeys.MenuGoTo];

        _menuExport.Text = _localizer[StringKeys.CmdExport];
        _menuFileClose.Text = _localizer[StringKeys.CmdClose];
        _menuEditTaxForm.Text = _localizer[StringKeys.CmdEditTaxForm];
        _menuSaveChanges.Text = _localizer[StringKeys.CmdSaveChanges];
        _menuSaveAndClose.Text = _localizer[StringKeys.CmdSaveAndClose];
        _menuCloseWithoutSaving.Text = _localizer[StringKeys.CmdCloseWithoutSaving];

        _btnEdit.ToolTipText = _localizer[StringKeys.CmdEditTaxForm];
        _btnSave.ToolTipText = _localizer[StringKeys.CmdSaveChanges];
        _btnExport.ToolTipText = _localizer[StringKeys.CmdExport];
        _btnClose.ToolTipText = _localizer[StringKeys.CmdClose];

        // Re-localize the section titles and the bookmark menu entries together.
        foreach (SectionControl section in _sections)
        {
            section.ApplyLocalization(_localizer);
        }

        foreach (ToolStripItem item in _viewMenu.DropDownItems)
        {
            if (item.Tag is ISection section)
            {
                item.Text = _localizer[section.TitleKey];
            }
        }

        _banner.Text = _isEditing
            ? _localizer[StringKeys.DeclEditableBanner]
            : _localizer[StringKeys.DeclReadOnlyBanner];
    }

    private void ApplyTheme()
    {
        SetButtonImage(_btnEdit, IconFactory.GetIcon(FluentGlyph.Edit, 36, _theme.IconColor));
        SetButtonImage(_btnSave, IconFactory.GetIcon(FluentGlyph.Save, 36, _theme.IconColor));
        SetButtonImage(_btnExport, IconFactory.GetIcon(FluentGlyph.Export, 36, _theme.IconColor));
        SetButtonImage(_btnClose, IconFactory.GetIcon(FluentGlyph.Cancel, 36, _theme.IconColor));

        UpdateBannerColors();
    }

    /// <summary>Replaces a toolbar button's image, disposing the previous one to avoid GDI leaks.</summary>
    private static void SetButtonImage(ToolStripItem item, Image image)
    {
        item.Image?.Dispose();
        item.Image = image;
    }

    /// <summary>Tints the banner so the read-only vs editing state is obvious in both themes.</summary>
    private void UpdateBannerColors()
    {
        if (_isEditing)
        {
            _banner.BackColor = _theme.IsDark ? Color.FromArgb(60, 50, 20) : Color.FromArgb(255, 248, 220);
            _banner.ForeColor = _theme.IsDark ? Color.Gold : Color.FromArgb(120, 80, 0);
        }
        else
        {
            _banner.BackColor = _theme.IsDark ? Color.FromArgb(40, 40, 44) : Color.FromArgb(235, 235, 240);
            _banner.ForeColor = _theme.IsDark ? Color.Gainsboro : Color.FromArgb(70, 70, 70);
        }
    }
}
