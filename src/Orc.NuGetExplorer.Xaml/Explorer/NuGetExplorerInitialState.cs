namespace Orc.NuGetExplorer;

using System;

public class NuGetExplorerInitialState : INuGetExplorerInitialState
{
    public NuGetExplorerInitialState(ExplorerTab tab, PackageSearchParameters? packageSearchParameters = null)
    {
        ArgumentNullException.ThrowIfNull(tab);

        Tab = tab;
        InitialSearchParameters = packageSearchParameters;
    }

    public ExplorerTab Tab { get; }
    public PackageSearchParameters? InitialSearchParameters { get; }
}