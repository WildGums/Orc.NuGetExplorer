using Catel.IoC;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Example;
using Orc.NuGetExplorer.Example.Services;
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

        serviceLocator.RegisterType<INuGetExplorerInitializationService, ExampleNuGetExplorerInitializationService>();
    }
}
