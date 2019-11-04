namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public static class IExtensibleProjectExtensions
    {
        public static SourceRepository AsSourceRepository(this IExtensibleProject project, ISourceRepositoryProvider repositoryProvider)
        {
            return repositoryProvider.CreateRepository(new PackageSource(project.ContentPath), NuGet.Protocol.FeedType.FileSystemV2);
        }
    }
}
