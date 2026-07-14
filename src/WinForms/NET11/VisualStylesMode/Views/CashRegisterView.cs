// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Text;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Functional simulation of a mechanical cash register built from standard WinForms push buttons.
/// </summary>
public partial class CashRegisterView : UserControl, IScenarioView, IFlatStyleScenarioView
{
    private static readonly CultureInfo s_currencyCulture = CultureInfo.GetCultureInfo("en-US");

    private readonly CashRegisterTransaction _transaction = new();
    private readonly Button[][] _denominationColumns;
    private readonly Button[] _departmentButtons;
    private readonly Button[] _allRegisterButtons;
    private Font? _ownedReceiptFont;

    public CashRegisterView()
    {
        InitializeComponent();
        _ownedReceiptFont = _receiptTextBox.Font;
        Disposed += CashRegisterView_Disposed;

        _denominationColumns =
        [
            [_thousands9Button, _thousands8Button, _thousands7Button, _thousands6Button, _thousands5Button, _thousands4Button, _thousands3Button, _thousands2Button, _thousands1Button],
            [_hundreds9Button, _hundreds8Button, _hundreds7Button, _hundreds6Button, _hundreds5Button, _hundreds4Button, _hundreds3Button, _hundreds2Button, _hundreds1Button],
            [_tens9Button, _tens8Button, _tens7Button, _tens6Button, _tens5Button, _tens4Button, _tens3Button, _tens2Button, _tens1Button],
            [_ones9Button, _ones8Button, _ones7Button, _ones6Button, _ones5Button, _ones4Button, _ones3Button, _ones2Button, _ones1Button],
            [_tenths9Button, _tenths8Button, _tenths7Button, _tenths6Button, _tenths5Button, _tenths4Button, _tenths3Button, _tenths2Button, _tenths1Button],
            [_hundredths9Button, _hundredths8Button, _hundredths7Button, _hundredths6Button, _hundredths5Button, _hundredths4Button, _hundredths3Button, _hundredths2Button, _hundredths1Button],
        ];

        _departmentButtons =
        [
            _department01Button,
            _department02Button,
            _department03Button,
            _department04Button,
            _department05Button,
            _department06Button,
            _department07Button,
            _department08Button,
            _department09Button,
            _department10Button,
            _department11Button,
            _department12Button,
            _department13Button,
            _department14Button,
            _department15Button,
            _department16Button,
            _department17Button,
            _department18Button,
            _department19Button,
            _department20Button,
        ];

        _allRegisterButtons =
        [
            .. _denominationColumns.SelectMany(static column => column),
            .. _departmentButtons,
            _taxButton,
            _voidButton,
            _subtotalButton,
            _totalButton,
        ];

        decimal[] multipliers = [1000M, 100M, 10M, 1M, 0.1M, 0.01M];
        for (int column = 0; column < _denominationColumns.Length; column++)
        {
            for (int row = 0; row < _denominationColumns[column].Length; row++)
            {
                _denominationColumns[column][row].Tag = (9 - row) * multipliers[column];
                _denominationColumns[column][row].AccessibleName =
                    $"Add {((decimal)_denominationColumns[column][row].Tag!).ToString("C2", s_currencyCulture)}";
            }
        }

        for (int index = 0; index < _departmentButtons.Length; index++)
        {
            _departmentButtons[index].Tag = index + 1;
        }

        ApplyFlatStyle(FlatStyle.Standard);
        RefreshRegister();
    }

    public string DisplayName => "Cash Register";

    public FlatStyle CurrentFlatStyle { get; private set; }

    public void ApplyFlatStyle(FlatStyle flatStyle)
    {
        CurrentFlatStyle = flatStyle;

        foreach (Button button in _allRegisterButtons)
        {
            button.FlatStyle = flatStyle;
        }

        ApplyPalette();
    }

