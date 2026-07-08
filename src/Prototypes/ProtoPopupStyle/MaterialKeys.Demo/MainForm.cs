using MaterialKeys;

namespace MaterialKeys.Demo;

/// <summary>
///  The main window: a cash-register keypad built entirely from <see cref="MaterialKeyButton"/>s.
/// </summary>
/// <remarks>
///  <para>
///   The keys are laid out in the designer; this file assigns each key to a
///   <see cref="KeyCategory"/> and paints it from the active <see cref="CashRegisterPalette"/>.
///   Number, function, department and the TOTAL key each get their own color, and the whole array
///   switches between the Classic and Dark color sets based on
///   <see cref="Application.IsDarkModeEnabled"/> — both at startup and when Windows changes theme
///   at runtime (<see cref="OnSystemColorsChanged"/>).
///  </para>
///  <para>
///   The TOTAL key is the form's <see cref="Form.AcceptButton"/>, so it renders the default-button
///   cue and responds to <kbd>Enter</kbd>. The "TEST MODAL DIALOG…" key opens
///   <see cref="PaymentConfirmDialog"/> to verify the default/focus cues inside a modal form.
///  </para>
/// </remarks>
public partial class MainForm : Form
{
    // Category assignment lives in code (not the designer) so the two color sets stay in one
    // place and can be swapped dynamically without fighting the control's theme-default plumbing.
    private readonly Dictionary<MaterialKeyButton, KeyCategory> _keyCategories;

    /// <summary>
    ///  Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();

        _keyCategories = new Dictionary<MaterialKeyButton, KeyCategory>
        {
            [_key7] = KeyCategory.Number,
            [_key8] = KeyCategory.Number,
            [_key9] = KeyCategory.Number,
            [_key4] = KeyCategory.Number,
            [_key5] = KeyCategory.Number,
            [_key6] = KeyCategory.Number,
            [_key1] = KeyCategory.Number,
            [_key2] = KeyCategory.Number,
            [_key3] = KeyCategory.Number,
            [_key0] = KeyCategory.Number,
            [_key00] = KeyCategory.Number,
            [_keyDot] = KeyCategory.Number,
            [_clearKey] = KeyCategory.Function,
            [_voidKey] = KeyCategory.Function,
            [_corrKey] = KeyCategory.Function,
            [_openDialogKey] = KeyCategory.Function,
            [_dept1Key] = KeyCategory.Department,
            [_dept2Key] = KeyCategory.Department,
            [_dept3Key] = KeyCategory.Department,
            [_dept4Key] = KeyCategory.Department,
            [_totalKey] = KeyCategory.Total
        };

        // Every key reports its press in the status label; the dialog key has its own handler.
        foreach (MaterialKeyButton key in _keyCategories.Keys)
        {
            key.Click += Key_Click;
        }

        _openDialogKey.Click += OpenDialogKey_Click;

        ApplyPalette();
    }

    private void Key_Click(object? sender, EventArgs e)
    {
        if (sender is MaterialKeyButton key && key != _openDialogKey)
        {
            _statusLabel.Text = $"Pressed: {key.Text}";
        }
    }

    private void OpenDialogKey_Click(object? sender, EventArgs e)
    {
        using PaymentConfirmDialog dialog = new();

        _statusLabel.Text = dialog.ShowDialog(this) == DialogResult.OK
            ? $"Paid with: {dialog.SelectedPayment}"
            : "Payment cancelled";
    }

    /// <inheritdoc/>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);

        // Repaint the whole array with the color set that matches the new Windows theme.
        ApplyPalette();
    }

    /// <summary>
    ///  Applies the active <see cref="CashRegisterPalette"/> to every key and themes the form
    ///  surface and labels.
    /// </summary>
    private void ApplyPalette()
    {
        BackColor = CashRegisterPalette.SurfaceColor;
        _headerLabel.ForeColor = CashRegisterPalette.OnSurfaceColor;
        _statusLabel.ForeColor = CashRegisterPalette.OnSurfaceColor;

        foreach ((MaterialKeyButton key, KeyCategory category) in _keyCategories)
        {
            KeyStyle style = CashRegisterPalette.Current[category];
            key.BackColor = style.Back;
            key.ForeColor = style.Fore;
            key.BorderColor = style.Border;
        }
    }
}
