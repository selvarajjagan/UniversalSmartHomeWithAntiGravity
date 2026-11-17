using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Connectors.Abstractions;

/// <summary>
/// Orchestrates the lifecycle, discovery, execution, and health of multiple device connectors.
/// </summary>
public interface IConnectorManager : IAsyncDisposable
{
    /// <summary>
    /// Gets all currently registered connectors.
    /// </summary>
    IReadOnlyCollection<IDeviceConnector> GetAllConnectors();

    /// <summary>
    /// Gets a connector by its identifier.
    /// </summary>
    IDeviceConnector? GetConnector(ConnectorId connectorId);

    /// <summary>
    /// Registers a new connector instance.
    /// </summary>
    Task RegisterConnectorAsync(IDeviceConnector connector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters and disconnects a connector instance.
    /// </summary>
    Task UnregisterConnectorAsync(ConnectorId connectorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects a specific connector.
    /// </summary>
    Task<ConnectorConnectionResult> ConnectAsync(ConnectorId connectorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects a specific connector.
    /// </summary>
    Task DisconnectAsync(ConnectorId connectorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers devices across all connected connectors.
    /// </summary>
    Task<IReadOnlyDictionary<ConnectorId, IReadOnlyList<DiscoveredDevice>>> DiscoverAllDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers devices from a specific connector.
    /// </summary>
    Task<IReadOnlyList<DiscoveredDevice>> DiscoverDevicesAsync(ConnectorId connectorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Routes and executes a validated domain command to the appropriate connector.
    /// </summary>
    Task<CommandExecutionResult> ExecuteCommandAsync(DeviceCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised whenever any connector observes a device state change.
    /// </summary>
    event Func<DeviceStateChangedEventArgs, Task>? StateChanged;

    /// <summary>
    /// Raised whenever any connector changes connection status.
    /// </summary>
    event EventHandler<ConnectorStatusChangedEventArgs>? ConnectorStatusChanged;
}

/// <summary>
/// Reconciles discovered devices from external connectors into internal SmartDevices.
/// Prevents duplicates, preserves external entity mappings, and avoids improper automatic merges.
/// </summary>
public interface IDeviceReconciliationService
{
    /// <summary>
    /// Reconciles a discovered device into a domain SmartDevice and corresponding DeviceMapping.
    /// </summary>
    SmartDevice Reconcile(
        DiscoveredDevice discoveredDevice,
        ConnectorId connectorId,
        HomeId homeId,
        IEnumerable<SmartDevice> existingDevices,
        IEnumerable<DeviceMapping> existingMappings);
}
