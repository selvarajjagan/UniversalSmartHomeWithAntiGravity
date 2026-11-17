using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.Exceptions;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Devices;

/// <summary>
/// Core domain entity representing a unified smart device across all integration protocols.
/// </summary>
public sealed class SmartDevice
{
    private readonly List<DeviceCapability> _capabilities = [];
    private readonly Dictionary<string, DeviceState> _latestStates = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _metadata = new(StringComparer.OrdinalIgnoreCase);

    public DeviceId Id { get; init; }
    public string Name { get; private set; }
    public string Manufacturer { get; private set; }
    public string Model { get; private set; }
    public DeviceType DeviceType { get; private set; }
    public HomeId HomeId { get; private set; }
    public RoomId? RoomId { get; private set; }
    public ConnectorId ConnectorId { get; private set; }
    public string ExternalDeviceId { get; private set; }
    public DeviceConnectionStatus ConnectionStatus { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }

    public IReadOnlyList<DeviceCapability> Capabilities => _capabilities.AsReadOnly();
    public IReadOnlyDictionary<string, DeviceState> LatestStates => _latestStates.AsReadOnly();
    public IReadOnlyDictionary<string, string> Metadata => _metadata.AsReadOnly();

    /// <summary>
    /// A device is controllable only if it is online and has at least one capability that supports executable commands.
    /// </summary>
    public bool IsControllable =>
        ConnectionStatus == DeviceConnectionStatus.Online &&
        _capabilities.Any(c => c.SupportedCommands.Count > 0);

    public SmartDevice(
        DeviceId id,
        string name,
        string manufacturer,
        string model,
        DeviceType deviceType,
        HomeId homeId,
        ConnectorId connectorId,
        string externalDeviceId,
        DeviceConnectionStatus connectionStatus = DeviceConnectionStatus.Unknown,
        RoomId? roomId = null,
        IEnumerable<DeviceCapability>? capabilities = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        DateTimeOffset? lastSeenAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(externalDeviceId);

        Id = id;
        Name = name;
        Manufacturer = string.IsNullOrWhiteSpace(manufacturer) ? "Unknown" : manufacturer;
        Model = string.IsNullOrWhiteSpace(model) ? "Unknown" : model;
        DeviceType = deviceType;
        HomeId = homeId;
        RoomId = roomId;
        ConnectorId = connectorId;
        ExternalDeviceId = externalDeviceId;
        ConnectionStatus = connectionStatus;
        LastSeenAt = lastSeenAt ?? DateTimeOffset.UtcNow;

        if (capabilities != null)
        {
            foreach (var cap in capabilities)
            {
                AddCapability(cap);
            }
        }

        if (metadata != null)
        {
            foreach (var (k, v) in metadata)
            {
                _metadata[k] = v;
            }
        }
    }

    /// <summary>
    /// Updates the display name of the device.
    /// </summary>
    public void Rename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        Name = newName;
    }

    /// <summary>
    /// Assigns the device to a room/area.
    /// </summary>
    public void AssignToRoom(RoomId? roomId)
    {
        RoomId = roomId;
    }

    /// <summary>
    /// Updates the connection status of the device and marks LastSeenAt.
    /// </summary>
    public void UpdateConnectionStatus(DeviceConnectionStatus status, DateTimeOffset? seenAt = null)
    {
        ConnectionStatus = status;
        LastSeenAt = seenAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Adds or updates a capability supported by this device.
    /// </summary>
    public void AddCapability(DeviceCapability capability)
    {
        ArgumentNullException.ThrowIfNull(capability);

        var existingIdx = _capabilities.FindIndex(c => string.Equals(c.Id, capability.Id, StringComparison.OrdinalIgnoreCase));
        if (existingIdx >= 0)
        {
            _capabilities[existingIdx] = capability;
        }
        else
        {
            _capabilities.Add(capability);
        }
    }

    /// <summary>
    /// Retrieves a capability by ID.
    /// </summary>
    public DeviceCapability? GetCapability(string capabilityId) =>
        _capabilities.FirstOrDefault(c => string.Equals(c.Id, capabilityId, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Retrieves a capability by type.
    /// </summary>
    public T? GetCapability<T>() where T : DeviceCapability =>
        _capabilities.OfType<T>().FirstOrDefault();

    /// <summary>
    /// Checks if the device has a specific capability.
    /// </summary>
    public bool HasCapability(CapabilityType capabilityType) =>
        _capabilities.Any(c => c.CapabilityType == capabilityType);

    /// <summary>
    /// Updates the state of a capability.
    /// </summary>
    public void UpdateState(DeviceState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var capability = GetCapability(state.CapabilityId);
        if (capability == null)
        {
            throw new DomainException($"Cannot update state for unknown capability '{state.CapabilityId}' on device '{Name}'.");
        }

        _latestStates[state.CapabilityId] = state;
        LastSeenAt = state.LastUpdatedAt;
        if (ConnectionStatus != DeviceConnectionStatus.Online && state.IsAvailable)
        {
            ConnectionStatus = DeviceConnectionStatus.Online;
        }
    }

    /// <summary>
    /// Gets the latest known state for a capability.
    /// </summary>
    public DeviceState? GetState(string capabilityId) =>
        _latestStates.TryGetValue(capabilityId, out var state) ? state : null;

    /// <summary>
    /// Validates whether a command can be executed on this device.
    /// </summary>
    public CommandValidationResult ValidateCommand(DeviceCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.DeviceId != Id)
        {
            return CommandValidationResult.Failure($"Command target DeviceId '{command.DeviceId}' does not match device '{Id}'.");
        }

        if (ConnectionStatus != DeviceConnectionStatus.Online)
        {
            return CommandValidationResult.Failure($"Device '{Name}' is currently {ConnectionStatus} and cannot accept commands.");
        }

        var capability = GetCapability(command.CapabilityId);
        if (capability == null)
        {
            return CommandValidationResult.Failure($"Device '{Name}' does not support capability '{command.CapabilityId}'.");
        }

        return command.Validate(capability);
    }

    /// <summary>
    /// Sets or updates metadata associated with this device.
    /// </summary>
    public void SetMetadata(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _metadata[key] = value;
    }
}
