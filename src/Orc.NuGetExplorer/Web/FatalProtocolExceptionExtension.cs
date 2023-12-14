namespace Orc.NuGetExplorer.Web;

using System;
using NuGet.Protocol.Core.Types;

public static class FatalProtocolExceptionExtension
{
    public static bool HidesUnauthorizedError(this FatalProtocolException fatalProtocolException)
    {
        ArgumentNullException.ThrowIfNull(fatalProtocolException);

        return fatalProtocolException.Message.Contains("returned an unexpected status code '401 Unauthorized'");
    }

    public static bool HidesForbiddenError(this FatalProtocolException fatalProtocolException)
    {
        ArgumentNullException.ThrowIfNull(fatalProtocolException);

        return fatalProtocolException.Message.Contains("returned an unexpected status code '403 Forbidden'");
    }
}