// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryCacheService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Caching;
    using Catel.Logging;
    using NuGet;

    internal class RepositoryCacheService : IRepositoryCacheService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly ICacheStorage<string, Tuple<IRepository, IPackageRepository>> _repositoryCache = new CacheStorage<string, Tuple<IRepository, IPackageRepository>>();
        #endregion

        #region Methods
        public IRepository GetSerialisableRepository(string name, PackageOperationType operationType, IPackageRepository packageRepository)
        {
            Argument.IsNotNull(() => packageRepository);

            var key = GetKey(operationType, name);

            return _repositoryCache.GetFromCacheOrFetch(key, () =>
            {
                var repository = new Repository
                {
                    Name = name,
                    OperationType = operationType
                };

                return new Tuple<IRepository, IPackageRepository>(repository, packageRepository);
            }).Item1;
        }

        public IPackageRepository GetNuGetRepository(IRepository repository)
        {
            var key = GetKey(repository.OperationType, repository.Name);

            return _repositoryCache.Get(key).Item2;
        }

        private static string GetKey(PackageOperationType operationType, string name)
        {
            return string.Format("{0}_{1}", operationType, name);
        }
        #endregion
    }
}