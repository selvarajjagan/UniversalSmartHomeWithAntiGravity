namespace UniversalSmartHome.Domain.Exceptions;

/// <summary>
/// Base exception for domain rule violations.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when attempting to execute a command that is not supported by the target device or capability.
/// </summary>
public class UnsupportedCommandException : DomainException
{
    public string CommandName { get; }
    public string CapabilityId { get; }

    public UnsupportedCommandException(string commandName, string capabilityId, string message)
        : base(message)
    {
        CommandName = commandName;
        CapabilityId = capabilityId;
    }
}

/// <summary>
/// Thrown when attempting to control a device that is offline or unavailable.
/// </summary>
public class DeviceNotControllableException : DomainException
{
    public DeviceNotControllableException(string message) : base(message) { }
}
