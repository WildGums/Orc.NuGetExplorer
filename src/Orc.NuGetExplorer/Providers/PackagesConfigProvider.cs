namespace Orc.NuGetExplorer.Providers
{
    using System.Collections.Generic;
    using Catel;
    using NuGet.Frameworks;
    using NuGet.ProjectManagement;
    using Orc.NuGetExplorer.Management;

    internal class PackagesConfigProvider : INuGetProjectConfigurationProvider
    {
        private readonly Dictionary<IExtensibleProject, NuGetProjectMetadata> _storedProjectMetadata = new Dictionary<IExtensibleProject, NuGetProjectMetadata>();
        private readonly IFrameworkNameProvider _frameworkNameProvider;

        private const string MetadataTargetFramework = "TargetFramework";
        private const string MetadataName = "Name";

        public PackagesConfigProvider(IFrameworkNameProvider frameworkNameProvider)
        {
            Argument.IsNotNull(() => frameworkNameProvider);

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
            NuGetProjectMetadata metadata = null;

            if (!_storedProjectMetadata.TryGetValue(project, out metadata))
            {
                metadata = new NuGetProjectMetadata();

                var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);

                metadata.Data.Add(MetadataTargetFramework, targetFramework);
                metadata.Data.Add(MetadataName, project.Name);

                _storedProjectMetadata.Add(project, metadata);
            }

            var packagesConfigProject = new PackagesConfigNuGetProject(project.ContentPath, metadata.Data);

            return packagesConfigProject;
        }
    }
}
