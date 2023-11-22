namespace Orc.NuGetExplorer;

using System;
using System.Runtime.Serialization;

[Serializable]
public class ApiValidationException : Exception
{
    public ApiValidationException()
    {
    }

    public ApiValidationException(string message)
        : base(message)
    {
    }

    public ApiValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
