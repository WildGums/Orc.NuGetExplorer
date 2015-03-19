// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public static class RepoName
    {
        #region Fields
        public const string All = "All";
        #endregion
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
        }
    }
}