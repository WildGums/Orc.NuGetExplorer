namespace Orc.NuGetExplorer.Packaging
{
    using System.Linq;
    using Catel;
    using NuGet.Packaging;
    using NuGet.Protocol.Core.Types;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    public class UpdatePackageSearchMetadataBuilder
    {
        private readonly ClonedPackageSearchMetadata _metadata;

        private readonly IPackageSearchMetadata _updatedVersionMetadata;

        private UpdatePackageSearchMetadataBuilder(ClonedPackageSearchMetadata metadata, IPackageSearchMetadata updatedVersionMetadata)
        {
            Argument.IsNotNull(() => metadata);
            Argument.IsNotNull(() => updatedVersionMetadata);

            _metadata = metadata;
            _updatedVersionMetadata = updatedVersionMetadata;
        }

        public static UpdatePackageSearchMetadataBuilder FromMetadatas(ClonedPackageSearchMetadata metadata, IPackageSearchMetadata updatedVersionMetadata)
             => new UpdatePackageSearchMetadataBuilder(metadata, updatedVersionMetadata);

        public IPackageSearchMetadata Build()
        {
            var clonedMetadata = new UpdatePackageSearchMetadata
            {
                Authors = _metadata.Authors,
                DependencySets = _metadata.DependencySets ?? Enumerable.Empty<PackageDependencyGroup>(),
                Description = _metadata.Description,
                DownloadCount = _metadata.DownloadCount,
                IconUrl = _metadata.IconUrl,
                Identity = _metadata.Identity,
                LicenseUrl = _metadata.LicenseUrl,
                Owners = _metadata.Owners,
                ProjectUrl = _metadata.ProjectUrl,
                Published = _metadata.Published,
                ReportAbuseUrl = _metadata.ReportAbuseUrl,
                RequireLicenseAcceptance = _metadata.RequireLicenseAcceptance,
                Summary = _metadata.Summary,
                Tags = _metadata.Tags,
                Title = _metadata.Title,
                LazyVersionsFactory = _metadata.LazyVersionsFactory,
                IsListed = _metadata.IsListed,
                PrefixReserved = _metadata.PrefixReserved,
                LicenseMetadata = _metadata.LicenseMetadata,
                FromVersion = new VersionInfo(_updatedVersionMetadata.Identity.Version, _updatedVersionMetadata.DownloadCount)
                {
                    PackageSearchMetadata = _updatedVersionMetadata
                }
            };

            return clonedMetadata;
        }
    }
}
