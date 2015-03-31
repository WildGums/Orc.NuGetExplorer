// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IPackageOperationService
    {
        #region Methods
        void UninstallPackage(IPackageDetails package);
        void InstallPackage(IPackageDetails package, bool allowedPrerelease);
        void UpdatePackages(IPackageDetails package, bool allowedPrerelease);
        #endregion
    }
}