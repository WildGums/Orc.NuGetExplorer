namespace Orc.NuGetExplorer.Management;

using System;
using Catel.IoC;

internal class DefaultExtensibleProjectProvider : IDefaultExtensibleProjectProvider
{
    private readonly IExtensibleProject _defaultProject;

    public DefaultExtensibleProjectProvider(ITypeFactory typeFactory, INuGetConfigurationService configurationService, IExtensibleProjectLocator extensibleProjectLocator)
    {
        ArgumentNullException.ThrowIfNull(typeFactory);
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(extensibleProjectLocator);

        _defaultProject = typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<DestFolder>(configurationService.GetDestinationFolder());

        extensibleProjectLocator.Register(_defaultProject);
        extensibleProjectLocator.Enable(_defaultProject);
    }

    public IExtensibleProject GetDefaultProject()
    {
        return _defaultProject;
    }
}