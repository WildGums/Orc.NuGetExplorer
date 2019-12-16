namespace Orc.NuGetExplorer.Example.Services
{
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Example.PackageManagement;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Services;

    public class ExampleNuGetExplorerInitializationService : NuGetExplorerInitializationService
    {
        public ExampleNuGetExplorerInitializationService(
            ILanguageService languageService,
            ICredentialProviderLoaderService credentialProviderLoaderService,
            INuGetProjectUpgradeService nuGetProjectUpgradeService,
            IViewModelLocator vmLocator,
            ITypeFactory typeFactory,
            IExtensibleProjectLocator projectLocator,
            IFileDirectoryService fileDirectoryService) : base(languageService, credentialProviderLoaderService, nuGetProjectUpgradeService, vmLocator, typeFactory)
        {
            Argument.IsNotNull(() => projectLocator);
            Argument.IsNotNull(() => fileDirectoryService);

            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterType<INuGetConfigurationResetService, ExampleNuGetConfigurationResetService>();

            //add loggers
            serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();
            var catelListener = serviceLocator.RegisterTypeAndInstantiate<CatelLogListener>();
            LogManager.AddListener(catelListener);

            //add upgrade listener
            serviceLocator.RegisterTypeAndInstantiate<ExampleUpgradeListener>();

            //add project extensions
            projectLocator.Register<ExampleFolderPackageManagement>(fileDirectoryService.GetApplicationRoamingFolder());
        }
    }
}
