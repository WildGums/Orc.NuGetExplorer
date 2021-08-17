namespace Orc.NuGetExplorer.Example.Services
{
    using System.Windows.Media;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Example.PackageManagement;
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
            Argument.IsNotNull(() => projectLocator);
            Argument.IsNotNull(() => accentColorService);

            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterType<INuGetConfigurationResetService, ExampleNuGetConfigurationResetService>();

            // initialize theme
            accentColorService.SetAccentColor(Colors.Orange);

            // add loggers
            serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();
            var catelListener = serviceLocator.RegisterTypeAndInstantiate<CatelLogListener>();
            LogManager.AddListener(catelListener);

            // add upgrade listener
            serviceLocator.RegisterTypeAndInstantiate<ExampleUpgradeListener>();

            // add project extensions
            projectLocator.Register<ExampleFolderPackageManagement>(DefaultNuGetFolders.GetApplicationRoamingFolder());

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
