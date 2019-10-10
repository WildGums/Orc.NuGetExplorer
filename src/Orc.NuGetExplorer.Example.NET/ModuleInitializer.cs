using Catel.IoC;
using Orc.old_NuGetExplorer;
using Orc.old_NuGetExplorer.Example;

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
    }
}