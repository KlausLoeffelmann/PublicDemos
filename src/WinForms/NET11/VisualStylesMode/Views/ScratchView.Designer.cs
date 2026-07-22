// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

partial class ScratchView
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - the interactive prototype UI is built in code (see
    ///  <see cref="BuildPrototypeUi"/>), so this shell only carries the standard container and the
    ///  UserControl's own scaling metadata.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        // 
        // ScratchView
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        Name = "ScratchView";
        Size = new Size(1128, 832);
        ResumeLayout(false);
    }

    #endregion
}
