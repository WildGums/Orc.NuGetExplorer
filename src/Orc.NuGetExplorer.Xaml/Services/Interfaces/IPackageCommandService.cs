// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCommandService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPackageCommandService
    {
        #region Methods
        Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails);
        Task<bool> CanExecuteAsync(PackageOperationType operationType, IPackageDetails package);

        Task ExecuteInstallAsync(IPackageDetails packageDetails, CancellationToken token);
        Task ExecuteInstallAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token);
        Task ExecuteUninstallAsync(IPackageDetails packageDetails, CancellationToken token);
        Task ExecuteUpdateAsync(IPackageDetails packageDetails, CancellationToken token);
        Task ExecuteUpdateAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token);
        #endregion
    }
}