    private void DenominationButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: decimal amount })
        {
            throw new InvalidOperationException("A denomination key is missing its decimal value.");
        }

        _transaction.AddDenomination(amount);
        RefreshRegister();
    }

    private void DepartmentButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: int departmentNumber })
        {
            throw new InvalidOperationException("A department key is missing its department number.");
        }

        if (_transaction.RegisterDepartment(departmentNumber))
        {
            RefreshRegister();
        }
    }

    private void TaxButton_Click(object? sender, EventArgs e)
    {
        if (_transaction.ApplyTax())
        {
            RefreshRegister();
        }
    }

    private void VoidButton_Click(object? sender, EventArgs e)
    {
        if (_transaction.Void())
        {
            RefreshRegister();
        }
    }

    private void SubtotalButton_Click(object? sender, EventArgs e)
    {
        if (_transaction.PrintSubtotal())
        {
            RefreshRegister();
        }
    }

    private void TotalButton_Click(object? sender, EventArgs e)
    {
        if (_transaction.FinalizeSale())
        {
            RefreshRegister();
        }
    }

    private void RefreshRegister()
    {
        _display.Value = _transaction.DisplayAmount;
        _display.DisplayMode = _transaction.DisplayMode;
        RenderReceipt();

        bool hasInput = _transaction.CurrentInput > 0M;
        bool hasItems = _transaction.NetSubtotal > 0M;
        bool saleOpen = !_transaction.IsFinalized;

        foreach (Button departmentButton in _departmentButtons)
        {
            departmentButton.Enabled = saleOpen && hasInput;
        }

        _taxButton.Enabled = saleOpen && hasItems && !_transaction.IsTaxApplied;
        _voidButton.Enabled = saleOpen && (hasInput || hasItems);
        _taxButton.Enabled &= !hasInput;
        _subtotalButton.Enabled = saleOpen && hasItems && !hasInput;
        _totalButton.Enabled = saleOpen && hasItems && !hasInput;
    }

    private void RenderReceipt()
    {
        StringBuilder receipt = new();
        receipt.AppendLine("      CLASSIC REGISTER");
        receipt.AppendLine("--------------------------------");

        ReceiptEntry? finalTotal = null;
        foreach (ReceiptEntry entry in _transaction.ReceiptEntries)
        {
            switch (entry.Kind)
            {
                case ReceiptEntryKind.Item:
                    AppendAmountLine(receipt, $"DEP {entry.DepartmentNumber:00}", entry.Amount);
                    break;
                case ReceiptEntryKind.Void:
                    AppendAmountLine(receipt, $"VOID DEP {entry.DepartmentNumber:00}", -entry.Amount);
                    break;
                case ReceiptEntryKind.Subtotal:
                    receipt.AppendLine("--------------------------------");
                    AppendAmountLine(receipt, "SUBTOTAL", entry.Amount);
                    break;
                case ReceiptEntryKind.Total:
                    finalTotal = entry;
                    break;
            }
        }

        if (_transaction.IsTaxApplied)
        {
            AppendAmountLine(receipt, $"TAX {CashRegisterTransaction.AppliedTaxRate:P2}", _transaction.Tax);
        }

        if (finalTotal is not null)
        {
            receipt.AppendLine("================================");
            AppendAmountLine(receipt, "TOTAL", finalTotal.Amount);
            receipt.AppendLine("          SALE COMPLETE");
        }
        else if (_transaction.ReceiptEntries.Count == 0)
        {
            receipt.AppendLine("             READY");
        }

        _receiptTextBox.Text = receipt.ToString();
        _receiptTextBox.SelectionStart = _receiptTextBox.TextLength;
        _receiptTextBox.ScrollToCaret();
    }

    private static void AppendAmountLine(StringBuilder receipt, string caption, decimal amount)
    {
        string amountText = amount.ToString("C2", s_currencyCulture);
        receipt.Append(caption.PadRight(18));
        receipt.AppendLine(amountText.PadLeft(14));
    }

    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        ApplyPalette();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);

        if (_rootLayout is null || _receiptTextBox is null)
        {
            return;
        }

        float textScale = Font.SizeInPoints / 11F;
        Font receiptFont = new("Consolas", 10F * textScale, FontStyle.Regular, GraphicsUnit.Point);
        Font? oldReceiptFont = _ownedReceiptFont ?? _receiptTextBox.Font;
        _ownedReceiptFont = receiptFont;
        _receiptTextBox.Font = receiptFont;
        oldReceiptFont.Dispose();

        // The 120% text-size layout is the compact design baseline. Above that point, grow the
        // scrollable register surface so accessibility text remains unclipped rather than squeezing
        // fixed key cells.
        float layoutScale = Math.Max(1F, textScale / 1.2F);
        Size contentSize = new(
            (int)Math.Ceiling(1200 * layoutScale),
            (int)Math.Ceiling(820 * layoutScale));
        _rootLayout.Size = contentSize;
        AutoScrollMinSize = contentSize;
    }

    private void CashRegisterView_Disposed(object? sender, EventArgs e)
    {
        _ownedReceiptFont?.Dispose();
        _ownedReceiptFont = null;
    }

    private void ApplyPalette()
    {
        if (CurrentFlatStyle == FlatStyle.System)
        {
            foreach (Button button in _allRegisterButtons)
            {
                button.UseVisualStyleBackColor = true;
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
            }

            return;
        }

        Color surface = SystemColors.Control;
        Color[] denominationColors =
        [
            Blend(surface, Color.LightBlue, 0.30F),
            Blend(surface, Color.LightBlue, 0.20F),
            Blend(surface, Color.Gray, 0.30F),
            Blend(surface, Color.Gray, 0.30F),
            Blend(surface, Color.Gray, 0.20F),
            Blend(surface, Color.Gray, 0.20F),
        ];

        for (int column = 0; column < _denominationColumns.Length; column++)
        {
            foreach (Button button in _denominationColumns[column])
            {
                ApplyButtonColor(button, denominationColors[column]);
            }
        }

        Color[] departmentColors =
        [
            Blend(surface, Color.CornflowerBlue, 0.40F),
            Blend(surface, Color.MediumAquamarine, 0.40F),
            Blend(surface, Color.Plum, 0.40F),
            Blend(surface, Color.LightSalmon, 0.40F),
        ];

        for (int index = 0; index < _departmentButtons.Length; index++)
        {
            ApplyButtonColor(_departmentButtons[index], departmentColors[index / 5]);
        }

        ApplyButtonColor(_taxButton, Color.White);
        ApplyButtonColor(_voidButton, surface);
        ApplyButtonColor(_subtotalButton, Blend(surface, Color.LightGreen, 0.35F));
        ApplyButtonColor(_totalButton, Blend(surface, Color.LightGreen, 0.65F));
    }

    private static void ApplyButtonColor(Button button, Color backColor)
    {
        button.UseVisualStyleBackColor = false;
        button.BackColor = backColor;
        button.ForeColor = GetContrastingTextColor(backColor);
        button.FlatAppearance.BorderColor = Application.IsDarkModeEnabled
            ? ControlPaint.Light(backColor, 0.25F)
            : ControlPaint.Dark(backColor, 0.20F);
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.12F);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.12F);
    }

    private static Color Blend(Color background, Color overlay, float amount)
    {
        float inverse = 1F - amount;
        return Color.FromArgb(
            255,
            (int)Math.Round((background.R * inverse) + (overlay.R * amount)),
            (int)Math.Round((background.G * inverse) + (overlay.G * amount)),
            (int)Math.Round((background.B * inverse) + (overlay.B * amount)));
    }

    private static Color GetContrastingTextColor(Color color)
    {
        double luminance =
            ((0.2126D * color.R) + (0.7152D * color.G) + (0.0722D * color.B)) / 255D;
        return luminance >= 0.55D ? Color.Black : Color.White;
    }
}
