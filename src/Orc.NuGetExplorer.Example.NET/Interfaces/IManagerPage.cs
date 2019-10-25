
namespace Orc.NuGetExplorer
{
    using Catel.Collections;
    using Models;
    using NuGetExplorer.ViewModels;

    public interface IManagerPage
    {
        FastObservableCollection<NuGetPackage> PackageItems { get; }

        void StartLoadingTimerOrInvalidateData();
    }
}
