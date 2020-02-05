namespace Orc.NuGetExplorer
{
    public interface INuGetExplorerInitialState
    {
        public ExplorerTab Tab { get; }

        public PackageSearchParameters InitialSearchParameters { get; }
    }
}
