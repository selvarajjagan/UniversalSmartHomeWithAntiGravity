using Microsoft.Extensions.Logging;
using UniversalSmartHome.Connectors.Abstractions;
using UniversalSmartHome.Connectors.Abstractions.Models;
using UniversalSmartHome.Domain.Devices;
using UniversalSmartHome.Domain.Enums;
using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Application.Services;

/// <summary>
/// Reconciles discovered devices against existing smart devices and device mappings.
/// Avoids naive name-based merging and maintains strict external identity tracing.
/// </summary>
public sealed class DeviceReconciliationService : IDeviceReconciliationService
{
    private readonly ILogger<DeviceReconciliationService> _logger;

    public DeviceReconciliationService(ILogger<DeviceReconciliationService> logger)
    {
        _logger = logger;
    }

    public SmartDevice Reconcile(
        DiscoveredDevice discoveredDevice,
        ConnectorId connectorId,
        HomeId homeId,
        IEnumerable<SmartDevice> existingDevices,
        IEnumerable<DeviceMapping> existingMappings)
    {
        ArgumentNullException.ThrowIfNull(discoveredDevice);

        // 1. Check if we already have a mapping for this connector + externalDeviceId
        var mapping = existingMappings.FirstOrDefault(m =>
            m.ConnectorId == connectorId &&
            string.Equals(m.ExternalDeviceId, discoveredDevice.ExternalDeviceId, StringComparison.OrdinalIgnoreCase));

        if (mapping != null)
        {
            var matchedDevice = existingDevices.FirstOrDefault(d => d.Id == mapping.DeviceId);
            if (matchedDevice != null)
            {
                _logger.LogDebug(
                    "Reconciled device '{ExternalId}' to existing SmartDevice '{DeviceId}' via mapping.",
                    discoveredDevice.ExternalDeviceId, matchedDevice.Id);

                // Update device capabilities and status
                foreach (var cap in discoveredDevice.Capabilities)
                {
                    matchedDevice.AddCapability(cap);
                }

                foreach (var state in discoveredDevice.InitialStates)
                {
                    matchedDevice.UpdateState(state);
                }

                matchedDevice.UpdateConnectionStatus(DeviceConnectionStatus.Online);
                return matchedDevice;
            }
        }

        // 2. Check if a device with this exact ConnectorId and ExternalDeviceId already exists in the device list
        var directMatch = existingDevices.FirstOrDefault(d =>
            d.ConnectorId == connectorId &&
            string.Equals(d.ExternalDeviceId, discoveredDevice.ExternalDeviceId, StringComparison.OrdinalIgnoreCase));

        if (directMatch != null)
        {
            _logger.LogDebug(
                "Reconciled device '{ExternalId}' to existing direct SmartDevice '{DeviceId}'.",
                discoveredDevice.ExternalDeviceId, directMatch.Id);

            foreach (var cap in discoveredDevice.Capabilities)
            {
                directMatch.AddCapability(cap);
            }

            foreach (var state in discoveredDevice.InitialStates)
            {
                directMatch.UpdateState(state);
            }

            directMatch.UpdateConnectionStatus(DeviceConnectionStatus.Online);
            return directMatch;
        }

        // 3. Create a brand new SmartDevice instance
        var newDeviceId = DeviceId.New();
        _logger.LogInformation(
            "Creating new SmartDevice '{DeviceId}' for discovered entity '{Name}' ({ExternalId}) from connector '{ConnectorId}'.",
            newDeviceId, discoveredDevice.Name, discoveredDevice.ExternalDeviceId, connectorId);

        var device = new SmartDevice(
            id: newDeviceId,
            name: discoveredDevice.Name,
            manufacturer: discoveredDevice.Manufacturer,
            model: discoveredDevice.Model,
            deviceType: discoveredDevice.SuggestedType,
            homeId: homeId,
            connectorId: connectorId,
            externalDeviceId: discoveredDevice.ExternalDeviceId,
            connectionStatus: DeviceConnectionStatus.Online,
            capabilities: discoveredDevice.Capabilities,
            metadata: discoveredDevice.Metadata);

        foreach (var state in discoveredDevice.InitialStates)
        {
            device.UpdateState(state);
        }

        return device;
    }
}
