// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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