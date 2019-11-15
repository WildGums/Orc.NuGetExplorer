// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;

    public interface IPackageOperationService
    {
        #region Methods
        Task UninstallPackageAsync(IPackageDetails package);
        Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease);
        Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease);
        #endregion
    }
}
