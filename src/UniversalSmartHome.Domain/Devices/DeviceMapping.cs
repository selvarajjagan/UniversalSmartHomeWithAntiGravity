using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Devices;

/// <summary>
/// Maintains the relationship between an internal SmartDevice and an external integration entity.
/// Supports multi-protocol reconciliation where one physical device is discovered via multiple providers.
/// </summary>
public sealed class DeviceMapping
{
    public Guid Id { get; init; }
    public DeviceId DeviceId { get; init; }
    public ConnectorId ConnectorId { get; init; }
    public string ExternalDeviceId { get; init; }
    public string? ExternalEntityType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyDictionary<string, string> Metadata { get; init; }

    public DeviceMapping(
        Guid id,
        DeviceId deviceId,
        ConnectorId connectorId,
        string externalDeviceId,
        string? externalEntityType = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        DateTimeOffset? createdAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalDeviceId);

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        DeviceId = deviceId;
        ConnectorId = connectorId;
        ExternalDeviceId = externalDeviceId;
        ExternalEntityType = externalEntityType;
        Metadata = metadata != null
            ? new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }
}
