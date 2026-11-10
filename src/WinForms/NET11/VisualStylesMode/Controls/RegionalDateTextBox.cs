// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace VisualStylesModeDemo.Controls;

public class RegionalDateTextBox : TextBox
{
    private readonly ErrorProvider _errorProvider = new() { BlinkStyle = ErrorBlinkStyle.NeverBlink };

    public RegionalDateTextBox()
    {
        PlaceholderText = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        AccessibleDescription = $"Enter a date in the regional format: {PlaceholderText}.";
        Text = DateTime.Today.ToString("d", CultureInfo.CurrentCulture);
    }

    protected override void OnValidating(CancelEventArgs e)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        if (DateOnly.TryParse(Text, culture, DateTimeStyles.None, out DateOnly date))
        {
            Text = date.ToString("d", culture);
            _errorProvider.SetError(this, string.Empty);
        }
        else
        {
            _errorProvider.SetError(this, $"Enter a valid date ({culture.DateTimeFormat.ShortDatePattern}).");
        }

        base.OnValidating(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _errorProvider.Dispose();
        }

        base.Dispose(disposing);
    }
}
