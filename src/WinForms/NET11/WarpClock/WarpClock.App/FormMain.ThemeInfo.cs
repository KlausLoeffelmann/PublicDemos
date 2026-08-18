using System.Reflection;
using WarpClock.Engine;

namespace WarpClock.App;

public partial class FormMain
{
    private static readonly PropertyInfo? s_oledViewProperty = typeof(WarpClockControl).GetProperty("OledView", BindingFlags.Public | BindingFlags.Instance);
    private RenderThemeInfo _preferredThemeInfoMode = RenderThemeInfo.FadeAlternateScreenSides;

    private void OnOledViewClick(object? sender, EventArgs e)
    {
        if (!SupportsOledView())
        {
            _statusInfo.Text = "OLED view is unavailable with the current engine build.";
            RefreshThemeInfoMenuState();
            return;
        }

        SetOledViewEnabled(!GetOledViewEnabled());
        if (_currentSelection is not null)
        {
            SelectTheme(_currentSelection, ThemeSelectionReason.OledViewToggle, applyThemeDefaults: false);
        }
        else
        {
            ApplyEffectiveThemeInfoMode();
        }

        MarkClockSettingsCustomized();
        _statusInfo.Text = $"OLED view: {(GetOledViewEnabled() ? "On" : "Off")}";
    }

    private void RefreshThemeInfoMenuState()
    {
        _miOledView.Enabled = SupportsOledView();
        _miOledView.Checked = GetOledViewEnabled();
        _miInfoNever.Checked = _preferredThemeInfoMode == RenderThemeInfo.Never;
        _miInfoFixed.Checked = _preferredThemeInfoMode == RenderThemeInfo.FixedPosition;
        _miInfoFadeFixed.Checked = _preferredThemeInfoMode == RenderThemeInfo.FadeInAndOutAtFixedPosition;
        _miInfoFadeSides.Checked = _preferredThemeInfoMode == RenderThemeInfo.FadeAlternateScreenSides;

        bool themeInfoAllowed = !ShouldSuppressThemeInfo();
        _themeInfoMenu.Enabled = themeInfoAllowed;
        _placementMenu.Enabled = themeInfoAllowed
            && _preferredThemeInfoMode is RenderThemeInfo.FixedPosition or RenderThemeInfo.FadeInAndOutAtFixedPosition;
    }

    private void ApplyEffectiveThemeInfoMode()
    {
        _clock.RenderThemeInfo = ShouldSuppressThemeInfo()
            ? RenderThemeInfo.Never
            : _preferredThemeInfoMode;

        RefreshThemeInfoMenuState();
    }

    private bool ShouldSuppressThemeInfo()
        => GetOledViewEnabled() || string.Equals(_current?.Catalog.Source, StockThemeSource, StringComparison.OrdinalIgnoreCase);

    private bool SupportsOledView()
        => s_oledViewProperty is not null;

    private bool GetOledViewEnabled()
    {
        if (s_oledViewProperty is null)
        {
            return false;
        }

        try
        {
            object? value = s_oledViewProperty.GetValue(_clock);
            if (value is bool boolValue)
            {
                return boolValue;
            }

            return value is not null
                && !string.Equals(value.ToString(), "Off", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private void SetOledViewEnabled(bool enabled)
    {
        if (s_oledViewProperty is null)
        {
            return;
        }

        try
        {
            if (s_oledViewProperty.PropertyType == typeof(bool))
            {
                s_oledViewProperty.SetValue(_clock, enabled);
                return;
            }

            if (s_oledViewProperty.PropertyType.IsEnum)
            {
                string name = enabled ? "General" : "Off";
                object enumValue = Enum.Parse(s_oledViewProperty.PropertyType, name, ignoreCase: true);
                s_oledViewProperty.SetValue(_clock, enumValue);
            }
        }
        catch
        {
        }
    }
}
