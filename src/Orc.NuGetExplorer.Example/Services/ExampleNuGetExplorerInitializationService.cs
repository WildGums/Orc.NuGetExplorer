namespace Orc.NuGetExplorer.Example;

using System;
using Catel.MVVM;
using Catel.Services;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Services;
using Orc.Theming;

public class ExampleNuGetExplorerInitializationService : NuGetExplorerInitializationService
{
    public ExampleNuGetExplorerInitializationService(
        ILanguageService languageService,
        ICredentialProviderLoaderService credentialProviderLoaderService,
        INuGetProjectUpgradeService nuGetProjectUpgradeService,
        INuGetConfigurationService nuGetConfigurationService,
        IViewModelLocator vmLocator,
        IServiceProvider serviceProvider,
        IExtensibleProjectLocator projectLocator,
        IAccentColorService accentColorService,
        ICommandManager commandManager)
        : base(languageService, credentialProviderLoaderService, nuGetProjectUpgradeService, 
            nuGetConfigurationService, vmLocator, serviceProvider, commandManager)
    {
 
        // IApiPackageRegistry testing
        //var apiRegistry = serviceLocator.ResolveType<IApiPackageRegistry>();
        //apiRegistry.Register("PackageName.Api", "1.0.0-version");

        // Example: changing storage for Credentials
        //credentialProviderLoaderService.SetCredentialPolicy(Enums.CredentialStoragePolicy.WindowsVaultConfigurationFallback);

        // Override size of packages queries
        nuGetConfigurationService.SetPackageQuerySize(40);
    }
}
