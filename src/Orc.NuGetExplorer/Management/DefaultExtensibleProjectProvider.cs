namespace Orc.NuGetExplorer.Management
{
    using System;
    using Catel.IoC;

    internal class DefaultExtensibleProjectProvider : IDefaultExtensibleProjectProvider
    {
        private readonly INuGetConfigurationService _configurationService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly ITypeFactory _typeFactory;

        private readonly IExtensibleProject _defaultProject;

        public DefaultExtensibleProjectProvider(ITypeFactory typeFactory, INuGetConfigurationService configurationService, IExtensibleProjectLocator extensibleProjectLocator)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(extensibleProjectLocator);

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
