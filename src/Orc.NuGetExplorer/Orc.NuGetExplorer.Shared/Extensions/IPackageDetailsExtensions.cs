// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageDetailsExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal static class IPackageDetailsExtensions
    {
        #region Methods
        public static IPackage ToNuGetPackage(this IPackageDetails package)
        {
            return ((PackageDetails) package).Package;
        }
        #endregion
    }
}