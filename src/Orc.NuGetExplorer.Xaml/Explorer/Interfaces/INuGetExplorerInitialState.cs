namespace Orc.NuGetExplorer;

public interface INuGetExplorerInitialState
{
    ExplorerTab Tab { get; }

    PackageSearchParameters? InitialSearchParameters { get; }
}