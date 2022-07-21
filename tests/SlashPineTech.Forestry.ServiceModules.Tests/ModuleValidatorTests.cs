using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace SlashPineTech.Forestry.ServiceModules.Tests;

public class ModuleValidatorTests
{
    [Fact]
    public void Validate_Does_Not_Throw_ModuleConfigurationException_For_Valid_Modules()
    {
        var module = new ExampleModule
        {
            ExampleProperty = "Valid Value"
        };

        ModuleValidator.Validate(module, "Section");
    }

    [Fact]
    public void Validate_Throws_ModuleConfigurationException_For_Invalid_Modules()
    {
        var module = new ExampleModule
        {
            ExampleProperty = null
        };

        Should.Throw<ModuleConfigurationException>(() =>
        {
            ModuleValidator.Validate(module, "Section");
        }).Message.ShouldBe(@"The configuration for section 'Section' has the following errors:
  * The ExampleProperty field is required.
");
    }

    #nullable disable
    private class ExampleModule : IServiceModule
    {
        [Required]
        public string ExampleProperty { get; set; }

        public void Configure(IServiceCollection services, IServiceConfigurationContext ctx)
        {
        }
    }
}
