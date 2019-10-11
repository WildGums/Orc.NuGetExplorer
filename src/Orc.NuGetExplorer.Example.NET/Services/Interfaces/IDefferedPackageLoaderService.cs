using Orc.NuGetExplorer.Pagination;

namespace Orc.NuGetExplorer.Services
{
    using NuGetExplorer.Pagination;
    using System.Threading.Tasks;

    public interface IDefferedPackageLoaderService
    {
        void Add(DeferToken token);

        Task StartLoadingAsync();
    }
}
