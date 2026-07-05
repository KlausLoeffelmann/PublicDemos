// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Input;

namespace ScratchProject.Views;

/// <summary>
///  Minimal <see cref="ICommand"/> implementation used to exercise <see cref="Button.Command"/> /
///  <see cref="Button.CommandParameter"/> in the Button Visual Styles scenario view.
/// </summary>
internal sealed class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
{
    private readonly Action<object?> _execute = execute;
    private readonly Func<object?, bool>? _canExecute = canExecute;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    /// <summary>
    ///  Lets the demo toggle <see cref="CanExecute(object?)"/> (e.g. via an Enabled/Disabled test
    ///  CheckBox) and notify every bound Button to re-query it.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
