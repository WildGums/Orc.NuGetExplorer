namespace Orc.NuGetExplorer.Services
{
    using System.Threading.Tasks;

    public interface INuGetExplorerInitializationService
    {
        Task<bool> UpgradeNuGetPackagesIfNeededAsync();
    }
}
