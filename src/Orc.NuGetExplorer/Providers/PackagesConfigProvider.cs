namespace Orc.NuGetExplorer.Providers
{
    using System.Collections.Generic;
    using NuGet.Frameworks;
    using NuGet.ProjectManagement;
    using Orc.NuGetExplorer.Management;

    internal class PackagesConfigProvider : INuGetProjectConfigurationProvider
    {
        private readonly Dictionary<IExtensibleProject, NuGetProjectMetadata> _storedProjectMetadata = new();
        private readonly IFrameworkNameProvider _frameworkNameProvider;

        private const string MetadataTargetFramework = "TargetFramework";
        private const string MetadataName = "Name";

        public PackagesConfigProvider(IFrameworkNameProvider frameworkNameProvider)
        {
            _frameworkNameProvider = frameworkNameProvider;
        }

        /// <summary>
        /// Creates minimal required metadata for initializing NuGet PackagesConfigNuGetProject from
        /// our IExtensibleProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public NuGetProject GetProjectConfig(IExtensibleProject project)
        {
            if (!_storedProjectMetadata.TryGetValue(project, out var metadata))
            {
                var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);

                metadata = BuildMetadataForConfig(targetFramework, project.Name);

                _storedProjectMetadata.Add(project, metadata);
            }

            var packagesConfigProject = new PackagesConfigNuGetProject(project.ContentPath, metadata.Data);

            return packagesConfigProject;
        }

        public NuGetProject GetPackagesConfig(string packagesConfigPath, NuGetFramework targetFramework, string projectName)
        {
            var metadata = BuildMetadataForConfig(targetFramework, projectName);
            var packagesConfigProject = new PackagesConfigNuGetProject(packagesConfigPath, metadata.Data);

            return packagesConfigProject;
        }

        private static NuGetProjectMetadata BuildMetadataForConfig(NuGetFramework targetFramework, string projectName)
        {
            var metadata = new NuGetProjectMetadata();

            metadata.Data.Add(MetadataTargetFramework, targetFramework);
            metadata.Data.Add(MetadataName, projectName);

            return metadata;
        }
    }
}
