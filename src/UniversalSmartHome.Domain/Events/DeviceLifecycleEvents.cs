using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Events;

/// <summary>
/// Raised when a device's connection status changes (e.g. Online, Offline, Unavailable).
/// </summary>
public sealed record DeviceConnectionStatusChangedEvent(
    DeviceId DeviceId,
    DeviceConnectionStatus OldStatus,
    DeviceConnectionStatus NewStatus,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public DeviceConnectionStatusChangedEvent(DeviceId deviceId, DeviceConnectionStatus oldStatus, DeviceConnectionStatus newStatus)
        : this(deviceId, oldStatus, newStatus, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Raised when a new smart device is discovered by an integration connector.
/// </summary>
public sealed record DeviceDiscoveredEvent(
    SmartDevice Device,
    ConnectorId ConnectorId,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public DeviceDiscoveredEvent(SmartDevice device, ConnectorId connectorId)
        : this(device, connectorId, DateTimeOffset.UtcNow)
    {
    }
}
