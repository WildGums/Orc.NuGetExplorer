namespace Orc.NuGetExplorer.Services
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;

    public class NuGetExplorerInitializationService : INuGetExplorerInitializationService
    {
        private readonly INuGetProjectUpgradeService _nuGetProjectUpgradeService;
        private readonly INuGetConfigurationService _nuGetConfigurationService;

        public NuGetExplorerInitializationService(ILanguageService languageService, ICredentialProviderLoaderService credentialProviderLoaderService,
            INuGetProjectUpgradeService nuGetProjectUpgradeService, INuGetConfigurationService nuGetConfigurationService, IViewModelLocator vmLocator, ITypeFactory typeFactory)
        {
            InitializeTypes(ServiceLocator.Default);

            //set language resources
            languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer", "Orc.NuGetExplorer.Properties", "Resources"));
            languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer.Xaml", "Orc.NuGetExplorer.Properties", "Resources"));

            // Node: here you can add any prerequisites if you need to do some operations with installed packages before starting NugetExplorer
            // nuGetProjectUpgradeService.AddUpgradeScenario(basicV3Scenario);

            _nuGetProjectUpgradeService = nuGetProjectUpgradeService;
            _nuGetConfigurationService = nuGetConfigurationService;
        }

        private void InitializeTypes(IServiceLocator serviceLocator)
        {
            // instantiate watchers
            serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
            serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();

            // instantiate package manager listener
            serviceLocator.RegisterTypeAndInstantiate<NuGetToCatelLogTranslator>();

            // register commands
            var commandManager = serviceLocator.ResolveRequiredType<ICommandManager>();
            commandManager.CreateCommandWithGesture(typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));
        }

        public string DefaultSourceKey => Settings.NuGet.FallbackUrl;

        public int PackageQuerySize
        {
            get { return _nuGetConfigurationService.GetPackageQuerySize(); }
            set
            {
                _nuGetConfigurationService.SetPackageQuerySize(value);
            }
        }

        public virtual async Task<bool> UpgradeNuGetPackagesIfNeededAsync()
        {
            return await _nuGetProjectUpgradeService.CheckCurrentConfigurationAndRunAsync();
        }
    }
}
