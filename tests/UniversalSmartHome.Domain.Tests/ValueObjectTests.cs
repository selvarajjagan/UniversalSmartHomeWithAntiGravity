using UniversalSmartHome.Domain.ValueObjects;
using Xunit;

namespace UniversalSmartHome.Domain.Tests;

public class ValueObjectTests
{
    [Fact]
    public void DeviceId_ValidGuid_CreatesSuccessfully()
    {
        var guid = Guid.NewGuid();
        var id = new DeviceId(guid);

        Assert.Equal(guid, id.Value);
        Assert.Equal(guid.ToString(), id.ToString());
    }

    [Fact]
    public void DeviceId_EmptyGuid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DeviceId(Guid.Empty));
    }

    [Fact]
    public void RgbColor_HexConversions_WorkBidirectionally()
    {
        var color = new RgbColor(255, 128, 64);
        var hex = color.ToHex();

        Assert.Equal("#FF8040", hex);

        var parsed = RgbColor.FromHex(hex);
        Assert.Equal(color.Red, parsed.Red);
        Assert.Equal(color.Green, parsed.Green);
        Assert.Equal(color.Blue, parsed.Blue);
    }

    [Fact]
    public void TemperatureValue_CelsiusToFahrenheitConversion_CalculatesAccurately()
    {
        var celsius = TemperatureValue.FromCelsius(20.0);
        var fahrenheit = celsius.ToFahrenheit();

        Assert.Equal(68.0, fahrenheit.Value);
        Assert.Equal(TemperatureUnit.Fahrenheit, fahrenheit.Unit);

        var backToC = fahrenheit.ToCelsius();
        Assert.Equal(20.0, backToC.Value);
    }
}
