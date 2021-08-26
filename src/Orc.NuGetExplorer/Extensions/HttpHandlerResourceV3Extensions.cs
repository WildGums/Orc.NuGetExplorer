namespace Orc.NuGetExplorer
{
    using System;
    using NuGet.Configuration;
    using NuGet.Protocol;

    internal static class HttpHandlerResourceV3Extensions
    {
        internal static T GetCredentialServiceImplementation<T>(this HttpHandlerResourceV3 httpResourceHandler) where T : class, ICredentialService
        {
            if (HttpHandlerResourceV3.CredentialService is not null)
            {
                return HttpHandlerResourceV3.CredentialService.Value as T;
            }

            throw new InvalidOperationException();
        }

        internal static void ResetCredentials(this HttpHandlerResourceV3 httpResourceHandler)
        {
            var credentialsService = httpResourceHandler.GetCredentialServiceImplementation<ExplorerCredentialService>();
            if (credentialsService is not null)
            {
                credentialsService.ClearRetryCache();
            }
        }
    }
}
