namespace Orc.NuGetExplorer
{
    using Catel.Data;

    public class ExplorerPage : ObservableObject
    {
        public ExplorerPage(INuGetExplorerInitialState parameters)
        {
            Parameters = parameters;
        }

        public bool IsActive { get; set; }

        public INuGetExplorerInitialState Parameters { get; set; }
    }
}
