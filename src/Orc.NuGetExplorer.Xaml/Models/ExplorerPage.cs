namespace Orc.NuGetExplorer;

using Catel.Data;

internal class ExplorerPage : ModelBase
{
    public ExplorerPage(INuGetExplorerInitialState parameters)
    {
        Parameters = parameters;
    }

    public bool IsActive { get; set; }

    public INuGetExplorerInitialState Parameters { get; set; }
}