// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Identifies which amount the simulated register currently presents to the operator.
/// </summary>
internal enum RegisterDisplayMode
{
    Input,
    Subtotal,
    Total,
}

/// <summary>
///  Identifies an immutable line in the simulated receipt's audit history.
/// </summary>
internal enum ReceiptEntryKind
{
    Item,
    Void,
    Subtotal,
    Total,
}

/// <summary>
///  One printed receipt event. Tax is derived from the live net subtotal rather than stored as an
///  immutable event, so void corrections keep it accurate.
/// </summary>
internal sealed record ReceiptEntry(ReceiptEntryKind Kind, int DepartmentNumber, decimal Amount);

/// <summary>
///  UI-independent state and arithmetic for one simulated cash-register sale.
/// </summary>
internal sealed class CashRegisterTransaction
{
    private const decimal TaxRate = 0.0825M;

    private readonly List<RegisteredItem> _items = [];
    private readonly List<ReceiptEntry> _receiptEntries = [];

    /// <summary>Gets the amount accumulated by denomination keys but not yet registered.</summary>
    public decimal CurrentInput { get; private set; }

    /// <summary>Gets whether the operator has applied the fixed tax rate to this sale.</summary>
    public bool IsTaxApplied { get; private set; }

    /// <summary>Gets whether Total has locked the current receipt.</summary>
    public bool IsFinalized { get; private set; }

    /// <summary>Gets the amount currently shown on the seven-segment display.</summary>
    public decimal DisplayAmount =>
        DisplayMode switch
        {
            RegisterDisplayMode.Input => CurrentInput,
            RegisterDisplayMode.Subtotal => NetSubtotal,
            RegisterDisplayMode.Total => Total,
            _ => CurrentInput,
        };

    /// <summary>Gets the semantic mode shown next to the seven-segment amount.</summary>
    public RegisterDisplayMode DisplayMode { get; private set; } = RegisterDisplayMode.Input;

    /// <summary>Gets the sum of all registered, non-voided department items.</summary>
    public decimal NetSubtotal => _items.Where(static item => !item.IsVoided).Sum(static item => item.Amount);

    /// <summary>Gets the current tax amount, rounded to cents away from zero.</summary>
    public decimal Tax => IsTaxApplied
        ? decimal.Round(NetSubtotal * TaxRate, 2, MidpointRounding.AwayFromZero)
        : 0M;

    /// <summary>Gets the current net total including applied tax.</summary>
    public decimal Total => NetSubtotal + Tax;

    /// <summary>Gets the fixed tax rate used by the simulation.</summary>
    public static decimal AppliedTaxRate => TaxRate;

    /// <summary>Gets the immutable receipt audit events in print order.</summary>
    public IReadOnlyList<ReceiptEntry> ReceiptEntries => _receiptEntries;

    /// <summary>
    ///  Adds a denomination to current input. The first money key after a finalized sale starts a
    ///  fresh receipt.
    /// </summary>
    public void AddDenomination(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (IsFinalized)
        {
            StartNewSale();
        }

        CurrentInput += amount;
        DisplayMode = RegisterDisplayMode.Input;
    }

    /// <summary>
    ///  Registers current input against a department and clears the input.
    /// </summary>
    /// <returns><see langword="true"/> when an item was registered.</returns>
    public bool RegisterDepartment(int departmentNumber)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(departmentNumber, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(departmentNumber, 20);

        if (IsFinalized || CurrentInput <= 0M)
        {
            return false;
        }

        RegisteredItem item = new(departmentNumber, CurrentInput);
        _items.Add(item);
        _receiptEntries.Add(new ReceiptEntry(ReceiptEntryKind.Item, departmentNumber, CurrentInput));

        CurrentInput = 0M;
        DisplayMode = RegisterDisplayMode.Input;
        return true;
    }

    /// <summary>
    ///  Enables 8.25% tax for the current sale.
    /// </summary>
    /// <returns><see langword="true"/> when tax could be applied.</returns>
    public bool ApplyTax()
    {
        if (IsFinalized || CurrentInput > 0M || NetSubtotal <= 0M)
        {
            return false;
        }

        IsTaxApplied = true;
        return true;
    }

    /// <summary>
    ///  Prints a snapshot of the current net subtotal and switches the display to it.
    /// </summary>
    /// <returns><see langword="true"/> when a subtotal was printed.</returns>
    public bool PrintSubtotal()
    {
        if (IsFinalized || CurrentInput > 0M || NetSubtotal <= 0M)
        {
            return false;
        }

        _receiptEntries.Add(new ReceiptEntry(ReceiptEntryKind.Subtotal, 0, NetSubtotal));
        DisplayMode = RegisterDisplayMode.Subtotal;
        return true;
    }

    /// <summary>
    ///  Clears current input, or when input is zero appends a VOID correction for the latest active
    ///  item.
    /// </summary>
    /// <returns><see langword="true"/> when input or a registered item was voided.</returns>
    public bool Void()
    {
        if (IsFinalized)
        {
            return false;
        }

        if (CurrentInput > 0M)
        {
            CurrentInput = 0M;
            DisplayMode = RegisterDisplayMode.Input;
            return true;
        }

        RegisteredItem? item = _items.LastOrDefault(static candidate => !candidate.IsVoided);
        if (item is null)
        {
            return false;
        }

        item.IsVoided = true;
        _receiptEntries.Add(new ReceiptEntry(ReceiptEntryKind.Void, item.DepartmentNumber, item.Amount));
        DisplayMode = RegisterDisplayMode.Input;
        return true;
    }

    /// <summary>
    ///  Prints and locks the final total.
    /// </summary>
    /// <returns><see langword="true"/> when the sale was finalized.</returns>
    public bool FinalizeSale()
    {
        if (IsFinalized || CurrentInput > 0M || NetSubtotal <= 0M)
        {
            return false;
        }

        _receiptEntries.Add(new ReceiptEntry(ReceiptEntryKind.Total, 0, Total));
        CurrentInput = 0M;
        DisplayMode = RegisterDisplayMode.Total;
        IsFinalized = true;
        return true;
    }

    private void StartNewSale()
    {
        _items.Clear();
        _receiptEntries.Clear();
        CurrentInput = 0M;
        IsTaxApplied = false;
        IsFinalized = false;
        DisplayMode = RegisterDisplayMode.Input;
    }

    private sealed class RegisteredItem(int departmentNumber, decimal amount)
    {
        public int DepartmentNumber { get; } = departmentNumber;

        public decimal Amount { get; } = amount;

        public bool IsVoided { get; set; }
    }
}
