namespace Orc.NuGetExplorer.Services
{
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer.Configuration;
    using Orc.NuGetExplorer.Scenario;
    using Orc.NuGetExplorer.ViewModels;
    using Orc.NuGetExplorer.Views;

    public class NuGetExplorerInitializationService : INuGetExplorerInitializationService
    {
        public NuGetExplorerInitializationService(ILanguageService languageService, ICredentialProviderLoaderService credentialProviderLoaderService,
            IViewModelLocator vmLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => languageService);
            Argument.IsNotNull(() => credentialProviderLoaderService);

            var serviceLocator = ServiceLocator.Default;

            AccentColorHelper.CreateAccentColorResourceDictionary();

            //instantiate watchers
            serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
            serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();

            //instantiate package manager listener
            serviceLocator.RegisterTypeAndInstantiate<NuGetToCatelLogTranslator>();

            //set language resources
            languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer", "Orc.NuGetExplorer.Properties", "Resources"));
            languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer.Xaml", "Orc.NuGetExplorer.Properties", "Resources"));

            //run upgrade
            //pre-initialization to prepare old data to new NuGetExplorer versions
            var upgradeRunner = serviceLocator.RegisterTypeAndInstantiate<RunScenarioConfigurationVersionChecker>();
            var basicV3Scenario = typeFactory.CreateInstanceWithParametersAndAutoCompletion<V3RestorePackageConfigAndReinstall>();
            upgradeRunner.AddUpgradeScenario(basicV3Scenario);
            upgradeRunner.Check();
        }
    }
}
