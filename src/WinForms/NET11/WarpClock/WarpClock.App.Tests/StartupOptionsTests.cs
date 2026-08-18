namespace WarpClock.App.Tests;

public sealed class StartupOptionsTests
{
    [Fact]
    public void Parse_SupportsInlineValuesAndImplicitBooleans()
    {
        StartupOptions options = StartupOptions.Parse(
        [
            "--StartTheme=Railway Classic",
            "/StartKioskMode:true",
            "--AlwaysOn",
            "--RecordFramerate",
            "--DebugRun",
            "5",
            "-DontPersist:false",
        ]);

        Assert.Equal("Railway Classic", options.StartTheme);
        Assert.True(options.StartKioskMode);
        Assert.True(options.AlwaysOn);
        Assert.True(options.RecordFramerate);
        Assert.Equal(5, options.DebugRunSeconds);
        Assert.False(options.DontPersist);
    }

    [Fact]
    public void Parse_SupportsExplicitFalseBooleanValues()
    {
        StartupOptions options = StartupOptions.Parse(
        [
            "--StartKioskMode=false",
            "--AlwaysOn", "false",
            "--RecordFramerate=false",
            "/DontPersist:no",
        ]);

        Assert.False(options.StartKioskMode);
        Assert.False(options.AlwaysOn);
        Assert.False(options.RecordFramerate);
        Assert.False(options.DontPersist);
    }

    [Fact]
    public void Parse_LeavesOptionalBooleansUnsetWhenNotProvided()
    {
        StartupOptions options = StartupOptions.Parse([]);

        Assert.Null(options.StartKioskMode);
        Assert.Null(options.AlwaysOn);
        Assert.Null(options.RecordFramerate);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("16")]
    [InlineData("abc")]
    public void Parse_RejectsInvalidDebugRunValues(string value)
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => StartupOptions.Parse(["--DebugRun", value]));

        Assert.Contains("DebugRun", ex.Message);
    }
}
