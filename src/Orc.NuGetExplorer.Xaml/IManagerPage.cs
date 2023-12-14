
namespace Orc.NuGetExplorer;

using Catel.Collections;

internal interface IManagerPage
{
    FastObservableCollection<NuGetPackage> PackageItems { get; }

    bool CanBatchUpdateOperations { get; }

    bool CanBatchInstallOperations { get; }

    void StartLoadingTimerOrInvalidateData();
}