using Microsoft.Extensions.Logging.Abstractions;
using UniversalSmartHome.Application.Services;
using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;
using Xunit;

namespace UniversalSmartHome.Application.Tests;

public class DeviceReconciliationServiceTests
{
    private readonly DeviceReconciliationService _sut = new(NullLogger<DeviceReconciliationService>.Instance);

    [Fact]
    public void Reconcile_NewDevice_CreatesBrandNewSmartDevice()
    {
        // Arrange
        var connectorId = ConnectorId.New();
        var homeId = HomeId.New();
        var discovered = new DiscoveredDevice(
            ExternalDeviceId: "light.living_room_ceiling",
            Name: "Living Room Ceiling Light",
            Manufacturer: "IKEA",
            Model: "Tradfri",
            SuggestedType: DeviceType.Light,
            Capabilities: [new PowerCapability()],
            InitialStates: []);

        // Act
        var result = _sut.Reconcile(discovered, connectorId, homeId, [], []);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Living Room Ceiling Light", result.Name);
        Assert.Equal("light.living_room_ceiling", result.ExternalDeviceId);
        Assert.Equal(connectorId, result.ConnectorId);
        Assert.Equal(homeId, result.HomeId);
        Assert.Equal(DeviceType.Light, result.DeviceType);
        Assert.Single(result.Capabilities);
    }

    [Fact]
    public void Reconcile_DeviceWithExistingMapping_ReconcilesToSameSmartDevice()
    {
        // Arrange
        var existingDeviceId = DeviceId.New();
        var connectorId = ConnectorId.New();
        var homeId = HomeId.New();

        var existingDevice = new SmartDevice(
            id: existingDeviceId,
            name: "Original Name",
            manufacturer: "IKEA",
            model: "Tradfri",
            deviceType: DeviceType.Light,
            homeId: homeId,
            connectorId: connectorId,
            externalDeviceId: "light.living_room_ceiling",
            connectionStatus: DeviceConnectionStatus.Offline,
            capabilities: [new PowerCapability()]);

        var mapping = new DeviceMapping(
            id: Guid.NewGuid(),
            deviceId: existingDeviceId,
            connectorId: connectorId,
            externalDeviceId: "light.living_room_ceiling");

        var discovered = new DiscoveredDevice(
            ExternalDeviceId: "light.living_room_ceiling",
            Name: "Discovered Name",
            Manufacturer: "IKEA",
            Model: "Tradfri",
            SuggestedType: DeviceType.Light,
            Capabilities: [new PowerCapability(), new LightingCapability()],
            InitialStates: []);

        // Act
        var result = _sut.Reconcile(
            discovered,
            connectorId,
            homeId,
            [existingDevice],
            [mapping]);

        // Assert: It should return the exact same device ID
        Assert.Equal(existingDeviceId, result.Id);
        Assert.Equal(DeviceConnectionStatus.Online, result.ConnectionStatus);
        // It should have merged new capabilities
        Assert.Equal(2, result.Capabilities.Count);
    }

    [Fact]
    public void Reconcile_DeviceWithMatchingNameOnly_DoesNotMergeAutomatically()
    {
        // Product Principle: Do not automatically merge devices solely because their names match.
        // Arrange
        var existingDeviceId = DeviceId.New();
        var connector1Id = ConnectorId.New();
        var connector2Id = ConnectorId.New();
        var homeId = HomeId.New();

        var existingDevice = new SmartDevice(
            id: existingDeviceId,
            name: "Living Room TV",
            manufacturer: "LG",
            model: "OLED65",
            deviceType: DeviceType.Television,
            homeId: homeId,
            connectorId: connector1Id,
            externalDeviceId: "media_player.lg_tv",
            capabilities: [new PowerCapability()]);

        var discoveredFromOtherConnector = new DiscoveredDevice(
            ExternalDeviceId: "tv_switch_01",
            Name: "Living Room TV", // Same name, completely different external ID and connector!
            Manufacturer: "Sonoff",
            Model: "Plug",
            SuggestedType: DeviceType.Switch,
            Capabilities: [new PowerCapability()],
            InitialStates: []);

        // Act
        var result = _sut.Reconcile(
            discoveredFromOtherConnector,
            connector2Id,
            homeId,
            [existingDevice],
            []);

        // Assert: A new device must be created, NOT merged with existingDevice!
        Assert.NotEqual(existingDeviceId, result.Id);
        Assert.Equal("tv_switch_01", result.ExternalDeviceId);
        Assert.Equal(connector2Id, result.ConnectorId);
    }
}
