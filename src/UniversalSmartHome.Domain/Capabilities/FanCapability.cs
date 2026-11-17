using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for fans and ventilation devices.
/// </summary>
public sealed class FanCapability : DeviceCapability
{
    public const string CapabilityId = "fan";
    public const string CommandSetSpeed = "SetSpeed";
    public const string CommandSetOscillation = "SetOscillation";
    public const string CommandSetDirection = "SetDirection";

    public const string PropertySpeedPercentage = "SpeedPercentage";
    public const string PropertyIsOscillating = "IsOscillating";
    public const string PropertyDirection = "Direction";

    public bool SupportsSpeed { get; }
    public bool SupportsOscillation { get; }
    public bool SupportsDirection { get; }
    public IReadOnlyList<string> PresetModes { get; }

    public FanCapability(
        bool supportsSpeed = true,
        bool supportsOscillation = false,
        bool supportsDirection = false,
        IEnumerable<string>? presetModes = null)
        : base(
            CapabilityId,
            "Fan Control",
            CapabilityType.Fan,
            GetCommands(supportsSpeed, supportsOscillation, supportsDirection),
            GetProperties(supportsSpeed, supportsOscillation, supportsDirection))
    {
        SupportsSpeed = supportsSpeed;
        SupportsOscillation = supportsOscillation;
        SupportsDirection = supportsDirection;
        PresetModes = (presetModes ?? []).ToList();
    }

    private static List<string> GetCommands(bool speed, bool oscillation, bool direction)
    {
        var list = new List<string>();
        if (speed) list.Add(CommandSetSpeed);
        if (oscillation) list.Add(CommandSetOscillation);
        if (direction) list.Add(CommandSetDirection);
        return list;
    }

    private static List<string> GetProperties(bool speed, bool oscillation, bool direction)
    {
        var list = new List<string>();
        if (speed) list.Add(PropertySpeedPercentage);
        if (oscillation) list.Add(PropertyIsOscillating);
        if (direction) list.Add(PropertyDirection);
        return list;
    }
}
