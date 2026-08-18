using System.Reflection;
using WarpClock.Abstractions;

namespace WarpClock.App.Tests;

public sealed class FormMainStockThemesTests
{
    [Fact]
    public void StockThemeFactories_IncludeLogical()
    {
        FieldInfo field = typeof(FormMain).GetField("s_stockThemeFactories", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("Could not locate FormMain stock theme factories.");

        Func<IClockTheme>[] factories = Assert.IsType<Func<IClockTheme>[]>(field.GetValue(null));

        Assert.Contains(factories, factory => string.Equals(factory.Method.Name, "Logical", StringComparison.Ordinal));
    }
}
