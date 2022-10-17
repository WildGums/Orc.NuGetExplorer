namespace Orc.NuGetExplorer
{
    public class NuGetExplorerInitialState : INuGetExplorerInitialState
    {
        public NuGetExplorerInitialState(ExplorerTab tab, PackageSearchParameters? packageSearchParameters = null)
        {
            Tab = tab;
            InitialSearchParameters = packageSearchParameters;
        }

        public ExplorerTab Tab { get; }
        public PackageSearchParameters? InitialSearchParameters { get; }
    }
}
