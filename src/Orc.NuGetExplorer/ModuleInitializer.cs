using Catel.Configuration;
using Catel.IoC;
using Catel.Services;
using NuGet.Configuration;
using NuGet.Credentials;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Configuration;
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


        serviceLocator.RegisterType<IConfigurationService, NugetConfigurationService>();
        serviceLocator.RegisterType<ISettings, ExplorerSettings>();
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();
        serviceLocator.RegisterType<IPackageSourceProvider, NuGetPackageSourceProvider>();

        // Services
        serviceLocator.RegisterType<INuGetConfigurationService, NugetConfigurationService>();
        serviceLocator.RegisterType<INuGetLogListeningSevice, NuGetLogListeningSevice>();
        //serviceLocator.RegisterType<IPackageCacheService, PackageCacheService>();
        serviceLocator.RegisterType<IPackageOperationContextService, PackageOperationContextService>();
        //serviceLocator.RegisterType<IPackageOperationService, PackageOperationService>();
        //serviceLocator.RegisterType<IPackageQueryService, PackageQueryService>();
        //serviceLocator.RegisterType<IPackageSourceFactory, PackageSourceFactory>();
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

        serviceLocator.RegisterType<IExtensibleProjectLocator, ExtensibleProjectLocator>();
        serviceLocator.RegisterType<INuGetExtensibleProjectManager, NuGetExtensibleProjectManager>();

        serviceLocator.RegisterType<IRepositoryContextService, RepositoryContextService>();
        serviceLocator.RegisterType<IRepositoryService, RepositoryService>();

        //package loaders
        serviceLocator.RegisterType<IPackagesLoaderService, PackagesLoaderService>();
        //todo use separate providers instead of tags
        serviceLocator.RegisterTypeWithTag<IPackagesLoaderService, LocalPackagesLoaderService>("Installed");
        serviceLocator.RegisterTypeWithTag<IPackagesLoaderService, UpdatePackagesLoaderService>("Updates");

        serviceLocator.RegisterType<IDefferedPackageLoaderService, DefferedPackageLoaderService>();
        serviceLocator.RegisterType<IPackagesUpdatesSearcherService, UpdatePackagesLoaderService>();

        var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

        serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
        serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();
        serviceLocator.RegisterTypeAndInstantiate<NuGetToCatelLogTranslator>();

        var languageService = serviceLocator.ResolveType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.NuGetExplorer", "Orc.NuGetExplorer.Properties", "Resources"));

        //serviceLocator.RegisterType<IApiPackageRegistry, ApiPackageRegistry>();
    }
}
