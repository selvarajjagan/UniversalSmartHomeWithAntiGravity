using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Commands;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.States;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Connectors.Abstractions.Models;

/// <summary>
/// Device information emitted during connector discovery before reconciliation into a SmartDevice.
/// </summary>
public sealed record DiscoveredDevice(
    string ExternalDeviceId,
    string Name,
    string Manufacturer,
    string Model,
    DeviceType SuggestedType,
    IReadOnlyList<DeviceCapability> Capabilities,
    IReadOnlyList<DeviceState> InitialStates,
    IReadOnlyDictionary<string, string>? Metadata = null,
    string? ExternalEntityType = null,
    string? AreaOrRoomName = null);

/// <summary>
/// Event arguments delivered when a device state change is observed by a connector.
/// </summary>
public sealed class DeviceStateChangedEventArgs : EventArgs
{
    public ConnectorId ConnectorId { get; }
    public string ExternalDeviceId { get; }
    public DeviceState State { get; }
    public DateTimeOffset Timestamp { get; }

    public DeviceStateChangedEventArgs(ConnectorId connectorId, string externalDeviceId, DeviceState state, DateTimeOffset? timestamp = null)
    {
        ConnectorId = connectorId;
        ExternalDeviceId = externalDeviceId;
        State = state;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }
}

/// <summary>
/// Status change event arguments for connector health monitoring.
/// </summary>
public sealed class ConnectorStatusChangedEventArgs : EventArgs
{
    public ConnectorId ConnectorId { get; }
    public bool IsConnected { get; }
    public string? Reason { get; }
    public Exception? Exception { get; }
    public DateTimeOffset Timestamp { get; }

    public ConnectorStatusChangedEventArgs(
        ConnectorId connectorId,
        bool isConnected,
        string? reason = null,
        Exception? exception = null)
    {
        ConnectorId = connectorId;
        IsConnected = isConnected;
        Reason = reason;
        Exception = exception;
        Timestamp = DateTimeOffset.UtcNow;
    }
}

/// <summary>
/// Result of establishing a connector connection.
/// </summary>
public sealed record ConnectorConnectionResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Exception? Exception = null)
{
    public static ConnectorConnectionResult Success() => new(true);
    public static ConnectorConnectionResult Failure(string error, Exception? ex = null) => new(false, error, ex);
}

/// <summary>
/// Result of executing a command on an external device through a connector.
/// </summary>
public sealed record CommandExecutionResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    IReadOnlyDictionary<string, object?>? ResultData = null)
{
    public static CommandExecutionResult Success(IReadOnlyDictionary<string, object?>? data = null) => new(true, null, data);
    public static CommandExecutionResult Failure(string errorMessage) => new(false, errorMessage);
}
