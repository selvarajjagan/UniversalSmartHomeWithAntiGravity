using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;
using Xunit;

namespace UniversalSmartHome.Connectors.Tests;

public class ConnectorAbstractionsTests
{
    [Fact]
    public void DiscoveredDevice_Record_InitializesCorrectly()
    {
        var discovered = new DiscoveredDevice(
            ExternalDeviceId: "media_player.sony_tv",
            Name: "Sony Bravia 4K",
            Manufacturer: "Sony",
            Model: "XR-65A80J",
            SuggestedType: DeviceType.Television,
            Capabilities: [new PowerCapability(), new VolumeCapability(), new MediaPlaybackCapability()],
            InitialStates: [],
            Metadata: new Dictionary<string, string> { ["IP"] = "192.168.1.150" },
            ExternalEntityType: "media_player",
            AreaOrRoomName: "Living Room");

        Assert.Equal("media_player.sony_tv", discovered.ExternalDeviceId);
        Assert.Equal("Sony Bravia 4K", discovered.Name);
        Assert.Equal("Sony", discovered.Manufacturer);
        Assert.Equal(DeviceType.Television, discovered.SuggestedType);
        Assert.Equal(3, discovered.Capabilities.Count);
        Assert.Equal("192.168.1.150", discovered.Metadata!["IP"]);
        Assert.Equal("Living Room", discovered.AreaOrRoomName);
    }

    [Fact]
    public void CommandExecutionResult_SuccessAndFailure_FactoryBehaviors()
    {
        var success = CommandExecutionResult.Success();
        Assert.True(success.IsSuccess);
        Assert.Null(success.ErrorMessage);

        var failure = CommandExecutionResult.Failure("Network timeout after 5000ms");
        Assert.False(failure.IsSuccess);
        Assert.Equal("Network timeout after 5000ms", failure.ErrorMessage);
    }
}
