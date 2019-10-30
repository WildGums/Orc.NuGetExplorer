using Catel.Configuration;
using Catel.IoC;
using Catel.MVVM;
using NuGet.Credentials;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer;
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

        serviceLocator.RegisterType<IEchoService, EchoService>();
        //serviceLocator.RegisterType<IDefaultPackageSourcesProvider, DefaultPackageSourcesProvider>();

        Catel.Logging.LogManager.IsDebugEnabled = true;
        Catel.Logging.LogManager.AddDebugListener();

        serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();

        serviceLocator.RegisterInstance<IApplicationCacheProvider>(new ApplcationCacheProvider());

        serviceLocator.RegisterType<INuGetProjectContextProvider, NuGetProjectContextProvider>();
        serviceLocator.RegisterType<IRepositoryContextService, RepositoryContextService>();

        serviceLocator.RegisterType<INuGetCacheManager, NuGetCacheManager>();


        //add all project extensions
        var manager = serviceLocator.ResolveType<IExtensibleProjectLocator>();

        var directoryService = serviceLocator.ResolveType<IFileDirectoryService>();

        manager.Register<ExampleFolderPackageManagement>(directoryService.GetApplicationRoamingFolder());
        manager.Register<ExamplePackageManagement>(directoryService.GetApplicationRoamingFolder());
    }
}
