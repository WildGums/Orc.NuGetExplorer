namespace Orc;

using System.ComponentModel;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Logging;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;
using Orc.NuGetExplorer.ViewModels;
using Orc.NuGetExplorer.Views;
using Orc.NuGetExplorer.Windows;

/// <summary>
/// Core module which allows the registration of default services in the service collection.
/// </summary>
public static class OrcNuGetExplorerXamlModule
{
    public static IServiceCollection AddOrcNuGetExplorerXaml(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IApplicationCacheProvider, ExplorerCacheProvider>();
        serviceCollection.TryAddSingleton<INuGetProjectContextProvider, NuGetProjectContextProvider>();

        serviceCollection.TryAddSingleton<ISynchronizeInvoke, SynchronizeInvoker>();
        serviceCollection.TryAddSingleton<IPackageMetadataMediaDownloadService, PackageMetadataMediaDownloadService>();
        serviceCollection.TryAddSingleton<IImageResolveService, PackageMetadataMediaDownloadService>();
        serviceCollection.TryAddSingleton<IPackageCommandService, PackageCommandService>();
        serviceCollection.TryAddSingleton<IPackagesUIService, PackagesUIService>();
        serviceCollection.TryAddSingleton<IBusyIndicatorInterruptService, XamlBusyIndicatorInterruptService>();
        serviceCollection.TryAddSingleton<ISynchronousUiVisualizer, SynchronousUIVisualizerService>();
        serviceCollection.TryAddSingleton<IAnimationService, AnimationService>();
        serviceCollection.TryAddSingleton<IProgressManager, ProgressManager>();

        serviceCollection.TryAddSingleton<IModelProvider<NuGetFeed>, ModelProvider<NuGetFeed>>();
        serviceCollection.TryAddSingleton<IModelProvider<ExplorerSettingsContainer>, ExplorerSettingsContainerModelProvider>();

        serviceCollection.TryAddSingleton<INuGetExplorerInitializationService, NuGetExplorerInitializationService>();

        serviceCollection.TryAddSingleton<NuGetLogListener>();
        serviceCollection.TryAddSingleton<ViewModelLocatorInitializer>();

        serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.NuGetExplorer.Xaml", "Orc.NuGetExplorer.Properties", "Resources"));

        return serviceCollection;
    }

    private class ViewModelLocatorInitializer : IConstructAtStartup
    {
        public ViewModelLocatorInitializer(IViewModelLocator viewModelLocator)
        {
            viewModelLocator.Register<PackageSourceSettingControl, PackageSourceSettingViewModel>();
        }
    }
}
