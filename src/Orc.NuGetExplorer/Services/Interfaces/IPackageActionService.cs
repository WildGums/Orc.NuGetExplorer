// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageActionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using NuGet;

    internal interface IPackageActionService
    {
        #region Methods
        string GetActionName(RepositoryCategoryType repositoryCategory);
        Task Execute(RepositoryCategoryType repositoryCategory, IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease);
        bool CanExecute(RepositoryCategoryType repositoryCategory, PackageDetails packageDetails);
        bool IsRefreshReqired(RepositoryCategoryType repositoryCategory);
        #endregion
    }
}