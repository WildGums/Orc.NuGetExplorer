using Orc.NuGetExplorer.ViewModels;

namespace Orc.NuGetExplorer
{
    using Catel.Collections;
    using NuGetExplorer.ViewModels;

    public interface IManagerPage
    {
        FastObservableCollection<PackageDetailsViewModel> PackageItems { get; }

        void StartLoadingTimerOrInvalidateData();
    }
}
