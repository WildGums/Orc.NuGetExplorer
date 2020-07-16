namespace Orc.NuGetExplorer.Models
{
    using Catel.Data;

    public class ExplorerPage : ModelBase
    {
        public ExplorerPage(INuGetExplorerInitialState parameters)
        {
            Parameters = parameters;
        }

        public bool IsActive { get; set; }

        public INuGetExplorerInitialState Parameters { get; set; }
    }
}
