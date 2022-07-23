namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public static class ISourceRepositoryProviderExtensions
    {
        public static SourceRepository TryGetRepository(this ISourceRepositoryProvider sourceRepositoryProvider, PackageSource source)
        {
            if (source is null)
            {
                return null;
            }

            var sourceRepo = sourceRepositoryProvider.CreateRepository(source);
            return sourceRepo;
        }
    }
}
