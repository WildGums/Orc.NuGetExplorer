// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageRepositoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackageRepositoryService
    {
        #region Properties
        IRepository LocalRepository { get; }
        #endregion

        #region Methods
        IDictionary<string, IRepository> GetRepositories(PackageOperationType packageOperationType);
        IDictionary<string, IRepository> GetSourceRepositories();
        IRepository GetSourceAggregateRepository();
        IDictionary<string, IRepository> GetUpdateRepositories();
        IRepository GetUpdateAggeregateRepository();
        #endregion
    }
}