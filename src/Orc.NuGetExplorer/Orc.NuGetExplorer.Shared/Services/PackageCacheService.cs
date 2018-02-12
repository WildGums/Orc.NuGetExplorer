﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCacheService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Catel;
    using Catel.Caching;
    using Catel.Logging;

    using NuGet;

    internal class PackageCacheService : IPackageCacheService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICacheStorage<string, PackageDetails> _packageDetailsCache = new CacheStorage<string, PackageDetails>();
        #endregion

        #region Constructors
        public PackageCacheService()
        {
        }
        #endregion

        #region Methods
        public PackageDetails GetPackageDetails(IPackageRepository packageRepository, IPackage package)
        {
            Argument.IsNotNull(() => package);

            return _packageDetailsCache.GetFromCacheOrFetch(package.GetKeyForCache(), () => new PackageDetails(package, packageRepository.FindPackagesById(package.Id).Select(p => p.Version.ToString())));
        }
        #endregion
    }
}