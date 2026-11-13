// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Globalization;
using VisualStylesModeDemo.Components;
using static VisualStylesModeDemo.Views.NativeMethods;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  A dense, responsive customer-entry form that exercises common WinForms input controls and a
///  notes editing toolbar above the RichTextBox.
/// </summary>
public partial class CustomerEntryView : UserControl, IScenarioView
{
    private const int EmSetRect = 0x00B3;

    public CustomerEntryView()
    {
        InitializeComponent();

        _birthDateTextBox.Text 
            = new DateTime(1987, 6, 15)
                .ToString("d", CultureInfo.CurrentCulture);

        _customerSinceTextBox.Text 
            = DateTime.Today.ToString("d", CultureInfo.CurrentCulture);

        PopulateSelections();
        UpdateFormattingButtons();
    }

    public string DisplayName 
        => "Customer Entry Form";

    /// <summary>Enables the bounded startup-only reveal comparison outside the Designer.</summary>
    internal void EnableRevealSlowdown()
    {
        components.Add(new CustomerRevealSlowdown(
            root: this,
            delayMilliseconds: DemoConstants.CustomerFirstPaintDelayMilliseconds,
            budgetMilliseconds: DemoConstants.CustomerRevealBudgetMilliseconds));
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ApplyNotesToolStripImages();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);

        if (IsHandleCreated)
        {
            ApplyNotesToolStripImages();
        }
    }

    private void PopulateSelections()
    {
        _titleComboBox.Items.AddRange(["Ms.", "Mr.", "Mx.", "Dr.", "Prof."]);
        _preferredContactComboBox.Items.AddRange(["Email", "Mobile phone", "Home phone", "Postal mail"]);
        _countryComboBox.Items.AddRange(["United States", "Canada", "Germany", "United Kingdom", "Other"]);
        _customerTypeComboBox.Items.AddRange(["Retail", "Business", "Government", "Non-profit"]);
        _accountStatusComboBox.Items.AddRange(["Prospect", "Active", "On hold", "Closed"]);
        _languageComboBox.Items.AddRange(["English", "German", "French", "Spanish"]);
        _timeZoneComboBox.Items.AddRange(["Pacific", "Mountain", "Central", "Eastern", "Central European"]);

        _titleComboBox.SelectedIndex = 1;
        _preferredContactComboBox.SelectedIndex = 0;
        _countryComboBox.SelectedIndex = 0;
        _customerTypeComboBox.SelectedIndex = 0;
        _accountStatusComboBox.SelectedIndex = 1;
        _languageComboBox.SelectedIndex = 0;
        _timeZoneComboBox.SelectedIndex = 0;
    }

    private void ApplyNotesToolStripImages()
    {
        Color iconColor = SystemColors.ControlText;

        (ToolStripButton tsButton, SymbolGlyph glyph)[] tsButtons =
        [
            (_cutToolStripButton, SymbolGlyph.Cut),
            (_copyToolStripButton, SymbolGlyph.Copy),
            (_pasteToolStripButton, SymbolGlyph.Paste),
            (_boldToolStripButton, SymbolGlyph.Bold),
            (_italicToolStripButton, SymbolGlyph.Italic),
            (_underlineToolStripButton, SymbolGlyph.Underline)
        ];

        Array.ForEach(
            tsButtons,
            action: button => _iconFactoryComponent.SetImage(
                item: button.tsButton,
                glyph: button.glyph,
                logicalSize: 24,
                dpi: DeviceDpi,
                color: iconColor));
            }

    private void CutToolStripButton_Click(object sender, EventArgs e)
        => _notesRichTextBox.Cut();

    private void CopyToolStripButton_Click(object sender, EventArgs e)
        => _notesRichTextBox.Copy();

    private void PasteToolStripButton_Click(object sender, EventArgs e)
        => _notesRichTextBox.Paste();

    private void BoldToolStripButton_Click(object sender, EventArgs e)
        => ToggleSelectionStyle(FontStyle.Bold);

    private void ItalicToolStripButton_Click(object sender, EventArgs e)
        => ToggleSelectionStyle(FontStyle.Italic);

    private void UnderlineToolStripButton_Click(object sender, EventArgs e)
        => ToggleSelectionStyle(FontStyle.Underline);

    private void NotesRichTextBox_SelectionChanged(object sender, EventArgs e)
        => UpdateFormattingButtons();

    private void NotesRichTextBox_HandleCreated(object sender, EventArgs e) 
        => ApplyNotesTextPadding();

    private void NotesRichTextBox_LayoutChanged(object sender, EventArgs e) 
        => ApplyNotesTextPadding();

    private void ApplyNotesTextPadding()
    {
        if (!_notesRichTextBox.IsHandleCreated)
        {
            return;
        }

        Padding padding = _notesRichTextBox.Padding;

        NativeRect formattingRectangle = new(
            left: padding.Left,
            top: padding.Top,
            right: Math.Max(padding.Left, _notesRichTextBox.ClientSize.Width - padding.Right),
            bottom: Math.Max(padding.Top, _notesRichTextBox.ClientSize.Height - padding.Bottom));

        SendMessage(_notesRichTextBox.Handle, EmSetRect, 0, ref formattingRectangle);
        _notesRichTextBox.Invalidate();
    }

    private void ToggleSelectionStyle(FontStyle style)
    {
        Font sourceFont = _notesRichTextBox.SelectionFont ?? _notesRichTextBox.Font;
        FontStyle newStyle = sourceFont.Style ^ style;

        using Font selectionFont = new(sourceFont, newStyle);

        _notesRichTextBox.SelectionFont = selectionFont;
        UpdateFormattingButtons();
        _notesRichTextBox.Focus();
    }

    private void UpdateFormattingButtons()
    {
        FontStyle style = _notesRichTextBox.SelectionFont?.Style 
            ?? FontStyle.Regular;

        _boldToolStripButton.Checked 
            = (style & FontStyle.Bold) != 0;

        _italicToolStripButton.Checked 
            = (style & FontStyle.Italic) != 0;

        _underlineToolStripButton.Checked 
            = (style & FontStyle.Underline) != 0;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (_notesRichTextBox.ContainsFocus)
        {
            FontStyle style = keyData switch
            {
                Keys.Control | Keys.B => FontStyle.Bold,
                Keys.Control | Keys.I => FontStyle.Italic,
                Keys.Control | Keys.U => FontStyle.Underline,
                _ => FontStyle.Regular,
            };

            if (style != FontStyle.Regular)
            {
                ToggleSelectionStyle(style);
                return true;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }
}

/// <summary>
///  Diagnostic ComboBox that traces native messages while 
///  investigating rendering.
/// </summary>
public class ComboBoxEx : ComboBox
{
    /// <inheritdoc/>
    protected override void DefWndProc(ref Message m)
    {
        base.DefWndProc(ref m);
        Debug.WriteLine(
            $"[{DateTime.Now:HH:mm:ss-fff}]ComboBoxEx:" +
            $" {m.Msg} - {m.WParam} - {m.LParam}");
    }
}
