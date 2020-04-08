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

        serviceLocator.RegisterType<INuGetProjectConfigurationProvider, PackagesConfigProvider>();

        serviceLocator.RegisterType<ISettings, NuGetSettings>();
        serviceLocator.RegisterType<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();

        // Services
        serviceLocator.RegisterType<INuGetConfigurationService, NuGetConfigurationService>();

        //serviceLocator.RegisterType<IPackageCacheService, PackageCacheService>();
        serviceLocator.RegisterType<IPackageOperationContextService, PackageOperationContextService>();

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
        serviceLocator.RegisterType<IPackageOperationNotificationService, PackageOperationNotificationService>();

        serviceLocator.RegisterType<IExtensibleProjectLocator, ExtensibleProjectLocator>();
        serviceLocator.RegisterType<INuGetPackageManager, NuGetProjectPackageManager>();
        serviceLocator.RegisterType<IFileDirectoryService, FileDirectoryService>();
        serviceLocator.RegisterType<IPackageInstallationService, PackageInstallationService>();

        serviceLocator.RegisterType<IDefaultExtensibleProjectProvider, DefaultExtensibleProjectProvider>();
        serviceLocator.RegisterType<IPackageMetadataProvider, PackageMetadataProvider>();

        serviceLocator.RegisterType<IRepositoryContextService, RepositoryContextService>();
        serviceLocator.RegisterType<IRepositoryService, RepositoryService>();

        //package loaders
        serviceLocator.RegisterType<IPackageLoaderService, PackagesLoaderService>();
        serviceLocator.RegisterTypeWithTag<IPackageLoaderService, LocalPackagesLoaderService>("Installed");
        serviceLocator.RegisterTypeWithTag<IPackageLoaderService, UpdatePackagesLoaderService>("Updates");

        serviceLocator.RegisterType<IDefferedPackageLoaderService, DefferedPackageLoaderService>();
        serviceLocator.RegisterType<IPackagesUpdatesSearcherService, UpdatePackagesLoaderService>();

        serviceLocator.RegisterType<INuGetCacheManager, NuGetCacheManager>();
        serviceLocator.RegisterType<IApiPackageRegistry, ApiPackageRegistry>();

        serviceLocator.RegisterType<IPackageQueryService, PackageQueryService>();
        serviceLocator.RegisterType<IPackageOperationService, PackageOperationService>();

        serviceLocator.RegisterType<INuGetProjectUpgradeService, NuGetProjectUpgradeService>();
    }
}
