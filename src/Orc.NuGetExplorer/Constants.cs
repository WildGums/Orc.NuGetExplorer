// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public static class RepositoryName
    {
        #region Fields
        public const string All = "All";
        #endregion
    }

    public static class Settings
    {
        public static class NuGet
        {
            #region Fields
            // Note: should have been NuGet.DestinationFolder string and the member should have been DestinationFolder. We cannot
            // change this because we already took a dependency on this, but next time please follow the naming conventions so we
            // know to what extension / Orc.* package a setting belongs to
            public const string DestinationFolder = "DestFolder";

            public const string PackageSources = "PackageSources";

            public const string FallbackUrl = "Plugins.FeedUrl";

            public const int PackageCount = 200;

            #endregion
        }
    }

    public static class ValidationTags
    {
        #region Fields
        public const string Api = "API";
        #endregion
    }

    public static class Constants
    {
        public const string DefaultNugetOrgUri = "https://api.nuget.org/v3/index.json";

        public const string DefaultNugetOrgName = "nuget.org";

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
            public const string CachedClearEndedWithError = "NuGet cache cleared with some errors. Is means some cached files  ";
            public const string CacheClearFailed = "Fatal error during cache clearing";

            public const string PackageParserInvalidIdentity = "parameter doesn't contain valid package identity";
            public const string PackageParserInvalidVersion = "parameter doesn't contain valid package version";
        }
    }
}
