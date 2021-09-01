namespace Orc.NuGetExplorer.Example.Services
{
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Example.PackageManagement;
    using Orc.NuGetExplorer.Management;

    public class ExampleNuGetExplorerInitializationService
    {
        public ExampleNuGetExplorerInitializationService(INuGetConfigurationService nuGetConfigurationService, IExtensibleProjectLocator projectLocator)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => projectLocator);

            var serviceLocator = ServiceLocator.Default;

            // Provide a custom configuration reset implementation
            serviceLocator.RegisterType<INuGetConfigurationResetService, ExampleNuGetConfigurationResetService>();

            // add loggers
            serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();
            var catelListener = serviceLocator.RegisterTypeAndInstantiate<CatelLogListener>();
            LogManager.AddListener(catelListener);

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
