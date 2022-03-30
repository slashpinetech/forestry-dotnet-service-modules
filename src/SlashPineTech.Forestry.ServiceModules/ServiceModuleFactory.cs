using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// A default implementation of an <see cref="IServiceModule"/> that
/// supports both standard and polymorphic service modules.
/// </summary>
public sealed class ServiceModuleFactory : IServiceModuleFactory
{
    private readonly IConfiguration _configuration;

    /*
     * {
     *   TModule1 -> {
     *     "A" -> TModule1ImplA,
     *     "B" -> TModule1ImplB
     *   },
     *   TModule2 -> {
     *     "A" -> TModule2ImplA,
     *     "B" -> TModule2ImplB
     *   }
     * }
     */
    private readonly Dictionary<Type, IDictionary<string, Type>> _registry = new();

    public ServiceModuleFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Discovers all polymorphic service modules in the provided assembly.
    /// </summary>
    public void DiscoverModules(Assembly assembly)
    {
        var assemblyTypes = assembly.GetTypes();

        var discoveredServiceModuleTypes = assemblyTypes
            .Where(x => x.GetCustomAttributes(typeof(ServiceModuleInfoAttribute), false).Length > 0)
            .ToList();

        foreach (var moduleType in discoveredServiceModuleTypes)
        {
            var discoveredServiceModuleImplementationTypes = assemblyTypes
                .Where(moduleType.IsAssignableFrom)
                .ToList();

            foreach (var moduleImplementationType in discoveredServiceModuleImplementationTypes)
            {
                RegisterModule(moduleType, moduleImplementationType);
            }
        }
    }

    private void RegisterModule(Type moduleType, Type moduleImplementationType)
    {
        if (!_registry.ContainsKey(moduleType))
        {
            _registry.Add(moduleType, new Dictionary<string, Type>());
        }

        var serviceModuleNameAttribute = (ServiceModuleNameAttribute?)Attribute.GetCustomAttribute(moduleImplementationType, typeof(ServiceModuleNameAttribute));
        string moduleImplementationName;
        if (serviceModuleNameAttribute != null)
        {
            moduleImplementationName = serviceModuleNameAttribute.Name;
        }
        else if (moduleImplementationType.Name.EndsWith("Module"))
        {
            moduleImplementationName = moduleImplementationType.Name[..^6];
        }
        else
        {
            moduleImplementationName = moduleImplementationType.Name;
        }

        _registry[moduleType].Add(moduleImplementationName, moduleImplementationType);
    }

    public IServiceModule CreateModule<TModuleType>(string? configSectionName = null) where TModuleType : class, IServiceModule
    {
        var moduleType = typeof(TModuleType);

        // if the registry does not contain this module type then it is not
        // a polymorphic module, so just return a new instance of the module.
        if (!_registry.ContainsKey(moduleType))
        {
            return Activator.CreateInstance<TModuleType>();
        }

        var serviceModuleInfoAttribute = (ServiceModuleInfoAttribute)Attribute.GetCustomAttribute(moduleType, typeof(ServiceModuleInfoAttribute))!;
        var typeQualifierPropertyName = serviceModuleInfoAttribute.QualifierPropertyName;

        // if no configuration was specified, then the most we can do is to
        // return the default implementation.
        if (string.IsNullOrEmpty(configSectionName) || string.IsNullOrEmpty(_configuration.GetSection(configSectionName)[typeQualifierPropertyName]))
        {
            var defaultImplType = serviceModuleInfoAttribute.DefaultImpl;
            if (defaultImplType == null)
            {
                throw new ServiceModuleException($"Attempted to register module '{moduleType}' but no configuration section was specified and no default implementation exists.");
            }

            return (IServiceModule?)Activator.CreateInstance(defaultImplType) ?? throw new ServiceModuleException($"Unable to instantiate an instance of '{defaultImplType}'.");
        }

        var typeQualifier = _configuration.GetSection(configSectionName)[typeQualifierPropertyName]!;
        var availableImplementations = _registry[typeof(TModuleType)];

        if (!availableImplementations.ContainsKey(typeQualifier))
        {
            throw new ServiceModuleException($"No implementation named '{typeQualifier}' found for module '{moduleType}'. Make sure that the implementation is attributed with [ServiceModuleName(\"{typeQualifier}\")].");
        }

        var implementationType = availableImplementations[typeQualifier];

        return (IServiceModule?)Activator.CreateInstance(implementationType) ?? throw new ServiceModuleException($"Unable to instantiate an instance of '{implementationType}'.");
    }
}
