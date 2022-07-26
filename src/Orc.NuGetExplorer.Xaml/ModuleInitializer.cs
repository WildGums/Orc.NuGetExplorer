using System.ComponentModel;
using Catel;
using Catel.IoC;
using Catel.MVVM;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Logging;
using Orc.NuGetExplorer.Models;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;
using Orc.NuGetExplorer.ViewModels;
using Orc.NuGetExplorer.Views;
using Orc.NuGetExplorer.Windows;

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

        serviceLocator.RegisterType<IApplicationCacheProvider, ExplorerCacheProvider>();
        serviceLocator.RegisterType<INuGetProjectContextProvider, NuGetProjectContextProvider>();

        serviceLocator.RegisterType<ISynchronizeInvoke, SynchronizeInvoker>();
        serviceLocator.RegisterType<IPackageMetadataMediaDownloadService, PackageMetadataMediaDownloadService>();
        serviceLocator.RegisterType<IImageResolveService, PackageMetadataMediaDownloadService>();
        serviceLocator.RegisterType<IPackageCommandService, PackageCommandService>();
        serviceLocator.RegisterType<IPackagesUIService, PackagesUIService>();
        serviceLocator.RegisterType<IPleaseWaitInterruptService, XamlPleaseWaitInterruptService>();
        serviceLocator.RegisterType<IMessageDialogService, MessageDialogService>();
        serviceLocator.RegisterType<ISynchronousUiVisualizer, SynchronousUIVisualizerService>();
        serviceLocator.RegisterType<IAnimationService, AnimationService>();
        serviceLocator.RegisterType<IProgressManager, ProgressManager>();

        serviceLocator.RegisterType<IModelProvider<NuGetFeed>, ModelProvider<NuGetFeed>>();
        serviceLocator.RegisterType<IModelProvider<ExplorerSettingsContainer>, ExplorerSettingsContainerModelProvider>();

        serviceLocator.RegisterType<INuGetExplorerInitializationService, NuGetExplorerInitializationService>();

        serviceLocator.RegisterType<NuGetLogListener>();

        var vmLocator = serviceLocator.ResolveType<IViewModelLocator>();

        // register some view models
        vmLocator.Register<PackageSourceSettingControl, PackageSourceSettingViewModel>();
    }
}
