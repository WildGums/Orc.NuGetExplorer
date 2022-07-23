namespace Orc.NuGetExplorer.Example.Services
{
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Example.PackageManagement;
    using Orc.NuGetExplorer.Example.Providers;
    using Orc.NuGetExplorer.Management;

    public class ExampleNuGetExplorerInitializationService
    {
        public ExampleNuGetExplorerInitializationService(INuGetConfigurationService nuGetConfigurationService, IExtensibleProjectLocator projectLocator)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => projectLocator);

            var serviceLocator = ServiceLocator.Default;

            // Example: override default project
            serviceLocator.RegisterType<IDefaultAppPackagesProjectProvider, NuGetProjectProvider>();

            serviceLocator.RegisterType<INuGetConfigurationResetService, ExampleNuGetConfigurationResetService>();

            // add loggers
            serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();
            var catelListener = serviceLocator.RegisterTypeAndInstantiate<CatelLogListener>();
            LogManager.AddListener(catelListener);

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
