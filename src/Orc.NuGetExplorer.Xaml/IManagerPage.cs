
namespace Orc.NuGetExplorer
{
    using Catel.Collections;
    using Models;

    internal interface IManagerPage
    {
        FastObservableCollection<NuGetPackage> PackageItems { get; }

        void StartLoadingTimerOrInvalidateData();
    }
}
