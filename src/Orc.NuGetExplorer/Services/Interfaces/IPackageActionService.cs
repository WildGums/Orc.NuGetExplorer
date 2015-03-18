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
        Task Execute(PackageOperationType operationType, PackageDetails packageDetails, IPackageRepository sourceRepository = null, bool allowedPrerelease = false);
        bool CanExecute(PackageOperationType operationType, PackageDetails package);
        bool IsRefreshReqired(PackageOperationType operationType);
        string GetPluralActionName(PackageOperationType operationType);
        #endregion
    }
}