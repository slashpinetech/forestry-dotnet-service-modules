namespace SlashPineTech.Forestry.ServiceModules.Tests;

// Contains fake dependencies wired up in ServiceModuleExtensionsTests
public partial class ServiceModuleExtensionsTests
{
    private interface IExampleService { }
    private class ExampleService : IExampleService { }

    private interface IChoice { }
    private class RedPillChoice : IChoice { }
    private class BluePillChoice : IChoice { }
}
