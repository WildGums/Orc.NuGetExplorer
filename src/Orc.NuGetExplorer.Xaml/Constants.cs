// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    internal static class RepositoryCategoryName
    {
        public const string Installed = "Installed";
        public const string Online = "Online";
        public const string Update = "Update";
    }

    internal static class ExplorerPageName
    {
        public const string Browse = "Browse";
        public const string Installed = "Installed";
        public const string Updates = "Updates";
    }

    internal static class DefaultName
    {
        public const string PackageSourceFeed = "https://api.nuget.org/v3/index.json";
        public const string PackageSourceName = "PackageSource";
    }

    internal static class ResourcePaths
    {
        public const string PackageDefaultIcon = "pack://application:,,,/Orc.NuGetExplorer.Xaml;component/Resources/Images/packageDefaultIcon.png";
    }

    internal static class AppSettings
    {
        internal static class NuGetExplorer
        {
            public const string LastRepositoryCaregory = "NuGetExplorer.LastRepositoryCaregory";
            public const string LastRepositoryCaregoryDefaultValue = RepositoryCategoryName.Installed;

            public const string LastRepository = "NuGetExplorer.LastRepository";
            public const string LastRepositoryDefaultValue = RepositoryName.All;
        }
    }
}
