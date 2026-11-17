using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Events;

/// <summary>
/// Raised when a device capability state changes.
/// </summary>
public sealed record DeviceStateChangedEvent(
    DeviceId DeviceId,
    DeviceState NewState,
    DateTimeOffset OccurredAt) : IDomainEvent
{
    public DeviceStateChangedEvent(DeviceId deviceId, DeviceState newState)
        : this(deviceId, newState, DateTimeOffset.UtcNow)
    {
    }
}
