namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// A factory for creating service modules.
/// </summary>
public interface IServiceModuleFactory
{
    /// <summary>
    /// Creates a new service module.
    /// </summary>
    /// <param name="configSectionName">The name of the configuration
    /// section for the module or null if the module does not require
    /// configuration.</param>
    /// <typeparam name="TModuleType">The type of service module to create.</typeparam>
    /// <returns>A new instance of the service module.</returns>
    IServiceModule CreateModule<TModuleType>(string? configSectionName = null)
        where TModuleType : class, IServiceModule;
}
