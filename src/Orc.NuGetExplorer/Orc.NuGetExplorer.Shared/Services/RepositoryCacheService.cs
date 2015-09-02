// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryCacheService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Logging;
    using NuGet;

    internal class RepositoryCacheService : IRepositoryCacheService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static int _idCounter;
        private readonly IDictionary<int, Tuple<IRepository, IPackageRepository>> _idTupleDictionary = new Dictionary<int, Tuple<IRepository, IPackageRepository>>();
        private readonly IDictionary<string, int> _keyIdDictionary = new Dictionary<string, int>();
        #endregion

        #region Methods
        public IRepository GetSerialisableRepository(string name, PackageOperationType operationType, Func<IPackageRepository> packageRepositoryFactory, bool renew = false)
        {
            Argument.IsNotNullOrEmpty(() => name);
            Argument.IsNotNull(() => packageRepositoryFactory);

            var key = GetKey(operationType, name);

            int id;
            if (_keyIdDictionary.TryGetValue(key, out id))
            {
                if (!renew)
                {
                    return _idTupleDictionary[id].Item1;
                }

                return CreateSerialisableRepository(name, operationType, packageRepositoryFactory, id);
            }

            id = _idCounter++;
            _keyIdDictionary.Add(key, id);

            return CreateSerialisableRepository(name, operationType, packageRepositoryFactory, id);
        }

        private IRepository CreateSerialisableRepository(string name, PackageOperationType operationType, Func<IPackageRepository> packageRepositoryFactory, int id)
        {
            Argument.IsNotNullOrEmpty(() => name);
            Argument.IsNotNull(() => packageRepositoryFactory);

            var repository = new Repository
            {
                Id = id,
                Name = name,
                OperationType = operationType
            };
            
            _idTupleDictionary[id] = new Tuple<IRepository, IPackageRepository>(repository, packageRepositoryFactory());

            return repository;
        }

        public IPackageRepository GetNuGetRepository(IRepository repository)
        {
            Argument.IsNotNull(() => repository);

            var id = ((Repository) repository).Id;

            return _idTupleDictionary[id].Item2;
        }

        private static string GetKey(PackageOperationType operationType, string name)
        {
            return string.Format("{0}_{1}", operationType, name);
        }
        #endregion
    }
}