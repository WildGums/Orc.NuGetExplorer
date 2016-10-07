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
        [ObsoleteEx(ReplacementTypeOrMember = "GetSerializableRepository", TreatAsErrorFromVersion = "1.1", RemoveInVersion = "2.0")]
        IRepository GetSerialisableRepository(string name, PackageOperationType operationType, Func<IPackageRepository> packageRepositoryFactory, bool renew = false);

        IRepository GetSerializableRepository(string name, string source, PackageOperationType operationType, Func<IPackageRepository> packageRepositoryFactory, bool renew = false);
        IPackageRepository GetNuGetRepository(IRepository repository);
        #endregion
    }
}