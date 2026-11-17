using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;

namespace UniversalSmartHome.Application.DTOs;

public sealed record DeviceDto(
    Guid Id,
    string Name,
    string Manufacturer,
    string Model,
    string DeviceType,
    Guid HomeId,
    Guid? RoomId,
    Guid ConnectorId,
    string ExternalDeviceId,
    string ConnectionStatus,
    bool IsControllable,
    DateTimeOffset LastSeenAt,
    IReadOnlyList<CapabilityDto> Capabilities,
    IReadOnlyDictionary<string, StateDto> LatestStates,
    IReadOnlyDictionary<string, string> Metadata)
{
    public static DeviceDto FromDomain(SmartDevice device) =>
        new(
            device.Id.Value,
            device.Name,
            device.Manufacturer,
            device.Model,
            device.DeviceType.ToString(),
            device.HomeId.Value,
            device.RoomId?.Value,
            device.ConnectorId.Value,
            device.ExternalDeviceId,
            device.ConnectionStatus.ToString(),
            device.IsControllable,
            device.LastSeenAt,
            device.Capabilities.Select(CapabilityDto.FromDomain).ToList(),
            device.LatestStates.ToDictionary(k => k.Key, v => StateDto.FromDomain(v.Value)),
            device.Metadata);
}

public sealed record CapabilityDto(
    string Id,
    string Name,
    string CapabilityType,
    IReadOnlyList<string> SupportedCommands,
    IReadOnlyList<string> SupportedProperties)
{
    public static CapabilityDto FromDomain(Domain.Capabilities.DeviceCapability cap) =>
        new(cap.Id, cap.Name, cap.CapabilityType.ToString(), cap.SupportedCommands, cap.SupportedProperties);
}

public sealed record StateDto(
    Guid DeviceId,
    string CapabilityId,
    string CapabilityType,
    IReadOnlyDictionary<string, object?> Properties,
    DateTimeOffset LastUpdatedAt,
    bool IsAvailable)
{
    public static StateDto FromDomain(DeviceState state) =>
        new(
            state.DeviceId.Value,
            state.CapabilityId,
            state.CapabilityType.ToString(),
            state.StateProperties,
            state.LastUpdatedAt,
            state.IsAvailable);
}

public sealed record ExecuteCommandDto(
    string CommandName,
    string CapabilityId,
    Dictionary<string, object?>? Parameters = null);

public sealed record ConnectorDto(
    Guid Id,
    string Name,
    string ConnectorType,
    bool IsConnected,
    bool AutoConnect,
    DateTimeOffset CreatedAt);

public sealed record HomeDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    IReadOnlyList<RoomDto> Rooms)
{
    public static HomeDto FromDomain(Home home) =>
        new(
            home.Id.Value,
            home.Name,
            home.Description,
            home.CreatedAt,
            home.Rooms.Select(r => new RoomDto(r.Id.Value, r.Name, r.Icon)).ToList());
}

public sealed record RoomDto(
    Guid Id,
    string Name,
    string? Icon);
