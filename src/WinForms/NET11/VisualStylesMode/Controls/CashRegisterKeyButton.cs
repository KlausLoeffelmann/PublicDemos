// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Controls;

/// <summary>
///  A standard WinForms push button with the common layout defaults used by every register key.
/// </summary>
internal sealed class CashRegisterKeyButton : Button
{
    public CashRegisterKeyButton()
    {
        Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        Margin = new Padding(3);
        UseVisualStyleBackColor = false;
    }
}
