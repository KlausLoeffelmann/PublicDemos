namespace LargeFormSmokeTest.Forms;

using LargeFormSmokeTest.Controls;
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

    // All hosted section UserControls, in display order. Built once in the constructor.
    private readonly List<SectionControl> _sections = [];

    private bool _isEditing;

    /// <summary>Initializes the declaration form for a given payer and tax year.</summary>
    public DeclarationForm(Person person, Declaration declaration)
    {
        _person = person;
        _declaration = declaration;

        InitializeComponent();

        BuildSections();
        BuildBookmarks();
        WireEvents();

        ApplyLocalization();
        ApplyTheme();

        // The form always opens locked; editing is an explicit, deliberate action.
        SetEditing(false);
        ResizeSections();
    }

    /// <summary>Instantiates and stacks the Anlage sections inside the scrolling host.</summary>
    private void BuildSections()
    {
        _sections.Add(new MantelbogenSection());
        _sections.Add(new AnlageNSection());
        _sections.Add(new AnlageKapSection());
        _sections.Add(new AnlageVSection());
        _sections.Add(new AnlageGSection());
        _sections.Add(new AnlageSSection());
        _sections.Add(new VorsorgeSection());
        _sections.Add(new AnlageKindSection());
        _sections.Add(new SonderausgabenSection());

        _host.SuspendLayout();

        foreach (SectionControl section in _sections)
        {
            section.LoadData(_person, _declaration);
            _host.Controls.Add(section);
        }

        _host.ResumeLayout(true);
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

        _host.SizeChanged += (_, _) => ResizeSections();
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
    ///  Stretches every section to the host's usable width. FlowLayoutPanel keeps each child's
    ///  designed width, so we widen them to fill (minus the vertical scrollbar) on every resize.
    /// </summary>
    private void ResizeSections()
    {
        int width = _host.ClientSize.Width - _host.Padding.Horizontal;

        foreach (SectionControl section in _sections)
        {
            section.Width = Math.Max(section.MinimumSize.Width, width - section.Margin.Horizontal);
        }
    }

    private void ApplyLocalization()
    {
        Text = $"{_localizer[StringKeys.DeclTitle]} — {_person.FullName} ({_declaration.Year})";

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
        _btnEdit.Image = IconFactory.GetIcon(FluentGlyph.Edit, 36, _theme.IconColor);
        _btnSave.Image = IconFactory.GetIcon(FluentGlyph.Save, 36, _theme.IconColor);
        _btnExport.Image = IconFactory.GetIcon(FluentGlyph.Export, 36, _theme.IconColor);
        _btnClose.Image = IconFactory.GetIcon(FluentGlyph.Cancel, 36, _theme.IconColor);

        UpdateBannerColors();
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
