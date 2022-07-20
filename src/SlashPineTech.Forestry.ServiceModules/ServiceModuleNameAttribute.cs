using System;
using JetBrains.Annotations;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// Describes a named implementation of a service module.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class ServiceModuleNameAttribute : Attribute
{
    public ServiceModuleNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
