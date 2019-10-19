// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCommandService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IPackageCommandService
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