using WingetPackageEditor.Core.Services;

namespace WingetPackageEditor.Tests;

public sealed class WingetListScannerTests
{
    [Fact]
    public void ParseIds_ExtractsIdColumn_FromAlignedOutput()
    {
        string header = "Name".PadRight(21) + "Id".PadRight(26) + "Version".PadRight(13) + "Available".PadRight(10) + "Source";
        string separator = new('-', 80);
        string row1 = "Git".PadRight(21) + "Git.Git".PadRight(26) + "2.43.0".PadRight(13) + "2.44.0".PadRight(10) + "winget";
        string row2 = "PowerShell 7-x64".PadRight(21) + "Microsoft.PowerShell".PadRight(26) + "7.4.1".PadRight(13) + "".PadRight(10) + "winget";
        string row3 = "Some App".PadRight(21) + "Foo.Bar".PadRight(26) + "1.0";

        string output = string.Join("\r\n", "Progress noise...", header, separator, row1, row2, row3);

        IReadOnlyList<string> ids = WingetListScanner.ParseIds(output);

        Assert.Equal(["Git.Git", "Microsoft.PowerShell", "Foo.Bar"], ids);
    }

    [Fact]
    public void ParseIds_ReturnsEmpty_WhenNoHeader()
    {
        Assert.Empty(WingetListScanner.ParseIds("nothing useful here"));
        Assert.Empty(WingetListScanner.ParseIds(""));
    }
}
