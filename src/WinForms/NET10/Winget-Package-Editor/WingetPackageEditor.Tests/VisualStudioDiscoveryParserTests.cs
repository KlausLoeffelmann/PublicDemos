using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;

namespace WingetPackageEditor.Tests;

public sealed class VisualStudioDiscoveryParserTests
{
    private const string SampleBlock = """
        Visual Studio Locator version 3.1.7+f39851e70f [query version 4.7.7.8087]
        Copyright (C) Microsoft Corporation. All rights reserved.

        instanceId: 480e759d
        installDate: 12/9/2025 10:35:13 PM
        installationName: VisualStudioPreview/18.7.0-insiders+11822.327
        installationPath: C:\Program Files\Microsoft Visual Studio\18\Insiders
        installationVersion: 18.7.11822.327
        productId: Microsoft.VisualStudio.Product.Enterprise
        productPath: C:\Program Files\Microsoft Visual Studio\18\Insiders\Common7\IDE\devenv.exe
        isPrerelease: 1
        displayName: Visual Studio Enterprise 2026
        channelId: VisualStudio.18.Preview
        catalog_productLineVersion: 18
        """;

    [Fact]
    public void ParseBlocks_SkipsBannerLines_AndReturnsOneBlockPerInstance()
    {
        IReadOnlyList<IReadOnlyDictionary<string, string>> blocks = VisualStudioDiscoveryParser.ParseBlocks(SampleBlock);

        Assert.Single(blocks);
        Assert.Equal("480e759d", blocks[0]["instanceId"]);
        // Values containing colons (URLs/paths) must be preserved past the first colon.
        Assert.Equal(@"C:\Program Files\Microsoft Visual Studio\18\Insiders", blocks[0]["installationPath"]);
    }

    [Fact]
    public void MapInstance_MapsEditionChannelYearAndShortVersion()
    {
        IReadOnlyDictionary<string, string> block = VisualStudioDiscoveryParser.ParseBlocks(SampleBlock)[0];

        VisualStudioInstanceInfo? instance = VisualStudioDiscoveryParser.MapInstance(block, []);

        Assert.NotNull(instance);
        Assert.Equal("480e759d", instance!.InstanceId);
        Assert.Equal("Enterprise", instance.Edition);
        Assert.Equal(VisualStudioChannel.Preview, instance.Channel);
        Assert.Equal("2026", instance.Year);
        Assert.Equal("18.0", instance.ShortVersion);
        Assert.True(instance.IsPrerelease);
        Assert.Equal("Preview-Enterprise", instance.SkuComboLabel);
    }

    [Fact]
    public void CorrelateHives_MatchesMainAndExperimentalHives_ForInstance()
    {
        VisualStudioHiveFolder[] folders =
        [
            new("18.0_480e759d", @"C:\hive\18.0_480e759d"),
            new("18.0_480e759dExp", @"C:\hive\18.0_480e759dExp"),
            new("17.0_deadbeef", @"C:\hive\17.0_deadbeef"),
            new("18.0_480e759dRoslynDeployment", @"C:\hive\18.0_480e759dRoslynDeployment")
        ];

        IReadOnlyList<VisualStudioHiveInfo> hives =
            VisualStudioDiscoveryParser.CorrelateHives(folders, "18.0", "480e759d");

        Assert.Equal(2, hives.Count);
        Assert.Contains(hives, hive => !hive.IsExperimental && hive.Name == "18.0_480e759d");
        Assert.Contains(hives, hive => hive.IsExperimental && hive.Name == "18.0_480e759dExp");
    }

    [Theory]
    [InlineData("VisualStudio.18.Release", VisualStudioChannel.Release)]
    [InlineData("VisualStudio.18.Preview", VisualStudioChannel.Preview)]
    [InlineData("VisualStudio.18.IntPreview.Canary", VisualStudioChannel.Canary)]
    [InlineData("VisualStudio.18.Main", VisualStudioChannel.Main)]
    [InlineData("", VisualStudioChannel.Unknown)]
    public void MapChannel_ClassifiesKnownChannels(string channelId, VisualStudioChannel expected)
        => Assert.Equal(expected, VisualStudioDiscoveryParser.MapChannel(channelId));

    [Theory]
    [InlineData("16", "2019")]
    [InlineData("17", "2022")]
    [InlineData("18", "2026")]
    public void MapYear_MapsProductLineVersionToReleaseYear(string lineVersion, string expected)
    {
        Dictionary<string, string> block = new(StringComparer.OrdinalIgnoreCase)
        {
            ["catalog_productLineVersion"] = lineVersion
        };

        Assert.Equal(expected, VisualStudioDiscoveryParser.MapYear(block, shortVersion: string.Empty));
    }
}
