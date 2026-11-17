using UniversalSmartHome.Domain.Capabilities;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.States;

/// <summary>
/// Strongly-typed power state.
/// </summary>
public readonly record struct PowerStateValue(bool IsOn)
{
    public DeviceState ToDeviceState(DeviceId deviceId) =>
        new(
            deviceId,
            PowerCapability.CapabilityId,
            CapabilityType.Power,
            new Dictionary<string, object?> { [PowerCapability.PropertyIsOn] = IsOn });

    public static PowerStateValue FromDeviceState(DeviceState state) =>
        new(state.GetProperty<bool>(PowerCapability.PropertyIsOn));
}

/// <summary>
/// Strongly-typed lighting state.
/// </summary>
public readonly record struct LightingStateValue(
    bool IsOn,
    int? BrightnessPercentage = null,
    RgbColor? Color = null,
    int? ColorTemperatureKelvin = null)
{
    public DeviceState ToDeviceState(DeviceId deviceId)
    {
        var dict = new Dictionary<string, object?>
        {
            [PowerCapability.PropertyIsOn] = IsOn
        };
        if (BrightnessPercentage.HasValue) dict[LightingCapability.PropertyBrightness] = BrightnessPercentage.Value;
        if (Color.HasValue) dict[LightingCapability.PropertyColor] = Color.Value.ToHex();
        if (ColorTemperatureKelvin.HasValue) dict[LightingCapability.PropertyColorTemperature] = ColorTemperatureKelvin.Value;

        return new DeviceState(deviceId, LightingCapability.CapabilityId, CapabilityType.Lighting, dict);
    }

    public static LightingStateValue FromDeviceState(DeviceState state)
    {
        var isOn = state.GetProperty<bool>(PowerCapability.PropertyIsOn);
        var brightness = state.GetProperty<int?>(LightingCapability.PropertyBrightness);
        var colorHex = state.GetProperty<string>(LightingCapability.PropertyColor);
        var color = !string.IsNullOrEmpty(colorHex) ? RgbColor.FromHex(colorHex) : (RgbColor?)null;
        var colorTemp = state.GetProperty<int?>(LightingCapability.PropertyColorTemperature);

        return new LightingStateValue(isOn, brightness, color, colorTemp);
    }
}

/// <summary>
/// Strongly-typed climate/thermostat state.
/// </summary>
public readonly record struct ThermostatStateValue(
    double? CurrentTemperature,
    double? TargetTemperature,
    HvacMode HvacMode,
    double? TargetHigh = null,
    double? TargetLow = null)
{
    public DeviceState ToDeviceState(DeviceId deviceId)
    {
        var dict = new Dictionary<string, object?>
        {
            [ThermostatCapability.PropertyHvacMode] = HvacMode.ToString()
        };
        if (CurrentTemperature.HasValue) dict[ThermostatCapability.PropertyCurrentTemperature] = CurrentTemperature.Value;
        if (TargetTemperature.HasValue) dict[ThermostatCapability.PropertyTargetTemperature] = TargetTemperature.Value;
        if (TargetHigh.HasValue) dict[ThermostatCapability.PropertyTargetTemperatureHigh] = TargetHigh.Value;
        if (TargetLow.HasValue) dict[ThermostatCapability.PropertyTargetTemperatureLow] = TargetLow.Value;

        return new DeviceState(deviceId, ThermostatCapability.CapabilityId, CapabilityType.Thermostat, dict);
    }

    public static ThermostatStateValue FromDeviceState(DeviceState state)
    {
        var current = state.GetProperty<double?>(ThermostatCapability.PropertyCurrentTemperature);
        var target = state.GetProperty<double?>(ThermostatCapability.PropertyTargetTemperature);
        var modeStr = state.GetProperty<string>(ThermostatCapability.PropertyHvacMode);
        var mode = Enum.TryParse<HvacMode>(modeStr, true, out var parsed) ? parsed : HvacMode.Off;
        var high = state.GetProperty<double?>(ThermostatCapability.PropertyTargetTemperatureHigh);
        var low = state.GetProperty<double?>(ThermostatCapability.PropertyTargetTemperatureLow);

        return new ThermostatStateValue(current, target, mode, high, low);
    }
}

