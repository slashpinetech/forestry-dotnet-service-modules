using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SlashPineTech.Forestry.ServiceModules;

public class ServiceModulesBuilder
{
    private readonly IServiceModuleFactory _factory;
    private readonly IServiceCollection _services;
    private readonly IServiceConfigurationContext _serviceConfigurationContext;

    public ServiceModulesBuilder(
        IServiceModuleFactory factory,
        IServiceCollection services,
        IServiceConfigurationContext serviceConfigurationContext
    )
    {
        _factory = factory;
        _services = services;
        _serviceConfigurationContext = serviceConfigurationContext;
    }

    /// <summary>
    /// Adds a service module to the services collection.
    /// </summary>
    /// <param name="configSectionName">The name of the configuration
    /// section to bind to or null if the module does not have
    /// configuration.</param>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    public ServiceModulesBuilder AddModule<TModule>(string? configSectionName = null) where TModule : class, IServiceModule
    {
        var module = _factory.CreateModule<TModule>(configSectionName);

        if (!string.IsNullOrEmpty(configSectionName))
        {
            _serviceConfigurationContext.Configuration.GetSection(configSectionName)?.Bind(module);

            ModuleValidator.Validate(module, configSectionName);
        }

        module.Configure(_services, _serviceConfigurationContext);

        return this;
    }
}
