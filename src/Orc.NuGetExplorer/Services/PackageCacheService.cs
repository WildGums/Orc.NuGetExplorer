// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCacheService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Caching;
    using Catel.Caching.Policies;
    using NuGet;

    internal class PackageCacheService : IPackageCacheService
    {
        #region Fields
        private readonly ICacheStorage<string, PackageDetails> _packageDetailsCache = new CacheStorage<string, PackageDetails>(() => ExpirationPolicy.Duration(TimeSpan.FromSeconds(60)));
        #endregion

        #region Constructors
        public PackageCacheService()
        {
        }
        #endregion

        #region Methods
        public PackageDetails GetPackageDetails(IPackage package)
        {
            Argument.IsNotNull(() => package);

            return _packageDetailsCache.GetFromCacheOrFetch(package.GetFullName(), () => new PackageDetails(package));
        }
        #endregion
    }
}