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
        string GetActionName(PackageOperationType operationType);
        Task Execute(PackageOperationType operationType, IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease);
        bool CanExecute(PackageOperationType operationType, PackageDetails packageDetails);
        bool IsRefreshReqired(PackageOperationType operationType);
        #endregion
    }
}