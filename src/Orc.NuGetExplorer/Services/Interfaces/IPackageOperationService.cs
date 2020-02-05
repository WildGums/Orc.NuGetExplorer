// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPackageOperationService
    {
        #region Methods
        Task UninstallPackageAsync(IPackageDetails package, CancellationToken token = default);
        Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease = false, CancellationToken token = default);
        Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease = false, CancellationToken token = default);
        #endregion
    }
}
