namespace Orc.NuGetExplorer.Services
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer.Scenario;

    public class NuGetExplorerInitializationService : INuGetExplorerInitializationService
    {
        private readonly INuGetProjectUpgradeService _nuGetProjectUpgradeService;

        public NuGetExplorerInitializationService(ILanguageService languageService, ICredentialProviderLoaderService credentialProviderLoaderService,
            INuGetProjectUpgradeService nuGetProjectUpgradeService, IViewModelLocator vmLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => languageService);
            Argument.IsNotNull(() => credentialProviderLoaderService);
            Argument.IsNotNull(() => nuGetProjectUpgradeService);

            var serviceLocator = ServiceLocator.Default;

            //instantiate watchers
            serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
            serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();

            //instantiate package manager listener
            serviceLocator.RegisterTypeAndInstantiate<NuGetToCatelLogTranslator>();

            //set language resources
            languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer", "Orc.NuGetExplorer.Properties", "Resources"));
            languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer.Xaml", "Orc.NuGetExplorer.Properties", "Resources"));

            //run upgrade
            //pre-initialization to prepare old data to new NuGetExplorer
            var basicV3Scenario = typeFactory.CreateInstanceWithParametersAndAutoCompletion<V3RestorePackageConfigAndReinstall>();
            nuGetProjectUpgradeService.AddUpgradeScenario(basicV3Scenario);

            _nuGetProjectUpgradeService = nuGetProjectUpgradeService;
        }

        public string DefaultSourceKey => Settings.NuGet.FallbackUrl;

        public virtual async Task<bool> UpgradeNuGetPackagesIfNeededAsync()
        {
            return await _nuGetProjectUpgradeService.CheckCurrentConfigurationAndRunAsync();
        }
    }
}
