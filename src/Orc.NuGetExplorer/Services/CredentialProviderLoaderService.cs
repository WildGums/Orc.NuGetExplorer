namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Configuration;
using Catel.Logging;
using Catel.Services;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Credentials;
using NuGet.Protocol;
using NuGetExplorer.Providers;

internal class CredentialProviderLoaderService : ICredentialProviderLoaderService
{
    private static readonly Microsoft.Extensions.Logging.ILogger Logger = LogManager.GetLogger(typeof(CredentialProviderLoaderService));

    private readonly IConfigurationService _configurationService;
    private readonly ILanguageService _languageService;

    public CredentialProviderLoaderService(IConfigurationService configurationService, ILanguageService languageService)
    {
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(languageService);

        _configurationService = configurationService;
        _languageService = languageService;

        // this provider add yourself as default V3 credential

        // set own provider 
#pragma warning disable IDISP005 // Return type should indicate that the value should be disposed.
        HttpHandlerResourceV3.CredentialService = new Lazy<ICredentialService>(() => new ExplorerCredentialService(
            new AsyncLazy<IReadOnlyList<ICredentialProvider>>(() => GetCredentialProvidersAsync()),
            false,
            true)
        );
#pragma warning restore IDISP005 // Return type should indicate that the value should be disposed.
    }

    public void SetCredentialPolicy(CredentialStoragePolicy storagePolicy)
    {
        Logger.LogInformation("Changing credential storage policy to {StoragePolicy}", storagePolicy);

        _configurationService.SetCredentialStoragePolicy(storagePolicy);
    }

    public async Task<IReadOnlyList<ICredentialProvider>> GetCredentialProvidersAsync()
    {
        var providers = new List<ICredentialProvider>();

        var windowsUserProvider = new WindowsCredentialProvider(_configurationService, _languageService);

        providers.Add(windowsUserProvider);

        return providers;
    }
}
