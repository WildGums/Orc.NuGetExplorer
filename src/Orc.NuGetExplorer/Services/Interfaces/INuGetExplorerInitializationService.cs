namespace Orc.NuGetExplorer.Services;

using System.Threading.Tasks;

public interface INuGetExplorerInitializationService
{
    string DefaultSourceKey { get; }

    int PackageQuerySize { get; set; }

    Task<bool> UpgradeNuGetPackagesIfNeededAsync();
}