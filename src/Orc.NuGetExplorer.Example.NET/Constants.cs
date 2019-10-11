namespace Orc.NuGetExplorer
{
    public static class Constants
    {
        public const string NamePlaceholder = "Package source";

        public const string SourcePlaceholder = "http://packagesource";

        public const string DefaultNugetOrgUri = "https://api.nuget.org/v3/index.json";

        public const string DefaultNugetOrgName = "nuget.org";

        public const string ConfigKeySeparator = "|";

        public const string PackageInstallationConflictMessage = "Conflict during package installation";

        public const string ProductName = "NuGetPackageManager";

        public const string CompanyName = "WildGums";

        public const string PackageManagement = "Package management";

        public const string NotInstalled = "not installed";

        public static class Messages
        {
            public const string CacheClearEndedSuccessful = "NuGet cache cleared";
            public const string CachedClearEndedWithError = "NuGet cache cleared with some errors. Is means some cached files  ";
            public const string CacheClearFailed = "Fatal error during cache clearing";
        }
    }
}
