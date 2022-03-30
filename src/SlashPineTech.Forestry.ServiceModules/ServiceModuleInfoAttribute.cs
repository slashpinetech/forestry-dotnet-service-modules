using System;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// Describes a polymorphic service module.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class ServiceModuleInfoAttribute : Attribute
{
    /// <summary>
    /// The name of the property that serves as the type qualifier for the
    /// module described by this attribute.
    /// </summary>
    public string QualifierPropertyName { get; init; } = "Type";

    /// <summary>
    /// The type that represents the default implementation of the module
    /// described by this attribute or null if this module does not have a
    /// default implementation.
    /// </summary>
    public Type? DefaultImpl { get; init; }
}
