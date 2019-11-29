namespace Orc.NuGetExplorer.Services
{
    using System.Threading.Tasks;

    public interface INuGetExplorerInitializationService
    {
        string DefaultSourceKey { get; }

        Task<bool> UpgradeNuGetPackagesIfNeededAsync();
    }
}
