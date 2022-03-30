using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// Extension methods for registering modules with
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceModuleExtensions
{
    public static ServiceModulesBuilder AddModules(this IServiceCollection services,
        Assembly assembly,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        var factory = new ServiceModuleFactory(configuration);
        factory.DiscoverModules(assembly);

        return new ServiceModulesBuilder(
            factory,
            services,
            env,
            configuration
        );
    }
}
