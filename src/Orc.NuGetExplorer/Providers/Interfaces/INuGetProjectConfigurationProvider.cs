namespace Orc.NuGetExplorer
{
    using NuGet.ProjectManagement;

    public interface INuGetProjectConfigurationProvider
    {
        NuGetProject GetProjectConfig(IExtensibleProject project);
    }
}
