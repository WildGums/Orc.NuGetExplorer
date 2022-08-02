namespace Orc.NuGetExplorer
{
    using NuGet.Frameworks;
    using NuGet.ProjectManagement;

    public interface INuGetProjectConfigurationProvider
    {
        NuGetProject GetPackagesConfig(string packagesConfigPath, NuGetFramework targetFramework, string projectName);
        NuGetProject GetProjectConfig(IExtensibleProject project);
    }
}
