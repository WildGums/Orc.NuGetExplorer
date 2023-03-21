namespace Orc.NuGetExplorer;

public static class RepositoryName
{
    public const string All = "All";
}

public static class Settings
{
    public static class NuGet
    {
        // Note: should have been NuGet.DestinationFolder string and the member should have been DestinationFolder. We cannot
        // change this because we already took a dependency on this, but next time please follow the naming conventions so we
        // know to what extension / Orc.* package a setting belongs to
        public const string DestinationFolder = "DestFolder";

        public const string PackageSources = "PackageSources";

        public const string FallbackUrl = "Plugins.FeedUrl";

        public const string IncludePrereleasePackages = "NuGetExplorer.IncludePrerelease";
        public const string HideInstalledPackages = "NuGetExplorer.HideInstalled";

        public const string CredentialStorage = "NuGetExplorer.CredentialStoragePolicy";

        public const int PackageCount = 200;
    }
}

public static class ValidationTags
{
    public const string Api = "API";
}

public static class Constants
{
    public const string DefaultNuGetOrgUri = "https://api.nuget.org/v3/index.json";

    public const string DefaultNuGetOrgName = "nuget.org";

    public const string ConfigKeySeparator = "|";

    public const string PackageInstallationConflictMessage = "Conflict during package installation";

    public const string ProductName = "NuGetPackageManager";

    public const string CompanyName = "WildGums";

    public const string PackageManagement = "Package management";

    public const string NotInstalled = "not installed";

    public const string CombinedSourceName = "All";

    public static class Messages
    {
        public const string CacheClearEndedSuccessful = "NuGet cache cleared";
        public const string CachedClearEndedWithError = "NuGet cache cleared with some errors. It means some cached files may kept on cache folder";
        public const string CacheClearFailed = "Fatal error during cache clearing";

        public const string PackageParserInvalidIdentity = "parameter doesn't contain valid package identity";
        public const string PackageParserInvalidVersion = "parameter doesn't contain valid package version";
    }

    public static class Log
    {
        public const string GetHttpRequestInfoPattern = "  GET https";
        public const string SetHttpRequestInfoPattern = "  SET https";
        public const string OkHttpRequestInfoPattern = "  OK https";
        public const string NotFoundHttpRequestInfoPattern = "  NotFound https";
        public const string CacheHttpRequestInfoPattern = "  CACHE https";
    }
}