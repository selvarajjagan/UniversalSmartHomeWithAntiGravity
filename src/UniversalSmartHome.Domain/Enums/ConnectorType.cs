namespace UniversalSmartHome.Domain.Enums;

/// <summary>
/// Identifies the protocol or platform integration type for a connector.
/// </summary>
public enum ConnectorType
{
    HomeAssistant = 1,
    MCP,
    Matter,
    SmartThings,
    LGThinQ,
    PhilipsHue,
    DirectWiFi,
    Custom
}
