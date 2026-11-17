using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for lighting devices that support dimming, color temperature, and RGB colors.
/// </summary>
public sealed class LightingCapability : DeviceCapability
{
    public const string CapabilityId = "lighting";
    public const string CommandSetBrightness = "SetBrightness";
    public const string CommandSetColor = "SetColor";
    public const string CommandSetColorTemperature = "SetColorTemperature";

    public const string PropertyBrightness = "Brightness";
    public const string PropertyColor = "Color";
    public const string PropertyColorTemperature = "ColorTemperature";

    public bool SupportsBrightness { get; }
    public bool SupportsColor { get; }
    public bool SupportsColorTemperature { get; }
    public int? MinColorTempKelvin { get; }
    public int? MaxColorTempKelvin { get; }

    public LightingCapability(
        bool supportsBrightness = true,
        bool supportsColor = false,
        bool supportsColorTemperature = false,
        int? minColorTempKelvin = null,
        int? maxColorTempKelvin = null)
        : base(
            CapabilityId,
            "Lighting Controls",
            CapabilityType.Lighting,
            GetCommands(supportsBrightness, supportsColor, supportsColorTemperature),
            GetProperties(supportsBrightness, supportsColor, supportsColorTemperature))
    {
        SupportsBrightness = supportsBrightness;
        SupportsColor = supportsColor;
        SupportsColorTemperature = supportsColorTemperature;
        MinColorTempKelvin = minColorTempKelvin;
        MaxColorTempKelvin = maxColorTempKelvin;
    }

    private static List<string> GetCommands(bool brightness, bool color, bool colorTemp)
    {
        var list = new List<string>();
        if (brightness) list.Add(CommandSetBrightness);
        if (color) list.Add(CommandSetColor);
        if (colorTemp) list.Add(CommandSetColorTemperature);
        return list;
    }

    private static List<string> GetProperties(bool brightness, bool color, bool colorTemp)
    {
        var list = new List<string>();
        if (brightness) list.Add(PropertyBrightness);
        if (color) list.Add(PropertyColor);
        if (colorTemp) list.Add(PropertyColorTemperature);
        return list;
    }
}
