using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Catel.Services.Models;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.ViewModels;
using Orc.NuGetExplorer.Views;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterType<IImageResolveService, ImageResolveService>();  
        serviceLocator.RegisterType<IPackageBatchService, PackageBatchService>();
        serviceLocator.RegisterType<IPackageCommandService, PackageCommandService>();
        serviceLocator.RegisterType<IPackageDetailsService, PackageDetailsService>(); 
        serviceLocator.RegisterType<IPackagesUIService, PackagesUIService>();
        serviceLocator.RegisterType<IPagingService, PagingService>();  
        serviceLocator.RegisterType<IRepositoryNavigatorService, RepositoryNavigatorService>();
        serviceLocator.RegisterType<ISearchSettingsService, SearchSettingsService>();
        serviceLocator.RegisterType<ISearchResultService, SearchResultService>();
        serviceLocator.RegisterType<IPleaseWaitInterruptService, XamlPleaseWaitInterruptService>();        

        serviceLocator.RegisterType<IRepositoryNavigationFactory, RepositoryNavigationFactory>();

        var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
        viewModelLocator.Register(typeof(PackageSourceSettingControl), typeof(PackageSourceSettingViewModel));

        var languageService = serviceLocator.ResolveType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer.Xaml", "Orc.NuGetExplorer.Properties", "Resources"));
    }
}