using System;
using System.Collections.Generic;
using System.Text;

namespace SlashPineTech.Forestry.ServiceModules;

/// <summary>
/// Describes one or more errors in module configuration.
/// </summary>
public sealed class ModuleConfigurationException : Exception
{
    /// <summary>
    /// Instantiates a new <see cref="ModuleConfigurationException"/>.
    /// </summary>
    /// <param name="configSectionName">The name of the configuration
    /// section that the module was attempting to bind to.</param>
    /// <param name="messages">The validation errors that were encountered.</param>
    public ModuleConfigurationException(string configSectionName, IEnumerable<string> messages) : base(FormatMessage(configSectionName, messages))
    {
    }

    private static string FormatMessage(string configSectionName, IEnumerable<string> messages)
    {
        var formattedMessage = new StringBuilder();
        formattedMessage.Append("The configuration for section '");
        formattedMessage.Append(configSectionName);
        formattedMessage.AppendLine("' has the following errors:");

        foreach (var message in messages)
        {
            formattedMessage.Append("  * ");
            formattedMessage.AppendLine(message);
        }

        return formattedMessage.ToString();
    }
}
