using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for devices that can be turned on, turned off, or toggled.
/// </summary>
public sealed class PowerCapability : DeviceCapability
{
    public const string CapabilityId = "power";
    public const string CommandTurnOn = "TurnOn";
    public const string CommandTurnOff = "TurnOff";
    public const string CommandToggle = "Toggle";
    public const string PropertyIsOn = "IsOn";

    public bool SupportsToggle { get; }

    public PowerCapability(bool supportsToggle = true)
        : base(
            CapabilityId,
            "Power Control",
            CapabilityType.Power,
            supportsToggle
                ? [CommandTurnOn, CommandTurnOff, CommandToggle]
                : [CommandTurnOn, CommandTurnOff],
            [PropertyIsOn])
    {
        SupportsToggle = supportsToggle;
    }
}
