namespace Orc.NuGetExplorer
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Credentials;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Cache;
    using Orc.NuGetExplorer.Configuration;
    using Orc.NuGetExplorer.Loggers;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Services;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcNuGetExplorerModule
    {
        public static IServiceCollection AddOrcNuGetExplorer(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<INuGetLogListeningService, NuGetLogListeningService>();
            serviceCollection.TryAddSingleton<ILogger, NuGetLogger>();

            serviceCollection.TryAddSingleton<IFrameworkNameProvider, DefaultFrameworkNameProvider>();
            serviceCollection.TryAddSingleton<IFrameworkCompatibilityProvider, DefaultCompatibilityProvider>();
            serviceCollection.TryAddSingleton<IPackageSourceProvider, NuGetPackageSourceProvider>();
            serviceCollection.TryAddSingleton<ISourceRepositoryProvider, DefaultSourceRepositoryProvider>();
            serviceCollection.TryAddSingleton<INuGetProjectContextProvider, EmptyProjectContextProvider>();
            //serviceCollection.TryAddSingleton<IPackageCoreReader, PackageReaderBase>();

            serviceCollection.TryAddSingleton<IDefaultNuGetFramework, DefaultNuGetFramework>();

            serviceCollection.TryAddSingleton<INuGetProjectConfigurationProvider, PackagesConfigProvider>();

            serviceCollection.TryAddSingleton<ISettings, NuGetSettings>();
            serviceCollection.TryAddSingleton<IDefaultPackageSourcesProvider, EmptyDefaultPackageSourcesProvider>();

            // Services
            serviceCollection.TryAddSingleton<INuGetConfigurationService, NuGetConfigurationService>();

            //serviceCollection.TryAddSingleton<IPackageCacheService, PackageCacheService>();
            serviceCollection.TryAddSingleton<IPackageOperationContextService, PackageOperationContextService>();

            //serviceCollection.TryAddSingleton<IPackageSourceFactory, PackageSourceFactory>();
            serviceCollection.TryAddSingleton<IRollbackPackageOperationService, RollbackPackageOperationService>();
            serviceCollection.TryAddSingleton<IBackupFileSystemService, BackupFileSystemService>();
            serviceCollection.TryAddSingleton<ITemporaryFIleSystemContextService, TemporaryFIleSystemContextService>();
            serviceCollection.TryAddSingleton<IFileSystemService, FileSystemService>();
            //serviceCollection.TryAddSingleton<IPleaseWaitInterruptService, PleaseWaitInterruptService>();
            serviceCollection.TryAddSingleton<ICredentialProvider, WindowsCredentialProvider>();
            serviceCollection.TryAddSingleton<ICredentialProviderLoaderService, CredentialProviderLoaderService>();

            serviceCollection.TryAddSingleton<INuGetFeedVerificationService, NuGetFeedVerificationService>();

            serviceCollection.TryAddSingleton<IPackageOperationNotificationService, PackageOperationNotificationService>();

            serviceCollection.TryAddSingleton<IExtensibleProjectLocator, ExtensibleProjectLocator>();
            serviceCollection.TryAddSingleton<INuGetPackageManager, NuGetProjectPackageManager>();
            serviceCollection.TryAddSingleton<IPackageInstallationService, PackageInstallationService>();

            serviceCollection.TryAddSingleton<IDefaultExtensibleProjectProvider, DefaultExtensibleProjectProvider>();
            serviceCollection.TryAddSingleton<IPackageMetadataProvider, PackageMetadataProvider>();

            serviceCollection.TryAddSingleton<IRepositoryContextService, RepositoryContextService>();
            serviceCollection.TryAddSingleton<IRepositoryService, RepositoryService>();

            //package loaders
            serviceCollection.TryAddSingleton<IPackageLoaderService, PackagesLoaderService>();
            serviceCollection.TryAddKeyedSingleton<IPackageLoaderService, LocalPackagesLoaderService>("Installed");
            serviceCollection.TryAddKeyedSingleton<IPackageLoaderService, UpdatePackagesLoaderService>("Updates");

            serviceCollection.TryAddSingleton<IDefferedPackageLoaderService, DeferredPackageLoaderService>();
            serviceCollection.TryAddSingleton<IPackagesUpdatesSearcherService, UpdatePackagesLoaderService>();

            serviceCollection.TryAddSingleton<INuGetCacheManager, NuGetCacheManager>();
            serviceCollection.TryAddSingleton<IApiPackageRegistry, ApiPackageRegistry>();

            serviceCollection.TryAddSingleton<IPackageQueryService, PackageQueryService>();
            serviceCollection.TryAddSingleton<IPackageOperationService, PackageOperationService>();

            serviceCollection.TryAddSingleton<INuGetProjectUpgradeService, NuGetProjectUpgradeService>();
            serviceCollection.TryAddSingleton<IDownloadingProgressTrackerService, DownloadingProgressTrackerService>();

            // Validation
            serviceCollection.TryAddSingleton<IPackageValidatorProvider, DefaultPackageValidatorProvider>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.NuGetExplorer", "Orc.NuGetExplorer.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.NuGetExplorer", "https://github.com/wildgums/orc.nugetexplorer"));

            return serviceCollection;
        }
    }
}
