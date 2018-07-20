// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet;

    internal static class IPackageExtensions
    {
        #region Methods
        public static bool IsPrerelease(this IPackage package)
        {
            Argument.IsNotNull(() => package);

            return !string.IsNullOrWhiteSpace(package.Version.SpecialVersion);
        }

        public static string GetKeyForCache(this IPackage package, bool allowPrereleaseVersions)
        {
            Argument.IsNotNull(() => package);

            return string.Format("{0}_{1}_{2}", package.GetType().Name, package.GetFullName(), allowPrereleaseVersions);
        }
        #endregion
    }
}