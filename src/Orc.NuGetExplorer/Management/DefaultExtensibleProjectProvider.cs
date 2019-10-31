namespace Orc.NuGetExplorer.Management
{
    using Catel;

    internal class DefaultExtensibleProjectProvider : IDefaultExtensibleProjectProvider
    {
        private readonly INuGetConfigurationService _configurationService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;

        private IExtensibleProject _defaultProject;

        public DefaultExtensibleProjectProvider(INuGetConfigurationService configurationService, IExtensibleProjectLocator extensibleProjectLocator)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => extensibleProjectLocator);

            _configurationService = configurationService;
            _extensibleProjectLocator = extensibleProjectLocator;

            CreateAndRegisterDefaultProject();
        }

        public IExtensibleProject GetDefaultProject()
        {
            return _defaultProject;
        }

        private void CreateAndRegisterDefaultProject()
        {
            _defaultProject = new DestFolder(_configurationService.GetDestinationFolder());
            _extensibleProjectLocator.Register(_defaultProject);
        }
    }
}
