using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// Additional context that may be useful when configuring services.
/// </summary>
public interface IServiceConfigurationContext
{
    /// <summary>
    /// The app configuration that was provided to the <code>.AddModules()</code>
    /// call during startup.
    /// </summary>
    /// <remarks>
    /// It is still recommended to bind configuration directly to a module, but
    /// sometimes it can be useful to look at the entire configuration object or
    /// at other configuration sections, such as for feature flagging.
    /// </remarks>
    IConfiguration Configuration { get; }

    /// <summary>
    /// The web environment that was provided to the <code>.AddModules()</code>
    /// call during startup.
    /// </summary>
    IWebHostEnvironment Environment { get; }
}
