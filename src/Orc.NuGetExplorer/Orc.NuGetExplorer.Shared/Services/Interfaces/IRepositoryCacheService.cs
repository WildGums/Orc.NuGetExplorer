// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryCacheService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using NuGet;

    internal interface IRepositoryCacheService
    {
        #region Methods
        IRepository GetSerializableRepository(string name, string source, PackageOperationType operationType, Func<IPackageRepository> packageRepositoryFactory, bool renew = false);
        IPackageRepository GetNuGetRepository(IRepository repository);
        #endregion
    }
}