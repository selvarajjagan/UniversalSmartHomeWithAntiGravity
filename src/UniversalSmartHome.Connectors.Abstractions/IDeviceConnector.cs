using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Connectors.Abstractions;

/// <summary>
/// Core contract implemented by all device connectors (Home Assistant, MCP, Matter, etc.).
/// </summary>
public interface IDeviceConnector : IAsyncDisposable
{
    /// <summary>
    /// Unique identifier for this configured connector instance.
    /// </summary>
    ConnectorId ConnectorId { get; }

    /// <summary>
    /// Friendly display name of this connector.
    /// </summary>
    string ConnectorName { get; }

    /// <summary>
    /// Protocol or platform integration type.
    /// </summary>
    ConnectorType ConnectorType { get; }

    /// <summary>
    /// Current connection status.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Raised whenever the connector's connection status changes.
    /// </summary>
    event EventHandler<ConnectorStatusChangedEventArgs>? StatusChanged;

    /// <summary>
    /// Connects to the external system/device.
    /// </summary>
    Task<ConnectorConnectionResult> ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gracefully disconnects from the external system.
    /// </summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers devices exposed by the external system.
    /// </summary>
    Task<IReadOnlyList<DiscoveredDevice>> DiscoverDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves current state for a specific capability of a device.
    /// </summary>
    Task<DeviceState?> GetStateAsync(string externalDeviceId, string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a command on the target physical device.
    /// </summary>
    Task<CommandExecutionResult> ExecuteCommandAsync(DeviceCommand command, string externalDeviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to real-time state changes pushed from the external system.
    /// </summary>
    Task<IAsyncDisposable> SubscribeToStateChangesAsync(Func<DeviceStateChangedEventArgs, Task> stateChangeHandler, CancellationToken cancellationToken = default);
}
