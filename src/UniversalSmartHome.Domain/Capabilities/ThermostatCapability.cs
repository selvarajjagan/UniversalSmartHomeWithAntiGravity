using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for climate and thermostat devices with temperature and HVAC mode controls.
/// </summary>
public sealed class ThermostatCapability : DeviceCapability
{
    public const string CapabilityId = "thermostat";
    public const string CommandSetTargetTemperature = "SetTargetTemperature";
    public const string CommandSetHvacMode = "SetHvacMode";

    public const string PropertyCurrentTemperature = "CurrentTemperature";
    public const string PropertyTargetTemperature = "TargetTemperature";
    public const string PropertyHvacMode = "HvacMode";
    public const string PropertyTargetTemperatureHigh = "TargetTemperatureHigh";
    public const string PropertyTargetTemperatureLow = "TargetTemperatureLow";

    public IReadOnlyList<HvacMode> SupportedHvacModes { get; }
    public double MinTemperature { get; }
    public double MaxTemperature { get; }
    public double TemperatureStep { get; }

    public ThermostatCapability(
        IEnumerable<HvacMode>? supportedModes = null,
        double minTemperature = 10.0,
        double maxTemperature = 35.0,
        double temperatureStep = 0.5)
        : base(
            CapabilityId,
            "Thermostat / Climate",
            CapabilityType.Thermostat,
            [CommandSetTargetTemperature, CommandSetHvacMode],
            [PropertyCurrentTemperature, PropertyTargetTemperature, PropertyHvacMode])
    {
        SupportedHvacModes = (supportedModes ?? [HvacMode.Off, HvacMode.Heat, HvacMode.Cool, HvacMode.Auto]).ToList();
        MinTemperature = minTemperature;
        MaxTemperature = maxTemperature;
        TemperatureStep = temperatureStep;
    }
}
