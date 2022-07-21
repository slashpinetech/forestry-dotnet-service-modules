using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SlashPineTech.Forestry.ServiceModules;

public class ServiceConfigurationContext : IServiceConfigurationContext
{
    public ServiceConfigurationContext(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }
}
