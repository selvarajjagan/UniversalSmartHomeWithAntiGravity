using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using UniversalSmartHome.Connectors.Abstractions;
using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Application.Services;

/// <summary>
/// Thread-safe manager coordinating the lifecycle, discovery, execution, and health of multiple connectors.
/// Ensures complete failure isolation so that issues in one connector do not degrade others.
/// </summary>
public sealed class ConnectorManager : IConnectorManager
{
    private readonly ConcurrentDictionary<ConnectorId, IDeviceConnector> _connectors = new();
    private readonly ConcurrentDictionary<ConnectorId, IAsyncDisposable> _subscriptions = new();
    private readonly ILogger<ConnectorManager> _logger;

    public event Func<DeviceStateChangedEventArgs, Task>? StateChanged;
    public event EventHandler<ConnectorStatusChangedEventArgs>? ConnectorStatusChanged;

    public ConnectorManager(ILogger<ConnectorManager> logger)
    {
        _logger = logger;
    }

    public IReadOnlyCollection<IDeviceConnector> GetAllConnectors() => _connectors.Values.ToList().AsReadOnly();

    public IDeviceConnector? GetConnector(ConnectorId connectorId) =>
        _connectors.TryGetValue(connectorId, out var connector) ? connector : null;

    public async Task RegisterConnectorAsync(IDeviceConnector connector, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connector);

        if (_connectors.TryAdd(connector.ConnectorId, connector))
        {
            _logger.LogInformation(
                "Registered connector '{Name}' [{Type}] ({Id}).",
                connector.ConnectorName, connector.ConnectorType, connector.ConnectorId);

            connector.StatusChanged += OnConnectorStatusChanged;

            // Automatically subscribe to state changes
            try
            {
                var subscription = await connector.SubscribeToStateChangesAsync(OnStateChangedAsync, cancellationToken);
                _subscriptions[connector.ConnectorId] = subscription;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to subscribe to state changes for connector '{Id}'.", connector.ConnectorId);
            }
        }
        else
        {
            _logger.LogWarning("Connector '{Id}' is already registered.", connector.ConnectorId);
        }
    }

    public async Task UnregisterConnectorAsync(ConnectorId connectorId, CancellationToken cancellationToken = default)
    {
        if (_connectors.TryRemove(connectorId, out var connector))
        {
            _logger.LogInformation("Unregistering connector '{Id}'.", connectorId);
            connector.StatusChanged -= OnConnectorStatusChanged;

            if (_subscriptions.TryRemove(connectorId, out var subscription))
            {
                try
                {
                    await subscription.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disposing state subscription for connector '{Id}'.", connectorId);
                }
            }

            try
            {
                await connector.DisconnectAsync(cancellationToken);
                await connector.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disconnecting connector '{Id}'.", connectorId);
            }
        }
    }

    public async Task<ConnectorConnectionResult> ConnectAsync(ConnectorId connectorId, CancellationToken cancellationToken = default)
    {
        var connector = GetConnector(connectorId);
        if (connector == null)
        {
            return ConnectorConnectionResult.Failure($"Connector '{connectorId}' is not registered.");
        }

        try
        {
            _logger.LogInformation("Connecting connector '{Name}' ({Id})...", connector.ConnectorName, connectorId);
            var result = await connector.ConnectAsync(cancellationToken);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully connected connector '{Name}'.", connector.ConnectorName);
            }
            else
            {
                _logger.LogWarning("Failed to connect connector '{Name}': {Error}", connector.ConnectorName, result.ErrorMessage);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception connecting connector '{Id}'.", connectorId);
            return ConnectorConnectionResult.Failure(ex.Message, ex);
        }
    }

    public async Task DisconnectAsync(ConnectorId connectorId, CancellationToken cancellationToken = default)
    {
        var connector = GetConnector(connectorId);
        if (connector != null)
        {
            try
            {
                _logger.LogInformation("Disconnecting connector '{Name}' ({Id})...", connector.ConnectorName, connectorId);
                await connector.DisconnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disconnecting connector '{Id}'.", connectorId);
            }
        }
    }

    public async Task<IReadOnlyDictionary<ConnectorId, IReadOnlyList<DiscoveredDevice>>> DiscoverAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<ConnectorId, IReadOnlyList<DiscoveredDevice>>();

        foreach (var connector in _connectors.Values)
        {
            if (!connector.IsConnected)
            {
                _logger.LogDebug("Skipping discovery for disconnected connector '{Name}'.", connector.ConnectorName);
                results[connector.ConnectorId] = [];
                continue;
            }

            try
            {
                _logger.LogInformation("Initiating device discovery on connector '{Name}'...", connector.ConnectorName);
                var devices = await connector.DiscoverDevicesAsync(cancellationToken);
                results[connector.ConnectorId] = devices;
                _logger.LogInformation("Connector '{Name}' discovered {Count} devices.", connector.ConnectorName, devices.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Discovery failed on connector '{Name}'. Other connectors will continue unaffected.", connector.ConnectorName);
                results[connector.ConnectorId] = [];
            }
        }

        return results;
    }

    public async Task<IReadOnlyList<DiscoveredDevice>> DiscoverDevicesAsync(ConnectorId connectorId, CancellationToken cancellationToken = default)
    {
        var connector = GetConnector(connectorId);
        if (connector == null)
        {
            _logger.LogWarning("Cannot discover devices: connector '{Id}' not found.", connectorId);
            return [];
        }

        if (!connector.IsConnected)
        {
            _logger.LogWarning("Cannot discover devices: connector '{Name}' is not connected.", connector.ConnectorName);
            return [];
        }

        try
        {
            return await connector.DiscoverDevicesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Device discovery failed on connector '{Name}'.", connector.ConnectorName);
            return [];
        }
    }

    public async Task<CommandExecutionResult> ExecuteCommandAsync(DeviceCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Connector routing is handled by the caller or repository mapping
        // In the direct overload, we locate the appropriate connector
        return CommandExecutionResult.Failure("Direct execution requires device routing.");
    }

    private void OnConnectorStatusChanged(object? sender, ConnectorStatusChangedEventArgs e)
    {
        ConnectorStatusChanged?.Invoke(this, e);
    }

    private async Task OnStateChangedAsync(DeviceStateChangedEventArgs args)
    {
        if (StateChanged != null)
        {
            try
            {
                await StateChanged.Invoke(args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling state change notification for external device '{ExternalId}'.", args.ExternalDeviceId);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var subscription in _subscriptions.Values)
        {
            try { await subscription.DisposeAsync(); } catch { }
        }
        _subscriptions.Clear();

        foreach (var connector in _connectors.Values)
        {
            try { await connector.DisposeAsync(); } catch { }
        }
        _connectors.Clear();
    }
}
