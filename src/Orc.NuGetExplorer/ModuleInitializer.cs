using Catel.IoC;
using Orc.NuGetExplorer;
using NuGet;

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

        serviceLocator.RegisterType<IPackageQueryService, PackageQueryService>();
        serviceLocator.RegisterType<IPackageCacheService, PackageCacheService>();
        serviceLocator.RegisterType<IRepositoryNavigationFactory, RepositoryNavigationFactory>();
        serviceLocator.RegisterType<IPackageRepositoryService, PackageRepositoryService>();
        serviceLocator.RegisterType<IRepositoryNavigationService, RepositoryNavigationService>();
        serviceLocator.RegisterType<INuGetConfigurationService, NuGetConfigurationService>();
        serviceLocator.RegisterType<IPackagesUIService, PackagesUIService>();
        serviceLocator.RegisterType<INuGetPackageManager, NuGetPackageManager>();
        serviceLocator.RegisterType<IPackageDetailsService, PackageDetailsService>();
        serviceLocator.RegisterType<IPagingService, PagingService>();
        serviceLocator.RegisterType<IPackageManagementListeningService, PackageManagementListeningService>();
        serviceLocator.RegisterType<IPackageActionService, PackageActionService>();
        serviceLocator.RegisterType<INuGetFeedVerificationService, NuGetFeedVerificationService>();
        serviceLocator.RegisterType<ISettings, NuGetSettings>();
        serviceLocator.RegisterType<ICredentialProvider, CredentialProvider>();
        serviceLocator.RegisterType<IAutentificationProvider, NullAutentificationProvider>();
        serviceLocator.RegisterType<IPackageSourceProvider, NuGetPackageSourceProvider>();
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();
        serviceLocator.RegisterType<IPackageSourceFactory, PackageSourceFactory>();

        serviceLocator.RegisterInstance<IPackageRepositoryFactory>(PackageRepositoryFactory.Default);
    }
}