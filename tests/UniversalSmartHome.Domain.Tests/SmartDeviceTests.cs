using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;
using Xunit;

namespace UniversalSmartHome.Domain.Tests;

public class SmartDeviceTests
{
    [Fact]
    public void IsControllable_WhenDeviceIsOnlineAndHasCommands_ReturnsTrue()
    {
        // Arrange
        var device = new SmartDevice(
            id: DeviceId.New(),
            name: "Living Room Lamp",
            manufacturer: "Philips",
            model: "Hue White",
            deviceType: DeviceType.Light,
            homeId: HomeId.New(),
            connectorId: ConnectorId.New(),
            externalDeviceId: "light.living_room",
            connectionStatus: DeviceConnectionStatus.Online,
            capabilities: [new PowerCapability()]);

        // Act & Assert
        Assert.True(device.IsControllable);
    }

    [Fact]
    public void IsControllable_WhenDeviceIsOffline_ReturnsFalse()
    {
        // Arrange
        var device = new SmartDevice(
            id: DeviceId.New(),
            name: "Living Room Lamp",
            manufacturer: "Philips",
            model: "Hue White",
            deviceType: DeviceType.Light,
            homeId: HomeId.New(),
            connectorId: ConnectorId.New(),
            externalDeviceId: "light.living_room",
            connectionStatus: DeviceConnectionStatus.Offline,
            capabilities: [new PowerCapability()]);

        // Act & Assert
        Assert.False(device.IsControllable);
    }

    [Fact]
    public void IsControllable_WhenDeviceHasOnlyReadOnlySensors_ReturnsFalse()
    {
        // Arrange
        var device = new SmartDevice(
            id: DeviceId.New(),
            name: "Living Room Motion",
            manufacturer: "Aqara",
            model: "RTCGQ11LM",
            deviceType: DeviceType.Sensor,
            homeId: HomeId.New(),
            connectorId: ConnectorId.New(),
            externalDeviceId: "binary_sensor.motion",
            connectionStatus: DeviceConnectionStatus.Online,
            capabilities: [new SensorCapability("motion", "Motion Sensor", SensorType.Motion, isBinary: true)]);

        // Act & Assert
        Assert.False(device.IsControllable);
    }

    [Fact]
    public void UpdateState_ValidCapability_UpdatesLatestStateAndLastSeen()
    {
        // Arrange
        var deviceId = DeviceId.New();
        var device = new SmartDevice(
            id: deviceId,
            name: "Smart Bulb",
            manufacturer: "Yeelight",
            model: "Color Bulb",
            deviceType: DeviceType.Light,
            homeId: HomeId.New(),
            connectorId: ConnectorId.New(),
            externalDeviceId: "light.bulb",
            connectionStatus: DeviceConnectionStatus.Online,
            capabilities: [new PowerCapability(), new LightingCapability()]);

        var powerState = new PowerStateValue(true).ToDeviceState(deviceId);

        // Act
        device.UpdateState(powerState);

        // Assert
        var retrieved = device.GetState(PowerCapability.CapabilityId);
        Assert.NotNull(retrieved);
        var typedState = PowerStateValue.FromDeviceState(retrieved);
        Assert.True(typedState.IsOn);
    }

    [Fact]
    public void ValidateCommand_WhenCommandMatchesCapability_ReturnsSuccess()
    {
        // Arrange
        var deviceId = DeviceId.New();
        var device = new SmartDevice(
            id: deviceId,
            name: "AC",
            manufacturer: "Daikin",
            model: "Emura",
            deviceType: DeviceType.AirConditioner,
            homeId: HomeId.New(),
            connectorId: ConnectorId.New(),
            externalDeviceId: "climate.daikin",
            connectionStatus: DeviceConnectionStatus.Online,
            capabilities: [new PowerCapability(), new ThermostatCapability()]);

        var cmd = CommonDeviceCommands.SetTargetTemperature(deviceId, 23.5);

        // Act
        var result = device.ValidateCommand(cmd);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateCommand_WhenCommandUnsupported_ReturnsFailure()
    {
        // Arrange
        var deviceId = DeviceId.New();
        var device = new SmartDevice(
            id: deviceId,
            name: "Simple Switch",
            manufacturer: "Sonoff",
            model: "Basic",
            deviceType: DeviceType.Switch,
            homeId: HomeId.New(),
            connectorId: ConnectorId.New(),
            externalDeviceId: "switch.sonoff",
            connectionStatus: DeviceConnectionStatus.Online,
            capabilities: [new PowerCapability(supportsToggle: false)]);

        // Toggle is disabled for this power capability
        var toggleCmd = CommonDeviceCommands.Toggle(deviceId);

        // Act
        var result = device.ValidateCommand(toggleCmd);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("does not support command", result.Errors[0]);
    }
}
