using Microsoft.Extensions.DependencyInjection;

namespace SlashPineTech.Forestry.ServiceModules.Tests;

// Contains the service modules used in ServiceModuleExtensionsTests
public partial class ServiceModuleExtensionsTests
{
    private class SimpleModule : IServiceModule
    {
        public void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
            services.AddSingleton<IExampleService, ExampleService>();
        }
    }

    private class ConfigurationModule : IServiceModule
    {
        public bool IsEnabled { get; set; }

        public void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
        }
    }

    [ServiceModuleInfo(QualifierPropertyName = "Choice", DefaultImpl = typeof(BluePillModule))]
    private abstract class MatrixModule : IServiceModule
    {
        public abstract void Configure(IServiceCollection services, IServiceConfigurationContext ctx);
    }

    // Life-changing truth
    [ServiceModuleName("RedPill")]
    private class RedPillModule : MatrixModule
    {
        public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
            services.AddScoped<IChoice, RedPillChoice>();
        }
    }

    // Content with ignorance
    [ServiceModuleName("BluePill")]
    private class BluePillModule : MatrixModule
    {
        public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
            services.AddScoped<IChoice, BluePillChoice>();
        }
    }

    [ServiceModuleInfo]
    private abstract class TypeModule : IServiceModule
    {
        public abstract void Configure(IServiceCollection services, IServiceConfigurationContext ctx);
    }

    [ServiceModuleName("A")]
    private class TypeAModule : TypeModule
    {
        public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
            services.AddTransient<string>(_ => "A");
        }
    }

    // Name is derived, 'Module' suffix is removed
    private class TypeBModule : TypeModule
    {
        public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
            services.AddSingleton(typeof(string), "B");
        }
    }

    // Name is derived, does not end in Module
    private class TypeC : TypeModule
    {
        public override void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
            services.AddSingleton(typeof(string), "C");
        }
    }
}
