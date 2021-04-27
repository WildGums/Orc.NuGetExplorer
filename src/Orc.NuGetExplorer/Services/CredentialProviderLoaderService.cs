namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Credentials;
    using NuGet.Protocol;
    using NuGetExplorer.Providers;

    internal class CredentialProviderLoaderService : ICredentialProviderLoaderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IConfigurationService _configurationService;

        public CredentialProviderLoaderService(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;

            //this provider add yourself as default V3 credential

            //set own provider 
            HttpHandlerResourceV3.CredentialService = new Lazy<ICredentialService>(() => new ExplorerCredentialService(
                    new AsyncLazy<IEnumerable<ICredentialProvider>>(() => GetCredentialProvidersAsync()),
                    false,
                    true)
                );
        }

        public void SetCredentialPolicy(CredentialStoragePolicy storagePolicy)
        {
            Log.Info($"Changing credential storage policy to {storagePolicy}");

            _configurationService.SetCredentialStoragePolicy(storagePolicy);
        }

        public async Task<IEnumerable<ICredentialProvider>> GetCredentialProvidersAsync()
        {
            var providers = new List<ICredentialProvider>();

            var windowsUserProvider = new WindowsCredentialProvider(_configurationService);

            providers.Add(windowsUserProvider);

            return providers;
        }
    }
}
