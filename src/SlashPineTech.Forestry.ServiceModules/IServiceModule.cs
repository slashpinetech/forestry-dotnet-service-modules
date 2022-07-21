using Microsoft.Extensions.DependencyInjection;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// Encapsulates ASP.NET Core service configuration for a module.
/// </summary>
public interface IServiceModule
{
    /// <summary>
    /// Configures the provided services for this module.
    /// </summary>
    void Configure(IServiceCollection services, IServiceConfigurationContext ctx);
}
