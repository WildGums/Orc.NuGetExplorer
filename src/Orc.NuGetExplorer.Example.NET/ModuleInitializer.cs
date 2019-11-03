using Catel.IoC;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Example;
using Orc.NuGetExplorer.Management;
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

        serviceLocator.RegisterTypeAndInstantiate<SimpleLogListener>();

        //add all project extensions
        var manager = serviceLocator.ResolveType<IExtensibleProjectLocator>();

        var directoryService = serviceLocator.ResolveType<IFileDirectoryService>();

        manager.Register<ExampleFolderPackageManagement>(directoryService.GetApplicationRoamingFolder());
        manager.Register<ExamplePackageManagement>(directoryService.GetApplicationRoamingFolder());
    }
}
