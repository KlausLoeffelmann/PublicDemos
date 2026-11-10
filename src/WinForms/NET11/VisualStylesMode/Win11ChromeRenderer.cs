// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;

namespace VisualStylesModeDemo;

internal static class Win11ChromeRenderer
{
    internal static void Draw(
        Graphics graphics,
        Win11FormBaseClass form,
        Font captionFont,
        bool active,
        int hotButton,
        int pressedButton)
    {
        bool highContrast = SystemInformation.HighContrast;
        bool dark = Application.IsDarkModeEnabled && !highContrast;
        Color background = highContrast
            ? active ? SystemColors.ActiveCaption : SystemColors.InactiveCaption
            : dark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(243, 243, 243);
        Color foreground = highContrast
            ? active ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText
            : active ? dark ? Color.White : Color.FromArgb(26, 26, 26)
            : dark ? Color.FromArgb(153, 153, 153) : Color.FromArgb(128, 128, 128);

        using SolidBrush backgroundBrush = new(background);
        graphics.FillRectangle(backgroundBrush, 0, 0, form.ClientSize.Width, form.TitleBarHeight);

        if (form.ShowIcon && form.ControlBox && form.Icon is Icon icon)
        {
            graphics.DrawIcon(icon, form.IconBounds);
        }

        TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
            | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping;
        if (form.RightToLeft == RightToLeft.Yes && form.RightToLeftLayout)
        {
            flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;
        }

        TextRenderer.DrawText(graphics, form.Text, captionFont, form.TitleBounds, foreground, flags);

        foreach (int hit in Win11FormBaseClass.CaptionButtons)
        {
            Rectangle bounds = form.GetCaptionButtonBounds(hit);
            if (bounds.IsEmpty)
            {
                continue;
            }

            bool enabled = form.IsCaptionButtonEnabled(hit);
            bool hovered = hotButton == hit && enabled;
            bool pressed = hovered && pressedButton == hit;
            Color glyphColor = enabled ? foreground : highContrast ? SystemColors.GrayText
                : dark ? Color.FromArgb(90, 90, 90) : Color.FromArgb(170, 170, 170);
            if (hovered)
            {
                Color hoverColor = highContrast ? SystemColors.Highlight
                    : hit == Win11FormBaseClass.HtClose
                        ? Color.FromArgb(196, 43, 28)
                        : dark ? Color.FromArgb(pressed ? 45 : 55, pressed ? 45 : 55, pressed ? 45 : 55)
                        : Color.FromArgb(pressed ? 218 : 229, pressed ? 218 : 229, pressed ? 218 : 229);
                using SolidBrush hoverBrush = new(hoverColor);
                graphics.FillRectangle(hoverBrush, bounds);
                if (hit == Win11FormBaseClass.HtClose)
                {
                    glyphColor = highContrast ? SystemColors.HighlightText
                        : pressed ? Color.FromArgb(230, 230, 230) : Color.White;
                }
                else if (highContrast)
                {
                    glyphColor = SystemColors.HighlightText;
                }
            }

            DrawGlyph(graphics, bounds, hit, form.WindowState == FormWindowState.Maximized,
                glyphColor, form.DeviceDpi / 96f);
        }

        if (form.WindowState != FormWindowState.Maximized && form.ClientSize.Width > 0 && form.ClientSize.Height > 0)
        {
            Color border = highContrast ? SystemColors.WindowFrame
                : dark ? Color.FromArgb(active ? 90 : 55, active ? 90 : 55, active ? 90 : 55)
                : Color.FromArgb(active ? 143 : 204, active ? 143 : 204, active ? 143 : 204);
            using Pen borderPen = new(border);
            graphics.DrawRectangle(borderPen, 0, 0, form.ClientSize.Width - 1, form.ClientSize.Height - 1);
        }
    }

    private static void DrawGlyph(Graphics graphics, Rectangle bounds, int hit, bool maximized, Color color, float scale)
    {
        int side = Math.Max(8, (int)Math.Round(10 * scale));
        int left = bounds.Left + (bounds.Width - side) / 2;
        int top = bounds.Top + (bounds.Height - side) / 2;
        using Pen pen = new(color, Math.Max(1, (int)Math.Round(scale)));
        GraphicsState state = graphics.Save();
        graphics.SmoothingMode = SmoothingMode.None;

        switch (hit)
        {
            case Win11FormBaseClass.HtMinButton:
                graphics.DrawLine(pen, left, top + side / 2, left + side, top + side / 2);
                break;
            case Win11FormBaseClass.HtMaxButton when maximized:
                int offset = Math.Max(2, (int)Math.Round(2 * scale));

                graphics.DrawLines(pen,
                [
                    new Point(left + offset, top + offset), new Point(left + offset, top),
                    new Point(left + side, top), new Point(left + side, top + side - offset),
                    new Point(left + side - offset, top + side - offset)
                ]);

                graphics.DrawRectangle(pen, left, top + offset, side - offset, side - offset);
                break;
            case Win11FormBaseClass.HtMaxButton:
                graphics.DrawRectangle(pen, left, top, side, side);
                break;
            case Win11FormBaseClass.HtClose:
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.DrawLine(pen, left, top, left + side, top + side);
                graphics.DrawLine(pen, left + side, top, left, top + side);
                break;
        }

        graphics.Restore(state);
    }
}