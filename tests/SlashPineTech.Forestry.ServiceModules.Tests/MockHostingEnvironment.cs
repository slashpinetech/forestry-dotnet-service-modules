using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace SlashPineTech.Forestry.ServiceModules.Tests;

public class MockHostingEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = string.Empty;
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = string.Empty;
    public string WebRootPath { get; set; } = string.Empty;
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
}
