using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace SlashPineTech.Forestry.ServiceModules.Tests;

public partial class ServiceModuleExtensionsTests
{
    [Fact]
    public void AddModules_AddModule_Adds_Configurations_To_ServiceCollection()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<SimpleModule>();

        services.ShouldContain(it => it.Lifetime == ServiceLifetime.Singleton &&
                                     it.ServiceType == typeof(IExampleService) &&
                                     it.ImplementationType == typeof(ExampleService));
    }

    [Fact]
    public void AddModules_AddModule_Binds_Configuration_To_Module()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();
        cfg.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Example:IsEnabled", "true")
        });

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<ConfigurationModule>("Example");
    }

    [Fact]
    public void AddModules_AddModule_Binds_Configuration_To_RedPill_Module()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();
        cfg.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Matrix:Choice", "RedPill")
        });

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<MatrixModule>("Matrix");

        services.ShouldContain(it => it.Lifetime == ServiceLifetime.Scoped &&
                                     it.ServiceType == typeof(IChoice) &&
                                     it.ImplementationType == typeof(RedPillChoice));
    }

    [Fact]
    public void AddModules_AddModule_Binds_Configuration_To_BluePill_Module()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();
        cfg.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Matrix:Choice", "BluePill")
        });

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<MatrixModule>("Matrix");

        services.ShouldContain(it => it.Lifetime == ServiceLifetime.Scoped &&
                                     it.ServiceType == typeof(IChoice) &&
                                     it.ImplementationType == typeof(BluePillChoice));
    }

    [Fact]
    public void AddModules_AddModule_Binds_Configuration_To_Default_Module_When_Choice_Is_Absent()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<MatrixModule>("Matrix");

        services.ShouldContain(it => it.Lifetime == ServiceLifetime.Scoped &&
                                     it.ServiceType == typeof(IChoice) &&
                                     it.ImplementationType == typeof(BluePillChoice));
    }

    [Fact]
    public void AddModule_Throws_ServiceModuleException_If_No_Type_Is_Configured_And_No_Default_Impl()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();

        Should.Throw<ServiceModuleException>(() =>
        {
            services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
                .AddModule<TypeModule>("Type");
        }).Message.ShouldBe("Attempted to register module 'SlashPineTech.Forestry.ServiceModules.Tests.ServiceModuleExtensionsTests+TypeModule' but no configuration section was specified and no default implementation exists.");
    }

    [Fact]
    public void AddModule_Throws_ServiceModuleException_If_An_Invalid_Type_Is_Specified()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();
        cfg.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Type:Type", "Z")
        });

        Should.Throw<ServiceModuleException>(() =>
        {
            services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
                .AddModule<TypeModule>("Type");
        }).Message.ShouldBe(@"No implementation named 'Z' found for module 'SlashPineTech.Forestry.ServiceModules.Tests.ServiceModuleExtensionsTests+TypeModule'. Make sure that the implementation is attributed with [ServiceModuleName(""Z"")].");
    }

    [Fact]
    public void Module_Binding_Selects_Module_With_Module_Suffix_And_No_Attribute()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();
        cfg.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Type:Type", "TypeB")
        });

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<TypeModule>("Type");

        services.ShouldContain(it => it.Lifetime == ServiceLifetime.Singleton &&
                                     it.ServiceType == typeof(string) &&
                                     ReferenceEquals(it.ImplementationInstance, "B"));
    }

    [Fact]
    public void Module_Binding_Selects_Module_With_No_Module_Suffix_And_No_Attribute()
    {
        var services = new ServiceCollection();
        var env = new MockHostingEnvironment();
        var cfg = new ConfigurationManager();
        cfg.AddInMemoryCollection(new List<KeyValuePair<string, string>>
        {
            new("Type:Type", "TypeC")
        });

        services.AddModules(typeof(ServiceModuleExtensionsTests).Assembly, env, cfg)
            .AddModule<TypeModule>("Type");

        services.ShouldContain(it => it.Lifetime == ServiceLifetime.Singleton &&
                                     it.ServiceType == typeof(string) &&
                                     ReferenceEquals(it.ImplementationInstance, "C"));
    }
}
