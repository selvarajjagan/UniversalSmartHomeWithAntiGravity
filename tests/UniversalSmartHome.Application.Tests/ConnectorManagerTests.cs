using Microsoft.Extensions.Logging.Abstractions;
using UniversalSmartHome.Application.Services;
using UniversalSmartHome.Connectors.Abstractions;
using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;
using Xunit;

namespace UniversalSmartHome.Application.Tests;

public class ConnectorManagerTests
{
    private class FakeConnector : IDeviceConnector
    {
        public ConnectorId ConnectorId { get; }
        public string ConnectorName { get; }
        public ConnectorType ConnectorType { get; }
        public bool IsConnected { get; private set; }

        public event EventHandler<ConnectorStatusChangedEventArgs>? StatusChanged;

        private readonly List<DiscoveredDevice> _devices;
        private readonly bool _shouldFailDiscovery;

        public FakeConnector(
            ConnectorId id,
            string name,
            ConnectorType type,
            IEnumerable<DiscoveredDevice>? devices = null,
            bool shouldFailDiscovery = false)
        {
            ConnectorId = id;
            ConnectorName = name;
            ConnectorType = type;
            _devices = (devices ?? []).ToList();
            _shouldFailDiscovery = shouldFailDiscovery;
        }

        public Task<ConnectorConnectionResult> ConnectAsync(CancellationToken cancellationToken = default)
        {
            IsConnected = true;
            StatusChanged?.Invoke(this, new ConnectorStatusChangedEventArgs(ConnectorId, true));
            return Task.FromResult(ConnectorConnectionResult.Success());
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            IsConnected = false;
            StatusChanged?.Invoke(this, new ConnectorStatusChangedEventArgs(ConnectorId, false));
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<DiscoveredDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken = default)
        {
            if (_shouldFailDiscovery)
                throw new InvalidOperationException("Simulated network failure on discovery.");

            return Task.FromResult<IReadOnlyList<DiscoveredDevice>>(_devices.AsReadOnly());
        }

        public Task<DeviceState?> GetStateAsync(string externalDeviceId, string capabilityId, CancellationToken cancellationToken = default) =>
            Task.FromResult<DeviceState?>(null);

        public Task<CommandExecutionResult> ExecuteCommandAsync(DeviceCommand command, string externalDeviceId, CancellationToken cancellationToken = default) =>
            Task.FromResult(CommandExecutionResult.Success());

        public Task<IAsyncDisposable> SubscribeToStateChangesAsync(Func<DeviceStateChangedEventArgs, Task> stateChangeHandler, CancellationToken cancellationToken = default) =>
            Task.FromResult<IAsyncDisposable>(new DummyDisposable());

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        private class DummyDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }

    [Fact]
    public async Task RegisterAndConnect_RegistersConnectorSuccessfully()
    {
        var manager = new ConnectorManager(NullLogger<ConnectorManager>.Instance);
        var connectorId = ConnectorId.New();
        var connector = new FakeConnector(connectorId, "Home Assistant Local", ConnectorType.HomeAssistant);

        await manager.RegisterConnectorAsync(connector);
        var retrieved = manager.GetConnector(connectorId);

        Assert.NotNull(retrieved);
        Assert.Equal("Home Assistant Local", retrieved.ConnectorName);

        var connectResult = await manager.ConnectAsync(connectorId);
        Assert.True(connectResult.IsSuccess);
        Assert.True(retrieved.IsConnected);
    }

    [Fact]
    public async Task DiscoverAllDevicesAsync_WhenOneConnectorFails_OtherConnectorSucceedsIndependently()
    {
        // Product Requirement: One connector failure must not crash the entire application or stop other connectors.
        var manager = new ConnectorManager(NullLogger<ConnectorManager>.Instance);

        var c1Id = ConnectorId.New();
        var c2Id = ConnectorId.New();

        var device = new DiscoveredDevice(
            "light.lamp",
            "Lamp",
            "Philips",
            "Hue",
            DeviceType.Light,
            [new PowerCapability()],
            []);

        var failingConnector = new FakeConnector(c1Id, "Failing Connector", ConnectorType.MCP, shouldFailDiscovery: true);
        var workingConnector = new FakeConnector(c2Id, "Working Connector", ConnectorType.HomeAssistant, devices: [device]);

        await manager.RegisterConnectorAsync(failingConnector);
        await manager.RegisterConnectorAsync(workingConnector);

        await manager.ConnectAsync(c1Id);
        await manager.ConnectAsync(c2Id);

        // Act
        var results = await manager.DiscoverAllDevicesAsync();

        // Assert: Failing connector has empty list, working connector discovered 1 device
        Assert.Empty(results[c1Id]);
        Assert.Single(results[c2Id]);
        Assert.Equal("light.lamp", results[c2Id][0].ExternalDeviceId);
    }
}
