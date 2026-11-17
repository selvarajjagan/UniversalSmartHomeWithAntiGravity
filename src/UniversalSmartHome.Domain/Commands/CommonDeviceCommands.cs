using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Commands;

/// <summary>
/// Factory and specialized strongly-typed commands for smart devices.
/// </summary>
public static class CommonDeviceCommands
{
    public static DeviceCommand TurnOn(DeviceId deviceId) =>
        new(deviceId, PowerCapability.CommandTurnOn, PowerCapability.CapabilityId, CapabilityType.Power);

    public static DeviceCommand TurnOff(DeviceId deviceId) =>
        new(deviceId, PowerCapability.CommandTurnOff, PowerCapability.CapabilityId, CapabilityType.Power);

    public static DeviceCommand Toggle(DeviceId deviceId) =>
        new(deviceId, PowerCapability.CommandToggle, PowerCapability.CapabilityId, CapabilityType.Power);

    public static DeviceCommand SetBrightness(DeviceId deviceId, int brightnessPercentage)
    {
        if (brightnessPercentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(brightnessPercentage), "Brightness must be between 0 and 100.");

        return new DeviceCommand(
            deviceId,
            LightingCapability.CommandSetBrightness,
            LightingCapability.CapabilityId,
            CapabilityType.Lighting,
            new Dictionary<string, object?> { ["Brightness"] = brightnessPercentage });
    }

    public static DeviceCommand SetColor(DeviceId deviceId, RgbColor color) =>
        new(
            deviceId,
            LightingCapability.CommandSetColor,
            LightingCapability.CapabilityId,
            CapabilityType.Lighting,
            new Dictionary<string, object?> { ["Color"] = color.ToHex(), ["Red"] = color.Red, ["Green"] = color.Green, ["Blue"] = color.Blue });

    public static DeviceCommand SetColorTemperature(DeviceId deviceId, int kelvin) =>
        new(
            deviceId,
            LightingCapability.CommandSetColorTemperature,
            LightingCapability.CapabilityId,
            CapabilityType.Lighting,
            new Dictionary<string, object?> { ["ColorTemperatureKelvin"] = kelvin });

    public static DeviceCommand SetTargetTemperature(DeviceId deviceId, double temperature) =>
        new(
            deviceId,
            ThermostatCapability.CommandSetTargetTemperature,
            ThermostatCapability.CapabilityId,
            CapabilityType.Thermostat,
            new Dictionary<string, object?> { ["Temperature"] = temperature });

    public static DeviceCommand SetHvacMode(DeviceId deviceId, HvacMode mode) =>
        new(
            deviceId,
            ThermostatCapability.CommandSetHvacMode,
            ThermostatCapability.CapabilityId,
            CapabilityType.Thermostat,
            new Dictionary<string, object?> { ["HvacMode"] = mode.ToString() });

    public static DeviceCommand SetFanSpeed(DeviceId deviceId, int speedPercentage)
    {
        if (speedPercentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(speedPercentage), "Speed must be between 0 and 100.");

        return new DeviceCommand(
            deviceId,
            FanCapability.CommandSetSpeed,
            FanCapability.CapabilityId,
            CapabilityType.Fan,
            new Dictionary<string, object?> { ["SpeedPercentage"] = speedPercentage });
    }

    public static DeviceCommand SetFanOscillation(DeviceId deviceId, bool oscillate) =>
        new(
            deviceId,
            FanCapability.CommandSetOscillation,
            FanCapability.CapabilityId,
            CapabilityType.Fan,
            new Dictionary<string, object?> { ["Oscillating"] = oscillate });

    public static DeviceCommand SetVolume(DeviceId deviceId, int volumeLevel)
    {
        if (volumeLevel is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(volumeLevel), "Volume must be between 0 and 100.");

        return new DeviceCommand(
            deviceId,
            VolumeCapability.CommandSetVolume,
            VolumeCapability.CapabilityId,
            CapabilityType.Volume,
            new Dictionary<string, object?> { ["VolumeLevel"] = volumeLevel });
    }

    public static DeviceCommand SetMute(DeviceId deviceId, bool isMuted) =>
        new(
            deviceId,
            VolumeCapability.CommandSetMute,
            VolumeCapability.CapabilityId,
            CapabilityType.Volume,
            new Dictionary<string, object?> { ["IsMuted"] = isMuted });

    public static DeviceCommand Play(DeviceId deviceId) =>
        new(deviceId, MediaPlaybackCapability.CommandPlay, MediaPlaybackCapability.CapabilityId, CapabilityType.Playback);

    public static DeviceCommand Pause(DeviceId deviceId) =>
        new(deviceId, MediaPlaybackCapability.CommandPause, MediaPlaybackCapability.CapabilityId, CapabilityType.Playback);

    public static DeviceCommand Stop(DeviceId deviceId) =>
        new(deviceId, MediaPlaybackCapability.CommandStop, MediaPlaybackCapability.CapabilityId, CapabilityType.Playback);

    public static DeviceCommand NextTrack(DeviceId deviceId) =>
        new(deviceId, MediaPlaybackCapability.CommandNext, MediaPlaybackCapability.CapabilityId, CapabilityType.Playback);

    public static DeviceCommand PreviousTrack(DeviceId deviceId) =>
        new(deviceId, MediaPlaybackCapability.CommandPrevious, MediaPlaybackCapability.CapabilityId, CapabilityType.Playback);
}
