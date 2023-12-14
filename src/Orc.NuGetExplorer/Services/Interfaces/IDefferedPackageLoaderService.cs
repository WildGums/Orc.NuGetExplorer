namespace Orc.NuGetExplorer.Services;

using System.Threading.Tasks;
using NuGetExplorer.Pagination;

public interface IDefferedPackageLoaderService
{
    void Add(DeferToken token);

    Task StartLoadingAsync();
}