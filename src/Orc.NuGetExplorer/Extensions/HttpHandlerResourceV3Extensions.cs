namespace Orc.NuGetExplorer;

using System;
using Catel.Logging;
using NuGet.Configuration;
using NuGet.Protocol;

public static class HttpHandlerResourceV3Extensions
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public static T GetCredentialServiceImplementation<T>(this HttpHandlerResourceV3 httpResourceHandler)
        where T : class, ICredentialService
    {
        ArgumentNullException.ThrowIfNull(httpResourceHandler);

        if (HttpHandlerResourceV3.CredentialService is not null)
        {
            return (T)HttpHandlerResourceV3.CredentialService.Value;
        }

        throw Log.ErrorAndCreateException<InvalidOperationException>("CredentialService is null");
    }

    public static void ResetCredentials(this HttpHandlerResourceV3 httpResourceHandler)
    {
        ArgumentNullException.ThrowIfNull(httpResourceHandler);

        var credentialsService = httpResourceHandler.GetCredentialServiceImplementation<ExplorerCredentialService>();

        if (credentialsService is not null)
        {
            credentialsService.ClearRetryCache();
        }
    }
}
