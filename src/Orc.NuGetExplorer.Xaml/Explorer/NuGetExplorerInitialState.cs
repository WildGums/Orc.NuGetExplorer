namespace Orc.NuGetExplorer
{
    using Catel;

    public class NuGetExplorerInitialState : INuGetExplorerInitialState
    {
        public NuGetExplorerInitialState(ExplorerTab tab, PackageSearchParameters? packageSearchParameters = null)
        {
            Argument.IsNotNull(() => tab);

            Tab = tab;
            InitialSearchParameters = packageSearchParameters;
        }

        public ExplorerTab Tab { get; }
        public PackageSearchParameters? InitialSearchParameters { get; }
    }
}
