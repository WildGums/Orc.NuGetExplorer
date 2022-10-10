namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPackageCommandService
    {
        #region Methods
        string GetActionName(PackageOperationType operationType);

        [ObsoleteEx(TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0", ReplacementTypeOrMember = "ExecuteAsync")]
        Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails, IRepository sourceRepository = null, bool allowedPrerelease = false);
        Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails);
        Task<bool> CanExecuteAsync(PackageOperationType operationType, IPackageDetails package);

        [ObsoleteEx(TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        bool IsRefreshRequired(PackageOperationType operationType);
        [ObsoleteEx(TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        string GetPluralActionName(PackageOperationType operationType);

        Task ExecuteInstallAsync(IPackageDetails packageDetails, CancellationToken token);
        Task ExecuteInstallAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token);
        Task ExecuteUninstallAsync(IPackageDetails packageDetails, CancellationToken token);
        Task ExecuteUpdateAsync(IPackageDetails packageDetails, CancellationToken token);
        Task ExecuteUpdateAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token);
        #endregion
    }
}
