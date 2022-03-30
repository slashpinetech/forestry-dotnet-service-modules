using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SlashPineTech.Forestry.ServiceModules;

public static class ModuleValidator
{
    /// <summary>
    /// Validates the properties of an <see cref="IServiceModule"/>.
    /// </summary>
    /// <param name="module">The module instance to validate.</param>
    /// <param name="configSectionName">The name of the configuration
    /// section that the module is bound to.</param>
    /// <exception cref="ModuleConfigurationException">If module validation
    /// failed.</exception>
    public static void Validate(IServiceModule module, string configSectionName)
    {
        var ctx = new ValidationContext(module);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(module, ctx, validationResults, true))
        {
            throw new ModuleConfigurationException(
                configSectionName,
                validationResults.Select(x => x.ErrorMessage)!
            );
        }
    }
}
