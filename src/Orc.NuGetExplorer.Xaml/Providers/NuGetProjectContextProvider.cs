namespace Orc.NuGetExplorer.Providers;

using System;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;
using NuGet.ProjectManagement;
using Orc.NuGetExplorer.Management;


public class NuGetProjectContextProvider : INuGetProjectContextProvider
{
    private readonly IServiceProvider _serviceProvider;

    public NuGetProjectContextProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INuGetProjectContext GetProjectContext(FileConflictAction fileConflictAction)
    {
        var projectContext = ActivatorUtilities.CreateInstance<NuGetProjectContext>(_serviceProvider, fileConflictAction);

        return projectContext;
    }
}
