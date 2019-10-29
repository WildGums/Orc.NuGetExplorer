using Catel.Configuration;
using Catel.IoC;
using Catel.Services;
using NuGet.Configuration;
using NuGet.Credentials;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Configuration;
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


        serviceLocator.RegisterType<IConfigurationService, NugetConfigurationService>();
        serviceLocator.RegisterType<ISettings, ExplorerSettings>();
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();
        serviceLocator.RegisterType<IPackageSourceProvider, NuGetPackageSourceProvider>();

        // Services
        serviceLocator.RegisterType<INuGetConfigurationService, NugetConfigurationService>();
        //serviceLocator.RegisterType<INuGetFeedVerificationService, NuGetFeedVerificationService>();
        serviceLocator.RegisterType<INuGetLogListeningSevice, NuGetLogListeningSevice>();
        //serviceLocator.RegisterType<IPackageCacheService, PackageCacheService>();
        serviceLocator.RegisterType<IPackageOperationContextService, PackageOperationContextService>();
        //serviceLocator.RegisterType<IPackageOperationService, PackageOperationService>();
        //serviceLocator.RegisterType<IPackageQueryService, PackageQueryService>();
        //serviceLocator.RegisterType<IRepositoryService, RepositoryService>();
        //serviceLocator.RegisterType<IPackageSourceFactory, PackageSourceFactory>();
        //serviceLocator.RegisterType<IPackagesUpdatesSearcherService, PackagesUpdatesSearcherService>();
        serviceLocator.RegisterType<IRollbackPackageOperationService, RollbackPackageOperationService>();
        serviceLocator.RegisterType<IBackupFileSystemService, BackupFileSystemService>();
        serviceLocator.RegisterType<ITemporaryFIleSystemContextService, TemporaryFIleSystemContextService>();
        serviceLocator.RegisterType<IFileSystemService, FileSystemService>();
        //serviceLocator.RegisterType<IPleaseWaitInterruptService, PleaseWaitInterruptService>();
        serviceLocator.RegisterType<ICredentialProvider, WindowsCredentialProvider>();
        serviceLocator.RegisterType<ICredentialProviderLoaderService, CredentialProviderLoaderService>();
        serviceLocator.RegisterType<INuGetFeedVerificationService, NuGetFeedVerificationService>();

        serviceLocator.RegisterType<IAuthenticationProvider, AuthenticationProvider>();
        serviceLocator.RegisterType<IPackageOperationNotificationService, DummyPackageOperationNotificationService>();

        //var nuGetPackageManager = serviceLocator.ResolveType<IPackageManager>();
        //serviceLocator.RegisterInstance(typeof(IPackageOperationNotificationService), nuGetPackageManager);

        var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

        serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
        serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();
        serviceLocator.RegisterTypeAndInstantiate<NuGetToCatelLogTranslator>();

        var languageService = serviceLocator.ResolveType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer", "Orc.NuGetExplorer.Properties", "Resources"));

        //serviceLocator.RegisterType<IApiPackageRegistry, ApiPackageRegistry>();
    }
}
