using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Enums;
using Xunit;

namespace UniversalSmartHome.Domain.Tests;

public class DeviceCapabilityTests
{
    [Fact]
    public void PowerCapability_Defaults_SupportTurnOnTurnOffAndToggle()
    {
        var power = new PowerCapability(supportsToggle: true);

        Assert.Equal("power", power.Id);
        Assert.Equal(CapabilityType.Power, power.CapabilityType);
        Assert.True(power.SupportsCommand("TurnOn"));
        Assert.True(power.SupportsCommand("TurnOff"));
        Assert.True(power.SupportsCommand("Toggle"));
        Assert.True(power.SupportsProperty("IsOn"));
    }

    [Fact]
    public void LightingCapability_ConfiguredOptions_ReflectInSupportedLists()
    {
        var light = new LightingCapability(
            supportsBrightness: true,
            supportsColor: true,
            supportsColorTemperature: false);

        Assert.True(light.SupportsCommand("SetBrightness"));
        Assert.True(light.SupportsCommand("SetColor"));
        Assert.False(light.SupportsCommand("SetColorTemperature"));
        Assert.True(light.SupportsProperty("Brightness"));
        Assert.True(light.SupportsProperty("Color"));
        Assert.False(light.SupportsProperty("ColorTemperature"));
    }

    [Fact]
    public void ThermostatCapability_ExposesConfiguredModesAndLimits()
    {
        var thermostat = new ThermostatCapability(
            supportedModes: [HvacMode.Off, HvacMode.Cool, HvacMode.Heat],
            minTemperature: 16.0,
            maxTemperature: 30.0);

        Assert.Contains(HvacMode.Cool, thermostat.SupportedHvacModes);
        Assert.DoesNotContain(HvacMode.Dry, thermostat.SupportedHvacModes);
        Assert.Equal(16.0, thermostat.MinTemperature);
        Assert.Equal(30.0, thermostat.MaxTemperature);
    }

    [Fact]
    public void FanCapability_WithSpeedAndOscillation_SupportsExpectedCommands()
    {
        var fan = new FanCapability(supportsSpeed: true, supportsOscillation: true, supportsDirection: false);

        Assert.True(fan.SupportsCommand("SetSpeed"));
        Assert.True(fan.SupportsCommand("SetOscillation"));
        Assert.False(fan.SupportsCommand("SetDirection"));
        Assert.True(fan.SupportsProperty("SpeedPercentage"));
        Assert.True(fan.SupportsProperty("IsOscillating"));
    }
}
