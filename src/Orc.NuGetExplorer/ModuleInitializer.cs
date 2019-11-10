using Catel.Configuration;
using Catel.IoC;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Credentials;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Cache;
using Orc.NuGetExplorer.Configuration;
using Orc.NuGetExplorer.Loggers;
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

        serviceLocator.RegisterType<INuGetLogListeningSevice, NuGetLogListeningSevice>();
        serviceLocator.RegisterType<ILogger, NuGetLogger>();

        serviceLocator.RegisterType<IFrameworkNameProvider, DefaultFrameworkNameProvider>();
        serviceLocator.RegisterType<IFrameworkCompatibilityProvider, DefaultCompatibilityProvider>();
        serviceLocator.RegisterType<IPackageSourceProvider, NuGetPackageSourceProvider>();
        serviceLocator.RegisterType<ISourceRepositoryProvider, DefaultSourceRepositoryProvider>();
        serviceLocator.RegisterType<INuGetProjectContextProvider, EmptyProjectContextProvider>();
        serviceLocator.RegisterType<IPackageCoreReader, PackageReaderBase>();

        serviceLocator.RegisterType<IDefaultNuGetFramework, DefaultNuGetFramework>();
        serviceLocator.RegisterType<IExtendedSourceRepositoryProvider, DefaultSourceRepositoryProvider>();

        serviceLocator.RegisterType<INuGetProjectConfigurationProvider, PackagesConfigProvider>();

        serviceLocator.RegisterType<IConfigurationService, NugetConfigurationService>();
        serviceLocator.RegisterType<ISettings, NuGetSettings>();
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();

        // Services
        serviceLocator.RegisterType<INuGetConfigurationService, NugetConfigurationService>();

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
        serviceLocator.RegisterType<INuGetPackageManager, NuGetProjectPackageManager>();
        serviceLocator.RegisterType<IFileDirectoryService, FileDirectoryService>();
        serviceLocator.RegisterType<IPackageInstallationService, PackageInstallationService>();

        serviceLocator.RegisterType<IDefaultExtensibleProjectProvider, DefaultExtensibleProjectProvider>();

        serviceLocator.RegisterType<IRepositoryContextService, RepositoryContextService>();
        serviceLocator.RegisterType<IRepositoryService, RepositoryService>();

        //package loaders
        serviceLocator.RegisterType<IPackagesLoaderService, PackagesLoaderService>();
        serviceLocator.RegisterTypeWithTag<IPackagesLoaderService, LocalPackagesLoaderService>("Installed");
        serviceLocator.RegisterTypeWithTag<IPackagesLoaderService, UpdatePackagesLoaderService>("Updates");

        serviceLocator.RegisterType<IDefferedPackageLoaderService, DefferedPackageLoaderService>();
        serviceLocator.RegisterType<IPackagesUpdatesSearcherService, UpdatePackagesLoaderService>();

        serviceLocator.RegisterType<INuGetCacheManager, NuGetCacheManager>();
        serviceLocator.RegisterType<IApiPackageRegistry, ApiPackageRegistry>();
    }
}
