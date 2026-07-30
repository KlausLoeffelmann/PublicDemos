using MaterialKeys;

namespace MaterialKeys.Demo;

/// <summary>
///  A small modal dialog used to visually verify the <see cref="MaterialKeyButton"/> default-button
///  and focus cues.
/// </summary>
/// <remarks>
///  <para>
///   The dialog deliberately hosts several selectable keys with one <see cref="Form.AcceptButton"/>
///   (CASH). Because CASH is the accept button it renders the discreet default-button ring even
///   while another key is focused, so pressing <kbd>Enter</kbd> triggers it from anywhere; pressing
///   <kbd>Tab</kbd> walks the focus cue across the other keys. This is the primary manual test for
///   the control's default/focus rendering.
///  </para>
///  <para>
///   Colors are not baked into the designer file: they are applied from the active
///   <see cref="CashRegisterPalette"/> so the dialog follows the same Light/Dark switch as the main
///   window. <see cref="SelectedPayment"/> reports which key the operator chose.
///  </para>
/// </remarks>
public partial class PaymentConfirmDialog : Form
{
    // Maps each key to the palette category that colors it. Built once, in code, so the designer
    // file stays free of dynamic color/category state.
    private readonly Dictionary<MaterialKeyButton, KeyCategory> _keyCategories;

    /// <summary>
    ///  Initializes a new instance of the <see cref="PaymentConfirmDialog"/> class.
    /// </summary>
    public PaymentConfirmDialog()
    {
        InitializeComponent();

        _keyCategories = new Dictionary<MaterialKeyButton, KeyCategory>
        {
            [_cashKey] = KeyCategory.Total,       // the prominent, red, default action
            [_cardKey] = KeyCategory.Department,
            [_voucherKey] = KeyCategory.Department,
            [_cancelKey] = KeyCategory.Function
        };

        foreach (MaterialKeyButton key in _keyCategories.Keys)
        {
            key.Click += PaymentKey_Click;
        }

        ApplyPalette();
    }

    /// <summary>
    ///  Gets the caption of the key the operator chose, or <see langword="null"/> if the dialog was
    ///  cancelled.
    /// </summary>
    public string? SelectedPayment { get; private set; }

    private void PaymentKey_Click(object? sender, EventArgs e)
    {
        // Cancel is reported as no selection; every other key records its caption.
        if (sender is MaterialKeyButton key && key != _cancelKey)
        {
            SelectedPayment = key.Text;
        }
    }

    /// <inheritdoc/>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);

        // Follow a runtime Light/Dark switch while the dialog is open.
        ApplyPalette();
    }

    /// <summary>
    ///  Applies the category colors of the current <see cref="CashRegisterPalette"/> to every key
    ///  and themes the dialog surface.
    /// </summary>
    private void ApplyPalette()
    {
        BackColor = CashRegisterPalette.SurfaceColor;
        _promptLabel.ForeColor = CashRegisterPalette.OnSurfaceColor;

        foreach ((MaterialKeyButton key, KeyCategory category) in _keyCategories)
        {
            KeyStyle style = CashRegisterPalette.Current[category];
            key.BackColor = style.Back;
            key.ForeColor = style.Fore;
            key.BorderColor = style.Border;
        }
    }
}
