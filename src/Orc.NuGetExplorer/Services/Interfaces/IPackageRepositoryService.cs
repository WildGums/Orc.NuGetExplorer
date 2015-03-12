// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageRepositoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using NuGet;

    internal interface IPackageRepositoryService
    {
        #region Properties
        IPackageRepository LocalRepository { get; }
        #endregion

        #region Methods
        IDictionary<string, IPackageRepository> GetRepositories(RepositoryCategoryType category);
        IDictionary<string, IPackageRepository> GetRemoteRepositories();
        IPackageRepository GetAggregateRepository();
        IDictionary<string, IPackageRepository> GetUpdateRepositories();
        #endregion
    }
}