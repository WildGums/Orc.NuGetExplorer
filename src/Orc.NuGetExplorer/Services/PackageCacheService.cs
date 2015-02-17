// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCacheService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Caching;
    using Catel.Caching.Policies;
    using NuGet;

    public class PackageCacheService : IPackageCacheService
    {
        private readonly ICacheStorage<string, PackageDetails> _packageDetailsCache = new CacheStorage<string, PackageDetails>(() => ExpirationPolicy.Duration(TimeSpan.FromSeconds(60)));

        public PackageCacheService()
        {
            
        }

        public PackageDetails GetPackageDetails(IPackage package)
        {
            Argument.IsNotNull(() => package);

            return _packageDetailsCache.GetFromCacheOrFetch(package.Id, () => new PackageDetails(package));
        }
    }
}