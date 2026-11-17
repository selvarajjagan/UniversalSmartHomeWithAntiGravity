using Microsoft.Extensions.Logging;
using UniversalSmartHome.Application.DTOs;
using UniversalSmartHome.Application.Interfaces;
using UniversalSmartHome.Connectors.Abstractions;
using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.Exceptions;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Application.Services;

/// <summary>
/// Application service providing high-level smart device management, discovery orchestration, and command execution.
/// </summary>
public interface IDeviceService
{
    Task<IReadOnlyList<DeviceDto>> GetAllDevicesAsync(CancellationToken cancellationToken = default);
    Task<DeviceDto?> GetDeviceByIdAsync(DeviceId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeviceDto>> GetDevicesByHomeIdAsync(HomeId homeId, CancellationToken cancellationToken = default);
    Task<CommandExecutionResult> ExecuteCommandAsync(DeviceCommand command, CancellationToken cancellationToken = default);
    Task RenameDeviceAsync(DeviceId id, string newName, CancellationToken cancellationToken = default);
    Task AssignRoomAsync(DeviceId id, RoomId? roomId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeviceDto>> SyncDiscoveredDevicesAsync(ConnectorId connectorId, HomeId homeId, CancellationToken cancellationToken = default);
}

public sealed class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IConnectorManager _connectorManager;
    private readonly IDeviceReconciliationService _reconciliationService;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        IDeviceRepository deviceRepository,
        IConnectorManager connectorManager,
        IDeviceReconciliationService reconciliationService,
        ILogger<DeviceService> logger)
    {
        _deviceRepository = deviceRepository;
        _connectorManager = connectorManager;
        _reconciliationService = reconciliationService;
        _logger = logger;

        _connectorManager.StateChanged += OnConnectorStateChangedAsync;
    }

    public async Task<IReadOnlyList<DeviceDto>> GetAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        return devices.Select(DeviceDto.FromDomain).ToList();
    }

    public async Task<DeviceDto?> GetDeviceByIdAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken);
        return device != null ? DeviceDto.FromDomain(device) : null;
    }

    public async Task<IReadOnlyList<DeviceDto>> GetDevicesByHomeIdAsync(HomeId homeId, CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetByHomeIdAsync(homeId, cancellationToken);
        return devices.Select(DeviceDto.FromDomain).ToList();
    }

    public async Task<CommandExecutionResult> ExecuteCommandAsync(DeviceCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var device = await _deviceRepository.GetByIdAsync(command.DeviceId, cancellationToken);
        if (device == null)
        {
            return CommandExecutionResult.Failure($"Device '{command.DeviceId}' not found.");
        }

        // Domain validation: verify device status, capability existence, and command support
        var validation = device.ValidateCommand(command);
        if (!validation.IsValid)
        {
            var errorMsg = string.Join("; ", validation.Errors);
            _logger.LogWarning("Command validation failed for device '{DeviceId}': {Errors}", command.DeviceId, errorMsg);
            return CommandExecutionResult.Failure(errorMsg);
        }

        var connector = _connectorManager.GetConnector(device.ConnectorId);
        if (connector == null || !connector.IsConnected)
        {
            return CommandExecutionResult.Failure($"Connector for device '{device.Name}' is not connected.");
        }

        _logger.LogInformation(
            "Executing command '{Command}' on device '{Device}' via connector '{Connector}'...",
            command.Name, device.Name, connector.ConnectorName);

        var result = await connector.ExecuteCommandAsync(command, device.ExternalDeviceId, cancellationToken);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Command '{Command}' succeeded on device '{Device}'.", command.Name, device.Name);
        }
        else
        {
            _logger.LogWarning("Command '{Command}' failed on device '{Device}': {Error}", command.Name, device.Name, result.ErrorMessage);
        }

        return result;
    }

    public async Task RenameDeviceAsync(DeviceId id, string newName, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException($"Device '{id}' not found.");

        device.Rename(newName);
        await _deviceRepository.SaveAsync(device, cancellationToken);
    }

    public async Task AssignRoomAsync(DeviceId id, RoomId? roomId, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new DomainException($"Device '{id}' not found.");

        device.AssignToRoom(roomId);
        await _deviceRepository.SaveAsync(device, cancellationToken);
    }

    public async Task<IReadOnlyList<DeviceDto>> SyncDiscoveredDevicesAsync(ConnectorId connectorId, HomeId homeId, CancellationToken cancellationToken = default)
    {
        var discovered = await _connectorManager.DiscoverDevicesAsync(connectorId, cancellationToken);
        var existingDevices = await _deviceRepository.GetAllAsync(cancellationToken);
        var existingMappings = new List<DeviceMapping>();
        foreach (var dev in existingDevices)
        {
            var maps = await _deviceRepository.GetMappingsForDeviceAsync(dev.Id, cancellationToken);
            existingMappings.AddRange(maps);
        }

        var reconciledDevices = new List<SmartDevice>();

        foreach (var disc in discovered)
        {
            var device = _reconciliationService.Reconcile(disc, connectorId, homeId, existingDevices, existingMappings);
            await _deviceRepository.SaveAsync(device, cancellationToken);

            var mapping = new DeviceMapping(
                Guid.NewGuid(),
                device.Id,
                connectorId,
                disc.ExternalDeviceId,
                disc.ExternalEntityType,
                disc.Metadata);

            await _deviceRepository.SaveMappingAsync(mapping, cancellationToken);
            reconciledDevices.Add(device);
        }

        return reconciledDevices.Select(DeviceDto.FromDomain).ToList();
    }

    private async Task OnConnectorStateChangedAsync(DeviceStateChangedEventArgs args)
    {
        try
        {
            var device = await _deviceRepository.GetByExternalIdAsync(args.ConnectorId, args.ExternalDeviceId);
            if (device != null)
            {
                device.UpdateState(args.State);
                await _deviceRepository.SaveAsync(device);
                _logger.LogDebug("Updated state for device '{DeviceId}' from connector '{ConnectorId}'.", device.Id, args.ConnectorId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply state update from connector '{ConnectorId}' for device '{ExternalId}'.", args.ConnectorId, args.ExternalDeviceId);
        }
    }
}
