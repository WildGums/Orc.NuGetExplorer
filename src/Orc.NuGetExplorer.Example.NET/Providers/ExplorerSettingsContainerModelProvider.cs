using Catel.IoC;
using Orc.NuGetExplorer.Models;

namespace Orc.NuGetExplorer.Providers
{
    public class ExplorerSettingsContainerModelProvider : ModelProvider<ExplorerSettingsContainer>
    {
        private readonly ITypeFactory _typeFactory;

        public ExplorerSettingsContainerModelProvider(ITypeFactory typeFactory)
        {
            _typeFactory = typeFactory;

            //create instance of Model
            Model = _typeFactory.CreateInstance<ExplorerSettingsContainer>();
        }
    }
}
