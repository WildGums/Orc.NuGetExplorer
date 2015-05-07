// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryCacheService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using NuGet;

    internal interface IRepositoryCacheService
    {
        #region Methods
        IRepository GetSerialisableRepository(string name, PackageOperationType operationType, Func<IPackageRepository> packageRepositoryFactory, bool renew = false);
        IPackageRepository GetNuGetRepository(IRepository repository);
        #endregion
    }
}