namespace UniversalSmartHome.Domain.Enums;

/// <summary>
/// Represents the connectivity status of a smart device.
/// </summary>
public enum DeviceConnectionStatus
{
    Unknown = 0,
    Online,
    Offline,
    Unavailable,
    Connecting,
    Error
}
