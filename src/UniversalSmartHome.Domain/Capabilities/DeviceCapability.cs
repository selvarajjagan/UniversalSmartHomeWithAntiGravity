using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Represents a capability supported by a smart device.
/// </summary>
public class DeviceCapability
{
    public string Id { get; init; }
    public string Name { get; init; }
    public CapabilityType CapabilityType { get; init; }
    public IReadOnlyList<string> SupportedCommands { get; init; }
    public IReadOnlyList<string> SupportedProperties { get; init; }

    public DeviceCapability(
        string id,
        string name,
        CapabilityType capabilityType,
        IEnumerable<string>? supportedCommands = null,
        IEnumerable<string>? supportedProperties = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
        CapabilityType = capabilityType;
        SupportedCommands = (supportedCommands ?? []).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        SupportedProperties = (supportedProperties ?? []).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>
    /// Checks whether a specific command is supported by this capability.
    /// </summary>
    public bool SupportsCommand(string commandName) =>
        SupportedCommands.Contains(commandName, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a specific property is supported by this capability.
    /// </summary>
    public bool SupportsProperty(string propertyName) =>
        SupportedProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
}
