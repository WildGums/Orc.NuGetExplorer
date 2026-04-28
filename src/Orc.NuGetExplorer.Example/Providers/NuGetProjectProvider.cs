namespace Orc.NuGetExplorer.Example;

using System;
using Microsoft.Extensions.DependencyInjection;
using Orc.NuGetExplorer.Management;

public class NuGetProjectProvider : IDefaultExtensibleProjectProvider
{
    private readonly IExtensibleProjectLocator _extensibleProjectLocator;

    private readonly IExtensibleProject _defaultProject;

    public NuGetProjectProvider(IExtensibleProjectLocator extensibleProjectLocator, IServiceProvider serviceProvider)
    {
        _extensibleProjectLocator = extensibleProjectLocator;

        _defaultProject = ActivatorUtilities.CreateInstance<ExampleProject>(serviceProvider);

        _extensibleProjectLocator.Register(_defaultProject);
        _extensibleProjectLocator.Enable(_defaultProject);
    }

    public IExtensibleProject GetDefaultProject()
    {
        return _defaultProject;
    }
}
