namespace Orc.NuGetExplorer.Management;

using System;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;

internal class DefaultExtensibleProjectProvider : IDefaultExtensibleProjectProvider
{
    private readonly IExtensibleProject _defaultProject;

    public DefaultExtensibleProjectProvider(IServiceProvider serviceProvider, 
        INuGetConfigurationService configurationService, IExtensibleProjectLocator extensibleProjectLocator)
    {
        _defaultProject = ActivatorUtilities.CreateInstance<DestFolder>(serviceProvider, configurationService.GetDestinationFolder());

        extensibleProjectLocator.Register(_defaultProject);
        extensibleProjectLocator.Enable(_defaultProject);
    }

    public IExtensibleProject GetDefaultProject()
    {
        return _defaultProject;
    }
}
