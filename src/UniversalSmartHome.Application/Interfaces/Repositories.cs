using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Application.Interfaces;

/// <summary>
/// Data access contract for smart devices and their mappings.
/// </summary>
public interface IDeviceRepository
{
    Task<SmartDevice?> GetByIdAsync(DeviceId id, CancellationToken cancellationToken = default);
    Task<SmartDevice?> GetByExternalIdAsync(ConnectorId connectorId, string externalDeviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SmartDevice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SmartDevice>> GetByHomeIdAsync(HomeId homeId, CancellationToken cancellationToken = default);
    Task SaveAsync(SmartDevice device, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeviceId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeviceMapping>> GetMappingsForDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default);
    Task SaveMappingAsync(DeviceMapping mapping, CancellationToken cancellationToken = default);
}

/// <summary>
/// Data access contract for homes and rooms.
/// </summary>
public interface IHomeRepository
{
    Task<Home?> GetByIdAsync(HomeId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Home>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(Home home, CancellationToken cancellationToken = default);
    Task DeleteAsync(HomeId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Data access contract for stored connector configurations.
/// </summary>
public interface IConnectorRepository
{
    Task<IReadOnlyList<ConnectorEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ConnectorEntity?> GetByIdAsync(ConnectorId id, CancellationToken cancellationToken = default);
    Task SaveAsync(ConnectorEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ConnectorId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Lightweight entity representing stored connector metadata and configuration reference.
/// </summary>
public sealed record ConnectorEntity(
    ConnectorId Id,
    string Name,
    string ConnectorType,
    bool AutoConnect,
    DateTimeOffset CreatedAt,
    string? ConfigurationJson = null);
