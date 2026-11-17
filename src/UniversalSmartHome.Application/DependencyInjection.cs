using Microsoft.Extensions.DependencyInjection;
using UniversalSmartHome.Application.Services;
using UniversalSmartHome.Connectors.Abstractions;

namespace UniversalSmartHome.Application;

public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registers UniversalSmartHome Application layer services into the dependency injection container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDeviceReconciliationService, DeviceReconciliationService>();
        services.AddSingleton<IConnectorManager, ConnectorManager>();
        services.AddScoped<IDeviceService, DeviceService>();

        return services;
    }
}
