[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Orc.NuGetExplorer.Tests")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v5.0", FrameworkDisplayName="")]
public static class LoadAssembliesOnStartup { }
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.NuGetExplorer
{
    [System.Serializable]
    public class ApiValidationException : System.Exception
    {
        public ApiValidationException() { }
        public ApiValidationException(string message) { }
        protected ApiValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public ApiValidationException(string message, System.Exception innerException) { }
    }
    public static class Constants
    {
        public const string CombinedSourceName = "All";
        public const string CompanyName = "WildGums";
        public const string ConfigKeySeparator = "|";
        public const string DefaultNuGetOrgName = "nuget.org";
        public const string DefaultNuGetOrgUri = "https://api.nuget.org/v3/index.json";
        public const string NotInstalled = "not installed";
        public const string PackageInstallationConflictMessage = "Conflict during package installation";
        public const string PackageManagement = "Package management";
        public const string ProductName = "NuGetPackageManager";
        public static class Log
        {
            public const string CacheHttpRequestInfoPattern = "  CACHE https";
            public const string GetHttpRequestInfoPattern = "  GET https";
            public const string NotFoundHttpRequestInfoPattern = "  NotFound https";
            public const string OkHttpRequestInfoPattern = "  OK https";
            public const string SetHttpRequestInfoPattern = "  SET https";
        }
        public static class Messages
        {
            public const string CacheClearEndedSuccessful = "NuGet cache cleared";
            public const string CacheClearFailed = "Fatal error during cache clearing";
            public const string CachedClearEndedWithError = "NuGet cache cleared with some errors. It means some cached files may kept on cach" +
                "e folder";
            public const string PackageParserInvalidIdentity = "parameter doesn\'t contain valid package identity";
            public const string PackageParserInvalidVersion = "parameter doesn\'t contain valid package version";
        }
    }
    public enum CredentialStoragePolicy
    {
        None = 0,
        WindowsVault = 1,
        WindowsVaultConfigurationFallback = 2,
        Configuration = 3,
    }
    public static class DefaultNuGetComparers
    {
        public static System.Collections.Generic.IEqualityComparer<NuGet.Configuration.PackageSource> PackageSource { get; set; }
        public static System.Collections.Generic.IEqualityComparer<NuGet.Protocol.Core.Types.SourceRepository> SourceRepository { get; set; }
    }
    public static class DefaultNuGetFolders
    {
        public static readonly string DefaultGlobalPackagesFolderPath;
        public static string GetApplicationLocalFolder() { }
        public static string GetApplicationRoamingFolder() { }
        public static string GetGlobalPackagesFolder() { }
    }
    public class DefaultNuGetFramework : Orc.NuGetExplorer.IDefaultNuGetFramework
    {
        public DefaultNuGetFramework(NuGet.Frameworks.IFrameworkNameProvider frameworkNameProvider) { }
        public NuGet.Frameworks.NuGetFramework GetFirst() { }
        public System.Collections.Generic.IEnumerable<NuGet.Frameworks.NuGetFramework> GetHighest() { }
        public System.Collections.Generic.IEnumerable<NuGet.Frameworks.NuGetFramework> GetLowest() { }
    }
    public class DeletemeWatcher : Orc.NuGetExplorer.PackageManagerWatcherBase
    {
        public DeletemeWatcher(Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService, Orc.NuGetExplorer.IFileSystemService fileSystemService, Orc.FileSystem.IDirectoryService directoryService, Orc.NuGetExplorer.Management.INuGetPackageManager nuGetPackageManager, Orc.NuGetExplorer.Management.IDefaultExtensibleProjectProvider projectProvider, Catel.Messaging.IMessageMediator messageMediator) { }
    }
    public class DependencyInfoResourceCollection : System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.DependencyInfoResource>, System.Collections.IEnumerable
    {
        public DependencyInfoResourceCollection(NuGet.Protocol.Core.Types.DependencyInfoResource resource) { }
        public DependencyInfoResourceCollection(System.Collections.Generic.IReadOnlyList<NuGet.Protocol.Core.Types.DependencyInfoResource> resources) { }
        public System.Collections.Generic.IEnumerator<NuGet.Protocol.Core.Types.DependencyInfoResource> GetEnumerator() { }
        public System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo> ResolvePackageAsync(NuGet.Packaging.Core.PackageIdentity package, NuGet.Frameworks.NuGetFramework projectFramework, NuGet.Protocol.Core.Types.SourceCacheContext cacheContext, NuGet.Common.ILogger log, System.Threading.CancellationToken token) { }
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo>> ResolvePackagesAsync(NuGet.Packaging.Core.PackageIdentity package, NuGet.Frameworks.NuGetFramework projectFramework, NuGet.Protocol.Core.Types.SourceCacheContext cacheContext, NuGet.Common.ILogger log, System.Threading.CancellationToken token) { }
    }
    public static class DispatchHelper
    {
        public static System.Threading.Tasks.Task DispatchIfNecessaryAsync(System.Action action) { }
    }
    public static class DownloadResourceResultExtensions
    {
        public static string GetResourceRoot(this NuGet.Protocol.Core.Types.DownloadResourceResult downloadResourceResult) { }
        public static bool IsAvailable(this NuGet.Protocol.Core.Types.DownloadResourceResult downloadResourceResult) { }
    }
    public class EmptyDefaultPackageSourcesProvider : Orc.NuGetExplorer.IDefaultPackageSourcesProvider
    {
        public EmptyDefaultPackageSourcesProvider() { }
        public string DefaultSource { get; set; }
        public System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> GetDefaultPackages() { }
    }
    public class ExplorerCredentialService : NuGet.Configuration.ICredentialService
    {
        public ExplorerCredentialService(NuGet.Common.AsyncLazy<System.Collections.Generic.IEnumerable<NuGet.Credentials.ICredentialProvider>> providers, bool nonInteractive, bool handlesDefaultCredentials) { }
        public bool HandlesDefaultCredentials { get; }
        public void ClearRetryCache() { }
        public System.Threading.Tasks.Task<System.Net.ICredentials> GetCredentialsAsync(System.Uri uri, System.Net.IWebProxy proxy, NuGet.Configuration.CredentialRequestType type, string message, System.Threading.CancellationToken cancellationToken) { }
        public bool IsValidResponse(NuGet.Credentials.CredentialResponse response) { }
        public bool TryGetLastKnownGoodCredentialsFromCache(System.Uri uri, bool isProxy, out System.Net.ICredentials credentials) { }
    }
    public enum FeedVerificationResult
    {
        Unknown = 0,
        Valid = 1,
        AuthenticationRequired = 2,
        AuthorizationRequired = 3,
        Invalid = 4,
    }
    public static class FolderNuGetProjectExtension
    {
        public static System.Collections.Generic.IEnumerable<string> GetPackageDirectories(this NuGet.ProjectManagement.FolderNuGetProject project) { }
    }
    public static class FrameworkParser
    {
        public static NuGet.Frameworks.NuGetFramework ToSpecificPlatform(NuGet.Frameworks.NuGetFramework framework) { }
        public static NuGet.Frameworks.NuGetFramework TryParseFrameworkName(string frameworkString, NuGet.Frameworks.IFrameworkNameProvider frameworkNameProvider) { }
    }
    public static class HttpHandlerResourceV3Extensions
    {
        public static T GetCredentialServiceImplementation<T>(this NuGet.Protocol.HttpHandlerResourceV3 httpResourceHandler)
            where T :  class, NuGet.Configuration.ICredentialService { }
        public static void ResetCredentials(this NuGet.Protocol.HttpHandlerResourceV3 httpResourceHandler) { }
    }
    public interface IApiPackageRegistry
    {
        bool IsRegistered(string packageName);
        void Register(string packageName, string version);
        void Validate(Orc.NuGetExplorer.IPackageDetails package);
    }
    public interface IBackupFileSystemService
    {
        void BackupFile(string filePath);
        void BackupFolder(string fullPath);
        void Restore(string fullPath);
    }
    public interface ICloneable<out T>
    {
        T Clone();
    }
    public static class IConfigurationServiceExtensions
    {
        public static Orc.NuGetExplorer.CredentialStoragePolicy GetCredentialStoragePolicy(this Catel.Configuration.IConfigurationService configurationService) { }
        public static void SetCredentialStoragePolicy(this Catel.Configuration.IConfigurationService configurationService, Orc.NuGetExplorer.CredentialStoragePolicy value) { }
    }
    public interface ICredentialProviderLoaderService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Credentials.ICredentialProvider>> GetCredentialProvidersAsync();
        void SetCredentialPolicy(Orc.NuGetExplorer.CredentialStoragePolicy storagePolicy);
    }
    public interface IDefaultNuGetFramework
    {
        System.Collections.Generic.IEnumerable<NuGet.Frameworks.NuGetFramework> GetHighest();
        System.Collections.Generic.IEnumerable<NuGet.Frameworks.NuGetFramework> GetLowest();
    }
    public interface IDefaultPackageSourcesProvider
    {
        string DefaultSource { get; set; }
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> GetDefaultPackages();
    }
    public static class IDirectoryServiceExtensions
    {
        public static void ForceDeleteDirectory(this Orc.FileSystem.IDirectoryService directoryService, Orc.FileSystem.IFileService fileService, string folderPath, out System.Collections.Generic.List<string> failedEntries) { }
    }
    public interface IExtendedSourceRepositoryProvider : NuGet.Protocol.Core.Types.ISourceRepositoryProvider
    {
        NuGet.Protocol.Core.Types.SourceRepository CreateLocalRepository(string source);
        NuGet.Protocol.Core.Types.SourceRepository CreateRepository(NuGet.Configuration.PackageSource source, bool forceUpdate);
    }
    public interface IExtensibleProject
    {
        string ContentPath { get; }
        string Framework { get; }
        string Name { get; }
        System.Collections.Immutable.ImmutableList<NuGet.Frameworks.NuGetFramework> SupportedPlatforms { get; set; }
        string GetInstallPath(NuGet.Packaging.Core.PackageIdentity packageIdentity);
        void Install();
        void Uninstall();
        void Update();
    }
    public static class IExtensibleProjectExtensions
    {
        public static NuGet.Protocol.Core.Types.SourceRepository AsSourceRepository(this Orc.NuGetExplorer.IExtensibleProject project, NuGet.Protocol.Core.Types.ISourceRepositoryProvider repositoryProvider) { }
    }
    public static class IFileServiceExtensions
    {
        public static void ForceDeleteFiles(this Orc.FileSystem.IFileService fileService, string filePath, System.Collections.Generic.List<string> failedEntries) { }
        public static void SetAttributes(this Orc.FileSystem.IFileService fileService, string filePath, System.IO.FileAttributes attribute) { }
    }
    public interface IFileSystemService
    {
        void CreateDeleteme(string name, string path);
        void RemoveDeleteme(string name, string path);
    }
    public interface INuGetConfigurationService
    {
        void DisablePackageSource(string name, string source);
        string GetDestinationFolder();
        bool GetIsPrereleaseAllowed(Orc.NuGetExplorer.IRepository repository);
        int GetPackageQuerySize();
        bool IsProjectConfigured(Orc.NuGetExplorer.IExtensibleProject project);
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> LoadPackageSources(bool onlyEnabled = false);
        void RemovePackageSource(Orc.NuGetExplorer.IPackageSource source);
        bool SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true, bool verifyFeed = true);
        void SavePackageSources(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> packageSources);
        void SaveProjects(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IExtensibleProject> extensibleProjects);
        void SetDestinationFolder(string value);
        void SetIsPrereleaseAllowed(Orc.NuGetExplorer.IRepository repository, bool value);
        void SetPackageQuerySize(int size);
    }
    public interface INuGetFeedVerificationService
    {
        Orc.NuGetExplorer.FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true);
        System.Threading.Tasks.Task<Orc.NuGetExplorer.FeedVerificationResult> VerifyFeedAsync(string source, bool authenticateIfRequired = true, System.Threading.CancellationToken cancellationToken = default);
    }
    public interface INuGetLogListeningSevice
    {
        event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Debug;
        event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Error;
        event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Info;
        event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Warning;
        void SendDebug(string message);
        void SendError(string message);
        void SendInfo(string message);
        void SendWarning(string message);
    }
    public interface INuGetProjectConfigurationProvider
    {
        NuGet.ProjectManagement.NuGetProject GetProjectConfig(Orc.NuGetExplorer.IExtensibleProject project);
    }
    public interface INuGetProjectContextProvider
    {
        NuGet.ProjectManagement.INuGetProjectContext GetProjectContext(NuGet.ProjectManagement.FileConflictAction fileConflictAction);
    }
    public interface IPackageDetails
    {
        System.Collections.Generic.IEnumerable<string> Authors { get; }
        System.Collections.Generic.IList<string> AvailableVersions { get; }
        string Dependencies { get; }
        string Description { get; }
        int? DownloadCount { get; }
        string FullName { get; }
        System.Uri IconUrl { get; }
        string Id { get; }
        bool IsAbsoluteLatestVersion { get; }
        bool? IsInstalled { get; set; }
        bool IsLatestVersion { get; }
        bool IsPrerelease { get; }
        NuGet.Versioning.NuGetVersion NuGetVersion { get; }
        System.DateTimeOffset? Published { get; }
        string SelectedVersion { get; set; }
        string SpecialVersion { get; }
        string Title { get; }
        Catel.Data.IValidationContext ValidationContext { get; }
        System.Version Version { get; }
        NuGet.Packaging.Core.PackageIdentity GetIdentity();
        void ResetValidationContext();
    }
    public interface IPackageLoaderService
    {
        Orc.NuGetExplorer.Providers.IPackageMetadataProvider PackageMetadataProvider { get; }
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> LoadAsync(string searchTerm, Orc.NuGetExplorer.Pagination.PageContinuation pageContinuation, NuGet.Protocol.Core.Types.SearchFilter searchFilter, System.Threading.CancellationToken token);
    }
    public interface IPackageManager { }
    public interface IPackageOperationContext
    {
        System.Collections.Generic.IList<System.Exception> Exceptions { get; }
        Orc.NuGetExplorer.ITemporaryFileSystemContext FileSystemContext { get; set; }
        Orc.NuGetExplorer.IPackageOperationContext Parent { get; set; }
        Orc.NuGetExplorer.IRepository Repository { get; set; }
    }
    public interface IPackageOperationContextService
    {
        Orc.NuGetExplorer.IPackageOperationContext CurrentContext { get; }
        event System.EventHandler<Orc.NuGetExplorer.OperationContextEventArgs> OperationContextDisposing;
        System.IDisposable UseOperationContext(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
    }
    public interface IPackageOperationNotificationService
    {
        bool MuteAutomaticEvents { get; set; }
        event System.EventHandler<Orc.NuGetExplorer.PackageOperationEventArgs> OperationFinished;
        event System.EventHandler<Orc.NuGetExplorer.PackageOperationEventArgs> OperationStarting;
        event System.EventHandler<Orc.NuGetExplorer.PackageOperationBatchEventArgs> OperationsBatchFinished;
        event System.EventHandler<Orc.NuGetExplorer.PackageOperationBatchEventArgs> OperationsBatchStarting;
        System.IDisposable DisableNotifications();
        void NotifyAutomaticOperationBatchFinished(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
        void NotifyAutomaticOperationBatchStarting(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
        void NotifyAutomaticOperationFinished(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
        void NotifyAutomaticOperationStarting(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
        void NotifyOperationBatchFinished(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
        void NotifyOperationBatchStarting(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
        void NotifyOperationFinished(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
        void NotifyOperationStarting(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
    }
    public interface IPackageOperationService
    {
        System.Threading.Tasks.Task InstallPackageAsync(Orc.NuGetExplorer.IPackageDetails package, bool allowedPrerelease = false, System.Threading.CancellationToken token = default);
        System.Threading.Tasks.Task UninstallPackageAsync(Orc.NuGetExplorer.IPackageDetails package, System.Threading.CancellationToken token = default);
        System.Threading.Tasks.Task UpdatePackagesAsync(Orc.NuGetExplorer.IPackageDetails package, bool allowedPrerelease = false, System.Threading.CancellationToken token = default);
    }
    public interface IPackageQueryService
    {
        System.Threading.Tasks.Task<Orc.NuGetExplorer.IPackageDetails> GetPackageAsync(Orc.NuGetExplorer.IRepository packageRepository, string packageId, string version);
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageDetails>> GetPackagesAsync(Orc.NuGetExplorer.IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10);
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> GetVersionsOfPackageAsync(Orc.NuGetExplorer.IRepository packageRepository, Orc.NuGetExplorer.IPackageDetails package, bool allowPrereleaseVersions, int skip);
        System.Threading.Tasks.Task<bool> PackageExistsAsync(Orc.NuGetExplorer.IRepository packageRepository, Orc.NuGetExplorer.IPackageDetails packageDetails);
        System.Threading.Tasks.Task<bool> PackageExistsAsync(Orc.NuGetExplorer.IRepository packageRepository, string packageId);
        System.Threading.Tasks.Task<bool> PackageExistsAsync(Orc.NuGetExplorer.IRepository packageRepository, string filter, bool allowPrereleaseVersions);
    }
    public interface IPackageSource
    {
        bool IsEnabled { get; }
        bool IsOfficial { get; }
        string Name { get; }
        string Source { get; }
    }
    public interface IPackageSourceFactory
    {
        Orc.NuGetExplorer.IPackageSource CreatePackageSource(string source, string name, bool isEnabled, bool isOfficial);
    }
    public static class IPackagesBatchServiceExtensions { }
    public interface IPackagesUpdatesSearcherService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> SearchForPackagesUpdatesAsync(bool? allowPrerelease = default, bool authenticateIfRequired = true, System.Threading.CancellationToken token = default);
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = default, bool authenticateIfRequired = true, System.Threading.CancellationToken token = default);
    }
    public static class IPackagesUpdatesSearcherServiceExtensions { }
    public interface IPleaseWaitInterruptService
    {
        System.IDisposable InterruptTemporarily();
    }
    public interface IRepository
    {
        bool IsLocal { get; }
        string Name { get; }
        Orc.NuGetExplorer.PackageOperationType OperationType { get; }
        string Source { get; }
    }
    public interface IRepositoryContextService
    {
        Orc.NuGetExplorer.Management.SourceContext AcquireContext(NuGet.Configuration.PackageSource source);
        Orc.NuGetExplorer.Management.SourceContext AcquireContext(bool ignoreLocal = false);
        NuGet.Protocol.Core.Types.SourceRepository GetRepository(NuGet.Configuration.PackageSource source);
    }
    public static class IRepositoryExtensions
    {
        public static NuGet.Configuration.PackageSource ToPackageSource(this Orc.NuGetExplorer.IRepository repository) { }
    }
    public interface IRepositoryService
    {
        Orc.NuGetExplorer.IRepository LocalRepository { get; }
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IRepository> GetRepositories(Orc.NuGetExplorer.PackageOperationType packageOperationType);
        Orc.NuGetExplorer.IRepository GetSourceAggregateRepository();
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IRepository> GetSourceRepositories();
        Orc.NuGetExplorer.IRepository GetUpdateAggeregateRepository();
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IRepository> GetUpdateRepositories();
    }
    public interface IRollbackPackageOperationService
    {
        void ClearRollbackActions(Orc.NuGetExplorer.IPackageOperationContext context);
        void PushRollbackAction(System.Action rollbackAction, Orc.NuGetExplorer.IPackageOperationContext context);
        void Rollback(Orc.NuGetExplorer.IPackageOperationContext context);
    }
    public interface ITemporaryFileSystemContext : System.IDisposable
    {
        string RootDirectory { get; }
        string GetDirectory(string relativeDirectoryName);
        string GetFile(string relativeFilePath);
    }
    public class InstallerResult
    {
        public InstallerResult(System.Collections.Generic.IDictionary<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo, NuGet.Protocol.Core.Types.DownloadResourceResult> downloadResult) { }
        public InstallerResult(string errorMessage) { }
        public string ErrorMessage { get; }
        public System.Collections.Generic.IDictionary<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo, NuGet.Protocol.Core.Types.DownloadResourceResult> Result { get; }
    }
    public class LogHelper
    {
        public LogHelper() { }
        public static void LogUnclearedPaths(System.Collections.Generic.List<string> unclearedPaths, Catel.Logging.ILog log) { }
    }
    public class MultiplySourceSearchResource : NuGet.Protocol.Core.Types.PackageSearchResource
    {
        public override System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> SearchAsync(string searchTerm, NuGet.Protocol.Core.Types.SearchFilter filters, int skip, int take, NuGet.Common.ILogger log, System.Threading.CancellationToken cancellationToken) { }
        public static System.Threading.Tasks.Task<Orc.NuGetExplorer.MultiplySourceSearchResource> CreateAsync(NuGet.Protocol.Core.Types.SourceRepository[] sourceRepositories) { }
    }
    public class NoVerboseHttpNuGetLogListeningService : Orc.NuGetExplorer.INuGetLogListeningSevice
    {
        public NoVerboseHttpNuGetLogListeningService() { }
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Debug;
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Error;
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Info;
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Warning;
        public void SendDebug(string message) { }
        public void SendError(string message) { }
        public void SendInfo(string message) { }
        public void SendWarning(string message) { }
    }
    public class NuGetLogRecordEventArgs : System.EventArgs
    {
        public NuGetLogRecordEventArgs(string message) { }
        public string Message { get; }
    }
    public class NuGetToCatelLogTranslator : Orc.NuGetExplorer.PackageManagerLogListenerBase
    {
        public NuGetToCatelLogTranslator(Orc.NuGetExplorer.INuGetLogListeningSevice nuGetLogListeningSevice) { }
        protected override void OnDebug(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected override void OnError(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected override void OnInfo(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected override void OnWarning(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
    }
    public class OperationContextEventArgs : System.EventArgs
    {
        public OperationContextEventArgs(Orc.NuGetExplorer.IPackageOperationContext packageOperationContext) { }
        public Orc.NuGetExplorer.IPackageOperationContext PackageOperationContext { get; }
    }
    public static class PackageCollectionExtensions
    {
        public static NuGet.Packaging.Core.PackageIdentity[] GetLatest(this Orc.NuGetExplorer.Packaging.PackageCollection packages, NuGet.Versioning.IVersionComparer versionComparer) { }
    }
    public static class PackageIdentityExtensions
    {
        public static string ToFullString(this NuGet.Packaging.Core.PackageIdentity packageIdentity) { }
    }
    public class PackageIdentityParser
    {
        public PackageIdentityParser() { }
        public static NuGet.Packaging.Core.PackageIdentity Parse(string packageString) { }
    }
    public abstract class PackageManagerContextWatcherBase : Orc.NuGetExplorer.PackageManagerWatcherBase
    {
        protected PackageManagerContextWatcherBase(Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService, Orc.NuGetExplorer.IPackageOperationContextService packageOperationContextService) { }
        public Orc.NuGetExplorer.IPackageOperationContext CurrentContext { get; }
        public bool HasContextErrors { get; }
        protected virtual void OnOperationContextDisposing(object sender, Orc.NuGetExplorer.OperationContextEventArgs e) { }
    }
    public abstract class PackageManagerLogListenerBase
    {
        protected PackageManagerLogListenerBase(Orc.NuGetExplorer.INuGetLogListeningSevice nuGetLogListeningSevice) { }
        protected virtual void OnDebug(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected virtual void OnError(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected virtual void OnInfo(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected virtual void OnWarning(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
    }
    public abstract class PackageManagerWatcherBase
    {
        protected PackageManagerWatcherBase(Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService) { }
        protected virtual void OnOperationFinished(object sender, Orc.NuGetExplorer.PackageOperationEventArgs e) { }
        protected virtual void OnOperationStarting(object sender, Orc.NuGetExplorer.PackageOperationEventArgs e) { }
        protected virtual void OnOperationsBatchFinished(object sender, Orc.NuGetExplorer.PackageOperationBatchEventArgs e) { }
        protected virtual void OnOperationsBatchStarting(object sender, Orc.NuGetExplorer.PackageOperationBatchEventArgs e) { }
    }
    public class PackageOperationBatchEventArgs : System.ComponentModel.CancelEventArgs
    {
        public bool IsAutomatic { get; set; }
        public Orc.NuGetExplorer.PackageOperationType OperationType { get; }
        public Orc.NuGetExplorer.IPackageDetails[] Packages { get; }
    }
    public class PackageOperationEventArgs : System.ComponentModel.CancelEventArgs
    {
        public string InstallPath { get; }
        public bool IsAutomatic { get; set; }
        public Orc.NuGetExplorer.IPackageDetails PackageDetails { get; }
        public Orc.NuGetExplorer.PackageOperationType PackageOperationType { get; }
    }
    public class PackageOperationNotificationService : Orc.NuGetExplorer.IPackageOperationNotificationService
    {
        public PackageOperationNotificationService() { }
        public bool IsNotificationsDisabled { get; }
        public bool MuteAutomaticEvents { get; set; }
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationEventArgs> OperationFinished;
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationEventArgs> OperationStarting;
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationBatchEventArgs> OperationsBatchStarting;
        public System.IDisposable DisableNotifications() { }
        public void NotifyAutomaticOperationBatchFinished(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages) { }
        public void NotifyAutomaticOperationBatchStarting(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages) { }
        public void NotifyAutomaticOperationFinished(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails) { }
        public void NotifyAutomaticOperationStarting(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails) { }
        public void NotifyOperationBatchFinished(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages) { }
        public void NotifyOperationBatchStarting(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages) { }
        public void NotifyOperationFinished(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails) { }
        public void NotifyOperationStarting(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails) { }
    }
    public enum PackageOperationType
    {
        None = 0,
        Install = 1,
        Uninstall = 2,
        Update = 3,
    }
    public static class PackageSearchMetadataExtensions
    {
        public static System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.VersionInfo> ToVersionInfo(this System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata> packages, bool includePrerelease) { }
    }
    public class PackageSearchParameters
    {
        public PackageSearchParameters() { }
        public PackageSearchParameters(bool prereleasIncluded, string searchString, bool isRecommendedOnly) { }
        public bool IsPrereleaseIncluded { get; set; }
        public bool IsRecommendedOnly { get; set; }
        public string SearchString { get; set; }
    }
    public class PackageSourceWrapper
    {
        public PackageSourceWrapper(System.Collections.Generic.IReadOnlyList<string> sources) { }
        public PackageSourceWrapper(string source) { }
        public bool IsMultipleSource { get; }
        public System.Collections.Generic.IReadOnlyList<NuGet.Configuration.PackageSource> PackageSources { get; }
        public override string ToString() { }
        public static NuGet.Configuration.PackageSource op_Explicit(Orc.NuGetExplorer.PackageSourceWrapper wrapper) { }
    }
    public sealed class Repository : Orc.NuGetExplorer.IRepository
    {
        public Repository() { }
        public int Id { get; set; }
        public bool IsLocal { get; }
        public string Name { get; set; }
        public Orc.NuGetExplorer.PackageOperationType OperationType { get; set; }
        public string Source { get; set; }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
    }
    public static class RepositoryName
    {
        public const string All = "All";
    }
    public class RollbackWatcher : Orc.NuGetExplorer.PackageManagerContextWatcherBase
    {
        public RollbackWatcher(Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService, Orc.NuGetExplorer.IPackageOperationContextService packageOperationContextService, Orc.NuGetExplorer.IRollbackPackageOperationService rollbackPackageOperationService, Orc.NuGetExplorer.IBackupFileSystemService backupFileSystemService, Orc.NuGetExplorer.IFileSystemService fileSystemService, Orc.FileSystem.IDirectoryService directoryService) { }
        protected override void OnOperationContextDisposing(object sender, Orc.NuGetExplorer.OperationContextEventArgs e) { }
        protected override void OnOperationStarting(object sender, Orc.NuGetExplorer.PackageOperationEventArgs e) { }
    }
    public static class Settings
    {
        public static class NuGet
        {
            public const string CredentialStorage = "NuGetExplorer.CredentialStoragePolicy";
            public const string DestinationFolder = "DestFolder";
            public const string FallbackUrl = "Plugins.FeedUrl";
            public const string IncludePrereleasePackages = "NuGetExplorer.IncludePrerelease";
            public const int PackageCount = 200;
            public const string PackageSources = "PackageSources";
        }
    }
    public static class StringExtensions
    {
        public static string GetSafeScopeName(this string value) { }
        public static System.Collections.Generic.IList<string> SplitOrEmpty(this string value, char separator = ,) { }
    }
    public static class TaskExtensions
    {
        public static System.Threading.Tasks.Task<Orc.NuGetExplorer.TaskResultOrException<>[]> WhenAllOrExceptionAsync<T>(this System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<T>> tasks) { }
        public static System.Threading.Tasks.Task<Orc.NuGetExplorer.TaskResultOrException<T>> WrapResultOrExceptionAsync<T>(this System.Threading.Tasks.Task<T> task) { }
    }
    public class TaskResultOrException<T>
    {
        public TaskResultOrException(System.Exception ex) { }
        public TaskResultOrException(T result) { }
        public System.Exception Exception { get; }
        public bool IsSuccess { get; }
        public T Result { get; }
        public T UnwrapResult() { }
    }
    public static class UriExtensions
    {
        public static System.Uri GetRootUri(this System.Uri uri) { }
    }
    public static class V2SearchHelper
    {
        public static System.Threading.Tasks.Task GetVersionsMetadataAsync(NuGet.Protocol.Core.Types.IPackageSearchMetadata package) { }
    }
    public static class ValidationTags
    {
        public const string Api = "API";
    }
}
namespace Orc.NuGetExplorer.Cache
{
    public interface INuGetCacheManager
    {
        bool ClearAll();
        bool ClearHttpCache();
        NuGet.Protocol.Core.Types.SourceCacheContext GetCacheContext();
        NuGet.Protocol.Core.Types.HttpSourceCacheContext GetHttpCacheContext();
        NuGet.Protocol.Core.Types.HttpSourceCacheContext GetHttpCacheContext(int retryCount, bool directDownload = false);
    }
    public class IconCache
    {
        public IconCache(Catel.Caching.Policies.ExpirationPolicy cacheItemPolicy = null) { }
        public System.Windows.Media.Imaging.BitmapImage FallbackValue { get; set; }
        public Catel.Caching.Policies.ExpirationPolicy StoringPolicy { get; }
        public System.Windows.Media.Imaging.BitmapImage GetFromCache(System.Uri iconUri) { }
        public bool IsCached(System.Uri iconUri) { }
        public void SaveToCache(System.Uri iconUri, byte[] streamContent) { }
    }
    public class NuGetCacheManager : Orc.NuGetExplorer.Cache.INuGetCacheManager
    {
        public NuGetCacheManager(Orc.FileSystem.IDirectoryService directoryService, Orc.FileSystem.IFileService fileService) { }
        public bool ClearAll() { }
        public bool ClearHttpCache() { }
        public NuGet.Protocol.Core.Types.SourceCacheContext GetCacheContext() { }
        public NuGet.Protocol.Core.Types.HttpSourceCacheContext GetHttpCacheContext() { }
        public NuGet.Protocol.Core.Types.HttpSourceCacheContext GetHttpCacheContext(int retryCount, bool directDownload = false) { }
    }
}
namespace Orc.NuGetExplorer.Configuration
{
    public abstract class ConfigurationListenerBase
    {
        protected ConfigurationListenerBase(NuGet.Configuration.ISettings settings) { }
        public virtual System.Threading.Tasks.Task OnSettingsReadAsync() { }
    }
    public enum ConfigurationSection
    {
        Feeds = 0,
        ProjectExtensions = 1,
    }
    public interface IVersionedSettings : NuGet.Configuration.ISettings
    {
        bool IsLastVersion { get; }
        System.Version MinimalVersion { get; }
        System.Version Version { get; }
        event System.EventHandler SettingsRead;
        void UpdateMinimalVersion();
        void UpdateVersion();
    }
}
namespace Orc.NuGetExplorer.Enums
{
    public enum MetadataOrigin
    {
        Browse = 0,
        Installed = 1,
        Updates = 2,
    }
    public enum PackageStatus
    {
        NotInstalled = -2,
        UpdateAvailable = -1,
        LastVersionInstalled = 0,
        Pending = 1,
    }
}
namespace Orc.NuGetExplorer.Loggers
{
    public class NuGetLogger : NuGet.Common.ILogger
    {
        public NuGetLogger(Orc.NuGetExplorer.INuGetLogListeningSevice logListeningService) { }
        public NuGetLogger(bool verbose, Orc.NuGetExplorer.INuGetLogListeningSevice logListeningService) { }
        public System.Threading.Tasks.Task LogAsync(NuGet.Common.ILogMessage message) { }
        public System.Threading.Tasks.Task LogAsync(NuGet.Common.LogLevel level, string data) { }
        public void LogDebug(string data) { }
        public void LogError(string data) { }
        public void LogInformation(string data) { }
        public void LogInformationSummary(string data) { }
        public void LogMinimal(string data) { }
        public void LogVerbose(string data) { }
        public void LogWarning(string data) { }
    }
}
namespace Orc.NuGetExplorer.Management
{
    public class DestFolder : Orc.NuGetExplorer.IExtensibleProject
    {
        public DestFolder(string destinationFolder, Orc.NuGetExplorer.IDefaultNuGetFramework defaultFramework) { }
        public string ContentPath { get; }
        public string Framework { get; }
        public string Name { get; }
        public System.Collections.Immutable.ImmutableList<NuGet.Frameworks.NuGetFramework> SupportedPlatforms { get; set; }
        public string GetInstallPath(NuGet.Packaging.Core.PackageIdentity packageIdentity) { }
        public void Install() { }
        public override string ToString() { }
        public void Uninstall() { }
        public void Update() { }
    }
    public interface IDefaultExtensibleProjectProvider
    {
        Orc.NuGetExplorer.IExtensibleProject GetDefaultProject();
    }
    public interface IExtensibleProjectLocator
    {
        bool IsConfigLoaded { get; }
        void Disable(Orc.NuGetExplorer.IExtensibleProject extensibleProject);
        void Enable(Orc.NuGetExplorer.IExtensibleProject extensibleProject);
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IExtensibleProject> GetAllExtensibleProjects(bool onlyEnabled = true);
        bool IsEnabled(Orc.NuGetExplorer.IExtensibleProject extensibleProject);
        void PersistChanges();
        void Register(Orc.NuGetExplorer.IExtensibleProject project);
        void Register<T>()
            where T : Orc.NuGetExplorer.IExtensibleProject;
        void Register<T>(params object[] parameters)
            where T : Orc.NuGetExplorer.IExtensibleProject;
        void RestoreStateFromConfig();
    }
    public interface INuGetPackageManager : Orc.NuGetExplorer.IPackageManager
    {
        event Catel.AsyncEventHandler<Orc.NuGetExplorer.Management.EventArgs.InstallNuGetProjectEventArgs> Install;
        event Catel.AsyncEventHandler<Orc.NuGetExplorer.Management.EventArgs.UninstallNuGetProjectEventArgs> Uninstall;
        event Catel.AsyncEventHandler<Orc.NuGetExplorer.Management.EventArgs.UpdateNuGetProjectEventArgs> Update;
        System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourceRepository> AsLocalRepositories(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IExtensibleProject> projects);
        System.Threading.Tasks.Task<Orc.NuGetExplorer.Packaging.PackageCollection> CreatePackagesCollectionFromProjectsAsync(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IExtensibleProject> projects, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageReference>> GetInstalledPackagesAsync(Orc.NuGetExplorer.IExtensibleProject project, System.Threading.CancellationToken token);
        System.Threading.Tasks.Task<NuGet.Versioning.NuGetVersion> GetVersionInstalledAsync(Orc.NuGetExplorer.IExtensibleProject project, string packageId, System.Threading.CancellationToken token);
        System.Threading.Tasks.Task<bool> InstallPackageForProjectAsync(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package, System.Threading.CancellationToken token, bool showErrors = true);
        System.Threading.Tasks.Task<bool> IsPackageInstalledAsync(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package, System.Threading.CancellationToken token);
        System.Threading.Tasks.Task UninstallPackageForProjectAsync(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package, System.Threading.CancellationToken token);
        System.Threading.Tasks.Task UpdatePackageForProjectAsync(Orc.NuGetExplorer.IExtensibleProject project, string packageid, NuGet.Versioning.NuGetVersion targetVersion, System.Threading.CancellationToken token);
    }
    public class ProjectInstallException : Orc.NuGetExplorer.Management.ProjectManageException
    {
        public ProjectInstallException(string message) { }
        public ProjectInstallException(string message, System.Exception innerException) { }
        public System.Collections.Generic.IEnumerable<NuGet.Packaging.Core.PackageIdentity> CurrentBatch { get; set; }
    }
    public class ProjectManageException : System.Exception
    {
        public ProjectManageException(string message) { }
        public ProjectManageException(string message, System.Exception innerException) { }
    }
    public class ProjectStateException : System.Exception
    {
        public ProjectStateException(string message) { }
    }
    public class SourceContext : System.IDisposable
    {
        public SourceContext(System.Collections.Generic.IReadOnlyList<NuGet.Configuration.PackageSource> packageSources) { }
        public SourceContext(System.Collections.Generic.IReadOnlyList<NuGet.Protocol.Core.Types.SourceRepository> sourceRepositories) { }
        public System.Collections.Generic.IReadOnlyList<NuGet.Configuration.PackageSource> PackageSources { get; }
        public System.Collections.Generic.IReadOnlyList<NuGet.Protocol.Core.Types.SourceRepository> Repositories { get; }
        public static Orc.NuGetExplorer.Management.SourceContext CurrentContext { get; }
        public static Orc.NuGetExplorer.Management.SourceContext EmptyContext { get; set; }
        public void Dispose() { }
    }
}
namespace Orc.NuGetExplorer.Management.EventArgs
{
    public class BatchedInstallNuGetProjectEventArgs : Orc.NuGetExplorer.Management.EventArgs.InstallNuGetProjectEventArgs
    {
        public BatchedInstallNuGetProjectEventArgs(Orc.NuGetExplorer.Management.EventArgs.InstallNuGetProjectEventArgs eventArgs) { }
        public bool IsBatchEnd { get; set; }
    }
    public class BatchedUninstallNuGetProjectEventArgs : Orc.NuGetExplorer.Management.EventArgs.UninstallNuGetProjectEventArgs
    {
        public BatchedUninstallNuGetProjectEventArgs(Orc.NuGetExplorer.Management.EventArgs.UninstallNuGetProjectEventArgs eventArgs) { }
        public bool IsBatchEnd { get; set; }
    }
    public class InstallNuGetProjectEventArgs : Orc.NuGetExplorer.Management.EventArgs.NuGetProjectEventArgs
    {
        public InstallNuGetProjectEventArgs(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package, bool result) { }
        public bool Result { get; }
    }
    public class NuGetProjectEventArgs : System.EventArgs
    {
        public NuGetProjectEventArgs(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package) { }
        public NuGet.Packaging.Core.PackageIdentity Package { get; }
        public Orc.NuGetExplorer.IExtensibleProject Project { get; }
    }
    public class UninstallNuGetProjectEventArgs : Orc.NuGetExplorer.Management.EventArgs.NuGetProjectEventArgs
    {
        public UninstallNuGetProjectEventArgs(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package, bool result) { }
        public bool Result { get; }
    }
    public class UpdateNuGetProjectEventArgs : Orc.NuGetExplorer.Management.EventArgs.NuGetProjectEventArgs
    {
        public UpdateNuGetProjectEventArgs(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity package) { }
        public UpdateNuGetProjectEventArgs(Orc.NuGetExplorer.IExtensibleProject project, NuGet.Packaging.Core.PackageIdentity beforeUpdate, System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.Management.EventArgs.NuGetProjectEventArgs> updateEventArgs) { }
        public NuGet.Versioning.NuGetVersion InstalledVersion { get; set; }
        public System.Collections.Generic.List<NuGet.Versioning.NuGetVersion> RemovedVersions { get; set; }
    }
}
namespace Orc.NuGetExplorer.Management.Exceptions
{
    public class IncompatiblePackageException : Orc.NuGetExplorer.Management.ProjectInstallException
    {
        public IncompatiblePackageException(string message) { }
        public IncompatiblePackageException(string message, System.Exception innerException) { }
    }
    public class MissingPackageException : Orc.NuGetExplorer.Management.ProjectInstallException
    {
        public MissingPackageException(string message) { }
        public MissingPackageException(string message, System.Exception innerException) { }
    }
}
namespace Orc.NuGetExplorer.Messaging
{
    public interface INuGetExplorerServiceMessage { }
    public class PackagingDeletemeMessage : Catel.Messaging.MessageBase<Orc.NuGetExplorer.Messaging.PackagingDeletemeMessage, Orc.NuGetExplorer.Packaging.PackageOperationInfo>, Orc.NuGetExplorer.Messaging.INuGetExplorerServiceMessage
    {
        public PackagingDeletemeMessage() { }
        public PackagingDeletemeMessage(Orc.NuGetExplorer.Packaging.PackageOperationInfo content) { }
    }
}
namespace Orc.NuGetExplorer.Models
{
    public sealed class CombinedNuGetSource : Orc.NuGetExplorer.IPackageSource, Orc.NuGetExplorer.Models.INuGetSource
    {
        public CombinedNuGetSource(System.Collections.Generic.IReadOnlyList<Orc.NuGetExplorer.Models.INuGetSource> feedList) { }
        public bool IsAccessible { get; }
        public bool IsEnabled { get; }
        public bool IsOfficial { get; set; }
        public bool IsSelected { get; set; }
        public bool IsVerified { get; }
        public string Name { get; }
        public string Source { get; }
        public void AddFeed(Orc.NuGetExplorer.Models.NuGetFeed feed) { }
        public System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.Models.NuGetFeed> GetAllSources() { }
        public Orc.NuGetExplorer.PackageSourceWrapper GetPackageSource() { }
        public bool IsAllFeedsAccessible() { }
        public bool IsAllVerified() { }
        public void RemoveFeed(Orc.NuGetExplorer.Models.NuGetFeed feed) { }
        public override string ToString() { }
    }
    public sealed class ExplorerSettingsContainer : Catel.Data.ModelBase, Orc.NuGetExplorer.Models.INuGetSettings
    {
        public static readonly Catel.Data.PropertyData DefaultFeedProperty;
        public static readonly Catel.Data.PropertyData IsPreReleaseIncludedProperty;
        public static readonly Catel.Data.PropertyData IsRecommendedOnlyProperty;
        public static readonly Catel.Data.PropertyData NuGetFeedsProperty;
        public static readonly Catel.Data.PropertyData ObservedFeedProperty;
        public static readonly Catel.Data.PropertyData SearchStringProperty;
        public ExplorerSettingsContainer() { }
        public Orc.NuGetExplorer.Models.INuGetSource DefaultFeed { get; set; }
        public bool IsPreReleaseIncluded { get; set; }
        public bool IsRecommendedOnly { get; set; }
        public System.Collections.Generic.List<Orc.NuGetExplorer.Models.NuGetFeed> NuGetFeeds { get; set; }
        public Orc.NuGetExplorer.Models.INuGetSource ObservedFeed { get; set; }
        public string SearchString { get; set; }
        public void Clear() { }
        public System.Collections.Generic.IReadOnlyList<NuGet.Configuration.PackageSource> GetAllPackageSources() { }
        protected override void OnPropertyChanged(Catel.Data.AdvancedPropertyChangedEventArgs e) { }
    }
    public interface INuGetSettings
    {
        System.Collections.Generic.IReadOnlyList<NuGet.Configuration.PackageSource> GetAllPackageSources();
    }
    public interface INuGetSource : Orc.NuGetExplorer.IPackageSource
    {
        bool IsAccessible { get; }
        bool IsSelected { get; set; }
        bool IsVerified { get; }
        Orc.NuGetExplorer.PackageSourceWrapper GetPackageSource();
    }
    public class NuGetActionTarget : Catel.Data.ModelBase
    {
        public static readonly Catel.Data.PropertyData IsValidProperty;
        public NuGetActionTarget() { }
        public bool IsTargetProjectCanBeChanged { get; }
        public bool IsValid { get; }
        public System.Collections.Generic.IReadOnlyList<Orc.NuGetExplorer.IExtensibleProject> TargetProjects { get; }
        public void Add(Orc.NuGetExplorer.IExtensibleProject project) { }
        protected override void OnPropertyChanged(Catel.Data.AdvancedPropertyChangedEventArgs e) { }
        public void Remove(Orc.NuGetExplorer.IExtensibleProject project) { }
    }
    public sealed class NuGetFeed : Catel.Data.ModelBase, Orc.NuGetExplorer.ICloneable<Orc.NuGetExplorer.Models.NuGetFeed>, Orc.NuGetExplorer.IPackageSource, Orc.NuGetExplorer.Models.INuGetSource, System.ComponentModel.IDataErrorInfo, System.ComponentModel.INotifyDataErrorInfo
    {
        public static readonly Catel.Data.PropertyData ErrorProperty;
        public static readonly Catel.Data.PropertyData IsAccessibleProperty;
        public static readonly Catel.Data.PropertyData IsEnabledProperty;
        public static readonly Catel.Data.PropertyData IsNameValidProperty;
        public static readonly Catel.Data.PropertyData IsOfficialProperty;
        public static readonly Catel.Data.PropertyData IsRestrictedProperty;
        public static readonly Catel.Data.PropertyData IsSelectedProperty;
        public static readonly Catel.Data.PropertyData IsVerifiedNowProperty;
        public static readonly Catel.Data.PropertyData IsVerifiedProperty;
        public static readonly Catel.Data.PropertyData NameProperty;
        public static readonly Catel.Data.PropertyData SerializationIdentifierProperty;
        public static readonly Catel.Data.PropertyData SourceProperty;
        public static readonly Catel.Data.PropertyData VerificationResultProperty;
        public NuGetFeed() { }
        public NuGetFeed(string name, string source) { }
        public NuGetFeed(string name, string source, bool isEnabled) { }
        public NuGetFeed(string name, string source, bool isEnabled, bool isOfficial) { }
        public string Error { get; }
        public bool HasErrors { get; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsAccessible { get; set; }
        public bool IsEnabled { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsNameValid { get; }
        public bool IsOfficial { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsSelected { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsVerified { get; }
        [System.Xml.Serialization.XmlIgnore]
        public bool IsVerifiedNow { get; set; }
        public string this[string columnName] { get; }
        public string Name { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public System.Guid SerializationIdentifier { get; set; }
        public string Source { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public Orc.NuGetExplorer.FeedVerificationResult VerificationResult { get; set; }
        public event System.EventHandler<System.ComponentModel.DataErrorsChangedEventArgs> ErrorsChanged;
        public Orc.NuGetExplorer.Models.NuGetFeed Clone() { }
        public void ForceCancelEdit() { }
        public void ForceEndEdit() { }
        public System.Collections.IEnumerable GetErrors(string propertyName) { }
        public Orc.NuGetExplorer.PackageSourceWrapper GetPackageSource() { }
        public System.Uri GetUriSource() { }
        public new void Initialize() { }
        public bool IsLocal() { }
        public bool IsValid() { }
        protected override void OnPropertyChanged(Catel.Data.AdvancedPropertyChangedEventArgs e) { }
        public override string ToString() { }
    }
    public sealed class NuGetPackage : Catel.Data.ModelBase, Orc.NuGetExplorer.IPackageDetails
    {
        public static readonly Catel.Data.PropertyData AuthorsProperty;
        public static readonly Catel.Data.PropertyData AvailableVersionsProperty;
        public static readonly Catel.Data.PropertyData DependenciesProperty;
        public static readonly Catel.Data.PropertyData DescriptionProperty;
        public static readonly Catel.Data.PropertyData DownloadCountProperty;
        public static readonly Catel.Data.PropertyData IconUrlProperty;
        public static readonly Catel.Data.PropertyData InstalledVersionProperty;
        public static readonly Catel.Data.PropertyData IsCheckedProperty;
        public static readonly Catel.Data.PropertyData IsInstalledProperty;
        public static readonly Catel.Data.PropertyData IsLoadedProperty;
        public static readonly Catel.Data.PropertyData LastVersionProperty;
        public static readonly Catel.Data.PropertyData SelectedVersionProperty;
        public static readonly Catel.Data.PropertyData SpecialVersionProperty;
        public static readonly Catel.Data.PropertyData StatusProperty;
        public static readonly Catel.Data.PropertyData SummaryProperty;
        public static readonly Catel.Data.PropertyData TitleProperty;
        public static readonly Catel.Data.PropertyData ValidationContextProperty;
        public static readonly Catel.Data.PropertyData VersionsInfoProperty;
        public NuGetPackage(NuGet.Protocol.Core.Types.IPackageSearchMetadata packageMetadata, Orc.NuGetExplorer.Enums.MetadataOrigin fromPage) { }
        public string Authors { get; }
        public System.Collections.Generic.IList<string> AvailableVersions { get; set; }
        public string Dependencies { get; set; }
        public string Description { get; }
        public int? DownloadCount { get; }
        public Orc.NuGetExplorer.Enums.MetadataOrigin FromPage { get; }
        public string FullName { get; }
        public System.Uri IconUrl { get; }
        public string Id { get; }
        public NuGet.Packaging.Core.PackageIdentity Identity { get; }
        public NuGet.Versioning.NuGetVersion InstalledVersion { get; set; }
        public bool IsAbsoluteLatestVersion { get; }
        public bool IsChecked { get; set; }
        public bool? IsInstalled { get; set; }
        public bool IsLatestVersion { get; }
        public bool IsLoaded { get; }
        public bool IsPrerelease { get; }
        public NuGet.Versioning.NuGetVersion LastVersion { get; }
        public NuGet.Versioning.NuGetVersion NuGetVersion { get; }
        public System.DateTimeOffset? Published { get; }
        public string SelectedVersion { get; set; }
        public string SpecialVersion { get; set; }
        public Orc.NuGetExplorer.Enums.PackageStatus Status { get; set; }
        public string Summary { get; }
        public string Title { get; }
        public Catel.Data.IValidationContext ValidationContext { get; set; }
        public System.Version Version { get; }
        public System.Collections.Generic.IReadOnlyList<NuGet.Versioning.NuGetVersion> Versions { get; }
        public System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.VersionInfo> VersionsInfo { get; }
        public  static  event System.EventHandler AnyNuGetPackageCheckedChanged;
        public void AddDependencyInfo(NuGet.Versioning.NuGetVersion version, System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageDependencyGroup> dependencyGroups) { }
        public System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageDependencyGroup> GetDependencyInfo(NuGet.Versioning.NuGetVersion version) { }
        public NuGet.Packaging.Core.PackageIdentity GetIdentity() { }
        public NuGet.Protocol.Core.Types.IPackageSearchMetadata GetMetadata() { }
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Versioning.NuGetVersion>> LoadVersionsAsync() { }
        public System.Threading.Tasks.Task MergeMetadataAsync(NuGet.Protocol.Core.Types.IPackageSearchMetadata searchMetadata, Orc.NuGetExplorer.Enums.MetadataOrigin pageToken) { }
        protected override void OnPropertyChanged(Catel.Data.AdvancedPropertyChangedEventArgs e) { }
        public void ResetValidationContext() { }
    }
}
namespace Orc.NuGetExplorer.Native
{
    [System.Serializable]
    public class CredentialException : System.ComponentModel.Win32Exception
    {
        public CredentialException() { }
        public CredentialException(int error) { }
        public CredentialException(string message) { }
        public CredentialException(int error, string message) { }
        protected CredentialException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public CredentialException(string message, System.Exception innerException) { }
    }
}
namespace Orc.NuGetExplorer.Packaging
{
    public class EmptyPackageDetails : Orc.NuGetExplorer.IPackageDetails
    {
        public EmptyPackageDetails(NuGet.Packaging.Core.PackageIdentity package) { }
        public System.Collections.Generic.IEnumerable<string> Authors { get; }
        public System.Collections.Generic.IList<string> AvailableVersions { get; }
        public string Dependencies { get; }
        public string Description { get; }
        public int? DownloadCount { get; }
        public string FullName { get; }
        public System.Uri IconUrl { get; }
        public string Id { get; }
        [System.Obsolete("Use `IsLatestVersion` instead. Will be removed in version 5.0.0.", true)]
        public bool IsAbsoluteLatestVersion { get; }
        public bool? IsInstalled { get; set; }
        public bool IsLatestVersion { get; }
        public bool IsPrerelease { get; }
        public NuGet.Versioning.NuGetVersion NuGetVersion { get; }
        public System.DateTimeOffset? Published { get; }
        public string SelectedVersion { get; set; }
        public string SpecialVersion { get; }
        public string Title { get; }
        public Catel.Data.IValidationContext ValidationContext { get; }
        public System.Version Version { get; }
        public NuGet.Packaging.Core.PackageIdentity GetIdentity() { }
        public void ResetValidationContext() { }
    }
    public class NuGetPackageCombinator
    {
        public NuGetPackageCombinator() { }
        public static System.Threading.Tasks.Task<Orc.NuGetExplorer.Enums.PackageStatus> CombineAsync(Orc.NuGetExplorer.Models.NuGetPackage package, Orc.NuGetExplorer.Enums.MetadataOrigin tokenPage, NuGet.Protocol.Core.Types.IPackageSearchMetadata metadata) { }
    }
    public sealed class PackageCollection : System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.Packaging.PackageCollectionItem>, System.Collections.IEnumerable
    {
        public PackageCollection(Orc.NuGetExplorer.Packaging.PackageCollectionItem[] packages) { }
        public bool ContainsId(string packageId) { }
        public System.Collections.Generic.IEnumerator<Orc.NuGetExplorer.Packaging.PackageCollectionItem> GetEnumerator() { }
    }
    public sealed class PackageCollectionItem : NuGet.Packaging.Core.PackageIdentity
    {
        public PackageCollectionItem(string id, NuGet.Versioning.NuGetVersion version, System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageReference> installedReferences) { }
        public System.Collections.Generic.List<NuGet.Packaging.PackageReference> PackageReferences { get; }
    }
    public class PackageDetails : Orc.NuGetExplorer.IPackageDetails
    {
        public PackageDetails(NuGet.Protocol.Core.Types.IPackageSearchMetadata metadata, bool isLatestVersion = false) { }
        public PackageDetails(NuGet.Protocol.Core.Types.IPackageSearchMetadata metadata, NuGet.Packaging.Core.PackageIdentity identity, bool isLatestVersion = false) { }
        public System.Collections.Generic.IEnumerable<string> Authors { get; }
        public System.Collections.Generic.IList<string> AvailableVersions { get; }
        public virtual string Dependencies { get; set; }
        public System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageDependencyGroup> DependencySets { get; set; }
        public string Description { get; }
        public int? DownloadCount { get; }
        public string FullName { get; }
        public System.Uri IconUrl { get; }
        public string Id { get; }
        [System.Obsolete("Use `IsLatestVersion` instead. Will be removed in version 5.0.0.", true)]
        public bool IsAbsoluteLatestVersion { get; }
        public bool? IsInstalled { get; set; }
        public bool IsLatestVersion { get; }
        public bool IsPrerelease { get; }
        public NuGet.Versioning.NuGetVersion NuGetVersion { get; }
        public System.DateTimeOffset? Published { get; }
        public string SelectedVersion { get; set; }
        public string SpecialVersion { get; }
        public string Title { get; }
        public Catel.Data.IValidationContext ValidationContext { get; set; }
        public System.Version Version { get; }
        public virtual NuGet.Packaging.Core.PackageIdentity GetIdentity() { }
        public virtual void ResetValidationContext() { }
    }
    public class PackageDetailsFactory
    {
        public PackageDetailsFactory() { }
        public static Orc.NuGetExplorer.IPackageDetails Create(NuGet.Packaging.Core.PackageIdentity packageIdentity) { }
        public static Orc.NuGetExplorer.IPackageDetails Create(Orc.NuGetExplorer.PackageOperationType operationType, NuGet.Protocol.Core.Types.IPackageSearchMetadata versionMetadata, NuGet.Packaging.Core.PackageIdentity packageIdentity, bool? isLastVersion) { }
        public static Orc.NuGetExplorer.IPackageDetails Create(Orc.NuGetExplorer.PackageOperationType operationType, NuGet.Protocol.Core.Types.IPackageSearchMetadata versionMetadata, NuGet.Versioning.NuGetVersion packageVersion, bool? isLastVersion) { }
    }
    public class PackageOperationInfo
    {
        public PackageOperationInfo(string operationPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails package) { }
        public string OperationPath { get; }
        public Orc.NuGetExplorer.PackageOperationType OperationType { get; }
        public Orc.NuGetExplorer.IPackageDetails Package { get; }
    }
    public class UpdatePackageSearchMetadata : NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder.ClonedPackageSearchMetadata
    {
        public UpdatePackageSearchMetadata() { }
        public NuGet.Protocol.Core.Types.VersionInfo FromVersion { get; set; }
        public new NuGet.Common.AsyncLazy<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.VersionInfo>> LazyVersionsFactory { get; set; }
    }
    public class UpdatePackageSearchMetadataBuilder
    {
        public NuGet.Protocol.Core.Types.IPackageSearchMetadata Build() { }
        public static Orc.NuGetExplorer.Packaging.UpdatePackageSearchMetadataBuilder FromMetadatas(NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder.ClonedPackageSearchMetadata metadata, NuGet.Protocol.Core.Types.IPackageSearchMetadata updatedVersionMetadata) { }
    }
}
namespace Orc.NuGetExplorer.Pagination
{
    public class DeferToken
    {
        public DeferToken() { }
        public Orc.NuGetExplorer.Enums.MetadataOrigin LoadType { get; set; }
        public Orc.NuGetExplorer.Models.NuGetPackage Package { get; set; }
        public System.Func<NuGet.Protocol.Core.Types.IPackageSearchMetadata> PackageSelector { get; set; }
        public NuGet.Protocol.Core.Types.IPackageSearchMetadata Result { get; set; }
        public System.Action<Orc.NuGetExplorer.Enums.PackageStatus> UpdateAction { get; set; }
    }
    public class PageContinuation
    {
        public PageContinuation(Orc.NuGetExplorer.Pagination.PageContinuation continuation) { }
        public PageContinuation(Orc.NuGetExplorer.Pagination.PageContinuation continuation, bool onlyLocal) { }
        public PageContinuation(int pageSize, Orc.NuGetExplorer.PackageSourceWrapper packageSourceWrapper) { }
        public int Current { get; }
        public bool IsValid { get; }
        public int LastNumber { get; }
        public int Next { get; }
        public bool OnlyLocal { get; set; }
        public int Size { get; }
        public Orc.NuGetExplorer.PackageSourceWrapper Source { get; }
        public int GetNext() { }
        public int GetNext(int count) { }
        public int GetPrevious() { }
    }
}
namespace Orc.NuGetExplorer.Providers
{
    public class DefaultSourceRepositoryProvider : NuGet.Protocol.Core.Types.ISourceRepositoryProvider, Orc.NuGetExplorer.IExtendedSourceRepositoryProvider
    {
        public DefaultSourceRepositoryProvider(Orc.NuGetExplorer.Providers.IModelProvider<Orc.NuGetExplorer.Models.ExplorerSettingsContainer> settingsProvider, Orc.NuGetExplorer.INuGetConfigurationService nuGetConfigurationService) { }
        public NuGet.Configuration.IPackageSourceProvider PackageSourceProvider { get; }
        public NuGet.Protocol.Core.Types.SourceRepository CreateLocalRepository(string source) { }
        public NuGet.Protocol.Core.Types.SourceRepository CreateRepository(NuGet.Configuration.PackageSource source) { }
        public NuGet.Protocol.Core.Types.SourceRepository CreateRepository(NuGet.Configuration.PackageSource source, NuGet.Protocol.FeedType type) { }
        public NuGet.Protocol.Core.Types.SourceRepository CreateRepository(NuGet.Configuration.PackageSource source, bool forceUpdate) { }
        public System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourceRepository> GetRepositories() { }
    }
    public class EmptyProjectContextProvider : Orc.NuGetExplorer.INuGetProjectContextProvider
    {
        public EmptyProjectContextProvider() { }
        public NuGet.ProjectManagement.INuGetProjectContext GetProjectContext(NuGet.ProjectManagement.FileConflictAction fileConflictAction) { }
    }
    public class ExplorerCacheProvider : Orc.NuGetExplorer.Providers.IApplicationCacheProvider
    {
        public ExplorerCacheProvider() { }
        public Orc.NuGetExplorer.Cache.IconCache EnsureIconCache() { }
    }
    public interface IApplicationCacheProvider
    {
        Orc.NuGetExplorer.Cache.IconCache EnsureIconCache();
    }
    public interface IModelProvider<T> : System.ComponentModel.INotifyPropertyChanged
        where T : Catel.Data.ModelBase
    {
        T Model { get; set; }
        T Create();
    }
    public interface IPackageMetadataProvider
    {
        System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetHighestPackageMetadataAsync(string packageId, bool includePrerelease, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetLocalPackageMetadataAsync(NuGet.Packaging.Core.PackageIdentity identity, bool includePrerelease, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetLowestLocalPackageMetadataAsync(string packageid, bool includePrrelease, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetPackageMetadataAsync(NuGet.Packaging.Core.PackageIdentity identity, bool includePrerelease, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> GetPackageMetadataListAsync(string packageId, bool includePrerelease, bool includeUnlisted, System.Threading.CancellationToken cancellationToken);
    }
    public class ModelProvider<T> : Orc.NuGetExplorer.Providers.IModelProvider<T>, System.ComponentModel.INotifyPropertyChanged
        where T : Catel.Data.ModelBase
    {
        public ModelProvider(Catel.IoC.ITypeFactory typeFactory) { }
        public virtual T Model { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public virtual T Create() { }
    }
    public class PackageMetadataProvider : Orc.NuGetExplorer.Providers.IPackageMetadataProvider
    {
        public PackageMetadataProvider(Orc.FileSystem.IDirectoryService directoryService, Orc.NuGetExplorer.IRepositoryService repositoryService, NuGet.Protocol.Core.Types.ISourceRepositoryProvider repositoryProvider) { }
        public PackageMetadataProvider(Orc.FileSystem.IDirectoryService directoryService, System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourceRepository> sourceRepositories, System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourceRepository> optionalGlobalLocalRepositories, NuGet.Protocol.Core.Types.SourceRepository localRepository = null) { }
        public System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetHighestPackageMetadataAsync(string packageId, bool includePrerelease, System.Threading.CancellationToken cancellationToken) { }
        public System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetLocalPackageMetadataAsync(NuGet.Packaging.Core.PackageIdentity identity, bool includePrerelease, System.Threading.CancellationToken cancellationToken) { }
        public System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetLowestLocalPackageMetadataAsync(string packageid, bool includePrrelease, System.Threading.CancellationToken cancellationToken) { }
        public System.Threading.Tasks.Task<NuGet.Protocol.Core.Types.IPackageSearchMetadata> GetPackageMetadataAsync(NuGet.Packaging.Core.PackageIdentity identity, bool includePrerelease, System.Threading.CancellationToken cancellationToken) { }
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> GetPackageMetadataListAsync(string packageId, bool includePrerelease, bool includeUnlisted, System.Threading.CancellationToken cancellationToken) { }
        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata>> GetPackageMetadataListAsyncFromSourceAsync(NuGet.Protocol.Core.Types.SourceRepository repository, string packageId, bool includePrerelease, bool includeUnlisted, System.Threading.CancellationToken cancellationToken) { }
        public static Orc.NuGetExplorer.Providers.PackageMetadataProvider CreateFromSourceContext(Orc.FileSystem.IDirectoryService directoryService, Orc.NuGetExplorer.IRepositoryContextService repositoryService, Orc.NuGetExplorer.Management.IExtensibleProjectLocator projectSource, Orc.NuGetExplorer.Management.INuGetPackageManager projectManager) { }
    }
    public class WindowsCredentialProvider : NuGet.Credentials.ICredentialProvider
    {
        public WindowsCredentialProvider(Catel.Configuration.IConfigurationService configurationService) { }
        public string Id { get; }
        public System.Threading.Tasks.Task<NuGet.Credentials.CredentialResponse> GetAsync(System.Uri uri, System.Net.IWebProxy proxy, NuGet.Configuration.CredentialRequestType type, string message, bool isRetry, bool nonInteractive, System.Threading.CancellationToken cancellationToken) { }
    }
}
namespace Orc.NuGetExplorer.Resolver
{
    public class PackageResolver
    {
        public PackageResolver() { }
        public System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo> Resolve(Orc.NuGetExplorer.Resolver.PackageResolverContext context, System.Threading.CancellationToken token) { }
        public System.Threading.Tasks.Task<System.Collections.Generic.List<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo>> ResolveWithVersionOverrideAsync(Orc.NuGetExplorer.Resolver.PackageResolverContext context, Orc.NuGetExplorer.IExtensibleProject project, NuGet.Resolver.DependencyBehavior dependencyBehavior, System.Action<Orc.NuGetExplorer.IExtensibleProject, NuGet.Packaging.PackageReference> conflictResolveAction, System.Threading.CancellationToken cancellationToken) { }
    }
    public class PackageResolverContext : NuGet.Resolver.PackageResolverContext
    {
        public static readonly Orc.NuGetExplorer.Resolver.PackageResolverContext Empty;
        public PackageResolverContext(NuGet.Resolver.DependencyBehavior dependencyBehavior, System.Collections.Generic.IEnumerable<string> targetIds, System.Collections.Generic.IEnumerable<string> requiredPackageIds, System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageReference> packagesConfig, System.Collections.Generic.IEnumerable<NuGet.Packaging.Core.PackageIdentity> preferredVersions, System.Collections.Generic.IEnumerable<NuGet.Protocol.Core.Types.SourcePackageDependencyInfo> availablePackages, System.Collections.Generic.IEnumerable<NuGet.Configuration.PackageSource> packageSources, System.Collections.Generic.IEnumerable<string> ignoredIds, NuGet.Common.ILogger log) { }
        public System.Collections.Generic.IEnumerable<string> IgnoredIds { get; set; }
    }
    public static class PackageResolverExtensions
    {
        public static System.Collections.Generic.IEnumerable<NuGet.Packaging.Core.PackageIdentity> Resolve(this Orc.NuGetExplorer.Resolver.PackageResolver resolver, Orc.NuGetExplorer.Resolver.PackageResolverContext context, System.Threading.CancellationToken token) { }
    }
}
namespace Orc.NuGetExplorer.Scenario
{
    public interface IUpgradeScenario
    {
        System.Threading.Tasks.Task<bool> RunAsync();
    }
    public abstract class UpgradeListenerBase
    {
        protected UpgradeListenerBase(Orc.NuGetExplorer.Services.INuGetProjectUpgradeService upgradeRunner) { }
        protected virtual void OnUpgraded(object sender, System.EventArgs e) { }
        protected virtual void OnUpgrading(object sender, System.EventArgs e) { }
    }
    public class V3RestorePackageConfigAndReinstall : Orc.NuGetExplorer.Scenario.IUpgradeScenario
    {
        public V3RestorePackageConfigAndReinstall(Orc.NuGetExplorer.Management.IDefaultExtensibleProjectProvider projectProvider, Orc.NuGetExplorer.Management.INuGetPackageManager nuGetPackageManager, Orc.NuGetExplorer.IRepositoryContextService repositoryContextService, NuGet.Common.ILogger logger, Catel.Configuration.IConfigurationService configurationService, Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService, Orc.FileSystem.IDirectoryService directoryService) { }
        public System.Threading.Tasks.Task<bool> RunAsync() { }
        public override string ToString() { }
    }
}
namespace Orc.NuGetExplorer.Scopes
{
    public class AuthenticationScope : Catel.Disposable
    {
        public AuthenticationScope(bool? canPromptForAuthentication = default) { }
        public bool CanPromptForAuthentication { get; }
        public bool HasPromptedForAuthentication { get; set; }
    }
}
namespace Orc.NuGetExplorer.Services
{
    public class DownloadingProgressTrackerService : Orc.NuGetExplorer.Services.IDownloadingProgressTrackerService
    {
        public DownloadingProgressTrackerService(NuGet.Common.ILogger nugetLogger, Orc.FileSystem.IDirectoryService directoryService, Orc.FileSystem.IFileService fileService) { }
        public System.Threading.Tasks.Task<Catel.IDisposableToken<System.IProgress<float>>> TrackDownloadOperationAsync(Orc.NuGetExplorer.Services.IPackageInstallationService packageInstallationService, NuGet.Protocol.Core.Types.SourcePackageDependencyInfo packageDependencyInfo) { }
    }
    public interface IDefferedPackageLoaderService
    {
        void Add(Orc.NuGetExplorer.Pagination.DeferToken token);
        System.Threading.Tasks.Task StartLoadingAsync();
    }
    public interface IDownloadingProgressTrackerService
    {
        System.Threading.Tasks.Task<Catel.IDisposableToken<System.IProgress<float>>> TrackDownloadOperationAsync(Orc.NuGetExplorer.Services.IPackageInstallationService packageInstallationService, NuGet.Protocol.Core.Types.SourcePackageDependencyInfo packageDependencyInfo);
    }
    public interface INuGetExplorerInitializationService
    {
        string DefaultSourceKey { get; }
        int PackageQuerySize { get; set; }
        System.Threading.Tasks.Task<bool> UpgradeNuGetPackagesIfNeededAsync();
    }
    public interface INuGetProjectUpgradeService
    {
        event System.EventHandler UpgradeEnd;
        event System.EventHandler UpgradeStart;
        void AddUpgradeScenario(Orc.NuGetExplorer.Scenario.IUpgradeScenario scenario);
        System.Threading.Tasks.Task<bool> CheckCurrentConfigurationAndRunAsync();
    }
    public interface IPackageInstallationService
    {
        NuGet.Packaging.VersionFolderPathResolver InstallerPathResolver { get; }
        System.Threading.Tasks.Task<Orc.NuGetExplorer.InstallerResult> InstallAsync(NuGet.Packaging.Core.PackageIdentity package, Orc.NuGetExplorer.IExtensibleProject project, System.Collections.Generic.IReadOnlyList<NuGet.Protocol.Core.Types.SourceRepository> repositories, bool ignoreMissingPackages = false, System.Threading.CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<long?> MeasurePackageSizeFromRepositoryAsync(NuGet.Packaging.Core.PackageIdentity packageIdentity, NuGet.Protocol.Core.Types.SourceRepository sourceRepository);
        System.Threading.Tasks.Task UninstallAsync(NuGet.Packaging.Core.PackageIdentity package, Orc.NuGetExplorer.IExtensibleProject project, System.Collections.Generic.IEnumerable<NuGet.Packaging.PackageReference> installedPackageReferences, System.Threading.CancellationToken cancellationToken = default);
    }
    public class NuGetConfigurationService : Orc.NuGetExplorer.INuGetConfigurationService
    {
        public NuGetConfigurationService(Catel.Configuration.IConfigurationService configurationService, Catel.Services.IAppDataService appDataService) { }
        public void DisablePackageSource(string name, string source) { }
        public string GetDestinationFolder() { }
        public bool GetIsPrereleaseAllowed(Orc.NuGetExplorer.IRepository repository) { }
        public int GetPackageQuerySize() { }
        public bool IsProjectConfigured(Orc.NuGetExplorer.IExtensibleProject project) { }
        public System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> LoadPackageSources(bool onlyEnabled = false) { }
        public void RemovePackageSource(Orc.NuGetExplorer.IPackageSource source) { }
        public bool SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true, bool verifyFeed = true) { }
        public void SavePackageSources(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> packageSources) { }
        public void SaveProjects(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IExtensibleProject> extensibleProjects) { }
        public void SetDestinationFolder(string value) { }
        public void SetIsPrereleaseAllowed(Orc.NuGetExplorer.IRepository repository, bool value) { }
        public void SetPackageQuerySize(int size) { }
    }
}
namespace Orc.NuGetExplorer.Watchers.Base
{
    public abstract class PackageSourcesWatcherBase
    {
        protected PackageSourcesWatcherBase(NuGet.Configuration.IPackageSourceProvider packageSourceProvider) { }
        protected virtual void OnPackageSourcesChanged(string packageSource) { }
    }
}
namespace Orc.NuGetExplorer.Web
{
    public static class FatalProtocolExceptionExtension
    {
        public static bool HidesForbiddenError(this NuGet.Protocol.Core.Types.FatalProtocolException fatalProtocolException) { }
        public static bool HidesUnauthorizedError(this NuGet.Protocol.Core.Types.FatalProtocolException fatalProtocolException) { }
    }
    public class FatalProtocolExceptionHandler : Orc.NuGetExplorer.Web.IHttpExceptionHandler<NuGet.Protocol.Core.Types.FatalProtocolException>
    {
        public FatalProtocolExceptionHandler() { }
        public Orc.NuGetExplorer.FeedVerificationResult HandleException(NuGet.Protocol.Core.Types.FatalProtocolException exception, string source) { }
    }
    public class HttpWebExceptionHandler : Orc.NuGetExplorer.Web.IHttpExceptionHandler<System.Net.WebException>
    {
        public HttpWebExceptionHandler() { }
        public Orc.NuGetExplorer.FeedVerificationResult HandleException(System.Net.WebException exception, string source) { }
    }
    public interface IHttpExceptionHandler<T>
    {
        Orc.NuGetExplorer.FeedVerificationResult HandleException(T exception, string source);
    }
}