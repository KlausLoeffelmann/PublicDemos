// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo;

partial class Win11FormBaseClass
{
    private System.ComponentModel.IContainer components = null;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(800, 500);
        FormBorderStyle = FormBorderStyle.None;
        MinimumSize = new Size(240, 120);
        Name = "Win11FormBaseClass";
        Text = "Windows 11 custom chrome";
        ResumeLayout(false);
    }
}