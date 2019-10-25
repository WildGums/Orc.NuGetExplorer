using Catel.Configuration;
using Catel.IoC;
using Catel.MVVM;
using NuGet.Credentials;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer.Cache;
using Orc.NuGetExplorer.Example;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Models;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;
using Orc.NuGetExplorer.ViewModels;
using Orc.NuGetExplorer.Views;
using Orc.NuGetExplorer.Windows;
using Orc.NuGetExplorer.Windows.Service;
using SourceRepositoryProvider = Orc.NuGetExplorer.Providers.SourceRepositoryProvider;

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

        var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
        viewModelLocator.Register<PackageDetailsView, PackageDetailsViewModel>();

        serviceLocator.RegisterType<IEchoService, EchoService>();
       // serviceLocator.RegisterType<IDefaultPackageSourcesProvider, DefaultPackageSourcesProvider>();

        serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();

        serviceLocator.RegisterType<IConfigurationService, NugetConfigurationService>();
        serviceLocator.RegisterType<IModelProvider<NuGetFeed>, ModelProvider<NuGetFeed>>();

        serviceLocator.RegisterType<IModelProvider<ExplorerSettingsContainer>, ExplorerSettingsContainerModelProvider>();

        serviceLocator.RegisterType<INuGetFeedVerificationService, NuGetFeedVerificationService>();

        serviceLocator.RegisterType<ICredentialProvider, WindowsCredentialProvider>();
        serviceLocator.RegisterType<ICredentialProviderLoaderService, CredentialProviderLoaderService>();

        serviceLocator.RegisterType<IPackageInstallationService, PackageInstallationService>();

        var appCache = new ApplcationCacheProvider();

        serviceLocator.RegisterInstance<IApplicationCacheProvider>(appCache);
        serviceLocator.RegisterType<IPackageMetadataMediaDownloadService, PackageMetadataMediaDownloadService>();

        serviceLocator.RegisterType<ISourceRepositoryProvider, SourceRepositoryProvider>();
        serviceLocator.RegisterType<INuGetProjectContextProvider, NuGetProjectContextProvider>();

        serviceLocator.RegisterType<IRepositoryService, RepositoryService>();

        serviceLocator.RegisterType<IExtensibleProjectLocator, ExtensibleProjectLocator>();
        serviceLocator.RegisterType<INuGetExtensibleProjectManager, NuGetExtensibleProjectManager>();

        serviceLocator.RegisterType<IFrameworkNameProvider, DefaultFrameworkNameProvider>();
        serviceLocator.RegisterType<IFrameworkCompatibilityProvider, DefaultCompatibilityProvider>();

        serviceLocator.RegisterType<IPackageCoreReader, PackageReaderBase>();

        serviceLocator.RegisterType<ISynchronousUiVisualizer, SynchronousUIVisualizerService>();
        serviceLocator.RegisterType<IMessageDialogService, MessageDialogService>();

        serviceLocator.RegisterType<IFileDirectoryService, FileDirectoryService>();

        serviceLocator.RegisterType<INuGetCacheManager, NuGetCacheManager>();

        serviceLocator.RegisterType<IAnimationService, AnimationService>();

        serviceLocator.RegisterType<IProgressManager, ProgressManager>();

        //package loaders
        serviceLocator.RegisterType<IPackagesLoaderService, PackagesLoaderService>();
        //todo use separate providers instead of tags
        serviceLocator.RegisterTypeWithTag<IPackagesLoaderService, LocalPackagesLoaderService>("Installed");
        serviceLocator.RegisterTypeWithTag<IPackagesLoaderService, UpdatePackagesLoaderService>("Updates");

        serviceLocator.RegisterType<IDefferedPackageLoaderService, DefferedPackageLoaderService>();

        //add all project extensions

        var manager = serviceLocator.ResolveType<IExtensibleProjectLocator>();

        var directoryService = serviceLocator.ResolveType<IFileDirectoryService>();

        manager.Register<ExampleFolderPackageManagement>(directoryService.GetApplicationRoamingFolder());
        manager.Register<ExamplePackageManagement>(directoryService.GetApplicationRoamingFolder());
    }
}
