namespace Orc.NuGetExplorer.Services;

using System;
using System.Threading.Tasks;
using Catel;
using Catel.MVVM;
using Catel.Services;

public class NuGetExplorerInitializationService : INuGetExplorerInitializationService
{
    private readonly INuGetProjectUpgradeService _nuGetProjectUpgradeService;
    private readonly INuGetConfigurationService _nuGetConfigurationService;
    private readonly ICommandManager _commandManager;

    public NuGetExplorerInitializationService(ILanguageService languageService, 
        ICredentialProviderLoaderService credentialProviderLoaderService,
        INuGetProjectUpgradeService nuGetProjectUpgradeService, INuGetConfigurationService nuGetConfigurationService, 
        IViewModelLocator vmLocator, IServiceProvider serviceProvider,
        ICommandManager commandManager)
    {
        InitializeTypes(serviceProvider);

        // Note: here you can add any prerequisites if you need to do some operations with installed packages before starting NugetExplorer
        // nuGetProjectUpgradeService.AddUpgradeScenario(basicV3Scenario);

        _nuGetProjectUpgradeService = nuGetProjectUpgradeService;
        _nuGetConfigurationService = nuGetConfigurationService;
        _commandManager = commandManager;
    }

    private void InitializeTypes(IServiceProvider serviceProvider)
    {
        // register commands
        _commandManager.CreateCommandWithGesture(serviceProvider, typeof(Commands.Packages), nameof(Commands.Packages.BatchUpdate));
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
