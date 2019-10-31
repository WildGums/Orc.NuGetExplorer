using Catel.IoC;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Cache;
using Orc.NuGetExplorer.Example;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;

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
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, DefaultPackageSourcesProvider>();

        Catel.Logging.LogManager.IsDebugEnabled = true;
        Catel.Logging.LogManager.AddDebugListener();

        serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();
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
