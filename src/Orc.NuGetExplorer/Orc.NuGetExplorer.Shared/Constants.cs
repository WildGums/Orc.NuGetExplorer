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
}