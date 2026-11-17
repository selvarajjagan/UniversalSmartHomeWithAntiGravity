using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Commands;

/// <summary>
/// Represents a normalized command issued to a smart device.
/// </summary>
public class DeviceCommand
{
    public DeviceId DeviceId { get; init; }
    public string Name { get; init; }
    public string CapabilityId { get; init; }
    public CapabilityType CapabilityType { get; init; }
    public IReadOnlyDictionary<string, object?> Parameters { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public DeviceCommand(
        DeviceId deviceId,
        string name,
        string capabilityId,
        CapabilityType capabilityType,
        IReadOnlyDictionary<string, object?>? parameters = null,
        DateTimeOffset? createdAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(capabilityId);

        DeviceId = deviceId;
        Name = name;
        CapabilityId = capabilityId;
        CapabilityType = capabilityType;
        Parameters = parameters != null
            ? new Dictionary<string, object?>(parameters, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets a parameter value cast to the requested type.
    /// </summary>
    public T? GetParameter<T>(string parameterName)
    {
        if (!Parameters.TryGetValue(parameterName, out var value) || value == null)
            return default;

        if (value is T typed)
            return typed;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Validates this command against the target device capability definition.
    /// </summary>
    public virtual CommandValidationResult Validate(DeviceCapability capability)
    {
        if (!string.Equals(capability.Id, CapabilityId, StringComparison.OrdinalIgnoreCase))
        {
            return CommandValidationResult.Failure(
                $"Command capability '{CapabilityId}' does not match target capability '{capability.Id}'.");
        }

        if (!capability.SupportsCommand(Name))
        {
            return CommandValidationResult.Failure(
                $"Capability '{capability.Name}' does not support command '{Name}'. Supported commands: {string.Join(", ", capability.SupportedCommands)}");
        }

        return CommandValidationResult.Success();
    }
}