/// <summary>
/// Strongly-typed fan state.
/// </summary>
public readonly record struct FanStateValue(
    bool IsOn,
    int? SpeedPercentage = null,
    bool? IsOscillating = null,
    string? Direction = null)
{
    public DeviceState ToDeviceState(DeviceId deviceId)
    {
        var dict = new Dictionary<string, object?>
        {
            [PowerCapability.PropertyIsOn] = IsOn
        };
        if (SpeedPercentage.HasValue) dict[FanCapability.PropertySpeedPercentage] = SpeedPercentage.Value;
        if (IsOscillating.HasValue) dict[FanCapability.PropertyIsOscillating] = IsOscillating.Value;
        if (Direction != null) dict[FanCapability.PropertyDirection] = Direction;

        return new DeviceState(deviceId, FanCapability.CapabilityId, CapabilityType.Fan, dict);
    }

    public static FanStateValue FromDeviceState(DeviceState state)
    {
        var isOn = state.GetProperty<bool>(PowerCapability.PropertyIsOn);
        var speed = state.GetProperty<int?>(FanCapability.PropertySpeedPercentage);
        var osc = state.GetProperty<bool?>(FanCapability.PropertyIsOscillating);
        var dir = state.GetProperty<string>(FanCapability.PropertyDirection);

        return new FanStateValue(isOn, speed, osc, dir);
    }
}

/// <summary>
/// Strongly-typed media and volume state.
/// </summary>
public readonly record struct MediaPlaybackStateValue(
    PlaybackStatus Status,
    int VolumeLevel,
    bool IsMuted,
    string? Title = null,
    string? Artist = null,
    string? Album = null,
    int? DurationSeconds = null,
    int? PositionSeconds = null)
{
    public DeviceState ToDeviceState(DeviceId deviceId)
    {
        var dict = new Dictionary<string, object?>
        {
            [MediaPlaybackCapability.PropertyPlaybackStatus] = Status.ToString(),
            [VolumeCapability.PropertyVolumeLevel] = VolumeLevel,
            [VolumeCapability.PropertyIsMuted] = IsMuted
        };
        if (Title != null) dict[MediaPlaybackCapability.PropertyMediaTitle] = Title;
        if (Artist != null) dict[MediaPlaybackCapability.PropertyMediaArtist] = Artist;
        if (Album != null) dict[MediaPlaybackCapability.PropertyMediaAlbum] = Album;
        if (DurationSeconds.HasValue) dict[MediaPlaybackCapability.PropertyMediaDurationSeconds] = DurationSeconds.Value;
        if (PositionSeconds.HasValue) dict[MediaPlaybackCapability.PropertyMediaPositionSeconds] = PositionSeconds.Value;

        return new DeviceState(deviceId, MediaPlaybackCapability.CapabilityId, CapabilityType.Playback, dict);
    }

    public static MediaPlaybackStateValue FromDeviceState(DeviceState state)
    {
        var statusStr = state.GetProperty<string>(MediaPlaybackCapability.PropertyPlaybackStatus);
        var status = Enum.TryParse<PlaybackStatus>(statusStr, true, out var parsed) ? parsed : PlaybackStatus.Idle;
        var volume = state.GetProperty<int>(VolumeCapability.PropertyVolumeLevel);
        var muted = state.GetProperty<bool>(VolumeCapability.PropertyIsMuted);
        var title = state.GetProperty<string>(MediaPlaybackCapability.PropertyMediaTitle);
        var artist = state.GetProperty<string>(MediaPlaybackCapability.PropertyMediaArtist);
        var album = state.GetProperty<string>(MediaPlaybackCapability.PropertyMediaAlbum);
        var dur = state.GetProperty<int?>(MediaPlaybackCapability.PropertyMediaDurationSeconds);
        var pos = state.GetProperty<int?>(MediaPlaybackCapability.PropertyMediaPositionSeconds);

        return new MediaPlaybackStateValue(status, volume, muted, title, artist, album, dur, pos);
    }
}
