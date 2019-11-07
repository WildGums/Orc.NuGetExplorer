namespace Orc.NuGetExplorer.Management
{
    using Catel;
    using Catel.IoC;

    internal class DefaultExtensibleProjectProvider : IDefaultExtensibleProjectProvider
    {
        private readonly INuGetConfigurationService _configurationService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly ITypeFactory _typeFactory;

        private IExtensibleProject _defaultProject;

        public DefaultExtensibleProjectProvider(INuGetConfigurationService configurationService, IExtensibleProjectLocator extensibleProjectLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => typeFactory);

            _configurationService = configurationService;
            _extensibleProjectLocator = extensibleProjectLocator;
            _typeFactory = typeFactory;

            CreateAndRegisterDefaultProject();
        }

        public IExtensibleProject GetDefaultProject()
        {
            return _defaultProject;
        }

        private void CreateAndRegisterDefaultProject()
        {
            _defaultProject = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<DestFolder>(_configurationService.GetDestinationFolder());
            _extensibleProjectLocator.Register(_defaultProject);
            _extensibleProjectLocator.Enable(_defaultProject);
        }
    }
}
