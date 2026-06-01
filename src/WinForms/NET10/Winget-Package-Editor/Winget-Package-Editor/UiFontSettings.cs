namespace Winget_Package_Editor;

internal sealed class UiFontSettings
{
    public string FontFamily { get; set; } = "Segoe UI";

    public float MenuStripSize { get; set; } = 11F;

    public float StandardSize { get; set; } = 10F;

    public float TreeMainNodeDelta { get; set; } = 1F;

    public float StatusStripSize { get; set; } = 10F;

    public bool TreeMainNodeBold { get; set; } = true;

    public UiFontSettings Clone()
    {
        return new UiFontSettings
        {
            FontFamily = FontFamily,
            MenuStripSize = MenuStripSize,
            StandardSize = StandardSize,
            TreeMainNodeDelta = TreeMainNodeDelta,
            StatusStripSize = StatusStripSize,
            TreeMainNodeBold = TreeMainNodeBold
        };
    }
}
