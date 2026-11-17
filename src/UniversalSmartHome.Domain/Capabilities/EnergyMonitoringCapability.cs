using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for devices providing energy consumption and electrical metering.
/// </summary>
public sealed class EnergyMonitoringCapability : DeviceCapability
{
    public const string CapabilityId = "energy_monitoring";

    public const string PropertyPowerWatts = "PowerWatts";
    public const string PropertyEnergyKWh = "EnergyKWh";
    public const string PropertyVoltage = "Voltage";
    public const string PropertyCurrentAmperes = "CurrentAmperes";

    public bool MeasuresPower { get; }
    public bool MeasuresEnergy { get; }
    public bool MeasuresVoltage { get; }
    public bool MeasuresCurrent { get; }

    public EnergyMonitoringCapability(
        bool measuresPower = true,
        bool measuresEnergy = true,
        bool measuresVoltage = false,
        bool measuresCurrent = false)
        : base(
            CapabilityId,
            "Energy Monitoring",
            CapabilityType.EnergyMonitoring,
            [], // Typically read-only
            GetProperties(measuresPower, measuresEnergy, measuresVoltage, measuresCurrent))
    {
        MeasuresPower = measuresPower;
        MeasuresEnergy = measuresEnergy;
        MeasuresVoltage = measuresVoltage;
        MeasuresCurrent = measuresCurrent;
    }

    private static List<string> GetProperties(bool power, bool energy, bool voltage, bool current)
    {
        var list = new List<string>();
        if (power) list.Add(PropertyPowerWatts);
        if (energy) list.Add(PropertyEnergyKWh);
        if (voltage) list.Add(PropertyVoltage);
        if (current) list.Add(PropertyCurrentAmperes);
        return list;
    }
}
