// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageDetailsExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    public static class IPackageDetailsExtensions
    {
        public static IPackage ToNuGetPackage(this IPackageDetails package)
        {
            return ((PackageDetails) package).Package;
        }
    }
}