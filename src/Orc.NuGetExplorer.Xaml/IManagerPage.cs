
namespace Orc.NuGetExplorer;

using System.Collections.ObjectModel;

internal interface IManagerPage
{
    ObservableCollection<NuGetPackage> PackageItems { get; }

    bool CanBatchUpdateOperations { get; }

    bool CanBatchInstallOperations { get; }

    void StartLoadingTimerOrInvalidateData();
}
