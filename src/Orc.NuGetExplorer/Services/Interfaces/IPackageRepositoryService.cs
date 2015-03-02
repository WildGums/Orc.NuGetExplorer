// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageRepositoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using NuGet;

    public interface IPackageRepositoryService
    {
        #region Methods
        IDictionary<string, IPackageRepository> GetRepositories(RepoCategoryType category);
        IPackageRepository GetLocalRepository(string path);
        IDictionary<string, IPackageRepository> GetRemoteRepositories();
        IPackageRepository GetAggregateRepository();
        #endregion
    }
}