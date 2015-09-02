// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCommandService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;

    internal interface IPackageCommandService
    {
        #region Methods
        string GetActionName(PackageOperationType operationType);
        void Execute(PackageOperationType operationType, IPackageDetails packageDetails, IRepository sourceRepository = null, bool allowedPrerelease = false);
        bool CanExecute(PackageOperationType operationType, IPackageDetails package);
        bool IsRefreshRequired(PackageOperationType operationType);
        string GetPluralActionName(PackageOperationType operationType);
        #endregion
    }
}