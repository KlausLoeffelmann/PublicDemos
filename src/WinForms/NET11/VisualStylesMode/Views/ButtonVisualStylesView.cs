// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Input;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Visual/functional matrix for plain <see cref="Button"/> controls: FlatStyle variations, the new
///  .NET 11 per-control <see cref="Control.VisualStylesMode"/>, Enabled/Disabled, Command/CommandParameter
///  binding (see <see cref="RelayCommand"/>), and BackgroundImage/BackgroundImageLayout combinations.
///  Every button has a companion CheckBox so any combination of buttons can be pushed into the
///  shared PropertyGrid at once.
/// </summary>
public partial class ButtonVisualStylesView : UserControl, IScenarioView
{
    private readonly CheckBox[] _checkBoxes;
    private readonly RelayCommand _sharedCommand;
    private bool _commandCanExecute = true;

    public ButtonVisualStylesView()
    {
        InitializeComponent();

        _checkBoxes =
        [
            _flatStyleStandardCheckBox,
            _flatStylePopupCheckBox,
            _flatStyleFlatCheckBox,
            _flatStyleSystemCheckBox,
            _visualStylesClassicCheckBox,
            _visualStylesNet11CheckBox,
            _visualStylesLatestCheckBox,
            _enabledButtonCheckBox,
            _disabledButtonCheckBox,
            _commandAlphaCheckBox,
            _commandBetaCheckBox,
            _commandToggleEnabledCheckBox,
            _backgroundImageTileCheckBox,
            _backgroundImageStretchCheckBox,
            _backgroundImageZoomCheckBox,
            _backgroundImageCenterCheckBox,
        ];

        foreach ((CheckBox checkBox, Control target) in new (CheckBox, Control)[]
        {
            (_flatStyleStandardCheckBox, _flatStyleStandardButton),
            (_flatStylePopupCheckBox, _flatStylePopupButton),
            (_flatStyleFlatCheckBox, _flatStyleFlatButton),
            (_flatStyleSystemCheckBox, _flatStyleSystemButton),
            (_visualStylesClassicCheckBox, _visualStylesClassicButton),
            (_visualStylesNet11CheckBox, _visualStylesNet11Button),
            (_visualStylesLatestCheckBox, _visualStylesLatestButton),
            (_enabledButtonCheckBox, _enabledButton),
            (_disabledButtonCheckBox, _disabledButton),
            (_commandAlphaCheckBox, _commandAlphaButton),
            (_commandBetaCheckBox, _commandBetaButton),
            (_commandToggleEnabledCheckBox, _commandToggleEnabledButton),
            (_backgroundImageTileCheckBox, _backgroundImageTileButton),
            (_backgroundImageStretchCheckBox, _backgroundImageStretchButton),
            (_backgroundImageZoomCheckBox, _backgroundImageZoomButton),
            (_backgroundImageCenterCheckBox, _backgroundImageCenterButton),
        })
        {
            ScenarioSelectionHelper.Bind(checkBox, target, CheckBox_CheckedChanged);
        }

        // A single shared ICommand bound to two buttons with different CommandParameter values,
        // exercising Button.Command / Button.CommandParameter (see RelayCommand.cs).
        _sharedCommand = new RelayCommand(OnCommandExecuted, _ => _commandCanExecute);
        _commandAlphaButton.Command = _sharedCommand;
        _commandAlphaButton.CommandParameter = "Alpha";
        _commandBetaButton.Command = _sharedCommand;
        _commandBetaButton.CommandParameter = "Beta";
        _commandToggleEnabledButton.Click += CommandToggleEnabledButton_Click;

        // Generated placeholder art for the BackgroundImage scenarios (no external assets needed).
        _backgroundImageTileButton.BackgroundImage = CreateCheckerboardBitmap(new Size(24, 24), Color.SteelBlue, Color.LightSteelBlue);
        _backgroundImageStretchButton.BackgroundImage = CreateGradientBitmap(new Size(120, 60), Color.DarkSlateBlue, Color.MediumPurple);
        _backgroundImageZoomButton.BackgroundImage = CreateGradientBitmap(new Size(60, 60), Color.DarkGreen, Color.YellowGreen);
        _backgroundImageCenterButton.BackgroundImage = CreateGradientBitmap(new Size(40, 40), Color.DarkRed, Color.Orange);
    }

    public event EventHandler? SelectionChanged;

    public string DisplayName => "Button Visual Styles";

    public IReadOnlyList<Control> GetSelectedControls() => ScenarioSelectionHelper.GetChecked(_checkBoxes);

    public void SelectAll()
    {
        foreach (CheckBox checkBox in _checkBoxes)
        {
            checkBox.Checked = true;
        }
    }

    public void ClearSelection()
    {
        foreach (CheckBox checkBox in _checkBoxes)
        {
            checkBox.Checked = false;
        }
    }

    private void CheckBox_CheckedChanged(object? sender, EventArgs e) =>
        SelectionChanged?.Invoke(this, EventArgs.Empty);

    private void OnCommandExecuted(object? parameter) =>
        _commandResultLabel.Text = $"Last command result: executed with parameter '{parameter}'";

    private void CommandToggleEnabledButton_Click(object? sender, EventArgs e)
    {
        _commandCanExecute = !_commandCanExecute;
        _sharedCommand.RaiseCanExecuteChanged();
        _commandResultLabel.Text = $"Last command result: CanExecute is now {_commandCanExecute}";
    }

    /// <summary>
    ///  Builds a small checkerboard bitmap at runtime so the BackgroundImage/Tile scenario doesn't
    ///  need an external image asset.
    /// </summary>
    private static Bitmap CreateCheckerboardBitmap(Size tileSize, Color colorA, Color colorB)
    {
        Bitmap bitmap = new(tileSize.Width, tileSize.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        int half = tileSize.Width / 2;
        using (Brush brushA = new SolidBrush(colorA))
        using (Brush brushB = new SolidBrush(colorB))
        {
            graphics.FillRectangle(brushA, 0, 0, half, half);
            graphics.FillRectangle(brushB, half, 0, half, half);
            graphics.FillRectangle(brushB, 0, half, half, half);
            graphics.FillRectangle(brushA, half, half, half, half);
        }

        return bitmap;
    }

    /// <summary>
    ///  Builds a simple diagonal gradient bitmap at runtime for the BackgroundImage/Stretch,
    ///  /Zoom, and /Center scenarios so no external image asset is required.
    /// </summary>
    private static Bitmap CreateGradientBitmap(Size size, Color startColor, Color endColor)
    {
        Bitmap bitmap = new(size.Width, size.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using System.Drawing.Drawing2D.LinearGradientBrush brush = new(
            new Rectangle(Point.Empty, size),
            startColor,
            endColor,
            System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
        graphics.FillRectangle(brush, new Rectangle(Point.Empty, size));

        return bitmap;
    }
}
