namespace Orc.NuGetExplorer.Example.Services
{
    using System;
    using System.Windows.Media;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Example.PackageManagement;
    using Orc.NuGetExplorer.Example.Providers;
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
            ITypeFactory typeFactory,
            IExtensibleProjectLocator projectLocator,
            IAccentColorService accentColorService)
            : base(languageService, credentialProviderLoaderService, nuGetProjectUpgradeService, nuGetConfigurationService, vmLocator, typeFactory)
        {
            ArgumentNullException.ThrowIfNull(projectLocator);
            ArgumentNullException.ThrowIfNull(accentColorService);

            var serviceLocator = ServiceLocator.Default;

            // Example: override default project
            serviceLocator.RegisterType<IDefaultExtensibleProjectProvider, NuGetProjectProvider>();

            serviceLocator.RegisterType<INuGetConfigurationResetService, ExampleNuGetConfigurationResetService>();

            // initialize theme
            accentColorService.SetAccentColor(Colors.Orange);

            // add loggers
            serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();
            var catelListener = serviceLocator.RegisterTypeAndInstantiate<CatelLogListener>();
            LogManager.AddListener(catelListener);

            // add upgrade listener
            serviceLocator.RegisterTypeAndInstantiate<ExampleUpgradeListener>();

            // IApiPackageRegistry testing
            var apiRegistry = serviceLocator.ResolveType<IApiPackageRegistry>();
            //apiRegistry.Register("PackageName.Api", "1.0.0-version");

            // Example: changing storage for Credentials
            //credentialProviderLoaderService.SetCredentialPolicy(Enums.CredentialStoragePolicy.WindowsVaultConfigurationFallback);

            // Override size of packages queries
            nuGetConfigurationService.SetPackageQuerySize(40);
        }
    }
}
