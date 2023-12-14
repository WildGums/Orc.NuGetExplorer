namespace Orc.NuGetExplorer;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface IPackageCommandService
{
    string GetActionName(PackageOperationType operationType);
    Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails);
    Task<bool> CanExecuteAsync(PackageOperationType operationType, IPackageDetails package);

    Task ExecuteInstallAsync(IPackageDetails packageDetails, CancellationToken token);
    Task ExecuteInstallAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token);
    Task ExecuteUninstallAsync(IPackageDetails packageDetails, CancellationToken token);
    Task ExecuteUpdateAsync(IPackageDetails packageDetails, CancellationToken token);
    Task ExecuteUpdateAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token);
}