using System;

namespace SlashPineTech.Forestry.ServiceModules;

public class ServiceModuleException : Exception
{
    public ServiceModuleException(string? message) : base(message)
    {
    }
}
