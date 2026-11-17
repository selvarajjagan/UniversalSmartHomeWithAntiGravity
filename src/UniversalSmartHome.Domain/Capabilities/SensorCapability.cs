using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for sensor devices providing telemetry or binary state.
/// </summary>
public sealed class SensorCapability : DeviceCapability
{
    public const string PropertyValue = "Value";
    public const string PropertyBinaryState = "BinaryState";
    public const string PropertyUnitOfMeasurement = "UnitOfMeasurement";

    public SensorType SensorType { get; }
    public string? UnitOfMeasurement { get; }
    public bool IsBinary { get; }

    public SensorCapability(
        string id,
        string name,
        SensorType sensorType,
        string? unitOfMeasurement = null,
        bool isBinary = false)
        : base(
            id,
            name,
            CapabilityType.Sensor,
            [], // Sensors are read-only
            isBinary
                ? [PropertyBinaryState]
                : [PropertyValue, PropertyUnitOfMeasurement])
    {
        SensorType = sensorType;
        UnitOfMeasurement = unitOfMeasurement;
        IsBinary = isBinary;
    }
}
