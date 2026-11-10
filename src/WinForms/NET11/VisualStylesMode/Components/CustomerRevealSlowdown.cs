// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics;

namespace VisualStylesModeDemo.Components;

/// <summary>
///  Slows each existing child window's first paint without changing layout or reveal semantics.
/// </summary>
internal sealed class CustomerRevealSlowdown : Component
{
    private readonly List<FirstPaintWindow> _windows = [];
    private readonly Stopwatch _elapsed = new();
    private readonly int _delayMilliseconds;
    private readonly int _budgetMilliseconds;

    /// <summary>Attaches a one-shot paint hook to the customer's existing control tree.</summary>
    internal CustomerRevealSlowdown(Control root, int delayMilliseconds, int budgetMilliseconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(delayMilliseconds);
        ArgumentOutOfRangeException.ThrowIfNegative(budgetMilliseconds);
        _delayMilliseconds = delayMilliseconds;
        _budgetMilliseconds = budgetMilliseconds;

        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime
            && delayMilliseconds > 0
            && budgetMilliseconds > 0)
        {
            AttachChildren(root);
        }
    }

    private void AttachChildren(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            _windows.Add(new FirstPaintWindow(child, this));
            AttachChildren(child);
        }
    }

    private void DelayFirstPaint()
    {
        if (!_elapsed.IsRunning)
        {
            _elapsed.Start();
        }

        int remaining = (int)Math.Max(0, _budgetMilliseconds - _elapsed.ElapsedMilliseconds);
        if (remaining > 0)
        {
            // Deliberately block only initial painting. Never pump messages or force extra layouts:
            // either would change the reveal behavior that this demo is meant to compare.
            Thread.Sleep(Math.Min(_delayMilliseconds, remaining));
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (FirstPaintWindow window in _windows)
            {
                window.Detach();
            }

            _windows.Clear();
        }

        base.Dispose(disposing);
    }

    private sealed class FirstPaintWindow : NativeWindow
    {
        private const int WmPaint = 0x000F;
        private readonly Control _control;
        private readonly CustomerRevealSlowdown _owner;
        private bool _painted;

        internal FirstPaintWindow(Control control, CustomerRevealSlowdown owner)
        {
            _control = control;
            _owner = owner;
            control.HandleCreated += Control_HandleCreated;
            control.HandleDestroyed += Control_HandleDestroyed;

            if (control.IsHandleCreated)
            {
                AssignHandle(control.Handle);
            }
        }

        private void Control_HandleCreated(object? sender, EventArgs e) => AssignHandle(_control.Handle);

        private void Control_HandleDestroyed(object? sender, EventArgs e) => ReleaseHandle();

        internal void Detach()
        {
            _control.HandleCreated -= Control_HandleCreated;
            _control.HandleDestroyed -= Control_HandleDestroyed;
            ReleaseHandle();
        }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            bool firstPaint = m.Msg == WmPaint && !_painted;
            if (firstPaint)
            {
                _painted = true;
            }

            base.WndProc(ref m);

            if (firstPaint)
            {
                // Let the real paint finish first; all subsequent messages bypass this hook.
                Detach();
                _owner.DelayFirstPaint();
            }
        }
    }
}
