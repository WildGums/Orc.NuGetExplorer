namespace Orc.NuGetExplorer.Management
{
    using Catel;
    using Catel.IoC;

    internal class DefaultExtensibleProjectProvider : IDefaultExtensibleProjectProvider
    {
        private readonly INuGetConfigurationService _configurationService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly ITypeFactory _typeFactory;

        private readonly IExtensibleProject _defaultProject;

        public DefaultExtensibleProjectProvider(ITypeFactory typeFactory, INuGetConfigurationService configurationService, IExtensibleProjectLocator extensibleProjectLocator)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => extensibleProjectLocator);

            _configurationService = configurationService;
            _extensibleProjectLocator = extensibleProjectLocator;
            _typeFactory = typeFactory;

            _defaultProject = _typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<DestFolder>(_configurationService.GetDestinationFolder());
            
            _extensibleProjectLocator.Register(_defaultProject);
            _extensibleProjectLocator.Enable(_defaultProject);
        }

        public IExtensibleProject GetDefaultProject()
        {
            return _defaultProject;
        }
    }
}
