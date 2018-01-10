[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Orc.NuGetExplorer.Tests")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
[assembly: System.Windows.ThemeInfoAttribute(System.Windows.ResourceDictionaryLocation.SourceAssembly, System.Windows.ResourceDictionaryLocation.SourceAssembly)]


public class static MethodTimeLogger
{
    public static void Log(System.Reflection.MethodBase methodBase, long milliseconds) { }
    public static void Log(System.Type type, string methodName, long milliseconds) { }
}
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.NuGetExplorer
{
    
    public class ApiValidationException : System.Exception
    {
        public ApiValidationException() { }
        public ApiValidationException(string message) { }
        public ApiValidationException(string message, System.Exception innerException) { }
        protected ApiValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public class DeletemeWatcher : Orc.NuGetExplorer.PackageManagerWatcherBase
    {
        public DeletemeWatcher(Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService) { }
        protected override void OnOperationFinished(object sender, Orc.NuGetExplorer.PackageOperationEventArgs e) { }
    }
    public class static DispatchHelper
    {
        public static System.Threading.Tasks.Task DispatchIfNecessaryAsync(System.Action action) { }
    }
    public enum FeedVerificationResult
    {
        Unknown = 0,
        Valid = 1,
        AuthenticationRequired = 2,
        Invalid = 3,
    }
    public interface IApiPackageRegistry
    {
        bool IsRegistered(string packageName);
        void Register(string packageName, string version);
        void Validate(Orc.NuGetExplorer.IPackageDetails package);
    }
    public interface IDefaultPackageSourcesProvider
    {
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> GetDefaultPackages();
    }
    public interface IFileSystemService
    {
        void CopyDirectory(string sourceDirectory, string destinationDirectory);
        bool DeleteDirectory(string path);
    }
    public interface INuGetConfigurationService
    {
        void DisablePackageSource(string name, string source);
        string GetDestinationFolder();
        bool GetIsPrereleaseAllowed(Orc.NuGetExplorer.IRepository repository);
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> LoadPackageSources(bool onlyEnabled = False);
        bool SavePackageSource(string name, string source, bool isEnabled = True, bool isOfficial = True, bool verifyFeed = True);
        void SavePackageSources(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageSource> packageSources);
        void SetDestinationFolder(string value);
        void SetIsPrereleaseAllowed(Orc.NuGetExplorer.IRepository repository, bool value);
    }
    public class static INuGetConfigurationServiceExtensions { }
    public interface INuGetFeedVerificationService
    {
        Orc.NuGetExplorer.FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = True);
    }
    public class static INuGetFeedVerificationServiceExtensions { }
    public interface INuGetLogListeningSevice
    {
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Debug;
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Error;
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Info;
        public event System.EventHandler<Orc.NuGetExplorer.NuGetLogRecordEventArgs> Warning;
        void SendDebug(string message);
        void SendError(string message);
        void SendInfo(string message);
        void SendWarning(string message);
    }
    public interface IPackageBatchService
    {
        void ShowPackagesBatch(System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageDetails> packageDetails, Orc.NuGetExplorer.PackageOperationType operationType);
    }
    public interface IPackageDetails
    {
        System.Collections.Generic.IEnumerable<string> Authors { get; }
        string Dependencies { get; }
        string Description { get; }
        System.Nullable<int> DownloadCount { get; }
        string FullName { get; }
        System.Uri IconUrl { get; }
        string Id { get; }
        bool IsAbsoluteLatestVersion { get; }
        System.Nullable<bool> IsInstalled { get; set; }
        bool IsLatestVersion { get; }
        bool IsPrerelease { get; }
        System.Nullable<System.DateTimeOffset> Published { get; }
        string SpecialVersion { get; }
        string Title { get; }
        Catel.Data.IValidationContext ValidationContext { get; }
        System.Version Version { get; }
        void ResetValidationContext();
    }
    public interface IPackageOperationContext
    {
        System.Collections.Generic.IList<System.Exception> CatchedExceptions { get; }
        Orc.NuGetExplorer.ITemporaryFileSystemContext FileSystemContext { get; set; }
        Orc.NuGetExplorer.IPackageOperationContext Parent { get; set; }
        Orc.NuGetExplorer.IRepository Repository { get; set; }
    }
    public interface IPackageOperationContextService
    {
        Orc.NuGetExplorer.IPackageOperationContext CurrentContext { get; }
        public event System.EventHandler<Orc.NuGetExplorer.OperationContextEventArgs> OperationContextDisposing;
        System.IDisposable UseOperationContext(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
    }
    public interface IPackageOperationNotificationService
    {
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationEventArgs> OperationFinished;
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationBatchEventArgs> OperationsBatchStarting;
        public event System.EventHandler<Orc.NuGetExplorer.PackageOperationEventArgs> OperationStarting;
        void NotifyOperationBatchFinished(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
        void NotifyOperationBatchStarting(Orc.NuGetExplorer.PackageOperationType operationType, params Orc.NuGetExplorer.IPackageDetails[] packages);
        void NotifyOperationFinished(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
        void NotifyOperationStarting(string installPath, Orc.NuGetExplorer.PackageOperationType operationType, Orc.NuGetExplorer.IPackageDetails packageDetails);
    }
    public interface IPackageOperationService
    {
        void InstallPackage(Orc.NuGetExplorer.IPackageDetails package, bool allowedPrerelease);
        void UninstallPackage(Orc.NuGetExplorer.IPackageDetails package);
        void UpdatePackages(Orc.NuGetExplorer.IPackageDetails package, bool allowedPrerelease);
    }
    public interface IPackageQueryService
    {
        int CountPackages(Orc.NuGetExplorer.IRepository packageRepository, string filter, bool allowPrereleaseVersions);
        int CountPackages(Orc.NuGetExplorer.IRepository packageRepository, string packageId);
        int CountPackages(Orc.NuGetExplorer.IRepository packageRepository, Orc.NuGetExplorer.IPackageDetails packageDetails);
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageDetails> GetPackages(Orc.NuGetExplorer.IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10);
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageDetails> GetVersionsOfPackage(Orc.NuGetExplorer.IRepository packageRepository, Orc.NuGetExplorer.IPackageDetails package, bool allowPrereleaseVersions, ref int skip, int minimalTake = 10);
    }
    public class static IPackageQueryServiceExtensions { }
    public class static IPackagesBatchServiceExtensions { }
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
    public interface IPackagesUIService
    {
        System.Threading.Tasks.Task ShowPackagesExplorerAsync();
    }
    public interface IPackagesUpdatesSearcherService
    {
        System.Collections.Generic.IEnumerable<Orc.NuGetExplorer.IPackageDetails> SearchForUpdates(System.Nullable<bool> allowPrerelease = null, bool authenticateIfRequired = True);
    }
    public class static IPackagesUpdatesSearcherServiceExtensions { }
    public interface IPleaseWaitInterruptService
    {
        System.IDisposable InterruptTemporarily();
    }
    public interface IRepository
    {
        string Name { get; }
        Orc.NuGetExplorer.PackageOperationType OperationType { get; }
        string Source { get; }
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
    public interface ITemporaryFileSystemContext : System.IDisposable
    {
        string RootDirectory { get; }
        string GetDirectory(string relativeDirectoryName);
        string GetFile(string relativeFilePath);
    }
    public class NuGetLogRecordEventArgs : System.EventArgs
    {
        public NuGetLogRecordEventArgs(string message) { }
        public string Message { get; }
    }
    public class NuGetSettingsCredentialProvider : NuGet.SettingsCredentialProvider
    {
        public NuGetSettingsCredentialProvider(NuGet.ICredentialProvider credentialProvider, NuGet.IPackageSourceProvider packageSourceProvider) { }
        public NuGetSettingsCredentialProvider(NuGet.ICredentialProvider credentialProvider, NuGet.IPackageSourceProvider packageSourceProvider, NuGet.ILogger logger) { }
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
    public abstract class PackageManagerLogListenerBase
    {
        public PackageManagerLogListenerBase(Orc.NuGetExplorer.INuGetLogListeningSevice nuGetLogListeningSevice) { }
        protected virtual void OnDebug(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected virtual void OnError(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected virtual void OnInfo(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
        protected virtual void OnWarning(object sender, Orc.NuGetExplorer.NuGetLogRecordEventArgs e) { }
    }
    public abstract class PackageManagerWatcherBase
    {
        public PackageManagerWatcherBase(Orc.NuGetExplorer.IPackageOperationNotificationService packageOperationNotificationService) { }
        protected virtual void OnOperationFinished(object sender, Orc.NuGetExplorer.PackageOperationEventArgs e) { }
        protected virtual void OnOperationsBatchFinished(object sender, Orc.NuGetExplorer.PackageOperationBatchEventArgs e) { }
        protected virtual void OnOperationsBatchStarting(object sender, Orc.NuGetExplorer.PackageOperationBatchEventArgs e) { }
        protected virtual void OnOperationStarting(object sender, Orc.NuGetExplorer.PackageOperationEventArgs e) { }
    }
    public class PackageOperationBatchEventArgs : System.ComponentModel.CancelEventArgs
    {
        public Orc.NuGetExplorer.PackageOperationType OperationType { get; }
        public Orc.NuGetExplorer.IPackageDetails[] Packages { get; }
    }
    public class PackageOperationEventArgs : System.ComponentModel.CancelEventArgs
    {
        public string InstallPath { get; }
        public Orc.NuGetExplorer.IPackageDetails PackageDetails { get; }
        public Orc.NuGetExplorer.PackageOperationType PackageOperationType { get; }
    }
    public enum PackageOperationType
    {
        Install = 0,
        Uninstall = 1,
        Update = 2,
    }
    public class PackageWrapper : NuGet.IPackage, NuGet.IPackageMetadata, NuGet.IPackageName, NuGet.IServerPackageMetadata
    {
        public PackageWrapper(NuGet.IPackage package, System.Collections.Generic.IEnumerable<NuGet.PackageDependencySet> dependencySets) { }
        public System.Collections.Generic.IEnumerable<NuGet.IPackageAssemblyReference> AssemblyReferences { get; }
        public System.Collections.Generic.IEnumerable<string> Authors { get; }
        public string Copyright { get; }
        public System.Collections.Generic.IEnumerable<NuGet.PackageDependencySet> DependencySets { get; }
        public string Description { get; }
        public bool DevelopmentDependency { get; }
        public int DownloadCount { get; }
        public System.Collections.Generic.IEnumerable<NuGet.FrameworkAssemblyReference> FrameworkAssemblies { get; }
        public System.Uri IconUrl { get; }
        public string Id { get; }
        public bool IsAbsoluteLatestVersion { get; }
        public bool IsLatestVersion { get; }
        public string Language { get; }
        public System.Uri LicenseUrl { get; }
        public bool Listed { get; }
        public System.Version MinClientVersion { get; }
        public System.Collections.Generic.IEnumerable<string> Owners { get; }
        public System.Collections.Generic.ICollection<NuGet.PackageReferenceSet> PackageAssemblyReferences { get; }
        public System.Uri ProjectUrl { get; }
        public System.Nullable<System.DateTimeOffset> Published { get; }
        public string ReleaseNotes { get; }
        public System.Uri ReportAbuseUrl { get; }
        public bool RequireLicenseAcceptance { get; }
        public string Summary { get; }
        public string Tags { get; }
        public string Title { get; }
        public NuGet.SemanticVersion Version { get; }
        public void ExtractContents(NuGet.IFileSystem fileSystem, string extractPath) { }
        public System.Collections.Generic.IEnumerable<NuGet.IPackageFile> GetFiles() { }
        public System.IO.Stream GetStream() { }
        public System.Collections.Generic.IEnumerable<System.Runtime.Versioning.FrameworkName> GetSupportedFrameworks() { }
    }
    public class Repository : Orc.NuGetExplorer.IRepository
    {
        public Repository() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public Orc.NuGetExplorer.PackageOperationType OperationType { get; set; }
        public string Source { get; set; }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
    }
    public class static RepositoryName
    {
        public const string All = "All";
    }
    public class static Settings
    {
        public class static NuGet
        {
            public const string DestinationFolder = "DestFolder";
            public const string PackageSources = "PackageSources";
        }
    }
    public class static StringExtensions
    {
        public static string GetSafeScopeName(this string value) { }
    }
    public class static ValidationTags
    {
        public const string Api = "API";
    }
}
namespace Orc.NuGetExplorer.Native
{
    
    public class CredentialException : System.ComponentModel.Win32Exception
    {
        public CredentialException() { }
        public CredentialException(int error) { }
        public CredentialException(string message) { }
        public CredentialException(int error, string message) { }
        public CredentialException(string message, System.Exception innerException) { }
        protected CredentialException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public class static User32
    {
        public static System.IntPtr GetActiveWindow() { }
    }
}
namespace Orc.NuGetExplorer.Scopes
{
    
    public class AuthenticationScope : Catel.Disposable
    {
        public AuthenticationScope(System.Nullable<bool> canPromptForAuthentication = null) { }
        public bool CanPromptForAuthentication { get; }
        public bool HasPromptedForAuthentication { get; set; }
    }
}