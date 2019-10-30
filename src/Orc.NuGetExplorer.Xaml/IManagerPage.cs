
namespace Orc.NuGetExplorer
{
    using Catel.Collections;
    using Models;

    public interface IManagerPage
    {
        FastObservableCollection<NuGetPackage> PackageItems { get; }

        void StartLoadingTimerOrInvalidateData();
    }
}
