using Catel.IoC;
using Catel.Logging;
using Catel.Services;
using Orc.NuGetExplorer;
using NuGet;
using IPackageManager = Orc.NuGetExplorer.IPackageManager;
using PackageManager = Orc.NuGetExplorer.PackageManager;

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

        // Services        
        serviceLocator.RegisterType<IAuthenticationSilencerService, AuthenticationSilencerService>();                    
        serviceLocator.RegisterType<INuGetConfigurationService, NuGetConfigurationService>();
        serviceLocator.RegisterType<INuGetFeedVerificationService, NuGetFeedVerificationService>();
        serviceLocator.RegisterType<INuGetLogListeningSevice, NuGetLogListeningSevice>();        
        serviceLocator.RegisterType<IPackageCacheService, PackageCacheService>();                       
        serviceLocator.RegisterType<IPackageOperationContextService, PackageOperationContextService>();
        serviceLocator.RegisterType<IPackageOperationService, PackageOperationService>();        
        serviceLocator.RegisterType<IPackageQueryService, PackageQueryService>();
        serviceLocator.RegisterType<IPackageRepositoryService, PackageRepositoryService>();
        serviceLocator.RegisterType<IPackageSourceFactory, PackageSourceFactory>();                
        serviceLocator.RegisterType<IPackagesUpdatesSearcherService, PackagesUpdatesSearcherService>();
        serviceLocator.RegisterType<IRepositoryCacheService, RepositoryCacheService>();  
        serviceLocator.RegisterType<IRollbackPackageOperationService, RollbackPackageOperationService>();
        serviceLocator.RegisterType<IBackupFileSystemService, BackupFileSystemService>();
        serviceLocator.RegisterType<ITemporaryFIleSystemContextService, TemporaryFIleSystemContextService>();
        serviceLocator.RegisterType<IFIleSystemService, FIleSystemService>();        

        serviceLocator.RegisterType<ILogger, NuGetLogger>();

        serviceLocator.RegisterType<IPackageManager, PackageManager>();
        
        serviceLocator.RegisterInstance<IPackageRepositoryFactory>(PackageRepositoryFactory.Default);

        serviceLocator.RegisterType<IAuthenticationProvider, AuthenticationProvider>();
        serviceLocator.RegisterType<IPackageSourceProvider, NuGetPackageSourceProvider>();
        serviceLocator.RegisterType<ICredentialProvider, CredentialProvider>();
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();

        serviceLocator.RegisterType<ISettings, NuGetSettings>();
       
        var nuGetPackageManager = serviceLocator.ResolveType<IPackageManager>();
        serviceLocator.RegisterInstance(typeof(IPackageOperationNotificationService), nuGetPackageManager);

/*        Log.Debug("Forcing the loading of assembly Catel by the following types");
        Log.Debug("  * {0}", typeof(DispatcherService).Name);*/

        var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
        HttpClient.DefaultCredentialProvider = typeFactory.CreateInstance<NuGetSettingsCredentialProvider>();        

        serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
        serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();
        serviceLocator.RegisterTypeAndInstantiate<NuGetToCatelLogTranstalor>();
    }
}