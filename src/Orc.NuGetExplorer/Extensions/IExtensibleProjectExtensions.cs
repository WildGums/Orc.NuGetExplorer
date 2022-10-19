namespace Orc.NuGetExplorer
{
    using System;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public static class IExtensibleProjectExtensions
    {
        public static SourceRepository AsSourceRepository(this IExtensibleProject project, ISourceRepositoryProvider repositoryProvider)
        {
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(repositoryProvider);

            return repositoryProvider.CreateRepository(new PackageSource(project.ContentPath), NuGet.Protocol.FeedType.FileSystemV2);
        }
    }
}
