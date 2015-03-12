// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageActionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal interface IPackageActionService
    {
        string GetActionName(RepositoryCategoryType repositoryCategory);
        void Execute(RepositoryCategoryType repositoryCategory, IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease);
        bool CanExecute(RepositoryCategoryType repositoryCategory, PackageDetails packageDetails);
        bool IsRefreshReqired(RepositoryCategoryType repositoryCategory);
    }
}