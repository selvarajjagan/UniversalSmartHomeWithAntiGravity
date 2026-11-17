using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for volume and mute control on media devices.
/// </summary>
public sealed class VolumeCapability : DeviceCapability
{
    public const string CapabilityId = "volume";
    public const string CommandSetVolume = "SetVolume";
    public const string CommandSetMute = "SetMute";
    public const string CommandVolumeUp = "VolumeUp";
    public const string CommandVolumeDown = "VolumeDown";

    public const string PropertyVolumeLevel = "VolumeLevel";
    public const string PropertyIsMuted = "IsMuted";

    public bool SupportsMute { get; }
    public int MinVolume { get; }
    public int MaxVolume { get; }
    public int Step { get; }

    public VolumeCapability(
        bool supportsMute = true,
        int minVolume = 0,
        int maxVolume = 100,
        int step = 1)
        : base(
            CapabilityId,
            "Volume Control",
            CapabilityType.Volume,
            supportsMute
                ? [CommandSetVolume, CommandSetMute, CommandVolumeUp, CommandVolumeDown]
                : [CommandSetVolume, CommandVolumeUp, CommandVolumeDown],
            supportsMute
                ? [PropertyVolumeLevel, PropertyIsMuted]
                : [PropertyVolumeLevel])
    {
        SupportsMute = supportsMute;
        MinVolume = minVolume;
        MaxVolume = maxVolume;
        Step = step;
    }
}
