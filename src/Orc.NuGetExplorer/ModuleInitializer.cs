using Catel.IoC;
using Orc.NuGetExplorer;

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

        serviceLocator.RegisterType<IPackageSourceService, PackageSourceService>();
        serviceLocator.RegisterType<IPackageQueryService, PackageQueryService>();
        serviceLocator.RegisterType<IPackageCacheService, PackageCacheService>();
        serviceLocator.RegisterType<IRepoNavigationFactory, RepoNavigationFactory>();
        serviceLocator.RegisterType<IPackagesManager, PackagesManager>();
        serviceLocator.RegisterType<IPackageRepositoryService, PackageRepositoryService>();
    }
}