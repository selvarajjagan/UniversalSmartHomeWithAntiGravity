using System.Text.Json;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.States;

/// <summary>
/// Encapsulates the runtime state of a device capability.
/// </summary>
public sealed class DeviceState
{
    public DeviceId DeviceId { get; init; }
    public string CapabilityId { get; init; }
    public CapabilityType CapabilityType { get; init; }
    public IReadOnlyDictionary<string, object?> StateProperties { get; init; }
    public DateTimeOffset LastUpdatedAt { get; init; }
    public bool IsAvailable { get; init; }

    public DeviceState(
        DeviceId deviceId,
        string capabilityId,
        CapabilityType capabilityType,
        IReadOnlyDictionary<string, object?>? stateProperties = null,
        DateTimeOffset? lastUpdatedAt = null,
        bool isAvailable = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(capabilityId);

        DeviceId = deviceId;
        CapabilityId = capabilityId;
        CapabilityType = capabilityType;
        StateProperties = stateProperties != null
            ? new Dictionary<string, object?>(stateProperties, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        LastUpdatedAt = lastUpdatedAt ?? DateTimeOffset.UtcNow;
        IsAvailable = isAvailable;
    }

    /// <summary>
    /// Gets a typed property value from the state dictionary.
    /// </summary>
    public T? GetProperty<T>(string propertyName)
    {
        if (!StateProperties.TryGetValue(propertyName, out var raw) || raw == null)
            return default;

        if (raw is T typed)
            return typed;

        try
        {
            if (raw is JsonElement element)
                return JsonSerializer.Deserialize<T>(element.GetRawText());

            return (T)Convert.ChangeType(raw, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Creates a new DeviceState with an updated property value.
    /// </summary>
    public DeviceState WithProperty(string propertyName, object? value)
    {
        var dict = new Dictionary<string, object?>(StateProperties, StringComparer.OrdinalIgnoreCase)
        {
            [propertyName] = value
        };

        return new DeviceState(
            DeviceId,
            CapabilityId,
            CapabilityType,
            dict,
            DateTimeOffset.UtcNow,
            IsAvailable);
    }
}
