using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SlashPineTech.Forestry.ServiceModules.Tests;

// Contains the service modules used in ServiceModuleExtensionsTests
public partial class ServiceModuleExtensionsTests
{
    private class SimpleModule : IServiceModule
    {
        public void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSingleton<IExampleService, ExampleService>();
        }
    }

    private class ConfigurationModule : IServiceModule
    {
        public bool IsEnabled { get; set; }

        public void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
        }
    }

    [ServiceModuleInfo(QualifierPropertyName = "Choice", DefaultImpl = typeof(BluePillModule))]
    private abstract class MatrixModule : IServiceModule
    {
        public abstract void Configure(IServiceCollection services, IWebHostEnvironment env);
    }

    // Life-changing truth
    [ServiceModuleName("RedPill")]
    private class RedPillModule : MatrixModule
    {
        public override void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddScoped<IChoice, RedPillChoice>();
        }
    }

    // Content with ignorance
    [ServiceModuleName("BluePill")]
    private class BluePillModule : MatrixModule
    {
        public override void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddScoped<IChoice, BluePillChoice>();
        }
    }

    [ServiceModuleInfo]
    private abstract class TypeModule : IServiceModule
    {
        public abstract void Configure(IServiceCollection services, IWebHostEnvironment env);
    }

    [ServiceModuleName("A")]
    private class TypeAModule : TypeModule
    {
        public override void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddTransient<string>(_ => "A");
        }
    }

    // Name is derived, 'Module' suffix is removed
    private class TypeBModule : TypeModule
    {
        public override void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSingleton(typeof(string), "B");
        }
    }

    // Name is derived, does not end in Module
    private class TypeC : TypeModule
    {
        public override void Configure(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSingleton(typeof(string), "C");
        }
    }
}
