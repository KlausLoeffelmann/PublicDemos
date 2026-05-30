namespace WinBaas.Models;

/// <summary>
///  One Visual Studio hive folder belonging to a SKU.
/// </summary>
public sealed class VsHive
{
    /// <summary>The hive folder name, e.g. <c>18.0_b83bbaee</c>.</summary>
    public required string Name { get; init; }

    /// <summary>The absolute hive folder path.</summary>
    public required string FullPath { get; init; }
}
