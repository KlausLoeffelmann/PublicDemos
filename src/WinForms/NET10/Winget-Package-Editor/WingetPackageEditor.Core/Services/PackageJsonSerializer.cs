using System.Text.Json;
using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

public static class PackageJsonSerializer
{
    public static JsonSerializerOptions DefaultOptions { get; } = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true
    };

    public static string Serialize(WingetPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);
        return JsonSerializer.Serialize(package, DefaultOptions);
    }

    public static WingetPackage? Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        return JsonSerializer.Deserialize<WingetPackage>(json, DefaultOptions);
    }

    /// <summary>
    ///  Creates a deep copy of <paramref name="package"/> by round-tripping it through JSON.
    ///  Polymorphic app entries are preserved.
    /// </summary>
    public static WingetPackage Clone(WingetPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);
        return Deserialize(Serialize(package))
            ?? throw new InvalidOperationException("Failed to clone package.");
    }
}
