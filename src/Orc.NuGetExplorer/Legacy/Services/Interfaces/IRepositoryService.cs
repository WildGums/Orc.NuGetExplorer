// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System.Collections.Generic;

    public interface IRepositoryService
    {
        #region Properties
        IRepository LocalRepository { get; }
        #endregion

        #region Methods
        IEnumerable<IRepository> GetRepositories(PackageOperationType packageOperationType);
        IEnumerable<IRepository> GetSourceRepositories();
        IRepository GetSourceAggregateRepository();
        IEnumerable<IRepository> GetUpdateRepositories();
        IRepository GetUpdateAggeregateRepository();
        #endregion
    }
}